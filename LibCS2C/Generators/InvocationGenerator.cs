using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
        /// Generates the object part
        /// </summary>
        /// <param name="node">The node for the object</param>
        /// <returns>The object part</returns>
        private string GenerateObjectPart(SyntaxNode node)
        {
            IEnumerable<SyntaxNode> children = node.ChildNodes();
            if (children.Count() == 0)
            {
                return "obj";
            }
            else
            {
                SyntaxNode firstChild = children.First();
                SyntaxKind firstChildKind = firstChild.Kind();

                // OBJECT.METHOD(...)
                if (firstChildKind == SyntaxKind.IdentifierName)
                {
                    WriterDestination destination = m_context.Writer.CurrentDestination;
                    m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                    m_context.Generators.Expression.Generate(firstChild);
                    m_context.Writer.CurrentDestination = destination;
                    return m_context.Writer.FlushTempBuffer();
                }
                // base.METHOD(...)
                else if (firstChildKind == SyntaxKind.BaseExpression)
                {
                    return "(void*)obj";
                }
                else
                {
                    WriterDestination destination = m_context.Writer.CurrentDestination;
                    m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                    m_context.Generators.Expression.Generate(firstChild as ExpressionSyntax);
                    m_context.Writer.CurrentDestination = destination;
                    return m_context.Writer.FlushTempBuffer();
                }
            }
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
            bool useFunctionPointer = false;

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
                ImmutableArray<SyntaxReference> references = firstSymbol.DeclaringSyntaxReferences;
                
                IMethodSymbol methodSym = null;
                if (firstSymbol.Kind == SymbolKind.Method)
                {
                    methodSym = firstSymbol as IMethodSymbol;
                }
                
                SyntaxNode reference = null;
                if (references.Length > 0)
                    reference = references[0].GetSyntax();

                if (reference != null && reference.Kind() == SyntaxKind.VariableDeclarator)
                {
                    foreach (SyntaxNode child in node.ChildNodes())
                    {
                        SyntaxKind childKind = child.Kind();
                        if (m_context.Generators.Expression.IsSubExpression(childKind))
                            m_context.Generators.Expression.Generate(child);
                    }
                }
                else
                {
                    needsObjReference = !methodSym.IsStatic;
                    SyntaxNode type = (first.Kind() == SyntaxKind.IdentifierName) ? first : first.ChildNodes().First();
                    string prototype = m_context.Generators.MethodDeclaration.CreateMethodPrototype(methodSym, false, false);

                    // If the type of the reference is an interface, we need to use the lookup table
                    // If there are classes that extend the type of the reference, we need to use the lookup table
                    // Else, we can call it directly
                    ITypeSymbol typeOfReference = m_context.Model.GetTypeInfo(type).Type;
                    if (typeOfReference == null)
                    {
                        m_context.Writer.Append(prototype);
                    }
                    else
                    {
                        MethodDeclarationSyntax methodDeclaration = reference as MethodDeclarationSyntax;
                        string lookFor = string.Format("{0}_{1}", m_context.ConvertNameSpace(typeOfReference.ContainingNamespace), typeOfReference.Name);
                        useFunctionPointer = (typeOfReference.TypeKind == TypeKind.Interface || (m_context.TypeIsExtended.ContainsKey(lookFor) && m_context.TypeIsExtended[lookFor]));

                        if (useFunctionPointer)
                        {
                            m_context.Writer.Append(string.Format("((fp_{0})({1}->lookup_table[{2}])", prototype, GenerateObjectPart(first), m_context.MethodTable.GetID(lookFor, methodDeclaration)));
                        }
                        else
                        {
                            m_context.Writer.Append(prototype);
                        }
                    }
                }
            }

            if (useFunctionPointer)
                m_context.Writer.Append(")");

            m_context.Writer.Append("(");

            // Reference to the object if needed
            if (needsObjReference)
            {
                string objName = GenerateObjectPart(first);

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
