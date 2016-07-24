using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class DoStatementGenerator : GeneratorBase<DoStatementSyntax>
    {
        /// <summary>
        /// Do statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public DoStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }
        
        /// <summary>
        /// Generates a do statement
        /// </summary>
        /// <param name="node">The do statement</param>
        public override void Generate(DoStatementSyntax node)
        {
            m_context.Writer.AppendLine("do");
            m_context.Generators.Block.GenerateChild(node.Statement);
            m_context.Writer.Append("while(");
            m_context.Generators.Expression.Generate(node.Condition);
            m_context.Writer.Append(")");
        }
    }
}
