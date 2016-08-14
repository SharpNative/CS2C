using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace LibCS2C.Generators
{
    public class ArrayCreationExpressionGenerator : GeneratorBase<ArrayCreationExpressionSyntax>
    {
        /// <summary>
        /// Sizeof statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ArrayCreationExpressionGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates an array creation expression
        /// </summary>
        /// <param name="node">The array creation expression</param>
        public override void Generate(ArrayCreationExpressionSyntax node)
        {
            IEnumerable<SyntaxNode> children = node.Type.ChildNodes();
            SyntaxNode second = children.ElementAt(1);
            
            ArrayRankSpecifierSyntax rank = second as ArrayRankSpecifierSyntax;
            ExpressionSyntax sizeExpression = rank.Sizes.First();

            // Malloc returns a pointer to the type
            // So that means this type has a pointer too much
            string type = m_context.ConvertTypeName(node.Type);
            type = type.Substring(0, type.Length - 1);

            /*m_context.Writer.Append(string.Format("malloc(sizeof({0}) * (", type));
            m_context.Generators.Expression.Generate(sizeExpression);
            m_context.Writer.Append("))");*/

            m_context.Writer.Append("calloc((");
            m_context.Generators.Expression.Generate(sizeExpression);
            m_context.Writer.Append(string.Format("), sizeof({0}))", type));
        }
    }
}
