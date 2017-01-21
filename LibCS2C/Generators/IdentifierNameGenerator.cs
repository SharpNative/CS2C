using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibCS2C.Generators
{
    public class IdentifierNameGenerator : GeneratorBase<IdentifierNameSyntax>
    {
        /// <summary>
        /// Identifier name generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public IdentifierNameGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generate code for identifier name
        /// </summary>
        /// <param name="node"></param>
        public override void Generate(IdentifierNameSyntax node)
        {
            ISymbol symbol = m_context.Model.GetSymbolInfo(node).Symbol;
            if (symbol == null)
                throw new Exception("Could not get the symbol info of: " + node);

            if (symbol.Kind == SymbolKind.Field && !symbol.IsStatic)
                m_context.Writer.Append("obj->");

            m_context.Writer.Append(m_context.TypeConvert.ConvertVariableName(node));
        }
    }
}
