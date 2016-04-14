using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ParameterSyntax : ISyntaxBase
    {
        public ParameterSyntax(TypeSyntax type, Token parameterName)
        {
            Type = type;
            ParameterName = parameterName;
        }

        public TypeSyntax Type { get; private set; }
        public Token ParameterName { get; private set; }
        public SyntaxType SyntaxType => SyntaxType.ParameterSyntax;

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