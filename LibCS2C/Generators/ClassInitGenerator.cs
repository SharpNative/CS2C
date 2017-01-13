using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace LibCS2C.Generators
{
    class ClassInitGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private ClassCodeData m_classCode;

        /// <summary>
        /// Class struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="classCode">Class code</param>
        public ClassInitGenerator(WalkerContext context, ClassCodeData classCode)
        {
            m_context = context;
            m_classCode = classCode;
        }

        /// <summary>
        /// Generates the class initialization method
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            string methodPrototype = string.Format("void* classInit_{0}_{1}(void)", m_context.TypeConvert.CurrentNamespaceFormatted, node.Identifier);

            // Method prototype
            m_context.Writer.CurrentDestination = WriterDestination.MethodPrototypes;
            m_context.Writer.Append(methodPrototype);
            m_context.Writer.AppendLine(";");

            // Method declaration
            // Class initialization method: returns a pointer to this object
            m_context.Writer.CurrentDestination = WriterDestination.MethodDeclarations;
            m_context.Writer.AppendLine(methodPrototype);
            m_context.Writer.AppendLine("{");
            m_context.Writer.AppendLine(string.Format("\t{0}* object = calloc(1, sizeof({0}));", m_context.TypeConvert.CurrentClassStructName));
            m_context.Writer.AppendLine("\tif(!object) return NULL;");
            
            // For indirect function calls
            m_context.Writer.AppendLine(string.Format("\tobject->lookup_table = methods_{0};", m_context.TypeConvert.CurrentClassNameFormatted));

            // Loop through the fields and initialize them
            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_classCode.nonStaticFields)
            {
                m_context.Writer.Append(string.Format("\tobject->field_{0} = ", pair.Key));
                ExpressionSyntax expression = pair.Value.Value;
                m_context.Generators.Expression.Generate(expression);
                m_context.Writer.AppendLine(";");
            }

            // Loop through the properties and initialize them
            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_classCode.propertyInitialValuesNonStatic)
            {
                m_context.Writer.Append(string.Format("\tobject->prop_{0} = ", pair.Key));
                ExpressionSyntax expression = pair.Value.Value;
                m_context.Generators.Expression.Generate(expression);
                m_context.Writer.AppendLine(";");
            }

            m_context.Writer.AppendLine("\treturn object;");
            m_context.Writer.AppendLine("}");
            m_context.Writer.AppendLine("");
        }
    }
}
