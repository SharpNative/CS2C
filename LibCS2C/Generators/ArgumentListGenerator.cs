﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    class ArgumentListGenerator : GeneratorBase<ArgumentListSyntax>
    {
        private ExpressionGenerator m_expressionGen;

        /// <summary>
        /// Argument list generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ArgumentListGenerator(WalkerContext context)
        {
            m_context = context;
            m_expressionGen = new ExpressionGenerator(m_context);
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
                IEnumerable<SyntaxNode> children = argument.ChildNodes();
                foreach(ExpressionSyntax child in children)
                {
                    m_expressionGen.Generate(child);
                }

                // A comma if it's not the last argument
                if (argument != argNodes.Last())
                    m_context.Writer.Append(", ");
            }
        }
    }
}
