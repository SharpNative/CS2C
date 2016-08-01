using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class ForStatementGenerator : GeneratorBase<ForStatementSyntax>
    {
        /// <summary>
        /// For statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ForStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a for statement
        /// </summary>
        /// <param name="node">The for statement</param>
        public override void Generate(ForStatementSyntax node)
        {
            m_context.Writer.Append("for(");

            // Declaration
            if (node.Declaration != null)
                m_context.Generators.Variable.Generate(node.Declaration);
            
            m_context.Writer.Append(";");

            // Condition
            if (node.Condition != null)
                m_context.Generators.Expression.Generate(node.Condition);

            m_context.Writer.Append(";");

            // Incrementors
            SeparatedSyntaxList<ExpressionSyntax> nodes = node.Incrementors;
            foreach (ExpressionSyntax expression in nodes)
            {
                m_context.Generators.Expression.Generate(expression);
            }

            m_context.Writer.AppendLine(")");

            // Code inside the loop
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
