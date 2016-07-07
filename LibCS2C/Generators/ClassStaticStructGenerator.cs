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
    class ClassStaticStructGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private ClassCodeData m_classCode;

        /// <summary>
        /// Class struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="classCode">Class code</param>
        public ClassStaticStructGenerator(WalkerContext context, ClassCodeData classCode)
        {
            m_context = context;
            m_classCode = classCode;
        }

        /// <summary>
        /// Generate the initializers
        /// </summary>
        /// <param name="types">Types</param>
        /// <param name="values">Values</param>
        private void GenerateInitializer(Dictionary<string, TypeSyntax> types, Dictionary<string, EqualsValueClauseSyntax> values)
        {
            // Loop through and initialize if possible
            foreach (KeyValuePair<string, TypeSyntax> pair in types)
            {
                EqualsValueClauseSyntax value;
                if (values.TryGetValue(pair.Key, out value))
                {
                    ExpressionSyntax expression = value.Value;

                    // If it's a literal, we can initialize it safely
                    if (m_context.IsLiteralExpression(expression.Kind()))
                    {
                        m_context.Generators.Expression.Generate(expression);
                        m_context.Writer.AppendLine(",");
                    }
                    else
                    {
                        // Uninitialized for now
                        m_context.Writer.AppendLine("0,");
                    }
                }
                else
                {
                    // Uninitialized for now
                    // C wants {0} for structs instead of 0
                    ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(pair.Value).Type;
                    if (!m_context.TypeConvert.IsGeneric(pair.Value) && typeSymbol.TypeKind == TypeKind.Struct)
                    {
                        m_context.Writer.AppendLine("{0},");
                    }
                    else
                    {
                        m_context.Writer.AppendLine("0,");
                    }
                }
            }
        }

        /// <summary>
        /// Generates a struct member
        /// </summary>
        /// <param name="name">The name of the member</param>
        /// <param name="type">The type of the member</param>
        private void GenerateStructMember(string name, TypeSyntax type)
        {
            string typeName = m_context.ConvertTypeName(type);

            // Check if there's a variable initializer
            // If there is one, we need to change the type from pointer to array
            // so C knows that it needs to reserve memory
            if (m_classCode.staticFields.ContainsKey(name))
            {
                ExpressionSyntax expression = m_classCode.staticFields[name].Value;
                if (expression.Kind() == SyntaxKind.ArrayInitializerExpression)
                {
                    InitializerExpressionSyntax initializer = expression as InitializerExpressionSyntax;
                    typeName = typeName.Substring(0, typeName.Length - 1);
                    name += "[" + initializer.Expressions.Count() + "]";
                }
            }

            m_context.Writer.AppendLine(string.Format("\t{0} {1};", typeName, name));
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

            foreach (KeyValuePair<string, TypeSyntax> pair in m_classCode.staticFieldTypes)
            {
                GenerateStructMember(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, TypeSyntax> pair in m_classCode.propertyTypesStatic)
            {
                GenerateStructMember("prop_" + pair.Key, pair.Value);
            }

            m_context.Writer.Append("} classStatics_");
            m_context.Writer.Append(convertedClassName);

            // Initializers
            m_context.Writer.AppendLine(" = {");
            m_context.Writer.Indent();
            
            GenerateInitializer(m_classCode.staticFieldTypes, m_classCode.staticFields);
            GenerateInitializer(m_classCode.propertyTypesStatic, m_classCode.propertyInitialValuesStatic);

            m_context.Writer.UnIndent();
            m_context.Writer.AppendLine("};");
        }
    }
}
