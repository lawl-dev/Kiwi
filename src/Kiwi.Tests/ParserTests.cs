using System;
using Kiwi.Lexer;
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

        private const string FunctionSource = "func FunctionSample(int a, int[] b, ..int c) -> int" + "\r\n" +
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
        public void TestDataFunction()
        {
            const string functionSource = "func FunctionSample(int a, int b, int c) -> data Dto(int a, string b)" + "\r\n" +
                                          "{{" + "\r\n" +
                                          "     {0}" + "\r\n" + //statements placeholder
                                          "}}";

            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(functionSource, string.Empty), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsNotEmpty(ast.NamespaceMember[0].ClassMember[0].FunctionMember);
            Assert.IsInstanceOf<DataClassFunctionSyntax>(ast.NamespaceMember[0].ClassMember[0].FunctionMember[0]);
            Assert.AreEqual("FunctionSample", ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].FunctionName.Value);
        }

        [Test]
        public void TestExpressionFunction()
        {
            const string functionSource = "func FunctionSample(int a, int b, int c) -> return a * b * c";

            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, functionSource, string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsNotEmpty(ast.NamespaceMember[0].ClassMember[0].FunctionMember);
            Assert.IsInstanceOf<ExpressionFunctionSyntax>(ast.NamespaceMember[0].ClassMember[0].FunctionMember[0]);
            Assert.AreEqual("FunctionSample", ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].FunctionName.Value);
        }

        [TestCase("return 1", typeof(ReturnStatementSyntax))]
        [TestCase("var i : 1", typeof(VariableDeclarationStatementSyntax))]
        [TestCase("i : 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :+ 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :- 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :* 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :/ 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :^ 1", typeof(AssignmentStatementSyntax))]
        [TestCase("if(i = 1)" + "\r\n" +
                  "{" + "\r\n" +
                  "     //code" + "\r\n" +
                  "}", typeof(IfStatementSyntax))]
        [TestCase("if(i = 1)" + "\r\n" +
                  "{" + "\r\n" +
                  "     //code" + "\r\n" +
                  "}" + "\r\n" +
                  "else" + "\r\n" +
                  "{" + "\r\n" +
                  "     //code" + "\r\n" +
                  "}", typeof(IfElseStatementSyntax))]
        [TestCase("for(var i : 0; i < 100; i:+1)" + "\r\n" +
                  "{" + "\r\n" +
                  "     return 1" + "\r\n" +
                  "}", typeof(ForStatementSyntax))]
        [TestCase("switch(variable)" + "\r\n" +
                  "{" + "\r\n" +
                  "     case 1 -> return 1" + "\r\n" +
                  "     case 2 -> {" + "\r\n" +
                  "                     return 2" + "\r\n" +
                  "               }" + "\r\n" +
                  "     else -> return 1337" + "\r\n" +
                  "}", typeof(SwitchStatementSyntax))]
        [TestCase("for(i in GetInts(1*2+3))" + "\r\n" +
                  "{" + "\r\n" +
                  "     //code" + "\r\n" +
                  "}", typeof(ForInStatementSyntax))]
        [TestCase("forr(i in GetInts(1*2+3))" + "\r\n" +
                  "{" + "\r\n" +
                  "     //code" + "\r\n" +
                  "}", typeof(ReverseForInStatementSyntax))]
        [TestCase("when" + "\r\n" +
                  "{" + "\r\n" +
                  "     a = b && c > d  -> {" + "\r\n" +
                  "                 //code" + "\r\n" +
                  "                }" + "\r\n" +
                  "     buu > b -> dooooStuff()" + "\r\n" +
                  "}", typeof(SimpleWhenStatementSyntax))]
        [TestCase("when (condition)" + "\r\n" +
                  "{" + "\r\n" +
                  "     is Type -> Do()" + "\r\n" +
                  "     !=value  -> Do2()" + "\r\n" +
                  "     in 1, 2, 3  -> Do3()" + "\r\n" +
                  "     !in 0..20   -> Do4()" + "\r\n" +
                  "     > 20    -> {" + "\r\n" +
                  "                 //code" + "\r\n" +
                  "                }" + "\r\n" +
                  "}", typeof(ConditionalWhenStatementSyntax))]
        public void TestStatements(string statementSource, Type type)
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(FunctionSource, statementSource), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsInstanceOf(type, ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].Statements[0]);
        }

        [TestCase("return if(1 = 2 * 5) 1 else 2", typeof(IfElseExpressionSyntax))]
        [TestCase("return 1 * 2", typeof(BinaryExpressionSyntax))]
        [TestCase("return 12f", typeof(FloatExpressionSyntax))]
        [TestCase("return 12", typeof(IntExpressionSyntax))]
        [TestCase("return variable", typeof(MemberExpressionSyntax))]
        [TestCase("return new Object()", typeof(ObjectCreationExpressionSyntax))]
        [TestCase("return -1", typeof(SignExpressionSyntax))]
        [TestCase("return \"Hallo\"", typeof(StringExpressionSyntax))]
        [TestCase("return 1..1337", typeof(BinaryExpressionSyntax))]
        [TestCase("return variable.Member", typeof(MemberAccessExpressionSyntax))]
        [TestCase("return variable.Function()", typeof(InvocationExpressionSyntax))]
        [TestCase("return variable[0]", typeof(ArrayAccessExpression))]
        [TestCase("return variable.Member[0].Function()[1]", typeof(ArrayAccessExpression))]
        [TestCase("return Function()", typeof(InvocationExpressionSyntax))]
        [TestCase("return Function(a, b, 10)", typeof(InvocationExpressionSyntax))]
        [TestCase("return func(int a, int b) -> return a * b", typeof(AnonymousFunctionExpressionSyntax))]
        [TestCase("return func(a, b) -> return a * b", typeof(ImplicitParameterTypeAnonymousFunctionExpressionSyntax))]
        public void TestExpressions(string statementSource, Type type)
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Format(FunctionSource, statementSource), string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsInstanceOf(type, ((ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].Statements[0]).Expression);
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
            var returnStatementSyntax = (ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].Statements[0];

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
            var returnStatementSyntax = (ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].Statements[0];

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
            var returnStatementSyntax = (ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].Statements[0];

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
            var returnStatementSyntax = (ReturnStatementSyntax)ast.NamespaceMember[0].ClassMember[0].FunctionMember[0].Statements[0];

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