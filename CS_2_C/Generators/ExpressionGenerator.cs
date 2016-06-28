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
        private CastExpressionGenerator m_castExpressionGen;

        private AssignmentGenerator m_leftShiftAssignmentGen;
        private AssignmentGenerator m_rightShiftAssignmentGen;
        private AssignmentGenerator m_binaryOrAssignmentGen;
        private AssignmentGenerator m_exclusiveOrAssignmentGen;
        private AssignmentGenerator m_binaryAndAssignmentGen;
        
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
            m_castExpressionGen = new CastExpressionGenerator(m_context);

            m_leftShiftAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.LeftShift);
            m_rightShiftAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.RightShift);
            m_binaryOrAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.BinaryOr);
            m_exclusiveOrAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.ExclusiveOr);
            m_binaryAndAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.BinaryAnd);
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
            else if (kind == SyntaxKind.ObjectCreationExpression)
            {
                m_objectCreationGen.Generate(node as ObjectCreationExpressionSyntax);
            }
            else if(kind == SyntaxKind.LeftShiftAssignmentExpression)
            {
                m_leftShiftAssignmentGen.Generate(node as AssignmentExpressionSyntax);
            }
            else if (kind == SyntaxKind.RightShiftAssignmentExpression)
            {
                m_rightShiftAssignmentGen.Generate(node as AssignmentExpressionSyntax);
            }
            else if (kind == SyntaxKind.OrAssignmentExpression)
            {
                m_binaryOrAssignmentGen.Generate(node as AssignmentExpressionSyntax);
            }
            else if (kind == SyntaxKind.AndAssignmentExpression)
            {
                m_binaryAndAssignmentGen.Generate(node as AssignmentExpressionSyntax);
            }
            else if (kind == SyntaxKind.ExclusiveOrAssignmentExpression)
            {
                m_exclusiveOrAssignmentGen.Generate(node as AssignmentExpressionSyntax);
            }
            else if (kind == SyntaxKind.AddExpression ||
                     kind == SyntaxKind.SubtractExpression ||
                     kind == SyntaxKind.MultiplyExpression ||
                     kind == SyntaxKind.DivideExpression)
            {
                ChildSyntaxList children = node.ChildNodesAndTokens();
                foreach (SyntaxNodeOrToken child in children)
                {
                    SyntaxKind childKind = child.Kind();

                    if (child.IsToken)
                    {
                        m_context.Writer.Append(" " + child.ToString() + " ");
                    }
                    else
                    {
                        if (childKind == SyntaxKind.IdentifierName)
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
            else if (kind == SyntaxKind.CastExpression)
            {
                m_castExpressionGen.Generate(node as CastExpressionSyntax);
            }
            else if(kind == SyntaxKind.IdentifierName)
            {
                IdentifierNameSyntax name = node as IdentifierNameSyntax;
                m_context.Writer.Append(m_context.ConvertVariableName(name));
            }
            else
            {
                m_context.Writer.Append(node.ToString());
            }
        }
    }
}
