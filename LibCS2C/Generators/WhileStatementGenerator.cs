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
    class WhileStatementGenerator : GeneratorBase<WhileStatementSyntax>
    {
        private ExpressionGenerator m_expressionGen;
        
        /// <summary>
        /// While statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public WhileStatementGenerator(WalkerContext context)
        {
            m_context = context;
            m_expressionGen = new ExpressionGenerator(m_context);
        }
        
        /// <summary>
        /// Generates a while statement
        /// </summary>
        /// <param name="node">The while statement</param>
        public override void Generate(WhileStatementSyntax node)
        {
            m_context.Writer.Append("while(");

            m_expressionGen.Generate(node.Condition);

            m_context.Writer.AppendLine(")");
            
            m_context.Writer.AppendLine("{");
            m_context.Writer.Indent();

            IEnumerable<SyntaxNode> children = node.ChildNodes();
            foreach(SyntaxNode child in children)
            {
                if(child.Kind() == SyntaxKind.Block)
                {
                    BlockGenerator blockGen = new BlockGenerator(m_context);
                    blockGen.Generate(child as BlockSyntax);
                    break;
                }
            }
            
            m_context.Writer.UnIndent();
            m_context.Writer.AppendLine("}");
        }
    }
}
