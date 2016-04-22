using System;
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

        [Test]
        public void Test_MemberAccessExpressionBinding()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) -> return a * b" +
                               "    }" +
                               "" +
                               "    class MyClass2" +
                               "    {" +
                               "        var addMethod : new MyClass().Add" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            var boundCompilationUnit = binder.Bind(new List<CompilationUnitSyntax>() { ast }).Single();
            var function = boundCompilationUnit.Namespaces[0].Types.Single(x => x.Name == "MyClass").Functions.Single(x=> x.Name == "Add");
            var field = boundCompilationUnit.Namespaces[0].Types.Single(x => x.Name == "MyClass2").Fields.Single(x => x.Name == "addMethod");
            Assert.That(() => field.Type, Is.InstanceOf<FunctionCompilerGeneratedType>().And.Property("ReturnType").InstanceOf<IntCompilerGeneratedType>());
            CollectionAssert.AllItemsAreInstancesOfType(((FunctionCompilerGeneratedType)field.Type).ParameterTypes, typeof(IntCompilerGeneratedType));
        }

        [Test]
        public void Test_IsExpression()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) -> return a * b" +
                               "    }" +
                               "" +
                               "    class MyClass2" +
                               "    {" +
                               "        var myClassField : new MyClass()" +
                               "        func Foo() -> bool" +
                               "        {" +
                               "            return myClassField is MyClass" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            var boundCompilationUnit = binder.Bind(new List<CompilationUnitSyntax>() { ast }).Single();
            var type = boundCompilationUnit.Namespaces[0].Types.Single(x => x.Name == "MyClass");
            var function = (BoundFunction)boundCompilationUnit.Namespaces[0].Types.Single(x => x.Name == "MyClass2").Functions.Single(x => x.Name == "Foo");
            var returnStatement = (BoundReturnStatement)((BoundScopeStatement)function.Statements).Statements[0];
            var boundBinaryExpression = (BoundBinaryExpression)returnStatement.BoundExpression;
            Assert.AreEqual(BinaryOperators.Is, boundBinaryExpression.Operator);
            Assert.IsInstanceOf<BoundTypeExpression>(boundBinaryExpression.Right);
            Assert.AreSame(((BoundTypeExpression)boundBinaryExpression.Right).ReferencedType, type);
        }

        [Test]
        public void Test_IsExpression_ThrowsTypeNotDefined()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) -> return a * b" +
                               "    }" +
                               "" +
                               "    class MyClass2" +
                               "    {" +
                               "        var myClassField : new MyClass()" +
                               "        func Foo() -> bool" +
                               "        {" +
                               "            return myClassField is MyClassS" +
                               "        }" +
                               "    }" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            Assert.That(() => binder.Bind(new List<CompilationUnitSyntax>() { ast }), Throws.InstanceOf<KiwiSemanticException>().With.Message.EqualTo("MyClassS undefined Type"));
        }

        [Test]
        public void Test_IfElseExpression_IfConditionMustBeBool()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) -> return if(a) a else b" +
                               "    }" +
                               "" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            Assert.That(() => binder.Bind(new List<CompilationUnitSyntax>() { ast }), Throws.InstanceOf<KiwiSemanticException>().With.Message.EqualTo("If condition must be of Type Bool"));
        }

        [Test]
        public void Test_IfStatement_IfConditionMustBeBool()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) " +
                               "        { " +
                               "            if(a)" +
                               "            {" +
                               "                " +
                               "            }" +
                               "        }" +
                               "    }" +
                               "" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            Assert.That(() => binder.Bind(new List<CompilationUnitSyntax>() { ast }), Throws.InstanceOf<KiwiSemanticException>().With.Message.EqualTo("If condition must be of Type Bool"));
        }

        [Test]
        public void Test_IfElseStatement_IfConditionMustBeBool()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) " +
                               "        { " +
                               "            if(a)" +
                               "            {" +
                               "                " +
                               "            }" +
                               "            else" +
                               "            {" +
                               "                " +
                               "            }" +
                               "        }" +
                               "    }" +
                               "" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            Assert.That(() => binder.Bind(new List<CompilationUnitSyntax>() { ast }), Throws.InstanceOf<KiwiSemanticException>().With.Message.EqualTo("If condition must be of Type Bool"));
        }

        [Test]
        public void Test_ForStatement_ConditionMustBeBool()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) " +
                               "        { " +
                               "            for(var i : 0; i; i:+1)" +
                               "            {" +
                               "                " +
                               "            }" +
                               "        }" +
                               "    }" +
                               "" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            Assert.That(() => binder.Bind(new List<CompilationUnitSyntax>() { ast }), Throws.InstanceOf<KiwiSemanticException>().With.Message.EqualTo("For condition must be of Type Bool"));
        }

        [Test]
        public void Test_SwitchStatement_CasesTypeMustMatchSwitchConditionType()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) " +
                               "        { " +
                               "            switch(a)" +
                               "            {" +
                               "                case \"LOL\" -> Add(1, 0)" +
                               "            }" +
                               "        }" +
                               "    }" +
                               "" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            Assert.That(() => binder.Bind(new List<CompilationUnitSyntax>() { ast }), Throws.InstanceOf<KiwiSemanticException>().With.Message.EqualTo("Switch cases condition type must match switch condition type"));
        }

        [Test]
        public void Test_Scope_VariableNotDefined()
        {
            const string src = "namespace MyNamespace" +
                               "{" +
                               "    class MyClass" +
                               "    {" +
                               "        func Add(int a, int b) " +
                               "        { " +
                               "            {" +
                               "                var i : 1337 + b" +
                               "            }" +
                               "            var c : a * b * i" +
                               "        }" +
                               "    }" +
                               "" +
                               "}";

            var lexer = new Lexer.Lexer();
            var tokens = lexer.Lex(src);
            var parser = new Parser.Parser(tokens);

            var ast = parser.Parse();

            var binder = new Binder();

            Assert.That(() => binder.Bind(new List<CompilationUnitSyntax>() { ast }), Throws.InstanceOf<KiwiSemanticException>().With.Message.EqualTo("i not defined"));
        }
    }
}