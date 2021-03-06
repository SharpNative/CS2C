﻿using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace LibCS2C.Generators
{
    public class IfStatementGenerator : GeneratorBase<IfStatementSyntax>
    {
        /// <summary>
        /// If statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public IfStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }
        
        /// <summary>
        /// Generates an if statement
        /// </summary>
        /// <param name="node">The if statement</param>
        public override void Generate(IfStatementSyntax node)
        {
            ChildSyntaxList list = node.ChildNodesAndTokens();
            
            foreach (SyntaxNodeOrToken child in list)
            {
                SyntaxKind kind = child.Kind();

                // Node
                if (child.IsNode)
                {
                    SyntaxNode childNode = child.AsNode();

                    if (m_context.Generators.Expression.IsSubExpression(kind))
                    {
                        m_context.Generators.Expression.Generate(childNode);
                    }
                    else if (kind == SyntaxKind.ElseClause)
                    {
                        SyntaxNode firstNode = childNode.ChildNodes().First();
                        SyntaxKind childKind = firstNode.Kind();

                        if (childKind == SyntaxKind.IfStatement)
                        {
                            m_context.Writer.Append("else ");
                            Generate(firstNode as IfStatementSyntax);
                        }
                        else
                        {
                            m_context.Writer.AppendLine("else");

                            SyntaxNode first = childNode.ChildNodes().First();
                            SyntaxKind firstKind = first.Kind();

                            m_context.Writer.AppendLine("{");
                            if (firstKind == SyntaxKind.Block)
                            {
                                m_context.Generators.Block.Generate(first as BlockSyntax);
                            }
                            else
                            {
                                m_context.Writer.Indent();
                                m_context.Generators.Block.GenerateChild(first);
                                m_context.Writer.UnIndent();
                            }
                            m_context.Writer.AppendLine("}");
                        }
                    }
                    else if (kind == SyntaxKind.Block)
                    {
                        m_context.Writer.AppendLine("{");
                        m_context.Generators.Block.Generate(childNode as BlockSyntax);
                        m_context.Writer.AppendLine("}");
                    }
                    else
                    {
                        m_context.Writer.AppendLine("");
                        m_context.Writer.AppendIndent();
                        m_context.Generators.Block.GenerateChild(childNode);
                    }
                }
                // Token, just emit it
                else
                {
                    m_context.Writer.Append(child.ToString());
                }
            }
        }
    }
}
