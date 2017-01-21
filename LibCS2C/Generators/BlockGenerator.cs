using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace LibCS2C.Generators
{
    public class BlockGenerator : GeneratorBase<BlockSyntax>
    {
        /// <summary>
        /// Block generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public BlockGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generate the child node
        /// </summary>
        /// <param name="childNode">The child node</param>
        public void GenerateChild(SyntaxNode childNode)
        {
            bool semiColonNeeded = true;
            switch (childNode.Kind())
            {
                case SyntaxKind.VariableDeclaration:
                    m_context.Generators.Variable.Generate(childNode as VariableDeclarationSyntax);
                    break;

                case SyntaxKind.ReturnStatement:
                    m_context.Generators.ReturnStatement.Generate(childNode as ReturnStatementSyntax);
                    break;

                case SyntaxKind.ExpressionStatement:
                    m_context.Generators.ExpressionStatement.Generate(childNode as ExpressionStatementSyntax);
                    break;

                case SyntaxKind.LocalDeclarationStatement:
                    m_context.Generators.LocalDeclaration.Generate(childNode as LocalDeclarationStatementSyntax);
                    break;

                case SyntaxKind.IfStatement:
                    m_context.Generators.IfStatement.Generate(childNode as IfStatementSyntax);
                    semiColonNeeded = false;
                    break;

                case SyntaxKind.ForStatement:
                    m_context.Generators.ForStatement.Generate(childNode as ForStatementSyntax);
                    break;

                case SyntaxKind.WhileStatement:
                    m_context.Generators.WhileStatement.Generate(childNode as WhileStatementSyntax);
                    break;

                case SyntaxKind.FixedStatement:
                    m_context.Generators.FixedStatement.Generate(childNode as FixedStatementSyntax);
                    semiColonNeeded = false;
                    break;

                case SyntaxKind.Block:
                    m_context.Writer.AppendLine("{");
                    Generate(childNode as BlockSyntax);
                    m_context.Writer.AppendLine("}");
                    semiColonNeeded = false;
                    break;

                case SyntaxKind.DoStatement:
                    m_context.Generators.DoStatement.Generate(childNode as DoStatementSyntax);
                    break;

                case SyntaxKind.ContinueStatement:
                    m_context.Writer.Append("continue");
                    break;

                case SyntaxKind.BreakStatement:
                    m_context.Writer.Append("break");
                    break;

                case SyntaxKind.SwitchStatement:
                    m_context.Generators.SwitchStatement.Generate(childNode as SwitchStatementSyntax);
                    semiColonNeeded = false;
                    break;

                case SyntaxKind.EmptyStatement:
                    semiColonNeeded = false;
                    break;

                case SyntaxKind.IdentifierName:
                    m_context.Generators.IdentifierName.Generate(childNode as IdentifierNameSyntax);
                    semiColonNeeded = false;
                    break;

                case SyntaxKind.GotoStatement:
                    m_context.Generators.GotoStatement.Generate(childNode as GotoStatementSyntax);
                    semiColonNeeded = true;
                    break;

                case SyntaxKind.LabeledStatement:
                    m_context.Generators.LabeledStatement.Generate(childNode as LabeledStatementSyntax);
                    break;

                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                    m_context.Generators.checkedStatement.Generate(childNode as CheckedStatementSyntax);
                    break;

                default:
                    throw new NotImplementedException("Unknown SyntaxKind in Block: " + childNode.Kind());
            }

            // At the end of the line
            if (semiColonNeeded)
                m_context.Writer.AppendLine(";");

            if (!m_context.Writer.IsPostBufferEmpty())
                m_context.Writer.AppendLine(string.Format("{0};", m_context.Writer.FlushPostBuffer()));
        }

        /// <summary>
        /// Generates a block
        /// </summary>
        /// <param name="node">The block</param>
        public override void Generate(BlockSyntax node)
        {
            m_context.Writer.Indent();

            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (SyntaxNode childNode in nodes)
            {
                GenerateChild(childNode);
            }

            m_context.Writer.UnIndent();
        }
    }
}
