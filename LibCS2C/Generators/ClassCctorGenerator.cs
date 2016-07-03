﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    class ClassCctorGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private Dictionary<string, EqualsValueClauseSyntax> m_staticFields;
        private Dictionary<string, EqualsValueClauseSyntax> m_staticProperties;

        /// <summary>
        /// Class .cctor generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ClassCctorGenerator(WalkerContext context, Dictionary<string, EqualsValueClauseSyntax> staticFields, Dictionary<string, EqualsValueClauseSyntax> staticProperties)
        {
            m_context = context;
            m_staticFields = staticFields;
            m_staticProperties = staticProperties;
        }

        /// <summary>
        /// Generate the .cctor method of a class
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            string convertedClassName = m_context.ConvertClassName(node.Identifier.ToString());
            string methodName = string.Format("classCctor_{0}", convertedClassName);
            string methodPrototype = string.Format("void {0}(void)", methodName);

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

            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_staticFields)
            {
                m_context.Writer.Append(string.Format("\tclassStatics_{0}.{1} = ", convertedClassName, pair.Key));
                ExpressionSyntax expression = pair.Value.Value;
                m_context.Generators.Expression.Generate(expression);
                m_context.Writer.AppendLine(";");
            }

            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_staticProperties)
            {
                m_context.Writer.Append(string.Format("\tclassStatics_{0}.prop_{1} = ", convertedClassName, pair.Key));
                ExpressionSyntax expression = pair.Value.Value;
                m_context.Generators.Expression.Generate(expression);
                m_context.Writer.AppendLine(";");
            }

            m_context.Writer.AppendLine("}");
        }
    }
}
