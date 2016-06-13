using System.Linq;
using Kiwi.Lexer;
using NUnit.Framework;

namespace Kiwi.Tests
{
    public class LexerTests
    {
        [TestCase(TokenType.FuncKeyword, "func")]
        [TestCase(TokenType.Operator, "operator")]
        [TestCase(TokenType.Primary, "primary")]
        [TestCase(TokenType.InfixKeyword, "infix")]
        [TestCase(TokenType.TrueKeyword, "true")]
        [TestCase(TokenType.FalseKeyword, "false")]
        [TestCase(TokenType.ProtocolKeyword, "protocol")]
        [TestCase(TokenType.DataKeyword, "data")]
        [TestCase(TokenType.IntKeyword, "int")]
        [TestCase(TokenType.FloatKeyword, "float")]
        [TestCase(TokenType.LetKeyword, "let")]
        [TestCase(TokenType.CaseKeyword, "case")]
        [TestCase(TokenType.StringKeyword, "string")]
        [TestCase(TokenType.VarKeyword, "var")]
        [TestCase(TokenType.OpenBrace, "{")]
        [TestCase(TokenType.ClosingBrace, "}")]
        [TestCase(TokenType.OpenParenth, "(")]
        [TestCase(TokenType.ClosingParenth, ")")]
        [TestCase(TokenType.HypenGreater, "->")]
        [TestCase(TokenType.Semicolon, ";")]
        [TestCase(TokenType.Comma, ",")]
        [TestCase(TokenType.TwoDots, "..")]
        [TestCase(TokenType.ClassKeyword, "class")]
        [TestCase(TokenType.IsKeyword, "is")]
        [TestCase(TokenType.AndKeyword, "&&")]
        [TestCase(TokenType.ConstructorKeyword, "Constructor")]
        [TestCase(TokenType.ReturnKeyword, "return")]
        [TestCase(TokenType.EqualGreater, "=>")]
        [TestCase(TokenType.Add, "+")]
        [TestCase(TokenType.Or, "||")]
        [TestCase(TokenType.Sub, "-")]
        [TestCase(TokenType.OpenBracket, "[")]
        [TestCase(TokenType.ClosingBracket, "]")]
        [TestCase(TokenType.Mult, "*")]
        [TestCase(TokenType.Div, "/")]
        [TestCase(TokenType.ColonAdd, ":+")]
        [TestCase(TokenType.Pow, "^")]
        [TestCase(TokenType.Dot, ".")]
        [TestCase(TokenType.NewKeyword, "new")]
        [TestCase(TokenType.EnumKeyword, "enum")]
        [TestCase(TokenType.MatchKeyword, "match")]
        [TestCase(TokenType.Equal, "=")]
        [TestCase(TokenType.NotEqual, "!=")]
        [TestCase(TokenType.Greater, ">")]
        [TestCase(TokenType.SwitchKeyword, "switch")]
        [TestCase(TokenType.ForKeyword, "for")]
        [TestCase(TokenType.ForReverseKeyword, "forr")]
        [TestCase(TokenType.InKeyword, "in")]
        [TestCase(TokenType.NotInKeyword, "!in")]
        [TestCase(TokenType.NotKeyword, "!")]
        [TestCase(TokenType.Less, "<")]
        [TestCase(TokenType.WhileKeyword, "while")]
        [TestCase(TokenType.UsingKeyword, "using")]
        [TestCase(TokenType.RepeatKeyword, "repeat")]
        [TestCase(TokenType.IfKeyword, "if")]
        [TestCase(TokenType.ElseKeyword, "else")]
        [TestCase(TokenType.Colon, ":")]
        [TestCase(TokenType.ColonSub, ":-")]
        [TestCase(TokenType.ColonDiv, ":/")]
        [TestCase(TokenType.ColonMult, ":*")]
        [TestCase(TokenType.ColonPow, ":^")]
        [TestCase(TokenType.Whitespace, " ")]
        [TestCase(TokenType.Tab, "	")]
        [TestCase(TokenType.NewLine, "\r\n")]
        [TestCase(TokenType.Identifier, "MyVariableName")]
        [TestCase(TokenType.Int, "12312312")]
        [TestCase(TokenType.Float, "123123f")]
        [TestCase(TokenType.Float, "123123F")]
        [TestCase(TokenType.Float, "123123.111f")]
        [TestCase(TokenType.Float, "123123.111F")]
        [TestCase(TokenType.Float, "123123.111")]
        [TestCase(TokenType.Comment, "//Comment")]
        [TestCase(TokenType.Comment, "/*Dudiledu\r\nDada bu*/")]
        [TestCase(TokenType.String, "\"Hallo\"")]
        public void TestSingleToken(TokenType expectedTokenType, string source)
        {
            var lexer = new Lexer.Lexer();
            var result = lexer.Lex(source);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(expectedTokenType, result[0].Type);
            Assert.AreEqual(source, result[0].Value);
        }

        [Test]
        public void TestSampleProtocol()
        {
            const string protocolSource = "protocol DescriptorSample" + "\r\n" +
                                            "{" + "\r\n" +
                                            "    func FunctionNameSample(TypeNameSample parameterNameSample, ..TypeNameSample paramsParameterName) -> TypeNameSample;"
                                            + "\r\n"
                                            +
                                            "    func FunctionNameSample(TypeNameSample parameterNameSample, ..TypeNameSample paramsParameterName) -> data ReturnDataSample(TypeNameSample parameterNameSample);"
                                            + "\r\n"
                                            +
                                            "}";

            var tokenTypesSource = new[]
                                   {
                                       TokenType.ProtocolKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenBrace,
                                       TokenType.FuncKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenParenth,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.Comma,
                                       TokenType.TwoDots,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.ClosingParenth,
                                       TokenType.HypenGreater,
                                       TokenType.Identifier,
                                       TokenType.Semicolon,
                                       TokenType.FuncKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenParenth,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.Comma,
                                       TokenType.TwoDots,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.ClosingParenth,
                                       TokenType.HypenGreater,
                                       TokenType.DataKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenParenth,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.ClosingParenth,
                                       TokenType.Semicolon,
                                       TokenType.ClosingBrace
                                   };
            ValidateLexerResults(protocolSource, tokenTypesSource);
        }

        [Test]
        public void TestSampleClass()
        {
            const string classSource = "class ClassNameSample is ProtocolNameSample" + "\r\n" +
                                       "{" + "\r\n" +
                                       "    let FieldTypeSample fieldNameSample : \"Hallo\";" + "\r\n" +
                                       "    FieldTypeSample2 = 1 * 2 + 3 / 4;" + "\r\n" +
                                       "    func FunctionNameSample(TypeNameSample parameterNameSample, ..TypeNameSample paramsParameterName) -> TypeNameSample"
                                       + "\r\n"
                                       +
                                       "    {" + "\r\n" +
                                       "        return new TypeNameSample();" + "\r\n" +
                                       "    }" + "\r\n" +
                                       "" + "\r\n" +
                                       "    func FunctionNameSample(TypeNameSample parameterNameSample, ..TypeNameSample paramsParameterName) -> data ReturnDataSample(TypeNameSample parameterNameSample, int lol)"
                                       + "\r\n" +
                                       "    {" + "\r\n" +
                                       "        return new TypeNameSample(), 1;" + "\r\n" +
                                       "    }" + "\r\n" +
                                       "}";
            var tokenTypesSource = new[]
                                   {
                                       TokenType.ClassKeyword,
                                       TokenType.Identifier,
                                       TokenType.IsKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenBrace,
                                       TokenType.LetKeyword,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.Colon,
                                       TokenType.String,
                                       TokenType.Semicolon,
                                       TokenType.Identifier,
                                       TokenType.Equal,
                                       TokenType.Int,
                                       TokenType.Mult,
                                       TokenType.Int,
                                       TokenType.Add,
                                       TokenType.Int,
                                       TokenType.Div,
                                       TokenType.Int,
                                       TokenType.Semicolon,
                                       TokenType.FuncKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenParenth,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.Comma,
                                       TokenType.TwoDots,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.ClosingParenth,
                                       TokenType.HypenGreater,
                                       TokenType.Identifier,
                                       TokenType.OpenBrace,
                                       TokenType.ReturnKeyword,
                                       TokenType.NewKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenParenth,
                                       TokenType.ClosingParenth,
                                       TokenType.Semicolon,
                                       TokenType.ClosingBrace,
                                       TokenType.FuncKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenParenth,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.Comma,
                                       TokenType.TwoDots,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.ClosingParenth,
                                       TokenType.HypenGreater,
                                       TokenType.DataKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenParenth,
                                       TokenType.Identifier,
                                       TokenType.Identifier,
                                       TokenType.Comma,
                                       TokenType.IntKeyword,
                                       TokenType.Identifier,
                                       TokenType.ClosingParenth,
                                       TokenType.OpenBrace,
                                       TokenType.ReturnKeyword,
                                       TokenType.NewKeyword,
                                       TokenType.Identifier,
                                       TokenType.OpenParenth,
                                       TokenType.ClosingParenth,
                                       TokenType.Comma,
                                       TokenType.Int,
                                       TokenType.Semicolon,
                                       TokenType.ClosingBrace,
                                       TokenType.ClosingBrace
                                   };
            ValidateLexerResults(classSource, tokenTypesSource);
        }

        [Test]
        public void TestEnum()
        {
            const string enumSource = "enum EnumSample" + "\r\n" +
                                      "{" + "\r\n" +
                                      "  First," + "\r\n" +
                                      "  Second," + "\r\n" +
                                      "  Last : 1337" + "\r\n" +
                                      "}";

            var tokenTypeSource = new[]
                                  {
                                      TokenType.EnumKeyword,
                                      TokenType.Identifier,
                                      TokenType.OpenBrace,
                                      TokenType.Identifier,
                                      TokenType.Comma,
                                      TokenType.Identifier,
                                      TokenType.Comma,
                                      TokenType.Identifier,
                                      TokenType.Colon,
                                      TokenType.Int,
                                      TokenType.ClosingBrace
                                  };

            ValidateLexerResults(enumSource, tokenTypeSource);
        }

        [Test]
        public void TestForIn()
        {
            const string forInSource = "for(i in ints)" + "\r\n" +
                                       "{" + "\r\n" +
                                       "    //code" + "\r\n" +
                                       "}";
            var tokenTypeSource = new[]
                                  {
                                      TokenType.ForKeyword,
                                      TokenType.OpenParenth,
                                      TokenType.Identifier,
                                      TokenType.InKeyword,
                                      TokenType.Identifier,
                                      TokenType.ClosingParenth,
                                      TokenType.OpenBrace,
                                      TokenType.Comment,
                                      TokenType.ClosingBrace
                                  };
            ValidateLexerResults(forInSource, tokenTypeSource);
        }

        [Test]
        public void TestFor()
        {
            const string forSource = "for(int i : 1; i < 100; i :+ 1)" + "\r\n" +
                                     "{" + "\r\n" +
                                     "  //code" + "\r\n" +
                                     "}";

            var tokenTypeSource = new[]
                                  {
                                      TokenType.ForKeyword,
                                      TokenType.OpenParenth,
                                      TokenType.IntKeyword,
                                      TokenType.Identifier,
                                      TokenType.Colon,
                                      TokenType.Int,
                                      TokenType.Semicolon,
                                      TokenType.Identifier,
                                      TokenType.Less,
                                      TokenType.Int,
                                      TokenType.Semicolon,
                                      TokenType.Identifier,
                                      TokenType.ColonAdd,
                                      TokenType.Int,
                                      TokenType.ClosingParenth,
                                      TokenType.OpenBrace,
                                      TokenType.Comment,
                                      TokenType.ClosingBrace
                                  };
            ValidateLexerResults(forSource, tokenTypeSource);
        }

        [Test]
        public void TestWhen()
        {
            const string whenSource = "match(variableSample)" + "\r\n" +
                                      "{" + "\r\n" +
                                      "case is TypeSample -> " + "\r\n" +
                                      "     {" + "\r\n" +
                                      "         //code" + "\r\n" +
                                      "     }" + "\r\n" +
                                      "case 1             -> //code" + "\r\n" +
                                      "case in 2, 3, 4, 5    -> //code" + "\r\n" +
                                      "case in 6..99         -> //code" + "\r\n" +
                                      "else          -> //code" + "\r\n" +
                                      "}";
            var tokenTypeSource = new[]
                                  {
                                      TokenType.MatchKeyword,
                                      TokenType.OpenParenth,
                                      TokenType.Identifier,
                                      TokenType.ClosingParenth,
                                      TokenType.OpenBrace,
                                      TokenType.IsKeyword,
                                      TokenType.Identifier,
                                      TokenType.HypenGreater,
                                      TokenType.OpenBrace,
                                      TokenType.Comment,
                                      TokenType.ClosingBrace,
                                      TokenType.Int,
                                      TokenType.HypenGreater,
                                      TokenType.Comment,
                                      TokenType.Int,
                                      TokenType.Comma,
                                      TokenType.Int,
                                      TokenType.Comma,
                                      TokenType.Int,
                                      TokenType.Comma,
                                      TokenType.Int,
                                      TokenType.HypenGreater,
                                      TokenType.Comment,
                                      TokenType.Int,
                                      TokenType.TwoDots,
                                      TokenType.Int,
                                      TokenType.HypenGreater,
                                      TokenType.Comment,
                                      TokenType.ElseKeyword,
                                      TokenType.HypenGreater,
                                      TokenType.Comment,
                                      TokenType.ClosingBrace
                                  };
            ValidateLexerResults(whenSource, tokenTypeSource);
        }

        [Test]
        public void TestSwitch()
        {
            const string swtichSource = "switch(result[0])" + "\r\n" +
                                        "{" + "\r\n" +
                                        "   case 1  ->" + "\r\n" +
                                        "       {" + "\r\n" +
                                        "           //code" + "\r\n" +
                                        "       }" + "\r\n" +
                                        "   case 2  -> //code" + "\r\n" +
                                        "   else -> //code" + "\r\n" +
                                        "}";
            var tokenTypeSource = new[]
                                  {
                                      TokenType.SwitchKeyword,
                                      TokenType.OpenParenth,
                                      TokenType.Identifier,
                                      TokenType.OpenBracket,
                                      TokenType.Int,
                                      TokenType.ClosingBracket,
                                      TokenType.ClosingParenth,
                                      TokenType.OpenBrace,
                                      TokenType.CaseKeyword,
                                      TokenType.Int,
                                      TokenType.HypenGreater,
                                      TokenType.OpenBrace,
                                      TokenType.Comment,
                                      TokenType.ClosingBrace,
                                      TokenType.CaseKeyword,
                                      TokenType.Int,
                                      TokenType.HypenGreater,
                                      TokenType.Comment,
                                      TokenType.ElseKeyword,
                                      TokenType.HypenGreater,
                                      TokenType.Comment,
                                      TokenType.ClosingBrace
                                  };

            ValidateLexerResults(swtichSource, tokenTypeSource);
        }

        [Test]
        public void TestNamespace()
        {
            const string namespaceSource = "namespace" + "\r\n" +
                                           "{" + "\r\n" +
                                           "}";
            var tokenTypeSource = new[]
                                  {
                                      TokenType.NamespaceKeyword,
                                      TokenType.OpenBrace,
                                      TokenType.ClosingBrace
                                  };

            ValidateLexerResults(namespaceSource, tokenTypeSource);
        }

        private static void ValidateLexerResults(string source, TokenType[] tokenizedSourceWithoutWhitespaceAndNewLine)
        {
            var lexer = new Lexer.Lexer();
            var result = lexer.Lex(source);
            var recoveredSource = string.Join("", result.Select(x => x.ToString()));
            Assert.AreEqual(source, recoveredSource);
            var tokenTypes =
                result.Select(x => x.Type)
                      .Where(x => x != TokenType.Whitespace)
                      .Where(x => x != TokenType.Tab)
                      .Where(x => x != TokenType.NewLine)
                      .ToList();
            Assert.AreEqual(tokenizedSourceWithoutWhitespaceAndNewLine.Length, tokenTypes.Count);
            for (var i = 0; i < tokenTypes.Count; i++)
            {
                Assert.AreEqual(tokenTypes[i], tokenizedSourceWithoutWhitespaceAndNewLine[i]);
            }
        }
    }
}