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
    public class ForStatementGenerator : GeneratorBase<ForStatementSyntax>
    {
        /// <summary>
        /// For statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ForStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a for statement
        /// </summary>
        /// <param name="node">The for statement</param>
        public override void Generate(ForStatementSyntax node)
        {
            m_context.Writer.Append("for(");

            if (node.Declaration != null)
                m_context.Generators.Variable.Generate(node.Declaration);
            
            m_context.Writer.Append(";");

            if (node.Condition != null)
                m_context.Generators.Expression.Generate(node.Condition);

            m_context.Writer.Append(";");

            SeparatedSyntaxList<ExpressionSyntax> nodes = node.Incrementors;
            foreach (ExpressionSyntax expression in nodes)
            {
                m_context.Generators.Expression.Generate(expression);
            }

            m_context.Writer.AppendLine(")");

            m_context.Writer.AppendLine("{");

            IEnumerable<SyntaxNode> children = node.ChildNodes();
            foreach (SyntaxNode child in children)
            {
                if (child.Kind() == SyntaxKind.Block)
                {
                    m_context.Generators.Block.Generate(child as BlockSyntax);
                    break;
                }
            }
            
            m_context.Writer.AppendLine("}");
        }
    }
}
