using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
{
    class ReturnStatementGenerator : GeneratorBase<ReturnStatementSyntax>
    {
        /// <summary>
        /// Return statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ReturnStatementGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates the return statement code
        /// </summary>
        /// <param name="node">The return statement</param>
        public override void Generate(ReturnStatementSyntax node)
        {
            m_context.Writer.AppendIndent();
            m_context.Writer.Append("return ");

            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(SyntaxNode childNode in nodes)
            {
                SyntaxKind kind = childNode.Kind();
                if(kind == SyntaxKind.IdentifierName)
                {
                    m_context.Writer.Append(m_context.ConvertVariableName(childNode));
                }
                else
                {
                    m_context.Writer.Append(childNode.ToString());
                }
            }

            m_context.Writer.AppendLine(";");
        }
    }
}
