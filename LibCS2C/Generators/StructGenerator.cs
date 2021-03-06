﻿using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibCS2C.Generators
{
    public class StructGenerator : GeneratorBase<StructDeclarationSyntax>
    {
        /// <summary>
        /// struct generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public StructGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generates a struct declaration
        /// </summary>
        /// <param name="node">The struct declaration</param>
        public override void Generate(StructDeclarationSyntax node)
        {
            WriterDestination destination = m_context.Writer.CurrentDestination;

            // Check for attributes
            bool packed = false;

            SyntaxList<AttributeListSyntax> attribLists = node.AttributeLists;
            foreach (AttributeListSyntax attribList in attribLists)
            {
                SeparatedSyntaxList<AttributeSyntax> attribs = attribList.Attributes;
                foreach (AttributeSyntax attrib in attribs)
                {
                    IdentifierNameSyntax name = attrib.ChildNodes().First() as IdentifierNameSyntax;
                    string identifier = name.Identifier.ToString();

                    // Defines layout of the struct
                    if (identifier.Equals("StructLayoutAttribute") || identifier.Equals("StructLayout"))
                    {
                        SeparatedSyntaxList<AttributeArgumentSyntax> argsList = attrib.ArgumentList.Arguments;
                        foreach (AttributeArgumentSyntax arg in argsList)
                        {
                            SyntaxNode first = arg.ChildNodes().First();
                            SyntaxKind kind = first.Kind();

                            if (kind == SyntaxKind.NameEquals)
                            {
                                NameEqualsSyntax nameEquals = first as NameEqualsSyntax;
                                string nameIdentifier = nameEquals.Name.Identifier.ToString();

                                if (nameIdentifier.Equals("Pack"))
                                {
                                    packed = true;
                                }
                            }
                        }
                    }
                    // Unknown attribute
                    else
                    {
                        throw new NotImplementedException("Unknown attribute on struct: " + identifier);
                    }
                }
            }

            // Create struct name
            string structName;
            if (node.Parent is ClassDeclarationSyntax)
            {
                structName = string.Format("{0}_{1}", m_context.TypeConvert.CurrentClassNameFormatted, node.Identifier.ToString());
            }
            else
            {
                structName = string.Format("{0}_{1}", m_context.TypeConvert.CurrentNamespaceFormatted, node.Identifier.ToString());
            }

            // Temporarily hold all the data
            Dictionary<string, TypeSyntax> dataTypes = new Dictionary<string, TypeSyntax>();
            Dictionary<string, string> dataSuffixes = new Dictionary<string, string>();

            // Collect the data and put it in the dictionaries
            IEnumerable<SyntaxNode> children = node.ChildNodes();
            foreach (SyntaxNode child in children)
            {
                SyntaxKind kind = child.Kind();

                if (kind == SyntaxKind.FieldDeclaration)
                {
                    FieldDeclarationSyntax field = child as FieldDeclarationSyntax;
                    IEnumerable<SyntaxNode> fieldChildren = field.ChildNodes();

                    foreach (VariableDeclarationSyntax fieldChild in fieldChildren)
                    {
                        foreach (VariableDeclaratorSyntax variable in fieldChild.Variables)
                        {
                            string identifier = "field_" + variable.Identifier.ToString();
                            dataTypes.Add(identifier, fieldChild.Type);

                            BracketedArgumentListSyntax argumentList = (from a in variable.ChildNodes()
                                                                        where a.Kind() == SyntaxKind.BracketedArgumentList
                                                                        select a).FirstOrDefault() as BracketedArgumentListSyntax;

                            if (argumentList != default(BracketedArgumentListSyntax))
                            {
                                ArgumentSyntax argument = argumentList.Arguments[0];
                                WriterDestination dest = m_context.Writer.CurrentDestination;
                                m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                                m_context.Generators.ArgumentList.GenerateArgument(argument);
                                string arg = m_context.Writer.FlushTempBuffer();
                                dataSuffixes.Add(identifier, "[" + arg + "]");
                                m_context.Writer.CurrentDestination = dest;
                            }
                        }
                    }
                }
                else if (kind == SyntaxKind.PropertyDeclaration)
                {
                    PropertyDeclarationSyntax property = child as PropertyDeclarationSyntax;
                    string identifier = "prop_" + property.Identifier.ToString();
                    dataTypes.Add(identifier, property.Type);
                }
            }

            // Struct prototype
            string structPrototype = string.Format("struct struct_{0}", structName);
            m_context.Writer.CurrentDestination = WriterDestination.StructPrototypes;
            m_context.Writer.AppendLine(string.Format("{0};", structPrototype));

            // Struct definition
            m_context.Writer.CurrentDestination = WriterDestination.Structs;
            m_context.Writer.AppendLine(structPrototype);
            m_context.Writer.AppendLine("{");

            foreach (KeyValuePair<string, TypeSyntax> pair in dataTypes)
            {
                m_context.Writer.Append(string.Format("\t{0} {1}", m_context.ConvertTypeName(pair.Value), pair.Key));
                if (dataSuffixes.ContainsKey(pair.Key))
                    m_context.Writer.Append(dataSuffixes[pair.Key]);
                m_context.Writer.AppendLine(";");
            }

            // Attributes
            if (packed)
                m_context.Writer.AppendLine("} __attribute__((packed));");
            else
                m_context.Writer.AppendLine("};");


            // Method prototype of init code
            string methodName = string.Format("struct struct_{0} structInit_{0}(void)", structName);
            m_context.Writer.CurrentDestination = WriterDestination.MethodPrototypes;
            m_context.Writer.Append("extern " + methodName);
            m_context.Writer.AppendLine(";");

            // Init method declaration
            m_context.Writer.CurrentDestination = WriterDestination.MethodDeclarations;
            m_context.Writer.AppendLine("inline " + methodName);
            m_context.Writer.AppendLine("{");
            string structType = "struct struct_" + structName;
            m_context.Writer.AppendLine(string.Format("\t{0} object;", structType));
            m_context.Writer.AppendLine(string.Format("\tmemset(&object, 0, sizeof({0}));", structType));
            m_context.Writer.AppendLine("\treturn object;");
            m_context.Writer.AppendLine("}");

            m_context.Writer.CurrentDestination = destination;
        }
    }
}
