using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace LibCS2C.Generators
{
    public class SimpleAssignmentGenerator : GeneratorBase<ExpressionSyntax>
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
        public override void Generate(ExpressionSyntax node)
        {
            // The first node will be an identifier
            // Check its type, if it's a property, that means we need to use the setter
            ChildSyntaxList nodes = node.ChildNodesAndTokens();
            ISymbol symbol = m_context.Model.GetSymbolInfo(node.ChildNodes().First()).Symbol;
            bool isProperty = (symbol != null && symbol.Kind == SymbolKind.Property);
            
            string prefix = "";
            if (isProperty)
            {
                // If the first node is a memberaccess, we need to get the object name from that node
                string objName = "obj";
                SyntaxNode firstNode = nodes[0].AsNode();
                
                if (firstNode is MemberAccessExpressionSyntax)
                {
                    SyntaxNode firstChild = firstNode.ChildNodes().First();
                    SyntaxKind firstChildKind = firstChild.Kind();

                    // Check if the argument needs to be passed as a reference
                    ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(firstChild).Type;
                    if (!m_context.GenericTypeConvert.IsGeneric(typeSymbol.Name) && typeSymbol.TypeKind == TypeKind.Struct)
                    {
                        prefix = "&";
                    }

                    if (firstChildKind == SyntaxKind.IdentifierName)
                    {
                        objName = m_context.TypeConvert.ConvertVariableName(firstChild);
                    }
                    else
                    {
                        WriterDestination destination = m_context.Writer.CurrentDestination;
                        m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                        m_context.Generators.Expression.Generate(firstChild as ExpressionSyntax);
                        m_context.Writer.CurrentDestination = destination;
                        objName = m_context.Writer.FlushTempBuffer();
                    }
                }
                
                m_context.Writer.Append(string.Format("{0}_{1}_setter({2}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, prefix));
                if (!symbol.IsStatic)
                    m_context.Writer.Append(string.Format("{0}, ", objName));
            }
            
            bool first = true;
            foreach (SyntaxNodeOrToken child in nodes)
            {
                SyntaxKind kind = child.Kind();
                
                if (kind == SyntaxKind.IdentifierName)
                {
                    // Skip the first identifier if it's a property
                    // because we already emitted the code for the setter
                    if (!isProperty || !first)
                    {
                        SyntaxNode childNode = child.AsNode();
                        ISymbol identifierSymbol = m_context.Model.GetSymbolInfo(childNode).Symbol;
                        string converted = m_context.TypeConvert.ConvertVariableName(childNode);

                        if (identifierSymbol.Kind == SymbolKind.Field && !identifierSymbol.IsStatic)
                            m_context.Writer.Append("obj->" + converted);
                        else
                            m_context.Writer.Append(converted);
                    }
                }
                else if (kind == SyntaxKind.EqualsToken)
                {
                    if (!isProperty)
                        m_context.Writer.Append(" = ");
                }
                else if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    // Ignore if property because we would get an getter here
                    if (!first || !isProperty)
                        m_context.Generators.Expression.Generate(child.AsNode());
                }
                else
                {
                    m_context.Generators.Expression.Generate(child.AsNode());
                }

                first = false;
            }

            if (isProperty)
            {
                m_context.Writer.Append(")");
            }
        }
    }
}
