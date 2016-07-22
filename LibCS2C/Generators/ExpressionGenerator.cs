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
    public class ExpressionGenerator : GeneratorBase<SyntaxNode>
    {
        /// <summary>
        /// Expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ExpressionGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates the expression code
        /// </summary>
        /// <param name="node">The expression node</param>
        public override void Generate(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ArrayCreationExpression:
                    m_context.Generators.ArrayCreationExpression.Generate(node as ArrayCreationExpressionSyntax);
                    break;

                case SyntaxKind.AddressOfExpression:
                    m_context.Generators.AddressOfExpression.Generate(node as PrefixUnaryExpressionSyntax);
                    break;

                case SyntaxKind.ElementAccessExpression:
                    m_context.Generators.ElementAccess.Generate(node as ElementAccessExpressionSyntax);
                    break;

                case SyntaxKind.SimpleMemberAccessExpression:
                    m_context.Generators.SimpleMemberAccess.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.ObjectCreationExpression:
                    m_context.Generators.ObjectCreationExpression.Generate(node as ObjectCreationExpressionSyntax);
                    break;

                case SyntaxKind.SimpleAssignmentExpression:
                    m_context.Generators.SimpleAssignment.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.InvocationExpression:
                    m_context.Generators.Invocation.Generate(node as InvocationExpressionSyntax);
                    break;

                case SyntaxKind.LeftShiftAssignmentExpression:
                    m_context.Generators.LeftShiftAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.RightShiftAssignmentExpression:
                    m_context.Generators.RightShiftAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.OrAssignmentExpression:
                    m_context.Generators.BinaryOrAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.AndAssignmentExpression:
                    m_context.Generators.BinaryAndAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    m_context.Generators.ExclusiveOrAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.AddAssignmentExpression:
                    m_context.Generators.AddAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.SubtractAssignmentExpression:
                    m_context.Generators.SubstractAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;
                    
                case SyntaxKind.MultiplyAssignmentExpression:
                    m_context.Generators.MultiplyAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.DivideAssignmentExpression:
                    m_context.Generators.DivideAssignment.Generate(node as AssignmentExpressionSyntax);
                    break;

                case SyntaxKind.LessThanExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.ConditionalExpression:
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
                    ISymbol symbol = m_context.Model.GetSymbolInfo(node).Symbol;
                    if (symbol.Kind == SymbolKind.Field && !symbol.IsStatic)
                        m_context.Writer.Append("obj->");

                    m_context.Writer.Append(m_context.ConvertVariableName(node));
                    break;

                case SyntaxKind.CastExpression:
                    m_context.Generators.CastExpression.Generate(node as CastExpressionSyntax);
                    break;

                case SyntaxKind.SizeOfExpression:
                    m_context.Generators.SizeOfExpression.Generate(node as SizeOfExpressionSyntax);
                    break;

                case SyntaxKind.PostDecrementExpression:
                    m_context.Generators.PostDecrementExpression.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.PostIncrementExpression:
                    m_context.Generators.PostIncrementExpression.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.PreDecrementExpression:
                    m_context.Generators.PreDecrementExpression.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.PreIncrementExpression:
                    m_context.Generators.PreIncrementExpression.Generate(node as ExpressionSyntax);
                    break;

                case SyntaxKind.ConditionalAccessExpression:
                    m_context.Generators.ConditionalAccessExpression.Generate(node as ConditionalAccessExpressionSyntax);
                    break;
                    
                default:
                    m_context.Writer.Append(node.ToString());
                    break;
            }
        }
    }
}
