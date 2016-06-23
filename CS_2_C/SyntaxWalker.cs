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

        public SyntaxWalker(FormattedStringBuilder sb) : base(SyntaxWalkerDepth.Token)
        {
            m_convert = new TypeConversion();
            m_sb = sb;
            m_curBraces = 0;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            m_currentClass = node;
            m_sb.AppendLine("/* Class <" + node.Identifier + "> */");
            base.VisitClassDeclaration(node);
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            foreach(VariableDeclaratorSyntax variable in node.Variables)
            {
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
                if(firstKind == SyntaxKind.IdentifierName)
                {
                    IdentifierNameSyntax name = nodes.First() as IdentifierNameSyntax;
                    memberName = m_currentNamespace.Name.ToString() + "_" + m_currentClass.Identifier + "_" + name.Identifier;
                }
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
                foreach(SyntaxNode node in nodes)
                {
                    if(node.Kind() == SyntaxKind.ArgumentList)
                    {
                        ArgumentListSyntax args = node as ArgumentListSyntax;
                        IEnumerable<SyntaxNode> argNodes = args.ChildNodes();
                        foreach (ArgumentSyntax argument in argNodes)
                        {
                            Console.WriteLine(argument.ToString());
                            m_sb.Append(argument.ToString());

                            if (argument != argNodes.Last())
                                m_sb.Append(", ");
                        }
                    }
                }

                m_sb.AppendLine(");");
            }

            base.VisitExpressionStatement(statementNode);
        }

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

            // className_methodName
            m_sb.AppendLine(string.Format("{0} {1}_{2}_{3}()", typeNameConverted, m_currentNamespace.Name.ToString().Replace(".", "_"), m_currentClass.Identifier, node.Identifier));

            base.VisitMethodDeclaration(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            //Console.WriteLine("identifier: "+node.Identifier);
            base.VisitIdentifierName(node);
        }

        public override void VisitToken(SyntaxToken token)
        {
            SyntaxKind kind = token.Kind();
            if (kind == SyntaxKind.CloseBraceToken)
            {
                if(m_curBraces > 2)
                {
                    m_sb.UnIndent();
                    m_sb.AppendLine("}");
                }
                
                m_curBraces--;
            }
            else if(kind == SyntaxKind.OpenBraceToken)
            {
                if (m_curBraces > 1)
                {
                    m_sb.Indent();
                    m_sb.AppendLine("{");
                }
                
                m_curBraces++;
            }

            base.VisitToken(token);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            m_currentNamespace = node;
            m_sb.AppendLine("/* Namespace <" + node.Name + "> */");
            base.VisitNamespaceDeclaration(node);
        }

        public override void Visit(SyntaxNode node)
        {
            /*Console.WriteLine(node.GetText());
            Console.WriteLine("Visited " + node.GetType());*/
            base.Visit(node);
        }
    }
}
