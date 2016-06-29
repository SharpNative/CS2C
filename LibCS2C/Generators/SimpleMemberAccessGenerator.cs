using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    class SimpleMemberAccessGenerator : GeneratorBase<ExpressionSyntax>
    {
        /// <summary>
        /// Member access generator generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="type">The method type: constructor or method</param>
        public SimpleMemberAccessGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a member access
        /// </summary>
        /// <param name="node">The expression of the member access</param>
        public override void Generate(ExpressionSyntax node)
        {
            SyntaxNodeOrToken[] children = node.ChildNodesAndTokens().ToArray();

            bool hasThis = true;
            bool first = true;

            for (int i = 0, l = children.Length; i < l; i++)
            {
                SyntaxNodeOrToken child = children[i];
                SyntaxKind kind = child.Kind();

                if (kind == SyntaxKind.ThisExpression)
                {
                    hasThis = true;
                }
                else if (kind == SyntaxKind.IdentifierName)
                {
                    IdentifierNameSyntax name = child.AsNode() as IdentifierNameSyntax;

                    if (hasThis && !first)
                        m_context.Writer.Append(string.Format("field_{0}", name.Identifier));
                    else
                        m_context.Writer.Append(m_context.ConvertVariableName(name));

                    first = false;
                }
                else if (kind == SyntaxKind.DotToken)
                {
                    if (!hasThis || !first)
                        m_context.Writer.Append("->");
                }
                else
                {
                    m_context.Writer.Append(child.ToString());
                }
            }
        }
    }
}
