﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Common;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class Parser : ParserBase
    {
        public Parser(List<Token> token) : base(new TransactableTokenStream(RemoveUnnecessaryToken(token)))
        {
        }

        private static TokenType[] BinaryOperators => new[]
                                                      {
                                                          TokenType.Equal,
                                                          TokenType.Mult,
                                                          TokenType.Div,
                                                          TokenType.Add,
                                                          TokenType.Sub,
                                                          TokenType.Pow,
                                                          TokenType.Less,
                                                          TokenType.Greater,
                                                          TokenType.Or,
                                                          TokenType.TwoDots,
                                                          TokenType.InKeyword,
                                                          TokenType.NotInKeyword,
                                                          TokenType.IsKeyword,
                                                          TokenType.AndKeyword,
                                                          TokenType.NotEqual
                                                      };

        private static TokenType[] BuildInTypes => new[]
                                                   {
                                                       TokenType.IntKeyword,
                                                       TokenType.FloatKeyword,
                                                       TokenType.StringKeyword
                                                   };

        private static TokenType[] AssignOperators => new[]
                                                      {
                                                          TokenType.Colon,
                                                          TokenType.ColonDiv,
                                                          TokenType.ColonMult,
                                                          TokenType.ColonAdd,
                                                          TokenType.ColonPow,
                                                          TokenType.ColonSub
                                                      };

        private static TokenType[] PostfixOperators => new[]
                                                       {
                                                           TokenType.Dot,
                                                           TokenType.OpenParenth,
                                                           TokenType.OpenBracket
                                                       };

        private static TokenType[] PrefixOperators => new[]
                                                      {
                                                          TokenType.Add,
                                                          TokenType.Sub,
                                                          TokenType.NotKeyword
                                                      };

        public CompilationUnitSyntax Parse()
        {
            var syntax = new List<ISyntaxBase>();
            while (TokenStream.Current != null)
            {
                switch (TokenStream.Current.Type)
                {
                    case TokenType.UsingKeyword:
                        syntax.Add(ParseUsing());
                        break;
                    case TokenType.NamespaceKeyword:
                        syntax.Add(ParseNamespace());
                        break;
                    default:
                        throw new KiwiSyntaxException("Unexpected Token. Expected Using or Namespace");
                }
            }
            return new CompilationUnitSyntax(syntax);
        }

        private NamespaceSyntax ParseNamespace()
        {
            ParseExpected(TokenType.NamespaceKeyword);
            var namespaceName = ParseExpected(TokenType.Symbol);
            var bodySyntax = ParseScope(ParseNamespaceBody);
            return new NamespaceSyntax(namespaceName, bodySyntax);
        }

        private ISyntaxBase ParseNamespaceBody()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.ClassKeyword:
                    return ParseClass();
                case TokenType.DataKeyword:
                    return ParseDataClass();
                case TokenType.EnumKeyword:
                    return ParseEnum();
                default:
                    throw new KiwiSyntaxException("Unexpected Token. Expected Class, Data or Enum");
            }
        }

        private UsingSyntax ParseUsing()
        {
            ParseExpected(TokenType.UsingKeyword);
            var namespaceName = ParseExpected(TokenType.Symbol);
            return new UsingSyntax(namespaceName);
        }

        private ClassSyntax ParseClass()
        {
            ParseExpected(TokenType.ClassKeyword);
            var className = ParseExpected(TokenType.Symbol);
            var inherit = TokenStream.Current.Type == TokenType.IsKeyword;
            Token descriptorName = null;

            if (inherit)
            {
                ParseExpected(TokenType.IsKeyword);
                descriptorName = ParseExpected(TokenType.Symbol);
            }

            var inner = ParseScope(ParseClassBody);
            return new ClassSyntax(className, descriptorName, inner);
        }

        private ISyntaxBase ParseClassBody()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.ConstructorKeyword:
                    return ParseConstructor();
                case TokenType.FuncKeyword:
                    return ParseFunction();
                case TokenType.VarKeyword:
                case TokenType.ConstKeyword:
                    return ParseField();
                default:
                    throw new KiwiSyntaxException("Unexpected Token. Expected Constructor, Func, Var or Const");
            }
        }

        private FieldSyntax ParseField()
        {
            var isConst = TokenStream.Current.Type == TokenType.ConstKeyword;
            var fieldTypeQualifier = ParseExpected(isConst ? TokenType.ConstKeyword : TokenType.VarKeyword);
            var fieldType = ParseExpected(TokenType.Symbol);
            var fieldName = ParseExpected(TokenType.Symbol);
            IExpressionSyntax fieldInitializer = null;

            if (isConst)
            {
                ParseExpected(TokenType.Colon);
                fieldInitializer = ParseExpression();
            }

            return new FieldSyntax(fieldTypeQualifier, fieldType, fieldName, fieldInitializer);
        }

        private IExpressionSyntax ParseExpression()
        {
            var firstExpression = ParseTermOrPrefixExpression();

            if (!BinaryOperators.Contains(TokenStream.Current.Type))
            {
                return firstExpression;
            }

            var expressionOperatorChain = new ExpressionOperatorChain();
            expressionOperatorChain.Add(firstExpression);

            while (BinaryOperators.Contains(TokenStream.Current.Type))
            {
                var @operator = TokenStream.Current;
                expressionOperatorChain.Add(@operator);
                TokenStream.Consume();
                var nExpression = ParseTermOrPrefixExpression();
                expressionOperatorChain.Add(nExpression);
            }

            return expressionOperatorChain.SolveOperatorPrecendence(BinaryOperators);
        }

        private IExpressionSyntax ParseTermOrPrefixExpression()
        {
            if (IsValidPrefixOperator(TokenStream.Current))
            {
                return ParsePrefixExpression();
            }

            var expression = ParseTermExpression();

            expression = ParsePostfixExpression(expression);
            return expression;
        }

        private IExpressionSyntax ParseTermExpression()
        {
            IExpressionSyntax expression;
            var current = TokenStream.Current;
            switch (current.Type)
            {
                case TokenType.TrueKeyword:
                    expression = ParseTrueExpression();
                    break;
                case TokenType.FalseKeyword:
                    expression = ParseFalseExpression();
                    break;
                case TokenType.Int:
                    expression = ParseIntExpression(current);
                    break;
                case TokenType.Float:
                    expression = ParseFloatExpression(current);
                    break;
                case TokenType.String:
                    expression = ParseStringExpression(current);
                    break;
                case TokenType.Symbol:
                    expression = ParseMemberExpression(current);
                    break;
                case TokenType.OpenParenth:
                    expression = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression).Single();
                    break;
                case TokenType.NewKeyword:
                    expression = ParseNewExpression();
                    break;
                case TokenType.IfKeyword:
                    expression = ParseIfElseExpression();
                    break;
                case TokenType.FuncKeyword:
                    expression = ParseFunctionExpression();
                    break;
                default:
                    throw new KiwiSyntaxException(
                        $"Unexpected Token {current}. Expected Sign Operator, New, Int, Float, String or Symbol Expression.");
            }
            return expression;
        }

        private IExpressionSyntax ParseMemberExpression(Token current)
        {
            ParseExpected(TokenType.Symbol);
            IExpressionSyntax expression = new MemberExpressionSyntax(current);
            return expression;
        }

        private IExpressionSyntax ParseStringExpression(Token current)
        {
            ParseExpected(TokenType.String);
            IExpressionSyntax expression = new StringExpressionSyntax(current);
            return expression;
        }

        private IExpressionSyntax ParseFloatExpression(Token current)
        {
            ParseExpected(TokenType.Float);
            IExpressionSyntax expression = new FloatExpressionSyntax(current);
            return expression;
        }

        private IExpressionSyntax ParseIntExpression(Token current)
        {
            ParseExpected(TokenType.Int);
            IExpressionSyntax expression = new IntExpressionSyntax(current);
            return expression;
        }

        private IExpressionSyntax ParseFalseExpression()
        {
            var falseToken = ParseExpected(TokenType.FalseKeyword);
            IExpressionSyntax expression = new BooleanExpressionSyntax(falseToken);
            return expression;
        }

        private IExpressionSyntax ParseTrueExpression()
        {
            var trueToken = ParseExpected(TokenType.TrueKeyword);
            IExpressionSyntax expression = new BooleanExpressionSyntax(trueToken);
            return expression;
        }

        private IExpressionSyntax ParseFunctionExpression()
        {
            ParseExpected(TokenType.FuncKeyword);
            TokenStream.TakeSnapshot();
            try
            {
                var anonymousFunctionExpression = ParseAnonymousFunctionExpression();
                TokenStream.CommitSnapshot();
                return anonymousFunctionExpression;
            }
            catch
            {
                TokenStream.RollbackSnapshot();
            }

            var implicitParameterList = ParseExpressionList();
            ParseExpected(TokenType.HypenGreater);
            var statement = ParseScopeOrSingleStatement();
            return new ImplicitParameterTypeAnonymousFunctionExpressionSyntax(implicitParameterList, statement);
        }

        private IExpressionSyntax ParseAnonymousFunctionExpression()
        {
            var parameter = ParseParameterList();
            ParseExpected(TokenType.HypenGreater);
            var statement = ParseScopeOrSingleStatement();
            return new AnonymousFunctionExpressionSyntax(parameter, statement);
        }

        private List<IExpressionSyntax> ParseExpressionList()
        {
            return ParseInnerCommmaSeperated(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression);
        }

        private List<ParameterSyntax> ParseParameterList()
        {
            return ParseInnerCommmaSeperated(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                ParseParameter);
        }

        private IExpressionSyntax ParsePrefixExpression()
        {
            var @operator = TokenStream.Current;
            if (!IsValidPrefixOperator(@operator))
            {
                throw new KiwiSyntaxException("Expected prefix operator");
            }

            TokenStream.Consume();
            return new SignExpressionSyntax(@operator, ParseTermOrPrefixExpression());
        }

        private IExpressionSyntax ParsePostfixExpression(IExpressionSyntax expression)
        {
            while (IsValidPostfixOperator(TokenStream.Current))
            {
                switch (TokenStream.Current.Type)
                {
                    case TokenType.Dot:
                        expression = ParseMemberAccessExpression(expression);
                        break;
                    case TokenType.OpenParenth:
                        expression = ParseInvocationExpression(expression);
                        break;
                    case TokenType.OpenBracket:
                        expression = ParseArrayAccessExpression(expression);
                        break;
                }
            }
            return expression;
        }

        private IExpressionSyntax ParseArrayAccessExpression(IExpressionSyntax expression)
        {
            var arrayParameter = new List<IExpressionSyntax>();
            while (TokenStream.Current.Type == TokenType.OpenBracket)
            {
                ParseExpected(TokenType.OpenBracket);
                arrayParameter.Add(ParseExpression());
                ParseExpected(TokenType.ClosingBracket);
            }
            expression = new ArrayAccessExpression(expression, arrayParameter);
            return expression;
        }

        private IExpressionSyntax ParseInvocationExpression(IExpressionSyntax expression)
        {
            var invokeParameter = ParseExpressionList();
            expression = new InvocationExpressionSyntax(expression, invokeParameter);
            return expression;
        }

        private IExpressionSyntax ParseMemberAccessExpression(IExpressionSyntax ownerExpression)
        {
            ParseExpected(TokenType.Dot);
            var memberName = ParseExpected(TokenType.Symbol);
            ownerExpression = new MemberAccessExpressionSyntax(ownerExpression, memberName);
            return ownerExpression;
        }

        private IfElseExpressionSyntax ParseIfElseExpression()
        {
            ParseExpected(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression).Single();
            var ifTrueExpression = ParseExpression();
            ParseExpected(TokenType.ElseKeyword);
            var ifFalseExpression = ParseExpression();
            return new IfElseExpressionSyntax(condition, ifTrueExpression, ifFalseExpression);
        }

        private IExpressionSyntax ParseNewExpression()
        {
            ParseExpected(TokenType.NewKeyword);
            var typeName = ParseSymbolOrBuildInType();
            if (TokenStream.Current.Type == TokenType.OpenBracket)
            {
                var dimension = 0;
                var sizes = new List<IExpressionSyntax>();
                for (; TokenStream.Current.Type == TokenType.OpenBracket; dimension++)
                {
                    ParseExpected(TokenType.OpenBracket);
                    sizes.Add(ParseExpression());
                    ParseExpected(TokenType.ClosingBracket);
                }
                return new ArrayCreationExpressionSyntax(typeName, sizes);
            }

            var parameter = ParseInner(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                ParseExpression,
                true).ToList();
            return new ObjectCreationExpressionSyntax(typeName, parameter);
        }

        private FunctionSyntax ParseFunction()
        {
            ParseExpected(TokenType.FuncKeyword);
            var functionName = ParseExpected(TokenType.Symbol);
            var functionParameter = ParseParameterList();

            var hasReturnValue = TokenStream.Current.Type == TokenType.HypenGreater;
            if (hasReturnValue)
            {
                return ParseFunctionThatReturnValue(functionName, functionParameter);
            }

            var statements = ParseScope(ParseStatement);

            return new FunctionSyntax(functionName, functionParameter, statements);
        }

        private IStatementSyntax ParseStatement()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.IfKeyword:
                    return ParseIfStatement();
                case TokenType.ReturnKeyword:
                    return ParseReturnStatement();
                case TokenType.WhenKeyword:
                    return ParseWhenStatement();
                case TokenType.SwitchKeyword:
                    return ParseSwitchStatement();
                case TokenType.VarKeyword:
                case TokenType.ConstKeyword:
                    return ParseVariableDeclaration();
                case TokenType.Symbol:
                    return ParseFunctionCallOrAssignStatement();
                case TokenType.ForKeyword:
                    return ParseForInOrForStatement();
                case TokenType.ForReverseKeyword:
                    return ParseForReverseStatement();
                default:
                    throw new KiwiSyntaxException(
                        "Unexpected Token. Expected If, Return, When, Switch, Var, Const, Symbol, For or Forr");
            }
        }

        private IStatementSyntax ParseForReverseStatement()
        {
            throw new NotImplementedException();
        }

        private IStatementSyntax ParseForInOrForStatement()
        {
            throw new NotImplementedException();
        }

        private IStatementSyntax ParseFunctionCallOrAssignStatement()
        {
            var expression = ParseExpression();

            if (IsMemberExpression(expression))
            {
                var current = TokenStream.Current;
                if (!IsValidAssignOperator(current))
                {
                    throw new KiwiSyntaxException("Unexpected assign operator {0}. Expected :, :+, :-, :/, :* or :^.");
                }
                ParseExpected(current.Type);
                var intializer = ParseExpression();
                return new AssignmentStatementSyntax(expression, current, intializer);
            }

            var invocationExpression = expression as InvocationExpressionSyntax;
            if (invocationExpression != null)
            {
                return new InvocationStatementSyntax(invocationExpression);
            }

            throw new KiwiSyntaxException(
                "Unexpected Syntax. Expected MemberAccessExpressionSyntax, ArrayAccessExpression, MemberExpressionSyntax or InvocationExpressionSyntax");
        }

        private static bool IsMemberExpression(IExpressionSyntax expression)
        {
            return expression is MemberAccessExpressionSyntax || expression is ArrayAccessExpression
                   || expression is MemberExpressionSyntax;
        }

        private ParameterSyntax ParseParameter()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.TwoDots:
                    return ParseParamsParameter();
                case TokenType.IntKeyword:
                case TokenType.FloatKeyword:
                case TokenType.StringKeyword:
                case TokenType.Symbol:
                    return ParseSimpleParameter();
                default:
                    throw new KiwiSyntaxException("Unexpected Token. Expected .., Int, Float, String or Symbol");
            }
        }

        private FunctionSyntax ParseFunctionThatReturnValue(Token functionName, List<ParameterSyntax> functionParameter)
        {
            ParseExpected(TokenType.HypenGreater);

            switch (TokenStream.Current.Type)
            {
                case TokenType.DataKeyword:
                    return ParseDataFunction(functionName, functionParameter);
                case TokenType.ReturnKeyword:
                    return ParseExpressionFunction(functionName, functionParameter);
                default:
                {
                    var returnType = ParseSimpleTypeOrArrayType();
                    var statements = ParseScope(ParseStatement);
                    return new ReturnFunctionSyntax(functionName, functionParameter.ToList(), statements, returnType);
                }
            }
        }

        private TypeSyntax ParseSimpleTypeOrArrayType()
        {
            var type = ParseSymbolOrBuildInType();
            if (TokenStream.Current.Type == TokenType.OpenBracket)
            {
                var dimension = 0;
                for (; TokenStream.Current.Type == TokenType.OpenBracket; dimension++)
                {
                    ParseExpected(TokenType.OpenBracket);
                    ParseExpected(TokenType.ClosingBracket);
                }
                return new ArrayTypeSyntax(type.TypeName, dimension);
            }
            return type;
        }

        private ExpressionFunctionSyntax ParseExpressionFunction(
            Token functionName,
            List<ParameterSyntax> functionParameter)
        {
            var statement = ParseReturnStatement();
            return new ExpressionFunctionSyntax(functionName, functionParameter, statement);
        }

        private DataClassFunctionSyntax ParseDataFunction(Token functionName, List<ParameterSyntax> functionParameter)
        {
            var dataClass = ParseDataClass();
            var statements = ParseScope(ParseStatement);
            return new DataClassFunctionSyntax(functionName, functionParameter.ToList(), statements, dataClass);
        }

        private VariableDeclarationStatementSyntax ParseVariableDeclaration()
        {
            var current = TokenStream.Current;
            Token variableQualifier;
            switch (current.Type)
            {
                case TokenType.VarKeyword:
                case TokenType.ConstKeyword:
                    ParseExpected(current.Type);
                    variableQualifier = current;
                    break;
                default:
                    throw new KiwiSyntaxException("Expected VariableQualifier");
            }
            var variableNames = new List<Token>();
            for (variableNames.Add(ParseExpected(TokenType.Symbol));
                 TokenStream.Current.Type == TokenType.Comma;
                 variableNames.Add(ParseExpected(TokenType.Symbol)))
            {
                ParseExpected(TokenType.Comma);
            }
            ParseExpected(TokenType.Colon);
            var initializer = ParseExpression();
            return new VariableDeclarationStatementSyntax(variableQualifier, variableNames, initializer);
        }

        private ReturnStatementSyntax ParseReturnStatement()
        {
            ParseExpected(TokenType.ReturnKeyword);
            var returnExpression = ParseExpression();
            return new ReturnStatementSyntax(returnExpression);
        }

        private DataSyntax ParseDataClass()
        {
            ParseExpected(TokenType.DataKeyword);
            var typeName = ParseExpected(TokenType.Symbol);
            var parameter = ParseParameterList();
            return new DataSyntax(typeName, parameter.ToList());
        }

        private ConstructorSyntax ParseConstructor()
        {
            ParseExpected(TokenType.ConstructorKeyword);

            var parameter = ParseParameterList();

            var bodySyntax = ParseScope(ParseStatement);

            return new ConstructorSyntax(parameter, bodySyntax);
        }

        private List<IStatementSyntax> ParseScopeOrSingleStatement()
        {
            var hasScope = TokenStream.Current.Type == TokenType.OpenBrace;
            var statements = hasScope ? ParseScope(ParseStatement) : new List<IStatementSyntax> { ParseStatement() };
            return statements;
        }

        private SwitchStatementSyntax ParseSwitchStatement()
        {
            ParseExpected(TokenType.SwitchKeyword);
            ParseExpected(TokenType.OpenParenth);
            var condition = ParseExpression();
            ParseExpected(TokenType.ClosingParenth);
            var caseAndDefaultSyntax = ParseScope(ParseCaseOrElse);

            ElseSyntax elseSyntax;
            try
            {
                elseSyntax = caseAndDefaultSyntax.SingleOrDefault(x => x is ElseSyntax) as ElseSyntax;
            }
            catch
            {
                throw new KiwiSyntaxException("Duplicate else label");
            }

            return new SwitchStatementSyntax(condition, caseAndDefaultSyntax.OfType<CaseSyntax>().ToList(), elseSyntax);
        }

        private ISyntaxBase ParseCaseOrElse()
        {
            switch (TokenStream.Current.Type)
            {
                case TokenType.CaseKeyword:
                    return ParseCase();
                case TokenType.ElseKeyword:
                    return ParseElse();
                default:
                    throw new KiwiSyntaxException("Unexpected Token. Expected Case or Else");
            }
        }

        private ElseSyntax ParseElse()
        {
            ParseExpected(TokenType.ElseKeyword);
            ParseExpected(TokenType.HypenGreater);

            var statements = ParseScopeOrSingleStatement();

            return new ElseSyntax(statements);
        }

        private ISyntaxBase ParseCase()
        {
            ParseExpected(TokenType.CaseKeyword);
            var expression = ParseExpression();
            ParseExpected(TokenType.HypenGreater);

            var statements = ParseScopeOrSingleStatement();

            return new CaseSyntax(expression, statements);
        }

        private IStatementSyntax ParseWhenStatement()
        {
            ParseExpected(TokenType.WhenKeyword);

            if (TokenStream.Current.Type == TokenType.OpenParenth)
            {
                return ParseConditionalWhenStatement();
            }
            return ParseSimpleWhenStatement();
        }

        private SimpleWhenStatementSyntax ParseSimpleWhenStatement()
        {
            var whenEntries = ParseInner(TokenType.OpenBrace, TokenType.ClosingBrace, ParseWhenSimpleWhenEntry);
            return new SimpleWhenStatementSyntax(whenEntries);
        }

        private WhenEntry ParseWhenSimpleWhenEntry()
        {
            var condition = ParseExpression();
            ParseExpected(TokenType.HypenGreater);
            var statements = ParseScopeOrSingleStatement();
            return new WhenEntry(condition, statements);
        }

        private ConditionalWhenStatementSyntax ParseConditionalWhenStatement()
        {
            ParseExpected(TokenType.OpenParenth);
            var condition = ParseExpression();
            ParseExpected(TokenType.ClosingParenth);

            var whenEntries = ParseInner(TokenType.OpenBrace, TokenType.ClosingBrace, ParseConditionalWhenEntry);
            return new ConditionalWhenStatementSyntax(condition, whenEntries);
        }

        private ConditionalWhenEntry ParseConditionalWhenEntry()
        {
            var binaryOperators = BinaryOperators;
            if (!binaryOperators.Contains(TokenStream.Current.Type))
            {
                throw new KiwiSyntaxException("Expected a binary operator");
            }

            var op = TokenStream.Current;
            TokenStream.Consume();

            var condition = ParseExpression();
            ParseExpected(TokenType.HypenGreater);
            var statements = ParseScopeOrSingleStatement();
            return new ConditionalWhenEntry(op, condition, statements);
        }

        private IfStatementSyntax ParseIfStatement()
        {
            ParseExpected(TokenType.IfKeyword);

            try
            {
                var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression);
                var statements = ParseScope(ParseStatement);
                if (TokenStream.Current.Type == TokenType.ElseKeyword)
                {
                    return ParseIfElseStatement(condition, statements);
                }
                return new IfStatementSyntax(condition, statements.ToList());
            }
            catch (KiwiSyntaxException exception)
            {
                throw new KiwiSyntaxException(exception.Message);
            }
        }

        private IfElseStatementSyntax ParseIfElseStatement(
            List<IExpressionSyntax> condition,
            List<IStatementSyntax> statements)
        {
            ParseExpected(TokenType.ElseKeyword);
            var elseBody = ParseScope(ParseStatement);
            return new IfElseStatementSyntax(condition, statements, elseBody.ToList());
        }

        private ParameterSyntax ParseSimpleParameter()
        {
            var typeName = ParseSimpleTypeOrArrayType();

            var parameterName = ParseExpected(TokenType.Symbol);
            return new ParameterSyntax(typeName, parameterName);
        }

        private TypeSyntax ParseSymbolOrBuildInType()
        {
            var current = TokenStream.Current;
            Token typeName;
            if (BuildInTypes.Contains(current.Type))
            {
                TokenStream.Consume();
                typeName = current;
            }
            else
            {
                typeName = ParseExpected(TokenType.Symbol);
            }

            return new TypeSyntax(typeName);
        }

        private ParameterSyntax ParseParamsParameter()
        {
            ParseExpected(TokenType.TwoDots);
            var typeName = ParseSimpleTypeOrArrayType();
            var parameterName = ParseExpected(TokenType.Symbol);
            return new ParameterSyntax(typeName, parameterName);
        }

        private EnumSyntax ParseEnum()
        {
            ParseExpected(TokenType.EnumKeyword);
            var enumName = ParseExpected(TokenType.Symbol);
            var memberSyntax = ParseInnerCommmaSeperated(
                TokenType.OpenBrace,
                TokenType.ClosingBrace,
                ParseEnumMember);
            return new EnumSyntax(enumName, memberSyntax);
        }

        private ISyntaxBase ParseEnumMember()
        {
            var memberName = ParseExpected(TokenType.Symbol);
            var hasExplicitValue = TokenStream.Current.Type == TokenType.Colon;
            IExpressionSyntax initializer = null;
            if (hasExplicitValue)
            {
                ParseExpected(TokenType.Colon);
                initializer = ParseExpression();
            }
            return new EnumMemberSyntax(memberName, initializer);
        }

        private List<TSyntax> ParseScope<TSyntax>(Func<TSyntax> parser) where TSyntax : ISyntaxBase
        {
            return ParseInner(TokenType.OpenBrace, TokenType.ClosingBrace, parser);
        }

        private static List<Token> RemoveUnnecessaryToken(List<Token> token)
        {
            return token.Where(x => x.Type != TokenType.Whitespace)
                        .Where(x => x.Type != TokenType.Tab)
                        .Where(x => x.Type != TokenType.NewLine)
                        .Where(x => x.Type != TokenType.Comment)
                        .ToList();
        }

        private static bool IsValidAssignOperator(Token token)
        {
            return AssignOperators.Contains(token.Type);
        }

        private static bool IsValidPostfixOperator(Token token)
        {
            return PostfixOperators.Contains(token.Type);
        }

        private static bool IsValidPrefixOperator(Token token)
        {
            return PrefixOperators.Contains(token.Type);
        }
    }
}