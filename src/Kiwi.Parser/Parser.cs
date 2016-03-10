using System;
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
                var result = Parse<ISyntaxBase>(ParseUsing, ParseNamespace);
                if (result == null)
                {
                    throw new KiwiSyntaxException("Unexpected Syntax. Expected UsingSyntax or NamespaceSyntax");
                }
                syntax.Add(result);
            }
            return new CompilationUnitSyntax(syntax);
        }

        private NamespaceSyntax ParseNamespace()
        {
            Consume(TokenType.NamespaceKeyword);
            var namespaceName = Consume(TokenType.Symbol);

            var bodySyntax = ParseScope(ParseNamespaceBody);
            return new NamespaceSyntax(namespaceName, bodySyntax);
        }

        private ISyntaxBase ParseNamespaceBody()
        {
            return Parse<ISyntaxBase>(ParseClass, ParseDataClass, ParseEnum);
        }

        private UsingSyntax ParseUsing()
        {
            Consume(TokenType.UsingKeyword);
            var namespaceName = Consume(TokenType.Symbol);
            return new UsingSyntax(namespaceName);
        }

        private ClassSyntax ParseClass()
        {
            Consume(TokenType.ClassKeyword);
            var className = Consume(TokenType.Symbol);
            var inherit = TokenStream.Current.Type == TokenType.IsKeyword;
            Token descriptorName = null;

            if (inherit)
            {
                Consume(TokenType.IsKeyword);
                descriptorName = Consume(TokenType.Symbol);
            }

            var inner = ParseScope(ParseClassBody);
            return new ClassSyntax(className, descriptorName, inner);
        }

        private ISyntaxBase ParseClassBody()
        {
            return Parse<ISyntaxBase>(ParseConstructor, ParseFunction, ParseField);
        }

        private FieldSyntax ParseField()
        {
            var isConst = TokenStream.Current.Type == TokenType.ConstKeyword;
            var fieldTypeQualifier = Consume(isConst ? TokenType.ConstKeyword : TokenType.VarKeyword);
            var fieldType = Consume(TokenType.Symbol);
            var fieldName = Consume(TokenType.Symbol);
            IExpressionSyntax fieldInitializer = null;

            if (isConst)
            {
                Consume(TokenType.Colon);
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
            Consume(TokenType.Symbol);
            IExpressionSyntax expression = new MemberExpressionSyntax(current);
            return expression;
        }

        private IExpressionSyntax ParseStringExpression(Token current)
        {
            IExpressionSyntax expression;
            Consume(TokenType.String);
            expression = new StringExpressionSyntax(current);
            return expression;
        }

        private IExpressionSyntax ParseFloatExpression(Token current)
        {
            IExpressionSyntax expression;
            Consume(TokenType.Float);
            expression = new FloatExpressionSyntax(current);
            return expression;
        }

        private IExpressionSyntax ParseIntExpression(Token current)
        {
            IExpressionSyntax expression;
            Consume(TokenType.Int);
            expression = new IntExpressionSyntax(current);
            return expression;
        }

        private IExpressionSyntax ParseFalseExpression()
        {
            IExpressionSyntax expression;
            var falseToken = Consume(TokenType.FalseKeyword);
            expression = new BooleanExpressionSyntax(falseToken);
            return expression;
        }

        private IExpressionSyntax ParseTrueExpression()
        {
            IExpressionSyntax expression;
            var trueToken = Consume(TokenType.TrueKeyword);
            expression = new BooleanExpressionSyntax(trueToken);
            return expression;
        }

        private IExpressionSyntax ParseFunctionExpression()
        {
            Consume(TokenType.FuncKeyword);
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
            Consume(TokenType.HypenGreater);
            var statement = ParseScopeOrSingleStatement();
            return new ImplicitParameterTypeAnonymousFunctionExpressionSyntax(implicitParameterList, statement);
        }

        private IExpressionSyntax ParseAnonymousFunctionExpression()
        {
            var parameter = ParseParameterList();
            Consume(TokenType.HypenGreater);
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
                Consume(TokenType.OpenBracket);
                arrayParameter.Add(ParseExpression());
                Consume(TokenType.ClosingBracket);
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
            Consume(TokenType.Dot);
            var memberName = Consume(TokenType.Symbol);
            ownerExpression = new MemberAccessExpressionSyntax(ownerExpression, memberName);
            return ownerExpression;
        }

        private IfElseExpressionSyntax ParseIfElseExpression()
        {
            Consume(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression).Single();
            var ifTrueExpression = ParseExpression();
            Consume(TokenType.ElseKeyword);
            var ifFalseExpression = ParseExpression();
            return new IfElseExpressionSyntax(condition, ifTrueExpression, ifFalseExpression);
        }

        private IExpressionSyntax ParseNewExpression()
        {
            Consume(TokenType.NewKeyword);
            var typeName = ParseSymbolOrBuildInType();
            if (TokenStream.Current.Type == TokenType.OpenBracket)
            {
                var dimension = 0;
                var sizes = new List<IExpressionSyntax>();
                for (; TokenStream.Current.Type == TokenType.OpenBracket; dimension++)
                {
                    Consume(TokenType.OpenBracket);
                    sizes.Add(ParseExpression());
                    Consume(TokenType.ClosingBracket);
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
            Consume(TokenType.FuncKeyword);
            var functionName = Consume(TokenType.Symbol);
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
            return Parse(
                ParseIfStatement,
                ParseReturnStatement,
                ParseIfStatement,
                ParseWhenStatement,
                ParseSwitchStatement,
                ParseVariableDeclaration,
                ParseAssignmentStatement,
                ParseForInStatement,
                ParseReverseForInStatement,
                ParseForStatement,
                ParseFunctionCallStatement);
        }

        private ParameterSyntax ParseParameter()
        {
            return Parse(ParseParamsParameter, ParseSimpleParameter);
        }

        private FunctionSyntax ParseFunctionThatReturnValue(Token functionName, List<ParameterSyntax> functionParameter)
        {
            Consume(TokenType.HypenGreater);

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
                    Consume(TokenType.OpenBracket);
                    Consume(TokenType.ClosingBracket);
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
                    Consume(current.Type);
                    variableQualifier = current;
                    break;
                default:
                    throw new KiwiSyntaxException("Expected VariableQualifier");
            }
            var variableNames = new List<Token>();
            for (variableNames.Add(Consume(TokenType.Symbol));
                 TokenStream.Current.Type == TokenType.Comma;
                 variableNames.Add(Consume(TokenType.Symbol)))
            {
                Consume(TokenType.Comma);
            }
            Consume(TokenType.Colon);
            var initializer = ParseExpression();
            return new VariableDeclarationStatementSyntax(variableQualifier, variableNames, initializer);
        }

        private ReturnStatementSyntax ParseReturnStatement()
        {
            Consume(TokenType.ReturnKeyword);
            var returnExpression = ParseExpression();
            return new ReturnStatementSyntax(returnExpression);
        }

        private DataSyntax ParseDataClass()
        {
            Consume(TokenType.DataKeyword);
            var typeName = Consume(TokenType.Symbol);
            var parameter = ParseParameterList();
            return new DataSyntax(typeName, parameter.ToList());
        }

        private ConstructorSyntax ParseConstructor()
        {
            Consume(TokenType.ConstructorKeyword);

            var parameter = ParseParameterList();

            var bodySyntax = ParseScope(ParseStatement);

            return new ConstructorSyntax(parameter, bodySyntax);
        }

        private ForInStatementSyntax ParseForInStatement()
        {
            Consume(TokenType.ForKeyword);
            IExpressionSyntax itemExpression;
            IExpressionSyntax collectionExpression;
            List<IStatementSyntax> statements;
            bool declareItemInnerScope;
            ParseForIn(out itemExpression, out declareItemInnerScope, out collectionExpression, out statements);
            return new ForInStatementSyntax(itemExpression, declareItemInnerScope, collectionExpression, statements);
        }

        private ForInStatementSyntax ParseReverseForInStatement()
        {
            Consume(TokenType.ForReverseKeyword);
            IExpressionSyntax itemExpression;
            IExpressionSyntax collectionExpression;
            List<IStatementSyntax> statements;
            bool declareItemInnerScope;
            ParseForIn(out itemExpression, out declareItemInnerScope, out collectionExpression, out statements);
            return new ReverseForInStatementSyntax(
                itemExpression,
                declareItemInnerScope,
                collectionExpression,
                statements);
        }

        private void ParseForIn(
            out IExpressionSyntax itemExpression,
            out bool declareItemInnerScope,
            out IExpressionSyntax collectionExpression,
            out List<IStatementSyntax> statements)
        {
            Consume(TokenType.OpenParenth);

            declareItemInnerScope = TokenStream.Current.Type == TokenType.VarKeyword;
            if (declareItemInnerScope)
            {
                Consume(TokenType.VarKeyword);
            }

            itemExpression = ParseTermExpression();
            Consume(TokenType.InKeyword);
            collectionExpression = ParseExpression();
            Consume(TokenType.ClosingParenth);
            statements = ParseScope(ParseStatement);
        }

        private ForStatementSyntax ParseForStatement()
        {
            Consume(TokenType.ForKeyword);
            Consume(TokenType.OpenParenth);

            var initExpression = ParseForInitExpression();
            Consume(TokenType.Semicolon);
            var condExpression = ParseExpression();
            Consume(TokenType.Semicolon);
            var loopExpression = ParseForInitExpression();
            Consume(TokenType.ClosingParenth);

            var statements = ParseScopeOrSingleStatement();

            return new ForStatementSyntax(initExpression, condExpression, loopExpression, statements);
        }

        private List<IStatementSyntax> ParseScopeOrSingleStatement()
        {
            var hasScope = TokenStream.Current.Type == TokenType.OpenBrace;
            var statements = hasScope ? ParseScope(ParseStatement) : new List<IStatementSyntax> { ParseStatement() };
            return statements;
        }

        private InvocationStatementSyntax ParseFunctionCallStatement()
        {
            var expression = ParseExpression();
            if (expression.GetType() != typeof(InvocationExpressionSyntax))
            {
                throw new KiwiSyntaxException("Expected InvocationExpression");
            }
            return new InvocationStatementSyntax((InvocationExpressionSyntax)expression);
        }

        private ISyntaxBase ParseForInitExpression()
        {
            var result = Parse<ISyntaxBase>(
                ParseVariableDeclaration,
                ParseAssignmentStatement,
                ParseExpression);

            if (!IsValidForInitExpression(result))
            {
                throw new KiwiSyntaxException(
                    "Only assignment call increment decrement and new object expressions can be used.");
            }

            return result;
        }

        private AssignmentStatementSyntax ParseAssignmentStatement()
        {
            var member = ParseExpression();

            if (!(member is MemberAccessExpressionSyntax || member is ArrayAccessExpression
                  || member is MemberExpressionSyntax))
            {
                throw new KiwiSyntaxException("Expected MemberAccessExpression");
            }

            var current = TokenStream.Current;
            if (!IsValidAssignOperator(current))
            {
                throw new KiwiSyntaxException("Unexpected assign operator {0}. Expected :, :+, :-, :/, :* or :^.");
            }
            Consume(current.Type);
            var intializer = ParseExpression();
            return new AssignmentStatementSyntax(member, current, intializer);
        }

        private SwitchStatementSyntax ParseSwitchStatement()
        {
            Consume(TokenType.SwitchKeyword);
            Consume(TokenType.OpenParenth);
            var condition = ParseExpression();
            Consume(TokenType.ClosingParenth);
            var caseAndDefaultSyntax = ParseScope(() => Parse(ParseCase, ParseElse));

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

        private ElseSyntax ParseElse()
        {
            Consume(TokenType.ElseKeyword);
            Consume(TokenType.HypenGreater);

            var statements = ParseScopeOrSingleStatement();

            return new ElseSyntax(statements);
        }

        private ISyntaxBase ParseCase()
        {
            Consume(TokenType.CaseKeyword);
            var expression = ParseExpression();
            Consume(TokenType.HypenGreater);

            var statements = ParseScopeOrSingleStatement();

            return new CaseSyntax(expression, statements);
        }

        private IStatementSyntax ParseWhenStatement()
        {
            Consume(TokenType.WhenKeyword);

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
            Consume(TokenType.HypenGreater);
            var statements = ParseScopeOrSingleStatement();
            return new WhenEntry(condition, statements);
        }

        private ConditionalWhenStatementSyntax ParseConditionalWhenStatement()
        {
            Consume(TokenType.OpenParenth);
            var condition = ParseExpression();
            Consume(TokenType.ClosingParenth);

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
            Consume(TokenType.HypenGreater);
            var statements = ParseScopeOrSingleStatement();
            return new ConditionalWhenEntry(op, condition, statements);
        }

        private IfStatementSyntax ParseIfStatement()
        {
            Consume(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpression);
            var statements = ParseScope(ParseStatement);
            if (TokenStream.Current.Type == TokenType.ElseKeyword)
            {
                return ParseIfElseStatement(condition, statements);
            }
            return new IfStatementSyntax(condition, statements.ToList());
        }

        private IfElseStatementSyntax ParseIfElseStatement(
            List<IExpressionSyntax> condition,
            List<IStatementSyntax> statements)
        {
            Consume(TokenType.ElseKeyword);
            var elseBody = ParseScope(ParseStatement);
            return new IfElseStatementSyntax(condition, statements, elseBody.ToList());
        }

        private ParameterSyntax ParseSimpleParameter()
        {
            var typeName = ParseSimpleTypeOrArrayType();

            var parameterName = Consume(TokenType.Symbol);
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
                typeName = Consume(TokenType.Symbol);
            }

            return new TypeSyntax(typeName);
        }

        private ParameterSyntax ParseParamsParameter()
        {
            Consume(TokenType.TwoDots);
            var typeName = ParseSimpleTypeOrArrayType();
            var parameterName = Consume(TokenType.Symbol);
            return new ParameterSyntax(typeName, parameterName);
        }

        private EnumSyntax ParseEnum()
        {
            Consume(TokenType.EnumKeyword);
            var enumName = Consume(TokenType.Symbol);
            var memberSyntax = ParseInnerCommmaSeperated(
                TokenType.OpenBrace,
                TokenType.ClosingBrace,
                ParseEnumMember);
            return new EnumSyntax(enumName, memberSyntax);
        }

        private ISyntaxBase ParseEnumMember()
        {
            var memberName = Consume(TokenType.Symbol);
            var hasExplicitValue = TokenStream.Current.Type == TokenType.Colon;
            IExpressionSyntax initializer = null;
            if (hasExplicitValue)
            {
                Consume(TokenType.Colon);
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

        private static bool IsValidForInitExpression(ISyntaxBase result)
        {
            return new[]
                   {
                       typeof(AssignmentStatementSyntax),
                       typeof(VariableDeclarationStatementSyntax),
                       typeof(CallExpressionSyntax),
                       typeof(ObjectCreationExpressionSyntax)
                   }.Contains(result.GetType());
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