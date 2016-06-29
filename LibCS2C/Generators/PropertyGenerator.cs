using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    class PropertyGenerator : GeneratorBase<PropertyDeclarationSyntax>
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

            bool isStatic = false;
            foreach (SyntaxNodeOrToken child in children)
            {
                SyntaxKind childKind = child.Kind();

                if(childKind == SyntaxKind.StaticKeyword)
                {
                    isStatic = true;
                }
                else if (childKind == SyntaxKind.AccessorList)
                {
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
                        m_context.Writer.AppendLine(string.Format("{0} {1}_{2}_getter({3}* obj)", m_context.ConvertTypeName(node.Type), m_context.CurrentClassNameFormatted, node.Identifier, m_context.CurrentClassStructName));
                        m_context.Writer.AppendLine("{");

                        if(getAccessor.Body == null)
                        {
                            if(isStatic)
                                m_context.Writer.AppendLine(string.Format("\treturn classStatics_{0}.prop_{1};", m_context.CurrentClassNameFormatted, node.Identifier));
                            else
                                m_context.Writer.AppendLine(string.Format("\treturn obj->prop_{0};", node.Identifier));
                        }
                        else
                        {
                            BlockGenerator block = new BlockGenerator(m_context);
                            block.Generate(getAccessor.Body);
                        }
                        
                        m_context.Writer.AppendLine("}");
                    }

                    if (setAccessor != default(AccessorDeclarationSyntax))
                    {
                        m_context.Writer.AppendLine(string.Format("{0} {1}_{2}_setter({3}* obj, {0} value)", m_context.ConvertTypeName(node.Type), m_context.CurrentClassNameFormatted, node.Identifier, m_context.CurrentClassStructName));
                        m_context.Writer.AppendLine("{");

                        if(setAccessor.Body == null)
                        {
                            if (isStatic)
                                m_context.Writer.AppendLine(string.Format("\tclassStatics_{0}.prop_{1} = value;", m_context.CurrentClassNameFormatted, node.Identifier));
                            else
                                m_context.Writer.AppendLine(string.Format("\tobj->prop_{0} = value;", node.Identifier));
                        }
                        else
                        {
                            BlockGenerator block = new BlockGenerator(m_context);
                            block.Generate(setAccessor.Body);
                        }

                        m_context.Writer.AppendLine("\treturn value;");
                        m_context.Writer.AppendLine("}");
                    }
                }
            }
        }
    }
}
