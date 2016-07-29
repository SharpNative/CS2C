using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibCS2C.Generators
{
    public class ExpressionStatementGenerator : GeneratorBase<ExpressionStatementSyntax>
    {
        /// <summary>
        /// Expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ExpressionStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates the expression code
        /// </summary>
        /// <param name="node">The expression statement node</param>
        public override void Generate(ExpressionStatementSyntax node)
        {
            m_context.Generators.Expression.Generate(node.Expression);
        }
    }
}
