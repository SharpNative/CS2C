using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class WhileStatementGenerator : GeneratorBase<WhileStatementSyntax>
    {
        /// <summary>
        /// While statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public WhileStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }
        
        /// <summary>
        /// Generates a while statement
        /// </summary>
        /// <param name="node">The while statement</param>
        public override void Generate(WhileStatementSyntax node)
        {
            m_context.Writer.Append("while(");
            m_context.Generators.Expression.Generate(node.Condition);
            m_context.Writer.AppendLine(")");

            // Code inside the loop
            if (node.Statement != null)
                m_context.Generators.Block.GenerateChild(node.Statement);
            else
                m_context.Writer.AppendLine(";");
        }
    }
}
