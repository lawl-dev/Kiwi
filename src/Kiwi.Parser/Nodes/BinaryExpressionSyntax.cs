using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class BinaryExpressionSyntax : IExpressionSyntax
    {
        public IExpressionSyntax LeftExpression { get; }
        public IExpressionSyntax RightExpression { get; }
        public Token Operator { get; }
        public SyntaxType SyntaxType => SyntaxType.BinaryExpressionSyntax;

        public BinaryExpressionSyntax(IExpressionSyntax leftExpression, IExpressionSyntax rightExpression, Token @operator)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
            Operator = @operator;
        }


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