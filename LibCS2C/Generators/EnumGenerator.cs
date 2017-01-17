using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> nodes = node.Members;

            string lastValue = "0";
            foreach (EnumMemberDeclarationSyntax child in nodes)
            {
                // Enum values are always ints
                string identifier = child.Identifier.ToString();
                string currentValue = lastValue;
                if (child.EqualsValue != null)
                {
                    currentValue = lastValue = child.EqualsValue.Value.ToString();
                }

                lastValue += "+1";
                if (insideClass)
                    m_context.Writer.Append(string.Format("#define enum_{0}_{1}_{2}", m_context.TypeConvert.CurrentClassNameFormatted, node.Identifier, identifier));
                else
                    m_context.Writer.Append(string.Format("#define enum_{0}_{1}_{2}", m_context.TypeConvert.CurrentNamespaceFormatted, node.Identifier, identifier));

                m_context.Writer.Append(" (");
                m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                m_context.Writer.Append(currentValue.ToString());
                m_context.Writer.CurrentDestination = WriterDestination.Defines;
                m_context.Writer.Append(m_context.Writer.FlushTempBuffer());
                m_context.Writer.AppendLine(")");
            }

            m_context.Writer.CurrentDestination = destination;
        }
    }
}
