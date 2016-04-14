using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundNamespace : BoundNode
    {
        public BoundNamespace(string namespaceName, NamespaceSyntax syntax) : base(syntax)
        {
            NamespaceName = namespaceName;
            TypesInternal = new List<BoundType>();
            EnumsInternal = new List<BoundEnum>();
        }

        public string NamespaceName { get; private set; }
        public ReadOnlyCollection<BoundType> Types => new ReadOnlyCollection<BoundType>(TypesInternal);
        internal List<BoundType> TypesInternal { get; set; }
        public List<BoundEnum> EnumsInternal { get; set; }
    }
}