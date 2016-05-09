using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class VariableDeclarationStatementSyntax : IStatementSyntax
    {
        public VariableDeclarationStatementSyntax(
            Token qualifier,
            Token identifier,
            IExpressionSyntax initExpression)
        {
            Qualifier = qualifier;
            Identifier = identifier;
            InitExpression = initExpression;
        }

        public Token Qualifier { get; }
        public Token Identifier { get; }
        public IExpressionSyntax InitExpression { get; }

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