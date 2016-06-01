using System;
using System.Linq;
using System.Runtime.InteropServices;
using Kiwi.Common;
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
        public void TestClass()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(NamespaceSource, string.Format(ClassSource, string.Empty, string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsInstanceOf<CompilationUnitSyntax>(ast);
            Assert.IsNotEmpty(ast.Namespaces);
            Assert.IsNotEmpty(ast.Namespaces[0].Classes);
            Assert.AreEqual("ClassSample", ast.Namespaces[0].Classes[0].Name.Value);
            Assert.AreEqual("DSampleDescriptor", ast.Namespaces[0].Classes[0].DescriptorName.Value);
        }

        [Test]
        public void TestFunction()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(
                        NamespaceSource,
                        string.Format(
                            ClassSource,
                            string.Format(FunctionSource, string.Empty),
                            string.Empty,
                            string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsNotEmpty(ast.Namespaces[0].Classes[0].Functions);
            Assert.AreEqual("FunctionSample", ast.Namespaces[0].Classes[0].Functions[0].Name.Value);
        }
        

        [Test]
        public void TestExpressionFunction()
        {
            const string functionSource = "func FunctionSample(int a, int b, int c) -> return a * b * c";

            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(
                        NamespaceSource,
                        string.Format(ClassSource, functionSource, string.Empty, string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsNotEmpty(ast.Namespaces[0].Classes[0].Functions);
            Assert.IsInstanceOf<FunctionSyntax>(ast.Namespaces[0].Classes[0].Functions[0]);
            Assert.AreEqual("FunctionSample", ast.Namespaces[0].Classes[0].Functions[0].Name.Value);
        }

        [TestCase("return 1", typeof(ReturnStatementSyntax))]
        [TestCase("var i : 1", typeof(VariableDeclarationStatementSyntax))]
        [TestCase("i : 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :+ 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :- 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :* 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :/ 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i :^ 1", typeof(AssignmentStatementSyntax))]
        [TestCase("i.a.b :^ 1", typeof(AssignmentStatementSyntax))]
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
        [TestCase("match" + "\r\n" +
                  "{" + "\r\n" +
                  "     case a = b && c > d  -> {" + "\r\n" +
                  "                 //code" + "\r\n" +
                  "                }" + "\r\n" +
                  "     case buu > b -> dooooStuff()" + "\r\n" +
                  "}", typeof(SimpleMatchStatementSyntax))]
        [TestCase("match (condition)" + "\r\n" +
                  "{" + "\r\n" +
                  "     case is Type -> Do()" + "\r\n" +
                  "     case !=value  -> Do2()" + "\r\n" +
                  "     case in 1, 2, 3  -> Do3()" + "\r\n" +
                  "     case !in 0..20   -> Do4()" + "\r\n" +
                  "     case > 20    -> {" + "\r\n" +
                  "                 //code" + "\r\n" +
                  "                }" + "\r\n" +
                  "}", typeof(ConditionalMatchStatementSyntax))]
        public void TestStatements(string statementSource, Type type)
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(
                        NamespaceSource,
                        string.Format(
                            ClassSource,
                            string.Format(FunctionSource, statementSource),
                            string.Empty,
                            string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsInstanceOf(type, ((ScopeStatementSyntax)ast.Namespaces[0].Classes[0].Functions[0].Statements).Statements[0]);
        }

        [TestCase("return if(1 = 2 * 5) 1 else 2", typeof(IfElseExpressionSyntax))]
        [TestCase("return 1 * 2", typeof(BinaryExpressionSyntax))]
        [TestCase("return 12f", typeof(FloatExpressionSyntax))]
        [TestCase("return 12", typeof(IntExpressionSyntax))]
        [TestCase("return variable", typeof(IdentifierExpressionSyntax))]
        [TestCase("return new Object()", typeof(ObjectCreationExpressionSyntax))]
        [TestCase("return -1", typeof(SignExpressionSyntax))]
        [TestCase("return \"Hallo\"", typeof(StringExpressionSyntax))]
        [TestCase("return 1..1337", typeof(BinaryExpressionSyntax))]
        [TestCase("return variable.Member", typeof(MemberAccessExpressionSyntax))]
        [TestCase("return variable.Function()", typeof(InvocationExpressionSyntax))]
        [TestCase("return variable[0]", typeof(ArrayAccessExpressionSyntax))]
        [TestCase("return variable.Member[0].Function()[1]", typeof(ArrayAccessExpressionSyntax))]
        [TestCase("return Function()", typeof(InvocationExpressionSyntax))]
        [TestCase("return Function(a, b, 10)", typeof(InvocationExpressionSyntax))]
        [TestCase("return func(int a, int b) -> return a * b", typeof(AnonymousFunctionExpressionSyntax))]
        [TestCase("return func(a, b) -> return a * b", typeof(ImplicitParameterTypeAnonymousFunctionExpressionSyntax))]
        public void TestExpressions(string statementSource, Type type)
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(
                        NamespaceSource,
                        string.Format(
                            ClassSource,
                            string.Format(FunctionSource, statementSource),
                            string.Empty,
                            string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            Assert.IsInstanceOf(
                type,
                ((ReturnStatementSyntax)((ScopeStatementSyntax)ast.Namespaces[0].Classes[0].Functions[0].Statements).Statements[0]).Expression);
        }

        [TestCase("return if(switch) 1 else 2", typeof(KiwiSyntaxException),
            "Unexpected Token switch. Expected Sign Operator, New, Int, Float, String or Identifier Expression.")]
        [TestCase("int if(switch) 1 else 2", typeof(KiwiSyntaxException),
            "Unexpected Token \"int\". Expected If, Return, Match, Switch, Var, Immutable, Identifier, For or Forr")]
        [TestCase("for(f(); i < 1; i :+ 1){}", typeof(KiwiSyntaxException),
            "Unexpected Statement. Expected AssignmentStatement")]
        [TestCase("i lol 1", typeof(KiwiSyntaxException),
            "Unexpected assign operator lol. Expected :, :+, :-, :/, :* or :^.")]
        [TestCase("i + 1", typeof(KiwiSyntaxException),
            "Unexpected Syntax. Expected MemberAccessExpressionSyntax, ArrayAccessExpression, IdentifierExpressionSyntax or InvocationExpressionSyntax"
            )]
        [TestCase("switch(i){" +
                  "else -> f()" +
                  "else -> f2()" +
                  "}", typeof(KiwiSyntaxException), "Duplicate else label")]
        [TestCase("switch(i){" +
                  "switch -> f()" +
                  "else -> f2()" +
                  "}", typeof(KiwiSyntaxException), "Unexpected Token. Expected Case or Else")]
        [TestCase("match(i){" +
                  "case -> f()" +
                  "else -> f2()" +
                  "}", typeof(KiwiSyntaxException), "Expected a binary operator")]
        public void TestStatementExceptions(string statementSource, Type exception, string message)
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(
                        NamespaceSource,
                        string.Format(
                            ClassSource,
                            string.Format(FunctionSource, statementSource),
                            string.Empty,
                            string.Empty)));
            var parser = new Parser.Parser(tokens);

            Assert.That(() => parser.Parse(), Throws.TypeOf(exception).With.Message.EqualTo(message));
        }

        [Test]
        public void TestSimpleOperatorPrecedence()
        {
            var lexer = new Lexer.Lexer();
            var tokens =
                lexer.Lex(
                    string.Format(
                        NamespaceSource,
                        string.Format(
                            ClassSource,
                            string.Format(FunctionSource, "return 1 + 2 * 3"),
                            string.Empty,
                            string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var returnStatementSyntax =
                (ReturnStatementSyntax)((ScopeStatementSyntax)ast.Namespaces[0].Classes[0].Functions[0].Statements).Statements[0];

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
                    string.Format(
                        NamespaceSource,
                        string.Format(
                            ClassSource,
                            string.Format(FunctionSource, "return +1 + 2 * -3"),
                            string.Empty,
                            string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var returnStatementSyntax =
                (ReturnStatementSyntax)((ScopeStatementSyntax)ast.Namespaces[0].Classes[0].Functions[0].Statements).Statements[0];

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
                    string.Format(
                        NamespaceSource,
                        string.Format(
                            ClassSource,
                            string.Format(FunctionSource, "return ++1 + 2 * --3"),
                            string.Empty,
                            string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var returnStatementSyntax =
                (ReturnStatementSyntax)((ScopeStatementSyntax)ast.Namespaces[0].Classes[0].Functions[0].Statements).Statements[0];

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
                    string.Format(
                        NamespaceSource,
                        string.Format(
                            ClassSource,
                            string.Format(FunctionSource, "return ++1 + (2 * --3)"),
                            string.Empty,
                            string.Empty)));
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var returnStatementSyntax =
                (ReturnStatementSyntax)((ScopeStatementSyntax)ast.Namespaces[0].Classes[0].Functions[0].Statements).Statements[0];

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
        public void Test_Infix_ClassLevel()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass2" +
                               "    {" +
                               "        infix func Add(int a, int b) -> return a + b" +
                               "        func Foo()" +
                               "        {" +
                               "            var a : 1 Add 2" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var namespaceSyntax = ast.Namespaces.Single(x => x.Name.Value == "MyNamespace");
            var classSyntax = namespaceSyntax.Classes.Single(x=> x.Name.Value == "MyClass2");
            var infixFunction = classSyntax.Functions.Single(x=>x.Name.Value == "Add");
            Assert.IsInstanceOf<InfixFunctionSyntax>(infixFunction);
        }

        [Test]
        public void Test_Infix_NamespaceLevel()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    infix func Add(int a, int b) -> return a + b" +
                               "    class MyClass2" +
                               "    {" +
                               "        func Foo()" +
                               "        {" +
                               "            var a : 1 Add 2" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();
            var namespaceSyntax = ast.Namespaces.Single(x => x.Name.Value == "MyNamespace");
            var infixFunction = namespaceSyntax.Functions.Single(x=>x.Name.Value == "Add");
            Assert.IsInstanceOf<InfixFunctionSyntax>(infixFunction);
        }

        [TestCase("operator Add(MyClassName opA, MyClassName opB) -> return true")]
        [TestCase("operator Sub(MyClassName opA, MyClassName opB) -> return true")]
        [TestCase("operator Mult(MyClassName opA, MyClassName opB) -> return true")]
        [TestCase("operator Div(MyClassName opA, MyClassName opB) -> return true")]
        [TestCase("operator Pow(MyClassName opA, MyClassName opB) -> return true")]
        [TestCase("operator Range(MyClassName opA, MyClassName opB) -> return true")]
        [TestCase("operator In(MyClassName opA, MyClassName opB) -> return true")]
        [TestCase("operator CompareTo(MyClassName opA, MyClassName opB) -> return true")]
        public void Test_ValidOperatorNames_DontThrow(string opCode)
        {
            const string template = "namespace MyNamespace" +
                                    "{{" +
                                    "    class MyClass2" +
                                    "    {{" +
                                    "        {0}" +
                                    "    }}" +
                                    "}}";




            var parserFunc = new Action<string>(
                opSrc =>
                {
                    var lexer = new Lexer.Lexer();
                    var tokens = lexer.Lex(string.Format(template, opSrc));
                    var parser = new Parser.Parser(tokens);
                    parser.Parse();
                });

            Assert.That(() => parserFunc(opCode), Throws.Nothing);
        }

        [Test]
        public void Test_InvalidOperatorNames_Throw()
        {
            const string template = "namespace MyNamespace" +
                                    "{{" +
                                    "    class MyClass2" +
                                    "    {{" +
                                    "        {0}" +
                                    "    }}" +
                                    "}}";




            var parserFunc = new Action<string>(
                opSrc =>
                {
                    var lexer = new Lexer.Lexer();
                    var tokens = lexer.Lex(string.Format(template, opSrc));
                    var parser = new Parser.Parser(tokens);
                    parser.Parse();
                });

            Assert.That(() => parserFunc("operator Addd(MyClassName opA, MyClassName opB) -> return true"), Throws.InstanceOf<KiwiSyntaxException>().With.Message.EqualTo("Invalid operator function name 'Addd'"));
        }

        [Test]
        public void Test_GenericFunction_NamespaceLevel()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    func GenericFunction!(TypeA, TypeB)(TypeA a, TypeB b)" +
                               "    {" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);
            
            Assert.DoesNotThrow(() => parser.Parse());
        }

        [Test]
        public void Test_GenericLocalFunction()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    func GenericFunction!(TypeA, TypeB)(TypeA a, TypeB b)" +
                               "    {" +
                               "        immut anonFunc : func!(TypeX, TypeY)(TypeX x, TypeY y) -> return x" +
                               "        " +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);
            
            Assert.DoesNotThrow(() => parser.Parse());
        }

        [Test]
        public void Test_GenericLocalFunction_Call()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    func GenericFunction!(TypeA, TypeB)(TypeA a, TypeB b)" +
                               "    {" +
                               "        immut anonFunc : func!(TypeX, TypeY)(TypeX x, TypeY y) -> return x" +
                               "        anonFunc!(a, a)" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);
            
            Assert.DoesNotThrow(() => parser.Parse());
        }

        [Test]
        public void Test_GenericClass()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class GenericClass(TypeA, TypeB)" +
                               "    {" +
                               "        var mutable1 : None TypeA;" +
                               "        var mutable2 : None TypeB;" +
                               "        Constructor(TypeA arg1, TypeB arg2)" +
                               "        {" +
                               "            mutable1 : arg1;" +
                               "            mutable2 : arg2;" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);
            
            Assert.DoesNotThrow(() => parser.Parse());
        }

        [Test]
        public void Test_TryCatch()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class GenericClass(TypeA, TypeB)" +
                               "    {" +
                               "        Constructor(TypeA arg1, TypeB arg2)" +
                               "        {" +
                               "            Try" +
                               "            {" +
                               "            }" +
                               "            catch(Error)" +
                               "            {" +
                               "            }" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);
            
            Assert.DoesNotThrow(() => parser.Parse());
        }

        [Test]
        public void Test_Throw()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class GenericClass(TypeA, TypeB)" +
                               "    {" +
                               "        Constructor(TypeA arg1, TypeB arg2)" +
                               "        {" +
                               "            throw new Error()" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);
            
            Assert.DoesNotThrow(() => parser.Parse());
        }

        [Test]
        public void Test_Optional()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class Class" +
                               "    {" +
                               "        Constructor(Optional!(int) i, Optional!(string) s)" +
                               "        {" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);
            
            Assert.DoesNotThrow(() => parser.Parse());
        }

        [TestCase("func PrivateFoo(){}")]
        [TestCase("open func OpenFoo(){}")]
        [TestCase("abstract func AbstractFoo(){}")]
        [TestCase("public abstract func AbstractFoo2(){}")]
        [TestCase("protected func ProtectedFoo(){}")]
        [TestCase("override func OpenFoo(){}")]
        [TestCase("open override func OpenFoo2(){}")]
        [TestCase("public override func AbstractFoo2(){ProtectedFoo()}")]
        public void Test_Function_VisibilityModifiers(string code)
        {
            const string template = "namespace MyNamespace" +
                                    "{{" +
                                    "    class MyClass" +
                                    "    {{" +
                                    "        {0}" +
                                    "    }}" +
                                    "}}";




            var parserFunc = new Action<string>(
                opSrc =>
                {
                    var lexer = new Lexer.Lexer();
                    var tokens = lexer.Lex(string.Format(template, opSrc));
                    var parser = new Parser.Parser(tokens);
                    parser.Parse();
                });

            Assert.That(() => parserFunc(code), Throws.Nothing);
        }

        [TestCase("abstract")]
        [TestCase("public abstract")]
        public void Test_Class_VisibilityModifiers(string modifiers)
        {
            const string template = "namespace MyNamespace" +
                                    "{{" +
                                    "    {0} class MyClass" +
                                    "    {{" +
                                    "    }}" +
                                    "}}";




            var parserFunc = new Action<string>(
                opSrc =>
                {
                    var lexer = new Lexer.Lexer();
                    var tokens = lexer.Lex(string.Format(template, opSrc));
                    var parser = new Parser.Parser(tokens);
                    parser.Parse();
                });

            Assert.That(() => parserFunc(modifiers), Throws.Nothing);
        }

        [TestCase("Descriptor")]
        [TestCase("BaseClass, Descriptor")]
        public void Test_Class_Inheritance(string code)
        {
            const string template = "namespace MyNamespace" +
                                    "{{" +
                                    "    class MyClass is {0}" +
                                    "    {{" +
                                    "    }}" +
                                    "}}";




            var parserFunc = new Action<string>(
                opSrc =>
                {
                    var lexer = new Lexer.Lexer();
                    var tokens = lexer.Lex(string.Format(template, opSrc));
                    var parser = new Parser.Parser(tokens);
                    parser.Parse();
                });

            Assert.That(() => parserFunc(code), Throws.Nothing);
        }
    }
}