using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace LibCS2C
{
    public class WalkerContext
    {
        /// <summary>
        /// Type conversion helper
        /// </summary>
        public GenericTypeConversion TypeConvert { get; private set; } = new GenericTypeConversion();

        /// <summary>
        /// .cctor list
        /// </summary>
        public List<string> CctorList { get; private set; } = new List<string>();

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
        /// Current destionation of the writer, this is because a destination depends on the structure of the code
        /// Therefor, we cannot assume one generator is one writer
        /// </summary>
        
        public WriterDestination CurrentDestination
        {
            get
            {
                return m_currentDestination;
            }

            set
            {
                m_currentDestination = value;
                Writer = m_stringBuilders[(int)value];
            }
        }

        /// <summary>
        /// If a postcode should be outputted
        /// </summary>
        public bool ShouldOutputPost { get; set; } = false;

        /// <summary>
        /// Gets the semantic Model
        /// </summary>
        public SemanticModel Model { get; set; }

        /// <summary>
        /// Gets the current namespace name formatted
        /// </summary>
        public string CurrentNamespaceFormatted { get { return CurrentNamespace.Name.ToString().Replace(".", "_"); } }

        /// <summary>
        /// A list with all the generators
        /// </summary>
        public AllGenerators Generators { get; private set; }
        
        // String builders
        public FormattedStringBuilder SbEnums { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbStructs { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbClassStructs { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbDelegates { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbMethodPrototypes { get; private set; } = new FormattedStringBuilder();
        public FormattedStringBuilder SbMethodDeclarations { get; private set; } = new FormattedStringBuilder();
        private FormattedStringBuilder m_sbTempBuffer = new FormattedStringBuilder();
        private FormattedStringBuilder m_sbPostBuffer = new FormattedStringBuilder();
        private FormattedStringBuilder[] m_stringBuilders;

        // Current writing destination
        private WriterDestination m_currentDestination;

        /// <summary>
        /// Contextwalker
        /// </summary>
        /// <param name="sb">The formatted string builder</param>
        public WalkerContext()
        {
            m_stringBuilders = new FormattedStringBuilder[] { SbEnums, SbStructs, SbClassStructs, SbDelegates, SbMethodPrototypes, SbMethodDeclarations, m_sbTempBuffer, m_sbPostBuffer };
            Generators = new AllGenerators(this);
        }

        /// <summary>
        /// Flushes the temporary buffer
        /// </summary>
        /// <returns>The buffer contents</returns>
        public string FlushTempBuffer()
        {
            string ret = m_sbTempBuffer.ToString();
            m_sbTempBuffer.Clear();
            return ret;
        }

        /// <summary>
        /// Checks if the post buffer is empty
        /// </summary>
        /// <returns>If it's empty</returns>
        public bool IsPostBufferEmpty()
        {
            return m_sbPostBuffer.IsEmpty();
        }

        /// <summary>
        /// Flushes the post buffer
        /// </summary>
        /// <returns>The buffer contents</returns>
        public string FlushPostBuffer()
        {
            string ret = m_sbPostBuffer.ToString();
            m_sbPostBuffer.Clear();
            return ret;
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
        public string ConvertTypeName(SyntaxNode type)
        {
            string typeNameConverted;
            if (TypeConvert.IsGeneric(type as TypeSyntax))
            {
                typeNameConverted = TypeConvert.Convert(type as TypeSyntax);
            }
            else if (type is QualifiedNameSyntax)
            {
                ITypeSymbol typeSymbol = Model.GetTypeInfo(type).Type;
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
                typeNameConverted = ConvertTypeName(ptr.ElementType) + "*";
            }
            else if (type is ArrayTypeSyntax)
            {
                ArrayTypeSyntax array = type as ArrayTypeSyntax;
                typeNameConverted = ConvertTypeName(array.ElementType) + "*";
            }
            else
            {
                ITypeSymbol typeSymbol = Model.Compilation.GetSemanticModel(type.Parent.SyntaxTree).GetTypeInfo(type).Type;
                string nameSpace = typeSymbol.ContainingNamespace.ToString().Replace(".", "_");
                
                if(typeSymbol.TypeKind == TypeKind.Delegate)
                {
                    typeNameConverted = string.Format("delegate_{0}_{1}", typeSymbol.ContainingType.ToString().Replace(".", "_"), type.ToString());
                }
                else if (typeSymbol.TypeKind == TypeKind.Class)
                {
                    typeNameConverted = string.Format("struct class_{0}_{1}*", nameSpace, type.ToString());
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
            ISymbol symbol = Model.GetSymbolInfo(node).Symbol;

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
                    string currentClass = CurrentNamespace.Name + "." + CurrentClass.Identifier;
                    if (currentClass == symbol.ContainingType.ToString())
                    {
                        typeNameConverted += "(obj)";
                    }
                }
            }
            // Method
            else if(symbol.Kind == SymbolKind.Method)
            {
                MethodDeclarationSyntax reference = symbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                typeNameConverted = Generators.MethodDeclaration.CreateMethodPrototype(reference, false);
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
