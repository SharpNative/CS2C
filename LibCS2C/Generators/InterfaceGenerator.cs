using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace LibCS2C.Generators
{
    public class InterfaceGenerator : GeneratorBase<InterfaceDeclarationSyntax>
    {
        /// <summary>
        /// interface generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public InterfaceGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates an interface declaration
        /// </summary>
        /// <param name="node">The interface declaration</param>
        public override void Generate(InterfaceDeclarationSyntax node)
        {
            string structName = "struct " + m_context.TypeConvert.ConvertClassName(node.Identifier.ToString());

            m_context.Writer.CurrentDestination = WriterDestination.ClassStructs;
            m_context.Writer.AppendLine(structName);
            m_context.Writer.AppendLine("{");

            // For method lookup at runtime
            m_context.Writer.AppendLine("void** lookup_table;");

            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (SyntaxNode child in nodes)
            {
                if (child.Kind() == SyntaxKind.PropertyDeclaration)
                {
                    PropertyDeclarationSyntax property = child as PropertyDeclarationSyntax;
                    m_context.Writer.AppendLine(string.Format("{0} prop_{1};", m_context.ConvertTypeName(property.Type), property.Identifier));
                }
            }

            m_context.Writer.AppendLine("};");

            m_context.Writer.CurrentDestination = WriterDestination.StructPrototypes;
            m_context.Writer.AppendLine(structName + ";");
        }
    }
}
