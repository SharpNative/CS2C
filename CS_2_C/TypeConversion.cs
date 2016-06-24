using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C
{
    /// <summary>
    /// Contains the native / generic type conversions
    /// </summary>
    class TypeConversion
    {
        private Dictionary<string, string> m_convert = new Dictionary<string, string>()
        {
            { "int", "int32_t" },
            { "uint", "uint32_t" },
            { "short", "int16_t" },
            { "ushort", "uint16_t" },
            { "byte", "uint8_t" },
            { "sbyte", "int8_t" },
            { "float", "float" },
            { "double", "double" },
            { "string", "char*" },
            { "void", "void" }
        };

        /// <summary>
        /// Checks if a given C# type is a generic type
        /// </summary>
        /// <param name="type">The C# type</param>
        /// <returns>If the type is a generic type</returns>
        public bool IsGeneric(TypeSyntax type)
        {
            //Console.WriteLine("full string: " + type.ToFullString().Trim());
            return m_convert.ContainsKey(type.ToString().Trim());
        }

        /// <summary>
        /// Converts the given C# type to a C type
        /// </summary>
        /// <param name="type">The C# type</param>
        /// <returns>The C type</returns>
        public string Convert(TypeSyntax type)
        {
            return m_convert[type.ToFullString().Trim()];
        }
    }
}
