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
    class ForStatementGenerator : GeneratorBase<ForStatementSyntax>
    {
        private ExpressionGenerator m_expressionGen;
        private VariableGenerator m_variableGen;
        
        /// <summary>
        /// If statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ForStatementGenerator(WalkerContext context)
        {
            m_context = context;
            m_expressionGen = new ExpressionGenerator(m_context);
            m_variableGen = new VariableGenerator(m_context);
        }
        
        /// <summary>
        /// Generates an if statement
        /// </summary>
        /// <param name="node">The if statement</param>
        public override void Generate(ForStatementSyntax node)
        {
            m_context.Writer.Append("for(");

            if(node.Declaration != null)
                m_variableGen.Generate(node.Declaration);
            else
                m_context.Writer.Append(";");

            if(node.Condition != null)
                m_expressionGen.Generate(node.Condition);
            
            m_context.Writer.Append(";");

            SeparatedSyntaxList<ExpressionSyntax> nodes = node.Incrementors;
            foreach (ExpressionSyntax expression in nodes)
            {
                m_expressionGen.Generate(expression);
                m_context.Writer.RemoveLastChars(3);
            }

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
