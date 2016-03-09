using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ConditionalWhenEntry : ISyntaxBase
    {
        public Token Operator { get; private set; }
        public IExpressionSyntax Condition { get; private set; }
        public List<IStatementSyntax> Statements { get; private set; }

        public ConditionalWhenEntry(Token @operator, IExpressionSyntax condition, List<IStatementSyntax> statements)
        {
            Operator = @operator;
            Condition = condition;
            Statements = statements;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}