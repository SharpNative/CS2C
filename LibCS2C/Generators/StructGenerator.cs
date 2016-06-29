using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    class StructGenerator : GeneratorBase<StructDeclarationSyntax>
    {
        private ExpressionGenerator m_expressionGen;

        /// <summary>
        /// struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public StructGenerator(WalkerContext context)
        {
            m_context = context;
            m_expressionGen = new ExpressionGenerator(m_context);
        }

        /// <summary>
        /// Generates a struct declaration
        /// </summary>
        /// <param name="node">The struct declaration</param>
        public override void Generate(StructDeclarationSyntax node)
        {
            string structName = "";
            if (node.Parent is ClassDeclarationSyntax)
            {
                structName = string.Format("{0}_{1}", m_context.CurrentClassNameFormatted, node.Identifier.ToString());
            }
            else
            {
                structName = string.Format("{0}_{1}", m_context.CurrentNamespaceFormatted, node.Identifier.ToString());
            }

            // Temporarily hold all the data
            Dictionary<string, EqualsValueClauseSyntax> fields = new Dictionary<string, EqualsValueClauseSyntax>();
            Dictionary<string, TypeSyntax> fieldTypes = new Dictionary<string, TypeSyntax>();

            // Collect the data and put it in the dictionaries
            IEnumerable<SyntaxNode> children = node.ChildNodes();
            foreach (SyntaxNode child in children)
            {
                SyntaxKind kind = child.Kind();

                if (kind == SyntaxKind.FieldDeclaration)
                {
                    FieldDeclarationSyntax field = child as FieldDeclarationSyntax;
                    IEnumerable<SyntaxNode> fieldChildren = field.ChildNodes();

                    foreach (VariableDeclarationSyntax fieldChild in fieldChildren)
                    {
                        foreach (VariableDeclaratorSyntax variable in fieldChild.Variables)
                        {
                            string identifier = variable.Identifier.ToString();
                            if (variable.Initializer != null)
                                fields.Add(identifier, variable.Initializer);

                            fieldTypes.Add(identifier, fieldChild.Type);
                        }
                    }
                }
            }

            // Struct
            m_context.Writer.AppendLine(string.Format("struct struct_{0}", structName));
            m_context.Writer.AppendLine("{");

            // Usage count for garbage collector
            m_context.Writer.AppendLine("\tint32_t usage_count;");

            foreach (KeyValuePair<string, TypeSyntax> pair in fieldTypes)
            {
                m_context.Writer.AppendLine("\t/* Field: " + pair.Key + " */");
                m_context.Writer.AppendLine(string.Format("\t{0} field_{1};", m_context.ConvertTypeName(pair.Value), pair.Key));
            }

            m_context.Writer.AppendLine("};");
            m_context.Writer.AppendLine("");

            // Init code
            m_context.Writer.AppendLine(string.Format("struct struct_{0}* structInit_{0}(void)", structName));
            m_context.Writer.AppendLine("{");
            m_context.Writer.AppendLine(string.Format("\tstruct struct_{0}* object = malloc(sizeof(struct struct_{0}));", structName));
            m_context.Writer.AppendLine("\tif(!object)");
            m_context.Writer.AppendLine("\t\treturn NULL;");
            m_context.Writer.AppendLine("\tobject->usage_count = 1;");

            // Loop through the fields and initialize them
            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in fields)
            {
                m_context.Writer.Append(string.Format("\tobject->field_{0} = ", pair.Key));
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
