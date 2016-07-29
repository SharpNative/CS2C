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
    public class InvocationGenerator : GeneratorBase<InvocationExpressionSyntax>
    {
        /// <summary>
        /// Invocation generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public InvocationGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates an invocation
        /// </summary>
        /// <param name="node">The expression</param>
        public override void Generate(InvocationExpressionSyntax node)
        {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            SyntaxNode first = nodes.First();
            ISymbol firstSymbol = m_context.Model.GetSymbolInfo(first).Symbol;
            SyntaxKind firstKind = first.Kind();
            
            bool needsObjReference = false;

            // Arguments
            ArgumentListSyntax argsList = node.ArgumentList;
            int argCount = argsList.ChildNodes().Count();

            // Member binding expression call
            if (firstKind == SyntaxKind.MemberBindingExpression)
            {
                m_context.Generators.Expression.Generate(node.Parent.ChildNodes().First());
            }
            // Normal method call
            else
            {
                MethodDeclarationSyntax reference = firstSymbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                needsObjReference = !m_context.Generators.MethodDeclaration.IsMethodStatic(reference);
                m_context.Writer.Append(m_context.Generators.MethodDeclaration.CreateMethodPrototype(reference, false));
            }

            m_context.Writer.Append("(");
            
            // Reference to the object if needed
            if (needsObjReference)
            {
                string objName;
                IEnumerable<SyntaxNode> children = first.ChildNodes();
                if(children.Count() == 0)
                {
                    objName = "obj";
                }
                else
                {
                    SyntaxNode firstChild = children.First();
                    SyntaxKind firstChildKind = firstChild.Kind();

                    // OBJECT.METHOD(...)
                    if (firstChildKind == SyntaxKind.IdentifierName)
                    {
                        ISymbol sym = m_context.Model.GetSymbolInfo(firstChild).Symbol;
                        
                        if (sym != null && sym.Kind == SymbolKind.Field)
                        {
                            m_context.Writer.Append("obj->field_");
                        }
                        
                        objName = (firstChild as IdentifierNameSyntax).Identifier.ToString();
                    }
                    // base.METHOD(...)
                    else if(firstChildKind == SyntaxKind.BaseExpression)
                    {
                        objName = "(void*)obj";
                    }
                    // member.member
                    else if (firstChildKind == SyntaxKind.SimpleMemberAccessExpression)
                    {
                        WriterDestination destination = m_context.CurrentDestination;
                        m_context.CurrentDestination = WriterDestination.TempBuffer;
                        m_context.Generators.SimpleMemberAccess.Generate(firstChild as MemberAccessExpressionSyntax);
                        m_context.CurrentDestination = destination;
                        objName = m_context.FlushTempBuffer();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                
                // Argument for the object reference
                m_context.Writer.Append(objName);
                if (argsList.Arguments.Count() > 0)
                    m_context.Writer.Append(", ");
            }

            m_context.Generators.ArgumentList.Generate(argsList);

            m_context.Writer.Append(")");
        }
    }
}
