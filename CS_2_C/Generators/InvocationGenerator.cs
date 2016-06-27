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
    class InvocationGenerator : GeneratorBase<ExpressionSyntax>
    {
        /// <summary>
        /// Invocation generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public InvocationGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates an invocation
        /// </summary>
        /// <param name="node">The expression</param>
        public override void Generate(ExpressionSyntax node)
        {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();

            // IdentifierNameSyntax -> own class
            // ... -> other class
            SyntaxNode first = nodes.First();
            SyntaxKind firstKind = first.Kind();

            string memberName;
            // Own class
            if (firstKind == SyntaxKind.IdentifierName)
            {
                IdentifierNameSyntax name = nodes.First() as IdentifierNameSyntax;
                memberName = m_context.CurrentClassNameFormatted + "_" + name.Identifier;
            }
            // Another class
            else if (firstKind == SyntaxKind.SimpleMemberAccessExpression)
            {
                MemberAccessExpressionSyntax name = first as MemberAccessExpressionSyntax;
                memberName = name.ToFullString().Trim().Replace(".", "_");
            }
            else
            {
                throw new NotSupportedException();
            }

            m_context.Writer.AppendIndent();
            m_context.Writer.Append(string.Format("{0}(", memberName));

            // Arguments
            ArgumentListGenerator argGen = new ArgumentListGenerator(m_context);
            foreach (SyntaxNode childNode in nodes)
            {
                if (childNode.Kind() == SyntaxKind.ArgumentList)
                {
                    argGen.Generate(childNode as ArgumentListSyntax);
                }
            }

            m_context.Writer.AppendLine(");");
        }
    }
}
