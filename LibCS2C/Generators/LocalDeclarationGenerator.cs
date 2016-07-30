using LibCS2C.Context;
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
    public class LocalDeclarationGenerator : GeneratorBase<LocalDeclarationStatementSyntax>
    {
        /// <summary>
        /// Local declaration generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public LocalDeclarationGenerator(WalkerContext context)
        {
            m_context = context;
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
                m_context.Generators.Variable.Generate(child);
            }
        }
    }
}
