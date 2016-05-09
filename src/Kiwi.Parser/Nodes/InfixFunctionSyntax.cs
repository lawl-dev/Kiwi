using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class InfixFunctionSyntax : FunctionSyntax
    {
        public InfixFunctionSyntax(Token name, List<ParameterSyntax> parameter, IStatementSyntax statements, TypeSyntax returnType) : base(name, parameter, statements, returnType)
        {
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}