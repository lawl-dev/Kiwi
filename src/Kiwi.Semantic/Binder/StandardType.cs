using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class StandardType : IType
    {
        public StandardTypes Type { get; set; }

        public StandardType(StandardTypes standardTypes)
        {
            Type = standardTypes;
        }
    }
}