using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using LibCS2C.Generators;
using System.Collections.Generic;

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
        private StructGenerator m_structGen;
        private EnumGenerator m_enumGen;

        /// <summary>
        /// Walks through the syntax and outputs C code to a <see cref="FormattedStringBuilder">FormattedStringBuilder</see>
        /// </summary>
        /// <param name="sb">The formatted string builder</param>
        public SyntaxWalker(FormattedStringBuilder sb) : base(SyntaxWalkerDepth.Node)
        {
            m_sb = sb;
            m_context = new WalkerContext(sb);

            // Generators
            m_methodGen = new MethodGenerator(m_context, MethodGeneratorType.Method);
            m_constructorGen = new MethodGenerator(m_context, MethodGeneratorType.Constructor);
            m_classFieldGen = new ClassCodeGenerator(m_context);
            m_propertyGen = new PropertyGenerator(m_context);
            m_structGen = new StructGenerator(m_context);
            m_enumGen = new EnumGenerator(m_context);
        }

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
            m_sb.AppendLine("/* Struct <" + node.Identifier + "> */");
            m_structGen.Generate(node);
            base.VisitStructDeclaration(node);
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

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            m_enumGen.Generate(node);
            base.VisitEnumDeclaration(node);
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

        public void Finish()
        {
            m_context.Writer.AppendLine("void init(void)");
            m_context.Writer.AppendLine("{");

            foreach(string cctor in m_context.CctorList)
            {
                m_context.Writer.AppendLine(string.Format("\t{0}();", cctor));
            }

            m_context.Writer.AppendLine("}");
            
            string begin = "";
            begin += "/* Method prototypes */\r\n";
            foreach (string prototype in m_context.MethodPrototypes)
            {
                begin += prototype + ";\r\n";
            }

            begin += "/* Enums */\r\n";
            foreach (KeyValuePair<string, string> pair in m_context.Enums)
            {
                begin += string.Format("#define enum_{0} ({1})\r\n", pair.Key, pair.Value);
            }

            begin += "\r\n";
            m_context.Writer.Prepend(begin);
        }
    }
}
