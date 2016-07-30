using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace LibCS2C.Generators
{
    public class FixedStatementGenerator : GeneratorBase<FixedStatementSyntax>
    {
        /// <summary>
        /// Fixed statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public FixedStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }
        
        /// <summary>
        /// Generates a fixed statement
        /// </summary>
        /// <param name="node">The fixed statement</param>
        public override void Generate(FixedStatementSyntax node)
        {
            m_context.Writer.AppendLine("{");
            m_context.Writer.Indent();
            
            IEnumerable<SyntaxNode> children = node.ChildNodes();
            foreach (SyntaxNode child in children)
            {
                m_context.Generators.Block.GenerateChild(child);
            }

            m_context.Writer.UnIndent();
            m_context.Writer.AppendLine("}");
        }
    }
}
