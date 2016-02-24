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
            Assert.AreEqual("FunctionSample", ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].FunctionName.Value);
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
            var leftBinaryExpression = (BinaryExpressionSyntax)returnStatementSyntax.Expression;

            Assert.IsInstanceOf<IntExpressionSyntax>(leftBinaryExpression.LeftExpression);
            Assert.IsInstanceOf<BinaryExpressionSyntax>(leftBinaryExpression.RightExpression);
            Assert.AreEqual(leftBinaryExpression.Operator.Type, TokenType.Add);

            var rightBinaryExpression = (BinaryExpressionSyntax)leftBinaryExpression.RightExpression;
            Assert.IsInstanceOf<IntExpressionSyntax>(rightBinaryExpression.LeftExpression);
            Assert.IsInstanceOf<IntExpressionSyntax>(rightBinaryExpression.RightExpression);

            Assert.AreEqual(rightBinaryExpression.Operator.Type, TokenType.Mult);
        }

        [Test]
        public void TestSignOperator()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(FunctionSource, "return +1 + 2 * -3"), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var returnStatementSyntax = (ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].StatementMember[0];

            Assert.IsInstanceOf<BinaryExpressionSyntax>(returnStatementSyntax.Expression);
            var addBinaryExpression = (BinaryExpressionSyntax)returnStatementSyntax.Expression;
            Assert.AreEqual(addBinaryExpression.Operator.Type, TokenType.Add);
            Assert.IsInstanceOf<SignExpressionSyntax>(addBinaryExpression.LeftExpression);

            var firstSignExpressionSyntax = (SignExpressionSyntax)addBinaryExpression.LeftExpression;
            Assert.AreEqual(TokenType.Add, firstSignExpressionSyntax.Operator.Type);
            Assert.IsInstanceOf<IntExpressionSyntax>(firstSignExpressionSyntax.Expression);

            Assert.IsInstanceOf<BinaryExpressionSyntax>(addBinaryExpression.RightExpression);
            var multBinaryExpression = (BinaryExpressionSyntax)addBinaryExpression.RightExpression;

            Assert.IsInstanceOf<IntExpressionSyntax>(multBinaryExpression.LeftExpression);
            Assert.IsInstanceOf<SignExpressionSyntax>(multBinaryExpression.RightExpression);

            var secondSignExpressionSyntax2 = (SignExpressionSyntax)multBinaryExpression.RightExpression;
            Assert.AreEqual(TokenType.Sub, secondSignExpressionSyntax2.Operator.Type);
            Assert.AreEqual(multBinaryExpression.Operator.Type, TokenType.Mult);
        }

        [Test]
        public void TestMultipleSignOperator()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(FunctionSource, "return ++1 + 2 * --3"), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var returnStatementSyntax = (ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].StatementMember[0];

            Assert.IsInstanceOf<BinaryExpressionSyntax>(returnStatementSyntax.Expression);
            var addBinaryExpression = (BinaryExpressionSyntax)returnStatementSyntax.Expression;

            Assert.IsInstanceOf<SignExpressionSyntax>(addBinaryExpression.LeftExpression);
            var signExpressionSyntax = (SignExpressionSyntax)addBinaryExpression.LeftExpression;

            Assert.AreEqual(TokenType.Add, signExpressionSyntax.Operator.Type);
            Assert.IsInstanceOf<SignExpressionSyntax>(signExpressionSyntax.Expression);
            var signOfSignExpression = (SignExpressionSyntax)signExpressionSyntax.Expression;
            Assert.AreEqual(TokenType.Add, signOfSignExpression.Operator.Type);

            Assert.IsInstanceOf<IntExpressionSyntax>(signOfSignExpression.Expression);
            Assert.IsInstanceOf<BinaryExpressionSyntax>(addBinaryExpression.RightExpression);
            Assert.AreEqual(addBinaryExpression.Operator.Type, TokenType.Add);
            var multBinaryExpression = (BinaryExpressionSyntax)addBinaryExpression.RightExpression;
            Assert.IsInstanceOf<IntExpressionSyntax>(multBinaryExpression.LeftExpression);
            Assert.IsInstanceOf<SignExpressionSyntax>(multBinaryExpression.RightExpression);

            var secondSignExpression = (SignExpressionSyntax)multBinaryExpression.RightExpression;
            Assert.AreEqual(TokenType.Sub, secondSignExpression.Operator.Type);
            Assert.AreEqual(TokenType.Mult, multBinaryExpression.Operator.Type);
            Assert.IsInstanceOf<SignExpressionSyntax>(secondSignExpression.Expression);
            Assert.AreEqual(TokenType.Sub, ((SignExpressionSyntax)secondSignExpression.Expression).Operator.Type);
        }

        [Test]
        public void TestMultipleSignOperatorWithInnerExpession()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(FunctionSource, "return ++1 + (2 * --3)"), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var returnStatementSyntax = (ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].StatementMember[0];

            Assert.IsInstanceOf<BinaryExpressionSyntax>(returnStatementSyntax.Expression);
            var addBinaryExpression = (BinaryExpressionSyntax)returnStatementSyntax.Expression;

            Assert.IsInstanceOf<SignExpressionSyntax>(addBinaryExpression.LeftExpression);
            var signExpressionSyntax = (SignExpressionSyntax)addBinaryExpression.LeftExpression;

            Assert.AreEqual(TokenType.Add, signExpressionSyntax.Operator.Type);
            Assert.IsInstanceOf<SignExpressionSyntax>(signExpressionSyntax.Expression);
            var signOfSignExpression = (SignExpressionSyntax)signExpressionSyntax.Expression;
            Assert.AreEqual(TokenType.Add, signOfSignExpression.Operator.Type);

            Assert.IsInstanceOf<IntExpressionSyntax>(signOfSignExpression.Expression);
            Assert.IsInstanceOf<BinaryExpressionSyntax>(addBinaryExpression.RightExpression);
            Assert.AreEqual(addBinaryExpression.Operator.Type, TokenType.Add);
            var multBinaryExpression = (BinaryExpressionSyntax)addBinaryExpression.RightExpression;
            Assert.IsInstanceOf<IntExpressionSyntax>(multBinaryExpression.LeftExpression);
            Assert.IsInstanceOf<SignExpressionSyntax>(multBinaryExpression.RightExpression);

            var secondSignExpression = (SignExpressionSyntax)multBinaryExpression.RightExpression;
            Assert.AreEqual(TokenType.Sub, secondSignExpression.Operator.Type);
            Assert.AreEqual(TokenType.Mult, multBinaryExpression.Operator.Type);
            Assert.IsInstanceOf<SignExpressionSyntax>(secondSignExpression.Expression);
            Assert.AreEqual(TokenType.Sub, ((SignExpressionSyntax)secondSignExpression.Expression).Operator.Type);
        }
    }
}