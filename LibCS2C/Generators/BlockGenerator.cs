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
        public void GenerateChildren(SyntaxNode childNode)
        {
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
                    break;

                case SyntaxKind.ForStatement:
                    m_context.Generators.ForStatement.Generate(childNode as ForStatementSyntax);
                    break;

                case SyntaxKind.WhileStatement:
                    m_context.Generators.WhileStatement.Generate(childNode as WhileStatementSyntax);
                    break;

                case SyntaxKind.FixedStatement:
                    m_context.Generators.FixedStatement.Generate(childNode as FixedStatementSyntax);
                    break;

                case SyntaxKind.Block:
                    m_context.Writer.AppendLine("{");
                    Generate(childNode as BlockSyntax);
                    m_context.Writer.AppendLine("}");
                    break;

                case SyntaxKind.DoStatement:
                    m_context.Generators.DoStatement.Generate(childNode as DoStatementSyntax);
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
