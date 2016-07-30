using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibCS2C.Generators
{
    public class CastExpressionGenerator : GeneratorBase<CastExpressionSyntax>
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
            m_context.Generators.Expression.Generate(node.Expression);
        }
    }
}
