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
    class AddressOfExpressionGenerator : GeneratorBase<PrefixUnaryExpressionSyntax>
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
            ExpressionGenerator expressionGen = new ExpressionGenerator(m_context);
            expressionGen.Generate(node.Operand);
        }
    }
}
