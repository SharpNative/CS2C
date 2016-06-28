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
    class IfStatementGenerator : GeneratorBase<IfStatementSyntax>
    {
        private ExpressionGenerator m_expressionGen;


        /// <summary>
        /// If statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public IfStatementGenerator(WalkerContext context)
        {
            m_context = context;
            m_expressionGen = new ExpressionGenerator(m_context);
        }

        /// <summary>
        /// Internal generator
        /// </summary>
        /// <param name="node"></param>
        /// <param name="first"></param>
        private void GenerateInternal(IfStatementSyntax node, bool first)
        {
            m_context.Writer.Append("if(");
            foreach (SyntaxNode childNode in node.ChildNodes())
            {
                SyntaxKind kind = childNode.Kind();

                if (kind == SyntaxKind.EqualsExpression)
                {
                    m_expressionGen.Generate(childNode as ExpressionSyntax);
                }
                else if (kind == SyntaxKind.LogicalAndExpression ||
                         kind == SyntaxKind.LogicalOrExpression ||
                         kind == SyntaxKind.LogicalNotExpression)
                {
                    ChildSyntaxList children = childNode.ChildNodesAndTokens();
                    foreach (SyntaxNodeOrToken child in children)
                    {
                        SyntaxKind childKind = child.Kind();
                        if (childKind == SyntaxKind.EqualsExpression)
                        {
                            m_expressionGen.Generate(child.AsNode() as ExpressionSyntax);
                        }
                        else
                        {
                            m_context.Writer.Append(child.ToString() + " ");
                        }
                    }
                }
                else if (kind == SyntaxKind.ElseClause)
                {
                    SyntaxNode firstNode = childNode.ChildNodes().First();
                    SyntaxKind childKind = firstNode.Kind();

                    if (childKind == SyntaxKind.IfStatement)
                    {
                        m_context.Writer.AppendLine("}");
                        m_context.Writer.Append("else ");
                        GenerateInternal(firstNode as IfStatementSyntax, false);
                    }
                    else
                    {
                        m_context.Writer.AppendLine("}");
                        m_context.Writer.AppendLine("else");
                        m_context.Writer.AppendLine("{");

                        BlockGenerator blockGen = new BlockGenerator(m_context);
                        blockGen.Generate(childNode.ChildNodes().First() as BlockSyntax);
                    }
                }
                else if (kind == SyntaxKind.Block)
                {
                    m_context.Writer.AppendLine(")");

                    m_context.Writer.AppendLine("{");
                    BlockGenerator blockGen = new BlockGenerator(m_context);
                    blockGen.Generate(childNode as BlockSyntax);
                }
            }
            
            if(first)
                m_context.Writer.AppendLine("}");
        }

        /// <summary>
        /// Generates an if statement
        /// </summary>
        /// <param name="node">The if statement</param>
        public override void Generate(IfStatementSyntax node)
        {
            GenerateInternal(node, true);
        }
    }
}
