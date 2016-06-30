﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    class ArrayCreationExpressionGenerator : GeneratorBase<ArrayCreationExpressionSyntax>
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
            // new (IdentifierName or Type)[ArrayRankSpecifier]
            IEnumerable<SyntaxNode> children = node.Type.ChildNodes();
            SyntaxNode first = children.First();
            SyntaxNode second = children.ElementAt(1);
            
            ArrayRankSpecifierSyntax rank = second as ArrayRankSpecifierSyntax;
            ExpressionSyntax sizeExpression = rank.Sizes.First();

            m_context.Writer.Append(string.Format("malloc(sizeof({0}) * (", m_context.ConvertTypeName(first)));
            ExpressionGenerator expressionGen = new ExpressionGenerator(m_context);
            expressionGen.Generate(sizeExpression);
            m_context.Writer.Append("))");
        }
    }
}
