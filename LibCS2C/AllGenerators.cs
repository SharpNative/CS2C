using LibCS2C.Generators;

namespace LibCS2C
{
    public class AllGenerators
    {
        public AddressOfExpressionGenerator AddressOfExpression { get; private set; }
        public AssignmentGenerator LeftShiftAssignment { get; private set; }
        public AssignmentGenerator RightShiftAssignment { get; private set; }
        public AssignmentGenerator BinaryOrAssignment { get; private set; }
        public AssignmentGenerator ExclusiveOrAssignment { get; private set; }
        public AssignmentGenerator BinaryAndAssignment { get; private set; }
        public AssignmentGenerator AddAssignment { get; private set; }
        public AssignmentGenerator SubstractAssignment { get; private set; }
        public AssignmentGenerator MultiplyAssignment { get; private set; }
        public AssignmentGenerator DivideAssignment { get; private set; }
        public ArgumentListGenerator ArgumentList { get; private set; }
        public ArrayCreationExpressionGenerator ArrayCreationExpression { get; private set; }
        public BlockGenerator Block { get; private set; }
        public CastExpressionGenerator CastExpression { get; private set; }
        public ClassCodeGenerator ClassCode { get; private set; }
        public DoStatementGenerator DoStatement { get; private set; }
        public ElementAccessGenerator ElementAccess { get; private set; }
        public EnumGenerator Enum { get; private set; }
        public ExpressionGenerator Expression { get; private set; }
        public ExpressionStatementGenerator ExpressionStatement { get; private set; }
        public FixedStatementGenerator FixedStatement { get; private set; }
        public ForStatementGenerator ForStatement { get; private set; }
        public IfStatementGenerator IfStatement { get; private set; }
        public InvocationGenerator Invocation { get; private set; }
        public LocalDeclarationGenerator LocalDeclaration { get; private set; }
        public MethodGenerator ConstructorDeclaration { get; private set; }
        public MethodGenerator MethodDeclaration { get; private set; }
        public ObjectCreationExpressionGenerator ObjectCreationExpression { get; private set; }
        public PrePostExpressionGenerator PreIncrementExpression { get; private set; }
        public PrePostExpressionGenerator PreDecrementExpression { get; private set; }
        public PrePostExpressionGenerator PostIncrementExpression { get; private set; }
        public PrePostExpressionGenerator PostDecrementExpression { get; private set; }
        public PropertyGenerator Property { get; private set; }
        public ReturnStatementGenerator ReturnStatement { get; private set; }
        public SimpleAssignmentGenerator SimpleAssignment { get; private set; }
        public SimpleMemberAccessGenerator SimpleMemberAccess { get; private set; }
        public SizeofExpressionGenerator SizeOfExpression { get; private set; }
        public StructGenerator Struct { get; private set; }
        public VariableGenerator Variable { get; private set; }
        public WhileStatementGenerator WhileStatement { get; private set; }

        /// <summary>
        /// A list with all the generators
        /// </summary>
        /// <param name="context">The context</param>
        public AllGenerators(WalkerContext context)
        {
            AddressOfExpression = new AddressOfExpressionGenerator(context);
            LeftShiftAssignment = new AssignmentGenerator(context, AssignmentType.LeftShift);
            RightShiftAssignment = new AssignmentGenerator(context, AssignmentType.RightShift);
            BinaryOrAssignment = new AssignmentGenerator(context, AssignmentType.BinaryOr);
            ExclusiveOrAssignment = new AssignmentGenerator(context, AssignmentType.ExclusiveOr);
            BinaryAndAssignment = new AssignmentGenerator(context, AssignmentType.BinaryAnd);
            AddAssignment = new AssignmentGenerator(context, AssignmentType.Add);
            SubstractAssignment = new AssignmentGenerator(context, AssignmentType.Substract);
            MultiplyAssignment = new AssignmentGenerator(context, AssignmentType.Multiply);
            DivideAssignment = new AssignmentGenerator(context, AssignmentType.Divide);
            ArgumentList = new ArgumentListGenerator(context);
            ArrayCreationExpression = new ArrayCreationExpressionGenerator(context);
            Block = new BlockGenerator(context);
            CastExpression = new CastExpressionGenerator(context);
            ClassCode = new ClassCodeGenerator(context);
            DoStatement = new DoStatementGenerator(context);
            ElementAccess = new ElementAccessGenerator(context);
            Enum = new EnumGenerator(context);
            Expression = new ExpressionGenerator(context);
            ExpressionStatement = new ExpressionStatementGenerator(context);
            FixedStatement = new FixedStatementGenerator(context);
            ForStatement = new ForStatementGenerator(context);
            IfStatement = new IfStatementGenerator(context);
            Invocation = new InvocationGenerator(context);
            LocalDeclaration = new LocalDeclarationGenerator(context);
            ConstructorDeclaration = new MethodGenerator(context, MethodGeneratorType.Constructor);
            MethodDeclaration = new MethodGenerator(context, MethodGeneratorType.Method);
            ObjectCreationExpression = new ObjectCreationExpressionGenerator(context);
            PreIncrementExpression = new PrePostExpressionGenerator(context, ExpressionType.PreIncrement);
            PreDecrementExpression = new PrePostExpressionGenerator(context, ExpressionType.PreDecrement);
            PostIncrementExpression = new PrePostExpressionGenerator(context, ExpressionType.PostIncrement);
            PostDecrementExpression = new PrePostExpressionGenerator(context, ExpressionType.PostDecrement);
            Property = new PropertyGenerator(context);
            ReturnStatement = new ReturnStatementGenerator(context);
            SimpleAssignment = new SimpleAssignmentGenerator(context);
            SimpleMemberAccess = new SimpleMemberAccessGenerator(context);
            SizeOfExpression = new SizeofExpressionGenerator(context);
            Struct = new StructGenerator(context);
            Variable = new VariableGenerator(context);
            WhileStatement = new WhileStatementGenerator(context);
        }
    }
}
