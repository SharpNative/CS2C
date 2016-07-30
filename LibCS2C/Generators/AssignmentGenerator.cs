using LibCS2C.Context;
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
    public enum AssignmentType
    {
        LeftShift,
        RightShift,
        BinaryOr,
        ExclusiveOr,
        BinaryAnd,
        Add,
        Substract,
        Multiply,
        Divide
    }

    public class AssignmentGenerator : GeneratorBase<AssignmentExpressionSyntax>
    {
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
        /// Generates the assignment 
        /// </summary>
        /// <param name="node">The assignment</param>
        public override void Generate(AssignmentExpressionSyntax node)
        {
            ISymbol symbol = m_context.Model.GetSymbolInfo(node.Left).Symbol;

            string assignmentSymbol = "";
            switch (m_assignmentType)
            {
                case AssignmentType.BinaryAnd:
                    assignmentSymbol = "&";
                    break;

                case AssignmentType.BinaryOr:
                    assignmentSymbol = "|";
                    break;

                case AssignmentType.LeftShift:
                    assignmentSymbol = "<<";
                    break;

                case AssignmentType.RightShift:
                    assignmentSymbol = ">>";
                    break;

                case AssignmentType.ExclusiveOr:
                    assignmentSymbol = "^";
                    break;

                case AssignmentType.Add:
                    assignmentSymbol = "+";
                    break;

                case AssignmentType.Substract:
                    assignmentSymbol = "-";
                    break;

                case AssignmentType.Multiply:
                    assignmentSymbol = "*";
                    break;

                case AssignmentType.Divide:
                    assignmentSymbol = "/";
                    break;
            }

            // Property (getter/setter required)
            if (symbol != null && symbol.Kind == SymbolKind.Property)
            {
                if (symbol.IsStatic)
                    m_context.Writer.Append(string.Format("{0}_{1}_setter(", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name));
                else
                    m_context.Writer.Append(string.Format("{0}_{1}_setter(obj, ", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name));

                m_context.Generators.Expression.Generate(node.Left);
                m_context.Writer.Append(string.Format(" {0} ", assignmentSymbol));
                m_context.Generators.Expression.Generate(node.Right);
                m_context.Writer.Append(")");
            }
            // Normal variable / field
            else
            {
                m_context.Generators.Expression.Generate(node.Left);
                m_context.Writer.Append(string.Format(" {0}= ", assignmentSymbol));
                m_context.Generators.Expression.Generate(node.Right);
            }
        }
    }
}
