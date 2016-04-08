using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class Binder
    {
        private readonly BasicSymbolService _basicSymbolService = new BasicSymbolService();
        private readonly ScopeManager _scopeManager = new ScopeManager();
        private BoundType _this;

        public BoundCompilationUnit Bind(CompilationUnitSyntax compilationUnitSyntax)
        {
            var basicModel = _basicSymbolService.CreateBasicModel(compilationUnitSyntax);
            basicModel.BoundUsings.AddRange(compilationUnitSyntax.UsingMember.Select(x => BindUsing(x, basicModel.Namespaces)));
            foreach (var boundNamespace in basicModel.Namespaces)
            {
                BindNamespace(boundNamespace, (NamespaceSyntax)boundNamespace.Syntax);
            }
            return basicModel;
        }

        private void BindNamespace(BoundNamespace boundNamespace, NamespaceSyntax syntax)
        {
            _scopeManager.Load(boundNamespace);
            foreach (var boundType in boundNamespace.TypesInternal)
            {
                BindClass(syntax, boundType, (ClassSyntax)boundType.Syntax);
            }
        }

        private BoundType BindClass(NamespaceSyntax classNamespace, BoundType @class, ClassSyntax syntax)
        {
            _this = @class;
            foreach (var boundField in @class.FieldsInternal)
            {
                BindField(boundField, (FieldSyntax)boundField.Syntax);
            }
            foreach (var boundFunction in @class.FunctionsInternal)
            {
                BindFunction(boundFunction, (FunctionSyntax)boundFunction.Syntax);
            }
            @class.ConstructorsInternal.AddRange(syntax.ConstructorMember.Select(BindConstructor).ToList());
            _this = null;
            return @class;
        }

        private BoundFunction BindFunction(BoundFunction boundFunction, FunctionSyntax functionSyntax)
        {
            switch (functionSyntax.SyntaxType)
            {
                case SyntaxType.FunctionSyntax:
                    return BindVoid(boundFunction, functionSyntax);
                case SyntaxType.ExpressionFunctionSyntax:
                    return BindExpressionFunction(boundFunction, (ExpressionFunctionSyntax)functionSyntax);
                case SyntaxType.ReturnFunctionSyntax:
                    return Bind(boundFunction, (ReturnFunctionSyntax)functionSyntax);
            }
            throw new NotImplementedException();
        }

        private BoundFunction Bind(BoundFunction boundFunction, ReturnFunctionSyntax returnFunctionSyntax)
        {
            var boundParameters = returnFunctionSyntax.ParameterList.Select(BindParameter).ToList();
            var type = _scopeManager.LookupType(returnFunctionSyntax.ReturnType.TypeName.Value);
            boundFunction.Statements = returnFunctionSyntax.Statements.Select(BindStatement).ToList();
            boundFunction.Parameter = boundParameters;
            boundFunction.Type = new FunctionType(boundParameters.Select(x=>x.Type).ToList(), type);
            return boundFunction;
        }
        

        private BoundFunction BindExpressionFunction(BoundFunction boundFunction, ExpressionFunctionSyntax expressionFunctionSyntax)
        {
            var boundParameters = expressionFunctionSyntax.ParameterList.Select(BindParameter).ToList();
            var boundReturnStatement = (BoundReturnStatement)expressionFunctionSyntax.Statements.Select(BindStatement).ToList().Single();
            boundFunction.Parameter = boundParameters;
            boundFunction.Statements = new List<BoundStatement> { boundReturnStatement };
            boundFunction.Type = new FunctionType(boundParameters.Select(x => x.Type).ToList(), boundReturnStatement.BoundExpression.Type);
            boundFunction.ReturnType = boundReturnStatement.BoundExpression.Type;
            return boundFunction;
        }

        private BoundFunction BindVoid(BoundFunction function, FunctionSyntax functionSyntax)
        {
            var boundParameters = functionSyntax.ParameterList.Select(BindParameter).ToList();
            function.Statements = functionSyntax.Statements.Select(BindStatement).ToList();
            function.Parameter = boundParameters;
            function.Type = new FunctionType(boundParameters.Select(x => x.Type).ToList(), null);
            return function;
        }

        private BoundParameter BindParameter(ParameterSyntax parameterSyntax)
        {
            var type = _scopeManager.LookupType(parameterSyntax.Type.TypeName.Value);
            var boundParameter = new BoundParameter(parameterSyntax.ParameterName, type, parameterSyntax)
                                 {
                                     Type = type
                                 };
            _scopeManager.AddLocal(boundParameter.ParameterName.Value, boundParameter);
            return boundParameter;
        }

        private BoundConstructor BindConstructor(ConstructorSyntax constructorSyntax)
        {
            var boundParameters = constructorSyntax.ArgList.Select(BindParameter).ToList();
            var boundStatements = constructorSyntax.Statements.Select(BindStatement).ToList();
            return new BoundConstructor(boundParameters, boundStatements, constructorSyntax);
        }

        private BoundStatement BindStatement(IStatementSyntax statementSyntax)
        {
            switch (statementSyntax.SyntaxType)
            {
                case SyntaxType.VariablesDeclarationStatementSyntax:
                    return BindVariablesDeclarationStatement((VariablesDeclarationStatementSyntax)statementSyntax);
                case SyntaxType.VariableDeclarationStatementSyntax:
                    return BindVariableDeclarationStatement((VariableDeclarationStatementSyntax)statementSyntax);
                case SyntaxType.ReturnStatementSyntax:
                    return BindReturnStatement((ReturnStatementSyntax)statementSyntax);
            }
            throw new NotImplementedException();
        }

        private BoundReturnStatement BindReturnStatement(ReturnStatementSyntax statementSyntax)
        {
            var boundExpression = BindExpression(statementSyntax.Expression);
            return new BoundReturnStatement(boundExpression, statementSyntax);
        }

        private BoundStatement BindVariablesDeclarationStatement(VariablesDeclarationStatementSyntax statementSyntax)
        {
            var boundVariableDeclarationStatements = statementSyntax.Declarations.Select(BindVariableDeclarationStatement).ToList();
            return new BoundVariablesDeclarationStatement(boundVariableDeclarationStatements, statementSyntax);
        }

        private BoundVariableDeclarationStatement BindVariableDeclarationStatement(VariableDeclarationStatementSyntax syntax)
        {
            var qualifier = GetQualifier(syntax.VariableQualifier.Type);
            var boundExpression = BindExpression(syntax.InitExpression);

            var boundVariableDeclarationStatement = new BoundVariableDeclarationStatement(syntax.Identifier, qualifier, boundExpression, syntax);
            _scopeManager.AddLocal(syntax.Identifier.Value, boundVariableDeclarationStatement);
            return boundVariableDeclarationStatement;
        }

        private BoundUsing BindUsing(UsingSyntax usingSyntax, List<BoundNamespace> allNamespaces)
        {
            var boundNamespace = allNamespaces.Single(x=>x.NamespaceName.Value == usingSyntax.NamespaceName.Value);
            _scopeManager.Load(boundNamespace);
            var boundUsing = new BoundUsing(usingSyntax.NamespaceName, usingSyntax, boundNamespace);
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
            switch (expressionSyntax.SyntaxType)
            {
                case SyntaxType.BooleanExpressionSyntax:
                    return BindBooleanExpression((BooleanExpressionSyntax)expressionSyntax);
                case SyntaxType.InvocationExpressionSyntax:
                    return BindInvocationExpression((InvocationExpressionSyntax)expressionSyntax);
                case SyntaxType.MemberExpressionSyntax:
                    return BindMemerExpression((MemberExpressionSyntax)expressionSyntax, args);
                case SyntaxType.ObjectCreationExpressionSyntax:
                    return BindObjectCreationExpression((ObjectCreationExpressionSyntax)expressionSyntax);
            }
            throw new NotImplementedException();
        }

        private BoundObjectCreationExpression BindObjectCreationExpression(ObjectCreationExpressionSyntax expressionSyntax)
        {
            var type = (BoundType)_scopeManager.LookupType(expressionSyntax.Type.TypeName.Value);
            var boundParameter = expressionSyntax.Parameter.Select(x => BindExpression(x)).ToList();
            var parameterTypes = boundParameter.Select(x=>x.Type).ToList();
            BoundConstructor boundConstructor = null;
            if (parameterTypes.Any())
            {
                boundConstructor = type.ConstructorsInternal.Single(x => Match(x.Parameters.Select(y=>y.Type).ToList(), parameterTypes));
            }
            return new BoundObjectCreationExpression(type, boundConstructor, boundParameter, expressionSyntax);
        }
        
        private BoundExpression BindMemerExpression(MemberExpressionSyntax expressionSyntax, List<IType> parameterTypes = null)
        {
            var boundFunction =
                _this.FunctionsInternal.SingleOrDefault(
                    x =>
                    x.Name.Value == expressionSyntax.MemberName.Value
                    && Match(x.Parameter.Select(y => y.Type).ToList(), parameterTypes));
            if (boundFunction != null)
            {
                return new BoundMemberExpression(expressionSyntax.MemberName, boundFunction, expressionSyntax);
            }
            return new BoundMemberExpression(
                expressionSyntax.MemberName,
                _this.FieldsInternal.Single(x => x.Name.Value == expressionSyntax.MemberName.Value),
                expressionSyntax);
        }

        private static bool Match(List<IType> left, List<IType> right)
        {
            return left.Count == right.Count && left.Select((type, i) => right[i] == type).All(x => x);
        }

        private BoundExpression BindInvocationExpression(InvocationExpressionSyntax expressionSyntax)
        {
            var boundParameter = expressionSyntax.Parameter.Select(x=>BindExpression(x)).ToList();
            var boundToInvoke = BindExpression(expressionSyntax.ToInvoke, boundParameter.Select(x=>x.Type).ToList());
            var returnType = ((FunctionType)boundToInvoke.Type).ReturnType ?? new StandardType(StandardTypes.Void);
            return new BoundInvocationExpression(boundToInvoke, boundParameter, expressionSyntax, returnType);
        }

        private BoundExpression BindBooleanExpression(BooleanExpressionSyntax expressionSyntax)
        {
            return new BoundBooleanExpression(expressionSyntax);
        }
    }

    internal class BoundVariablesDeclarationStatement : BoundStatement
    {
        public List<BoundVariableDeclarationStatement> BoundVariableDeclarationStatements { get; set; }

        public BoundVariablesDeclarationStatement(List<BoundVariableDeclarationStatement> boundVariableDeclarationStatements, VariablesDeclarationStatementSyntax statementSyntax) : base(statementSyntax)
        {
            BoundVariableDeclarationStatements = boundVariableDeclarationStatements;
        }
    }

    internal enum AssignmentOperators
    {
        None,
        SimpleAssignment
    }
}