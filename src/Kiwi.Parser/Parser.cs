using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiwi.Common;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class Parser : ParserBase
    {
        private Func<ISyntaxBase> _namespaceBodyParser;
        private Func<ISyntaxBase> _classBodyParser;
        private Func<ISyntaxBase> _functionParameterParser;
        private Func<ISyntaxBase> _functionBodyParser;

        public Parser(List<Token> token) : base(new TransactableTokenStream(PrepareTokenSource(token)))
        {
            InitParser();
        }

        private static List<Token> PrepareTokenSource(List<Token> token)
        {
            return token.Where(x => x.Type != TokenType.Whitespace)
                        .Where(x => x.Type != TokenType.Tab)
                        .Where(x => x.Type != TokenType.NewLine)
                        .Where(x => x.Type != TokenType.Comment)
                        .ToList();
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
                ParseAssignmentStatementSyntax,
                ParseForInStatementSyntax,
                ParseReverseForInStatementSyntax,
                ParseForStatementSyntax);
        }

        public CompilationUnitSyntax Parse()
        {
            var syntax = new List<ISyntaxBase>();
            while (TokenStream.Current != null)
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
            if (TokenStream.Current.Type != TokenType.NamespaceKeyword)
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
            if (TokenStream.Current.Type != TokenType.UsingKeyword)
            {
                return null;
            }

            Consume(TokenType.UsingKeyword);
            var namespaceName = Consume(TokenType.Symbol);
            return new UsingSyntax(namespaceName);
        }

        private ClassSyntax ParseClassSyntax()
        {
            if (TokenStream.Current.Type != TokenType.ClassKeyword)
            {
                return null;
            }

            Consume(TokenType.ClassKeyword);
            var className = Consume(TokenType.Symbol);
            var inherit = TokenStream.Current.Type == TokenType.IsKeyword;
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
            if (TokenStream.Current.Type != TokenType.ConstKeyword && TokenStream.Current.Type != TokenType.VarKeyword)
            {
                return null;
            }

            var isConst = TokenStream.Current.Type == TokenType.ConstKeyword;
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
            
            if (!operators.Contains(TokenStream.Current.Type))
            {
                return firstExpression;
            }

            var expressionOperatorChain = new List<Tuple<Token, IExpressionSyntax>>();
            expressionOperatorChain.Add(new Tuple<Token, IExpressionSyntax>(null, firstExpression));
            while (operators.Contains(TokenStream.Current.Type))
            {
                var @operator = TokenStream.Current;
                expressionOperatorChain.Add(new Tuple<Token, IExpressionSyntax>(@operator, null));
                TokenStream.Consume();
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
            
            var current = TokenStream.Current;

            if (signOperators.Contains(current.Type))
            {
                TokenStream.Consume();
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
                    if (TokenStream.Current.Type == TokenType.TwoDots)
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
            while (memberOperators.Contains(TokenStream.Current.Type))
            {
                switch (TokenStream.Current.Type)
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
                        while(TokenStream.Current.Type == TokenType.LeftSquareBracket)
                        {
                            Consume(TokenType.LeftSquareBracket);
                            arrayParameter.Add(ParseExpressionSyntax());
                            Consume(TokenType.RightSquareBracket);
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
            if (TokenStream.Current.Type != TokenType.NewKeyword)
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
            if (TokenStream.Current.Type != TokenType.FuncKeyword)
            {
                return null;
            }

            Consume(TokenType.FuncKeyword);
            var functionName = Consume(TokenType.Symbol);
            var functionParameter = ParseInnerCommmaSeperated(TokenType.OpenParenth, TokenType.ClosingParenth, _functionParameterParser);

            var hasReturnValue = TokenStream.Current.Type == TokenType.HypenGreater;
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

            if (TokenStream.Current.Type == TokenType.DataKeyword)
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
                    return null;
            }
            var variableNames = new List<Token>();
            for (variableNames.Add(Consume(TokenType.Symbol));
                 TokenStream.Current.Type == TokenType.Comma;
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
            if (TokenStream.Current.Type != TokenType.ReturnKeyword)
            {
                return null;
            }

            Consume(TokenType.ReturnKeyword);
            var returnExpression = ParseExpressionSyntax();
            return new ReturnStatementSyntax(returnExpression);
        }

        private DataSyntax ParseDataClassSyntax()
        {
            if (TokenStream.Current.Type != TokenType.DataKeyword)
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
            if (TokenStream.Current.Type != TokenType.ConstructorKeyword)
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
            if (TokenStream.Current.Type != TokenType.ForKeyword)
            {
                return null;
            }
            TokenStream.TakeSnapshot();

            Consume(TokenType.ForKeyword);
            IExpressionSyntax itemExpression;
            IExpressionSyntax collectionExpression;
            List<ISyntaxBase> body;
            bool declareItemInnerScope;
            if (!TryParseForIn(out itemExpression, out declareItemInnerScope, out collectionExpression, out body))
            {
                TokenStream.RollbackSnapshot();
                return null;
            }
            return new ForInStatementSyntax(itemExpression, declareItemInnerScope, collectionExpression, body);
        }

        private ForInStatementSyntax ParseReverseForInStatementSyntax()
        {
            if (TokenStream.Current.Type != TokenType.ForReverseKeyword)
            {
                return null;
            }
            TokenStream.TakeSnapshot();

            Consume(TokenType.ForReverseKeyword);
            IExpressionSyntax itemExpression;
            IExpressionSyntax collectionExpression;
            List<ISyntaxBase> body;
            bool declareItemInnerScope;
            if (!TryParseForIn(out itemExpression, out declareItemInnerScope, out collectionExpression, out body))
            {
                TokenStream.RollbackSnapshot();
                return null;
            }
            return new ReverseForInStatementSyntax(itemExpression, declareItemInnerScope, collectionExpression, body);
        }

        private bool TryParseForIn(out IExpressionSyntax itemExpression, out bool declareItemInnerScope, out IExpressionSyntax collectionExpression, out List<ISyntaxBase> body)
        {
            Consume(TokenType.OpenParenth);

            declareItemInnerScope = TokenStream.Current.Type == TokenType.VarKeyword;
            if (declareItemInnerScope)
            {
                Consume(TokenType.VarKeyword);
            }

            itemExpression = ParseExpressionSyntax();

            if (TokenStream.Current.Type != TokenType.InKeyword)
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
            if (TokenStream.Current.Type != TokenType.ForKeyword)
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

            var hasScope = TokenStream.Current.Type == TokenType.OpenBracket;
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
                ParseAssignmentStatementSyntax,
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

        private AssignmentStatementSyntax ParseAssignmentStatementSyntax()
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
            if (TokenStream.Current.Type != TokenType.Symbol)
            {
                return null;
            }

            TokenStream.TakeSnapshot();
            var member = ParseExpressionSyntax();

            if (!(member is MemberAccessExpressionSyntax || member is ArrayAccessExpression
                 || member is MemberExpressionSyntax))
            {
                TokenStream.RollbackSnapshot();
                return null;
            }

            var @operator = TokenStream.Current;
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
            if (TokenStream.Current.Type != TokenType.SwitchKeyword)
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
            if (TokenStream.Current.Type != TokenType.DefaultKeyword)
            {
                return null;
            }

            Consume(TokenType.DefaultKeyword);
            Consume(TokenType.HypenGreater);

            var hasScope = TokenStream.Current.Type == TokenType.OpenBracket;
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
            if (TokenStream.Current.Type != TokenType.CaseKeyword)
            {
                return null;
            }

            Consume(TokenType.CaseKeyword);
            var expression = ParseExpressionSyntax();
            Consume(TokenType.HypenGreater);

            var hasScope = TokenStream.Current.Type == TokenType.OpenBracket;
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
            if (TokenStream.Current.Type != TokenType.WhenKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private IfStatementSyntax ParseIfStatementSyntax()
        {
            if (TokenStream.Current.Type != TokenType.IfKeyword)
            {
                return null;
            }

            Consume(TokenType.IfKeyword);
            var condition = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpressionSyntax);
            var body = ParseScope(_functionBodyParser);
            if (TokenStream.Current.Type == TokenType.ElseKeyword)
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

            var current = TokenStream.Current;
            Token typeName;
            if (buildInTypeNames.Contains(current.Type))
            {
                TokenStream.Consume();
                typeName = current;
            }
            else
            {
                typeName = Consume(TokenType.Symbol);
            }

            if (TokenStream.Current.Type != TokenType.LeftSquareBracket)
            {
                return new TypeSyntax(typeName);
            }

            int dimension = 0;
            var sizes = new List<IExpressionSyntax>();
            for (; TokenStream.Current.Type == TokenType.LeftSquareBracket; dimension++)
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
            if (TokenStream.Current.Type != TokenType.TwoDots)
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
            if (TokenStream.Current.Type != TokenType.EnumKeyword)
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
            var hasExplicitValue = TokenStream.Current.Type == TokenType.Colon;
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
    }
}