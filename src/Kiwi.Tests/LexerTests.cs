using System.Linq;
using Kiwi.Lexer;
using NUnit.Framework;

namespace Kiwi.Tests
{
    public class LexerTests
    {
        [TestCase(TokenType.FuncKeyword, "func")]
        [TestCase(TokenType.DescriptorKeyword, "descriptor")]
        [TestCase(TokenType.DataKeyword, "data")]
        [TestCase(TokenType.IntKeyword, "int")]
        [TestCase(TokenType.FloatKeyword, "float")]
        [TestCase(TokenType.ConstKeyword, "const")]
        [TestCase(TokenType.CaseKeyword, "case")]
        [TestCase(TokenType.StringKeyword, "string")]
        [TestCase(TokenType.VarKeyword, "var")]
        [TestCase(TokenType.OpenBracket, "{")]
        [TestCase(TokenType.ClosingBracket, "}")]
        [TestCase(TokenType.OpenParenth, "(")]
        [TestCase(TokenType.ClosingParenth, ")")]
        [TestCase(TokenType.HypenGreater, "->")]
        [TestCase(TokenType.Semicolon, ";")]
        [TestCase(TokenType.Comma, ",")]
        [TestCase(TokenType.TwoDots, "..")]
        [TestCase(TokenType.ClassKeyword, "class")]
        [TestCase(TokenType.IsKeyword, "is")]
        [TestCase(TokenType.ConstructorKeyword, "Constructor")]
        [TestCase(TokenType.ReturnKeyword, "return")]
        [TestCase(TokenType.EqualGreater, "=>")]
        [TestCase(TokenType.Add, "+")]
        [TestCase(TokenType.Sub, "-")]
        [TestCase(TokenType.LeftSquareBracket, "[")]
        [TestCase(TokenType.RightSquareBracket, "]")]
        [TestCase(TokenType.Mult, "*")]
        [TestCase(TokenType.Div, "/")]
        [TestCase(TokenType.ColonAdd, ":+")]
        [TestCase(TokenType.Pow, "^")]
        [TestCase(TokenType.Dot, ".")]
        [TestCase(TokenType.NewKeyword, "new")]
        [TestCase(TokenType.EnumKeyword, "enum")]
        [TestCase(TokenType.WhenKeyword, "when")]
        [TestCase(TokenType.Equal, "=")]
        [TestCase(TokenType.Greater, ">")]
        [TestCase(TokenType.SwitchKeyword, "switch")]
        [TestCase(TokenType.DefaultKeyword, "default")]
        [TestCase(TokenType.ForKeyword, "for")]
        [TestCase(TokenType.InKeyword, "in")]
        [TestCase(TokenType.Less, "<")]
        [TestCase(TokenType.WhileKeyword, "while")]
        [TestCase(TokenType.UsingKeyword, "using")]
        [TestCase(TokenType.RepeatKeyword, "repeat")]
        [TestCase(TokenType.IfKeyword, "if")]
        [TestCase(TokenType.ElseKeyword, "else")]
        [TestCase(TokenType.Colon, ":")]
        [TestCase(TokenType.Whitespace, " ")]
        [TestCase(TokenType.NewLine, "\r\n")]
        [TestCase(TokenType.Symbol, "MyVariableName")]
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
        public void TestSampleDescriptor()
        {
            const string descriptorSource = "descriptor DescriptorSample" + "\r\n" +
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
                                       TokenType.DescriptorKeyword,
                                       TokenType.Symbol,
                                       TokenType.OpenBracket,
                                       TokenType.FuncKeyword,
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
                                       TokenType.FuncKeyword,
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
                                       TokenType.DataKeyword,
                                       TokenType.Symbol,
                                       TokenType.OpenParenth,
                                       TokenType.Symbol,
                                       TokenType.Symbol,
                                       TokenType.ClosingParenth,
                                       TokenType.Semicolon,
                                       TokenType.ClosingBracket
                                   };
            ValidateLexerResults(descriptorSource, tokenTypesSource);
        }

        [Test]
        public void TestSampleClass()
        {
            const string classSource = "class ClassNameSample is DescriptorNameSample" + "\r\n" +
                                       "{" + "\r\n" +
                                       "    const FieldTypeSample fieldNameSample : \"Hallo\";" + "\r\n" +
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
                                       TokenType.Symbol,
                                       TokenType.IsKeyword,
                                       TokenType.Symbol,
                                       TokenType.OpenBracket,
                                       TokenType.ConstKeyword,
                                       TokenType.Symbol,
                                       TokenType.Symbol,
                                       TokenType.Colon,
                                       TokenType.String,
                                       TokenType.Semicolon,
                                       TokenType.Symbol,
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
                                       TokenType.OpenBracket,
                                       TokenType.ReturnKeyword,
                                       TokenType.NewKeyword,
                                       TokenType.Symbol,
                                       TokenType.OpenParenth,
                                       TokenType.ClosingParenth,
                                       TokenType.Semicolon,
                                       TokenType.ClosingBracket,
                                       TokenType.FuncKeyword,
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
                                       TokenType.DataKeyword,
                                       TokenType.Symbol,
                                       TokenType.OpenParenth,
                                       TokenType.Symbol,
                                       TokenType.Symbol,
                                       TokenType.Comma,
                                       TokenType.IntKeyword,
                                       TokenType.Symbol,
                                       TokenType.ClosingParenth,
                                       TokenType.OpenBracket,
                                       TokenType.ReturnKeyword,
                                       TokenType.NewKeyword,
                                       TokenType.Symbol,
                                       TokenType.OpenParenth,
                                       TokenType.ClosingParenth,
                                       TokenType.Comma,
                                       TokenType.Int,
                                       TokenType.Semicolon,
                                       TokenType.ClosingBracket,
                                       TokenType.ClosingBracket
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
                                      TokenType.Symbol,
                                      TokenType.OpenBracket,
                                      TokenType.Symbol,
                                      TokenType.Comma,
                                      TokenType.Symbol,
                                      TokenType.Comma,
                                      TokenType.Symbol,
                                      TokenType.Colon,
                                      TokenType.Int,
                                      TokenType.ClosingBracket
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
                                      TokenType.Symbol,
                                      TokenType.InKeyword,
                                      TokenType.Symbol,
                                      TokenType.ClosingParenth,
                                      TokenType.OpenBracket,
                                      TokenType.Comment,
                                      TokenType.ClosingBracket
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
                                      TokenType.Symbol,
                                      TokenType.Colon,
                                      TokenType.Int,
                                      TokenType.Semicolon,
                                      TokenType.Symbol,
                                      TokenType.Less,
                                      TokenType.Int,
                                      TokenType.Semicolon,
                                      TokenType.Symbol,
                                      TokenType.ColonAdd,
                                      TokenType.Int,
                                      TokenType.ClosingParenth,
                                      TokenType.OpenBracket,
                                      TokenType.Comment,
                                      TokenType.ClosingBracket
                                  };
            ValidateLexerResults(forSource, tokenTypeSource);
        }

        [Test]
        public void TestWhen()
        {
            const string whenSource = "when(variableSample)" + "\r\n" +
                                      "{" + "\r\n" +
                                      "is TypeSample -> " + "\r\n" +
                                      "     {" + "\r\n" +
                                      "         //code" + "\r\n" +
                                      "     }" + "\r\n" +
                                      "1             -> //code" + "\r\n" +
                                      "2, 3, 4, 5    -> //code" + "\r\n" +
                                      "6..99         -> //code" + "\r\n" +
                                      "else          -> //code" + "\r\n" +
                                      "}";
            var tokenTypeSource = new[]
                                  {
                                      TokenType.WhenKeyword,
                                      TokenType.OpenParenth,
                                      TokenType.Symbol,
                                      TokenType.ClosingParenth,
                                      TokenType.OpenBracket,
                                      TokenType.IsKeyword,
                                      TokenType.Symbol,
                                      TokenType.HypenGreater,
                                      TokenType.OpenBracket,
                                      TokenType.Comment,
                                      TokenType.ClosingBracket,
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
                                      TokenType.ClosingBracket
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
                                        "   default -> //code" + "\r\n" +
                                        "}";
            var tokenTypeSource = new[]
                                  {
                                      TokenType.SwitchKeyword,
                                      TokenType.OpenParenth,
                                      TokenType.Symbol,
                                      TokenType.LeftSquareBracket,
                                      TokenType.Int,
                                      TokenType.RightSquareBracket,
                                      TokenType.ClosingParenth,
                                      TokenType.OpenBracket,
                                      TokenType.CaseKeyword,
                                      TokenType.Int,
                                      TokenType.HypenGreater,
                                      TokenType.OpenBracket,
                                      TokenType.Comment,
                                      TokenType.ClosingBracket,
                                      TokenType.CaseKeyword,
                                      TokenType.Int,
                                      TokenType.HypenGreater,
                                      TokenType.Comment,
                                      TokenType.DefaultKeyword,
                                      TokenType.HypenGreater,
                                      TokenType.Comment,
                                      TokenType.ClosingBracket
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
                                      TokenType.OpenBracket,
                                      TokenType.ClosingBracket
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
                      .Where(x => x != TokenType.NewLine)
                      .ToList();
            Assert.AreEqual(tokenizedSourceWithoutWhitespaceAndNewLine.Length, tokenTypes.Count);
            for (int i = 0; i < tokenTypes.Count; i++)
            {
                Assert.AreEqual(tokenTypes[i], tokenizedSourceWithoutWhitespaceAndNewLine[i]);
            }
        }
    }
}