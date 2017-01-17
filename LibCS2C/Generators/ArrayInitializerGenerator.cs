using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibCS2C.Generators
{
    public class ArrayInitializerGenerator : GeneratorBase<InitializerExpressionSyntax>
    {
        /// <summary>
        /// Array initializer expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ArrayInitializerGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates an array intializer
        /// </summary>
        /// <param name="node">The expression</param>
        public override void Generate(InitializerExpressionSyntax node)
        {
            SeparatedSyntaxList<ExpressionSyntax> children = node.Expressions;

            m_context.Writer.Append("{");
            foreach (ExpressionSyntax child in children)
            {
                m_context.Generators.Expression.Generate(child);

                if (child != children.Last())
                    m_context.Writer.Append(", ");
            }
            m_context.Writer.Append("}");
        }
    }
}
