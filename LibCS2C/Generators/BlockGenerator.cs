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
    class BlockGenerator : GeneratorBase<BlockSyntax>
    {
        private VariableGenerator m_variableGen;
        private ReturnStatementGenerator m_returnStatementGen;
        private ExpressionStatementGenerator m_expressionStatementGen;
        private LocalDeclarationGenerator m_localDeclarationGen;
        private IfStatementGenerator m_ifStatementGen;
        private ForStatementGenerator m_forStatementGen;
        private WhileStatementGenerator m_whileStatementGen;
        private FixedStatementGenerator m_fixedStatementGen;

        /// <summary>
        /// Block generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public BlockGenerator(WalkerContext context)
        {
            m_context = context;
            m_variableGen = new VariableGenerator(m_context);
            m_returnStatementGen = new ReturnStatementGenerator(m_context);
            m_expressionStatementGen = new ExpressionStatementGenerator(m_context);
            m_localDeclarationGen = new LocalDeclarationGenerator(m_context);
            m_ifStatementGen = new IfStatementGenerator(m_context);
            m_forStatementGen = new ForStatementGenerator(m_context);
            m_whileStatementGen = new WhileStatementGenerator(m_context);
            m_fixedStatementGen = new FixedStatementGenerator(m_context);
        }

        /// <summary>
        /// Generate the child node
        /// </summary>
        /// <param name="childNode">The child node</param>
        public void GenerateChildren(SyntaxNode childNode)
        {
            switch (childNode.Kind())
            {
                case SyntaxKind.VariableDeclaration:
                    m_variableGen.Generate(childNode as VariableDeclarationSyntax);
                    break;

                case SyntaxKind.ReturnStatement:
                    m_returnStatementGen.Generate(childNode as ReturnStatementSyntax);
                    break;

                case SyntaxKind.ExpressionStatement:
                    m_expressionStatementGen.Generate(childNode as ExpressionStatementSyntax);
                    break;

                case SyntaxKind.LocalDeclarationStatement:
                    m_localDeclarationGen.Generate(childNode as LocalDeclarationStatementSyntax);
                    break;

                case SyntaxKind.IfStatement:
                    m_ifStatementGen.Generate(childNode as IfStatementSyntax);
                    break;

                case SyntaxKind.ForStatement:
                    m_forStatementGen.Generate(childNode as ForStatementSyntax);
                    break;

                case SyntaxKind.WhileStatement:
                    m_whileStatementGen.Generate(childNode as WhileStatementSyntax);
                    break;

                case SyntaxKind.FixedStatement:
                    m_fixedStatementGen.Generate(childNode as FixedStatementSyntax);
                    break;

                case SyntaxKind.Block:
                    m_context.Writer.AppendLine("{");
                    Generate(childNode as BlockSyntax);
                    m_context.Writer.AppendLine("}");
                    break;

                default:
                    throw new NotImplementedException();
            }

            // At the end of the line
            m_context.Writer.AppendLine(";");
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
                GenerateChildren(childNode);
            }

            m_context.Writer.UnIndent();
        }
    }
}
