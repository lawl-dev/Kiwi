using System.Collections.Generic;
using System.Linq;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;
using Kiwi.Semantic.Binder.Nodes;
using NUnit.Framework;

namespace Kiwi.Tests
{
    public class BinderTests
    {
        [Test]
        public void TestFieldTypeToVariableBinding()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        var field : false" +
                               "        var field2 : new MyClass2()" +
                               "        func MyFunc()" +
                               "        {" +
                               "            var i : field" +
                               "            var i2 : field2" +
                               "        }" +
                               "    }" +
                               "" +
                               "    class MyClass2" +
                               "    {" +
                               "        " +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();
            var semanticModel = binder.Bind(new List<CompilationUnitSyntax> { ast }).Single();

            var boundNamespace = semanticModel.Namespaces.Single(x => x.Name == "MyNamespace");
            var referencedBoundType = boundNamespace.Types.Single(x => x.Name == "MyClass2");
            var boundType = boundNamespace.Types.Single(x => x.Name == "MyClass");
            var boundFunction = (BoundFunction)boundType.Functions.Single(x => x.Name == "MyFunc");
            Assert.IsInstanceOf<BoolCompilerGeneratedType>(((IBoundMember)((BoundScopeStatement)boundFunction.Statements).Statements[0]).Type);
            Assert.AreSame(((IBoundMember)((BoundScopeStatement)boundFunction.Statements).Statements[1]).Type, referencedBoundType);
        }

        [Test]
        public void TestExpressionFunctionReturnTypeBinding()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) -> return new MyClass2()" +
                               "    }" +
                               "" +
                               "    class MyClass2" +
                               "    {" +
                               "        " +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();
            var semanticModel = binder.Bind(new List<CompilationUnitSyntax> { ast }).Single();

            var boundNamespace = semanticModel.Namespaces.Single(x => x.Name == "MyNamespace");
            var expectedFunctionReturnType = boundNamespace.Types.Single(x => x.Name == "MyClass2");
            var function = boundNamespace.Types.Single(x => x.Name == "MyClass").Functions.Single(x => x.Name == "Add");
            Assert.AreSame(expectedFunctionReturnType, function.ReturnType);
        }

        [Test]
        public void Test_VariableWithSameName_Throws()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b)" +
                               "        {" +
                               "            var a : 1" +
                               "            var b : 2" +
                               "            var a : 1" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();
            Assert.That(
                () => binder.Bind(new List<CompilationUnitSyntax> { ast }),
                Throws.TypeOf<KiwiSemanticException>().With.Message.EqualTo("a already defined."));
        }

        [Test]
        public void Test_VariableNotDefined_Throws()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b)" +
                               "        {" +
                               "            var a : b" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();
            Assert.That(
                () => binder.Bind(new List<CompilationUnitSyntax> { ast }),
                Throws.TypeOf<KiwiSemanticException>().With.Message.EqualTo("a already defined."));
        }
    }
}