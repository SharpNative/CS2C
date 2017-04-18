using LibCS2C.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        /// Gets a list of fields (as variable declarations) from a class definition
        /// </summary>
        /// <param name="node">The class definition</param>
        /// <returns>The list of fields</returns>
        public List<VariableDeclarationSyntax> GetFields(ClassDeclarationSyntax node)
        {
            List<VariableDeclarationSyntax> fields = new List<VariableDeclarationSyntax>();

            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (SyntaxNode childNode in nodes)
            {
                SyntaxKind kind = childNode.Kind();

                // Found a field
                if (kind == SyntaxKind.FieldDeclaration)
                {
                    FieldDeclarationSyntax fieldNode = childNode as FieldDeclarationSyntax;
                    IEnumerable<SyntaxNode> fieldNodeChildren = fieldNode.ChildNodes();

                    foreach (VariableDeclarationSyntax variable in fieldNodeChildren)
                    {
                        fields.Add(variable);
                    }
                }
            }

            return fields;
        }

        /// <summary>
        /// Gets a list of properties from a class definition
        /// </summary>
        /// <param name="node">The class definition</param>
        /// <returns>A list of properties</returns>
        public List<PropertyDeclarationSyntax> GetProperties(ClassDeclarationSyntax node)
        {
            List<PropertyDeclarationSyntax> properties = new List<PropertyDeclarationSyntax>();

            IEnumerable<SyntaxNode> nodes = node.ChildNodes();
            foreach (SyntaxNode childNode in nodes)
            {
                SyntaxKind kind = childNode.Kind();

                // Found a property
                if (kind == SyntaxKind.PropertyDeclaration)
                {
                    properties.Add(childNode as PropertyDeclarationSyntax);
                }
            }

            return properties;
        }

        /// <summary>
        /// Gets the fields and properties of a class definition
        /// </summary>
        /// <param name="node">The class definition</param>
        /// <param name="fields">(out) The fields</param>
        /// <param name="properties">(out) The properties</param>
        public void GetFieldsAndProperties(ClassDeclarationSyntax node, out List<VariableDeclarationSyntax> fields, out List<PropertyDeclarationSyntax> properties)
        {
            fields = new List<VariableDeclarationSyntax>();
            properties = new List<PropertyDeclarationSyntax>();

            if (node.BaseList != null)
            {
                IEnumerable<SyntaxNode> children = node.BaseList.ChildNodes();
                foreach (SimpleBaseTypeSyntax child in children)
                {
                    // Get the class definition so we can get the fields and properties of the base
                    ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(child.ChildNodes().First()).Type;
                    ImmutableArray<SyntaxReference> definitions = typeSymbol.DeclaringSyntaxReferences;

                    // We can only do this if a definition has been found and if we're not working with an interface
                    // because interfaces cannot have fields
                    if (definitions.Length > 0 && typeSymbol.TypeKind != TypeKind.Interface)
                    {
                        // Get fields and properties of the base
                        ClassDeclarationSyntax definition = definitions[0].GetSyntax() as ClassDeclarationSyntax;
                        List<VariableDeclarationSyntax> childFields = new List<VariableDeclarationSyntax>();
                        List<PropertyDeclarationSyntax> childProperties = new List<PropertyDeclarationSyntax>();
                        GetFieldsAndProperties(definition, out fields, out properties);

                        // Only non-static fields and properties are inherited
                        foreach (VariableDeclarationSyntax childField in childFields)
                        {
                            if (!m_context.Model.GetTypeInfo(childField).Type.IsStatic)
                                fields.Add(childField);
                        }

                        foreach (PropertyDeclarationSyntax childProperty in childProperties)
                        {
                            if (!m_context.Model.GetTypeInfo(childProperty).Type.IsStatic)
                                properties.Add(childProperty);
                        }
                    }
                }
            }

            fields.AddRange(GetFields(node));
            properties.AddRange(GetProperties(node));
        }

        /// <summary>
        /// Creates class initializers, class structs and .cctors
        /// </summary>
        /// <param name="node">The class declaration</param>
        public override void Generate(ClassDeclarationSyntax node)
        {
            // Temporarily hold all the fields/properties so we can put them in the initialization method
            ClassCodeData classCode = new ClassCodeData();

            // Get fields and properties of the current class
            List<VariableDeclarationSyntax> fields = null;
            List<PropertyDeclarationSyntax> properties = null;
            GetFieldsAndProperties(node, out fields, out properties);

            // Mark the base class as an extending class
            if (node.BaseList != null)
            {
                IEnumerable<SyntaxNode> children = node.BaseList.ChildNodes();
                foreach (SimpleBaseTypeSyntax child in children)
                {
                    ITypeSymbol typeSymbol = m_context.Model.GetTypeInfo(child.ChildNodes().First()).Type;
                    string str = string.Format("{0}_{1}", m_context.ConvertNameSpace(typeSymbol.ContainingNamespace), typeSymbol.Name);
                    m_context.TypeIsExtending[str] = true;
                }
            }

            // Fields
            foreach (VariableDeclarationSyntax fieldNodeChild in fields)
            {
                bool isStatic = false;
                bool isConst = false;

                // Check if this field is a static or a const field by looping through its tokens
                IEnumerable<SyntaxToken> fieldNodeTokens = fieldNodeChild.Parent.ChildTokens();
                foreach (SyntaxToken token in fieldNodeTokens)
                {
                    SyntaxKind tokenKind = token.Kind();
                    if (tokenKind == SyntaxKind.StaticKeyword)
                        isStatic = true;
                    else if (tokenKind == SyntaxKind.ConstKeyword)
                        isConst = true;
                }

                // Loop through the variable declarators in the field
                foreach (VariableDeclaratorSyntax variable in fieldNodeChild.Variables)
                {
                    string identifier = variable.Identifier.ToString();

                    // Constant value definition
                    if (isConst)
                    {
                        m_context.Writer.CurrentDestination = WriterDestination.Defines;
                        m_context.Writer.Append(string.Format("#define const_{0}_{1}", m_context.TypeConvert.CurrentClassNameFormatted, identifier));

                        m_context.Writer.Append(" (");
                        m_context.Writer.CurrentDestination = WriterDestination.TempBuffer;
                        m_context.Generators.Expression.Generate(variable.Initializer.Value);
                        m_context.Writer.CurrentDestination = WriterDestination.Defines;
                        m_context.Writer.Append(m_context.Writer.FlushTempBuffer());
                        m_context.Writer.AppendLine(")");
                    }
                    // Regular field
                    else
                    {
                        Dictionary<string, EqualsValueClauseSyntax> dictValues = isStatic ? classCode.staticFields : classCode.nonStaticFields;
                        Dictionary<string, TypeSyntax> dictTypes = isStatic ? classCode.staticFieldTypes : classCode.nonStaticFieldTypes;

                        dictTypes.Add(identifier, fieldNodeChild.Type);
                        if (variable.Initializer != null)
                            dictValues.Add(identifier, variable.Initializer);
                    }
                }
            }

            // Properties
            foreach (PropertyDeclarationSyntax property in properties)
            {
                string identifier = property.Identifier.ToString();

                // Check if the property is static by looping through its tokens
                bool isStatic = false;
                IEnumerable<SyntaxToken> tokens = property.ChildTokens();
                foreach (SyntaxToken token in tokens)
                {
                    if (token.Kind() == SyntaxKind.StaticKeyword)
                    {
                        isStatic = true;
                        break;
                    }
                }

                Dictionary<string, EqualsValueClauseSyntax> dictValues = isStatic ? classCode.propertyInitialValuesStatic : classCode.propertyInitialValuesNonStatic;
                Dictionary<string, TypeSyntax> dictTypes = isStatic ? classCode.propertyTypesStatic : classCode.propertyTypesNonStatic;

                dictTypes.Add(identifier, property.Type);
                if (property.Initializer != null)
                    dictValues.Add(identifier, property.Initializer);
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
