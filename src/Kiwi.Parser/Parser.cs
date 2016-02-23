using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Kiwi.Common;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class Parser
    {
        private readonly TransactableTokenStream _tokenStream;

        public Parser(List<Token> token)
        {
            var cleanedToken = token.Where(x => x.Type != TokenType.Whitespace)
                                    .Where(x => x.Type != TokenType.NewLine)
                                    .ToList();

            _tokenStream = new TransactableTokenStream(cleanedToken);
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

            Func<ISyntaxBase> namespaceBodyParser =
                () => Parse(ParseClassSyntax, ParseDataClassDeclarationSyntax, ParseEnumSyntax);
            var bodySyntax = ParseInner(TokenType.OpenBracket, TokenType.ClosingBracket, namespaceBodyParser);
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

            Func<ISyntaxBase> classBodyParser =
                () => Parse(ParseConstructorSyntax, ParseFunctionSyntax, ParseFieldSyntax);
            var inner = ParseInner(TokenType.OpenBracket, TokenType.ClosingBracket, classBodyParser);
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
                fieldInitializer = (IExpressionSyntax)ParseExpressionSyntax();
            }

            return new FieldSyntax(fieldTypeQualifier, fieldType, fieldName, fieldInitializer);
        }

        private IExpressionSyntax ParseExpressionSyntax()
        {
            var operators = new[]
                            {
                                TokenType.Mult,
                                TokenType.Div,
                                TokenType.Add,
                                TokenType.Sub,
                                TokenType.Pow,
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
                for (int index = 0; index < expressionOperatorChain.Count; index++)
                {
                    var expressionOrOperator = expressionOperatorChain[index];
                    if (expressionOrOperator.Item1 != null && expressionOrOperator.Item1.Type == @operator)
                    {
                        var leftExpression = expressionOperatorChain[index-1].Item2;
                        var rightExpression = expressionOperatorChain[index+1].Item2;

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

            if (signOperators.Contains(_tokenStream.Current.Type))
            {
                _tokenStream.Consume();
                return new SignExpressionSyntax(_tokenStream.Current, ParseExpressionSyntax());
            }

            if (_tokenStream.Current.Type == TokenType.NewKeyword)
            {
                return ParseNewExpression();
            }

            if (_tokenStream.Current.Type == TokenType.OpenParenth)
            {
                return (IExpressionSyntax)ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpressionSyntax).Single();
            }

            var current = _tokenStream.Current;
            switch (current.Type)
            {
                case TokenType.Int:
                    Consume(TokenType.Int);
                    return new IntExpressionSyntax(current);
                case TokenType.Float:
                    Consume(TokenType.Float);
                    return new FloatExpression(current);
                case TokenType.String:
                    Consume(TokenType.String);
                    return new StringExpression(current);
                case TokenType.Symbol:
                    Consume(TokenType.Symbol);
                    return new MemberAccessExpression(current);
                default:
                    throw new KiwiSyntaxException(
                        $"Unexpected Token {current}. Expected Sign Operator, New, Int, Float, String or Symbol Expression.");


            }
        }

        private IExpressionSyntax ParseNewExpression()
        {
            if (_tokenStream.Current.Type != TokenType.NewKeyword)
            {
                return null;
            }

            var typeName = Consume(TokenType.Symbol);
            var parameter = ParseInner(TokenType.OpenParenth, TokenType.ClosingParenth, ParseExpressionSyntax, true);
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

            Func<ISyntaxBase> functionParameterParser = () => Parse(ParseParameterSyntax, ParseParamsParameterSyntax);
            var parameterList = ParseInnerCommmaSeperated(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                functionParameterParser);

            var isVoid = _tokenStream.Current.Type != TokenType.HypenGreater;
            ISyntaxBase dataClassDeclarationSyntax = null;
            Token returnTypeName = null;
            if (!isVoid)
            {
                Consume(TokenType.HypenGreater);
                var withDataClassDeclaration = _tokenStream.Current.Type == TokenType.DataKeyword;
                if (withDataClassDeclaration)
                {
                    dataClassDeclarationSyntax = ParseDataClassDeclarationSyntax();
                }
                else
                {
                    returnTypeName = ParseSymbolOrBuildInTypeName();
                }
            }

            Func<ISyntaxBase> functionBodyParser = () => Parse(
                ParseReturnStatementSyntax,
                ParseIfStatementSyntax,
                ParseWhenStatementSyntax,
                ParseSwitchStatementSyntax,
                ParseVariableStatementSyntax,
                ParseForStatementSyntax,
                ParseForInStatementSyntax);

            var functionBody = ParseInner(TokenType.OpenBracket, TokenType.ClosingBracket, functionBodyParser);
            return new FunctionSyntax(
                functionName,
                parameterList,
                dataClassDeclarationSyntax,
                returnTypeName,
                functionBody);
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

        private ISyntaxBase ParseDataClassDeclarationSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.DataKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private ConstructorSyntax ParseConstructorSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ConstructorKeyword)
            {
                return null;
            }

            Consume(TokenType.ConstructorKeyword);

            Func<ISyntaxBase> constructorParameterParser = () => Parse(ParseParameterSyntax, ParseParamsParameterSyntax);
            var argList = ParseInnerCommmaSeperated(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                constructorParameterParser);

            Func<ISyntaxBase> constructorBodyParser = () =>
                                                     Parse(
                                                         ParseIfStatementSyntax,
                                                         ParseWhenStatementSyntax,
                                                         ParseSwitchStatementSyntax,
                                                         ParseVariableStatementSyntax,
                                                         ParseForStatementSyntax,
                                                         ParseForInStatementSyntax);
            var bodySyntax = ParseInner(
                TokenType.OpenBracket,
                TokenType.ClosingBracket,
                constructorBodyParser);

            return new ConstructorSyntax(argList, bodySyntax);
        }

        private ISyntaxBase ParseForInStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ForKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private ISyntaxBase ParseForStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.ForKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private ISyntaxBase ParseVariableStatementSyntax()
        {
            throw new NotImplementedException();
        }

        private ISyntaxBase ParseSwitchStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.SwitchKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private ISyntaxBase ParseWhenStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.WhenKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private ISyntaxBase ParseIfStatementSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.IfKeyword)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        private ParameterSyntax ParseParameterSyntax()
        {
            var typeName = ParseSymbolOrBuildInTypeName();

            var parameterName = Consume(TokenType.Symbol);
            return new ParameterSyntax(typeName, parameterName);
        }

        private Token ParseSymbolOrBuildInTypeName()
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
            return typeName;
        }

        private ParameterSyntax ParseParamsParameterSyntax()
        {
            if (_tokenStream.Current.Type != TokenType.TwoDots)
            {
                return null;
            }

            Consume(TokenType.TwoDots);
            var typeName = Consume(TokenType.Symbol);
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
                initializer = (IExpressionSyntax)ParseExpressionSyntax();
            }
            return new EnumMemberSyntax(memberName, initializer);
        }

        private List<ISyntaxBase> ParseInner(TokenType opening, TokenType closing, Func<ISyntaxBase> parser)
        {
            return ParseInner(opening, closing, parser, false);
        }

        private List<ISyntaxBase> ParseInnerCommmaSeperated(
            TokenType opening,
            TokenType closing,
            Func<ISyntaxBase> parser)
        {
            return ParseInner(opening, closing, parser, true);
        }

        private List<ISyntaxBase> ParseInner(
            TokenType opening,
            TokenType closing,
            Func<ISyntaxBase> parser,
            bool commaSeperated)
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

    internal class ObjectCreationExpressionSyntax : IExpressionSyntax
    {
        public Token TypeName { get; }
        public List<ISyntaxBase> Parameter { get; }

        public ObjectCreationExpressionSyntax(Token typeName, List<ISyntaxBase> parameter)
        {
            TypeName = typeName;
            Parameter = parameter;
        }
    }
}