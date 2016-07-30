using LibCS2C.Context;
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
    public class ConditionalAccessExpressionGenerator : GeneratorBase<ConditionalAccessExpressionSyntax>
    {
        /// <summary>
        /// Conditional access expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ConditionalAccessExpressionGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a conditional access expression generator
        /// </summary>
        /// <param name="node">The access expression</param>
        public override void Generate(ConditionalAccessExpressionSyntax node)
        {
            m_context.Writer.Append("if((");
            m_context.Generators.Expression.Generate(node.Expression);
            m_context.Writer.Append(") != null)");

            m_context.Writer.AppendLine("{");
            m_context.Writer.Indent();

            m_context.Generators.Invocation.Generate(node.WhenNotNull as InvocationExpressionSyntax);
            m_context.Writer.AppendLine(";");

            m_context.Writer.UnIndent();
            m_context.Writer.AppendLine("}");
        }
    }
}
