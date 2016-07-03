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
            foreach (VariableDeclaratorSyntax variable in node.Variables)
            {
                string typeName = m_context.ConvertTypeName(node.Type);
                
                m_context.Writer.AppendLine(string.Format("/* Variable {0} */", variable.ToString()));
                m_context.Writer.Append(string.Format("{0} {1} = ", typeName, variable.Identifier));

                ExpressionSyntax expression = variable.Initializer.Value;
                m_context.Generators.Expression.Generate(expression);
            }
        }
    }
}
