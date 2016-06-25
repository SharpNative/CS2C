using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CS_2_C.Generators;

namespace CS_2_C
{
    public class SyntaxWalker : CSharpSyntaxWalker
    {
        private FormattedStringBuilder m_sb;
        private WalkerContext m_context;

        private int m_curBraces;

        private MethodGenerator m_methodGen;
        private MethodGenerator m_constructorGen;

        /// <summary>
        /// Walks through the syntax and outputs C code to a <see cref="FormattedStringBuilder">FormattedStringBuilder</see>
        /// </summary>
        /// <param name="sb">The formatted string builder</param>
        public SyntaxWalker(FormattedStringBuilder sb) : base(SyntaxWalkerDepth.Token)
        {
            m_sb = sb;
            m_curBraces = 0;
            m_context = new WalkerContext(sb);

            // Generators
            m_methodGen = new MethodGenerator(m_context, MethodGeneratorType.Method);
            m_constructorGen = new MethodGenerator(m_context, MethodGeneratorType.Constructor);
        }

        /// <summary>
        /// Visits a class declaration
        /// </summary>
        /// <param name="node">The class declaration node</param>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            m_context.CurrentClass = node;
            m_sb.AppendLine("/* Class <" + node.Identifier + "> */");

            // Struct for classes
            m_sb.AppendLine(m_context.CurrentClassStructName);
            m_sb.AppendLine("{");

            // Temporarily hold all the fields so we can put them in the initialization method
            Dictionary<string, EqualsValueClauseSyntax> staticFields = new Dictionary<string, EqualsValueClauseSyntax>();
            Dictionary<string, EqualsValueClauseSyntax> nonStaticFields = new Dictionary<string, EqualsValueClauseSyntax>();
            Dictionary<string, TypeSyntax> staticFieldTypes = new Dictionary<string, TypeSyntax>();

            #region Get the needed info for fields and create struct

            // Loop through the children to find the fields
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(SyntaxNode childNode in nodes)
            {
                // Found a field
                if(childNode.Kind() == SyntaxKind.FieldDeclaration)
                {
                    FieldDeclarationSyntax fieldNode = childNode as FieldDeclarationSyntax;
                    IEnumerable<SyntaxNode> fieldNodeChildren = fieldNode.ChildNodes();

                    bool isStatic = false;

                    IEnumerable<SyntaxToken> fieldNodeTokens = fieldNode.ChildTokens();
                    foreach(SyntaxToken token in fieldNodeTokens)
                    {
                        if (token.Kind() == SyntaxKind.StaticKeyword)
                            isStatic = true;
                    }

                    foreach (SyntaxNode fieldNodeChild in fieldNodeChildren)
                    {
                        VariableDeclarationSyntax variableDeclaration = fieldNodeChild as VariableDeclarationSyntax;
                        foreach (VariableDeclaratorSyntax variable in variableDeclaration.Variables)
                        {
                            m_sb.AppendLine("\t/* Field: " + variable.Identifier + " */");
                            m_sb.AppendLine(string.Format("\t{0} {1};", m_context.ConvertTypeName(variableDeclaration.Type), variable.Identifier));

                            if (isStatic)
                            {
                                staticFields.Add(variable.Identifier.ToString(), variable.Initializer);
                                staticFieldTypes.Add(variable.Identifier.ToString(), variableDeclaration.Type);
                            }
                            else
                            {
                                nonStaticFields.Add(variable.Identifier.ToString(), variable.Initializer);
                            }
                        }
                    }
                }
            }

            // End of struct
            m_sb.AppendLine("};");

            #endregion

            #region Static fields

            string convertedClassName = m_context.ConvertClassName(node.Identifier.ToString());

            m_sb.AppendLine("struct");
            m_sb.AppendLine("{");
            foreach(KeyValuePair<string, EqualsValueClauseSyntax> pair in staticFields)
            {
                m_sb.AppendLine(string.Format("\t{0} {1};", staticFieldTypes[pair.Key], pair.Key));
            }
            m_sb.Append("}");
            m_sb.AppendLine(string.Format(" classStatics_{0};", convertedClassName));
            m_sb.AppendLine("");
            
            // .cctor
            m_sb.AppendLine(string.Format("void classCctor_{0}(void)", convertedClassName));
            m_sb.AppendLine("{");
            foreach (KeyValuePair<string, EqualsValueClauseSyntax> pair in staticFields)
            {
                m_sb.AppendLine(string.Format("\tclassStatics_{0}.{1} {2};", convertedClassName, pair.Key, staticFields[pair.Key]));
            }
            m_sb.AppendLine("}");
            m_sb.AppendLine("");

            #endregion

            #region Class initialization method

            // Class initialization method: returns a pointer to this object
            m_sb.AppendLine("");
            m_sb.AppendLine(string.Format("{0}* classInit_{1}_{2}(void)", m_context.CurrentClassStructName, m_context.CurrentNamespaceFormatted, node.Identifier));
            m_sb.AppendLine("{");
            m_sb.AppendLine(string.Format("\t{0}* object = malloc(sizeof({0}));", m_context.CurrentClassStructName));
            m_sb.AppendLine("\tif(!object)");
            m_sb.AppendLine("\t\treturn NULL;");
            
            // Loop through the fields and initialize them
            foreach(KeyValuePair<string, EqualsValueClauseSyntax> pair in nonStaticFields)
            {
                m_sb.AppendLine(string.Format("\tobject->{0} {1};", pair.Key, pair.Value));
            }
            
            m_sb.AppendLine("\treturn object;");
            m_sb.AppendLine("}");
            m_sb.AppendLine("");

            #endregion

            base.VisitClassDeclaration(node);
        }

        /// <summary>
        /// Visits a variable declaration
        /// </summary>
        /// <param name="node">The variable declaration node</param>
        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            // If the current braces is under 3, it means we're inside a class definition, outside a method
            if (m_curBraces < 3)
                return;

            foreach(VariableDeclaratorSyntax variable in node.Variables)
            {
                m_sb.AppendIndent();
                m_sb.AppendLine(string.Format("/* Variable {0} */", variable.Identifier));
                m_sb.AppendIndent();
                m_sb.AppendLine(string.Format("{0} {1} {2};", m_context.ConvertTypeName(node.Type), variable.Identifier, variable.Initializer));
            }

            base.VisitVariableDeclaration(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            m_constructorGen.Generate(node);
            base.VisitConstructorDeclaration(node);
        }

        /// <summary>
        /// Visits an expression
        /// </summary>
        /// <param name="statementNode">The expression node</param>
        public override void VisitExpressionStatement(ExpressionStatementSyntax statementNode)
        {
            IEnumerable<SyntaxNode> nodes = statementNode.Expression.ChildNodes();
            SyntaxKind kind = statementNode.Expression.Kind();

            if(kind == SyntaxKind.SimpleAssignmentExpression)
            {
                string code = statementNode.GetText().ToString().Trim();
                m_sb.AppendIndent();
                m_sb.AppendLine(string.Format("/* Expression {0} */", code));

                m_sb.AppendIndent();
                m_sb.AppendLine(code);
            }
            else if(kind == SyntaxKind.InvocationExpression)
            {
                // IdentifierNameSyntax -> own class
                // ... -> other class
                SyntaxNode first = nodes.First();
                SyntaxKind firstKind = first.Kind();

                string memberName;
                // Own class
                if(firstKind == SyntaxKind.IdentifierName)
                {
                    IdentifierNameSyntax name = nodes.First() as IdentifierNameSyntax;
                    memberName = m_context.CurrentClassNameFormatted + "_" + name.Identifier;
                }
                // Another class
                else if(firstKind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    MemberAccessExpressionSyntax name = nodes.First() as MemberAccessExpressionSyntax;
                    memberName = name.ToFullString().Trim().Replace(".", "_");
                }
                else
                {
                    throw new NotSupportedException();
                }

                m_sb.AppendIndent();
                m_sb.Append(string.Format("{0}(", memberName));

                // Arguments
                foreach(SyntaxNode node in nodes)
                {
                    if(node.Kind() == SyntaxKind.ArgumentList)
                    {
                        ArgumentListSyntax args = node as ArgumentListSyntax;
                        IEnumerable<SyntaxNode> argNodes = args.ChildNodes();

                        foreach (ArgumentSyntax argument in argNodes)
                        {
                            m_sb.Append(argument.ToString());

                            // A comma if it's not the last argument
                            if (argument != argNodes.Last())
                                m_sb.Append(", ");
                        }
                    }
                }

                m_sb.AppendLine(");");
            }

            base.VisitExpressionStatement(statementNode);
        }

        /// <summary>
        /// Visits a method declaration
        /// </summary>
        /// <param name="node">The method declaration node</param>
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            m_methodGen.Generate(node);
            base.VisitMethodDeclaration(node);
        }

        /// <summary>
        /// Visits a token
        /// </summary>
        /// <param name="token">The token</param>
        public override void VisitToken(SyntaxToken token)
        {
            SyntaxKind kind = token.Kind();
            if (kind == SyntaxKind.CloseBraceToken)
            {
                // First two braces are from namespace and class
                if(m_curBraces > 2)
                {
                    m_sb.UnIndent();
                    m_sb.AppendLine("}");
                }
                
                m_curBraces--;
            }
            else if(kind == SyntaxKind.OpenBraceToken)
            {
                // First two braces are from namespace and class
                if (m_curBraces > 1)
                {
                    m_sb.Indent();
                    m_sb.AppendLine("{");
                }
                
                m_curBraces++;
            }

            base.VisitToken(token);
        }

        /// <summary>
        /// Visits a namespace declaration
        /// </summary>
        /// <param name="node">The namespace declaration node</param>
        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            m_context.CurrentNamespace = node;
            m_sb.AppendLine("/* Namespace <" + node.Name + "> */");
            base.VisitNamespaceDeclaration(node);
        }

        /// <summary>
        /// Visits a node
        /// </summary>
        /// <param name="node">The node</param>
        public override void Visit(SyntaxNode node)
        {
            //Console.WriteLine("Visited " + node.GetType());
            base.Visit(node);
        }
    }
}
