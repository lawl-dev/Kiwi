using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FloatExpressionSyntax : IExpressionSyntax
    {
        public Token Value { get; }
        public SyntaxType SyntaxType => SyntaxType.FloatExpressionSyntax;

        public FloatExpressionSyntax(Token value)
        {
            Value = value;
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