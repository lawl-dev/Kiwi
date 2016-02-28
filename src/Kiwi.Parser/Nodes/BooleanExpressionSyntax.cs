using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class BooleanExpressionSyntax : IExpressionSyntax
    {
        public Token Value { get; }

        public BooleanExpressionSyntax(Token value)
        {
            Value = value;
        }
    }
}