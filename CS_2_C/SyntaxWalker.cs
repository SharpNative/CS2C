using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CS_2_C
{
    public class SyntaxWalker : CSharpSyntaxWalker
    {
        private FormattedStringBuilder m_sb;
        private TypeConversion m_convert;

        private ClassDeclarationSyntax m_currentClass;
        private NamespaceDeclarationSyntax m_currentNamespace;
        private int m_curBraces;

        /// <summary>
        /// Walks through the syntax and outputs C code to a <see cref="FormattedStringBuilder">FormattedStringBuilder</see>
        /// </summary>
        /// <param name="sb">The formatted string builder</param>
        public SyntaxWalker(FormattedStringBuilder sb) : base(SyntaxWalkerDepth.Token)
        {
            m_convert = new TypeConversion();
            m_sb = sb;
            m_curBraces = 0;
        }

        /// <summary>
        /// Visits a class declaration
        /// </summary>
        /// <param name="node">The class declaration node</param>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            m_currentClass = node;
            m_sb.AppendLine("/* Class <" + node.Identifier + "> */");
            base.VisitClassDeclaration(node);
        }

        /// <summary>
        /// Visits a variable declaration
        /// </summary>
        /// <param name="node">The variable declaration node</param>
        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            foreach(VariableDeclaratorSyntax variable in node.Variables)
            {
                // Convert type
                TypeSyntax type = node.Type;
                string typeNameConverted;
                if (m_convert.IsGeneric(type))
                {
                    typeNameConverted = m_convert.Convert(type);
                }
                else
                {
                    throw new NotImplementedException();
                }

                m_sb.AppendIndent();
                m_sb.AppendLine(string.Format("/* Variable {0} */", variable.Identifier));
                m_sb.AppendIndent();
                m_sb.AppendLine(string.Format("{0} {1} {2};", typeNameConverted, variable.Identifier, variable.Initializer));
            }

            base.VisitVariableDeclaration(node);
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
                    memberName = m_currentNamespace.Name.ToString() + "_" + m_currentClass.Identifier + "_" + name.Identifier;
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
            m_sb.AppendLine("/* Method <" + node.Identifier + "> */");

            TypeSyntax type = node.ReturnType;
            string typeNameConverted;
            if(m_convert.IsGeneric(type))
            {
                typeNameConverted = m_convert.Convert(type);
            }
            else
            {
                throw new NotImplementedException();
            }

            // namespaceName_className_methodName
            m_sb.Append(string.Format("{0} {1}_{2}_{3}(", typeNameConverted, m_currentNamespace.Name.ToString().Replace(".", "_"), m_currentClass.Identifier, node.Identifier));

            // Check for parameters
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach(SyntaxNode childNode in nodes)
            {
                if(childNode.Kind() == SyntaxKind.ParameterList)
                {
                    // Get parameters
                    ParameterListSyntax param = childNode as ParameterListSyntax;
                    IEnumerable<SyntaxNode> paramNodes = param.ChildNodes();

                    // TODO: out and ref
                    foreach (ParameterSyntax paramNode in paramNodes)
                    {
                        TypeSyntax paramType = paramNode.Type;
                        string paramTypeNameConverted;
                        if (m_convert.IsGeneric(paramType))
                        {
                            paramTypeNameConverted = m_convert.Convert(paramType);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        m_sb.Append(string.Format("{0} {1}", paramTypeNameConverted, paramNode.Identifier));

                        // A comma if it's not the last parameter
                        if (paramNode != paramNodes.Last())
                            m_sb.Append(", ");
                    }
                }
            }

            m_sb.AppendLine(")");

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
            m_currentNamespace = node;
            m_sb.AppendLine("/* Namespace <" + node.Name + "> */");
            base.VisitNamespaceDeclaration(node);
        }

        /// <summary>
        /// Visits a node
        /// </summary>
        /// <param name="node">The node</param>
        public override void Visit(SyntaxNode node)
        {
            /*Console.WriteLine(node.GetText());
            Console.WriteLine("Visited " + node.GetType());*/
            base.Visit(node);
        }
    }
}
