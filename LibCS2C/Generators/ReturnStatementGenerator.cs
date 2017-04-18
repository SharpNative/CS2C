using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class ReturnStatementGenerator : GeneratorBase<ReturnStatementSyntax>
    {
        /// <summary>
        /// Return statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ReturnStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates the return statement code
        /// </summary>
        /// <param name="node">The return statement</param>
        public override void Generate(ReturnStatementSyntax node)
        {
            m_context.Writer.Append("return ");

            if (node.Expression != null)
            {
                ITypeSymbol type = m_context.Model.GetTypeInfo(node.Expression).Type;
                if (type != null && !m_context.GenericTypeConvert.IsGeneric(type) && type.TypeKind == TypeKind.Class)
                    m_context.Writer.Append("(void*)");

                m_context.Generators.Expression.Generate(node.Expression);
            }
        }
    }
}
