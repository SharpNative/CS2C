using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace LibCS2C.Generators
{
    public class PropertyGenerator : GeneratorBase<PropertyDeclarationSyntax>
    {
        /// <summary>
        /// Property generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public PropertyGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Generate code for property declaration
        /// </summary>
        /// <param name="node"></param>
        public override void Generate(PropertyDeclarationSyntax node)
        {
            ChildSyntaxList children = node.ChildNodesAndTokens();
            SyntaxNode parentNode = node.Parent;
            SyntaxKind parentKind = parentNode.Kind();

            bool isStatic = false;
            foreach (SyntaxNodeOrToken child in children)
            {
                SyntaxKind childKind = child.Kind();

                if (childKind == SyntaxKind.StaticKeyword)
                {
                    isStatic = true;
                }
                else if (childKind == SyntaxKind.AccessorList)
                {
                    // Determine where the property is located and use that to determine the names
                    string objTypeName, typeName;
                    if (parentKind == SyntaxKind.StructDeclaration)
                    {
                        StructDeclarationSyntax structDeclaration = parentNode as StructDeclarationSyntax;
                        typeName = m_context.TypeConvert.ConvertClassName(structDeclaration.Identifier.ToString());
                        objTypeName = "struct struct_" + m_context.TypeConvert.ConvertClassName(structDeclaration.Identifier.ToString());
                    }
                    else if (parentKind == SyntaxKind.ClassDeclaration)
                    {
                        typeName = m_context.TypeConvert.CurrentClassNameFormatted;
                        objTypeName = m_context.TypeConvert.CurrentClassStructName;
                    }
                    else if (parentKind == SyntaxKind.InterfaceDeclaration)
                    {
                        InterfaceDeclarationSyntax interfaceDeclaration = parentNode as InterfaceDeclarationSyntax;
                        typeName = m_context.TypeConvert.ConvertClassName(interfaceDeclaration.Identifier.ToString());
                        objTypeName = "struct " + m_context.TypeConvert.ConvertClassName(interfaceDeclaration.Identifier.ToString());
                    }
                    else
                    {
                        throw new NotImplementedException("Unknown parent for getter: " + parentKind);
                    }

                    AccessorListSyntax accessors = child.AsNode() as AccessorListSyntax;
                    SyntaxList<AccessorDeclarationSyntax> accessorDeclarations = accessors.Accessors;

                    AccessorDeclarationSyntax getAccessor = (from a in accessorDeclarations
                                                             where a.Kind() == SyntaxKind.GetAccessorDeclaration
                                                             select a).FirstOrDefault();

                    AccessorDeclarationSyntax setAccessor = (from a in accessorDeclarations
                                                             where a.Kind() == SyntaxKind.SetAccessorDeclaration
                                                             select a).FirstOrDefault();


                    if (getAccessor != default(AccessorDeclarationSyntax))
                    {

                        string methodPrototype;
                        if (isStatic)
                            methodPrototype = string.Format("{0} {1}_{2}_getter(void)", m_context.ConvertTypeName(node.Type), typeName, node.Identifier);
                        else
                            methodPrototype = string.Format("{0} {1}_{2}_getter({3}* obj)", m_context.ConvertTypeName(node.Type), typeName, node.Identifier, objTypeName);

                        // Method prototype
                        m_context.Writer.CurrentDestination = WriterDestination.MethodPrototypes;
                        m_context.Writer.Append(methodPrototype);
                        m_context.Writer.AppendLine(";");

                        // Method declaration
                        m_context.Writer.CurrentDestination = WriterDestination.MethodDeclarations;
                        m_context.Writer.AppendLine(methodPrototype);
                        m_context.Writer.AppendLine("{");

                        if (getAccessor.Body == null)
                        {
                            if (isStatic)
                                m_context.Writer.AppendLine(string.Format("\treturn classStatics_{0}.prop_{1};", typeName, node.Identifier));
                            else
                                m_context.Writer.AppendLine(string.Format("\treturn obj->prop_{0};", node.Identifier));
                        }
                        else
                        {
                            m_context.Generators.Block.Generate(getAccessor.Body);
                        }

                        m_context.Writer.AppendLine("}");
                    }

                    if (setAccessor != default(AccessorDeclarationSyntax))
                    {
                        string methodPrototype;
                        if (isStatic)
                            methodPrototype = string.Format("{0} {1}_{2}_setter({0} value)", m_context.ConvertTypeName(node.Type), typeName, node.Identifier);
                        else
                            methodPrototype = string.Format("{0} {1}_{2}_setter({3}* obj, {0} value)", m_context.ConvertTypeName(node.Type), typeName, node.Identifier, objTypeName);

                        // Method prototype
                        m_context.Writer.CurrentDestination = WriterDestination.MethodPrototypes;
                        m_context.Writer.Append(methodPrototype);
                        m_context.Writer.AppendLine(";");

                        // Method declaration
                        m_context.Writer.CurrentDestination = WriterDestination.MethodDeclarations;
                        m_context.Writer.AppendLine(methodPrototype);
                        m_context.Writer.AppendLine("{");

                        if (setAccessor.Body == null)
                        {
                            if (isStatic)
                                m_context.Writer.AppendLine(string.Format("\tclassStatics_{0}.prop_{1} = value;", typeName, node.Identifier));
                            else
                                m_context.Writer.AppendLine(string.Format("\tobj->prop_{0} = value;", node.Identifier));
                        }
                        else
                        {
                            m_context.Generators.Block.Generate(setAccessor.Body);
                        }

                        m_context.Writer.AppendLine("\treturn value;");
                        m_context.Writer.AppendLine("}");
                    }
                }
            }
        }
    }
}
