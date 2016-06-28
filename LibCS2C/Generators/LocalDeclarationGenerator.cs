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
    class LocalDeclarationGenerator : GeneratorBase<LocalDeclarationStatementSyntax>
    {
        private VariableGenerator m_variableGen;

        /// <summary>
        /// Local declaration generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public LocalDeclarationGenerator(WalkerContext context)
        {
            m_context = context;
            m_variableGen = new VariableGenerator(m_context);
        }

        /// <summary>
        /// Generates the local declaration
        /// </summary>
        /// <param name="node">The local declaration node</param>
        public override void Generate(LocalDeclarationStatementSyntax node)
        {
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (VariableDeclarationSyntax child in nodes)
            {
                m_variableGen.Generate(child);
            }
        }
    }
}
