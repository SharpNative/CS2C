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
    class PrePostExpressionGenerator : GeneratorBase<ExpressionSyntax>
    {
        public enum ExpressionType
        {
            PreIncrement,
            PreDecrement,
            PostIncrement,
            PostDecrement
        }

        private ExpressionType m_expressionType;

        /// <summary>
        /// Post expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="expressionType">Expression type</param>
        public PrePostExpressionGenerator(WalkerContext context, ExpressionType expressionType)
        {
            m_context = context;
            m_expressionType = expressionType;
        }

        /// <summary>
        /// Generates the expression
        /// </summary>
        /// <param name="node">The expression</param>
        public override void Generate(ExpressionSyntax node)
        {
            ExpressionGenerator expressionGen = new ExpressionGenerator(m_context);

            IdentifierNameSyntax name = node.ChildNodes().First() as IdentifierNameSyntax;
            ISymbol symbol = m_context.Model.GetSymbolInfo(name).Symbol;
            bool isProperty = (symbol != null && symbol.Kind == SymbolKind.Property);

            string type = "";
            if (m_expressionType == ExpressionType.PreIncrement || m_expressionType == ExpressionType.PostIncrement)
                type = " + 1";
            else
                type = " - 1";

            if (isProperty)
            {
                string objName;
                if (symbol.IsStatic)
                    objName = "NULL";
                else
                    objName = "obj";

                m_context.Writer.Append(string.Format("{0}_{1}_setter({3}, {0}_{1}_getter({3}){2}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, type, objName));
            }
            else
            {
                m_context.Writer.Append(string.Format("{1} = {1}{0}", type, m_context.ConvertVariableName(name)));
            }

            if (isProperty)
            {
                m_context.Writer.Append(")");
            }
        }
    }
}
