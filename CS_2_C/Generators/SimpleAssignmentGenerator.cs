using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CS_2_C.Generators
{
    class SimpleAssignmentGenerator : GeneratorBase<ExpressionStatementSyntax>
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
        public override void Generate(ExpressionStatementSyntax node)
        {
            string code = node.GetText().ToString().Trim();
            m_context.Writer.AppendIndent();
            m_context.Writer.AppendLine(string.Format("/* Expression {0} */", code));

            m_context.Writer.AppendIndent();

            ChildSyntaxList nodes = node.ChildNodes().First().ChildNodesAndTokens();
            foreach (SyntaxNodeOrToken childNode in nodes)
            {
                SyntaxKind kind = childNode.Kind();
                
                if (kind == SyntaxKind.IdentifierName)
                {
                    m_context.Writer.Append(m_context.ConvertVariableName(childNode.AsNode()));
                }
                else if(kind == SyntaxKind.EqualsToken)
                {
                    m_context.Writer.Append(" = ");
                }
                else
                {
                    m_context.Writer.Append(childNode.ToString());
                }
            }
            
            m_context.Writer.AppendLine(";");
        }
    }
}
