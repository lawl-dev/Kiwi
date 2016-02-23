using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxUsingAst : SyntaxAstBase
    {
        public Token NamespaceName { get; private set; }

        public SyntaxUsingAst(Token namespaceName)
        {
            NamespaceName = namespaceName;
        }
    }
}