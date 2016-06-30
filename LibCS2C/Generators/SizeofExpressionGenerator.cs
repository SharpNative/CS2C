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
    class SizeofExpressionGenerator : GeneratorBase<SizeOfExpressionSyntax>
    {
        /// <summary>
        /// Sizeof statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public SizeofExpressionGenerator(WalkerContext context)
        {
            m_context = context;
        }
        
        /// <summary>
        /// Generates a sizeof expression
        /// </summary>
        /// <param name="node">The sizeof expression</param>
        public override void Generate(SizeOfExpressionSyntax node)
        {
            m_context.Writer.Append(string.Format("sizeof({0})", m_context.ConvertTypeName(node.Type)));
        }
    }
}
