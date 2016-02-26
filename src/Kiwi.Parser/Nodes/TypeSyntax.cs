using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class TypeSyntax : IExpressionSyntax
    {
        public Token TypeName { get; }

        public TypeSyntax(Token typeName)
        {
            TypeName = typeName;
        }
    }
}