using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Common.Extensions;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class Binder
    {
        private readonly BasicSymbolService _basicSymbolService = new BasicSymbolService();
        private readonly BindingContextService _contextService = new BindingContextService();
        
        public List<BoundCompilationUnit> Bind(List<CompilationUnitSyntax> compilationUnits)
        {
            var basicModels = _basicSymbolService.CreateBasicModel(compilationUnits);
            var allNamespaces = basicModels.SelectMany(x => x.Namespaces).ToList();

            foreach (var basicModel in basicModels)
            {
                basicModel.BoundUsings.AddRange(((CompilationUnitSyntax)basicModel.Syntax).UsingMember.Select(x => BindUsing(x, allNamespaces)));

                foreach (var boundNamespace in basicModel.Namespaces)
                {
                    BindNamespace(boundNamespace, (NamespaceSyntax)boundNamespace.Syntax);
                }
            }
            return basicModels;
        }

        private void BindNamespace(BoundNamespace boundNamespace, NamespaceSyntax syntax)
        {
            _contextService.Load(boundNamespace);
            foreach (var boundType in boundNamespace.TypesInternal)
            {
                BindClass(boundType, (ClassSyntax)boundType.Syntax);
            }
            _contextService.Unload(boundNamespace);
        }

        private BoundType BindClass(BoundType @class, ClassSyntax syntax)
        {
            _contextService.EnterClass(@class);
            foreach (var boundField in @class.FieldsInternal)
            {
                BindField(boundField, (FieldSyntax)boundField.Syntax);
            }
            foreach (var boundFunction in @class.FunctionsInternal)
            {
                _contextService.EnterScope();
                BindFunction(boundFunction, (FunctionSyntax)boundFunction.Syntax);
                _contextService.ExitScope();
            }
            @class.ConstructorsInternal.AddRange(syntax.ConstructorMember.Select(BindConstructor).ToList());
            _contextService.ExitClass();
            return @class;
        }

        private BoundFunction BindFunction(BoundFunction boundFunction, FunctionSyntax functionSyntax)
        {
            return functionSyntax.TypeSwitchExpression<FunctionSyntax, BoundFunction>()
                                 .Case<ReturnFunctionSyntax>(x => BindReturnFunction(boundFunction, x))
                                 .Case<ExpressionFunctionSyntax>(x => BindExpressionFunction(boundFunction, x))
                                 .Case<FunctionSyntax>(x => BindVoid(boundFunction, x))
                                 .Default(() => { throw new NotImplementedException(); })
                                 .Done();
        }

        private BoundFunction BindReturnFunction(BoundFunction boundFunction, ReturnFunctionSyntax returnFunctionSyntax)
        {
            var boundParameters = returnFunctionSyntax.ParameterList.Select(BindParameter).ToList();
            var type = _contextService.LookupType(returnFunctionSyntax.ReturnType.TypeName.Value);
            boundFunction.Statements = BindScope((ScopeStatementSyntax)returnFunctionSyntax.Statements);
            boundFunction.Parameter = boundParameters;
            boundFunction.Type = new FunctionCompilerGeneratedType(boundParameters.Select(x => x.Type).ToList(), type);
            return boundFunction;
        }

        private BoundScopeStatement BindScope(ScopeStatementSyntax statements)
        {
            _contextService.EnterScope();
            var boundScopeStatement = new BoundScopeStatement(statements.Statements.Select(BindStatement).ToList(), statements);
            _contextService.ExitScope();
            return boundScopeStatement;
        }

        private BoundFunction BindExpressionFunction(
            BoundFunction boundFunction,
            ExpressionFunctionSyntax expressionFunctionSyntax)
        {
            var boundParameters = expressionFunctionSyntax.ParameterList.Select(BindParameter).ToList();
            var boundReturnStatement = BindReturnStatement((ReturnStatementSyntax)expressionFunctionSyntax.Statements);
            boundFunction.Parameter = boundParameters;
            boundFunction.Statements = boundReturnStatement;
            boundFunction.Type = new FunctionCompilerGeneratedType(
                boundParameters.Select(x => x.Type).ToList(),
                boundReturnStatement.BoundExpression.Type);
            boundFunction.ReturnType = boundReturnStatement.BoundExpression.Type;
            return boundFunction;
        }

        private BoundFunction BindVoid(BoundFunction function, FunctionSyntax functionSyntax)
        {
            var boundParameters = functionSyntax.ParameterList.Select(BindParameter).ToList();
            function.Statements = BindScope((ScopeStatementSyntax)functionSyntax.Statements);
            function.Parameter = boundParameters;
            function.Type = new FunctionCompilerGeneratedType(boundParameters.Select(x => x.Type).ToList(), null);
            return function;
        }

        private BoundParameter BindParameter(ParameterSyntax parameterSyntax)
        {
            var type = _contextService.LookupType(parameterSyntax.Type.TypeName.Value);
            var boundParameter = new BoundParameter(parameterSyntax.ParameterName.Value, type, parameterSyntax);
            _contextService.AddLocal(boundParameter.Name, boundParameter);
            return boundParameter;
        }

        private BoundConstructor BindConstructor(ConstructorSyntax constructorSyntax)
        {
            var boundParameters = constructorSyntax.ArgList.Select(BindParameter).ToList();
            var boundStatements = BindScope(constructorSyntax.Statements);
            return new BoundConstructor(boundParameters, boundStatements, constructorSyntax);
        }

        private BoundStatement BindStatement(IStatementSyntax statementSyntax)
        {
            return statementSyntax.TypeSwitchExpression<IStatementSyntax, BoundStatement>()
                                  .Case<VariablesDeclarationStatementSyntax>(BindVariablesDeclarationStatement)
                                  .Case<VariableDeclarationStatementSyntax>(BindVariableDeclarationStatement)
                                  .Case<ReturnStatementSyntax>(BindReturnStatement)
                                  .Case<IfElseStatementSyntax>(BindIfElseStatement)
                                  .Case<IfStatementSyntax>(BindIfStatement)
                                  .Case<AssignmentStatementSyntax>(BindAssignmentStatement)
                                  .Case<ForStatementSyntax>(BindForStatement)
                                  .Case<ForInStatementSyntax>(BindForInStatement)
                                  .Default(() => { throw new NotImplementedException(); })
                                  .Done();
        }

        private BoundStatement BindForInStatement(ForInStatementSyntax statementSyntax)
        {
            throw new NotImplementedException();
        }

        private BoundStatement BindForStatement(ForStatementSyntax statementSyntax)
        {
            var initStatement = BindStatement(statementSyntax.InitStatement);
            var condition = BindExpression(statementSyntax.CondExpression);
            var loopStatement = BindStatement(statementSyntax.LoopStatement);
            var boundStatements = BindScope(statementSyntax.Statements);
            return new BoundForStatement(initStatement, condition, loopStatement, boundStatements, statementSyntax);
        }

        private BoundAssignStatement BindAssignmentStatement(AssignmentStatementSyntax statementSyntax)
        {
            var boundExpression = BindExpression(statementSyntax.Member);
            if (!(boundExpression is BoundMemberExpression)
                && !(boundExpression is BoundMemberAccessExpression))
            {
                throw new KiwiSemanticException("No Member");
            }
            var toAssignExpression = BindExpression(statementSyntax.ToAssign);

            AssignmentOperators assignOperator;
            switch (statementSyntax.Operator.Type)
            {
                case TokenType.Colon:
                    assignOperator = AssignmentOperators.SimpleAssignment;
                    break;
                case TokenType.ColonDiv:
                    assignOperator = AssignmentOperators.DivAssignment;
                    break;
                case TokenType.ColonMult:
                    assignOperator = AssignmentOperators.MultAssignment;
                    break;
                case TokenType.ColonAdd:
                    assignOperator = AssignmentOperators.AddAssignment;
                    break;
                case TokenType.ColonPow:
                    assignOperator = AssignmentOperators.PowAssignment;
                    break;
                case TokenType.ColonSub:
                    assignOperator = AssignmentOperators.SubAssignment;
                    break;
                default:
                    throw new KiwiSemanticException($"Unknown Assignoperator {statementSyntax.Operator.Value}");
            }
            return new BoundAssignStatement(boundExpression, toAssignExpression, assignOperator, statementSyntax);
        }

        private BoundStatement BindIfElseStatement(IfElseStatementSyntax statementSyntax)
        {
            var boundExpression = BindExpression(statementSyntax.Condition);
            var boundStatements = BindScope(statementSyntax.Statements);
            var elseBoundStatements = BindScope(statementSyntax.ElseStatements);
            return new BoundIfElseStatement(boundExpression, boundStatements, elseBoundStatements, statementSyntax);
        }

        private BoundStatement BindIfStatement(IfStatementSyntax statementSyntax)
        {
            var boundExpression = BindExpression(statementSyntax.Condition);
            _contextService.EnterScope();
            var boundStatements = BindScope(statementSyntax.Statements);
            _contextService.ExitScope();
            return new BoundIfStatement(boundExpression, boundStatements, statementSyntax);
        }

        private BoundReturnStatement BindReturnStatement(ReturnStatementSyntax statementSyntax)
        {
            var boundExpression = BindExpression(statementSyntax.Expression);
            return new BoundReturnStatement(boundExpression, statementSyntax);
        }

        private BoundStatement BindVariablesDeclarationStatement(VariablesDeclarationStatementSyntax statementSyntax)
        {
            var boundVariableDeclarationStatements =
                statementSyntax.Declarations.Select(BindVariableDeclarationStatement).ToList();
            return new BoundVariablesDeclarationStatement(boundVariableDeclarationStatements, statementSyntax);
        }

        private BoundVariableDeclarationStatement BindVariableDeclarationStatement(
            VariableDeclarationStatementSyntax syntax)
        {
            var qualifier = GetQualifier(syntax.VariableQualifier.Type);
            var boundExpression = BindExpression(syntax.InitExpression);

            var boundVariableDeclarationStatement = new BoundVariableDeclarationStatement(
                syntax.Identifier.Value,
                qualifier,
                boundExpression,
                syntax);
            _contextService.AddLocal(syntax.Identifier.Value, boundVariableDeclarationStatement);
            return boundVariableDeclarationStatement;
        }

        private BoundUsing BindUsing(UsingSyntax usingSyntax, List<BoundNamespace> allNamespaces)
        {
            var boundNamespace = allNamespaces.Single(x => x.Name == usingSyntax.NamespaceName.Value);
            _contextService.Load(boundNamespace);
            var boundUsing = new BoundUsing(usingSyntax.NamespaceName.Value, usingSyntax, boundNamespace);
            return boundUsing;
        }

        private BoundField BindField(BoundField field, FieldSyntax syntax)
        {
            var qualifier = GetQualifier(syntax.FieldTypeQualifier.Type);
            field.Qualifier = qualifier;
            var boundExpression = BindExpression(syntax.FieldInitializer);
            field.Initializer = boundExpression;
            field.Type = boundExpression.Type;
            return field;
        }

        private static VariableQualifier GetQualifier(TokenType tokenType)
        {
            VariableQualifier qualifier;
            switch (tokenType)
            {
                case TokenType.VarKeyword:
                    qualifier = VariableQualifier.Var;
                    break;
                case TokenType.ConstKeyword:
                    qualifier = VariableQualifier.Const;
                    break;
                default:
                    throw new KiwiSemanticException($"Forbidden Variable Qualifier {tokenType}");
            }
            return qualifier;
        }

        private BoundExpression BindExpression(IExpressionSyntax expressionSyntax, List<IType> args = null)
        {
            return expressionSyntax.TypeSwitchExpression<IExpressionSyntax, BoundExpression>()
                                   .Case<BooleanExpressionSyntax>(BindBooleanExpression)
                                   .Case<IntExpressionSyntax>(BindIntExpression)
                                   .Case<InvocationExpressionSyntax>(BindInvocationExpression)
                                   .Case<MemberExpressionSyntax>(x => BindMemerExpression(x, args))
                                   .Case<ObjectCreationExpressionSyntax>(BindObjectCreationExpression)
                                   .Case<ArrayCreationExpressionSyntax>(BindArrayCreationExpression)
                                   .Case<StringExpressionSyntax>(BindStringExpression)
                                   .Case<MemberAccessExpressionSyntax>(x => BindMemberAccessExpression(x, args))
                                   .Default(() => { throw new NotImplementedException(); })
                                   .Done();
        }

        private BoundMemberAccessExpression BindMemberAccessExpression(MemberAccessExpressionSyntax expressionSyntax, List<IType> parameterTypes)
        {
            var boundExpression = BindExpression(expressionSyntax.Owner);
            var boundFunction =
                boundExpression.Type.Functions.SingleOrDefault(
                    x =>
                    x.Name == expressionSyntax.MemberName.Value
                    && Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes));

            if (boundFunction != null)
            {
                return new BoundMemberAccessExpression(expressionSyntax.MemberName.Value, boundFunction, expressionSyntax);
            }

            var boundField = boundExpression.Type.Fields.SingleOrDefault(x => x.Name == expressionSyntax.MemberName.Value);
            if (boundField != null)
            {
                return new BoundMemberAccessExpression(
                    expressionSyntax.MemberName.Value,
                    boundField,
                    expressionSyntax);
            }
            throw new KiwiSemanticException($"{expressionSyntax.MemberName.Value} not defined");
        }

        private BoundExpression BindStringExpression(StringExpressionSyntax expressionSyntax)
        {
            return new BoundStringExpression(expressionSyntax.Value.Value, expressionSyntax);
        }

        private BoundExpression BindIntExpression(IntExpressionSyntax expressionSyntax)
        {
            return new BoundIntExpression(expressionSyntax);
        }

        private BoundObjectCreationExpression BindObjectCreationExpression(
            ObjectCreationExpressionSyntax expressionSyntax)
        {
            var type = (BoundType)_contextService.LookupType(expressionSyntax.Type.TypeName.Value);
            var boundParameter = expressionSyntax.Parameter.Select(x => BindExpression(x)).ToList();
            var parameterTypes = boundParameter.Select(x => x.Type).ToList();
            BoundConstructor boundConstructor = null;
            if (parameterTypes.Any())
            {
                boundConstructor =
                    type.ConstructorsInternal.Single(
                        x => Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes));
            }
            return new BoundObjectCreationExpression(type, boundConstructor, boundParameter, expressionSyntax);
        }

        private BoundArrayCreationExpression BindArrayCreationExpression(ArrayCreationExpressionSyntax expressionSyntax)
        {
            var type = (BoundType)_contextService.LookupType(expressionSyntax.Type.TypeName.Value);
            var boundParameter = expressionSyntax.Parameter.Select(x => BindExpression(x)).ToList();
            return new BoundArrayCreationExpression(type, expressionSyntax.Dimension, boundParameter, expressionSyntax);
        }

        private BoundExpression BindMemerExpression(
            MemberExpressionSyntax expressionSyntax,
            List<IType> parameterTypes = null)
        {
            
            
            var boundFunction = _contextService.GetAvailableFunctions().SingleOrDefault(
                    x =>
                    x.Name == expressionSyntax.MemberName.Value
                    && Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes));
            if (boundFunction != null)
            {
                return new BoundMemberExpression(expressionSyntax.MemberName.Value, boundFunction, expressionSyntax);
            }

            var boundField = _contextService.GetAvailableFields().SingleOrDefault(x => x.Name == expressionSyntax.MemberName.Value);
            if (boundField != null)
            {
                return new BoundMemberExpression(
                    expressionSyntax.MemberName.Value,
                    boundField,
                    expressionSyntax);
            }

            var boundLocal = _contextService.GetLocal(expressionSyntax.MemberName.Value);
            if (boundLocal != null)
            {
                return new BoundMemberExpression(
                    expressionSyntax.MemberName.Value,
                    boundLocal,
                    expressionSyntax);
            }
            throw new KiwiSemanticException($"{expressionSyntax.MemberName.Value} not defined");
        }

        private static bool Match(List<IType> left, List<IType> right)
        {
            return left.Count == right.Count && left.Select((type, i) => right[i] == type).All(x => x);
        }

        private BoundExpression BindInvocationExpression(InvocationExpressionSyntax expressionSyntax)
        {
            var boundParameter = expressionSyntax.Parameter.Select(x => BindExpression(x)).ToList();
            var boundToInvoke = BindExpression(expressionSyntax.ToInvoke, boundParameter.Select(x => x.Type).ToList());
            var returnType = ((FunctionCompilerGeneratedType)boundToInvoke.Type).ReturnType ?? new VoidCompilerGeneratedType();
            return new BoundInvocationExpression(boundToInvoke, boundParameter, expressionSyntax, returnType);
        }

        private BoundExpression BindBooleanExpression(BooleanExpressionSyntax expressionSyntax)
        {
            return new BoundBooleanExpression(expressionSyntax.Value.Value == "true", expressionSyntax);
        }
    }

    internal enum AssignmentOperators
    {
        None,
        SimpleAssignment,
        DivAssignment,
        MultAssignment,
        AddAssignment,
        PowAssignment,
        SubAssignment
    }
}
