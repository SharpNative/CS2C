using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
{
    class VariableGenerator : GeneratorBase<VariableDeclarationSyntax>
    {
        /// <summary>
        /// Variable declaration generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public VariableGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generate code for variable declaration
        /// </summary>
        /// <param name="node"></param>
        public override void Generate(VariableDeclarationSyntax node)
        {
            foreach (VariableDeclaratorSyntax variable in node.Variables)
            {
                string typeName = m_context.ConvertTypeName(node.Type);

                m_context.Writer.AppendIndent();
                m_context.Writer.AppendLine(string.Format("/* Variable {0} */", variable.Identifier));
                m_context.Writer.AppendIndent();
                m_context.Writer.Append(string.Format("{0} {1} = ", typeName, variable.Identifier));

                IEnumerable<SyntaxNode> nodes = variable.Initializer.ChildNodes();
                foreach(SyntaxNode childNode in nodes)
                {
                    SyntaxKind kind = childNode.Kind();
                    if(kind == SyntaxKind.ObjectCreationExpression)
                    {
                        ObjectCreationExpressionSyntax obj = childNode as ObjectCreationExpressionSyntax;
                        IEnumerable<SyntaxNode> objNodes = obj.ChildNodes();
                        
                        // NewKeyword IdentifierName ArgumentList
                        IdentifierNameSyntax name = objNodes.First() as IdentifierNameSyntax;
                        ArgumentListSyntax args = obj.ArgumentList;
                        string nameSpace = m_context.Model.GetTypeInfo(obj).Type.ContainingNamespace.ToString().Replace(".", "_");

                        // Class initialization
                        m_context.Writer.AppendLine(string.Format("classInit_{0}_{1}();", nameSpace, name.Identifier));

                        // Call Constructor
                        m_context.Writer.AppendIndent();
                        m_context.Writer.Append(string.Format("{0}_{1}_{2}(", nameSpace, name.Identifier, name.Identifier));

                        // Own object as argument
                        m_context.Writer.Append(variable.Identifier.ToString());

                        int argCount = args.ChildNodes().Count();
                        if (argCount > 0)
                            m_context.Writer.Append(", ");

                        ArgumentListGenerator argGen = new ArgumentListGenerator(m_context);
                        argGen.Generate(args);
                        
                        m_context.Writer.Append(")");
                    }
                    else
                    {
                        m_context.Writer.Append(childNode.ToString());
                    }
                }

                m_context.Writer.AppendLine(";");
            }
        }
    }
}
