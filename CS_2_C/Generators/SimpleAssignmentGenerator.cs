using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
{
    class SimpleAssignmentGenerator : GeneratorBase<ExpressionStatementSyntax>
    {
        /// <summary>
        /// Simple assignment generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public SimpleAssignmentGenerator(WalkerContext context)
        {
            m_context = context;
        }

        public override void Generate(ExpressionStatementSyntax node)
        {
            string code = node.GetText().ToString().Trim();
            m_context.Writer.AppendIndent();
            m_context.Writer.AppendLine(string.Format("/* Expression {0} */", code));
            m_context.Writer.AppendIndent();
            m_context.Writer.AppendLine(code);
        }
    }
}
