using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class SizeofExpressionGenerator : GeneratorBase<SizeOfExpressionSyntax>
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
