namespace Kiwi.Parser.Nodes
{
    public class SyntaxWalkerBase : ISyntaxVisitor
    {
        public virtual void Visit(AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax)
        {
            foreach (var parameterSyntax in anonymousFunctionExpressionSyntax.Parameter)
            {
                Visit(parameterSyntax);
            }
            Visit(anonymousFunctionExpressionSyntax.Statements);
        }
        
        public virtual void Visit(ArrayAccessExpressionSyntax arrayAccessExpressionSyntax)
        {
            Visit(arrayAccessExpressionSyntax.Owner);
            foreach (var expressionSyntax in arrayAccessExpressionSyntax.Parameter)
            {
                Visit(expressionSyntax);
            }
        }

        public virtual void Visit(ArrayCreationExpressionSyntax arrayCreationExpressionSyntax)
        {
            Visit(arrayCreationExpressionSyntax.Type);
            foreach (var expressionSyntax in arrayCreationExpressionSyntax.Parameter)
            {
                Visit(expressionSyntax);
            }
        }

        public virtual void Visit(ArrayTypeSyntax arrayTypeSyntax)
        {
        }

        public virtual void Visit(AssignmentStatementSyntax assignmentStatementSyntax)
        {
            Visit(assignmentStatementSyntax.Member);
            Visit(assignmentStatementSyntax.ToAssign);
        }

        public virtual void Visit(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            Visit(binaryExpressionSyntax.LeftExpression);
            Visit(binaryExpressionSyntax.RightExpression);
        }

        public virtual void Visit(BooleanExpressionSyntax booleanExpressionSyntax)
        {
        }

        public virtual void Visit(CaseSyntax caseSyntax)
        {
            Visit(caseSyntax.Expression);
            Visit(caseSyntax.Statements);
        }

        public virtual void Visit(ClassSyntax classSyntax)
        {
            foreach (var fieldSyntax in classSyntax.Fields)
            {
                Visit(fieldSyntax);
            }
            foreach (var constructorSyntax in classSyntax.Constructors)
            {
                Visit(constructorSyntax);
            }
            foreach (var functionSyntax in classSyntax.Functions)
            {
                Visit(functionSyntax);
            }
        }

        public virtual void Visit(CompilationUnitSyntax compilationUnitSyntax)
        {
            foreach (var usingSyntax in compilationUnitSyntax.Usings)
            {
                Visit(usingSyntax);
            }
            foreach (var namespaceSyntax in compilationUnitSyntax.Namespaces)
            {
                Visit(namespaceSyntax);
            }
        }

        public virtual void Visit(ConditionalWhenEntry conditionalWhenEntry)
        {
            Visit(conditionalWhenEntry.Condition);
            Visit(conditionalWhenEntry.Statements);
        }

        public virtual void Visit(ConditionalMatchStatementSyntax conditionalMatchStatementSyntax)
        {
            Visit(conditionalMatchStatementSyntax.Condition);
            foreach (var conditionalWhenEntry in conditionalMatchStatementSyntax.WhenEntries)
            {
                Visit(conditionalWhenEntry);
            }
        }

        public virtual void Visit(ConstructorSyntax constructorSyntax)
        {
            foreach (var parameterSyntax in constructorSyntax.Parameter)
            {
                Visit(parameterSyntax);
            }
            Visit(constructorSyntax.Statements);
        }

        
        public virtual void Visit(DataSyntax dataSyntax)
        {
            foreach (var parameterSyntax in dataSyntax.Parameter)
            {
                Visit(parameterSyntax);
            }
        }

        public virtual void Visit(ElseSyntax elseSyntax)
        {
            Visit(elseSyntax.Statements);
        }

        public virtual void Visit(EnumMemberSyntax enumMemberSyntax)
        {
            if (enumMemberSyntax.Initializer != null)
            {
                Visit(enumMemberSyntax.Initializer);
            }
        }

        public virtual void Visit(EnumSyntax enumSyntax)
        {
            foreach (var enumMemberSyntax in enumSyntax.Member)
            {
                Visit(enumMemberSyntax);
            }
        }
        
        public virtual void Visit(FieldSyntax fieldSyntax)
        {
            if (fieldSyntax.Initializer != null)
            {
                Visit(fieldSyntax.Initializer);
            }
        }

        public virtual void Visit(FloatExpressionSyntax floatExpressionSyntax)
        {
        }

        public virtual void Visit(ForInStatementSyntax forInStatementSyntax)
        {
            if (forInStatementSyntax.VariableDeclarationStatement != null)
            {
                Visit(forInStatementSyntax.VariableDeclarationStatement);
            }
            if (forInStatementSyntax.ItemExpression != null)
            {
                Visit(forInStatementSyntax.ItemExpression);
            }
            Visit(forInStatementSyntax.CollExpression);
            Visit(forInStatementSyntax.Statements);
        }

        public virtual void Visit(ForStatementSyntax forStatementSyntax)
        {
            Visit(forStatementSyntax.InitStatement);
            Visit(forStatementSyntax.CondExpression);
            Visit(forStatementSyntax.LoopStatement);
            Visit(forStatementSyntax.Statements);
        }

        public virtual void Visit(FunctionSyntax functionSyntax)
        {
            foreach (var parameterSyntax in functionSyntax.Parameter)
            {
                Visit(parameterSyntax);
            }
            Visit(functionSyntax.Statements);
        }

        public virtual void Visit(OperatorFunctionSyntax functionSyntax)
        {
            foreach (var parameterSyntax in functionSyntax.Parameter)
            {
                Visit(parameterSyntax);
            }
            Visit(functionSyntax.Statements);
        }

        public virtual void Visit(IfElseExpressionSyntax ifElseExpressionSyntax)
        {
            Visit(ifElseExpressionSyntax.Condition);
            Visit(ifElseExpressionSyntax.IfTrueExpression);
            Visit(ifElseExpressionSyntax.IfFalseExpression);
        }

        public virtual void Visit(IfElseStatementSyntax ifElseStatementSyntax)
        {
            Visit(ifElseStatementSyntax.Condition);
            Visit(ifElseStatementSyntax.Statements);
            Visit(ifElseStatementSyntax.ElseStatements);
        }

        public virtual void Visit(IfStatementSyntax ifStatementSyntax)
        {
            Visit(ifStatementSyntax.Condition);
            Visit(ifStatementSyntax.Statements);
        }

        public virtual void Visit(
            ImplicitParameterTypeAnonymousFunctionExpressionSyntax
                implicitParameterTypeAnonymousFunctionExpressionSyntax)
        {
            foreach (var expressionSyntax in implicitParameterTypeAnonymousFunctionExpressionSyntax.Parameter)
            {
                Visit(expressionSyntax);
            }
            Visit(implicitParameterTypeAnonymousFunctionExpressionSyntax.Statements);
        }

        public virtual void Visit(IntExpressionSyntax intExpressionSyntax)
        {
        }

        public virtual void Visit(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            foreach (var expressionSyntax in invocationExpressionSyntax.Parameter)
            {
                Visit(expressionSyntax);
            }
            Visit(invocationExpressionSyntax.ToInvoke);
        }

        public virtual void Visit(InvocationStatementSyntax invocationStatementSyntax)
        {
            Visit(invocationStatementSyntax.InvocationExpression);
        }

        public virtual void Visit(MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            Visit(memberAccessExpressionSyntax.Owner);
        }

        public virtual void Visit(IdentifierExpressionSyntax identifierExpressionSyntax)
        {
        }

        public virtual void Visit(NamespaceSyntax namespaceSyntax)
        {
            foreach (var classSyntax in namespaceSyntax.Classes)
            {
                Visit(classSyntax);
            }
        }

        public virtual void Visit(ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
        {
            foreach (var expressionSyntax in objectCreationExpressionSyntax.Parameter)
            {
                Visit(expressionSyntax);
            }
        }

        public virtual void Visit(ParameterSyntax parameterSyntax)
        {
        }

        public virtual void Visit(ReturnStatementSyntax returnStatementSyntax)
        {
            Visit(returnStatementSyntax.Expression);
        }

        public virtual void Visit(SignExpressionSyntax signExpressionSyntax)
        {
            Visit(signExpressionSyntax.Expression);
        }

        public virtual void Visit(SimpleMatchStatementSyntax simpleMatchStatementSyntax)
        {
            foreach (var whenEntry in simpleMatchStatementSyntax.WhenEntries)
            {
                Visit(whenEntry);
            }
        }

        public virtual void Visit(StringExpressionSyntax stringExpressionSyntax)
        {
        }

        public virtual void Visit(SwitchStatementSyntax switchStatementSyntax)
        {
            Visit(switchStatementSyntax.Condition);
            foreach (var caseSyntax in switchStatementSyntax.Cases)
            {
                Visit(caseSyntax);
            }
            Visit(switchStatementSyntax.Else);
        }

        public virtual void Visit(TypeSyntax typeSyntax)
        {
        }

        public virtual void Visit(UsingSyntax usingSyntax)
        {
        }

        public virtual void Visit(VariableDeclarationStatementSyntax variableDeclarationStatementSyntax)
        {
            Visit(variableDeclarationStatementSyntax.InitExpression);
        }

        public virtual void Visit(VariablesDeclarationStatementSyntax variablesDeclarationStatementSyntax)
        {
            foreach (var variableDeclarationStatementSyntax in variablesDeclarationStatementSyntax.Declarations)
            {
                Visit(variableDeclarationStatementSyntax);
            }
        }

        public virtual void Visit(WhenEntry whenEntry)
        {
            Visit(whenEntry.Condition);
            Visit(whenEntry.Statements);
        }

        public virtual void Visit(WhenInExpressionSyntax whenInExpressionSyntax)
        {
            foreach (var expressionSyntax in whenInExpressionSyntax.InExpressionList)
            {
                Visit(expressionSyntax);
            }
        }

        public void Visit(ScopeStatementSyntax scopeStatement)
        {
            foreach (var statementSyntax in scopeStatement.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public void Visit(InfixFunctionInvocationExpressionSyntax infixFunctionInvocationExpressionSyntax)
        {
            Visit(infixFunctionInvocationExpressionSyntax.Left);
            Visit(infixFunctionInvocationExpressionSyntax.Identifier);
            Visit(infixFunctionInvocationExpressionSyntax.Right);
        }

        public void Visit(ISyntaxBase @base)
        {
            @base.Accept(this);
        }
    }
}