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
        }

        /// <summary>
        /// Generates a block
        /// </summary>
        /// <param name="node">The block</param>
        public override void Generate(BlockSyntax node)
        {
            m_context.Writer.Indent();
            
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();

            foreach(SyntaxNode childNode in nodes)
            {
                SyntaxKind kind = childNode.Kind();

                if(kind == SyntaxKind.VariableDeclaration)
                {
                    m_variableGen.Generate(childNode as VariableDeclarationSyntax);
                }
                else if(kind == SyntaxKind.ReturnStatement)
                {
                    m_returnStatementGen.Generate(childNode as ReturnStatementSyntax);
                }
                else if (kind == SyntaxKind.ExpressionStatement)
                {
                    m_expressionStatementGen.Generate(childNode as ExpressionStatementSyntax);
                }
                else if(kind == SyntaxKind.LocalDeclarationStatement)
                {
                    m_localDeclarationGen.Generate(childNode as LocalDeclarationStatementSyntax);
                }
                else if(kind == SyntaxKind.IfStatement)
                {
                    m_ifStatementGen.Generate(childNode as IfStatementSyntax);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            m_context.Writer.UnIndent();
        }
    }
}
