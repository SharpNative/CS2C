using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
{
    class ClassStructGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private Dictionary<string, TypeSyntax> m_nonStaticFieldTypes;
        private Dictionary<string, TypeSyntax> m_propertyTypes;

        /// <summary>
        /// Class struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ClassStructGenerator(WalkerContext context, Dictionary<string, TypeSyntax> nonStaticFieldTypes, Dictionary<string, TypeSyntax> propertyTypes)
        {
            m_context = context;
            m_nonStaticFieldTypes = nonStaticFieldTypes;
            m_propertyTypes = propertyTypes;
        }

        /// <summary>
        /// Generates the class struct
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            m_context.Writer.AppendLine(m_context.CurrentClassStructName);
            m_context.Writer.AppendLine("{");

            // Usage count for garbage collector
            m_context.Writer.AppendLine("\tint32_t usage_count;");

            foreach (KeyValuePair<string, TypeSyntax> pair in m_nonStaticFieldTypes)
            {
                ISymbol symbol = m_context.Model.GetSymbolInfo(pair.Value).Symbol;
                m_context.Writer.AppendLine("\t/* Field: " + pair.Key + " */");
                m_context.Writer.AppendLine(string.Format("\t{0} field_{1};", m_context.ConvertTypeName(pair.Value), pair.Key));
            }

            foreach (KeyValuePair<string, TypeSyntax> pair in m_propertyTypes)
            {
                ISymbol symbol = m_context.Model.GetSymbolInfo(pair.Value).Symbol;
                m_context.Writer.AppendLine("\t/* Property: " + pair.Key + " */");
                m_context.Writer.AppendLine(string.Format("\t{0} prop_{1};", m_context.ConvertTypeName(pair.Value), pair.Key));
            }

            m_context.Writer.AppendLine("};");
            m_context.Writer.AppendLine("");
        }
    }
}
