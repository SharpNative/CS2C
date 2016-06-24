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

        public MethodGenerator(WalkerContext context, MethodGeneratorType type)
        {
            m_context = context;
            m_type = type;
        }

        public void Generate(BaseMethodDeclarationSyntax node)
        {
            SyntaxToken identifier = default(SyntaxToken);
            string returnType = null;

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
                m_context.Writer.AppendLine("/* Method <" + identifier + "> */");
            }
            
            // namespaceName_className_methodName
            m_context.Writer.Append(string.Format("{0} {1}_{2}(", returnType, m_context.CurrentClassNameFormatted, identifier));

            // Check for parameters
            int paramCount = 0;
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
                        m_context.Writer.Append(string.Format("{0} {1}", m_context.ConvertTypeName(paramNode.Type), paramNode.Identifier));

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
