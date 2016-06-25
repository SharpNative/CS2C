using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
{
    class ClassInitGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private Dictionary<string, EqualsValueClauseSyntax> m_nonStaticFields;

        /// <summary>
        /// Class struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ClassInitGenerator(WalkerContext context, Dictionary<string, EqualsValueClauseSyntax> nonStaticFields)
        {
            m_context = context;
            m_nonStaticFields = nonStaticFields;
        }

        public override void Generate(ClassDeclarationSyntax node)
        {
            // Class initialization method: returns a pointer to this object
            m_context.Writer.AppendLine("");
            m_context.Writer.AppendLine(string.Format("{0}* classInit_{1}_{2}(void)", m_context.CurrentClassStructName, m_context.CurrentNamespaceFormatted, node.Identifier));
            m_context.Writer.AppendLine("{");
            m_context.Writer.AppendLine(string.Format("\t{0}* object = malloc(sizeof({0}));", m_context.CurrentClassStructName));
            m_context.Writer.AppendLine("\tif(!object)");
            m_context.Writer.AppendLine("\t\treturn NULL;");

            // Loop through the fields and initialize them
            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_nonStaticFields)
            {
                m_context.Writer.AppendLine(string.Format("\tobject->{0} {1};", pair.Key, pair.Value));
            }

            m_context.Writer.AppendLine("\treturn object;");
            m_context.Writer.AppendLine("}");
            m_context.Writer.AppendLine("");
        }
    }
}
