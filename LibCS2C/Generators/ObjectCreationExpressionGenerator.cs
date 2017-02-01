using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LibCS2C.Generators
{
    public class ObjectCreationExpressionGenerator : GeneratorBase<ObjectCreationExpressionSyntax>
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
            if (type.TypeKind == TypeKind.Class)
            {
                IdentifierNameSyntax identifier = node.ChildNodes().First() as IdentifierNameSyntax;
                ISymbol symbol = m_context.Model.GetSymbolInfo(node).Symbol;

                ImmutableArray<SyntaxReference> references = symbol.DeclaringSyntaxReferences;
                bool hasConstructor = (references.Length > 0);

                // Call constructor
                if (hasConstructor)
                {
                    m_context.Writer.Append(m_context.Generators.MethodDeclaration.CreateMethodPrototype((IMethodSymbol)symbol, false, false));
                    m_context.Writer.Append("(");
                }

                // Class initialization (returns the object, we can pass it as an argument to the constructor)
                m_context.Writer.Append(string.Format("classInit_{0}_{1}()", nameSpace, type.Name));

                // Remaining arguments (if constructor)
                if (hasConstructor)
                {
                    int argCount = args.ChildNodes().Count();
                    if (argCount > 0)
                        m_context.Writer.Append(", ");

                    m_context.Generators.ArgumentList.Generate(args);
                    m_context.Writer.Append(")");
                }
            }
            // Struct
            else
            {
                m_context.Writer.Append(string.Format("structInit_{0}_{1}()", type.ContainingSymbol.ToString().Replace(".", "_"), type.Name));
            }
        }
    }
}
