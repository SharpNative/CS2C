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
    public enum ExpressionType
    {
        PreIncrement,
        PreDecrement,
        PostIncrement,
        PostDecrement
    }

    public class PrePostExpressionGenerator : GeneratorBase<ExpressionSyntax>
    {
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
            SyntaxNode name = node.ChildNodes().First();
            ISymbol symbol = m_context.Model.GetSymbolInfo(name).Symbol;
            
            bool isProperty = (symbol != null && symbol.Kind == SymbolKind.Property);

            string type = "";
            if (m_expressionType == ExpressionType.PreIncrement || m_expressionType == ExpressionType.PostIncrement)
                type = " + 1";
            else
                type = " - 1";

            bool isPost = (m_expressionType == ExpressionType.PostIncrement || m_expressionType == ExpressionType.PostDecrement);
            WriterDestination destination = m_context.CurrentDestination;

            string getter = "";
            if (isProperty)
            {
                if (symbol.IsStatic)
                {
                    getter = string.Format("{0}_{1}_getter(){2}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, type);

                    if (isPost)
                        m_context.CurrentDestination = WriterDestination.PostBuffer;

                    m_context.Writer.Append(string.Format("{0}_{1}_setter({2})", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, getter));
                }
                else
                {
                    getter = string.Format("{0}_{1}_getter(obj){2}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, type);

                    if (isPost)
                        m_context.CurrentDestination = WriterDestination.PostBuffer;

                    m_context.Writer.Append(string.Format("{0}_{1}_setter(obj, {2})", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, getter));
                }
            }
            else
            {
                string prefix = "";
                if (symbol.Kind == SymbolKind.Field && !symbol.IsStatic)
                    prefix += "obj->";

                getter = prefix + m_context.ConvertVariableName(name);

                if (isPost)
                    m_context.CurrentDestination = WriterDestination.PostBuffer;

                m_context.Writer.Append(string.Format("{0} = {0}{1}", getter, type));
            }

            if (isPost)
            {
                m_context.CurrentDestination = destination;
            }

            if (m_context.ShouldOutputPost)
                m_context.Writer.Append(getter);
            else
                m_context.Writer.AppendLine(string.Format("{0}", m_context.FlushPostBuffer()));
        }
    }
}
