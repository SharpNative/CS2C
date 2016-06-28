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
        private Dictionary<string, EqualsValueClauseSyntax> m_propertyInitialValues;
        private ExpressionGenerator m_expressionGen;

        /// <summary>
        /// Class struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ClassInitGenerator(WalkerContext context, Dictionary<string, EqualsValueClauseSyntax> nonStaticFields, Dictionary<string, EqualsValueClauseSyntax> propertyInitialValues)
        {
            m_context = context;
            m_nonStaticFields = nonStaticFields;
            m_propertyInitialValues = propertyInitialValues;
            m_expressionGen = new ExpressionGenerator(m_context);
        }

        /// <summary>
        /// Generates the class initialization method
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            // Class initialization method: returns a pointer to this object
            m_context.Writer.AppendLine("");
            m_context.Writer.AppendLine(string.Format("{0}* classInit_{1}_{2}(void)", m_context.CurrentClassStructName, m_context.CurrentNamespaceFormatted, node.Identifier));
            m_context.Writer.AppendLine("{");
            m_context.Writer.AppendLine(string.Format("\t{0}* object = malloc(sizeof({0}));", m_context.CurrentClassStructName));
            m_context.Writer.AppendLine("\tif(!object)");
            m_context.Writer.AppendLine("\t\treturn NULL;");

            // For the garbage collector
            m_context.Writer.AppendLine("\tobject->usage_count = 1;");

            // Loop through the fields and initialize them
            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_nonStaticFields)
            {
                m_context.Writer.Append(string.Format("\tobject->field_{0} = ", pair.Key));
                ExpressionSyntax expression = pair.Value.Value;
                m_expressionGen.Generate(expression);
                m_context.Writer.AppendLine(";");
            }

            // Loop through the properties and initialize them
            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_propertyInitialValues)
            {
                m_context.Writer.Append(string.Format("\tobject->prop_{0} = ", pair.Key));
                ExpressionSyntax expression = pair.Value.Value;
                m_expressionGen.Generate(expression);
                m_context.Writer.AppendLine(";");
            }

            m_context.Writer.AppendLine("\treturn object;");
            m_context.Writer.AppendLine("}");
            m_context.Writer.AppendLine("");
        }
    }
}
