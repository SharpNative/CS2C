using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CS_2_C.Generators
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

            // NewKeyword IdentifierName ArgumentList
            IdentifierNameSyntax name = objNodes.First() as IdentifierNameSyntax;
            ArgumentListSyntax args = node.ArgumentList;
            string nameSpace = m_context.Model.GetTypeInfo(node).Type.ContainingNamespace.ToString().Replace(".", "_");

            // Class initialization
            m_context.Writer.AppendLine(string.Format("classInit_{0}_{1}();", nameSpace, name.Identifier));

            // Call Constructor
            m_context.Writer.AppendIndent();
            m_context.Writer.Append(string.Format("{0}_{1}_{2}(", nameSpace, name.Identifier, name.Identifier));

            // Own object as argument
            // TODO: fix this
            m_context.Writer.Append(/*variable.Identifier.ToString()*/"variable");

            // Remaining arguments
            int argCount = args.ChildNodes().Count();
            if (argCount > 0)
                m_context.Writer.Append(", ");

            ArgumentListGenerator argGen = new ArgumentListGenerator(m_context);
            argGen.Generate(args);

            m_context.Writer.Append(")");
        }
    }
}
