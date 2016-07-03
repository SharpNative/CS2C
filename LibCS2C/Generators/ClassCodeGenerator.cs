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
        /// Calls the class fields and .cctor generators
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            // Temporarily hold all the fields/properties so we can put them in the initialization method
            Dictionary<string, EqualsValueClauseSyntax> staticFields = new Dictionary<string, EqualsValueClauseSyntax>();
            Dictionary<string, EqualsValueClauseSyntax> nonStaticFields = new Dictionary<string, EqualsValueClauseSyntax>();
            Dictionary<string, TypeSyntax> staticFieldTypes = new Dictionary<string, TypeSyntax>();
            Dictionary<string, TypeSyntax> nonStaticFieldTypes = new Dictionary<string, TypeSyntax>();
            Dictionary<string, TypeSyntax> propertyTypesNonStatic = new Dictionary<string, TypeSyntax>();
            Dictionary<string, EqualsValueClauseSyntax> propertyInitialValuesNonStatic = new Dictionary<string, EqualsValueClauseSyntax>();
            Dictionary<string, TypeSyntax> propertyTypesStatic = new Dictionary<string, TypeSyntax>();
            Dictionary<string, EqualsValueClauseSyntax> propertyInitialValuesStatic = new Dictionary<string, EqualsValueClauseSyntax>();

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
                                    staticFields.Add(identifier, variable.Initializer);

                                staticFieldTypes.Add(identifier, fieldNodeChild.Type);
                            }
                            else
                            {
                                if (variable.Initializer != null)
                                    nonStaticFields.Add(identifier, variable.Initializer);

                                nonStaticFieldTypes.Add(identifier, fieldNodeChild.Type);
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
                        propertyTypesNonStatic.Add(identifier, propertyDeclaration.Type);
                        propertyInitialValuesNonStatic.Add(identifier, propertyDeclaration.Initializer);
                    }
                    else
                    {
                        propertyTypesStatic.Add(identifier, propertyDeclaration.Type);
                        propertyInitialValuesStatic.Add(identifier, propertyDeclaration.Initializer);
                    }
                }
            }

            // Other generators
            ClassStructGenerator structGen = new ClassStructGenerator(m_context, nonStaticFieldTypes, propertyTypesNonStatic);
            ClassStaticStructGenerator staticStructGen = new ClassStaticStructGenerator(m_context, staticFieldTypes, propertyTypesStatic);
            ClassCctorGenerator cctorGen = new ClassCctorGenerator(m_context, staticFields, propertyInitialValuesStatic);
            ClassInitGenerator classInitGen = new ClassInitGenerator(m_context, nonStaticFields, propertyInitialValuesNonStatic);

            structGen.Generate(node);
            staticStructGen.Generate(node);
            cctorGen.Generate(node);
            classInitGen.Generate(node);
        }
    }
}
