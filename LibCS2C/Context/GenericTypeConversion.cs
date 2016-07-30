using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace LibCS2C.Context
{
    /// <summary>
    /// Contains the native / generic type conversions
    /// </summary>
    public class GenericTypeConversion
    {
        private Dictionary<string, string> m_convert = new Dictionary<string, string>()
        {
            { "void", "void" },

            { "bool", "int32_t" },
            { "long", "int64_t" },
            { "ulong", "uint64_t" },
            { "int", "int32_t" },
            { "uint", "uint32_t" },
            { "short", "int16_t" },
            { "ushort", "uint16_t" },
            { "byte", "uint8_t" },
            { "sbyte", "int8_t" },

            { "float", "float" },
            { "double", "double" },

            { "char", "char" },
            { "string", "char*" },

            { "Action", "void*" },
            { "object", "void*" },

            { "UInt8", "uint8_t" },
            { "UInt16", "uint16_t" },
            { "UInt32", "uint32_t" },
            { "UInt64", "uint64_t" },

            { "Char", "char" },
            { "Int8", "int8_t" },
            { "Int16", "int16_t" },
            { "Int32", "int32_t" },
            { "Int64", "int64_t" },
            { "Boolean", "int32_t" },
            { "Byte", "uint8_t" }
        };

        /// <summary>
        /// Checks if a given C# type is a generic type
        /// </summary>
        /// <param name="type">The C# type</param>
        /// <returns>If the type is a generic type</returns>
        public bool IsGeneric(TypeSyntax type)
        {
            return m_convert.ContainsKey(type.ToString().Trim());
        }

        /// <summary>
        /// Checks if a given C# type is a generic type
        /// </summary>
        /// <param name="type">The C# type name</param>
        /// <returns>If the type is a generic type</returns>
        public bool IsGeneric(string type)
        {
            return m_convert.ContainsKey(type);
        }

        /// <summary>
        /// Converts the given C# type to a C type
        /// </summary>
        /// <param name="type">The C# type</param>
        /// <returns>The C type</returns>
        public string Convert(TypeSyntax type)
        {
            return m_convert[type.ToString().Trim()];
        }
    }
}
