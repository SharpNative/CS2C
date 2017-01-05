using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace LibCS2C.Context
{
    public class MethodTable
    {
        private Dictionary<string, List<string>> m_methods = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> m_methodsPerClass = new Dictionary<string, List<string>>();

        private WalkerContext m_context;

        /// <summary>
        /// Initializes the method table
        /// </summary>
        /// <param name="context">The current context</param>
        public MethodTable(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Clears the table
        /// </summary>
        public void Clear()
        {
            m_methods.Clear();
        }

        /// <summary>
        /// Adds current class
        /// </summary>
        public void AddCurrentClass()
        {
            string currentClassName = m_context.TypeConvert.CurrentClassNameFormatted;
            if (!m_methodsPerClass.ContainsKey(currentClassName))
                m_methodsPerClass.Add(currentClassName, new List<string>());
        }
        
        /// <summary>
        /// Adds a new method to the table
        /// </summary>
        /// <param name="method">The method declaration</param>
        public void Add(MethodDeclarationSyntax method)
        {
            string str = GetMethodLookupName(method);
            string currentClassName = m_context.TypeConvert.CurrentClassNameFormatted;
            string className = currentClassName;

            TypeDeclarationSyntax parent = method.Parent as TypeDeclarationSyntax;
            bool found = false;
            if (parent.BaseList != null)
            {
                IEnumerable<SyntaxNode> children = parent.BaseList.ChildNodes();
                
                foreach (SimpleBaseTypeSyntax child in children)
                {
                    IdentifierNameSyntax identifier = child.ChildNodes().First() as IdentifierNameSyntax;

                    ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(identifier).Type;

                    ImmutableArray<ISymbol> members = typeSymbol.GetMembers();
                    foreach (ISymbol member in members)
                    {
                        Console.WriteLine(member.Name + " | " + method.Identifier.ToString());

                        if (member.Name == method.Identifier.ToString())
                        {
                            className = string.Format("{0}_{1}", typeSymbol.ContainingNamespace.ToString().Replace('.', '_'), typeSymbol.Name);
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found)
                return;
            
            if (!m_methods.ContainsKey(className))
                m_methods.Add(className, new List<string>());

            if (!m_methodsPerClass.ContainsKey(currentClassName))
                m_methodsPerClass.Add(currentClassName, new List<string>());

            m_methods[className].Add(str);
            m_methodsPerClass[currentClassName].Add(str);
        }

        /// <summary>
        /// Gets the ID of a method
        /// </summary>
        /// <param name="className">The class name</param>
        /// <param name="method">The method declaration</param>
        /// <returns>Its ID</returns>
        public int GetID(string className, MethodDeclarationSyntax method)
        {
            string str = GetMethodLookupName(method);
            return m_methods[className].IndexOf(str);
        }

        /// <summary>
        /// Gets the lookup name of a method
        /// </summary>
        /// <param name="method">The method declaration</param>
        /// <returns>Its lookup name</returns>
        private string GetMethodLookupName(MethodDeclarationSyntax method)
        {
            TypeDeclarationSyntax parent = method.Parent as TypeDeclarationSyntax;
            NamespaceDeclarationSyntax nameSpace = parent.Parent as NamespaceDeclarationSyntax;

            string suffix = string.Format("{0}_{1}", nameSpace.Name.ToString().Replace('.', '_'), parent.Identifier.ToString());
            string str = m_context.Generators.MethodDeclaration.CreateMethodPrototype(method, false, false);

            return str.Substring(suffix.Length);
        }

        /// <summary>
        /// Converts the method table to a prototype C array
        /// </summary>
        /// <returns>The code</returns>
        public string ToPrototypeArrayCode()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, List<string>> pair in m_methodsPerClass)
            {
                sb.AppendLine(string.Format("static void* methods_{0}[];", pair.Key));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts the method table to a C array
        /// </summary>
        /// <returns>The code</returns>
        public string ToArrayCode()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, List<string>> pair in m_methodsPerClass)
            {
                sb.Append(string.Format("static void* methods_{0}[] = ", pair.Key));
                sb.Append("{");
                pair.Value.ForEach((string prototype) =>
                {
                    sb.Append(string.Format("{0}{1},", pair.Key, prototype));
                });
                sb.AppendLine("};");
            }

            return sb.ToString();
        }
    }
}
