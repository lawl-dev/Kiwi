using System.Linq;
using Kiwi.Lexer;
using NUnit.Framework;

namespace Kiwi.Tests
{
    public class LexerTests
    {
        [TestCase(TokenType.Func, "func")]
        [TestCase(TokenType.Descriptor, "descriptor")]
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
        [TestCase(TokenType.Sub, "-")]
        [TestCase(TokenType.Mult, "*")]
        [TestCase(TokenType.Div, "/")]
        [TestCase(TokenType.Pow, "^")]
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

        [Test]
        public void TestSampleDescriptor()
        {
            const string descriptorSource = "descriptor DescriptorSample" + "\r\n" +
                                            "{" + "\r\n" +
                                            "    func FunctionNameSample(TypeNameSample parameterNameSample, ..TypeNameSample paramsParameterName) -> TypeNameSample;" + "\r\n"
                                            +
                                            "    func FunctionNameSample(TypeNameSample parameterNameSample, ..TypeNameSample paramsParameterName) -> data ReturnDataSample(TypeNameSample parameterNameSample);" + "\r\n"
                                            +
                                            "}";

            var tokenTypesSource = new TokenType[]
                                                 {
                                                     TokenType.Descriptor,
                                                     TokenType.Symbol, 
                                                     TokenType.OpenBracket, 
                                                     TokenType.Func, 
                                                     TokenType.Symbol, 
                                                     TokenType.OpenParenth, 
                                                     TokenType.Symbol, 
                                                     TokenType.Symbol, 
                                                     TokenType.Comma, 
                                                     TokenType.TwoDots, 
                                                     TokenType.Symbol, 
                                                     TokenType.Symbol, 
                                                     TokenType.ClosingParenth, 
                                                     TokenType.HypenGreater, 
                                                     TokenType.Symbol, 
                                                     TokenType.Semicolon,
                                                     TokenType.Func,
                                                     TokenType.Symbol,
                                                     TokenType.OpenParenth,
                                                     TokenType.Symbol,
                                                     TokenType.Symbol,
                                                     TokenType.Comma,
                                                     TokenType.TwoDots,
                                                     TokenType.Symbol,
                                                     TokenType.Symbol,
                                                     TokenType.ClosingParenth,
                                                     TokenType.HypenGreater,
                                                     TokenType.Data,
                                                     TokenType.Symbol, 
                                                     TokenType.OpenParenth, 
                                                     TokenType.Symbol, 
                                                     TokenType.Symbol, 
                                                     TokenType.ClosingParenth, 
                                                     TokenType.Semicolon, 
                                                     TokenType.ClosingBracket
                                                 };
            var lexer = new Lexer.Lexer();
            var result = lexer.Lex(descriptorSource);
            var recoveredSource = string.Join("", result.Select(x => x.ToString()));
            Assert.AreEqual(descriptorSource, recoveredSource);
            var tokenTypes =
                result.Select(x => x.Type)
                      .Where(x => x != TokenType.Whitespace)
                      .Where(x => x != TokenType.NewLine)
                      .ToList();
            Assert.AreEqual(tokenTypesSource.Length, tokenTypes.Count);
            for (int i = 0; i < tokenTypes.Count; i++)
            {
                Assert.AreEqual(tokenTypes[i], tokenTypesSource[i]);
            }
        }
    }
}
