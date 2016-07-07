using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace LibCS2C
{
    public class SyntaxWalker : CSharpSyntaxWalker
    {
        private WalkerContext m_context;

        /// <summary>
        /// Walks through the syntax and outputs C code to a <see cref="FormattedStringBuilder">FormattedStringBuilder</see>
        /// </summary>
        public SyntaxWalker() : base(SyntaxWalkerDepth.Node)
        {
            m_context = new WalkerContext();
        }

        /// <summary>
        /// Sets the current document
        /// </summary>
        /// <param name="doc">The document</param>
        public void SetDocument(Document doc)
        {
            m_context.Model = doc.GetSemanticModelAsync().Result;
        }

        /// <summary>
        /// Visits a struct declaration
        /// </summary>
        /// <param name="node">The struct declaration node</param>
        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            m_context.CurrentDestination = WriterDestination.Structs;
            m_context.Writer.AppendLine("/* Struct <" + node.Identifier + "> */");
            m_context.Generators.Struct.Generate(node);
            base.VisitStructDeclaration(node);
        }

        /// <summary>
        /// Visits a class declaration
        /// </summary>
        /// <param name="node">The class declaration node</param>
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            m_context.CurrentDestination = WriterDestination.ClassStructs;
            m_context.CurrentClass = node;
            m_context.Generators.ClassCode.Generate(node);
            base.VisitClassDeclaration(node);
        }

        /// <summary>
        /// Visit a constructor declaration
        /// </summary>
        /// <param name="node">The constructor declaration node</param>
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            m_context.Generators.ConstructorDeclaration.Generate(node);
            base.VisitConstructorDeclaration(node);
        }

        /// <summary>
        /// Visits a method declaration
        /// </summary>
        /// <param name="node">The method declaration node</param>
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            m_context.Generators.MethodDeclaration.Generate(node);
            base.VisitMethodDeclaration(node);
        }

        /// <summary>
        /// Visits a property declaration
        /// </summary>
        /// <param name="node">The property node</param>
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            m_context.Generators.Property.Generate(node);
            base.VisitPropertyDeclaration(node);
        }

        /// <summary>
        /// Visits an enum declaration
        /// </summary>
        /// <param name="node">The enum node</param>
        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            m_context.Generators.Enum.Generate(node);
            base.VisitEnumDeclaration(node);
        }

        /// <summary>
        /// Visits a namespace declaration
        /// </summary>
        /// <param name="node">The namespace declaration node</param>
        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            m_context.CurrentNamespace = node;
            base.VisitNamespaceDeclaration(node);
        }

        /// <summary>
        /// Outputs the code in a string
        /// </summary>
        /// <returns>The code</returns>
        public override string ToString()
        {
            // Append all the code
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(m_context.SbEnums.ToString());
            sb.AppendLine(m_context.SbStructs.ToString());
            sb.AppendLine(m_context.SbClassStructs.ToString());
            sb.AppendLine(m_context.SbMethodPrototypes.ToString());
            sb.AppendLine(m_context.SbMethodDeclarations.ToString());

            // Add .cctor calls in init method
            sb.AppendLine("void init(void)");
            sb.AppendLine("{");
            foreach (string cctor in m_context.CctorList)
            {
                sb.AppendLine("\t" + cctor + "();");
            }
            sb.AppendLine("}");

            // Output string
            return sb.ToString();
        }
    }
}
