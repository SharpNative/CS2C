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
    public class SimpleMemberAccessGenerator : GeneratorBase<ExpressionSyntax>
    {
        /// <summary>
        /// Member access generator generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="type">The method type: constructor or method</param>
        public SimpleMemberAccessGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a member access
        /// </summary>
        /// <param name="node">The expression of the member access</param>
        public override void Generate(ExpressionSyntax node)
        {
            ITypeSymbol symbolType = m_context.Model.GetTypeInfo(node).Type;
            SyntaxNodeOrToken[] children = node.ChildNodesAndTokens().ToArray();

            // Enum
            if (symbolType.TypeKind == TypeKind.Enum)
            {
                IdentifierNameSyntax name = children[2].AsNode() as IdentifierNameSyntax;
                m_context.Writer.Append("enum_" + symbolType.ToString().Replace(".", "_") + "_" + name.Identifier);
            }
            // Normal member access
            else
            {
                // children: (this / object) (dot) (identifier)
                
                // If it's static, we don't need the first part (identifier)
                // because the reference is already in the second part (identifier)
                ISymbol symbol = m_context.Model.GetSymbolInfo(node).Symbol;
                if (!symbol.IsStatic)
                {
                    SyntaxNode first = children[0].AsNode();
                    SyntaxKind firstKind = first.Kind();

                    if (firstKind == SyntaxKind.ThisExpression)
                    {
                        m_context.Writer.Append("obj");
                    }
                    else if (firstKind == SyntaxKind.IdentifierName)
                    {
                        m_context.Writer.Append(m_context.ConvertVariableName(first as IdentifierNameSyntax));
                    }
                    else if (firstKind == SyntaxKind.ElementAccessExpression)
                    {
                        m_context.Generators.ElementAccess.Generate(first as ElementAccessExpressionSyntax);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    ITypeSymbol type = m_context.Model.GetTypeInfo(first).Type;
                    if (type.TypeKind == TypeKind.Struct)
                        m_context.Writer.Append(".");
                    else
                        m_context.Writer.Append("->");
                }
                
                IdentifierNameSyntax name = children[2].AsNode() as IdentifierNameSyntax;
                m_context.Writer.Append(m_context.ConvertVariableName(name));
            }
        }
    }
}
