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
        private void GenerateObjectPart(SyntaxNode node)
        {
            SyntaxKind firstKind = node.Kind();

            if (firstKind == SyntaxKind.ThisExpression)
            {
                m_context.Writer.Append("obj");
            }
            else if (firstKind == SyntaxKind.IdentifierName)
            {
                m_context.Writer.Append(m_context.TypeConvert.ConvertVariableName(node as IdentifierNameSyntax));
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
            bool isEnum = symbolType.TypeKind == TypeKind.Enum;

            // After the type is checked as enum, check if it is an assignment to a enum type
            // if so, we cannot transform this to a constant
            if(isEnum)
            {
                SyntaxNode parent = node.Parent;
                if (parent is AssignmentExpressionSyntax)
                {
                    AssignmentExpressionSyntax assignment = parent as AssignmentExpressionSyntax;
                    if (node != assignment.Right)
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
                ISymbol symbol = m_context.Model.GetSymbolInfo(node).Symbol;
                SyntaxNode first = children[0].AsNode();
                
                // Object part that contains the requested variable
                bool objectFirst = (!symbol.IsStatic && symbol.Kind != SymbolKind.Property);
                if (objectFirst)
                {
                    GenerateObjectPart(first);

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
                    ISymbol sym = m_context.Model.GetSymbolInfo(first).Symbol;

                    if (!m_context.GenericTypeConvert.IsGeneric(typeSymbol.Name) && typeSymbol.TypeKind == TypeKind.Struct)
                        m_context.Writer.Append("&");
                    
                    if (sym != null && sym.Kind == SymbolKind.Field)
                        m_context.Writer.Append("obj->");

                    GenerateObjectPart(first);
                    m_context.Writer.Append(")");
                }
            }
        }
    }
}
