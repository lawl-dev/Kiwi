using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundNamespace : BoundNode
    {
        public BoundNamespace(string name, NamespaceSyntax syntax) : base(syntax)
        {
            Name = name;
            TypesInternal = new List<BoundType>();
            EnumsInternal = new List<BoundEnum>();
        }

        public string Name { get; }
        public IEnumerable<BoundType> Types => new ReadOnlyCollection<BoundType>(TypesInternal);
        internal List<BoundType> TypesInternal { get; set; }
        internal List<BoundEnum> EnumsInternal { get; set; }
    }
}