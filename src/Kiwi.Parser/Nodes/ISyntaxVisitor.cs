namespace Kiwi.Parser.Nodes
{
    public interface ISyntaxVisitor
    {
        void Visit(AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(ArrayAccessExpression anonymousFunctionExpressionSyntax);
        void Visit(ArrayCreationExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(ArrayTypeSyntax anonymousFunctionExpressionSyntax);
        void Visit(AssignmentStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(BinaryExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(BooleanExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(CaseSyntax anonymousFunctionExpressionSyntax);
        void Visit(ClassSyntax anonymousFunctionExpressionSyntax);
        void Visit(CompilationUnitSyntax anonymousFunctionExpressionSyntax);
        void Visit(ConditionalWhenEntry anonymousFunctionExpressionSyntax);
        void Visit(ConditionalWhenStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(ConstructorSyntax anonymousFunctionExpressionSyntax);
        void Visit(DataClassFunctionSyntax anonymousFunctionExpressionSyntax);
        void Visit(DataSyntax anonymousFunctionExpressionSyntax);
        void Visit(ElseSyntax anonymousFunctionExpressionSyntax);
        void Visit(EnumMemberSyntax anonymousFunctionExpressionSyntax);
        void Visit(EnumSyntax anonymousFunctionExpressionSyntax);
        void Visit(ExpressionFunctionSyntax anonymousFunctionExpressionSyntax);
        void Visit(FieldSyntax anonymousFunctionExpressionSyntax);
        void Visit(FloatExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(ForInStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(ForStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(FunctionSyntax anonymousFunctionExpressionSyntax);
        void Visit(IfElseExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(IfElseStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(IfStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(ImplicitParameterTypeAnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(IntExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(InvocationExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(InvocationStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(MemberAccessExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(MemberExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(NamespaceSyntax anonymousFunctionExpressionSyntax);
        void Visit(ObjectCreationExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(ParameterSyntax anonymousFunctionExpressionSyntax);
        void Visit(ReturnStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(SignExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(SimpleWhenStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(StringExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(SwitchStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(TypeSyntax anonymousFunctionExpressionSyntax);
        void Visit(UsingSyntax anonymousFunctionExpressionSyntax);
        void Visit(VariableDeclarationStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(VariablesDeclarationStatementSyntax anonymousFunctionExpressionSyntax);
        void Visit(WhenEntry anonymousFunctionExpressionSyntax);
        void Visit(WhenInExpressionSyntax anonymousFunctionExpressionSyntax);
    }
}