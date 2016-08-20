using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

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
            m_context.Writer.CurrentDestination = WriterDestination.Defines;

            bool insideClass = (node.Parent is ClassDeclarationSyntax);
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();

            foreach (EnumMemberDeclarationSyntax child in nodes)
            {
                string identifier = child.Identifier.ToString();
                ExpressionSyntax value = child.EqualsValue.Value;

                if (insideClass)
                    m_context.Writer.Append(string.Format("#define enum_{0}_{1}_{2}", m_context.TypeConvert.CurrentClassNameFormatted, node.Identifier, identifier));
                else
                    m_context.Writer.Append(string.Format("#define enum_{0}_{1}_{2}", m_context.TypeConvert.CurrentNamespaceFormatted, node.Identifier, identifier));

                m_context.Writer.Append(" (");
                m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                m_context.Generators.Expression.Generate(value);
                m_context.Writer.CurrentDestination = WriterDestination.Defines;
                m_context.Writer.Append(m_context.Writer.FlushTempBuffer());
                m_context.Writer.AppendLine(")");
            }

            m_context.Writer.CurrentDestination = destination;
        }
    }
}
