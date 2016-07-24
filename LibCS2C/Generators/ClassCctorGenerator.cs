using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    class ClassCctorGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private ClassCodeData m_classCode;

        /// <summary>
        /// Class .cctor generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="classCode">Class code</param>
        public ClassCctorGenerator(WalkerContext context, ClassCodeData classCode)
        {
            m_context = context;
            m_classCode = classCode;
        }

        /// <summary>
        /// Generates the initializer
        /// </summary>
        /// <param name="className">The converted class name</param>
        /// <param name="name">The name of the member</param>
        /// <param name="value">The value</param>
        private void GenerateInitializer(string className, string name, EqualsValueClauseSyntax value)
        {
            ExpressionSyntax expression = value.Value;

            // If it's a literal expression, it is already in the struct initializer
            if (!m_context.IsLiteralExpression(expression.Kind()))
            {
                m_context.Writer.Append(string.Format("\tclassStatics_{0}.{1} = ", className, name));
                m_context.Generators.Expression.Generate(expression);
                m_context.Writer.AppendLine(";");
            }
        }

        /// <summary>
        /// Generate the .cctor method of a class
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            // Are there even things to initialize?
            if(m_classCode.staticFields.Count() == 0 && m_classCode.propertyInitialValuesStatic.Count() == 0)
            {
                return;
            }

            string convertedClassName = m_context.ConvertClassName(node.Identifier.ToString());
            string methodName = string.Format("classCctor_{0}", convertedClassName);
            string methodPrototype = string.Format("inline void {0}(void)", methodName);

            // Add to .cctor list so we can call it on initialization
            m_context.CctorList.Add(methodName);

            // Prototype
            m_context.CurrentDestination = WriterDestination.MethodPrototypes;
            m_context.Writer.Append(methodPrototype);
            m_context.Writer.AppendLine(";");

            // Declaration
            m_context.CurrentDestination = WriterDestination.MethodDeclarations;
            m_context.Writer.AppendLine(methodPrototype);
            m_context.Writer.AppendLine("{");

            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_classCode.staticFields)
            {
                GenerateInitializer(convertedClassName, pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_classCode.propertyInitialValuesStatic)
            {
                GenerateInitializer(convertedClassName, "prop_" + pair.Key, pair.Value);
            }

            m_context.Writer.AppendLine("}");
        }
    }
}
