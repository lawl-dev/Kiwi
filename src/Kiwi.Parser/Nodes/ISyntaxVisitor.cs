namespace Kiwi.Parser.Nodes
{
    public interface ISyntaxVisitor
    {
        void Visit(AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax);
        void Visit(ArrayAccessExpressionSyntax arrayAccessExpressionSyntax);
        void Visit(ArrayCreationExpressionSyntax arrayCreationExpressionSyntax);
        void Visit(ArrayTypeSyntax arrayTypeSyntax);
        void Visit(AssignmentStatementSyntax assignmentStatementSyntax);
        void Visit(BinaryExpressionSyntax binaryExpressionSyntax);
        void Visit(BooleanExpressionSyntax booleanExpressionSyntax);
        void Visit(CaseSyntax caseSyntax);
        void Visit(ClassSyntax classSyntax);
        void Visit(CompilationUnitSyntax compilationUnitSyntax);
        void Visit(ConditionalWhenEntry conditionalWhenEntry);
        void Visit(ConditionalMatchStatementSyntax conditionalMatchStatementSyntax);
        void Visit(ConstructorSyntax constructorSyntax);
        void Visit(DataSyntax dataSyntax);
        void Visit(ElseSyntax elseSyntax);
        void Visit(EnumMemberSyntax enumMemberSyntax);
        void Visit(EnumSyntax enumSyntax);
        void Visit(FieldSyntax fieldSyntax);
        void Visit(FloatExpressionSyntax floatExpressionSyntax);
        void Visit(ForInStatementSyntax forInStatementSyntax);
        void Visit(ForStatementSyntax forStatementSyntax);
        void Visit(FunctionSyntax functionSyntax);
        void Visit(OperatorFunctionSyntax functionSyntax);
        void Visit(IfElseExpressionSyntax ifElseExpressionSyntax);
        void Visit(IfElseStatementSyntax ifElseStatementSyntax);
        void Visit(IfStatementSyntax ifStatementSyntax);

        void Visit(
            ImplicitParameterTypeAnonymousFunctionExpressionSyntax
                implicitParameterTypeAnonymousFunctionExpressionSyntax);

        void Visit(IntExpressionSyntax intExpressionSyntax);
        void Visit(InvocationExpressionSyntax invocationExpressionSyntax);
        void Visit(InvocationStatementSyntax invocationStatementSyntax);
        void Visit(MemberAccessExpressionSyntax memberAccessExpressionSyntax);
        void Visit(IdentifierExpressionSyntax identifierExpressionSyntax);
        void Visit(NamespaceSyntax namespaceSyntax);
        void Visit(ObjectCreationExpressionSyntax objectCreationExpressionSyntax);
        void Visit(ParameterSyntax parameterSyntax);
        void Visit(ReturnStatementSyntax returnStatementSyntax);
        void Visit(SignExpressionSyntax signExpressionSyntax);
        void Visit(SimpleMatchStatementSyntax simpleMatchStatementSyntax);
        void Visit(StringExpressionSyntax stringExpressionSyntax);
        void Visit(SwitchStatementSyntax switchStatementSyntax);
        void Visit(TypeSyntax typeSyntax);
        void Visit(UsingSyntax usingSyntax);
        void Visit(VariableDeclarationStatementSyntax variableDeclarationStatementSyntax);
        void Visit(VariablesDeclarationStatementSyntax variablesDeclarationStatementSyntax);
        void Visit(WhenEntry whenEntry);
        void Visit(WhenInExpressionSyntax whenInExpressionSyntax);
        void Visit(ScopeStatementSyntax scopeStatement);
        void Visit(InfixFunctionInvocationExpressionSyntax infixFunctionInvocationExpressionSyntax);
    }
}