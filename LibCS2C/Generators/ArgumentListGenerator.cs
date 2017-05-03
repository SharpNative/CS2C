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
        /// Generates an argument
        /// </summary>
        /// <param name="arg">The argument</param>
        public void GenerateArgument(ArgumentSyntax arg)
        {
            IEnumerable<SyntaxNode> children = arg.ChildNodes();
            foreach (ExpressionSyntax child in children)
            {
                ITypeSymbol type = m_context.Model.GetTypeInfo(child).Type;

                if (type != null && !m_context.GenericTypeConvert.IsGeneric(type) && type.TypeKind == TypeKind.Class)
                    m_context.Writer.Append("(void*)");

                m_context.Generators.Expression.Generate(child);
            }
        }

        /// <summary>
        /// Generates the argument list code
        /// </summary>
        /// <param name="node">The argument list</param>
        public override void Generate(ArgumentListSyntax node)
        {
            IEnumerable<SyntaxNode> argNodes = node.ChildNodes();
            foreach (ArgumentSyntax argument in argNodes)
            {
                GenerateArgument(argument);

                // A comma if it's not the last argument
                if (argument != argNodes.Last())
                    m_context.Writer.Append(", ");
            }
        }
    }
}
