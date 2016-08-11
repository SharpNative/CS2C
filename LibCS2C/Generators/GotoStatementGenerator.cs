using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibCS2C.Generators
{
    public class GotoStatementGenerator : GeneratorBase<GotoStatementSyntax>
    {
        /// <summary>
        /// Goto statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public GotoStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a goto statement
        /// </summary>
        /// <param name="node">The goto statement</param>
        public override void Generate(GotoStatementSyntax node)
        {
            m_context.Writer.Append(string.Format("goto {0}", node.Expression));
        }
    }
}
