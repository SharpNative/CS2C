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
    class AssignmentGenerator : GeneratorBase<AssignmentExpressionSyntax>
    {
        public enum AssignmentType
        {
            LeftShift,
            RightShift,
            BinaryOr,
            ExclusiveOr,
            BinaryAnd
        }

        private AssignmentType m_assignmentType;
        
        /// <summary>
        /// Assignment generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="assignmentType">Assignment type</param>
        public AssignmentGenerator(WalkerContext context, AssignmentType assignmentType)
        {
            m_context = context;
            m_assignmentType = assignmentType;
        }

        /// <summary>
        /// Generates the left shift assignment 
        /// </summary>
        /// <param name="node">The left shift assignment</param>
        public override void Generate(AssignmentExpressionSyntax node)
        {
            ExpressionGenerator expressionGen = new ExpressionGenerator(m_context);
            expressionGen.Generate(node.Left);

            switch(m_assignmentType)
            {
                case AssignmentType.BinaryAnd:
                    m_context.Writer.Append(" &= ");
                    break;
                case AssignmentType.BinaryOr:
                    m_context.Writer.Append(" |= ");
                    break;
                case AssignmentType.LeftShift:
                    m_context.Writer.Append(" <<= ");
                    break;
                case AssignmentType.RightShift:
                    m_context.Writer.Append(" >>= ");
                    break;
                case AssignmentType.ExclusiveOr:
                    m_context.Writer.Append(" ^= ");
                    break;
                default:
                    break;
            }
            
            expressionGen.Generate(node.Right);
            m_context.Writer.AppendLine(";");
        }
    }
}
