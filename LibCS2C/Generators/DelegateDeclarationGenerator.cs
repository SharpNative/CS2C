using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibCS2C.Generators
{
    public class DelegateDeclarationGenerator : GeneratorBase<DelegateDeclarationSyntax>
    {
        /// <summary>
        /// Delegate declaration generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public DelegateDeclarationGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a delegate declaration
        /// </summary>
        /// <param name="node">The delegate</param>
        public override void Generate(DelegateDeclarationSyntax node)
        {
            WriterDestination destination = m_context.Writer.CurrentDestination;
            m_context.Writer.CurrentDestination = WriterDestination.Delegates;

            m_context.Writer.Append(string.Format("typedef {0} (*delegate_{1}_{2}) (", m_context.ConvertTypeName(node.ReturnType), m_context.TypeConvert.CurrentClassNameFormatted, node.Identifier));

            IEnumerable<SyntaxNode> paramNodes = node.ParameterList.ChildNodes();
            foreach (ParameterSyntax paramNode in paramNodes)
            {
                m_context.Writer.Append(string.Format("{0} {1}", m_context.ConvertTypeName(paramNode.Type), paramNode.Identifier));

                // A comma if it's not the last parameter
                if (paramNode != paramNodes.Last())
                    m_context.Writer.Append(", ");
            }

            m_context.Writer.AppendLine(");");

            m_context.Writer.CurrentDestination = destination;
        }
    }
}
