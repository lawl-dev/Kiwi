using System.Diagnostics;
using System.Linq.Expressions;
using Kiwi.Lexer;
using Kiwi.Parser;
using Kiwi.Parser.Nodes;
using NUnit.Framework;

namespace Kiwi.Tests
{
    public class ParserTests
    {
        private const string NamespaceSource = "namespace NameSpaceSample" + "\r\n" +
                                               "{{" + "\r\n" +
                                               "    {0}" + "\r\n" + //class placeholder
                                               "}}";

        private const string ClassSource = "class ClassSample is DSampleDescriptor" + "\r\n" +
                                           "{{" + "\r\n" +
                                           "    {0}" + "\r\n" + //constructor placeholder
                                           "    {1}" + "\r\n" + //field placeholder
                                           "    {2}" + "\r\n" + //function placeholder
                                           "}}";

        private const string FunctionSource = "func FunctionSample(int a, int b, int c) -> int" + "\r\n" +
                                              "{{" + "\r\n" +
                                              "     {0}" + "\r\n" + //statements placeholder
                                              "}}";

        [Test]
        public void TestNamespace()
        {
            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(string.Format(NamespaceSource, string.Empty));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsInstanceOf<CompilationUnitSyntax>(ast);
            Assert.IsInstanceOf<NamespaceSyntax>(ast.MemberSyntax[0]);
        }

        [Test]
        public void TestClass()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Empty, string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsInstanceOf<CompilationUnitSyntax>(ast);
            Assert.IsInstanceOf<NamespaceSyntax>(ast.MemberSyntax[0]);
            Assert.IsNotEmpty(((NamespaceSyntax)ast.MemberSyntax[0]).ClassMember);
            Assert.AreEqual("ClassSample", ((NamespaceSyntax)ast.MemberSyntax[0]).ClassMember[0].ClassName.Value);
            Assert.AreEqual("DSampleDescriptor", ((NamespaceSyntax)ast.MemberSyntax[0]).ClassMember[0].DescriptorName.Value);
        }

        [Test]
        public void TestFunction()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(FunctionSource, string.Empty), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsNotEmpty(ast.NamespaceMember[0].ClassMember[0].FunctionMember);
            Assert.AreEqual("FunctionSample", ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].FunctionName);
        }

        [Test]
        public void TestReturn()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(FunctionSource, "return 1"), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsInstanceOf<ReturnStatementSyntax>(ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].StatementMember[0]);
        }

        [Test]
        public void TestSimpleOperatorPrecedence()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(FunctionSource, "return 1 + 2 * 3"), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var returnStatementSyntax = (ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].StatementMember[0];
            Assert.IsInstanceOf<BinaryExpressionSyntax>(returnStatementSyntax.Expression);
            var binaryExpressionSyntax = (BinaryExpressionSyntax)returnStatementSyntax.Expression;
            Assert.IsInstanceOf<IntExpressionSyntax>(binaryExpressionSyntax.LeftExpression);
            Assert.IsInstanceOf<BinaryExpressionSyntax>(binaryExpressionSyntax.RightExpression);
            Assert.AreEqual(binaryExpressionSyntax.Operator.Type, TokenType.Add);
            var binaryExpressionSyntax2 = (BinaryExpressionSyntax)binaryExpressionSyntax.RightExpression;
            Assert.IsInstanceOf<IntExpressionSyntax>(binaryExpressionSyntax2.LeftExpression);
            Assert.IsInstanceOf<IntExpressionSyntax>(binaryExpressionSyntax2.RightExpression);
            Assert.AreEqual(binaryExpressionSyntax2.Operator.Type, TokenType.Mult);

        }
    }
}