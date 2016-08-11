using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class LabeledStatementGenerator : GeneratorBase<LabeledStatementSyntax>
    {
        /// <summary>
        /// Labeled statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public LabeledStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a labeled statement
        /// </summary>
        /// <param name="node">The labeled statement</param>
        public override void Generate(LabeledStatementSyntax node)
        {
            m_context.Writer.AppendLine(string.Format("{0}:", node.Identifier));

            // Code inside the label
            if (node.Statement != null)
            {
                m_context.Writer.AppendLine("{");
                m_context.Writer.Indent();
                m_context.Generators.Block.GenerateChild(node.Statement);
                m_context.Writer.UnIndent();
                m_context.Writer.AppendLine("}");
            }
            else
            {
                m_context.Writer.AppendLine(";");
            }
        }
    }
}
