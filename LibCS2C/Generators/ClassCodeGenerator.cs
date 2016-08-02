using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibCS2C.Generators
{
    public class ClassCodeGenerator : GeneratorBase<ClassDeclarationSyntax>
    {
        /// <summary>
        /// Class field generator
        /// </summary>
        /// <param name="context">The walker context</param>
        public ClassCodeGenerator(WalkerContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// Creates the base class struct code
        /// </summary>
        /// <returns>The code for the base class struct</returns>
        public string CreateBaseClassStruct()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("struct base_class");
            sb.AppendLine("{");
            sb.AppendLine("\tint32_t usage_count;");
            sb.AppendLine("\tvoid** lookup_table;");
            sb.AppendLine("};");
            return sb.ToString();
        }

        /// <summary>
        /// Calls the class fields and .cctor generators
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            // Temporarily hold all the fields/properties so we can put them in the initialization method
            ClassCodeData classCode = new ClassCodeData();

            // Mark the base class as an extending class
            if(node.BaseList != null)
            {
                IEnumerable<SyntaxNode> children = node.BaseList.ChildNodes();
                foreach(SimpleBaseTypeSyntax child in children)
                {
                    ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(child.ChildNodes().First()).Type;
                    string str = string.Format("{0}_{1}", typeSymbol.ContainingNamespace.ToString().Replace('.', '_'), typeSymbol.Name);
                    m_context.TypeIsExtended[str] = true;
                }
            }

            // Loop through the children to find the fields
            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (SyntaxNode childNode in nodes)
            {
                SyntaxKind kind = childNode.Kind();

                // Found a field
                if (kind == SyntaxKind.FieldDeclaration)
                {
                    FieldDeclarationSyntax fieldNode = childNode as FieldDeclarationSyntax;
                    IEnumerable<SyntaxNode> fieldNodeChildren = fieldNode.ChildNodes();

                    bool isStatic = false;

                    IEnumerable<SyntaxToken> fieldNodeTokens = fieldNode.ChildTokens();
                    foreach (SyntaxToken token in fieldNodeTokens)
                    {
                        if (token.Kind() == SyntaxKind.StaticKeyword)
                            isStatic = true;
                    }

                    foreach (VariableDeclarationSyntax fieldNodeChild in fieldNodeChildren)
                    {
                        foreach (VariableDeclaratorSyntax variable in fieldNodeChild.Variables)
                        {
                            string identifier = variable.Identifier.ToString();
                            if (isStatic)
                            {
                                if (variable.Initializer != null)
                                    classCode.staticFields.Add(identifier, variable.Initializer);

                                classCode.staticFieldTypes.Add(identifier, fieldNodeChild.Type);
                            }
                            else
                            {
                                if (variable.Initializer != null)
                                    classCode.nonStaticFields.Add(identifier, variable.Initializer);

                                classCode.nonStaticFieldTypes.Add(identifier, fieldNodeChild.Type);
                            }
                        }
                    }
                }
                // Found a property
                else if(kind == SyntaxKind.PropertyDeclaration)
                {
                    PropertyDeclarationSyntax propertyDeclaration = childNode as PropertyDeclarationSyntax;
                    string identifier = propertyDeclaration.Identifier.ToString();

                    bool isStatic = false;
                    IEnumerable<SyntaxToken> tokens = propertyDeclaration.ChildTokens();
                    foreach(SyntaxToken token in tokens)
                    {
                        if(token.Kind() == SyntaxKind.StaticKeyword)
                        {
                            isStatic = true;
                            break;
                        }
                    }

                    if(!isStatic)
                    {
                        classCode.propertyTypesNonStatic.Add(identifier, propertyDeclaration.Type);

                        if (propertyDeclaration.Initializer != null)
                            classCode.propertyInitialValuesNonStatic.Add(identifier, propertyDeclaration.Initializer);
                    }
                    else
                    {
                        classCode.propertyTypesStatic.Add(identifier, propertyDeclaration.Type);

                        if (propertyDeclaration.Initializer != null)
                            classCode.propertyInitialValuesStatic.Add(identifier, propertyDeclaration.Initializer);
                    }
                }
            }

            // Other generators
            ClassStructGenerator structGen = new ClassStructGenerator(m_context, classCode);
            ClassStaticStructGenerator staticStructGen = new ClassStaticStructGenerator(m_context, classCode);
            ClassInitGenerator classInitGen = new ClassInitGenerator(m_context, classCode);
            ClassCctorGenerator classCctorGen = new ClassCctorGenerator(m_context, classCode);

            m_context.Writer.CurrentDestination = WriterDestination.ClassStructs;
            structGen.Generate(node);
            m_context.Writer.CurrentDestination = WriterDestination.ClassStructs;
            staticStructGen.Generate(node);
            classInitGen.Generate(node);
            classCctorGen.Generate(node);
        }
    }
}
