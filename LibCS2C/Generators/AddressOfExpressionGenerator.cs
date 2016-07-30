using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class AddressOfExpressionGenerator : GeneratorBase<PrefixUnaryExpressionSyntax>
    {
        /// <summary>
        /// Address of expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public AddressOfExpressionGenerator(WalkerContext context)
        {
            m_context = context;
        }
        
        /// <summary>
        /// Generates an address of expression
        /// </summary>
        /// <param name="node">The address of expression</param>
        public override void Generate(PrefixUnaryExpressionSyntax node)
        {
            m_context.Writer.Append("&");
            m_context.Generators.Expression.Generate(node.Operand);
        }
    }
}
