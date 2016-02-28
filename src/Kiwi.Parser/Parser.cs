using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiwi.Common;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class Parser
    {
        private readonly TransactableTokenStream _tokenStream;
        private Func<ISyntaxBase> _namespaceBodyParser;
        private Func<ISyntaxBase> _classBodyParser;
        private Func<ISyntaxBase> _functionParameterParser;
        private Func<ISyntaxBase> _functionBodyParser;

        public Parser(List<Token> token)
        {
            var cleanedToken = token.Where(x => x.Type != TokenType.Whitespace)
                                    .Where(x => x.Type != TokenType.Tab)
                                    .Where(x => x.Type != TokenType.NewLine)
                                    .Where(x => x.Type != TokenType.Comment)
                                    .ToList();
            _tokenStream = new TransactableTokenStream(cleanedToken);
            InitParser();
        }

        private void InitParser()
        {
            _namespaceBodyParser = () => Parse(ParseClassSyntax, ParseDataClassSyntax, ParseEnumSyntax);
            _classBodyParser = () => Parse(ParseConstructorSyntax, ParseFunctionSyntax, ParseFieldSyntax);
            _functionParameterParser = () => Parse(ParseParameterSyntax, ParseParamsParameterSyntax);
            _functionBodyParser = () => Parse(
                ParseIfStatementSyntax,
                ParseReturnStatementSyntax,
                ParseIfStatementSyntax,
                ParseWhenStatementSyntax,
                ParseSwitchStatementSyntax,
                ParseVariableDeclarationSyntax,
                ParseVariableAssignmentStatementSyntax,
                ParseForInStatementSyntax,
                ParseReverseForInStatementSyntax,
                ParseForStatementSyntax);
        }

        public CompilationUnitSyntax Parse()
        {
            var syntax = new List<ISyntaxBase>();
            while (_tokenStream.Current != null)
            {
                var result = Parse(ParseUsingSyntax, ParseNamespaceSyntax);
                if (result == null)
                {
                    throw new KiwiSyntaxException("Unexpected Syntax. Expected UsingSyntax or NamespaceSyntax");
                }
                syntax.Add(result);
            }
            return new CompilationUnitSyntax(syntax);
        }

        private NamespaceSyntax ParseNamespaceSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.NamespaceKeyword)
            {
                return null;
            }

            Consume(TokenType.NamespaceKeyword);
            var namespaceName = Consume(TokenType.Symbol);
            
            var bodySyntax = ParseScope(_namespaceBodyParser);
            return new NamespaceSyntax(namespaceName, bodySyntax);
        }

        private UsingSyntax ParseUsingSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.UsingKeyword)
            {
                return null;
            }

            Consume(TokenType.UsingKeyword);
            var namespaceName = Consume(TokenType.Symbol);
            return new UsingSyntax(namespaceName);
        }

        private ClassSyntax ParseClassSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ClassKeyword)
            {
                return null;
            }

            Consume(TokenType.ClassKeyword);
            var className = Consume(TokenType.Symbol);
            var inherit = _tokenStream.Current.Type == TokenType.IsKeyword;
            Token descriptorName = null;

            if (inherit)
            {
                Consume(TokenType.IsKeyword);
                descriptorName = Consume(TokenType.Symbol);
            }
            
            var inner = ParseScope(_classBodyParser);
            return new ClassSyntax(className, descriptorName, inner);
        }

        private FieldSyntax ParseFieldSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ConstKeyword && _tokenStream.Current.Type != TokenType.VarKeyword)
            {
                return null;
            }

            var isConst = _tokenStream.Current.Type == TokenType.ConstKeyword;
            var fieldTypeQualifier = Consume(isConst ? TokenType.ConstKeyword : TokenType.VarKeyword);
            var fieldType = Consume(TokenType.Symbol);
            var fieldName = Consume(TokenType.Symbol);
            IExpressionSyntax fieldInitializer = null;

            if (isConst)
            {
                Consume(TokenType.Colon);
                fieldInitializer = ParseExpressionSyntax();
            }

            return new FieldSyntax(fieldTypeQualifier, fieldType, fieldName, fieldInitializer);
        }

        private IExpressionSyntax ParseExpressionSyntax()
        {
            var operators = new[]
                            {
                                TokenType.Equal,
                                TokenType.Mult,
                                TokenType.Div,
                                TokenType.Add,
                                TokenType.Sub,
                                TokenType.Pow,
                                TokenType.Less,
                                TokenType.Greater,
                                TokenType.Or
                            };

            var firstExpression = ParseSingleExpression();
            
            if (!operators.Contains(_tokenStream.Current.Type))
            {
                return firstExpression;
            }

            var expressionOperatorChain = new List<Tuple<Token, IExpressionSyntax>>();
            expressionOperatorChain.Add(new Tuple<Token, IExpressionSyntax>(null, firstExpression));
            while (operators.Contains(_tokenStream.Current.Type))
            {
                var @operator = _tokenStream.Current;
                expressionOperatorChain.Add(new Tuple<Token, IExpressionSyntax>(@operator, null));
                _tokenStream.Consume();
                var nExpression = ParseSingleExpression();
                expressionOperatorChain.Add(new Tuple<Token, IExpressionSyntax>(null, nExpression));
            }

            foreach (var @operator in operators)
            {
                for (int index = expressionOperatorChain.Count - 1; index >= 0; index--)
                {
                    var expressionOrOperator = expressionOperatorChain[index];
                    if (expressionOrOperator.Item1 != null && expressionOrOperator.Item1.Type == @operator)
                    {
                        var leftExpression = expressionOperatorChain[index - 1].Item2;
                        var rightExpression = expressionOperatorChain[index + 1].Item2;

                        var binaryExpressionSyntax = new BinaryExpressionSyntax(leftExpression, rightExpression, expressionOrOperator.Item1);
                        expressionOperatorChain[index - 1] = new Tuple<Token, IExpressionSyntax>(null, binaryExpressionSyntax);
                        expressionOperatorChain.RemoveAt(index);
                        expressionOperatorChain.RemoveAt(index);
                    }
                }
            }

            Debug.Assert(expressionOperatorChain.Count == 1);
            return expressionOperatorChain[0].Item2;
        }

        private IExpressionSyntax ParseSingleExpression()
        {
            var signOperators = new[]
                            {
                                TokenType.Add,
                                TokenType.Sub
                            };
            
            var current = _tokenStream.Current;

            if (signOperators.Contains(current.Type))
            {
                _tokenStream.Consume();
                return new SignExpressionSyntax(current, ParseSingleExpression());
            }
            
            IExpressionSyntax expression;
            switch (current.Type)
            {
                case TokenType.TrueKeyword:
                    var trueToken = Consume(TokenType.FalseKeyword);
                    expression = new BooleanExpressionSyntax(trueToken);
                    break;
                case TokenType.FalseKeyword:
                    var falseToken = Consume(TokenType.FalseKeyword);
                    expression = new BooleanExpressionSyntax(falseToken);
                    break;
                case TokenType.Int:
                    Consume(TokenType.Int);
                    var intExpression = new IntExpressionSyntax(current);
                    if (_tokenStream.Current.Type == TokenType.TwoDots)
                    {
                        Consume(TokenType.TwoDots);
                        var rightIntExpression = Consume(TokenType.Int);
                        expression = new RangeExpressionSyntax(intExpression, rightIntExpression);
                    }
                    else
                    {
                        expression = intExpression;
                    }
                    break;
                case TokenType.Float:
                    Consume(TokenType.Float);
                    expression =  new FloatExpressionSyntax(current);
                    break;
                case TokenType.String:
                    Consume(TokenType.String);
                    expression = new StringExpressionSyntax(current);
                    break;
                case TokenType.Symbol:
                    Consume(TokenType.Symbol);
                    expression = new MemberExpressionSyntax(current);
                    break;
                case TokenType.OpenParenth:
                    expression = (IExpressionSyntax)ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpressionSyntax).Single();
                    break;
                case TokenType.NewKeyword:
                    expression = ParseNewExpression();
                    break;
                case TokenType.IfKeyword:
                    expression = ParseIfElseExpression();
                    break;
                default:
                    throw new KiwiSyntaxException(
                        $"Unexpected Token {current}. Expected Sign Operator, New, Int, Float, String or Symbol Expression.");
            }

            var memberOperators = new[] { TokenType.Dot, TokenType.OpenParenth, TokenType.LeftSquareBracket };
            while (memberOperators.Contains(_tokenStream.Current.Type))
            {
                switch (_tokenStream.Current.Type)
                {
                    case TokenType.Dot:
                        Consume(TokenType.Dot);
                        var memberName = Consume(TokenType.Symbol);
                        expression = new MemberAccessExpressionSyntax(expression, memberName);
                        break;
                    case TokenType.OpenParenth:
                        var invokeParameter =
                            ParseInnerCommmaSeperated(
                                TokenType.OpenParenth,
                                TokenType.ClosingParenth,
                                ParseExpressionSyntax).Cast<IExpressionSyntax>().ToList();
                        expression = new InvocationExpressionSyntax(expression, invokeParameter);
                        break;
                    case TokenType.LeftSquareBracket:
                        var arrayParameter = new List<IExpressionSyntax>();
                        for (;
                            _tokenStream.Current.Type == TokenType.LeftSquareBracket;
                            Consume(TokenType.RightSquareBracket))
                        {
                            Consume(TokenType.LeftSquareBracket);
                            arrayParameter.Add(ParseExpressionSyntax());
                        }
                        expression = new ArrayAccessExpression(expression, arrayParameter);
                        break;
                }
            }


            return expression;
        }

        private IfElseExpressionSyntax ParseIfElseExpression()
        {
            Consume(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpressionSyntax).Single();
            var ifTrueExpression = ParseExpressionSyntax();
            Consume(TokenType.ElseKeyword);
            var ifFalseExpression = ParseExpressionSyntax();
            return new IfElseExpressionSyntax((IExpressionSyntax)condition, ifTrueExpression, ifFalseExpression);
        }

        private IExpressionSyntax ParseNewExpression()
        {
            if (_tokenStream.Current.Type != TokenType.NewKeyword)
            {
                return null;
            }

            Consume(TokenType.NewKeyword);
            var typeName = ParseSymbolOrBuildInType(true);
            var parameter = typeName is ArrayTypeSyntax ? null : ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpressionSyntax, true);
            return new ObjectCreationExpressionSyntax(typeName, parameter);
        }

        private FunctionSyntax ParseFunctionSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.FuncKeyword)
            {
                return null;
            }

            Consume(TokenType.FuncKeyword);
            var functionName = Consume(TokenType.Symbol);
            var functionParameter = ParseInnerCommmaSeperated(TokenType.OpenParenth, TokenType.ClosingParenth, _functionParameterParser);

            var hasReturnValue = _tokenStream.Current.Type == TokenType.HypenGreater;
            if (hasReturnValue)
            {
                return ParseFunctionThatReturnValue(functionName, functionParameter);
            }

            var body = ParseScope(_functionBodyParser);

            return new FunctionSyntax(functionName, functionParameter.Cast<ParameterSyntax>().ToList(), body);
        }

        private FunctionSyntax ParseFunctionThatReturnValue(Token functionName, List<ISyntaxBase> functionParameter)
        {
            Consume(TokenType.HypenGreater);

            if (_tokenStream.Current.Type == TokenType.DataKeyword)
            {
                return ParseDataFunctionSyntax(functionName, functionParameter);
            }

            var returnType = ParseSymbolOrBuildInType(false);
            var body = ParseScope(_functionBodyParser);
            return new ReturnFunctionSyntax(functionName, functionParameter.Cast<ParameterSyntax>().ToList(), body, returnType);
        }

        private FunctionSyntax ParseDataFunctionSyntax(Token functionName, List<ISyntaxBase> functionParameter)
        {
            var dataClass = ParseDataClassSyntax();
            var body = ParseScope(_functionBodyParser);
            return new DataClassFunctionSyntax(functionName, functionParameter.Cast<ParameterSyntax>().ToList(), body, dataClass);
        }

        private VariableDeclarationStatementSyntax ParseVariableDeclarationSyntax()
        {
            var current = _tokenStream.Current;
            Token variableQualifier;
            switch (current.Type)
            {
                case TokenType.VarKeyword:
                case TokenType.ConstKeyword:
                    Consume(current.Type);
                    variableQualifier = current;
                    break;
                default:
                    return null;
            }
            var variableNames = new List<Token>();
            for (variableNames.Add(Consume(TokenType.Symbol));
                 _tokenStream.Current.Type == TokenType.Comma;
                 variableNames.Add(Consume(TokenType.Symbol)))
            {
                Consume(TokenType.Comma);
            }
            Consume(TokenType.Colon);
            var initializer = ParseExpressionSyntax();
            return new VariableDeclarationStatementSyntax(variableQualifier, variableNames, initializer);
        }

        private ReturnStatementSyntax ParseReturnStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ReturnKeyword)
            {
                return null;
            }

            Consume(TokenType.ReturnKeyword);
            var returnExpression = ParseExpressionSyntax();
            return new ReturnStatementSyntax(returnExpression);
        }

        private DataSyntax ParseDataClassSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.DataKeyword)
            {
                return null;
            }

            Consume(TokenType.DataKeyword);
            var typeName = Consume(TokenType.Symbol);
            var parameter = ParseInnerCommmaSeperated(TokenType.OpenParenth, TokenType.ClosingParenth, _functionParameterParser);
            return new DataSyntax(typeName, parameter.Cast<ParameterSyntax>().ToList());
        }

        private ConstructorSyntax ParseConstructorSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ConstructorKeyword)
            {
                return null;
            }

            Consume(TokenType.ConstructorKeyword);

            var argList = ParseInnerCommmaSeperated(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                _functionParameterParser);

            var bodySyntax = ParseScope(_functionBodyParser);

            return new ConstructorSyntax(argList, bodySyntax);
        }

        private ForInStatementSyntax ParseForInStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ForKeyword)
            {
                return null;
            }
            _tokenStream.TakeSnapshot();

            Consume(TokenType.ForKeyword);
            IExpressionSyntax itemExpression;
            IExpressionSyntax collectionExpression;
            List<ISyntaxBase> body;
            bool declareItemInnerScope;
            if (!TryParseForIn(out itemExpression, out declareItemInnerScope, out collectionExpression, out body))
            {
                _tokenStream.RollbackSnapshot();
                return null;
            }
            return new ForInStatementSyntax(itemExpression, declareItemInnerScope, collectionExpression, body);
        }

        private ForInStatementSyntax ParseReverseForInStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ForReverseKeyword)
            {
                return null;
            }
            _tokenStream.TakeSnapshot();

            Consume(TokenType.ForReverseKeyword);
            IExpressionSyntax itemExpression;
            IExpressionSyntax collectionExpression;
            List<ISyntaxBase> body;
            bool declareItemInnerScope;
            if (!TryParseForIn(out itemExpression, out declareItemInnerScope, out collectionExpression, out body))
            {
                _tokenStream.RollbackSnapshot();
                return null;
            }
            return new ReverseForInStatementSyntax(itemExpression, declareItemInnerScope, collectionExpression, body);
        }

        private bool TryParseForIn(out IExpressionSyntax itemExpression, out bool declareItemInnerScope, out IExpressionSyntax collectionExpression, out List<ISyntaxBase> body)
        {
            Consume(TokenType.OpenParenth);

            declareItemInnerScope = _tokenStream.Current.Type == TokenType.VarKeyword;
            if (declareItemInnerScope)
            {
                Consume(TokenType.VarKeyword);
            }

            itemExpression = ParseExpressionSyntax();

            if (_tokenStream.Current.Type != TokenType.InKeyword)
            {
                collectionExpression = null;
                body = null;
                return false;
            }

            Consume(TokenType.InKeyword);
            collectionExpression = ParseExpressionSyntax();
            Consume(TokenType.ClosingParenth);
            body = ParseScope(_functionBodyParser);
            return true;
        }

        private ForStatementSyntax ParseForStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ForKeyword)
            {
                return null;
            }
            
            Consume(TokenType.ForKeyword);
            Consume(TokenType.OpenParenth);

            var initExpression = ParseForInitExpression();
            Consume(TokenType.Semicolon);
            var condExpression = ParseExpressionSyntax();
            Consume(TokenType.Semicolon);
            var loopExpression = ParseForInitExpression();
            Consume(TokenType.ClosingParenth);

            var hasScope = _tokenStream.Current.Type == TokenType.OpenBracket;
            List<ISyntaxBase> body;
            if (hasScope)
            {
                body = ParseScope(_functionBodyParser);
            }
            else
            {
                body = new List<ISyntaxBase>() { _functionBodyParser() };
            }

            return new ForStatementSyntax(initExpression, condExpression, loopExpression, body);
        }

        private ISyntaxBase ParseForInitExpression()
        {
            var result = Parse(
                ParseVariableDeclarationSyntax,
                ParseVariableAssignmentStatementSyntax,
                ParseExpressionSyntax);

            var allowedSyntax = new[]
                                {
                                    typeof(AssignmentStatementSyntax),
                                    typeof(VariableDeclarationStatementSyntax),
                                    typeof(CallExpressionSyntax),
                                    typeof(ObjectCreationExpressionSyntax)
                                };

            if (!allowedSyntax.Contains(result.GetType()))
            {
                throw new KiwiSyntaxException(
                    "Only assignment call increment decrement and new object expressions can be used.");
            }

            return result;
        }

        private AssignmentStatementSyntax ParseVariableAssignmentStatementSyntax()
        {
            var assignOperators = new[]
                                  {
                                      TokenType.Colon,
                                      TokenType.ColonDiv,
                                      TokenType.ColonMult,
                                      TokenType.ColonAdd,
                                      TokenType.ColonPow,
                                      TokenType.ColonSub
                                  };
            if (_tokenStream.Current.Type != TokenType.Symbol)
            {
                return null;
            }

            _tokenStream.TakeSnapshot();
            var member = ParseExpressionSyntax();

            if (!(member is MemberAccessExpressionSyntax || member is ArrayAccessExpression
                 || member is MemberExpressionSyntax))
            {
                _tokenStream.RollbackSnapshot();
                return null;
            }

            var @operator = _tokenStream.Current;
            if (!assignOperators.Contains(@operator.Type))
            {
                throw new KiwiSyntaxException("Unexpected assign operator {0}. Expected :, :+, :-, :/, :* or :^.");
            }
            Consume(@operator.Type);
            var intializer = ParseExpressionSyntax();
            return new AssignmentStatementSyntax(member, @operator, intializer);
        }   

        private SwitchStatementSyntax ParseSwitchStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.SwitchKeyword)
            {
                return null;
            }

            Consume(TokenType.SwitchKeyword);
            Consume(TokenType.OpenParenth);
            var condition = ParseExpressionSyntax();
            Consume(TokenType.ClosingParenth);
            var caseAndDefaultSyntax = ParseScope(() => Parse(ParseCase, ParseDefault));

            DefaultSyntax defaultSyntax;
            try
            {
                defaultSyntax = caseAndDefaultSyntax.SingleOrDefault(x => x is DefaultSyntax) as DefaultSyntax;
            }
            catch
            {
                throw new KiwiSyntaxException("Duplicate default label");
            }

            return new SwitchStatementSyntax(condition, caseAndDefaultSyntax.OfType<CaseSyntax>().ToList(), defaultSyntax);
        }

        private DefaultSyntax ParseDefault()
        {
            if (_tokenStream.Current.Type != TokenType.DefaultKeyword)
            {
                return null;
            }

            Consume(TokenType.DefaultKeyword);
            Consume(TokenType.HypenGreater);

            var hasScope = _tokenStream.Current.Type == TokenType.OpenBracket;
            List<ISyntaxBase> body;
            if (hasScope)
            {
                body = ParseScope(_functionBodyParser);
            }
            else
            {
                body = new List<ISyntaxBase>() { _functionBodyParser() };
            }

            return new DefaultSyntax(body);
        }

        private ISyntaxBase ParseCase()
        {
            if (_tokenStream.Current.Type != TokenType.CaseKeyword)
            {
                return null;
            }

            Consume(TokenType.CaseKeyword);
            var expression = ParseExpressionSyntax();
            Consume(TokenType.HypenGreater);

            var hasScope = _tokenStream.Current.Type == TokenType.OpenBracket;
            List<ISyntaxBase> body;
            if (hasScope)
            {
                body = ParseScope(_functionBodyParser);
            }
            else
            {
                body = new List<ISyntaxBase>() { _functionBodyParser() };
            }

            return new CaseSyntax(expression, body);
        }

        private ISyntaxBase ParseWhenStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.WhenKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private IfStatementSyntax ParseIfStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.IfKeyword)
            {
                return null;
            }

            Consume(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpressionSyntax);
            var body = ParseScope(_functionBodyParser);
            if (_tokenStream.Current.Type == TokenType.ElseKeyword)
            {
                return ParseIfElseStatementSyntax(condition, body);
            }
            return new IfStatementSyntax(condition, body.Cast<IStatetementSyntax>().ToList());
        }

        private IfElseStatementSyntax ParseIfElseStatementSyntax(List<ISyntaxBase> condition, List<ISyntaxBase> body)
        {
            Consume(TokenType.ElseKeyword);
            var elseBody = ParseScope(_functionBodyParser);
            return new IfElseStatementSyntax(condition, body.Cast<IStatetementSyntax>().ToList(), elseBody.Cast<IStatetementSyntax>().ToList());
        }

        private ParameterSyntax ParseParameterSyntax()
        {
            var typeName = ParseSymbolOrBuildInType(false);

            var parameterName = Consume(TokenType.Symbol);
            return new ParameterSyntax(typeName, parameterName);
        }

        private TypeSyntax ParseSymbolOrBuildInType(bool exceptArraySizes)
        {
            var buildInTypeNames = new[]
                                   {
                                       TokenType.IntKeyword,
                                       TokenType.FloatKeyword,
                                   };

            var current = _tokenStream.Current;
            Token typeName;
            if (buildInTypeNames.Contains(current.Type))
            {
                _tokenStream.Consume();
                typeName = current;
            }
            else
            {
                typeName = Consume(TokenType.Symbol);
            }

            if (_tokenStream.Current.Type != TokenType.LeftSquareBracket)
            {
                return new TypeSyntax(typeName);
            }

            int dimension = 0;
            var sizes = new List<IExpressionSyntax>();
            for (; _tokenStream.Current.Type == TokenType.LeftSquareBracket; dimension++)
            {
                Consume(TokenType.LeftSquareBracket);
                if (exceptArraySizes)
                {
                    sizes.Add(ParseExpressionSyntax());
                }
                Consume(TokenType.RightSquareBracket);
            }

            return new ArrayTypeSyntax(typeName, dimension, sizes);
        }

        private ParameterSyntax ParseParamsParameterSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.TwoDots)
            {
                return null;
            }

            Consume(TokenType.TwoDots);
            var typeName = ParseSymbolOrBuildInType(false);
            var parameterName = Consume(TokenType.Symbol);
            return new ParameterSyntax(typeName, parameterName);
        }

        private EnumSyntax ParseEnumSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.EnumKeyword)
            {
                return null;
            }

            Consume(TokenType.EnumKeyword);
            var enumName = Consume(TokenType.Symbol);
            var memberSyntax = ParseInnerCommmaSeperated(
                TokenType.OpenBracket,
                TokenType.ClosingBracket,
                ParseEnumMemberSyntax);
            return new EnumSyntax(enumName, memberSyntax);
        }

        private ISyntaxBase ParseEnumMemberSyntax()
        {
            var memberName = Consume(TokenType.Symbol);
            var hasExplicitValue = _tokenStream.Current.Type == TokenType.Colon;
            IExpressionSyntax initializer = null;
            if (hasExplicitValue)
            {
                Consume(TokenType.Colon);
                initializer = ParseExpressionSyntax();
            }
            return new EnumMemberSyntax(memberName, initializer);
        }

        private List<ISyntaxBase> ParseScope(Func<ISyntaxBase> parser)
        {
            return ParseInner(TokenType.OpenBracket, TokenType.ClosingBracket, parser);
        }

        private List<ISyntaxBase> ParseInnerCommmaSeperated(
            TokenType opening,
            TokenType closing,
            Func<ISyntaxBase> parser)
        {
            return ParseInner(opening, closing, parser, true);
        }

        private List<ISyntaxBase> ParseInner(TokenType opening, TokenType closing, Func<ISyntaxBase> parser, bool commaSeperated = false)
        {
            var innerSyntax = new List<ISyntaxBase>();
            Consume(opening);
            while (_tokenStream.Current.Type != closing)
            {
                var inner = parser();
                if (inner == null)
                {
                    throw new KiwiSyntaxException("Unexpected Syntax in Sope");
                }
                
                innerSyntax.Add(inner);
                if (_tokenStream.Current.Type != closing && commaSeperated)
                {
                    Consume(TokenType.Comma);
                }
            }

            Consume(closing);
            return innerSyntax;
        }

        private ISyntaxBase Parse(params Func<ISyntaxBase>[] possibleParseFunctions)
        {
            foreach (var parseFunction in possibleParseFunctions)
            {
                _tokenStream.TakeSnapshot();
                var result = parseFunction();
                if (result == null)
                {
                    _tokenStream.RollbackSnapshot();
                }
                else
                {
                    _tokenStream.CommitSnapshot();
                    return result;
                }
            }
            return null;
        }

        private Token Consume(TokenType tokenType)
        {
            var currentToken = _tokenStream.Current;
            if (currentToken.Type != tokenType)
            {
                throw new KiwiSyntaxException($"Unexepted TokenType {currentToken.Type}. Excepted {tokenType}");
            }
            _tokenStream.Consume();
            return currentToken;
        }
    }
}