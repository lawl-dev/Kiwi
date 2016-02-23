using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxParameterAst : SyntaxAstBase
    {
        private readonly Token _parameterName;
        public Token TypeName { get; set; }

        public SyntaxParameterAst(Token typeName, Token parameterName)
        {
            _parameterName = parameterName;
            TypeName = typeName;
        }
    }
}