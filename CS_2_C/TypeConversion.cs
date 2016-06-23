using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C
{
    class TypeConversion
    {
        private Dictionary<string, string> m_convert = new Dictionary<string, string>()
        {
            { "int", "int" },
            { "float", "float" },
            { "string", "char*" },
            { "void", "void" }
        };

        public bool IsGeneric(TypeSyntax type)
        {
            //Console.WriteLine("full string: " + type.ToFullString().Trim());
            return m_convert.ContainsKey(type.ToString().Trim());
        }

        public string Convert(TypeSyntax type)
        {
            return m_convert[type.ToFullString().Trim()];
        }
    }
}
