using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace LibCS2C.Generators
{
    public class SimpleAssignmentGenerator : GeneratorBase<AssignmentExpressionSyntax>
    {
        /// <summary>
        /// Simple assignment generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public SimpleAssignmentGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a simple assignment
        /// </summary>
        /// <param name="node">The expression node</param>
        public override void Generate(AssignmentExpressionSyntax node)
        {
            // The first node will be an identifier
            // Check its type, if it's a property, that means we need to use the setter
            ISymbol symbol = m_context.Model.GetSymbolInfo(node.ChildNodes().First()).Symbol;
            bool isProperty = (symbol != null && symbol.Kind == SymbolKind.Property);

            if (isProperty)
            {
                // If the first node is a memberaccess, we need to get the object name from that node
                string objName = "obj";
                string prefix = "";

                ChildSyntaxList expression = (node as ExpressionSyntax).ChildNodesAndTokens();
                SyntaxNode firstNode = expression[0].AsNode();

                if (firstNode is MemberAccessExpressionSyntax)
                {
                    SyntaxNode firstChild = firstNode.ChildNodes().First();
                    SyntaxKind firstChildKind = firstChild.Kind();

                    // Check if the argument needs to be passed as a reference
                    ITypeSymbol childTypeSymbol = m_context.Model.GetTypeInfo(firstChild).Type;
                    if (!m_context.GenericTypeConvert.IsGeneric(childTypeSymbol) && childTypeSymbol.TypeKind == TypeKind.Struct)
                    {
                        prefix = "&";
                    }
                    
                    WriterDestination destination = m_context.Writer.CurrentDestination;
                    m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                    m_context.Generators.Expression.Generate(firstChild as ExpressionSyntax);
                    m_context.Writer.CurrentDestination = destination;
                    objName = m_context.Writer.FlushTempBuffer();
                }

                m_context.Writer.Append(string.Format("{0}_{1}_setter({2}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, prefix));
                if (!symbol.IsStatic)
                    m_context.Writer.Append(string.Format("{0},", objName));
            }
            else
            {
                m_context.Generators.Expression.Generate(node.Left);
                m_context.Writer.Append("=");
            }

            // If the type on the right is an object, cast it
            ITypeSymbol leftSymbol = m_context.Model.GetTypeInfo(node.Left).Type;
            ITypeSymbol rightSymbol = m_context.Model.GetTypeInfo(node.Right).Type;
            if (leftSymbol != null && rightSymbol != null && !leftSymbol.Name.Equals(rightSymbol.Name) && rightSymbol.TypeKind == TypeKind.Class && !m_context.GenericTypeConvert.IsGeneric(rightSymbol))
                m_context.Writer.Append("(void*)");

            m_context.Generators.Expression.Generate(node.Right);

            if (isProperty)
                m_context.Writer.Append(")");
        }
    }
}
