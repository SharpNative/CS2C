using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibCS2C.Context
{
    public class TypeConversion
    {
        /// <summary>
        /// Formatted current class name
        /// </summary>
        public string CurrentClassNameFormatted { get { return ConvertClassName(m_context.CurrentClass.Identifier.ToString()); } }

        /// <summary>
        /// Gets the current class converted to the struct name
        /// </summary>
        public string CurrentClassStructName { get { return string.Format("struct class_{0}", CurrentClassNameFormatted); } }

        /// <summary>
        /// Gets the current namespace name formatted
        /// </summary>
        public string CurrentNamespaceFormatted { get { return m_context.CurrentNamespace.Name.ToString().Replace(".", "_"); } }

        // The context
        private WalkerContext m_context;

        /// <summary>
        /// Creates the type conversion helper
        /// </summary>
        /// <param name="context">The context</param>
        public TypeConversion(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Convert class name in the current namespace to a formatted name
        /// </summary>
        /// <param name="identifier">The class name</param>
        /// <returns>The formatted name</returns>
        public string ConvertClassName(string identifier)
        {
            return m_context.TypeConvert.CurrentNamespaceFormatted + "_" + identifier;
        }

        /// <summary>
        /// Converts the C# type to a C type name
        /// </summary>
        /// <param name="type">The C# type</param>
        /// <returns>The C type name</returns>
        public string ConvertTypeName(SyntaxNode type)
        {
            string typeNameConverted;
            if (type is QualifiedNameSyntax)
            {
                ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(type).Type;
                string containingType = typeSymbol.ContainingType.ToString().Replace('.', '_');
                string typeName = typeSymbol.Name;

                if (typeSymbol.TypeKind == TypeKind.Class)
                {
                    typeNameConverted = string.Format("struct class_{0}_{1}*", containingType, typeName);
                }
                else
                {
                    typeNameConverted = string.Format("struct struct_{0}_{1}", containingType, typeName);
                }
            }
            else if (type is PointerTypeSyntax)
            {
                PointerTypeSyntax ptr = type as PointerTypeSyntax;
                typeNameConverted = m_context.ConvertTypeName(ptr.ElementType) + "*";
            }
            else if (type is ArrayTypeSyntax)
            {
                ArrayTypeSyntax array = type as ArrayTypeSyntax;
                typeNameConverted = m_context.ConvertTypeName(array.ElementType) + new string('*', array.RankSpecifiers.Count);
            }
            else
            {
                ITypeSymbol typeSymbol = m_context.Model.Compilation.GetSemanticModel(type.Parent.SyntaxTree).GetTypeInfo(type).Type;
                string nameSpace = typeSymbol.ContainingNamespace.ToString().Replace(".", "_");

                if (typeSymbol.TypeKind == TypeKind.Delegate)
                {
                    typeNameConverted = string.Format("delegate_{0}_{1}", typeSymbol.ContainingType.ToString().Replace(".", "_"), type.ToString());
                }
                else if (typeSymbol.TypeKind == TypeKind.Class)
                {
                    typeNameConverted = string.Format("struct class_{0}_{1}*", nameSpace, type.ToString());
                }
                else if (typeSymbol.TypeKind == TypeKind.Enum)
                {
                    typeNameConverted = "int32_t";
                }
                else if (typeSymbol.TypeKind == TypeKind.Interface)
                {
                    typeNameConverted = "struct base_class*";
                }
                else
                {
                    if (typeSymbol.ContainingType == null)
                        typeNameConverted = string.Format("struct struct_{0}_{1}", nameSpace, type.ToString());
                    else
                        typeNameConverted = string.Format("struct struct_{0}_{1}", typeSymbol.ContainingType.ToString().Replace(".", "_"), type.ToString());
                }
            }

            return typeNameConverted;
        }

        /// <summary>
        /// Converts a variable name from C# to C
        /// </summary>
        /// <param name="node">The symbol node</param>
        /// <returns>The converted variable name</returns>
        public string ConvertVariableName(SyntaxNode node)
        {
            string typeNameConverted;
            ISymbol symbol = m_context.Model.GetSymbolInfo(node).Symbol;

            // Property
            if (symbol.Kind == SymbolKind.Property)
            {
                if (symbol.IsStatic)
                {
                    typeNameConverted = string.Format("{0}_{1}_getter()", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name);
                }
                else
                {
                    typeNameConverted = string.Format("{0}_{1}_getter", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name);
                    string currentClass = m_context.CurrentNamespace.Name + "." + m_context.CurrentClass.Identifier;
                    if (currentClass == symbol.ContainingType.ToString())
                    {
                        typeNameConverted += "(obj)";
                    }
                }
            }
            // Method
            else if (symbol.Kind == SymbolKind.Method)
            {
                MethodDeclarationSyntax reference = symbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                typeNameConverted = m_context.Generators.MethodDeclaration.CreateMethodPrototype(reference, false, false);
            }
            // Static field
            else if (symbol.IsStatic)
            {
                typeNameConverted = string.Format("classStatics_{0}.{1}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name);
            }
            // Argument or local variable
            else if (symbol.ContainingSymbol.Kind == SymbolKind.Method)
            {
                typeNameConverted = symbol.Name;
            }
            // Field
            else
            {
                typeNameConverted = string.Format("field_{0}", symbol.Name);
            }

            return typeNameConverted;
        }
    }
}
