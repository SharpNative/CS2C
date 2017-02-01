using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

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
        public string ConvertTypeName(ITypeSymbol type)
        {
            string typeNameConverted;

            bool nameContainsType = (type.ContainingType == null);
            string containingType = nameContainsType ? type.ToString().Replace('.', '_') : type.ContainingType.ToString().Replace('.', '_');
            string nameSpace = (type.ContainingNamespace == null) ? "" : m_context.ConvertNameSpace(type.ContainingNamespace);

            if (type.TypeKind == TypeKind.Class)
            {
                if (nameContainsType)
                    typeNameConverted = string.Format("struct class_{0}*", containingType);
                else
                    typeNameConverted = string.Format("struct class_{0}_{1}*", containingType, type.Name);
            }
            else if (type.TypeKind == TypeKind.Enum)
            {
                typeNameConverted = "int32_t";
            }
            else if (type.TypeKind == TypeKind.Struct)
            {
                if (nameContainsType)
                    typeNameConverted = string.Format("struct struct_{0}", containingType);
                else
                    typeNameConverted = string.Format("struct struct_{0}_{1}", containingType, type.Name);
            }
            else if (type.TypeKind == TypeKind.Pointer)
            {
                IPointerTypeSymbol pointer = (IPointerTypeSymbol)type;
                typeNameConverted = m_context.ConvertTypeName(pointer.PointedAtType) + "*";
            }
            else if (type.TypeKind == TypeKind.Array)
            {
                IArrayTypeSymbol array = (IArrayTypeSymbol)type;
                typeNameConverted = m_context.ConvertTypeName(array.ElementType) + new string('*', array.Rank);
            }
            else if (type.TypeKind == TypeKind.Delegate)
            {
                typeNameConverted = string.Format("delegate_{0}", type.ToString().Replace('.', '_'));
            }
            else if (type.TypeKind == TypeKind.Class)
            {
                typeNameConverted = string.Format("struct class_{0}_{1}*", nameSpace, type.ToString());
            }
            else if (type.TypeKind == TypeKind.Enum)
            {
                typeNameConverted = "int32_t";
            }
            else if (type.TypeKind == TypeKind.Interface)
            {
                typeNameConverted = "struct base_class*";
            }
            else
            {
                throw new NotImplementedException("Could not convert type name: " + type.TypeKind + " is unimplemented");
            }

            return typeNameConverted;
        }

        /// <summary>
        /// Converts a variable name from C# to C
        /// </summary>
        /// <param name="node">The node</param>
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
                typeNameConverted = m_context.Generators.MethodDeclaration.CreateMethodPrototype((IMethodSymbol)symbol, false, false);
            }
            // Static field
            else if (symbol.IsStatic)
            {
                FieldDeclarationSyntax fieldDeclaration = symbol.DeclaringSyntaxReferences[0].GetSyntax().Parent.Parent as FieldDeclarationSyntax;
                IEnumerable<SyntaxToken> children = fieldDeclaration.ChildTokens();

                bool isConst = false;
                foreach (SyntaxToken token in children)
                {
                    if (token.Kind() == SyntaxKind.ConstKeyword)
                    {
                        isConst = true;
                        break;
                    }
                }

                if (isConst)
                    typeNameConverted = string.Format("const_{0}_{1}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name);
                else
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
