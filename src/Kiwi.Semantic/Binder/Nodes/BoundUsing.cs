using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundUsing : BoundNode
    {
        public BoundUsing(string namespaceName, UsingSyntax usingSyntax, BoundNamespace boundNamespace)
            : base(usingSyntax)
        {
            NamespaceName = namespaceName;
            BoundNamespace = boundNamespace;
        }

        public string NamespaceName { get; set; }
        public BoundNamespace BoundNamespace { get; set; }
    }
}