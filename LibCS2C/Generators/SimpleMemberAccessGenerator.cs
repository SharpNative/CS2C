using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

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
        /// Generates the object part
        /// </summary>
        /// <param name="node">The object node</param>
        /// <param name="identifierAsExpression">If the identifier should be parsed as an expression (for object part)</param>
        private void GenerateObjectPart(SyntaxNode node, bool identifierAsExpression)
        {
            SyntaxKind firstKind = node.Kind();

            if (firstKind == SyntaxKind.ThisExpression)
            {
                m_context.Writer.Append("obj");
            }
            else if (firstKind == SyntaxKind.IdentifierName)
            {
                if (identifierAsExpression)
                    m_context.Generators.Expression.Generate(node);
                else
                    m_context.Writer.Append(m_context.TypeConvert.ConvertVariableName(node));
            }
            else if (m_context.Generators.Expression.IsSubExpression(firstKind))
            {
                m_context.Generators.Expression.Generate(node);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Generates a member access
        /// </summary>
        /// <param name="node">The expression of the member access</param>
        public override void Generate(ExpressionSyntax node)
        {
            ITypeSymbol symbolType = m_context.Model.GetTypeInfo(node).Type;
            SyntaxNodeOrToken[] children = node.ChildNodesAndTokens().ToArray();


            Console.WriteLine(node + " " + symbolType.TypeKind + " " + symbolType.Kind);


            bool isEnum = symbolType.TypeKind == TypeKind.Enum;
            
            // After the type is checked as enum, check if it is an assignment to a enum type
            // if so, we cannot transform this to a constant
            if (isEnum)
            {
                SyntaxNode parent = node.Parent;

                SyntaxKind[] kinds = { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression, SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanOrEqualExpression, SyntaxKind.GreaterThanExpression, SyntaxKind.LessThanExpression };
                if (parent is AssignmentExpressionSyntax || kinds.Contains(parent.Kind()))
                {
                    // Check if the right hand of the expression isn't equal to this node, then we know it comes from the left
                    // and should be a variable instead of an enum
                    if (node != parent.ChildNodes().ElementAt(1))
                    {
                        isEnum = false;
                    }
                }
            }

            // Enum
            if (isEnum)
            {
                IdentifierNameSyntax name = children[2].AsNode() as IdentifierNameSyntax;
                m_context.Writer.Append("enum_" + symbolType.ToString().Replace(".", "_") + "_" + name.Identifier);
            }
            // Normal member access
            else
            {
                // If it's static, we don't need the first part (identifier)
                // because the reference is already in the second part (identifier)
                SyntaxNode first = children[0].AsNode();
                ISymbol symbol = m_context.Model.GetSymbolInfo(node).Symbol;
                ISymbol firstSymbol = m_context.Model.GetSymbolInfo(first).Symbol;

                // Object part that contains the requested variable
                bool objectFirst = (!symbol.IsStatic && symbol.Kind != SymbolKind.Property);
                // bool objectFirst = (firstSymbol != null && !firstSymbol.IsStatic && symbol.Kind != SymbolKind.Property);
                if (objectFirst)
                {
                    GenerateObjectPart(first, true);

                    ITypeSymbol type = m_context.Model.GetTypeInfo(first).Type;
                    if (type.TypeKind == TypeKind.Struct)
                        m_context.Writer.Append(".");
                    else
                        m_context.Writer.Append("->");
                }

                // Variable name
                IdentifierNameSyntax name = children[2].AsNode() as IdentifierNameSyntax;
                m_context.Writer.Append(m_context.TypeConvert.ConvertVariableName(name));

                // Property getter stuff
                if (symbol.Kind == SymbolKind.Property && !symbol.IsStatic)
                {
                    m_context.Writer.Append("(");

                    ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(first).Type;

                    if (!m_context.GenericTypeConvert.IsGeneric(typeSymbol.Name) && typeSymbol.TypeKind == TypeKind.Struct)
                        m_context.Writer.Append("&");

                    if (firstSymbol != null && (!firstSymbol.IsStatic && firstSymbol.Kind == SymbolKind.Field))
                        m_context.Writer.Append("obj->");

                    GenerateObjectPart(first, false);
                    m_context.Writer.Append(")");
                }
            }
        }
    }
}
