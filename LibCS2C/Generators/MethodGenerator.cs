using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibCS2C.Generators
{
    public class MethodGenerator : GeneratorBase<BaseMethodDeclarationSyntax>
    {
        /// <summary>
        /// Method declaration generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public MethodGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Checks if the method is static
        /// </summary>
        /// <param name="node">The method declaration node</param>
        /// <returns>If it's static</returns>
        public bool IsMethodStatic(BaseMethodDeclarationSyntax node)
        {
            IEnumerable<SyntaxToken> tokens = node.ChildTokens();
            foreach (SyntaxToken token in tokens)
            {
                if (token.Kind() == SyntaxKind.StaticKeyword)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the method prototype
        /// </summary>
        /// <param name="node">The method declaration</param>
        /// <param name="generateParams">If the parameters should be generated</param>
        /// <returns>The string containing the prototype</returns>
        public string CreateMethodPrototype(BaseMethodDeclarationSyntax node, bool generateParams)
        {
            // Check for parameters
            ParameterListSyntax param = node.ParameterList;
            IEnumerable<SyntaxNode> paramNodes = param.ChildNodes();

            // Check if the method is static
            bool isStatic = IsMethodStatic(node);

            // Count parameters, non-static methods require an object reference
            int paramCount = paramNodes.Count();
            if (!isStatic)
                paramCount++;
            
            // Method name: namespaceName_className_methodName_PARAMCOUNT_PARAMTYPES
            StringBuilder argumentBuilder = new StringBuilder();
            StringBuilder paramTypeBuilder = new StringBuilder();

            SyntaxToken identifier;
            if(node is MethodDeclarationSyntax)
            {
                identifier = (node as MethodDeclarationSyntax).Identifier;
            }
            else if(node is ConstructorDeclarationSyntax)
            {
                identifier = (node as ConstructorDeclarationSyntax).Identifier;
            }
            else
            {
                throw new NotImplementedException();
            }

            ClassDeclarationSyntax classDeclaration = node.Parent as ClassDeclarationSyntax;
            NamespaceDeclarationSyntax nameSpace = classDeclaration.Parent as NamespaceDeclarationSyntax;
            string methodPrototype = string.Format("{0}_{1}_{2}_{3}", nameSpace.Name.ToString().Replace('.', '_'), classDeclaration.Identifier.ToString(), identifier, paramCount);

            // Not static? Object reference is required as parameter
            if (!isStatic)
            {
                paramTypeBuilder.Append("class_");
                argumentBuilder.Append(string.Format("{0}* obj", m_context.CurrentClassStructName));
                if (paramCount > 1)
                    argumentBuilder.Append(", ");
            }
            
            // TODO: out and ref
            foreach (ParameterSyntax paramNode in paramNodes)
            {
                string typeName = m_context.ConvertTypeName(paramNode.Type);
                paramTypeBuilder.Append(typeName.Replace(' ', '_').Replace('*', '_'));
                paramTypeBuilder.Append("_");
                
                argumentBuilder.Append(string.Format("{0} {1}", typeName, paramNode.Identifier));

                // A comma if it's not the last parameter
                if (paramNode != paramNodes.Last())
                    argumentBuilder.Append(", ");
            }

            // Insert void if no parameters are found
            if (generateParams && paramCount == 0)
            {
                argumentBuilder.Append("void");
            }

            methodPrototype += paramTypeBuilder.ToString();
            if(generateParams)
                methodPrototype += "(" + argumentBuilder.ToString() + ")";

            return methodPrototype;
        }

        /// <summary>
        /// Generates a method declaration
        /// </summary>
        /// <param name="node">The base method declaration</param>
        public override void Generate(BaseMethodDeclarationSyntax node)
        {
            SyntaxToken identifier = default(SyntaxToken);
            string returnType;
            bool isConstructor = (node.Kind() == SyntaxKind.ConstructorDeclaration);

            // Constructor of method declaration
            if (isConstructor)
            {
                ConstructorDeclarationSyntax nodeTyped = node as ConstructorDeclarationSyntax;
                identifier = nodeTyped.Identifier;
                returnType = m_context.CurrentClassStructName + "*";
            }
            else
            {
                MethodDeclarationSyntax nodeTyped = node as MethodDeclarationSyntax;
                identifier = nodeTyped.Identifier;
                returnType = m_context.ConvertTypeName(nodeTyped.ReturnType);
            }

            string methodPrototype = returnType + " " + CreateMethodPrototype(node, true);

            // Append to properties
            m_context.CurrentDestination = WriterDestination.MethodPrototypes;
            m_context.Writer.Append(methodPrototype);
            m_context.Writer.AppendLine(";");

            // If this has no body, we only generate the prototype
            if (node.Body == null)
                return;

            // Append the declaration so we can add contents
            m_context.CurrentDestination = WriterDestination.MethodDeclarations;
            m_context.Writer.AppendLine(methodPrototype);

            // Block containing the code of the method
            m_context.Writer.AppendLine("{");

            m_context.Generators.Block.Generate(node.Body);

            // If the method is a constructor, we need to return the object
            if (isConstructor)
                m_context.Writer.AppendLine("\treturn obj;");

            m_context.Writer.AppendLine("}");
        }
    }
}
