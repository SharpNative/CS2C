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

            ISymbol symbol = m_context.Model.GetSymbolInfo(node.ChildNodes().First()).Symbol;
            bool isProperty = (symbol != null && symbol.Kind == SymbolKind.Property);

            if (isProperty)
            {
                if (symbol.IsStatic)
                    m_context.Writer.Append(string.Format("{0}_{1}_setter(NULL, ", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name));
                else
                    m_context.Writer.Append(string.Format("{0}_{1}_setter(obj, ", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name));
            }

            bool first = true;
            ChildSyntaxList nodes = node.ChildNodesAndTokens();
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
                        string converted = m_context.ConvertVariableName(childNode);

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
                else if (m_context.IsSubExpression(kind))
                {
                    m_context.Generators.Expression.Generate(child.AsNode());
                }
                else
                {
                    m_context.Writer.Append(child.ToString());
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
