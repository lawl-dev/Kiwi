using Kiwi.Lexer;
using NUnit.Framework;

namespace Kiwi.Tests
{
    public class LexerTests
    {
        [TestCase(TokenType.Func, "func")]
        [TestCase(TokenType.Data, "data")]
        [TestCase(TokenType.IntKeyword, "int")]
        [TestCase(TokenType.FloatKeyword, "float")]
        [TestCase(TokenType.Const, "const")]
        [TestCase(TokenType.String, "string")]
        [TestCase(TokenType.Var, "var")]
        [TestCase(TokenType.OpenBracket, "{")]
        [TestCase(TokenType.ClosingBracket, "}")]
        [TestCase(TokenType.OpenParenth, "(")]
        [TestCase(TokenType.ClosingParenth, ")")]
        [TestCase(TokenType.HypenGreater, "->")]
        [TestCase(TokenType.Semicolon, ";")]
        [TestCase(TokenType.Comma, ",")]
        [TestCase(TokenType.TwoDots, "..")]
        [TestCase(TokenType.Class, "class")]
        [TestCase(TokenType.Is, "is")]
        [TestCase(TokenType.Constructor, "Constructor")]
        [TestCase(TokenType.Return, "return")]
        [TestCase(TokenType.EqualGreater, "=>")]
        [TestCase(TokenType.Plus, "+")]
        [TestCase(TokenType.Dot, ".")]
        [TestCase(TokenType.New, "new")]
        [TestCase(TokenType.Enum, "enum")]
        [TestCase(TokenType.When, "when")]
        [TestCase(TokenType.Equal, "=")]
        [TestCase(TokenType.Greater, ">")]
        [TestCase(TokenType.Switch, "switch")]
        [TestCase(TokenType.Default, "default")]
        [TestCase(TokenType.For, "for")]
        [TestCase(TokenType.In, "in")]
        [TestCase(TokenType.Less, "<")]
        [TestCase(TokenType.While, "while")]
        [TestCase(TokenType.Repeat, "repeat")]
        [TestCase(TokenType.If, "if")]
        [TestCase(TokenType.Else, "else")]
        [TestCase(TokenType.LessHypen, "<-")]
        [TestCase(TokenType.Whitespace, " ")]
        [TestCase(TokenType.NewLine, "\r\n")]
        [TestCase(TokenType.Symbol, "MyVariableName")]
        [TestCase(TokenType.Int, "12312312")]
        [TestCase(TokenType.Float, "123123.111")]
        [TestCase(TokenType.Comment, "//Comment")]
        [TestCase(TokenType.Comment, "/*Dudiledu\r\nDada bu*/")]
        public void TestSingleTokenTypes(TokenType expectedTokenType, string source)
        {
            var lexer = new Lexer.Lexer();
            var result = lexer.Lex(source);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(expectedTokenType, result[0].Type);
        }
    }
}
