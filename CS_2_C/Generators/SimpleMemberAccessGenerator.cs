using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
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
            SyntaxNode[] nodes = node.ChildNodes().ToArray();
            for(int i = 0, l = nodes.Length; i < l; i++)
            {
                SyntaxNode childNode = nodes[i];
                SyntaxKind childKind = childNode.Kind();

                if (childKind == SyntaxKind.ThisExpression)
                {
                    // If this is the start of the expression and an identifier follows
                    // that means an "obj->" was not emitted already
                    if(!(i == 0 && i + 1 < l && nodes[i + 1].Kind() == SyntaxKind.IdentifierName))
                        m_context.Writer.Append("obj->");
                }
                else if (childKind == SyntaxKind.IdentifierName)
                {
                    IdentifierNameSyntax name = childNode as IdentifierNameSyntax;
                    m_context.Writer.Append(m_context.ConvertVariableName(name));
                }
                else
                {
                    m_context.Writer.Append(childNode.ToString());
                }
            }
        }
    }
}
