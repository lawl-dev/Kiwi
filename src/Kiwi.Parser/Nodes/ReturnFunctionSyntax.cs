using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ReturnFunctionSyntax : FunctionSyntax
    {
        public ReturnFunctionSyntax(
            Token functionName,
            List<ParameterSyntax> parameterList,
            ScopeStatementSyntax statements,
            TypeSyntax returnType) : base(functionName, parameterList, statements)
        {
            ReturnType = returnType;
        }

        public TypeSyntax ReturnType { get; }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}