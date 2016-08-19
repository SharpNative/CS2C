using LibCS2C.Context;

namespace LibCS2C.Generators
{
    public class AllGenerators
    {
        public AssignmentGenerator AddAssignment { get; private set; }
        public AddressOfExpressionGenerator AddressOfExpression { get; private set; }
        public ArgumentListGenerator ArgumentList { get; private set; }
        public ArrayCreationExpressionGenerator ArrayCreationExpression { get; private set; }
        public AssignmentGenerator BinaryAndAssignment { get; private set; }
        public AssignmentGenerator BinaryOrAssignment { get; private set; }
        public BlockGenerator Block { get; private set; }
        public CastExpressionGenerator CastExpression { get; private set; }
        public ConditionalAccessExpressionGenerator ConditionalAccessExpression { get; private set; }
        public ClassCodeGenerator ClassCode { get; private set; }
        public DelegateDeclarationGenerator DelegateDeclaration { get; private set; }
        public DoStatementGenerator DoStatement { get; private set; }
        public AssignmentGenerator DivideAssignment { get; private set; }
        public ElementAccessGenerator ElementAccess { get; private set; }
        public EnumGenerator Enum { get; private set; }
        public AssignmentGenerator ExclusiveOrAssignment { get; private set; }
        public ExpressionGenerator Expression { get; private set; }
        public ExpressionStatementGenerator ExpressionStatement { get; private set; }
        public FixedStatementGenerator FixedStatement { get; private set; }
        public ForStatementGenerator ForStatement { get; private set; }
        public GotoStatementGenerator GotoStatement { get; private set; }
        public IfStatementGenerator IfStatement { get; private set; }
        public InvocationGenerator Invocation { get; private set; }
        public LabeledStatementGenerator LabeledStatement { get; private set; }
        public AssignmentGenerator LeftShiftAssignment { get; private set; }
        public LocalDeclarationGenerator LocalDeclaration { get; private set; }
        public MethodGenerator MethodDeclaration { get; private set; }
        public AssignmentGenerator ModuloAssignment { get; private set; }
        public AssignmentGenerator MultiplyAssignment { get; private set; }
        public ObjectCreationExpressionGenerator ObjectCreationExpression { get; private set; }
        public PrePostExpressionGenerator PreIncrementExpression { get; private set; }
        public PrePostExpressionGenerator PreDecrementExpression { get; private set; }
        public PrePostExpressionGenerator PostIncrementExpression { get; private set; }
        public PrePostExpressionGenerator PostDecrementExpression { get; private set; }
        public PointerMemberAccessGenerator PointerMemberAccessExpression { get; private set; }
        public PropertyGenerator Property { get; private set; }
        public ReturnStatementGenerator ReturnStatement { get; private set; }
        public AssignmentGenerator RightShiftAssignment { get; private set; }
        public SimpleAssignmentGenerator SimpleAssignment { get; private set; }
        public SimpleMemberAccessGenerator SimpleMemberAccess { get; private set; }
        public SizeofExpressionGenerator SizeOfExpression { get; private set; }
        public StructGenerator Struct { get; private set; }
        public AssignmentGenerator SubstractAssignment { get; private set; }
        public SwitchStatementGenerator SwitchStatement { get; private set; }
        public VariableGenerator Variable { get; private set; }
        public WhileStatementGenerator WhileStatement { get; private set; }

        /// <summary>
        /// A list with all the generators
        /// </summary>
        /// <param name="context">The context</param>
        public AllGenerators(WalkerContext context)
        {
            AddAssignment = new AssignmentGenerator(context, AssignmentType.Add);
            AddressOfExpression = new AddressOfExpressionGenerator(context);
            ArgumentList = new ArgumentListGenerator(context);
            ArrayCreationExpression = new ArrayCreationExpressionGenerator(context);
            BinaryAndAssignment = new AssignmentGenerator(context, AssignmentType.BinaryAnd);
            BinaryOrAssignment = new AssignmentGenerator(context, AssignmentType.BinaryOr);
            Block = new BlockGenerator(context);
            CastExpression = new CastExpressionGenerator(context);
            ConditionalAccessExpression = new ConditionalAccessExpressionGenerator(context);
            ClassCode = new ClassCodeGenerator(context);
            DelegateDeclaration = new DelegateDeclarationGenerator(context);
            DoStatement = new DoStatementGenerator(context);
            DivideAssignment = new AssignmentGenerator(context, AssignmentType.Divide);
            ElementAccess = new ElementAccessGenerator(context);
            Enum = new EnumGenerator(context);
            ExclusiveOrAssignment = new AssignmentGenerator(context, AssignmentType.ExclusiveOr);
            Expression = new ExpressionGenerator(context);
            ExpressionStatement = new ExpressionStatementGenerator(context);
            FixedStatement = new FixedStatementGenerator(context);
            ForStatement = new ForStatementGenerator(context);
            GotoStatement = new GotoStatementGenerator(context);
            IfStatement = new IfStatementGenerator(context);
            Invocation = new InvocationGenerator(context);
            LabeledStatement = new LabeledStatementGenerator(context);
            LeftShiftAssignment = new AssignmentGenerator(context, AssignmentType.LeftShift);
            LocalDeclaration = new LocalDeclarationGenerator(context);
            MethodDeclaration = new MethodGenerator(context);
            ModuloAssignment = new AssignmentGenerator(context, AssignmentType.Modulo);
            MultiplyAssignment = new AssignmentGenerator(context, AssignmentType.Multiply);
            ObjectCreationExpression = new ObjectCreationExpressionGenerator(context);
            PreIncrementExpression = new PrePostExpressionGenerator(context, ExpressionType.PreIncrement);
            PreDecrementExpression = new PrePostExpressionGenerator(context, ExpressionType.PreDecrement);
            PostIncrementExpression = new PrePostExpressionGenerator(context, ExpressionType.PostIncrement);
            PostDecrementExpression = new PrePostExpressionGenerator(context, ExpressionType.PostDecrement);
            PointerMemberAccessExpression = new PointerMemberAccessGenerator(context);
            Property = new PropertyGenerator(context);
            ReturnStatement = new ReturnStatementGenerator(context);
            RightShiftAssignment = new AssignmentGenerator(context, AssignmentType.RightShift);
            SimpleAssignment = new SimpleAssignmentGenerator(context);
            SimpleMemberAccess = new SimpleMemberAccessGenerator(context);
            SizeOfExpression = new SizeofExpressionGenerator(context);
            Struct = new StructGenerator(context);
            SubstractAssignment = new AssignmentGenerator(context, AssignmentType.Substract);
            SwitchStatement = new SwitchStatementGenerator(context);
            Variable = new VariableGenerator(context);
            WhileStatement = new WhileStatementGenerator(context);
        }
    }
}
