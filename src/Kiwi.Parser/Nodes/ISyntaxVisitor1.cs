namespace Kiwi.Parser.Nodes
{
    public interface ISyntaxVisitor<out TResult>
    {
        TResult Visit(ISyntaxBase @base);
        TResult Visit(AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax);
        TResult Visit(ArrayAccessExpression arrayAccessExpression);
        TResult Visit(ArrayCreationExpressionSyntax arrayCreationExpressionSyntax);
        TResult Visit(ArrayTypeSyntax arrayTypeSyntax);
        TResult Visit(AssignmentStatementSyntax assignmentStatementSyntax);
        TResult Visit(BinaryExpressionSyntax binaryExpressionSyntax);
        TResult Visit(BooleanExpressionSyntax booleanExpressionSyntax);
        TResult Visit(CaseSyntax caseSyntax);
        TResult Visit(ClassSyntax classSyntax);
        TResult Visit(CompilationUnitSyntax compilationUnitSyntax);
        TResult Visit(ConditionalWhenEntry conditionalWhenEntry);
        TResult Visit(ConditionalWhenStatementSyntax conditionalWhenStatementSyntax);
        TResult Visit(ConstructorSyntax constructorSyntax);
        TResult Visit(DataSyntax dataSyntax);
        TResult Visit(ElseSyntax elseSyntax);
        TResult Visit(EnumMemberSyntax enumMemberSyntax);
        TResult Visit(EnumSyntax enumSyntax);
        TResult Visit(ExpressionFunctionSyntax expressionFunctionSyntax);
        TResult Visit(FieldSyntax fieldSyntax);
        TResult Visit(FloatExpressionSyntax floatExpressionSyntax);
        TResult Visit(ForInStatementSyntax forInStatementSyntax);
        TResult Visit(ForStatementSyntax forStatementSyntax);
        TResult Visit(FunctionSyntax functionSyntax);
        TResult Visit(IfElseExpressionSyntax ifElseExpressionSyntax);
        TResult Visit(IfElseStatementSyntax ifElseStatementSyntax);
        TResult Visit(IfStatementSyntax ifStatementSyntax);

        TResult Visit(
            ImplicitParameterTypeAnonymousFunctionExpressionSyntax
                implicitParameterTypeAnonymousFunctionExpressionSyntax);

        TResult Visit(IntExpressionSyntax intExpressionSyntax);
        TResult Visit(InvocationExpressionSyntax invocationExpressionSyntax);
        TResult Visit(InvocationStatementSyntax invocationStatementSyntax);
        TResult Visit(MemberAccessExpressionSyntax memberAccessExpressionSyntax);
        TResult Visit(MemberOrTypeExpressionSyntax memberOrTypeExpressionSyntax);
        TResult Visit(NamespaceSyntax namespaceSyntax);
        TResult Visit(ObjectCreationExpressionSyntax objectCreationExpressionSyntax);
        TResult Visit(ParameterSyntax parameterSyntax);
        TResult Visit(ReturnStatementSyntax returnStatementSyntax);
        TResult Visit(SignExpressionSyntax signExpressionSyntax);
        TResult Visit(SimpleWhenStatementSyntax simpleWhenStatementSyntax);
        TResult Visit(StringExpressionSyntax stringExpressionSyntax);
        TResult Visit(SwitchStatementSyntax switchStatementSyntax);
        TResult Visit(TypeSyntax typeSyntax);
        TResult Visit(UsingSyntax usingSyntax);
        TResult Visit(VariableDeclarationStatementSyntax variableDeclarationStatementSyntax);
        TResult Visit(VariablesDeclarationStatementSyntax variablesDeclarationStatementSyntax);
        TResult Visit(WhenEntry whenEntry);
        TResult Visit(WhenInExpressionSyntax whenInExpressionSyntax);
    }
}