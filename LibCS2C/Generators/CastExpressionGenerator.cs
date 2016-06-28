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
    class CastExpressionGenerator : GeneratorBase<CastExpressionSyntax>
    {
        /// <summary>
        /// Cast expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public CastExpressionGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a cast
        /// </summary>
        /// <param name="node">The cast</param>
        public override void Generate(CastExpressionSyntax node)
        {
            m_context.Writer.Append(string.Format("({0})", m_context.ConvertTypeName(node.Type)));

            ExpressionGenerator expressionGen = new ExpressionGenerator(m_context);
            expressionGen.Generate(node.Expression);
        }
    }
}
