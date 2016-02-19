using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer.Strategies;

namespace Kiwi.Lexer
{
    public sealed class Lexer
    {
        private TransactableTokenStream _tokenStream;

        private readonly Dictionary<TokenType, string> _specialCharacters = new Dictionary<TokenType, string>()
                                                                   {
                                                                       {TokenType.OpenBracket, "{"},
                                                                       {TokenType.ClosingBracket, "}"},
                                                                       {TokenType.OpenParenth, "("},
                                                                       {TokenType.ClosingParenth, ")"},
                                                                       {TokenType.HypenGreater, "->"},
                                                                       {TokenType.LessHypen, "<-"},
                                                                       {TokenType.Semicolon, ";"},
                                                                       {TokenType.Comma, ","},
                                                                       {TokenType.TwoDots, ".."},
                                                                       {TokenType.EqualGreater, "=>"},
                                                                       {TokenType.Plus, "+"},
                                                                       {TokenType.Sub, "-"},
                                                                       {TokenType.Mult, "*"},
                                                                       {TokenType.Div, "/"},
                                                                       {TokenType.Pow, "^"},
                                                                       {TokenType.Dot, "."},
                                                                       {TokenType.Greater, ">"},
                                                                       {TokenType.Equal, "="},
                                                                       {TokenType.Less, "<"}
                                                                   };
        
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
            var keywords = new Dictionary<TokenType, string>()
                                                  {
                                                      {TokenType.Func, "func"},
                                                      {TokenType.Descriptor, "descriptor"},
                                                      {TokenType.Data, "data"},
                                                      {TokenType.IntKeyword, "int"},
                                                      {TokenType.FloatKeyword, "float"},
                                                      {TokenType.Const, "const"},
                                                      {TokenType.String, "string"},
                                                      {TokenType.Var, "var"},
                                                      {TokenType.Class, "class"},
                                                      {TokenType.Is, "is"},
                                                      {TokenType.Constructor, "Constructor"},
                                                      {TokenType.Return, "return"},
                                                      {TokenType.New, "new"},
                                                      {TokenType.Enum, "enum"},
                                                      {TokenType.When, "when"},
                                                      {TokenType.Switch, "switch"},
                                                      {TokenType.Default, "default"},
                                                      {TokenType.For, "for"},
                                                      {TokenType.In, "in"},
                                                      {TokenType.While, "while"},
                                                      {TokenType.Repeat, "repeat"},
                                                      {TokenType.If, "if"},
                                                      {TokenType.Else, "else"},
                                                  };

            var forbiddenSymbolNames = keywords.Select(x => x.Value).ToList();
            var keywordLexerStrategies = keywords.Select(keyword => new SyntaxLexerStrategy(keyword.Key, keyword.Value));
            var specialCharacterLexerStrategies = _specialCharacters.Select(character => new SyntaxLexerStrategy(character.Key, character.Value));

            var strategies = new List<TokenLexerStrategyBase>();
            strategies.Add(new SyntaxCommentLexerStrategy());
            strategies.Add(new SyntaxSymbolLexerStrategy(forbiddenSymbolNames));
            strategies.Add(new SyntaxLexerStrategy(TokenType.Whitespace, " "));
            strategies.Add(new SyntaxLexerStrategy(TokenType.NewLine, "\r\n"));
            strategies.AddRange(keywordLexerStrategies);
            strategies.AddRange(specialCharacterLexerStrategies);
            strategies.Add(new SyntaxFloatLexerStrategy());
            strategies.Add(new SyntaxIntegerLexerStrategy());
            return strategies;
        }
    }
}
