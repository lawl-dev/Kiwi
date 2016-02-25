using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class IntExpressionSyntax : IExpressionSyntax, IConstExpression
    {
        public Token Value { get; }

        public IntExpressionSyntax(Token value)
        {
            Value = value;
        }
    }
}