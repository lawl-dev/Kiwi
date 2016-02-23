using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class ParameterSyntax : ISyntaxBase
    {
        public Token TypeName { get; private set; }
        public Token ParameterName { get; private set; }

        public ParameterSyntax(Token typeName, Token parameterName)
        {
            TypeName = typeName;
            ParameterName = parameterName;
        }
    }
}