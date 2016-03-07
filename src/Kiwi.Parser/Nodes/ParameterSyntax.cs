using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ParameterSyntax : ISyntaxBase
    {
        public TypeSyntax Type { get; private set; }
        public Token ParameterName { get; private set; }

        public ParameterSyntax(TypeSyntax type, Token parameterName)
        {
            Type = type;
            ParameterName = parameterName;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}