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
        private bool _bindSignatures = true;
        
        public List<BoundCompilationUnit> Bind(List<CompilationUnitSyntax> compilationUnits)
        {
            var basicModels = _basicSymbolService.CreateBasicModel(compilationUnits);
            var boundNamespaces = basicModels.SelectMany(x => x.Namespaces).ToList();
            
            foreach (var basicModel in basicModels)
            {
                basicModel.Usings.AddRange(
                    ((CompilationUnitSyntax)basicModel.Syntax).Usings.Select(x => BindUsing(x, boundNamespaces)));

                foreach (var boundNamespace in basicModel.Namespaces)
                {
                    BindNamespace(boundNamespace);
                }
            }

            _bindSignatures = false; //TODO: Hack

            foreach (var basicModel in basicModels)
            {
                basicModel.Usings.AddRange(
                    ((CompilationUnitSyntax)basicModel.Syntax).Usings.Select(x => BindUsing(x, boundNamespaces)));

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
            @class.ConstructorsInternal.AddRange(syntax.Constructors.Select(BindConstructor).ToList());
            _contextService.ExitClass();
        }

        private void BindFunction(BoundFunction boundFunction, FunctionSyntax syntax)
        {
            _contextService.EnterScope();
            var boundParameters = syntax.Parameter.Select(BindParameter).ToList();
            boundFunction.Parameter = boundParameters;
            
            if (syntax.ReturnType != null)
            {
                boundFunction.ReturnType = BindType(syntax.ReturnType);
            }
            else if(syntax.Statements is ReturnStatementSyntax)
            {
                boundFunction.ReturnType = BindExpression(((ReturnStatementSyntax)syntax.Statements).Expression).Type;
            }
            boundFunction.Type = new FunctionCompilerGeneratedType(boundParameters.Select(x => x.Type).ToList(), boundFunction.ReturnType);
            boundFunction.IsInfixFunction = syntax is InfixFunctionSyntax;
            if (boundFunction.IsInfixFunction)
            {
                var parameter = string.Join(",", boundFunction.Parameter.Select(x=>x.Type));
                Ensure(
                    () => boundParameters.Count == 2,
                    $"infix' modifier is inapplicable on {boundFunction.Name}({parameter}) -> {boundFunction.ReturnType}. Parameter count != 2");
            }

            if (!_bindSignatures)
            {
                boundFunction.Statements = BindStatement(syntax.Statements);
            }
            _contextService.ExitScope();
        }
        
        private BoundScopeStatement BindScope(ScopeStatementSyntax syntax)
        {
            _contextService.EnterScope();
            var boundScopeStatement = new BoundScopeStatement(
                syntax.Statements.Select(BindStatement).ToList(),
                syntax);
            _contextService.ExitScope();
            return boundScopeStatement;
        }

        private BoundSwitchStatement BindSwitchStatement(SwitchStatementSyntax syntax)
        {
            var boundCondition = BindExpression(syntax.Condition);
            var boundCases = syntax.Cases.Select(BindCase).ToList();
            foreach (var boundCase in boundCases)
            {
                Ensure(() => TypeEquals(boundCase.BoundExpression.Type, boundCondition.Type), "Switch cases condition type must match switch condition type");
            }

            var boundElse = BindElse(syntax.Else);
            return new BoundSwitchStatement(boundCondition, boundCases, boundElse, syntax);
        }

        private BoundElse BindElse(ElseSyntax syntax)
        {
            var boundStatement = BindStatement(syntax.Statements);

            return new BoundElse(boundStatement, syntax);
        }

        private BoundCase BindCase(CaseSyntax syntax)
        {
            var boundExpression = BindExpression(syntax.Expression);
            var boundStatement = BindStatement(syntax.Statements);
            return new BoundCase(boundExpression, boundStatement, syntax);
        }

        private BoundParameter BindParameter(ParameterSyntax syntax)
        {
            var boundType = syntax.Type.TypeSwitchExpression<TypeSyntax, IType>()
                                           .Case<ArrayTypeSyntax>(BindArrayType)
                                           .Case<TypeSyntax>(BindType)
                                           .Default(() => { throw new NotImplementedException(); })
                                           .Done();

            var boundParameter = new BoundParameter(syntax.Name.Value, boundType, syntax);
            _contextService.AddLocal(boundParameter.Name, boundParameter);
            return boundParameter;
        }

        private IType BindType(TypeSyntax syntax)
        {
            return _contextService.LookupType(syntax.TypeName.Value);
        }

        private IType BindArrayType(ArrayTypeSyntax syntax)
        {
            var boundType = _contextService.LookupType(syntax.TypeName.Value);
            return new ArrayCompilerGeneratedType(boundType, syntax.Dimension);
        }

        private BoundConstructor BindConstructor(ConstructorSyntax syntax)
        {
            _contextService.EnterScope();
            var boundParameters = syntax.Parameter.Select(BindParameter).ToList();
            var boundStatements = BindScope(syntax.Statements);
            _contextService.ExitScope();
            return new BoundConstructor(boundParameters, boundStatements, syntax);
        }

        private BoundStatement BindStatement(IStatementSyntax syntax)
        {
            return syntax.TypeSwitchExpression<IStatementSyntax, BoundStatement>()
                                  .Case<VariablesDeclarationStatementSyntax>(BindVariablesDeclarationStatement)
                                  .Case<VariableDeclarationStatementSyntax>(BindVariableDeclarationStatement)
                                  .Case<ReturnStatementSyntax>(BindReturnStatement)
                                  .Case<IfElseStatementSyntax>(BindIfElseStatement)
                                  .Case<InvocationStatementSyntax>(BindInvocationStatement)
                                  .Case<IfStatementSyntax>(BindIfStatement)
                                  .Case<AssignmentStatementSyntax>(BindAssignmentStatement)
                                  .Case<ForStatementSyntax>(BindForStatement)
                                  .Case<ForInStatementSyntax>(BindForInStatement)
                                  .Case<ScopeStatementSyntax>(BindScope)
                                  .Case<SwitchStatementSyntax>(BindSwitchStatement)
                                  .Default(() => { throw new NotImplementedException(); })
                                  .Done();
        }

        private BoundInvocationStatement BindInvocationStatement(InvocationStatementSyntax syntax)
        {
            var boundInvocationExpression = BindInvocationExpression(syntax.InvocationExpression);
            return new BoundInvocationStatement(boundInvocationExpression, syntax);
        }

        private BoundStatement BindForInStatement(ForInStatementSyntax syntax)
        {
            throw new NotImplementedException();
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var boundInitStatement = BindStatement(syntax.InitStatement);
            var boundCondition = BindExpression(syntax.CondExpression);
            Ensure(() => TypeEquals(boundCondition.Type, new BoolCompilerGeneratedType()), "For condition must be of Type Bool");

            var boundLoopStatement = BindStatement(syntax.LoopStatement);
            var boundStatements = BindScope(syntax.Statements);
            return new BoundForStatement(boundInitStatement, boundCondition, boundLoopStatement, boundStatements, syntax);
        }

        private BoundAssignStatement BindAssignmentStatement(AssignmentStatementSyntax syntax)
        {
            var boundExpression = BindExpression(syntax.Member);
            Ensure(() => boundExpression is BoundMemberExpression ||
                         boundExpression is BoundMemberAccessExpression ||
                         boundExpression is BoundArrayAccessExpression, $"You cannot assign {nameof(boundExpression.GetType)} a value.");

            var boundToAssignExpression = BindExpression(syntax.ToAssign);
            
            AssignmentOperators assignOperator;
            switch (syntax.Operator.Type)
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
                    throw new KiwiSemanticException($"Unknown Assignoperator {syntax.Operator.Value}");
            }
            return new BoundAssignStatement(boundExpression, boundToAssignExpression, assignOperator, syntax);
        }

        private BoundStatement BindIfElseStatement(IfElseStatementSyntax syntax)
        {
            var boundExpression = BindExpression(syntax.Condition);
            Ensure(() => TypeEquals(boundExpression.Type, new BoolCompilerGeneratedType()), "If condition must be of Type Bool");


            var boundStatements = BindScope(syntax.Statements);
            var boundElseStatement = BindScope(syntax.ElseStatements);
            return new BoundIfElseStatement(boundExpression, boundStatements, boundElseStatement, syntax);
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var boundExpression = BindExpression(syntax.Condition);
            Ensure(() => TypeEquals(boundExpression.Type, new BoolCompilerGeneratedType()), "If condition must be of Type Bool");

            var boundStatements = BindScope(syntax.Statements);
            return new BoundIfStatement(boundExpression, boundStatements, syntax);
        }

        private BoundReturnStatement BindReturnStatement(ReturnStatementSyntax syntax)
        {
            var boundExpression = BindExpression(syntax.Expression);
            return new BoundReturnStatement(boundExpression, syntax);
        }

        private BoundStatement BindVariablesDeclarationStatement(VariablesDeclarationStatementSyntax syntax)
        {
            var boundVarDeclStatement = syntax.Declarations.Select(BindVariableDeclarationStatement).ToList();
            return new BoundVariablesDeclarationStatement(boundVarDeclStatement, syntax);
        }

        private BoundVariableDeclarationStatement BindVariableDeclarationStatement(VariableDeclarationStatementSyntax syntax)
        {
            var qualifier = GetQualifier(syntax.Qualifier.Type);
            var boundExpression = BindExpression(syntax.InitExpression);

            var boundVarDeclStatement = new BoundVariableDeclarationStatement(
                syntax.Identifier.Value,
                qualifier,
                boundExpression,
                syntax);
            _contextService.AddLocal(syntax.Identifier.Value, boundVarDeclStatement);
            return boundVarDeclStatement;
        }

        private BoundUsing BindUsing(UsingSyntax syntax, List<BoundNamespace> availableNamespaces)
        {
            var boundNamespace = availableNamespaces.Single(x => x.Name == syntax.NamespaceName.Value);
            _contextService.Load(boundNamespace);
            var boundUsing = new BoundUsing(syntax.NamespaceName.Value, syntax, boundNamespace);
            return boundUsing;
        }

        private void BindField(BoundField field, FieldSyntax syntax)
        {
            var qualifier = GetQualifier(syntax.Qualifier.Type);
            field.Qualifier = qualifier;
            var boundExpression = BindExpression(syntax.Initializer);
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
                case TokenType.ImmutKeyword:
                    qualifier = VariableQualifier.Immutable;
                    break;
                default:
                    throw new KiwiSemanticException($"Forbidden Variable Qualifier {tokenType}");
            }
            return qualifier;
        }

        private BoundExpression BindExpression(IExpressionSyntax syntax, List<IType> args = null)
        {
            return syntax.TypeSwitchExpression<IExpressionSyntax, BoundExpression>()
                                   .Case<AnonymousFunctionExpressionSyntax>(BindAnonymousFunctionExpression)
                                   .Case<BooleanExpressionSyntax>(BindBooleanExpression)
                                   .Case<IntExpressionSyntax>(BindIntExpression)
                                   .Case<FloatExpressionSyntax>(BindFloatExpression)
                                   .Case<StringExpressionSyntax>(BindStringExpression)
                                   .Case<InvocationExpressionSyntax>(BindInvocationExpression)
                                   .Case<InfixFunctionInvocationExpressionSyntax>(BindInfixFunctionInvocationExpression)
                                   .Case<IdentifierExpressionSyntax>(x => BindMemberExpression(x, args))
                                   .Case<ArrayCreationExpressionSyntax>(BindArrayCreationExpression)
                                   .Case<ObjectCreationExpressionSyntax>(BindObjectCreationExpression)
                                   .Case<MemberAccessExpressionSyntax>(x => BindMemberAccessExpression(x, args))
                                   .Case<ArrayAccessExpressionSyntax>(BindArrayAccessExpression)
                                   .Case<BinaryExpressionSyntax>(BindBinaryExpression)
                                   .Case<IfElseExpressionSyntax>(BindIfElseExpression)
                                   .Default(() => { throw new NotImplementedException(); })
                                   .Done();
        }

        private BoundArrayAccessExpression BindArrayAccessExpression(ArrayAccessExpressionSyntax syntax)
        {
            var boundExpression = BindExpression(syntax.Owner);
            Ensure(() => boundExpression.Type is ArrayCompilerGeneratedType, $"{boundExpression.Type} is no array");

            var arrayType = (ArrayCompilerGeneratedType)boundExpression.Type;

            var elementType = arrayType.Type;
            var boundParameter = syntax.Parameter.Select(x => BindExpression(x)).ToList();

            Ensure(() => boundParameter.Count == arrayType.Dimension, $"Parameter count ({boundParameter.Count}) must match array dimension count ({arrayType.Dimension})");
            return new BoundArrayAccessExpression(boundParameter, syntax, elementType );
        }

        private BoundIfElseExpression BindIfElseExpression(IfElseExpressionSyntax syntax)
        {
            var boundCondition = BindExpression(syntax.Condition);
            var boundIfTrue = BindExpression(syntax.IfTrueExpression);
            var boundIfFalse = BindExpression(syntax.IfFalseExpression);

            Ensure(() => TypeEquals(boundCondition.Type, new BoolCompilerGeneratedType()), "If condition must be of Type Bool");
            Ensure(() => TypeEquals(boundIfTrue.Type, boundIfFalse.Type), "IfTrue and IfFalse expression Type must match");

            var boundExpressionType = boundIfTrue.Type; //TODO: determind lowest type 
            return new BoundIfElseExpression(boundCondition, boundIfTrue, boundIfFalse, boundExpressionType, syntax);
        }

        private BoundExpression BindAnonymousFunctionExpression(AnonymousFunctionExpressionSyntax syntax)
        {
            var boundParameters = syntax.Parameter.Select(BindParameter).ToList();
            var boundStatement = BindStatement(syntax.Statements);
            throw new NotImplementedException();
        }

        private BoundBinaryExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            switch (syntax.Operator.Type)
            {
                case TokenType.AndKeyword:
                    return BindAndBinaryExpression(syntax);
                case TokenType.NotEqual:
                    return BindNotEqualBinaryExpression(syntax);
                case TokenType.Equal:
                    return BindEqualBinaryExpression(syntax);
                case TokenType.Mult:
                    return BindMultBinaryExpression(syntax);
                case TokenType.Div:
                    return BindDivBinaryExpression(syntax);
                case TokenType.Add:
                    return BindAddBinaryExpression(syntax);
                case TokenType.Sub:
                    return BindSubBinaryExpression(syntax);
                case TokenType.Pow:
                    return BindPowBinaryExpression(syntax);
                case TokenType.Less:
                    return BindLessBinaryExpression(syntax);
                case TokenType.Greater:
                    return BindGreaterBinaryExpression(syntax);
                case TokenType.Or:
                    return BindOrBinaryExpression(syntax);
                case TokenType.IsKeyword:
                    return BindIsBinaryExpression(syntax);
                case TokenType.TwoDots:
                    return BindRangeBinaryExpression(syntax);
                case TokenType.InKeyword:
                case TokenType.NotInKeyword:
                    //TODO: generics and iterator pattern
                    throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        private BoundBinaryExpression BindPowBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is IntCompilerGeneratedType || boundLeft.Type is FloatCompilerGeneratedType, $"The operator ^ cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is IntCompilerGeneratedType || boundRight.Type is FloatCompilerGeneratedType, $"The operator ^ cannot handle ${boundRight.Type}");
            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Sub,
                syntax,
                boundLeft.Type);
        }

        private BoundBinaryExpression BindLessBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is IntCompilerGeneratedType || boundLeft.Type is FloatCompilerGeneratedType, $"The operator < cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is IntCompilerGeneratedType || boundRight.Type is FloatCompilerGeneratedType, $"The operator < cannot handle ${boundRight.Type}");
            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Less,
                syntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindGreaterBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is IntCompilerGeneratedType || boundLeft.Type is FloatCompilerGeneratedType, $"The operator > cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is IntCompilerGeneratedType || boundRight.Type is FloatCompilerGeneratedType, $"The operator > cannot handle ${boundRight.Type}");
            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Greater,
                syntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindOrBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is BoolCompilerGeneratedType, $"The operator || cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is BoolCompilerGeneratedType, $"The operator || cannot handle ${boundRight.Type}");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Or,
                syntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindIsBinaryExpression(BinaryExpressionSyntax syntax)
        {
            Ensure(() => syntax.RightExpression is IdentifierExpressionSyntax, $"Expected {syntax.RightExpression} to be {nameof(IdentifierExpressionSyntax)}");

            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindTypeExpression((IdentifierExpressionSyntax)syntax.RightExpression);
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Is,
                syntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindRangeBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is IntCompilerGeneratedType, $"The operator .. cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is IntCompilerGeneratedType, $"The operator .. cannot handle ${boundRight.Type}");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Range,
                syntax,
                new ArrayCompilerGeneratedType(boundLeft.Type, 1));
        }

        private BoundBinaryExpression BindSubBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is IntCompilerGeneratedType || boundLeft.Type is FloatCompilerGeneratedType, $"The operator - cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is IntCompilerGeneratedType || boundRight.Type is FloatCompilerGeneratedType, $"The operator - cannot handle ${boundRight.Type}");
            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Sub,
                syntax,
                boundLeft.Type);
        }

        private BoundBinaryExpression BindAddBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is IntCompilerGeneratedType || boundLeft.Type is FloatCompilerGeneratedType, $"The operator + cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is IntCompilerGeneratedType || boundRight.Type is FloatCompilerGeneratedType, $"The operator + cannot handle ${boundRight.Type}");
            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Add,
                syntax,
                boundLeft.Type);
        }

        private BoundBinaryExpression BindDivBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is IntCompilerGeneratedType || boundLeft.Type is FloatCompilerGeneratedType, $"The operator / cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is IntCompilerGeneratedType || boundRight.Type is FloatCompilerGeneratedType, $"The operator / cannot handle ${boundRight.Type}");
            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Div,
                syntax,
                boundLeft.Type);
        }

        private BoundBinaryExpression BindMultBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is IntCompilerGeneratedType || boundLeft.Type is FloatCompilerGeneratedType, $"The operator * cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is IntCompilerGeneratedType || boundRight.Type is FloatCompilerGeneratedType, $"The operator * cannot handle ${boundRight.Type}");
            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), "Please ensure the operand types equals");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Mult,
                syntax,
                boundLeft.Type);
        }

        private BoundBinaryExpression BindEqualBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), $"The operator = cannt handle the operands of type {boundLeft.Type} and {boundRight.Type}.");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.Equal,
                syntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindNotEqualBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => TypeEquals(boundLeft.Type, boundRight.Type), $"The operator != cannt handle the operands of type {boundLeft.Type} and {boundRight.Type}.");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.NotEqual,
                syntax,
                new BoolCompilerGeneratedType());
        }

        private BoundBinaryExpression BindAndBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.LeftExpression);
            var boundRight = BindExpression(syntax.RightExpression);

            Ensure(() => boundLeft.Type is BoolCompilerGeneratedType, $"The operator && cannot handle ${boundLeft.Type}");
            Ensure(() => boundRight.Type is BoolCompilerGeneratedType, $"The operator && cannot handle ${boundRight.Type}");
            return new BoundBinaryExpression(
                boundLeft,
                boundRight,
                BinaryOperators.And,
                syntax,
                new BoolCompilerGeneratedType());
        }

        private BoundMemberAccessExpression BindMemberAccessExpression(MemberAccessExpressionSyntax syntax, List<IType> parameterTypes)
        {
            var boundExpression = BindExpression(syntax.Owner);
            var memberName = syntax.Name.Value;

            return Select<BoundMemberAccessExpression>
                .Case(() => (IBoundMember)boundExpression.Type.Functions.SingleOrDefault(x => x.Name == memberName))
                .Match(func => func != null && parameterTypes == null || !parameterTypes.Any())
                .Do(func => new BoundMemberAccessExpression(memberName, func, syntax))

                .Case(() => boundExpression.Type.Functions.SingleOrDefault(x => x.Name == memberName && Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes ?? new List<IType>())))
                .Match(func => func != null)
                .Do(func => new BoundMemberAccessExpression(
                                memberName,
                                func,
                                syntax))

                .Case(() => boundExpression.Type.Fields.SingleOrDefault(x => x.Name == memberName))
                .Match(field => field != null)
                .Do(field => new BoundMemberAccessExpression(
                                 memberName,
                                 field,
                                 syntax))

                .Else(() => { throw new KiwiSemanticException($"{memberName} not defined"); })
                .Done();
        }

        private static BoundExpression BindStringExpression(StringExpressionSyntax syntax)
        {
            return new BoundStringExpression(syntax.Value.Value, syntax);
        }

        private static BoundExpression BindIntExpression(IntExpressionSyntax syntax)
        {
            var value = int.Parse(syntax.Value.Value);

            return new BoundIntExpression(value, syntax);
        }

        private static BoundExpression BindFloatExpression(FloatExpressionSyntax syntax)
        {
            var value = float.Parse(syntax.Value.Value);

            return new BoundFloatExpression(value, syntax);
        }

        private BoundObjectCreationExpression BindObjectCreationExpression(ObjectCreationExpressionSyntax syntax)
        {
            var boundType = BindType(syntax.Type); 
            var boundParameter = syntax.Parameter.Select(x => BindExpression(x)).ToList();
            var parameterTypes = boundParameter.Select(x => x.Type).ToList();
            IConstructor boundConstructor = null;
            
            if (parameterTypes.Any())
            {
                string parameterTypeNames = string.Empty; //TODO: resolve names, maybe compilergeneratedTypes need name
                Ensure(() => boundType.Constructors.Any(x => Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes)), $"Cannot resolve constructor({parameterTypeNames}).");
                boundConstructor = boundType.Constructors.Single(x => Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes));
            }
            else
            {
                Ensure(() => !boundType.Constructors.Any(), $"{boundType} has no constructor without arguments.");
            }
            return new BoundObjectCreationExpression(boundType, boundConstructor, boundParameter, syntax);
        }

        private BoundArrayCreationExpression BindArrayCreationExpression(ArrayCreationExpressionSyntax syntax)
        {
            var boundType = BindType(syntax.Type);
            var boundParameter = syntax.Parameter.Select(x => BindExpression(x)).ToList();
            return new BoundArrayCreationExpression(boundType, syntax.Dimension, boundParameter, syntax);
        }

        private BoundTypeExpression BindTypeExpression(IdentifierExpressionSyntax syntax)
        {
            var boundType = _contextService.LookupType(syntax.Name.Value);
            return new BoundTypeExpression(boundType, syntax);
        }

        private BoundMemberExpression BindMemberExpression(IdentifierExpressionSyntax syntax, List<IType> parameterTypes = null)
        {
            var memberName = syntax.Name.Value;

            return Select<BoundMemberExpression>
                .Case(() => (IBoundMember)GetFunction(memberName, parameterTypes))
                    .Match(function => function != null)
                    .Do(function => new BoundMemberExpression(memberName, function, syntax))

                .Case(() => GetField(memberName))
                    .Match(field => field != null)
                    .Do(field => new BoundMemberExpression(memberName, field, syntax))

                .Case(() => _contextService.GetLocal(memberName))
                    .Match(local => local != null)
                    .Do(local => new BoundMemberExpression(memberName, local, syntax))

                .Else(() => { throw new KiwiSemanticException($"{memberName} not defined"); }).Done();
        }

        private IField GetField(string memberName)
        {
            return _contextService.GetAvailableFields().SingleOrDefault(x => x.Name == memberName);
        }

        private IFunction GetFunction(string memberName, List<IType> parameterTypes)
        {
            return _contextService.GetAvailableFunctions().SingleOrDefault(x => x.Name == memberName && Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes));
        }

        private static bool Match(List<IType> left, List<IType> right)
        {
            return left.Count == right?.Count && left.Select((type, i) => TypeEquals(right[i], type)).All(x => x);
        }

        private BoundInvocationExpression BindInvocationExpression(InvocationExpressionSyntax syntax)
        {
            var boundParameter = syntax.Parameter.Select(x => BindExpression(x)).ToList();
            var boundToInvoke = BindExpression(syntax.ToInvoke, boundParameter.Select(x => x.Type).ToList());
            var boundReturnType = ((FunctionCompilerGeneratedType)boundToInvoke.Type).ReturnType ?? new VoidCompilerGeneratedType();
            return new BoundInvocationExpression(boundToInvoke, boundParameter, syntax, boundReturnType);
        }

        private BoundInvocationExpression BindInfixFunctionInvocationExpression(InfixFunctionInvocationExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundInfixFunction = BindMemberExpression(syntax.Identifier, new List<IType>() {boundLeft.Type, boundRight.Type});
            var returnType = ((FunctionCompilerGeneratedType)boundInfixFunction.Type).ReturnType;

            Ensure(
                () => (boundInfixFunction.BoundMember as BoundFunction)?.IsInfixFunction == true,
                $"No infix function 'infix function {syntax.Identifier.Name.Value}({boundLeft.Type}, {boundRight.Type})' found");
            Ensure(() => returnType != null, $"Missing return Type: 'infix function {syntax.Identifier.Name.Value}({boundLeft.Type}, {boundRight.Type})'");

            
            return new BoundInvocationExpression(boundInfixFunction, new List<BoundExpression>() {boundLeft, boundRight}, syntax, returnType);
        }

        private BoundExpression BindBooleanExpression(BooleanExpressionSyntax syntax)
        {
            return new BoundBooleanExpression(syntax.Value.Value == "true", syntax);
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