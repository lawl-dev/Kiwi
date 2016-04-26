using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundEnum : BoundNode
    {
        public BoundEnum(string enumName, EnumSyntax enumSyntax) : base(enumSyntax)
        {
            EnumName = enumName;
        }

        public string EnumName { get; set; }
    }
}