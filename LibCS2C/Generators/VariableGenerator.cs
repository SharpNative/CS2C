using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            m_context.Writer.AppendLine(string.Format("/* Variable {0} */", node));
            m_context.Writer.Append(m_context.ConvertTypeName(node.Type));
            m_context.Writer.Append(" ");

            int varCount = node.Variables.Count;
            int i = 0;
            foreach (VariableDeclaratorSyntax variable in node.Variables)
            {
                m_context.Writer.Append(variable.Identifier.ToString());
                
                if (variable.Initializer != null)
                {
                    m_context.Writer.Append(" = ");
                    ExpressionSyntax expression = variable.Initializer.Value;
                    m_context.Generators.Expression.Generate(expression);
                }

                if (i != varCount - 1)
                    m_context.Writer.Append(", ");

                i++;
            }
        }
    }
}
