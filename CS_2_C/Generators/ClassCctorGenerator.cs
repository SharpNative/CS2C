using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
{
    class ClassCctorGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private Dictionary<string, EqualsValueClauseSyntax> m_staticFields;
        private ExpressionGenerator m_expressionGen;

        /// <summary>
        /// Class .cctor generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ClassCctorGenerator(WalkerContext context, Dictionary<string, EqualsValueClauseSyntax> staticFields)
        {
            m_context = context;
            m_staticFields = staticFields;
            m_expressionGen = new ExpressionGenerator(m_context);
        }

        /// <summary>
        /// Generate the .cctor method of a class
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            string convertedClassName = m_context.ConvertClassName(node.Identifier.ToString());

            m_context.Writer.AppendLine(string.Format("void classCctor_{0}(void)", convertedClassName));
            m_context.Writer.AppendLine("{");

            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in m_staticFields)
            {
                m_context.Writer.Append(string.Format("\tclassStatics_{0}.{1} = ", convertedClassName, pair.Key));
                ExpressionSyntax expression = pair.Value.Value;
                m_expressionGen.Generate(expression);
                m_context.Writer.AppendLine(";");
            }

            m_context.Writer.AppendLine("}");
            m_context.Writer.AppendLine("");
        }
    }
}
