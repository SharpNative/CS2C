using LibCS2C.Compilation;
using LibCS2C.Context;
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
        /// Create method arguments and parameters
        /// </summary>
        /// <param name="node">The node</param>
        /// <param name="createType">If a function pointer type is used</param>
        /// <returns>An array, index 0 contains the arguments, index 1 contains the parameter types, index 2 contains the argument names</returns>
        public string[] CreateMethodArgsAndParams(BaseMethodDeclarationSyntax node, bool createType)
        {
            // Check for parameters
            IEnumerable<SyntaxNode> paramNodes = node.ParameterList.ChildNodes();

            // Check if the method is static
            bool isStatic = IsMethodStatic(node);

            // Count parameters, non-static methods require an object reference
            int paramCount = paramNodes.Count();
            if (!isStatic)
                paramCount++;

            // Method name: namespaceName_className_methodName_PARAMCOUNT_PARAMTYPES_
            StringBuilder argumentBuilder = new StringBuilder();
            StringBuilder paramTypeBuilder = new StringBuilder();
            StringBuilder argNameBuilder = new StringBuilder();

            // Not static? Object reference is required as parameter
            if (!isStatic)
            {
                paramTypeBuilder.Append("class_");
                argNameBuilder.Append("obj");
                argumentBuilder.Append(string.Format("{0}* obj", (createType) ? "void" : m_context.TypeConvert.CurrentClassStructName));

                if (paramCount > 1)
                {
                    argumentBuilder.Append(", ");
                    argNameBuilder.Append(", ");
                }
            }

            // Parameters
            foreach (ParameterSyntax paramNode in paramNodes)
            {
                string typeName = m_context.ConvertTypeName(paramNode.Type);
                argNameBuilder.Append(paramNode.Identifier);
                paramTypeBuilder.Append(typeName.Replace(' ', '_').Replace('*', '_'));
                paramTypeBuilder.Append("_");

                argumentBuilder.Append(string.Format("{0} {1}", typeName, paramNode.Identifier));

                // A comma if it's not the last parameter
                if (paramNode != paramNodes.Last())
                {
                    argumentBuilder.Append(", ");
                    argNameBuilder.Append(", ");
                }
            }

            // Insert void if no parameters are found
            if (paramCount == 0)
                argumentBuilder.Append("void");

            return new string[] { argumentBuilder.ToString(), paramTypeBuilder.ToString(), argNameBuilder.ToString() };
        }

        /// <summary>
        /// Creates the method prototype
        /// </summary>
        /// <param name="node">The method declaration</param>
        /// <param name="generateArguments">If the arguments should be generated</param>
        /// <param name="createType">If a function pointer type should be created</param>
        /// <returns>The string containing the prototype</returns>
        public string CreateMethodPrototype(BaseMethodDeclarationSyntax node, bool generateArguments, bool createType)
        {
            SyntaxToken identifier;
            if (node is MethodDeclarationSyntax)
            {
                identifier = (node as MethodDeclarationSyntax).Identifier;
            }
            else /*if (node is ConstructorDeclarationSyntax)*/
            {
                identifier = (node as ConstructorDeclarationSyntax).Identifier;
            }

            TypeDeclarationSyntax parent = node.Parent as TypeDeclarationSyntax;
            NamespaceDeclarationSyntax nameSpace = parent.Parent as NamespaceDeclarationSyntax;

            // Check for parameters
            IEnumerable<SyntaxNode> paramNodes = node.ParameterList.ChildNodes();

            // Count parameters, non-static methods require an object reference
            int paramCount = paramNodes.Count();
            if (!IsMethodStatic(node))
                paramCount++;

            string methodPrototype = string.Format("{0}_{1}_{2}_{3}", nameSpace.Name.ToString().Replace('.', '_'), parent.Identifier.ToString(), identifier, paramCount);
            if (createType)
                methodPrototype = "(*fp_" + methodPrototype;

            // Create method arguments and parameters
            string[] argsAndParams = CreateMethodArgsAndParams(node, createType);

            methodPrototype += argsAndParams[1];
            if (createType)
                methodPrototype += ")";

            if (generateArguments)
                methodPrototype += "(" + argsAndParams[0] + ")";

            return methodPrototype;
        }

        /// <summary>
        /// Processes a plug attribute
        /// </summary>
        /// <param name="node">The node</param>
        /// <param name="attribute">The plug attribute</param>
        private void processAttributePlug(BaseMethodDeclarationSyntax node, AttributeSyntax attribute)
        {
            SeparatedSyntaxList<AttributeArgumentSyntax> argsList = attribute.ArgumentList.Arguments;

            // We expect there to be one argument with the name of the method to plug/override
            if (argsList.Count != 1)
                throw new Exception("Invalid use of Plug: argument count incorrect!");

            // The argument needs to be a string
            AttributeArgumentSyntax argument = argsList.First();
            SyntaxNode argumentContents = argument.ChildNodes().First();
            if (argumentContents.Kind() != SyntaxKind.StringLiteralExpression)
                throw new Exception("Invalid use of Plug: expected a string!");

            // If we take the stringliteral, there will be "" on the outside of the string
            string overrideName = argumentContents.ToString();
            overrideName = overrideName.Substring(1, overrideName.Length - 2);

            // Return type
            MethodDeclarationSyntax nodeTyped = node as MethodDeclarationSyntax;
            string returnType = m_context.ConvertTypeName(((MethodDeclarationSyntax)node).ReturnType);

            // Method arguments
            // Note that we use "inline static" here to make sure the call code is inlined
            string[] argsAndParams = CreateMethodArgsAndParams(node, false);
            string args = argsAndParams[0];
            string argNames = argsAndParams[2];
            string methodPrototype = string.Format("inline static {0} {1}({2})", returnType, overrideName, args);

            // Append to prototypes
            m_context.Writer.CurrentDestination = WriterDestination.MethodPrototypes;
            m_context.Writer.Append(methodPrototype);
            m_context.Writer.AppendLine(";");

            // Append the declaration so we can add contents
            m_context.Writer.CurrentDestination = WriterDestination.MethodDeclarations;
            m_context.Writer.AppendLine(methodPrototype);
            m_context.Writer.AppendLine("{");

            // Does it have a return value? Then don't only pass the method but also do a return
            if (!returnType.Equals("void"))
                m_context.Writer.Append("return ");

            // Call to real method implementation
            string methodName = CreateMethodPrototype(node, false, false);
            m_context.Writer.Append(methodName);
            m_context.Writer.Append("(");
            m_context.Writer.Append(argNames);
            m_context.Writer.AppendLine(");");

            m_context.Writer.AppendLine("}");
        }

        /// <summary>
        /// Processes the attributes
        /// </summary>
        /// <param name="node">The method node</param>
        private void processAttributes(BaseMethodDeclarationSyntax node)
        {
            SeparatedSyntaxList<AttributeSyntax> attributes = node.AttributeLists[0].Attributes;
            if (attributes.Count > 0)
            {
                foreach (AttributeSyntax attribute in attributes)
                {
                    IdentifierNameSyntax name = attribute.Name as IdentifierNameSyntax;
                    string attribIdentifier = name.Identifier.ToString();

                    // Plug
                    if (attribIdentifier.Equals("Plug"))
                    {
                        processAttributePlug(node, attribute);
                    }
                    // Unknown
                    else
                    {
                        throw new NotImplementedException("Unknown attribute on method: " + attribIdentifier);
                    }
                }
            }
        }

        /// <summary>
        /// Generates a method declaration
        /// </summary>
        /// <param name="node">The base method declaration</param>
        public override void Generate(BaseMethodDeclarationSyntax node)
        {
            string methodPrototype = "";
            string methodTypedef = "";

            bool isInsideClass = (node.Parent.Kind() == SyntaxKind.ClassDeclaration);
            bool isConstructor = (node.Kind() == SyntaxKind.ConstructorDeclaration);

            // Type of method
            if (isConstructor)
            {
                ConstructorDeclarationSyntax nodeTyped = node as ConstructorDeclarationSyntax;
                SyntaxToken identifier = nodeTyped.Identifier;
                methodPrototype = string.Format("void* {0}", CreateMethodPrototype(node, true, false));
            }
            else
            {
                MethodDeclarationSyntax nodeTyped = node as MethodDeclarationSyntax;
                SyntaxToken identifier = nodeTyped.Identifier;
                string returnType = m_context.ConvertTypeName(nodeTyped.ReturnType);

                methodTypedef = string.Format("typedef {0} {1}", returnType, CreateMethodPrototype(node, true, true));
                if (isInsideClass)
                {
                    m_context.MethodTable.Add(nodeTyped);
                    methodPrototype = string.Format("{0} {1}", returnType, CreateMethodPrototype(node, true, false));
                }
            }

            // Process attributes if needed
            if (node.AttributeLists.Count > 0)
                processAttributes(node);

            // Append to prototypes
            m_context.Writer.CurrentDestination = WriterDestination.MethodPrototypes;
            if (methodPrototype.Length > 0)
            {
                m_context.Writer.Append(methodPrototype);
                m_context.Writer.AppendLine(";");
            }
            m_context.Writer.Append(methodTypedef);
            m_context.Writer.AppendLine(";");

            // If this has no body, we only generate the prototype
            if (node.Body == null)
                return;

            // Append the declaration so we can add contents
            m_context.Writer.CurrentDestination = WriterDestination.MethodDeclarations;
            m_context.Writer.AppendLine(methodPrototype);

            // Block containing the code of the method
            m_context.Writer.AppendLine("{");

            // If it's not static, we should check
            if (CompilerSettings.EnableRuntimeChecks && !IsMethodStatic(node))
            {
                m_context.Writer.AppendLine("\tif(obj == NULL) fatal(__ERROR_NULL_CALLED__);");
            }

            m_context.Generators.Block.Generate(node.Body);

            // If the method is a constructor, we need to return the object
            if (isConstructor)
                m_context.Writer.AppendLine("\treturn obj;");

            m_context.Writer.AppendLine("}");
        }
    }
}
