﻿using System.Collections.Generic;
using System.Linq;
using Kiwi.Common;
using Kiwi.Lexer.Strategies;

namespace Kiwi.Lexer
{
    public sealed class Lexer
    {
        private TransactableTokenStream _tokenStream;

        public List<Token> Lex(string source)
        {
            var result = new List<Token>();

            _tokenStream = new TransactableTokenStream(source);

            var strategies = CreateLexerStrategies();

            while (_tokenStream.Current != null)
            {
                var strategy = strategies.FirstOrDefault(x => x.IsMatch(_tokenStream));
                if (strategy == null)
                {
                    throw new KiwiSyntaxException($"Unexpected Token {_tokenStream.Current}");
                }
                result.Add(strategy.GetToken(_tokenStream));
            }

            return result;
        }

        private List<TokenLexerStrategyBase> CreateLexerStrategies()
        {
            var keywords = new Dictionary<TokenType, string>
                           {
                               { TokenType.UsingKeyword, "using" },
                               { TokenType.NamespaceKeyword, "namespace" },
                               { TokenType.FuncKeyword, "func" },
                               { TokenType.DescriptorKeyword, "descriptor" },
                               { TokenType.DataKeyword, "data" },
                               { TokenType.IntKeyword, "int" },
                               { TokenType.FloatKeyword, "float" },
                               { TokenType.ConstKeyword, "const" },
                               { TokenType.CaseKeyword, "case" },
                               { TokenType.StringKeyword, "string" },
                               { TokenType.VarKeyword, "var" },
                               { TokenType.ClassKeyword, "class" },
                               { TokenType.IsKeyword, "is" },
                               { TokenType.ConstructorKeyword, "Constructor" },
                               { TokenType.ReturnKeyword, "return" },
                               { TokenType.NewKeyword, "new" },
                               { TokenType.EnumKeyword, "enum" },
                               { TokenType.WhenKeyword, "when" },
                               { TokenType.SwitchKeyword, "switch" },
                               { TokenType.DefaultKeyword, "default" },
                               { TokenType.ForReverseKeyword, "forr" },
                               { TokenType.ForKeyword, "for" },
                               { TokenType.InKeyword, "in" },
                               { TokenType.WhileKeyword, "while" },
                               { TokenType.RepeatKeyword, "repeat" },
                               { TokenType.IfKeyword, "if" },
                               { TokenType.TrueKeyword, "true" },
                               { TokenType.FalseKeyword, "false" },
                               { TokenType.ElseKeyword, "else" }
                           };
            var specialCharacters = new Dictionary<TokenType, string>
                                    {
                                        { TokenType.LeftSquareBracket, "[" },
                                        { TokenType.RightSquareBracket, "]" },
                                        { TokenType.OpenBracket, "{" },
                                        { TokenType.ClosingBracket, "}" },
                                        { TokenType.OpenParenth, "(" },
                                        { TokenType.ClosingParenth, ")" },
                                        { TokenType.HypenGreater, "->" },
                                        { TokenType.Or, "||" },
                                        { TokenType.ColonAdd, ":+" },
                                        { TokenType.ColonSub, ":-" },
                                        { TokenType.ColonDiv, ":/" },
                                        { TokenType.ColonMult, ":*" },
                                        { TokenType.ColonPow, ":^" },
                                        { TokenType.Colon, ":" },
                                        { TokenType.Semicolon, ";" },
                                        { TokenType.Comma, "," },
                                        { TokenType.TwoDots, ".." },
                                        { TokenType.EqualGreater, "=>" },
                                        { TokenType.Add, "+" },
                                        { TokenType.Sub, "-" },
                                        { TokenType.Mult, "*" },
                                        { TokenType.Div, "/" },
                                        { TokenType.Pow, "^" },
                                        { TokenType.Dot, "." },
                                        { TokenType.Greater, ">" },
                                        { TokenType.Equal, "=" },
                                        { TokenType.Less, "<" }
                                    };

            var forbiddenSymbolNames = keywords.Select(x => x.Value).ToList();
            var keywordLexerStrategies = keywords.Select(keyword => new SyntaxLexerStrategy(keyword.Key, keyword.Value));
            var specialCharacterLexerStrategies =
                specialCharacters.Select(character => new SyntaxLexerStrategy(character.Key, character.Value));

            var strategies = new List<TokenLexerStrategyBase>();
            strategies.Add(new SyntaxCommentLexerStrategy());
            strategies.Add(new SyntaxSymbolLexerStrategy(forbiddenSymbolNames));
            strategies.Add(new SyntaxLexerStrategy(TokenType.Whitespace, " "));
            strategies.Add(new SyntaxLexerStrategy(TokenType.Tab, "\t"));
            strategies.Add(new SyntaxLexerStrategy(TokenType.NewLine, "\r\n"));
            strategies.AddRange(keywordLexerStrategies);
            strategies.AddRange(specialCharacterLexerStrategies);
            strategies.Add(new SyntaxFloatLexerStrategy());
            strategies.Add(new SyntaxIntegerLexerStrategy());
            strategies.Add(new SyntaxStringLexerStrategy());
            return strategies;
        }
    }
}