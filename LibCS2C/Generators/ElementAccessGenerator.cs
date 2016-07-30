using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace LibCS2C.Generators
{
    public class ElementAccessGenerator : GeneratorBase<ElementAccessExpressionSyntax>
    {
        /// <summary>
        /// Element access expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ElementAccessGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates an element access expression
        /// </summary>
        /// <param name="node">The expression</param>
        public override void Generate(ElementAccessExpressionSyntax node)
        {
            m_context.Writer.ShouldOutputPost = true;

            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (SyntaxNode child in nodes)
            {
                SyntaxKind kind = child.Kind();

                if (kind == SyntaxKind.IdentifierName)
                {
                    ISymbol symbol = m_context.Model.GetSymbolInfo(child).Symbol;
                    if (symbol.Kind == SymbolKind.Field && !symbol.IsStatic)
                        m_context.Writer.Append("obj->");

                    m_context.Writer.Append(m_context.TypeConvert.ConvertVariableName(child));
                }
                else if (kind == SyntaxKind.BracketedArgumentList)
                {
                    BracketedArgumentListSyntax args = child as BracketedArgumentListSyntax;
                    IEnumerable<SyntaxNode> children = args.ChildNodes();

                    m_context.Writer.Append("[");

                    foreach (ArgumentSyntax childNode in children)
                    {
                        m_context.Generators.Expression.Generate(childNode.Expression);
                    }

                    m_context.Writer.Append("]");
                }
                else if (m_context.Generators.Expression.IsSubExpression(kind))
                {
                    m_context.Generators.Expression.Generate(child);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            m_context.Writer.ShouldOutputPost = false;
        }
    }
}
