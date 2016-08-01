using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibCS2C.Generators
{
    public class PointerMemberAccessGenerator : GeneratorBase<MemberAccessExpressionSyntax>
    {
        /// <summary>
        /// PointerMemberAccess expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public PointerMemberAccessGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a PointerMemberAccess
        /// </summary>
        /// <param name="node">The access</param>
        public override void Generate(MemberAccessExpressionSyntax node)
        {
            ChildSyntaxList children = node.ChildNodesAndTokens();

            bool first = true;
            foreach (SyntaxNodeOrToken child in children)
            {
                SyntaxKind childKind = child.Kind();
                if (childKind == SyntaxKind.MinusGreaterThanToken)
                {
                    m_context.Writer.Append("->");
                }
                else if(childKind == SyntaxKind.IdentifierName)
                {
                    ISymbol firstSymbol = m_context.Model.GetSymbolInfo(child.AsNode()).Symbol;
                    if (firstSymbol != null && (!firstSymbol.IsStatic && firstSymbol.Kind == SymbolKind.Field) && first)
                        m_context.Writer.Append("obj->");

                    m_context.Writer.Append(m_context.TypeConvert.ConvertVariableName(child.AsNode()));
                }
                else
                {
                    m_context.Generators.Expression.Generate(child.AsNode());
                }

                first = false;
            }
        }
    }
}
