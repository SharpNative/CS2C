using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace CS_2_C.Generators
{
    class SimpleAssignmentGenerator : GeneratorBase<ExpressionSyntax>
    {
        private SimpleMemberAccessGenerator m_simpleMemberAccessGen;

        /// <summary>
        /// Simple assignment generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public SimpleAssignmentGenerator(WalkerContext context)
        {
            m_context = context;
            m_simpleMemberAccessGen = new SimpleMemberAccessGenerator(m_context);
        }

        /// <summary>
        /// Generates a simple assignment
        /// </summary>
        /// <param name="node">The expression node</param>
        public override void Generate(ExpressionSyntax node)
        {
            string code = node.GetText().ToString().Trim();
            
            m_context.Writer.AppendLine(string.Format("/* Expression {0} */", code));

            ChildSyntaxList nodes = node.ChildNodesAndTokens();
            foreach (SyntaxNodeOrToken child in nodes)
            {
                SyntaxKind kind = child.Kind();

                if (kind == SyntaxKind.IdentifierName)
                {
                    m_context.Writer.Append(m_context.ConvertVariableName(child.AsNode()));
                }
                else if (kind == SyntaxKind.EqualsToken)
                {
                    m_context.Writer.Append(" = ");
                }
                else if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    m_simpleMemberAccessGen.Generate(child.AsNode() as ExpressionSyntax);
                }
                else if(kind == SyntaxKind.AddExpression ||
                        kind == SyntaxKind.SubtractExpression ||
                        kind == SyntaxKind.MultiplyExpression ||
                        kind == SyntaxKind.DivideExpression)
                {
                    ExpressionGenerator expressionGen = new ExpressionGenerator(m_context);
                    expressionGen.Generate(child.AsNode() as ExpressionSyntax);
                }
                else
                {
                    m_context.Writer.Append(child.ToString());
                }
            }
            
            m_context.Writer.AppendLine(";");
        }
    }
}
