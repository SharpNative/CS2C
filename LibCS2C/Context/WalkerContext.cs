using LibCS2C.Generators;
using LibCS2C.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace LibCS2C.Context
{
    public class WalkerContext
    {
        /// <summary>
        /// Generic type conversion helper
        /// </summary>
        public GenericTypeConversion GenericTypeConvert { get; private set; } = new GenericTypeConversion();

        /// <summary>
        /// Type conversion helper
        /// </summary>
        public TypeConversion TypeConvert { get; private set; }

        /// <summary>
        /// Method table
        /// </summary>
        public MethodTable MethodTable { get; private set; }

        /// <summary>
        /// If a class is ever extended by another type
        /// </summary>
        public Dictionary<string, bool> TypeIsExtending { get; private set; } = new Dictionary<string, bool>();

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
        /// Gets the semantic Model
        /// </summary>
        public SemanticModel Model { get; set; }

        /// <summary>
        /// A list with all the generators
        /// </summary>
        public AllGenerators Generators { get; private set; }

        /// <summary>
        /// The writer
        /// </summary>
        public Writer Writer { get; private set; }

        /// <summary>
        /// Contextwalker
        /// </summary>
        /// <param name="sb">The formatted string builder</param>
        public WalkerContext()
        {
            Generators = new AllGenerators(this);
            TypeConvert = new TypeConversion(this);
            MethodTable = new MethodTable(this);
            Writer = new Writer();
        }
        
        /// <summary>
        /// Converts the C# type to a C type name
        /// </summary>
        /// <param name="type">The C# type</param>
        /// <returns>The C type name</returns>
        public string ConvertTypeName(ITypeSymbol type)
        {
            if (GenericTypeConvert.IsGeneric(type))
            {
                return GenericTypeConvert.Convert(type);
            }
            else
            {
                return TypeConvert.ConvertTypeName(type);
            }
        }

        /// <summary>
        /// Converts the C# type to a C type name
        /// </summary>
        /// <param name="type">The C# type</param>
        /// <returns>The C type name</returns>
        public string ConvertTypeName(TypeSyntax type)
        {
            ITypeSymbol symbol = Model.GetTypeInfo(type).Type;
            return ConvertTypeName(symbol);
        }

        /// <summary>
        /// Converts a namespace to a C namespace name
        /// </summary>
        /// <param name="nameSpace">The namespace</param>
        /// <returns>The C name</returns>
        public string ConvertNameSpace(INamespaceSymbol nameSpace)
        {
            return nameSpace.ToString().Replace('.', '_');
        }
    }
}
