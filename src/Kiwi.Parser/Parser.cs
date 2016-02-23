using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kiwi.Common;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class Parser
    {
        private readonly TransactableTokenStream _tokenStream;

        public Parser(List<Token> tokens)
        {
            _tokenStream = new TransactableTokenStream(tokens);
        }


        public SyntaxCompilationUnitAst Parse()
        {
            var results = new List<SyntaxAstBase>();
            while (_tokenStream.Current != null)
            {
                var result = ParsePossible(ParseUsingSyntax, ParseNamespaceSyntax);
                if (result == null)
                {
                    throw new KiwiSyntaxException("Unexpected Syntax. Expected UsingSyntax or NamespaceSyntax");
                }
                results.Add(result);
            }
            return new SyntaxCompilationUnitAst(results);
        }

        private SyntaxNamespaceAst ParseNamespaceSyntax()
        {
            List<SyntaxAstBase> innerSyntax = ParseInner(
                TokenType.OpenBracket,
                TokenType.ClosingBracket, () => ParsePossible(ParseClassSyntax, ParseDataClassDeclarationSyntax, ParseEnumSyntax));
            
            return new SyntaxNamespaceAst(innerSyntax);
        }

        

        private List<SyntaxAstBase> ParseInner(
            TokenType opening,
            TokenType closing,
            Func<SyntaxAstBase> getter)
        {
            return ParseInner(opening, closing, getter, false);
        }

        private List<SyntaxAstBase> ParseInnerCommmaSeperated(
            TokenType opening,
            TokenType closing,
            Func<SyntaxAstBase> getter)
        {
            return ParseInner(opening, closing, getter, true);
        }

        private List<SyntaxAstBase> ParseInner(TokenType opening, TokenType closing, Func<SyntaxAstBase> getter, bool commaSeperated)
        {
            var innerSyntax = new List<SyntaxAstBase>();
            Take(opening);
            while (_tokenStream.Current.Type != closing)
            {
                var inner = getter();
                if (inner == null)
                {
                    throw new KiwiSyntaxException("Unexpected Syntax in Sope");
                }

                innerSyntax.Add(inner);

                if (commaSeperated)
                {
                    Take(TokenType.Comma);
                }
            }

            Take(closing);
            return innerSyntax;
        }

        private Token Take(TokenType tokenType)
        {
            var currentToken = _tokenStream.Current;
            if (currentToken.Type != tokenType)
            {
                throw new KiwiSyntaxException($"Unexepted TokenType {currentToken.Type}. Excepted {tokenType}");
            }
            _tokenStream.Consume();
            return currentToken;
        }

        private SyntaxAstBase ParsePossible(params Func<SyntaxAstBase>[] possibleParseFunctions)
        {
            _tokenStream.TakeSnapshot();
            foreach (var parseFunction in possibleParseFunctions)
            {
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
            _tokenStream.RollbackSnapshot();
            return null;
        }

        private SyntaxUsingAst ParseUsingSyntax()
        {
            Take(TokenType.UsingKeyword);
            var namespaceName = Take(TokenType.Symbol);
            return new SyntaxUsingAst(namespaceName);
        }

        private SyntaxClassAst ParseClassSyntax()
        {
            Take(TokenType.ClassKeyword);
            var className = Take(TokenType.Symbol);
            var inherit = _tokenStream.Current.Type == TokenType.IsKeyword;
            Token descriptorName = null;

            if (inherit)
            {
                Take(TokenType.IsKeyword);
                descriptorName = Take(TokenType.Symbol);
            }
            
            var inner = ParseInner(
                TokenType.OpenBracket,
                TokenType.ClosingBracket,
                () => ParsePossible(ParseConstructorSyntax, ParseFunctionSyntax, ParseFieldSyntax));

            return new SyntaxClassAst(className, inherit, descriptorName, inner);
        }

        private SyntaxFieldAst ParseFieldSyntax()
        {
            var isConst = _tokenStream.Current.Type == TokenType.ConstKeyword;
            var fieldTypeQualifier = Take(isConst ? TokenType.ConstKeyword : TokenType.VarKeyword);
            var fieldType = Take(TokenType.Symbol);
            var fieldName = Take(TokenType.Symbol);
            SyntaxAstBase fieldInitializer = null;
            if (isConst)
            {
                Take(TokenType.Colon);
                fieldInitializer = ParseExpressionSyntax();
            }

            return new SyntaxFieldAst(fieldTypeQualifier, fieldType, fieldName, fieldInitializer);
        }

        private SyntaxAstBase ParseExpressionSyntax()
        {
            throw new NotImplementedException();
        }

        private SyntaxFunctionAst ParseFunctionSyntax()
        {
            Take(TokenType.FuncKeyword);
            var functionName = Take(TokenType.Symbol);
            var parameterList = ParseInnerCommmaSeperated(
                TokenType.OpenParenth,
                TokenType.ClosingParenth,
                () => ParsePossible(ParseParameterSyntax, ParseParamsParameterSyntax));

            var isVoid = _tokenStream.Current.Type != TokenType.HypenGreater;
            SyntaxAstBase dataClassDeclarationSyntax = null;
            Token returnTypeName = null;
            if (!isVoid)
            {
                Take(TokenType.HypenGreater);
                var withDataClassDeclaration = _tokenStream.Current.Type == TokenType.DataKeyword;
                if (withDataClassDeclaration)
                {
                    dataClassDeclarationSyntax = ParseDataClassDeclarationSyntax();
                }
                else
                {
                    returnTypeName = Take(TokenType.Symbol);
                }
            }
            var innerSyntax = ParseInner(
                TokenType.OpenBracket,
                TokenType.ClosingBracket,
                () => ParsePossible(
                    ParseIfStatementSyntax,
                    ParseWhenStatementSyntax,
                    ParseSwitchStatementSyntax,
                    ParseVariableStatementSyntax,
                    ParseForStatementSyntax,
                    ParseForInStatementSyntax));

            return new SyntaxFunctionAst(functionName, parameterList, isVoid, dataClassDeclarationSyntax, returnTypeName, innerSyntax);
        }

        private SyntaxAstBase ParseDataClassDeclarationSyntax()
        {
            throw new NotImplementedException();
        }

        private SyntaxConstructorAst ParseConstructorSyntax()
        {
            Take(TokenType.ConstructorKeyword);
            var argList = ParseInnerCommmaSeperated(TokenType.OpenParenth, TokenType.ClosingParenth, () => ParsePossible(ParseParameterSyntax, ParseParamsParameterSyntax));
            var innerSyntax = ParseInner(
                TokenType.OpenBracket,
                TokenType.ClosingBracket,
                () =>
                ParsePossible(
                    ParseIfStatementSyntax,
                    ParseWhenStatementSyntax,
                    ParseSwitchStatementSyntax,
                    ParseVariableStatementSyntax,
                    ParseForStatementSyntax,
                    ParseForInStatementSyntax));

            return new SyntaxConstructorAst(argList, innerSyntax);
        }

        private SyntaxAstBase ParseForInStatementSyntax()
        {
            throw new NotImplementedException();
        }

        private SyntaxAstBase ParseForStatementSyntax()
        {
            throw new NotImplementedException();
        }

        private SyntaxAstBase ParseVariableStatementSyntax()
        {
            throw new NotImplementedException();
        }

        private SyntaxAstBase ParseSwitchStatementSyntax()
        {
            throw new NotImplementedException();
        }

        private SyntaxAstBase ParseWhenStatementSyntax()
        {
            throw new NotImplementedException();
        }

        private SyntaxAstBase ParseIfStatementSyntax()
        {
            throw new NotImplementedException();
        }

        private SyntaxParameterAst ParseParameterSyntax()
        {
            var typeName = Take(TokenType.Symbol);
            var parameterName = Take(TokenType.Symbol);
            return new SyntaxParameterAst(typeName, parameterName);
        }

        private SyntaxParameterAst ParseParamsParameterSyntax()
        {
            Take(TokenType.TwoDots);
            var typeName = Take(TokenType.Symbol);
            var parameterName = Take(TokenType.Symbol);
            return new SyntaxParameterAst(typeName, parameterName);
        }

        private SyntaxEnumAst ParseEnumSyntax()
        {
            Take(TokenType.EnumKeyword);
            var enumName = Take(TokenType.Symbol);
            var memberSyntax = ParseInnerCommmaSeperated(TokenType.OpenBracket, TokenType.ClosingBracket, ParseEnumMemberSyntax);
            return new SyntaxEnumAst(enumName, memberSyntax);
        }

        private SyntaxAstBase ParseEnumMemberSyntax()
        {
            var memberName = Take(TokenType.Symbol);
            var hasExplicitValue = _tokenStream.Current.Type == TokenType.Colon;
            SyntaxAstBase initializer = null;
            if (hasExplicitValue)
            {
                Take(TokenType.Colon);
                initializer = ParseExpressionSyntax();
            }
            return new SyntaxEnumMemberAst(memberName, initializer);
        }
    }
}
