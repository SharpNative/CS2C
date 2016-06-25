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

        /// <summary>
        /// Class struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ClassStructGenerator(WalkerContext context, Dictionary<string, TypeSyntax> nonStaticFieldTypes)
        {
            m_context = context;
            m_nonStaticFieldTypes = nonStaticFieldTypes;
        }

        public override void Generate(ClassDeclarationSyntax node)
        {
            m_context.Writer.AppendLine(m_context.CurrentClassStructName);
            m_context.Writer.AppendLine("{");

            foreach (KeyValuePair<string, TypeSyntax> pair in m_nonStaticFieldTypes)
            {
                m_context.Writer.AppendLine("\t/* Field: " + pair.Key + " */");
                m_context.Writer.AppendLine(string.Format("\t{0} {1};", m_context.ConvertTypeName(pair.Value), pair.Key));
            }

            m_context.Writer.AppendLine("};");
            m_context.Writer.AppendLine("");
        }
    }
}
