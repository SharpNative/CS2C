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

            // Member binding expression
            if (firstKind == SyntaxKind.MemberBindingExpression)
            {
                m_context.Generators.Expression.Generate(node.Parent.ChildNodes().First());
            }
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
                    objName = (children.First() as IdentifierNameSyntax).Identifier.ToString();
                }

                m_context.Writer.Append(objName);
                if (argsList.Arguments.Count() > 0)
                    m_context.Writer.Append(", ");
            }

            m_context.Generators.ArgumentList.Generate(argsList);

            m_context.Writer.Append(")");
        }
    }
}
