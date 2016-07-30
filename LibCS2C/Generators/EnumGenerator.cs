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
    public class EnumGenerator : GeneratorBase<EnumDeclarationSyntax>
    {
        /// <summary>
        /// Enum statement generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public EnumGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates an enum
        /// </summary>
        /// <param name="node">The enum</param>
        public override void Generate(EnumDeclarationSyntax node)
        {
            WriterDestination destination = m_context.Writer.CurrentDestination;
            m_context.Writer.CurrentDestination = WriterDestination.Enums;

            bool insideClass = (node.Parent is ClassDeclarationSyntax);
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();

            foreach(EnumMemberDeclarationSyntax child in nodes)
            {
                string identifier = child.Identifier.ToString();
                string value = child.EqualsValue.Value.ToString();

                if(insideClass)
                    m_context.Writer.AppendLine(string.Format("#define enum_{0}_{1}_{2} ({3})", m_context.TypeConvert.CurrentClassNameFormatted, node.Identifier, identifier, value));
                else
                    m_context.Writer.AppendLine(string.Format("#define enum_{0}_{1}_{2}", m_context.CurrentNamespaceFormatted, node.Identifier, identifier, value));
            }

            m_context.Writer.CurrentDestination = destination;
        }
    }
}
