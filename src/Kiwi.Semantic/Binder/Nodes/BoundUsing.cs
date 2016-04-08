using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundUsing : BoundNode
    {
        public Token NamespaceName { get; set; }
        public BoundNamespace BoundNamespace { get; set; }

        public BoundUsing(Token namespaceName, UsingSyntax usingSyntax, BoundNamespace boundNamespace) : base(usingSyntax)
        {
            NamespaceName = namespaceName;
            BoundNamespace = boundNamespace;
        }
    }
}