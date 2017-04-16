using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibCS2C.Generators
{
    public class ArgumentListGenerator : GeneratorBase<ArgumentListSyntax>
    {
        /// <summary>
        /// Argument list generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ArgumentListGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates the argument list code
        /// </summary>
        /// <param name="node">The argument list</param>
        public override void Generate(ArgumentListSyntax node)
        {
            Console.WriteLine(node.Parent.Kind());

            IEnumerable<SyntaxNode> argNodes = node.ChildNodes();
            foreach (ArgumentSyntax argument in argNodes)
            {
                IEnumerable<SyntaxNode> children = argument.ChildNodes();
                foreach (ExpressionSyntax child in children)
                {
                    ITypeSymbol type = m_context.Model.GetTypeInfo(child).Type;

                    if (type != null && type.TypeKind == TypeKind.Class)
                        m_context.Writer.Append("(void*)");

                    m_context.Generators.Expression.Generate(child);
                }

                // A comma if it's not the last argument
                if (argument != argNodes.Last())
                    m_context.Writer.Append(", ");
            }
        }
    }
}
