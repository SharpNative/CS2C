using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace LibCS2C.Generators
{
    class ClassStructGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        private ClassCodeData m_classCode;

        /// <summary>
        /// Class struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="classCode">Class code</param>
        public ClassStructGenerator(WalkerContext context, ClassCodeData classCode)
        {
            m_context = context;
            m_classCode = classCode;
        }

        /// <summary>
        /// Generates the class struct
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            m_context.Writer.CurrentDestination = WriterDestination.ClassStructs;
            m_context.Writer.AppendLine(m_context.TypeConvert.CurrentClassStructName);
            m_context.Writer.AppendLine("{");

            // For method lookup at runtime
            m_context.Writer.AppendLine("void** lookup_table;");

            foreach (KeyValuePair<string, TypeSyntax> pair in m_classCode.nonStaticFieldTypes)
            {
                // Check for extra modifiers
                IEnumerable<SyntaxToken> tokens = pair.Value.Parent.Parent.ChildTokens();
                foreach (SyntaxToken token in tokens)
                {
                    if (token.Kind() == SyntaxKind.VolatileKeyword)
                    {
                        m_context.Writer.Append("volatile ");
                        break;
                    }
                }

                m_context.Writer.AppendLine(string.Format("{0} field_{1};", m_context.ConvertTypeName(pair.Value), pair.Key));
            }
            
            // We need to keep the order of the base (with interfaces), so first generate those properties
            // and then generate the properties of this class that are not part of the interface
            // TODO: multiple levels of bases (?)
            BaseListSyntax baseList = node.BaseList;
            if (baseList != null)
            {
                IEnumerable<SyntaxNode> nodes = baseList.ChildNodes();
                foreach (SimpleBaseTypeSyntax child in nodes)
                {
                    // Get base type
                    ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(child.Type).Type;

                    // Loop through interface properties
                    InterfaceDeclarationSyntax interfaceDeclaration = typeSymbol.DeclaringSyntaxReferences[0].GetSyntax() as InterfaceDeclarationSyntax;
                    IEnumerable<SyntaxNode> interfaceNodes = node.ChildNodes();
                    foreach (SyntaxNode interfaceChild in interfaceNodes)
                    {
                        if (interfaceChild.Kind() == SyntaxKind.PropertyDeclaration)
                        {
                            PropertyDeclarationSyntax property = interfaceChild as PropertyDeclarationSyntax;
                            m_context.Writer.AppendLine(string.Format("{0} prop_{1};", m_context.ConvertTypeName(property.Type), property.Identifier));

                            // Remove from list so we don't generate them twice
                            m_classCode.propertyTypesNonStatic.Remove(property.Identifier.ToString());
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, TypeSyntax> pair in m_classCode.propertyTypesNonStatic)
            {
                m_context.Writer.AppendLine(string.Format("{0} prop_{1};", m_context.ConvertTypeName(pair.Value), pair.Key));
            }

            m_context.Writer.AppendLine("};");

            m_context.Writer.CurrentDestination = WriterDestination.StructPrototypes;
            m_context.Writer.AppendLine(string.Format("{0};", m_context.TypeConvert.CurrentClassStructName));
        }
    }
}
