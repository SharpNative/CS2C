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
    enum MethodGeneratorType
    {
        Method = 0,
        Constructor = 1
    }

    class MethodGenerator: IGenerator<BaseMethodDeclarationSyntax>
    {
        private WalkerContext m_context;
        private MethodGeneratorType m_type;

        /// <summary>
        /// Method declaration generator
        /// </summary>
        /// <param name="context">The walker context</param>
        /// <param name="type">The method type: constructor or method</param>
        public MethodGenerator(WalkerContext context, MethodGeneratorType type)
        {
            m_context = context;
            m_type = type;
        }

        /// <summary>
        /// Generates a method declaration
        /// </summary>
        /// <param name="node">The base method declaration</param>
        public void Generate(BaseMethodDeclarationSyntax node)
        {
            SyntaxToken identifier = default(SyntaxToken);
            string returnType = null;

            // Static methods don't require a reference to the object as parameter
            bool isStatic = false;

            if(m_type == MethodGeneratorType.Constructor)
            {
                ConstructorDeclarationSyntax nodeTyped = node as ConstructorDeclarationSyntax;
                identifier = nodeTyped.Identifier;
                returnType = m_context.CurrentClassStructName + "*";

                m_context.Writer.AppendLine("/* Constructor <" + identifier + "> */");
            }
            else if(m_type == MethodGeneratorType.Method)
            {
                MethodDeclarationSyntax nodeTyped = node as MethodDeclarationSyntax;
                identifier = nodeTyped.Identifier;
                returnType = m_context.ConvertTypeName(nodeTyped.ReturnType);
                
                // Check if the method is static
                IEnumerable<SyntaxToken> tokens = nodeTyped.ChildTokens();
                foreach(SyntaxToken token in tokens)
                {
                    if(token.Kind() == SyntaxKind.StaticKeyword)
                    {
                        isStatic = true;
                        break;
                    }
                }

                if(isStatic)
                    m_context.Writer.AppendLine("/* Static Method <" + identifier + "> */");
                else
                    m_context.Writer.AppendLine("/* Method <" + identifier + "> */");
            }
            
            // namespaceName_className_methodName
            m_context.Writer.Append(string.Format("{0} {1}_{2}(", returnType, m_context.CurrentClassNameFormatted, identifier));
            
            // Not static? Object reference is required as parameter
            if(!isStatic)
            {
                m_context.Writer.Append(string.Format("{0}* obj, ", m_context.CurrentClassStructName));
            }

            // Check for parameters
            int paramCount = (isStatic ? 0 : 1);
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (SyntaxNode childNode in nodes)
            {
                if (childNode.Kind() == SyntaxKind.ParameterList)
                {
                    // Get parameters
                    ParameterListSyntax param = childNode as ParameterListSyntax;
                    IEnumerable<SyntaxNode> paramNodes = param.ChildNodes();
                    paramCount = paramNodes.Count();

                    // TODO: out and ref
                    foreach (ParameterSyntax paramNode in paramNodes)
                    {
                        m_context.Writer.Append(string.Format("{0} param_{1}", m_context.ConvertTypeName(paramNode.Type), paramNode.Identifier));

                        // A comma if it's not the last parameter
                        if (paramNode != paramNodes.Last())
                            m_context.Writer.Append(", ");
                    }
                }
            }

            // Insert void if no parameters are found
            if (paramCount == 0)
            {
                m_context.Writer.Append("void");
            }

            m_context.Writer.AppendLine(")");
        }
    }
}
