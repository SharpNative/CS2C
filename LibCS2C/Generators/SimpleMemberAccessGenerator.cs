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
            bool first = true;
            SyntaxNodeOrToken[] nodes = node.ChildNodesAndTokens().ToArray();
            for (int i = 0, l = nodes.Length; i < l; i++)
            {
                SyntaxNodeOrToken child = nodes[i];
                SyntaxKind kind = child.Kind();

                if (kind == SyntaxKind.ThisExpression)
                {
                    // If this is the start of the expression and an identifier follows
                    // that means an "obj->" was not emitted already
                    if (!(i == 0 && i + 1 < l && nodes[i + 1].Kind() == SyntaxKind.IdentifierName))
                        m_context.Writer.Append("obj->");
                }
                else if (kind == SyntaxKind.IdentifierName)
                {
                    if (first)
                        m_context.Writer.Append(m_context.ConvertVariableName(child.AsNode() as IdentifierNameSyntax));
                    else
                        m_context.Writer.Append((child.AsNode() as IdentifierNameSyntax).Identifier.ToString());
                }
                else if (kind == SyntaxKind.DotToken)
                {
                    m_context.Writer.Append("->");
                }
                else
                {
                    m_context.Writer.Append(child.ToString());
                }

                first = false;
            }
        }
    }
}
