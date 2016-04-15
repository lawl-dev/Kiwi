using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FunctionSyntax : ISyntaxBase
    {
        public FunctionSyntax(
            Token functionName,
            List<ParameterSyntax> parameterList,
            List<IStatementSyntax> statements)
        {
            FunctionName = functionName;
            ParameterList = parameterList;
            Statements = statements;
        }

        public Token FunctionName { get; private set; }
        public List<ParameterSyntax> ParameterList { get; private set; }
        public List<IStatementSyntax> Statements { get; }

        public virtual void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}