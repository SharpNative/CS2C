using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CS_2_C
{
    class WalkerContext
    {
        public TypeConversion TypeConvert { get; private set; }

        public ClassDeclarationSyntax CurrentClass { get; set; }

        public NamespaceDeclarationSyntax CurrentNamespace { get; set; }

        public string CurrentClassNameFormatted { get { return ConvertClassName(CurrentClass.Identifier.ToString()); } }

        public string CurrentClassStructName { get { return string.Format("struct class_{0}", CurrentClassNameFormatted); } }

        public FormattedStringBuilder Writer { get; private set; }
        
        /// <summary>
        /// Gets the current namespace name formatted
        /// </summary>
        public string CurrentNamespaceFormatted { get { return CurrentNamespace.Name.ToString().Replace(".", "_"); } }

        public WalkerContext(FormattedStringBuilder sb)
        {
            TypeConvert = new TypeConversion();
            Writer = sb;
        }
        
        public string ConvertClassName(string identifier)
        {
            return CurrentNamespaceFormatted + "_" + identifier;
        }
        
        /// <summary>
        /// Converts the C# type to a C type name
        /// </summary>
        /// <param name="type">The c# type</param>
        /// <returns>The C type name</returns>
        public string ConvertTypeName(TypeSyntax type)
        {
            string typeNameConverted;
            if (TypeConvert.IsGeneric(type))
            {
                typeNameConverted = TypeConvert.Convert(type);
            }
            else
            {
                typeNameConverted = ConvertClassName(type.ToString());
            }

            return typeNameConverted;
        }
    }
}
