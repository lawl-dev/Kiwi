using System.Linq;
using Kiwi.Lexer;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
        [TestCase(TokenType.Plus, "+")]
        [TestCase(TokenType.Sub, "-")]
        [TestCase(TokenType.Mult, "*")]
        [TestCase(TokenType.Div, "/")]
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
        [TestCase(TokenType.RepeatKeyword, "repeat")]
        [TestCase(TokenType.IfKeyword, "if")]
        [TestCase(TokenType.ElseKeyword, "else")]
        [TestCase(TokenType.LessHypen, "<-")]
        [TestCase(TokenType.Whitespace, " ")]
        [TestCase(TokenType.NewLine, "\r\n")]
        [TestCase(TokenType.Symbol, "MyVariableName")]
        [TestCase(TokenType.Int, "12312312")]
        [TestCase(TokenType.Float, "123123.111")]
        [TestCase(TokenType.Comment, "//Comment")]
        [TestCase(TokenType.Comment, "/*Dudiledu\r\nDada bu*/")]
        [TestCase(TokenType.String, "\"Hallo\"")]
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
                                       "    const FieldTypeSample fieldNameSample = \"Hallo\";" + "\r\n" +
                                       "    FieldTypeSample2 = 1 * 2 + 3 / 4;" + "\r\n" +
                                       "    func FunctionNameSample(TypeNameSample parameterNameSample, ..TypeNameSample paramsParameterName) -> TypeNameSample" + "\r\n"
                                       +
                                       "    {" + "\r\n" +
                                       "        return new TypeNameSample();" + "\r\n" +
                                       "    }" + "\r\n" +
                                       "" + "\r\n" +
                                       "    func FunctionNameSample(TypeNameSample parameterNameSample, ..TypeNameSample paramsParameterName) -> data ReturnDataSample(TypeNameSample parameterNameSample, int lol)"
                                       + "\r\n" +
                                       "    {" + "\r\n" +
                                       "        return new TypeNameSample(), 1;" + "\r\n" +
                                       "    }" + "\r\n"+
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
                                                     TokenType.Equal, 
                                                     TokenType.String, 
                                                     TokenType.Semicolon, 
                                                     TokenType.Symbol, 
                                                     TokenType.Equal, 
                                                     TokenType.Int, 
                                                     TokenType.Mult, 
                                                     TokenType.Int, 
                                                     TokenType.Plus,
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
                                                     TokenType.ClosingBracket,  
                                                 };
            ValidateLexerResults(classSource, tokenTypesSource);
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
