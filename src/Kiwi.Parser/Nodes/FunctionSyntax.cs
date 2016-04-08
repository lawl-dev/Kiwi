using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FunctionSyntax : ISyntaxBase
    {
        public Token FunctionName { get; private set; }
        public List<ParameterSyntax> ParameterList { get; private set; }
        public List<IStatementSyntax> Statements { get;  }
        public virtual SyntaxType SyntaxType => SyntaxType.FunctionSyntax;

        public FunctionSyntax(
            Token functionName,
            List<ParameterSyntax> parameterList,
            List<IStatementSyntax> statements)
        {
            FunctionName = functionName;
            ParameterList = parameterList;
            Statements = statements;
        }
        
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