using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

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
        private bool m_isPost;
        private string m_type;
        private string m_shortType;

        /// <summary>
        /// Post expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="expressionType">Expression type</param>
        public PrePostExpressionGenerator(WalkerContext context, ExpressionType expressionType)
        {
            m_context = context;
            m_expressionType = expressionType;

            m_isPost = (expressionType == ExpressionType.PostIncrement || expressionType == ExpressionType.PostDecrement);

            if (expressionType == ExpressionType.PreIncrement || expressionType == ExpressionType.PostIncrement)
            {
                m_type = "+1";
                m_shortType = "++";
            }
            else
            {
                m_type = "-1";
                m_shortType = "--";
            }
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
            
            WriterDestination originalDestination = m_context.Writer.CurrentDestination;
            
            // Property
            if (isProperty)
            {
                // Set future value in post code
                bool doFutureValue = (m_isPost || m_context.Writer.ShouldOutputPost);
                if (doFutureValue)
                    m_context.Writer.CurrentDestination = WriterDestination.PostBuffer;

                string getter;
                if (symbol.IsStatic)
                {
                    getter = string.Format("{0}_{1}_getter()", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name);
                    m_context.Writer.Append(string.Format("{0}_{1}_setter({2}{3})", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, getter, m_type));
                }
                else
                {
                    string objectName = "obj";
                    IEnumerable<SyntaxNode> nodes = name.ChildNodes();
                    if (nodes.Count() > 1)
                        objectName = m_context.TypeConvert.ConvertVariableName(nodes.First());

                    getter = string.Format("{0}_{1}_getter({2})", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, objectName);
                    m_context.Writer.Append(string.Format("{0}_{1}_setter({4}, {2}{3})", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name, getter, m_type, objectName));
                }
                
                if (m_isPost)
                    m_context.Writer.CurrentDestination = originalDestination;

                if (doFutureValue)
                    m_context.Writer.Append(string.Format("{0}", getter));
            }
            // Variable
            else
            {
                WriterDestination destinationVariable = m_context.Writer.CurrentDestination;
                m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                m_context.Generators.Expression.Generate(name);
                string variable = m_context.Writer.FlushTempBuffer();
                m_context.Writer.CurrentDestination = destinationVariable;
                
                if (m_isPost)
                    m_context.Writer.Append(string.Format("{0}{1}", variable, m_shortType));
                else
                    m_context.Writer.Append(string.Format("{1}{0}", variable, m_shortType));
            }
        }
    }
}
