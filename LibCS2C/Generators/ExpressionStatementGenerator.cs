using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    class ExpressionStatementGenerator : GeneratorBase<ExpressionStatementSyntax>
    {
        private ExpressionGenerator expressionGen;

        /// <summary>
        /// Expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ExpressionStatementGenerator(WalkerContext context)
        {
            m_context = context;
            expressionGen = new ExpressionGenerator(context);
        }

        /// <summary>
        /// Generates the expression code
        /// </summary>
        /// <param name="node">The expression statement node</param>
        public override void Generate(ExpressionStatementSyntax node)
        {
            expressionGen.Generate(node.Expression);
        }
    }
}
