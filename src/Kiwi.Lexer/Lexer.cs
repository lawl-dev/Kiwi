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
                                                                       {TokenType.Dot, "."},
                                                                       {TokenType.Greater, ">"},
                                                                       {TokenType.Equal, "="},
                                                                       {TokenType.Less, "<"}
                                                                   };
        private readonly Dictionary<TokenType, string> _keywords = new Dictionary<TokenType, string>()
                                                  {
                                                      {TokenType.Func, "func"},
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
        public List<Token> Lex(string source)
        {
            _tokenStream = new TransactableTokenStream(source);

            var strategies = CreateLexerStrategies();

            return strategies.Where(x => x.IsMatch(_tokenStream)).Select(x => x.GetToken(_tokenStream)).ToList();
        }

        private List<TokenLexerStrategyBase> CreateLexerStrategies()
        {
            var strategies = new List<TokenLexerStrategyBase>();
            strategies.Add(new SyntaxSymbolLexerStrategy(_keywords.Select(x=>x.Value).ToList()));
            strategies.Add(new SyntaxLexerStrategy(TokenType.Whitespace, " "));
            strategies.Add(new SyntaxLexerStrategy(TokenType.NewLine, "\r\n"));
            strategies.AddRange(_keywords.Select(keyword => new SyntaxLexerStrategy(keyword.Key, keyword.Value)));
            strategies.AddRange(_specialCharacters.Select(character => new SyntaxLexerStrategy(character.Key, character.Value)));
            strategies.Add(new SyntaxFloatLexerStrategy());
            strategies.Add(new SyntaxIntegerLexerStrategy());
            strategies.Add(new SyntaxCommentLexerStrategy());
            return strategies;
        }
    }

    internal class SyntaxCommentLexerStrategy : TokenLexerStrategyBase
    {
        public override Token GetToken(TransactableTokenStream stream)
        {
            if (stream.Current != "/")
            {
                return null;
            }

            stream.TakeSnapshot();
            stream.Consume();
            string comment;
            switch (stream.Current)
            {
                case "/":
                    comment = LexSingleLineComment(stream);
                    break;
                case "*":
                    comment = LexMultiLineComment(stream);
                    break;
                default:
                    stream.RollbackSnapshot();
                    return null;
            }

            stream.CommitSnapshot();
            return new Token(TokenType.Comment, comment);
        }

        private static string LexMultiLineComment(TransactableTokenStream stream)
        {
            var comment = string.Empty;
            while (stream.Current != null && stream.Current != "*/")
            {
                comment += stream.Current;
                stream.Consume();
            }
            if (stream.Current == "*/")
            {
                comment += stream.Current;
                stream.Consume();
            }
            return comment;
        }

        private static string LexSingleLineComment(TransactableTokenStream stream)
        {
            var comment = string.Empty;
            while (stream.Current != null && stream.Current != "\r\n")
            {
                comment += stream.Current;
                stream.Consume();
            }
            return comment;
        }
    }
}
