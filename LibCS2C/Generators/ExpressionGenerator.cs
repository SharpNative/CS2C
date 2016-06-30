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
    class ExpressionGenerator : GeneratorBase<SyntaxNode>
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
        private AssignmentGenerator m_addAssignmentGen;
        private AssignmentGenerator m_substractAssignmentGen;
        private AssignmentGenerator m_multiplyAssignmentGen;
        private AssignmentGenerator m_divideAssignmentGen;

        private PrePostExpressionGenerator m_preIncrementExpressionGen;
        private PrePostExpressionGenerator m_preDecrementExpressionGen;
        private PrePostExpressionGenerator m_postIncrementExpressionGen;
        private PrePostExpressionGenerator m_postDecrementExpressionGen;

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
            m_addAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.Add);
            m_substractAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.Substract);
            m_multiplyAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.Multiply);
            m_divideAssignmentGen = new AssignmentGenerator(m_context, AssignmentGenerator.AssignmentType.Divide);

            m_preIncrementExpressionGen = new PrePostExpressionGenerator(m_context, PrePostExpressionGenerator.ExpressionType.PreIncrement);
            m_preDecrementExpressionGen = new PrePostExpressionGenerator(m_context, PrePostExpressionGenerator.ExpressionType.PreDecrement);
            m_postIncrementExpressionGen = new PrePostExpressionGenerator(m_context, PrePostExpressionGenerator.ExpressionType.PostIncrement);
            m_postDecrementExpressionGen = new PrePostExpressionGenerator(m_context, PrePostExpressionGenerator.ExpressionType.PostDecrement);
        }

        /// <summary>
        /// Generates the expression code
        /// </summary>
        /// <param name="node">The expression node</param>
        public override void Generate(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    m_simpleMemberAccessGen.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.ObjectCreationExpression:
                    m_objectCreationGen.Generate(node as ObjectCreationExpressionSyntax);
                    break;

                case SyntaxKind.SimpleAssignmentExpression:
                    m_simpleAssignmentGen.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.InvocationExpression:
                    m_invocationGen.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.LeftShiftAssignmentExpression:
                    m_leftShiftAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.RightShiftAssignmentExpression:
                    m_rightShiftAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.OrAssignmentExpression:
                    m_binaryOrAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.AndAssignmentExpression:
                    m_binaryAndAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    m_exclusiveOrAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.AddAssignmentExpression:
                    m_addAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.SubtractAssignmentExpression:
                    m_substractAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;
                    
                case SyntaxKind.MultiplyAssignmentExpression:
                    m_multiplyAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.DivideAssignmentExpression:
                    m_divideAssignmentGen.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.LessThanExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.BitwiseOrExpression:
                    ChildSyntaxList children = node.ChildNodesAndTokens();
                    foreach (SyntaxNodeOrToken child in children)
                    {
                        SyntaxKind childKind = child.Kind();

                        if (child.IsToken)
                        {
                            m_context.Writer.Append(" " + child + " ");
                        }
                        else
                        {
                            Generate(child.AsNode());
                        }
                    }
                    break;

                case SyntaxKind.IdentifierName:
                    m_context.Writer.Append(m_context.ConvertVariableName(node));
                    break;

                case SyntaxKind.CastExpression:
                    m_castExpressionGen.Generate(node as CastExpressionSyntax);
                    break;

                case SyntaxKind.PostDecrementExpression:
                    m_postDecrementExpressionGen.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.PostIncrementExpression:
                    m_postIncrementExpressionGen.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.PreDecrementExpression:
                    m_preDecrementExpressionGen.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.PreIncrementExpression:
                    m_preIncrementExpressionGen.Generate(node as ExpressionSyntax);
                    break;
                    
                default:
                    m_context.Writer.Append(node.ToString());
                    break;
            }
        }
    }
}
