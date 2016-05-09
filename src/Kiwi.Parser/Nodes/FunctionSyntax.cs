using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FunctionSyntax : ISyntaxBase
    {
        public FunctionSyntax(Token name, List<ParameterSyntax> parameter, IStatementSyntax statements, TypeSyntax returnType)
        {
            Name = name;
            Parameter = parameter;
            Statements = statements;
            ReturnType = returnType;
        }

        public Token Name { get; }
        public List<ParameterSyntax> Parameter { get; }
        public IStatementSyntax Statements { get; }
        public TypeSyntax ReturnType { get; }

        public virtual void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}