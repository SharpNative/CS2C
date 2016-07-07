using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace LibCS2C.Generators
{
    public class ClassCodeData
    {
        public Dictionary<string, EqualsValueClauseSyntax> staticFields { get; private set; } = new Dictionary<string, EqualsValueClauseSyntax>();
        public Dictionary<string, EqualsValueClauseSyntax> nonStaticFields { get; private set; } = new Dictionary<string, EqualsValueClauseSyntax>();
        public Dictionary<string, TypeSyntax> staticFieldTypes { get; private set; } = new Dictionary<string, TypeSyntax>();
        public Dictionary<string, TypeSyntax> nonStaticFieldTypes { get; private set; } = new Dictionary<string, TypeSyntax>();
        public Dictionary<string, TypeSyntax> propertyTypesNonStatic { get; private set; } = new Dictionary<string, TypeSyntax>();
        public Dictionary<string, EqualsValueClauseSyntax> propertyInitialValuesNonStatic { get; private set; } = new Dictionary<string, EqualsValueClauseSyntax>();
        public Dictionary<string, TypeSyntax> propertyTypesStatic { get; private set; } = new Dictionary<string, TypeSyntax>();
        public Dictionary<string, EqualsValueClauseSyntax> propertyInitialValuesStatic { get; private set; } = new Dictionary<string, EqualsValueClauseSyntax>();
    }
}
