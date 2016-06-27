using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
{
    class ExpressionGenerator : GeneratorBase<ExpressionSyntax>
    {
        private InvocationGenerator m_invocationGen;
        private SimpleAssignmentGenerator m_simpleAssignmentGen;
        private SimpleMemberAccessGenerator m_simpleMemberAccessGen;
        private ObjectCreationExpressionGenerator m_objectCreationGen;

        /// <summary>
        /// Expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ExpressionGenerator(WalkerContext context)
        {
            m_context = context;
            m_simpleAssignmentGen = new SimpleAssignmentGenerator(m_context);
            m_invocationGen = new InvocationGenerator(m_context);
            m_simpleMemberAccessGen = new SimpleMemberAccessGenerator(m_context);
            m_objectCreationGen = new ObjectCreationExpressionGenerator(m_context);
        }

        /// <summary>
        /// Generates the expression code
        /// </summary>
        /// <param name="node">The expression node</param>
        public override void Generate(ExpressionSyntax node)
        {
            SyntaxKind kind = node.Kind();
            
            if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                m_simpleMemberAccessGen.Generate(node);
            }
            else if (kind == SyntaxKind.SimpleAssignmentExpression)
            {
                m_simpleAssignmentGen.Generate(node);
            }
            else if (kind == SyntaxKind.InvocationExpression)
            {
                m_invocationGen.Generate(node);
            }
            else if(kind == SyntaxKind.ObjectCreationExpression)
            {
                m_objectCreationGen.Generate(node as ObjectCreationExpressionSyntax);
            }
            else if (kind == SyntaxKind.AddExpression ||
                     kind == SyntaxKind.SubtractExpression ||
                     kind == SyntaxKind.MultiplyExpression ||
                     kind == SyntaxKind.DivideExpression)
            {
                ChildSyntaxList children = node.ChildNodesAndTokens();
                foreach(SyntaxNodeOrToken child in children)
                {
                    SyntaxKind childKind = child.Kind();

                    if(child.IsToken)
                    {
                        m_context.Writer.Append(" " + child.ToString() + " ");
                    }
                    else
                    {
                        if(childKind == SyntaxKind.IdentifierName)
                        {
                            IdentifierNameSyntax name = child.AsNode() as IdentifierNameSyntax;
                            m_context.Writer.Append(m_context.ConvertVariableName(name));
                        }
                        else
                        {
                            m_context.Writer.Append(child.ToString());
                        }
                    }
                }
            }
            else
            {
                m_context.Writer.Append(node.ToString());
            }
        }
    }
}
