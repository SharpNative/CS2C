using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibCS2C
{
    class WalkerContext
    {
        /// <summary>
        /// Type conversion helper
        /// </summary>
        public TypeConversion TypeConvert { get; private set; }

        /// <summary>
        /// The current class
        /// </summary>
        public ClassDeclarationSyntax CurrentClass { get; set; }

        /// <summary>
        /// The current namespace
        /// </summary>
        public NamespaceDeclarationSyntax CurrentNamespace { get; set; }
        
        /// <summary>
        /// Formatted current class name
        /// </summary>
        public string CurrentClassNameFormatted { get { return ConvertClassName(CurrentClass.Identifier.ToString()); } }

        /// <summary>
        /// Gets the current class converted to the struct name
        /// </summary>
        public string CurrentClassStructName { get { return string.Format("struct class_{0}", CurrentClassNameFormatted); } }

        /// <summary>
        /// Gets the output writer
        /// </summary>
        public FormattedStringBuilder Writer { get; private set; }

        /// <summary>
        /// Gets the semantic Model
        /// </summary>
        public SemanticModel Model { get; private set; }

        /// <summary>
        /// Gets the current namespace name formatted
        /// </summary>
        public string CurrentNamespaceFormatted { get { return CurrentNamespace.Name.ToString().Replace(".", "_"); } }

        /// <summary>
        /// Contextwalker
        /// </summary>
        /// <param name="sb">The formatted string builder</param>
        public WalkerContext(FormattedStringBuilder sb, SemanticModel model)
        {
            TypeConvert = new TypeConversion();
            Writer = sb;
            Model = model;
        }

        /// <summary>
        /// Convert class name to a formatted name
        /// </summary>
        /// <param name="identifier">The class name</param>
        /// <returns>The formatted name</returns>
        public string ConvertClassName(string identifier)
        {
            return CurrentNamespaceFormatted + "_" + identifier;
        }

        /// <summary>
        /// Converts the C# type to a C type name
        /// </summary>
        /// <param name="type">The C# type</param>
        /// <returns>The C type name</returns>
        public string ConvertTypeName(TypeSyntax type)
        {
            string typeNameConverted;
            if (TypeConvert.IsGeneric(type))
            {
                typeNameConverted = TypeConvert.Convert(type);
            }
            else if (type is PointerTypeSyntax)
            {
                PointerTypeSyntax ptr = type as PointerTypeSyntax;
                typeNameConverted = ConvertTypeName(ptr.ElementType) + "*";
            }
            else
            {
                string nameSpace = Model.GetTypeInfo(type).Type.ContainingNamespace.ToString().Replace(".", "_");
                typeNameConverted = string.Format("struct class_{0}_{1}*", nameSpace, type.ToString());
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
            string typeNameConverted = "";
            ISymbol symbol = Model.GetSymbolInfo(node).Symbol;
            
            // Static field
            if (symbol.IsStatic)
            {
                typeNameConverted = string.Format("classStatics_{0}.{1}", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name);
            }
            // Property
            else if (symbol.Kind == SymbolKind.Property)
            {
                typeNameConverted = string.Format("{0}_{1}_getter(obj)", symbol.ContainingType.ToString().Replace(".", "_"), symbol.Name);
            }
            // Argument or local variable
            else if (symbol.ContainingSymbol.Kind == SymbolKind.Method)
            {
                typeNameConverted = symbol.Name;
            }
            // Field
            else
            {
                typeNameConverted = string.Format("obj->field_{0}", symbol.Name);
            }
            
            return typeNameConverted;
        }

        /// <summary>
        /// Checks if the syntax node kind is a sub expression
        /// </summary>
        /// <param name="kind">The syntax kind</param>
        /// <returns>If it's a sub expression</returns>
        public bool IsSubExpression(SyntaxKind kind)
        {
            return kind == SyntaxKind.AddExpression ||
                   kind == SyntaxKind.SubtractExpression ||
                   kind == SyntaxKind.MultiplyExpression ||
                   kind == SyntaxKind.DivideExpression;
        }
    }
}
