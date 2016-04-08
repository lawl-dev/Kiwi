using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kiwi.Semantic.Binder;
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
            var semanticModel = binder.Bind(ast);

            var boundNamespace = semanticModel.Namespaces.Single(x=>x.NamespaceName.Value == "MyNamespace");
            var referencedBoundType = boundNamespace.Types.Single(x => x.Name.Value == "MyClass2");
            var boundType = boundNamespace.Types.Single(x => x.Name.Value == "MyClass");
            var boundFunction = boundType.Functions.Single(x=>x.Name.Value == "MyFunc");
            Assert.AreEqual(((StandardType)((IBoundMember)boundFunction.Statements[0]).Type).Type, StandardTypes.Bool);
            Assert.AreSame(((IBoundMember)boundFunction.Statements[1]).Type, referencedBoundType);
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
            var semanticModel = binder.Bind(ast);
            var boundNamespace = semanticModel.Namespaces.Single(x=>x.NamespaceName.Value == "MyNamespace");
            var expectedFunctionReturnType = boundNamespace.Types.Single(x=>x.Name.Value == "MyClass2");
            var function = boundNamespace.Types.Single(x => x.Name.Value == "MyClass").Functions.Single(x=>x.Name.Value == "Add");
            Assert.AreSame(expectedFunctionReturnType, function.ReturnType);
        }
    }
}
