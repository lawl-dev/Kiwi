using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ConditionalWhenEntry : ISyntaxBase
    {
        public ConditionalWhenEntry(Token @operator, IExpressionSyntax condition, IStatementSyntax statements)
        {
            Operator = @operator;
            Condition = condition;
            Statements = statements;
        }

        public Token Operator { get; private set; }
        public IExpressionSyntax Condition { get; private set; }
        public IStatementSyntax Statements { get; private set; }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}