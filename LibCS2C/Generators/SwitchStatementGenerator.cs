using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibCS2C.Generators
{
    public class SwitchStatementGenerator : GeneratorBase<SwitchStatementSyntax>
    {
        /// <summary>
        /// Switch generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public SwitchStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a switch
        /// </summary>
        /// <param name="node">The switch</param>
        public override void Generate(SwitchStatementSyntax node)
        {
            m_context.Writer.Append("switch(");
            m_context.Generators.Expression.Generate(node.Expression);
            m_context.Writer.AppendLine(")");
            m_context.Writer.AppendLine("{");
            m_context.Writer.Indent();

            SyntaxList<SwitchSectionSyntax> sections = node.Sections;
            foreach (SwitchSectionSyntax section in sections)
            {
                // Labels for this section
                SyntaxList<SwitchLabelSyntax> labels = section.Labels;
                foreach (SwitchLabelSyntax label in labels)
                {
                    SyntaxKind labelKind = label.Kind();
                    if (labelKind == SyntaxKind.CaseSwitchLabel)
                    {
                        CaseSwitchLabelSyntax caseLabel = label as CaseSwitchLabelSyntax;
                        m_context.Writer.Append("case (");
                        m_context.Generators.Expression.Generate(caseLabel.Value);
                        m_context.Writer.AppendLine("):");
                    }
                    else /* if(labelKind==SyntaxKind.DefaultSwitchLabel) */
                    {
                        m_context.Writer.AppendLine("default:");
                    }
                }

                m_context.Writer.Indent();

                // Statement for this section
                SyntaxList<StatementSyntax> statements = section.Statements;
                foreach (StatementSyntax statement in statements)
                {
                    SyntaxKind kind = statement.Kind();
                    
                    if (kind == SyntaxKind.Block)
                    {
                        m_context.Writer.AppendLine("{");
                        m_context.Generators.Block.Generate(statement as BlockSyntax);
                        m_context.Writer.AppendLine("}");
                    }
                    else if(m_context.IsSubExpression(kind))
                    {
                        m_context.Generators.Expression.Generate(statement as ExpressionStatementSyntax);
                        m_context.Writer.AppendLine("");
                    }
                    else
                    {
                        m_context.Generators.Block.GenerateChild(statement);
                    }
                }

                m_context.Writer.UnIndent();
            }

            m_context.Writer.UnIndent();
            m_context.Writer.AppendLine("}");
        }
    }
}
