using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    class ClassStaticStructGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private Dictionary<string, TypeSyntax> m_staticFieldTypes;
        private Dictionary<string, TypeSyntax> m_staticPropertyTypes;

        /// <summary>
        /// Class struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ClassStaticStructGenerator(WalkerContext context, Dictionary<string, TypeSyntax> staticFieldTypes, Dictionary<string, TypeSyntax> staticPropertyTypes)
        {
            m_context = context;
            m_staticFieldTypes = staticFieldTypes;
            m_staticPropertyTypes = staticPropertyTypes;
        }

        /// <summary>
        /// Generates the static fields class struct
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            string convertedClassName = m_context.ConvertClassName(node.Identifier.ToString());

            m_context.Writer.AppendLine("struct");
            m_context.Writer.AppendLine("{");

            foreach (KeyValuePair<string, TypeSyntax> pair in m_staticFieldTypes)
            {
                m_context.Writer.AppendLine("\t/* Static Field: " + pair.Key + " */");
                m_context.Writer.AppendLine(string.Format("\t{0} {1};", m_context.ConvertTypeName(pair.Value), pair.Key));
            }

            foreach (KeyValuePair<string, TypeSyntax> pair in m_staticPropertyTypes)
            {
                m_context.Writer.AppendLine("\t/* Static Property: " + pair.Key + " */");
                m_context.Writer.AppendLine(string.Format("\t{0} prop_{1};", m_context.ConvertTypeName(pair.Value), pair.Key));
            }

            m_context.Writer.Append("}");
            m_context.Writer.AppendLine(string.Format(" classStatics_{0};", convertedClassName));
            m_context.Writer.AppendLine("");
        }
    }
}
