using System;

namespace Kiwi.Parser.Nodes
{
    public class SyntaxWalkerBase : ISyntaxVisitor
    {
        public void Visit(ISyntaxBase @base)
        {
            @base.Accept(this);
        }

        public virtual void Visit(AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax)
        {
            foreach (var parameterSyntax in anonymousFunctionExpressionSyntax.Parameter)
            {
                Visit(parameterSyntax);
            }
            foreach (var statementSyntax in anonymousFunctionExpressionSyntax.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(ArrayAccessExpression arrayAccessExpression)
        {
            Visit(arrayAccessExpression.Owner);
            foreach (var expressionSyntax in arrayAccessExpression.Parameter)
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
            foreach (var statementSyntax in caseSyntax.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(ClassSyntax classSyntax)
        {
            foreach (var fieldSyntax in classSyntax.FieldMember)
            {
                Visit(fieldSyntax);
            }
            foreach (var constructorSyntax in classSyntax.ConstructorMember)
            {
                Visit(constructorSyntax);
            }
            foreach (var functionSyntax in classSyntax.FunctionMember)
            {
                Visit(functionSyntax);
            }
        }

        public virtual void Visit(CompilationUnitSyntax compilationUnitSyntax)
        {
            foreach (var usingSyntax in compilationUnitSyntax.UsingMember)
            {
                Visit(usingSyntax);
            }
            foreach (var namespaceSyntax in compilationUnitSyntax.NamespaceMember)
            {
                Visit(namespaceSyntax);
            }
        }

        public virtual void Visit(ConditionalWhenEntry conditionalWhenEntry)
        {
            Visit(conditionalWhenEntry.Condition);
            foreach (var statementSyntax in conditionalWhenEntry.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(ConditionalWhenStatementSyntax conditionalWhenStatementSyntax)
        {
            Visit(conditionalWhenStatementSyntax.Condition);
            foreach (var conditionalWhenEntry in conditionalWhenStatementSyntax.WhenEntries)
            {
                Visit(conditionalWhenEntry);
            }
        }

        public virtual void Visit(ConstructorSyntax constructorSyntax)
        {
            foreach (var parameterSyntax in constructorSyntax.ArgList)
            {
                Visit(parameterSyntax);
            }
            foreach (var statementSyntax in constructorSyntax.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(DataClassFunctionSyntax dataClassFunctionSyntax)
        {
            foreach (var parameterSyntax in dataClassFunctionSyntax.ParameterList)
            {
                Visit(parameterSyntax);
            }
            Visit(dataClassFunctionSyntax.DataClassSyntax);
            foreach (var statementSyntax in dataClassFunctionSyntax.Statements)
            {
                Visit(statementSyntax);
            }
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
            foreach (var statementSyntax in elseSyntax.Statements)
            {
                Visit(statementSyntax);
            }
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
            foreach (var enumMemberSyntax in enumSyntax.EnumMember)
            {
                Visit(enumMemberSyntax);
            }
        }

        public virtual void Visit(ExpressionFunctionSyntax expressionFunctionSyntax)
        {
            foreach (var parameterSyntax in expressionFunctionSyntax.ParameterList)
            {
                Visit(parameterSyntax);
            }
            foreach (var statementSyntax in expressionFunctionSyntax.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(FieldSyntax fieldSyntax)
        {
            if (fieldSyntax.FieldInitializer != null)
            {
                Visit(fieldSyntax.FieldInitializer);
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
            foreach (var statementSyntax in forInStatementSyntax.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(ForStatementSyntax forStatementSyntax)
        {
            Visit(forStatementSyntax.InitStatement);
            Visit(forStatementSyntax.CondExpression);
            Visit(forStatementSyntax.LoopExpression);
            foreach (var statementSyntax in forStatementSyntax.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(FunctionSyntax functionSyntax)
        {
            foreach (var parameterSyntax in functionSyntax.ParameterList)
            {
                Visit(parameterSyntax);
            }
            foreach (var statementSyntax in functionSyntax.Statements)
            {
                Visit(statementSyntax);
            }
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
            foreach (var statementSyntax in ifElseStatementSyntax.Statements)
            {
                Visit(statementSyntax);
            }
            foreach (var statementSyntax in ifElseStatementSyntax.ElseStatements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(IfStatementSyntax ifStatementSyntax)
        {
            Visit(ifStatementSyntax.Condition);
            foreach (var statementSyntax in ifStatementSyntax.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(
            ImplicitParameterTypeAnonymousFunctionExpressionSyntax implicitParameterTypeAnonymousFunctionExpressionSyntax)
        {
            foreach (var expressionSyntax in implicitParameterTypeAnonymousFunctionExpressionSyntax.Parameter)
            {
                Visit(expressionSyntax);
            }
            foreach (var statementSyntax in implicitParameterTypeAnonymousFunctionExpressionSyntax.Statements)
            {
                Visit(statementSyntax);
            }
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

        public virtual void Visit(MemberExpressionSyntax memberExpressionSyntax)
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

        public virtual void Visit(SimpleWhenStatementSyntax simpleWhenStatementSyntax)
        {
            foreach (var whenEntry in simpleWhenStatementSyntax.WhenEntries)
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
            foreach (var statementSyntax in whenEntry.Statements)
            {
                Visit(statementSyntax);
            }
        }

        public virtual void Visit(WhenInExpressionSyntax whenInExpressionSyntax)
        {
            foreach (var expressionSyntax in whenInExpressionSyntax.InExpressionList)
            {
                Visit(expressionSyntax);
            }
        }
    }
}