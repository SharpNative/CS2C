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
    public class InvocationGenerator : GeneratorBase<ExpressionSyntax>
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
        public override void Generate(ExpressionSyntax node)
        {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            SyntaxNode first = nodes.First();
            SyntaxKind firstKind = first.Kind();
            
            string memberName = "";
            bool needsObjReference = false;

            // Own class
            if (firstKind == SyntaxKind.IdentifierName)
            {
                IdentifierNameSyntax name = first as IdentifierNameSyntax;
                memberName = string.Format("{0}_{1}", m_context.CurrentClassNameFormatted, name.Identifier);
            }
            // Another class
            else if (firstKind == SyntaxKind.SimpleMemberAccessExpression)
            {
                MemberAccessExpressionSyntax name = first as MemberAccessExpressionSyntax;
                ITypeSymbol symbol = m_context.Model.GetTypeInfo(name.ChildNodes().First()).Type;
                ITypeSymbol symbolParent = m_context.Model.GetTypeInfo(name).Type;
                
                needsObjReference = (symbolParent != null && !symbolParent.IsStatic);
                memberName = string.Format("{0}_{1}_{2}", symbol.ContainingSymbol.ToString().Replace(".", "_"), symbol.Name, name.Name);
            }
            else
            {
                throw new NotSupportedException();
            }
            
            m_context.Writer.Append(string.Format("{0}(", memberName));

            // Arguments
            ArgumentListSyntax argsList = (from a in nodes
                                           where a.Kind() == SyntaxKind.ArgumentList
                                           select a).FirstOrDefault() as ArgumentListSyntax;

            // Reference to the object if needed
            if (needsObjReference)
            {
                string objName = (first.ChildNodes().First() as IdentifierNameSyntax).Identifier.ToString();

                if(argsList.Arguments.Count() == 0)
                    m_context.Writer.Append(objName);
                else
                    m_context.Writer.Append(objName + ", ");
            }

            m_context.Generators.ArgumentList.Generate(argsList);

            m_context.Writer.Append(")");
        }
    }
}
