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
    class InvocationGenerator : GeneratorBase<ExpressionSyntax>
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
                memberName = m_context.CurrentClassNameFormatted + "_" + name.Identifier;
            }
            // Another class
            else if (firstKind == SyntaxKind.SimpleMemberAccessExpression)
            {
                ISymbol symbol = m_context.Model.GetSymbolInfo(first).Symbol;
                MemberAccessExpressionSyntax name = first as MemberAccessExpressionSyntax;

                // Static
                if (symbol.IsStatic)
                {
                    memberName = name.ToString().Replace(".", "_");
                }
                // Belongs to class
                else
                {
                    needsObjReference = true;
                    memberName = string.Format("{0}_{1}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name);
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            m_context.Writer.Append(string.Format("{0}(", memberName));

            // Arguments
            ArgumentListGenerator argGen = new ArgumentListGenerator(m_context);
            ArgumentListSyntax argsList = null;
            foreach (SyntaxNode childNode in nodes)
            {
                if (childNode.Kind() == SyntaxKind.ArgumentList)
                {
                    argsList = childNode as ArgumentListSyntax;
                    break;
                }
            }

            // Reference to the object if needed
            if (needsObjReference)
            {
                string objName = (first.ChildNodes().First() as IdentifierNameSyntax).Identifier.ToString();

                if(argsList.Arguments.Count() == 0)
                    m_context.Writer.Append(objName);
                else
                    m_context.Writer.Append(objName + ", ");
            }

            argGen.Generate(argsList);

            m_context.Writer.AppendLine(");");
        }
    }
}
