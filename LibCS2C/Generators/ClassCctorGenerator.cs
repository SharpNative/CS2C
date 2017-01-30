using LibCS2C.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

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
        /// Checks if we need to generator Cctor code
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>If we need Cctor code</returns>
        private bool needCctorCode(EqualsValueClauseSyntax value)
        {
            // If it's a literal expression, it is already in the struct initializer
            ExpressionSyntax expression = value.Value;
            return !m_context.Generators.Expression.IsLiteralExpression(expression.Kind());
        }

        /// <summary>
        /// Generates the initializer
        /// </summary>
        /// <param name="className">The converted class name</param>
        /// <param name="name">The name of the member</param>
        /// <param name="value">The value</param>
        private void GenerateInitializer(string className, string name, EqualsValueClauseSyntax value)
        {
            if (!needCctorCode(value))
                return;

            m_context.Writer.Append(string.Format("\tclassStatics_{0}.{1} = ", className, name));
            m_context.Generators.Expression.Generate(value.Value);
            m_context.Writer.AppendLine(";");
        }

        /// <summary>
        /// Generate the .cctor method of a class
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            // Are there even things to initialize in the cctor?
            bool need = false;

            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_classCode.staticFields)
            {
                if (needCctorCode(pair.Value))
                {
                    need = true;
                    break;
                }
            }

            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_classCode.propertyInitialValuesStatic)
            {
                if (needCctorCode(pair.Value))
                {
                    need = true;
                    break;
                }
            }

            if (!need)
                return;

            string convertedClassName = m_context.TypeConvert.ConvertClassName(node.Identifier.ToString());
            string methodName = string.Format("classCctor_{0}", convertedClassName);
            string methodPrototype = string.Format("static inline void {0}(void)", methodName);

            // Add to .cctor list so we can call it on initialization
            m_context.CctorList.Add(methodName);

            // Prototype
            m_context.Writer.CurrentDestination = WriterDestination.MethodPrototypes;
            m_context.Writer.Append(methodPrototype);
            m_context.Writer.AppendLine(";");

            // Declaration
            m_context.Writer.CurrentDestination = WriterDestination.MethodDeclarations;
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
