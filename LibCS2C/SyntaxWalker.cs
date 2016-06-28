using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using LibCS2C.Generators;

namespace LibCS2C
{
    public class SyntaxWalker : CSharpSyntaxWalker
    {
        private FormattedStringBuilder m_sb;
        private WalkerContext m_context;

        private MethodGenerator m_methodGen;
        private MethodGenerator m_constructorGen;
        private ClassCodeGenerator m_classFieldGen;
        private PropertyGenerator m_propertyGen;

        /// <summary>
        /// Walks through the syntax and outputs C code to a <see cref="FormattedStringBuilder">FormattedStringBuilder</see>
        /// </summary>
        /// <param name="sb">The formatted string builder</param>
        public SyntaxWalker(FormattedStringBuilder sb, SemanticModel model) : base(SyntaxWalkerDepth.Node)
        {
            m_sb = sb;
            m_context = new WalkerContext(sb, model);

            // Generators
            m_methodGen = new MethodGenerator(m_context, MethodGeneratorType.Method);
            m_constructorGen = new MethodGenerator(m_context, MethodGeneratorType.Constructor);
            m_classFieldGen = new ClassCodeGenerator(m_context);
            m_propertyGen = new PropertyGenerator(m_context);
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
        /// Visit a constructor declaration
        /// </summary>
        /// <param name="node">The constructor declaration node</param>
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            m_constructorGen.Generate(node);
            base.VisitConstructorDeclaration(node);
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
        /// Visits a property declaration
        /// </summary>
        /// <param name="node">The property node</param>
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            m_propertyGen.Generate(node);
            base.VisitPropertyDeclaration(node);
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
    }
}
