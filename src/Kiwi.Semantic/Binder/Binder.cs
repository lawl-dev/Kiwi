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
                basicModel.BoundUsings.AddRange(
                    ((CompilationUnitSyntax)basicModel.Syntax).UsingMember.Select(x => BindUsing(x, allNamespaces)));

                foreach (var boundNamespace in basicModel.Namespaces)
                {
                    BindNamespace(boundNamespace);
                }
            }
            return basicModels;
        }

        private void BindNamespace(BoundNamespace boundNamespace)
        {
            _contextService.Load(boundNamespace);
            foreach (var boundType in boundNamespace.TypesInternal)
            {
                BindClass(boundType, (ClassSyntax)boundType.Syntax);
            }
            _contextService.Unload(boundNamespace);
        }

        private void BindClass(BoundType @class, ClassSyntax syntax)
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
        }

        private void BindFunction(BoundFunction boundFunction, FunctionSyntax functionSyntax)
        {
            functionSyntax.TypeSwitch()
                          .Case<ReturnFunctionSyntax>(x => BindReturnFunction(boundFunction, x))
                          .Case<ExpressionFunctionSyntax>(x => BindExpressionFunction(boundFunction, x))
                          .Case<FunctionSyntax>(x => BindVoid(boundFunction, x))
                          .Default(() => { throw new NotImplementedException(); });
        }

        private void BindReturnFunction(BoundFunction boundFunction, ReturnFunctionSyntax returnFunctionSyntax)
        {
            var boundParameters = returnFunctionSyntax.ParameterList.Select(BindParameter).ToList();
            var type = _contextService.LookupType(returnFunctionSyntax.ReturnType.TypeName.Value);
            boundFunction.Statements = BindScope((ScopeStatementSyntax)returnFunctionSyntax.Statements);
            boundFunction.Parameter = boundParameters;
            boundFunction.Type = new FunctionCompilerGeneratedType(boundParameters.Select(x => x.Type).ToList(), type);
        }

        private BoundScopeStatement BindScope(ScopeStatementSyntax statements)
        {
            _contextService.EnterScope();
            var boundScopeStatement = new BoundScopeStatement(
                statements.Statements.Select(BindStatement).ToList(),
                statements);
            _contextService.ExitScope();
            return boundScopeStatement;
        }

        private BoundSwitchStatement BindSwitchStatement(SwitchStatementSyntax switchStatement)
        {
            var boundCondition = BindExpression(switchStatement.Condition);
            var boundCases = switchStatement.Cases.Select(BindCase).ToList();
            var boundElse = BindElse(switchStatement.Else);
            return new BoundSwitchStatement(boundCondition, boundCases, boundElse, switchStatement);
        }

        private BoundElse BindElse(ElseSyntax @else)
        {
            var boundStatement = BindStatement(@else.Statements);

            return new BoundElse(boundStatement, @else);
        }

        private BoundCase BindCase(CaseSyntax arg)
        {
            var boundExpression = BindExpression(arg.Expression);
            var boundStatement = BindStatement(arg.Statements);
            return new BoundCase(boundExpression, boundStatement, arg);
        }

        private void BindExpressionFunction(BoundFunction boundFunction, ExpressionFunctionSyntax expressionFunctionSyntax)
        {
            var boundParameters = expressionFunctionSyntax.ParameterList.Select(BindParameter).ToList();
            var boundReturnStatement = BindReturnStatement((ReturnStatementSyntax)expressionFunctionSyntax.Statements);
            boundFunction.Parameter = boundParameters;
            boundFunction.Statements = boundReturnStatement;
            boundFunction.Type = new FunctionCompilerGeneratedType(
                boundParameters.Select(x => x.Type).ToList(),
                boundReturnStatement.BoundExpression.Type);
            boundFunction.ReturnType = boundReturnStatement.BoundExpression.Type;
        }

        private void BindVoid(BoundFunction function, FunctionSyntax functionSyntax)
        {
            var boundParameters = functionSyntax.ParameterList.Select(BindParameter).ToList();
            function.Statements = BindScope((ScopeStatementSyntax)functionSyntax.Statements);
            function.Parameter = boundParameters;
            function.Type = new FunctionCompilerGeneratedType(boundParameters.Select(x => x.Type).ToList(), null);
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
                                  .Case<ScopeStatementSyntax>(BindScope)
                                  .Case<SwitchStatementSyntax>(BindSwitchStatement)
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

        private void BindField(BoundField field, FieldSyntax syntax)
        {
            var qualifier = GetQualifier(syntax.FieldTypeQualifier.Type);
            field.Qualifier = qualifier;
            var boundExpression = BindExpression(syntax.FieldInitializer);
            field.Initializer = boundExpression;
            field.Type = boundExpression.Type;
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
                                   .Case<AnonymousFunctionExpressionSyntax>(BindAnonymousFunctionExpression)
                                   .Case<BooleanExpressionSyntax>(BindBooleanExpression)
                                   .Case<IntExpressionSyntax>(BindIntExpression)
                                   .Case<FloatExpressionSyntax>(BindFloatExpression)
                                   .Case<StringExpressionSyntax>(BindStringExpression)
                                   .Case<InvocationExpressionSyntax>(BindInvocationExpression)
                                   .Case<MemberOrTypeExpressionSyntax>(x => BindMemberExpression(x, args))
                                   .Case<ObjectCreationExpressionSyntax>(BindObjectCreationExpression)
                                   .Case<ArrayCreationExpressionSyntax>(BindArrayCreationExpression)
                                   .Case<MemberAccessExpressionSyntax>(x => BindMemberAccessExpression(x, args))
                                   .Case<BinaryExpressionSyntax>(BindBinaryExpression)
                                   .Default(() => { throw new NotImplementedException(); })
                                   .Done();
        }

        private BoundExpression BindAnonymousFunctionExpression(AnonymousFunctionExpressionSyntax arg)
        {
            var boundParameters = arg.Parameter.Select(BindParameter).ToList();
            var boundStatement = BindStatement(arg.Statements);
            //TODO: Visitor to find Return Statements to determind the return type
            throw new NotImplementedException();
        }

        private BoundBinaryExpression BindBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            switch (binaryExpressionSyntax.Operator.Type)
            {
                case TokenType.AndKeyword:
                    return BindAndBinaryExpression(binaryExpressionSyntax);
                case TokenType.NotEqual:
                    return BindNotEqualBinaryExpression(binaryExpressionSyntax);
                case TokenType.Equal:
                    return BindEqualBinaryExpression(binaryExpressionSyntax);
                case TokenType.Mult:
                    return BindMultBinaryExpression(binaryExpressionSyntax);
                case TokenType.Div:
                    return BindDivBinaryExpression(binaryExpressionSyntax);
                case TokenType.Add:
                    return BindAddBinaryExpression(binaryExpressionSyntax);
                case TokenType.Sub:
                    return BindSubBinaryExpression(binaryExpressionSyntax);
                case TokenType.Pow:
                    return BindPowBinaryExpression(binaryExpressionSyntax);
                case TokenType.Less:
                    return BindLessBinaryExpression(binaryExpressionSyntax);
                case TokenType.Greater:
                    return BindGreaterBinaryExpression(binaryExpressionSyntax);
                case TokenType.Or:
                    return BindOrBinaryExpression(binaryExpressionSyntax);
                case TokenType.IsKeyword:
                    return BindIsBinaryExpression(binaryExpressionSyntax);
                case TokenType.TwoDots:
                    return BindRangeBinaryExpression(binaryExpressionSyntax);
                case TokenType.InKeyword:
                case TokenType.NotInKeyword:
                    //TODO: generics and iterator pattern
                    throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        private BoundBinaryExpression BindPowBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is IntCompilerGeneratedType || left.Type is FloatCompilerGeneratedType, $"The operator ^ cannot handle ${left.Type}");
            Ensure(() => right.Type is IntCompilerGeneratedType || right.Type is FloatCompilerGeneratedType, $"The operator ^ cannot handle ${right.Type}");
            Ensure(() => TypeEquals(left.Type, right.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Sub,
                binaryExpressionSyntax,
                left.Type);
        }

        private BoundBinaryExpression BindLessBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is IntCompilerGeneratedType || left.Type is FloatCompilerGeneratedType, $"The operator < cannot handle ${left.Type}");
            Ensure(() => right.Type is IntCompilerGeneratedType || right.Type is FloatCompilerGeneratedType, $"The operator < cannot handle ${right.Type}");
            Ensure(() => TypeEquals(left.Type, right.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Less,
                binaryExpressionSyntax,
                left.Type);
        }

        private BoundBinaryExpression BindGreaterBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is IntCompilerGeneratedType || left.Type is FloatCompilerGeneratedType, $"The operator > cannot handle ${left.Type}");
            Ensure(() => right.Type is IntCompilerGeneratedType || right.Type is FloatCompilerGeneratedType, $"The operator > cannot handle ${right.Type}");
            Ensure(() => TypeEquals(left.Type, right.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Greater,
                binaryExpressionSyntax,
                left.Type);
        }

        private BoundBinaryExpression BindOrBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is BoolCompilerGeneratedType, $"The operator || cannot handle ${left.Type}");
            Ensure(() => right.Type is BoolCompilerGeneratedType, $"The operator || cannot handle ${right.Type}");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Or,
                binaryExpressionSyntax,
                left.Type);
        }

        private BoundBinaryExpression BindIsBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            Ensure(() => binaryExpressionSyntax.RightExpression is MemberOrTypeExpressionSyntax, $"Expected {binaryExpressionSyntax.RightExpression} to be {nameof(MemberOrTypeExpressionSyntax)}");

            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindTypeExpression((MemberOrTypeExpressionSyntax)binaryExpressionSyntax.RightExpression);
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Is,
                binaryExpressionSyntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindRangeBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is IntCompilerGeneratedType, $"The operator .. cannot handle ${left.Type}");
            Ensure(() => right.Type is IntCompilerGeneratedType, $"The operator .. cannot handle ${right.Type}");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Range,
                binaryExpressionSyntax,
                new ArrayCompilerGeneratedType(left.Type, 1));
        }

        private BoundBinaryExpression BindSubBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is IntCompilerGeneratedType || left.Type is FloatCompilerGeneratedType, $"The operator - cannot handle ${left.Type}");
            Ensure(() => right.Type is IntCompilerGeneratedType || right.Type is FloatCompilerGeneratedType, $"The operator - cannot handle ${right.Type}");
            Ensure(() => TypeEquals(left.Type, right.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Sub,
                binaryExpressionSyntax,
                left.Type);
        }

        private BoundBinaryExpression BindAddBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is IntCompilerGeneratedType || left.Type is FloatCompilerGeneratedType, $"The operator + cannot handle ${left.Type}");
            Ensure(() => right.Type is IntCompilerGeneratedType || right.Type is FloatCompilerGeneratedType, $"The operator + cannot handle ${right.Type}");
            Ensure(() => TypeEquals(left.Type, right.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Add,
                binaryExpressionSyntax,
                left.Type);
        }

        private BoundBinaryExpression BindDivBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is IntCompilerGeneratedType || left.Type is FloatCompilerGeneratedType, $"The operator / cannot handle ${left.Type}");
            Ensure(() => right.Type is IntCompilerGeneratedType || right.Type is FloatCompilerGeneratedType, $"The operator / cannot handle ${right.Type}");
            Ensure(() => TypeEquals(left.Type, right.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Div,
                binaryExpressionSyntax,
                left.Type);
        }

        private BoundBinaryExpression BindMultBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is IntCompilerGeneratedType || left.Type is FloatCompilerGeneratedType, $"The operator * cannot handle ${left.Type}");
            Ensure(() => right.Type is IntCompilerGeneratedType || right.Type is FloatCompilerGeneratedType, $"The operator * cannot handle ${right.Type}");
            Ensure(() => TypeEquals(left.Type, right.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Mult,
                binaryExpressionSyntax,
                left.Type);
        }

        private BoundBinaryExpression BindEqualBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => TypeEquals(left.Type, right.Type), $"The operator = cannt handle the operands of type {left.Type} and {right.Type}.");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.Equal,
                binaryExpressionSyntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindNotEqualBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => TypeEquals(left.Type, right.Type), $"The operator != cannt handle the operands of type {left.Type} and {right.Type}.");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.NotEqual,
                binaryExpressionSyntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindAndBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            var left = BindExpression(binaryExpressionSyntax.LeftExpression);
            var right = BindExpression(binaryExpressionSyntax.RightExpression);

            Ensure(() => left.Type is BoolCompilerGeneratedType, $"The operator && cannot handle ${left.Type}");
            Ensure(() => right.Type is BoolCompilerGeneratedType, $"The operator && cannot handle ${right.Type}");
            return new BoundBinaryExpression(
                left,
                right,
                BinaryOperators.And,
                binaryExpressionSyntax,
                new BoolCompilerGeneratedType());
        }

        private BoundMemberAccessExpression BindMemberAccessExpression(
            MemberAccessExpressionSyntax expressionSyntax,
            List<IType> parameterTypes)
        {
            var boundExpression = BindExpression(expressionSyntax.Owner);

            var function = boundExpression.Type.Functions.SingleOrDefault(x => x.Name == expressionSyntax.MemberName.Value);
            if ((parameterTypes == null || !parameterTypes.Any()) && function != null)
            {
                return new BoundMemberAccessExpression(
                    expressionSyntax.MemberName.Value,
                    function,
                    expressionSyntax);
            }

            var boundFunction =
                boundExpression.Type.Functions.SingleOrDefault(
                    x =>
                    x.Name == expressionSyntax.MemberName.Value
                    && Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes ?? new List<IType>()));

            if (boundFunction != null)
            {
                return new BoundMemberAccessExpression(
                    expressionSyntax.MemberName.Value,
                    boundFunction,
                    expressionSyntax);
            }

            var boundField =
                boundExpression.Type.Fields.SingleOrDefault(x => x.Name == expressionSyntax.MemberName.Value);
            if (boundField != null)
            {
                return new BoundMemberAccessExpression(
                    expressionSyntax.MemberName.Value,
                    boundField,
                    expressionSyntax);
            }
            throw new KiwiSemanticException($"{expressionSyntax.MemberName.Value} not defined");
        }

        private static BoundExpression BindStringExpression(StringExpressionSyntax expressionSyntax)
        {
            return new BoundStringExpression(expressionSyntax.Value.Value, expressionSyntax);
        }

        private static BoundExpression BindIntExpression(IntExpressionSyntax expressionSyntax)
        {
            var value = int.Parse(expressionSyntax.Value.Value);

            return new BoundIntExpression(value, expressionSyntax);
        }

        private static BoundExpression BindFloatExpression(FloatExpressionSyntax expressionSyntax)
        {
            var value = float.Parse(expressionSyntax.Value.Value);

            return new BoundFloatExpression(value, expressionSyntax);
        }

        private BoundObjectCreationExpression BindObjectCreationExpression(
            ObjectCreationExpressionSyntax expressionSyntax)
        {
            var type = _contextService.LookupType(expressionSyntax.Type.TypeName.Value);
            var boundParameter = expressionSyntax.Parameter.Select(x => BindExpression(x)).ToList();
            var parameterTypes = boundParameter.Select(x => x.Type).ToList();
            IConstructor boundConstructor = null;
            if (parameterTypes.Any())
            {
                boundConstructor =
                    type.Constructors.Single(
                        x => Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes));
            }
            return new BoundObjectCreationExpression(type, boundConstructor, boundParameter, expressionSyntax);
        }

        private BoundArrayCreationExpression BindArrayCreationExpression(ArrayCreationExpressionSyntax expressionSyntax)
        {
            var type = _contextService.LookupType(expressionSyntax.Type.TypeName.Value);
            var boundParameter = expressionSyntax.Parameter.Select(x => BindExpression(x)).ToList();
            return new BoundArrayCreationExpression(type, expressionSyntax.Dimension, boundParameter, expressionSyntax);
        }

        private BoundTypeExpression BindTypeExpression(MemberOrTypeExpressionSyntax memberOrTypeExpressionSyntax)
        {
            var type = _contextService.LookupType(memberOrTypeExpressionSyntax.Name.Value);
            return new BoundTypeExpression(type, memberOrTypeExpressionSyntax);
        }

        private BoundExpression BindMemberExpression(
            MemberOrTypeExpressionSyntax orTypeExpressionSyntax,
            List<IType> parameterTypes = null)
        {
            var boundFunction = _contextService.GetAvailableFunctions().SingleOrDefault(
                x =>
                x.Name == orTypeExpressionSyntax.Name.Value
                && Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes));
            if (boundFunction != null)
            {
                return new BoundMemberExpression(orTypeExpressionSyntax.Name.Value, boundFunction, orTypeExpressionSyntax);
            }

            var boundField =
                _contextService.GetAvailableFields().SingleOrDefault(x => x.Name == orTypeExpressionSyntax.Name.Value);
            if (boundField != null)
            {
                return new BoundMemberExpression(
                    orTypeExpressionSyntax.Name.Value,
                    boundField,
                    orTypeExpressionSyntax);
            }

            var boundLocal = _contextService.GetLocal(orTypeExpressionSyntax.Name.Value);
            if (boundLocal != null)
            {
                return new BoundMemberExpression(
                    orTypeExpressionSyntax.Name.Value,
                    boundLocal,
                    orTypeExpressionSyntax);
            }
            throw new KiwiSemanticException($"{orTypeExpressionSyntax.Name.Value} not defined");
        }

        private static bool Match(List<IType> left, List<IType> right)
        {
            return left.Count == right.Count && left.Select((type, i) => right[i] == type).All(x => x);
        }

        private BoundExpression BindInvocationExpression(InvocationExpressionSyntax expressionSyntax)
        {
            var boundParameter = expressionSyntax.Parameter.Select(x => BindExpression(x)).ToList();
            var boundToInvoke = BindExpression(expressionSyntax.ToInvoke, boundParameter.Select(x => x.Type).ToList());
            var returnType = ((FunctionCompilerGeneratedType)boundToInvoke.Type).ReturnType
                             ?? new VoidCompilerGeneratedType();
            return new BoundInvocationExpression(boundToInvoke, boundParameter, expressionSyntax, returnType);
        }

        private BoundExpression BindBooleanExpression(BooleanExpressionSyntax expressionSyntax)
        {
            return new BoundBooleanExpression(expressionSyntax.Value.Value == "true", expressionSyntax);
        }

        private static void Ensure(Func<bool> func, string message)
        {
            if (!func())
            {
                throw new KiwiSemanticException(message);
            }
        }

        private static bool TypeEquals(IType t1, IType t2)
        {
            if (t1 is CompilerGeneratedTypeBase)
            {
                return t1.GetType() == t2.GetType();
            }
            return t1 == t2;
        }
    }

    public enum BinaryOperators
    {
        None,
        Equal,
        Mult,
        Div,
        Add,
        Sub,
        Less,
        Greater,
        Or,
        Is,
        Range,
        NotEqual,
        And
    }
}