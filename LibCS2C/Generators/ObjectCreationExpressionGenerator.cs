using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibCS2C.Generators
{
    class ObjectCreationExpressionGenerator : GeneratorBase<ObjectCreationExpressionSyntax>
    {
        /// <summary>
        /// Object creation expression generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ObjectCreationExpressionGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates the expression code
        /// </summary>
        /// <param name="node">The expression statement node</param>
        public override void Generate(ObjectCreationExpressionSyntax node)
        {
            IEnumerable<SyntaxNode> objNodes = node.ChildNodes();
            
            ArgumentListSyntax args = node.ArgumentList;
            ITypeSymbol type = m_context.Model.GetTypeInfo(node).Type;
            string nameSpace = type.ContainingNamespace.ToString().Replace(".", "_");
            
            // Class
            if(type.TypeKind == TypeKind.Class)
            {
                // Call Constructor
                m_context.Writer.Append(string.Format("{0}_{1}_{1}(", nameSpace, type.Name));

                // Class initialization (returns the object, we can pass it as an argument to the constructor)
                m_context.Writer.Append(string.Format("classInit_{0}_{1}()", nameSpace, type.Name));

                // Remaining arguments
                int argCount = args.ChildNodes().Count();
                if (argCount > 0)
                    m_context.Writer.Append(", ");

                ArgumentListGenerator argGen = new ArgumentListGenerator(m_context);
                argGen.Generate(args);

                m_context.Writer.Append(")");
            }
            // Struct
            else
            {
                m_context.Writer.Append(string.Format("structInit_{0}_{1}()", type.ContainingSymbol.ToString().Replace(".", "_"), type.Name));
            }
        }
    }
}
