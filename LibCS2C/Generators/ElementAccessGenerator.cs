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
    class ElementAccessGenerator : GeneratorBase<ElementAccessExpressionSyntax>
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
            ExpressionGenerator expressionGen = new ExpressionGenerator(m_context);

            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (SyntaxNode child in nodes)
            {
                SyntaxKind kind = child.Kind();

                if (kind == SyntaxKind.IdentifierName)
                {
                    m_context.Writer.Append(m_context.ConvertVariableName(child));
                }
                else if (kind == SyntaxKind.BracketedArgumentList)
                {
                    BracketedArgumentListSyntax args = child as BracketedArgumentListSyntax;
                    IEnumerable<SyntaxNode> children = args.ChildNodes();

                    m_context.Writer.Append("[");

                    foreach (ArgumentSyntax childNode in children)
                    {
                        expressionGen.Generate(childNode.Expression);
                    }

                    m_context.Writer.Append("]");
                }
            }
        }
    }
}
