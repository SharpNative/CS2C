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
        private VariableGenerator m_variableGen;
        private SimpleAssignmentGenerator m_simpleAssignmentGen;
        private InvocationGenerator m_invocationGen;
        private ClassFieldGenerator m_classFieldGen;
        private ReturnStatementGenerator m_returnStatementGen;

        /// <summary>
        /// Walks through the syntax and outputs C code to a <see cref="FormattedStringBuilder">FormattedStringBuilder</see>
        /// </summary>
        /// <param name="sb">The formatted string builder</param>
        public SyntaxWalker(FormattedStringBuilder sb, SemanticModel model) : base(SyntaxWalkerDepth.Token)
        {
            m_sb = sb;
            m_curBraces = 0;
            m_context = new WalkerContext(sb, model);

            // Generators
            m_methodGen = new MethodGenerator(m_context, MethodGeneratorType.Method);
            m_constructorGen = new MethodGenerator(m_context, MethodGeneratorType.Constructor);
            m_variableGen = new VariableGenerator(m_context);
            m_simpleAssignmentGen = new SimpleAssignmentGenerator(m_context);
            m_invocationGen = new InvocationGenerator(m_context);
            m_classFieldGen = new ClassFieldGenerator(m_context);
            m_returnStatementGen = new ReturnStatementGenerator(m_context);
        }

        /// <summary>
        /// Visits a class declaration
        /// </summary>
        /// <param name="node">The class declaration node</param>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            m_sb.AppendLine("/* Class <" + node.Identifier + "> */");
            m_context.CurrentClass = node;
            m_classFieldGen.Generate(node);
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

            m_variableGen.Generate(node);
            base.VisitVariableDeclaration(node);
        }

        /// <summary>
        /// Visit a constructor declaration
        /// </summary>
        /// <param name="node">The constructor declaration node</param>
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            m_constructorGen.Generate(node);
            base.VisitConstructorDeclaration(node);
        }

        /// <summary>
        /// Visits an expression
        /// </summary>
        /// <param name="statementNode">The expression node</param>
        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            SyntaxKind kind = node.Expression.Kind();
            if(kind == SyntaxKind.SimpleAssignmentExpression)
            {
                m_simpleAssignmentGen.Generate(node);
            }
            else if(kind == SyntaxKind.InvocationExpression)
            {
                m_invocationGen.Generate(node);
            }
            else
            {
                throw new NotImplementedException();
            }

            base.VisitExpressionStatement(node);
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
        /// Visits a return statement node
        /// </summary>
        /// <param name="node"></param>
        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            m_returnStatementGen.Generate(node);
            base.VisitReturnStatement(node);
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
