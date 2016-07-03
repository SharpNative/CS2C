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
    class DoStatementGenerator : GeneratorBase<DoStatementSyntax>
    {
        private ExpressionGenerator m_expressionGen;
        
        /// <summary>
        /// Do statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public DoStatementGenerator(WalkerContext context)
        {
            m_context = context;
            m_expressionGen = new ExpressionGenerator(m_context);
        }
        
        /// <summary>
        /// Generates a do statement
        /// </summary>
        /// <param name="node">The do statement</param>
        public override void Generate(DoStatementSyntax node)
        {
            m_context.Writer.AppendLine("do");
            m_context.Writer.AppendLine("{");

            BlockSyntax block = (from a in node.ChildNodes()
                                 where a.Kind() == SyntaxKind.Block
                                 select a).FirstOrDefault() as BlockSyntax;

            BlockGenerator blockGen = new BlockGenerator(m_context);
            blockGen.Generate(block);

            m_context.Writer.AppendLine("}");
            m_context.Writer.Append("while(");
            m_expressionGen.Generate(node.Condition);
            m_context.Writer.Append(")");
        }
    }
}
