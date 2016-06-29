using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace LibCS2C.Generators
{
    class SimpleAssignmentGenerator : GeneratorBase<ExpressionSyntax>
    {
        private SimpleMemberAccessGenerator m_simpleMemberAccessGen;
        private ElementAccessGenerator m_elementAccessGen;
        private CastExpressionGenerator m_castExpressionGen;

        /// <summary>
        /// Simple assignment generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public SimpleAssignmentGenerator(WalkerContext context)
        {
            m_context = context;
            m_simpleMemberAccessGen = new SimpleMemberAccessGenerator(m_context);
            m_elementAccessGen = new ElementAccessGenerator(m_context);
            m_castExpressionGen = new CastExpressionGenerator(m_context);
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
                        m_context.Writer.Append(m_context.ConvertVariableName(child.AsNode()));
                    }
                }
                else if (kind == SyntaxKind.ElementAccessExpression)
                {
                    m_elementAccessGen.Generate(child.AsNode() as ElementAccessExpressionSyntax);
                }
                else if (kind == SyntaxKind.EqualsToken)
                {
                    if (!isProperty)
                        m_context.Writer.Append(" = ");
                }
                else if (kind == SyntaxKind.CastExpression)
                {
                    m_castExpressionGen.Generate(child.AsNode() as CastExpressionSyntax);
                }
                else if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    m_simpleMemberAccessGen.Generate(child.AsNode() as ExpressionSyntax);
                }
                else if (m_context.IsSubExpression(kind))
                {
                    ExpressionGenerator expressionGen = new ExpressionGenerator(m_context);
                    expressionGen.Generate(child.AsNode() as ExpressionSyntax);
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

            m_context.Writer.AppendLine(";");
        }
    }
}
