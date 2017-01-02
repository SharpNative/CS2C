using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class checkedStatementGenerator : GeneratorBase<CheckedStatementSyntax>
    {
        /// <summary>
        /// Checked statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public checkedStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }
        
        /// <summary>
        /// Generates an checked statement
        /// </summary>
        /// <param name="node">The statement</param>
        public override void Generate(CheckedStatementSyntax node)
        {
            m_context.Generators.Block.Generate(node.Block);
        }
    }
}
