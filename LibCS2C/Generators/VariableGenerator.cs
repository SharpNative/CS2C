using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class VariableGenerator : GeneratorBase<VariableDeclarationSyntax>
    {
        /// <summary>
        /// Variable declaration generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public VariableGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generate code for variable declaration
        /// </summary>
        /// <param name="node"></param>
        public override void Generate(VariableDeclarationSyntax node)
        {
            m_context.Writer.Append(m_context.ConvertTypeName(node.Type));
            m_context.Writer.Append(" ");

            int varCount = node.Variables.Count;
            int i = 0;
            foreach (VariableDeclaratorSyntax variable in node.Variables)
            {
                m_context.Writer.Append(variable.Identifier.ToString());
                
                // Initial value
                if (variable.Initializer != null)
                {
                    m_context.Writer.Append(" = ");
                    ExpressionSyntax expression = variable.Initializer.Value;
                    m_context.Generators.Expression.Generate(expression);
                }

                // There can be more than one variable
                if (i != varCount - 1)
                    m_context.Writer.Append(", ");

                i++;
            }
        }
    }
}
