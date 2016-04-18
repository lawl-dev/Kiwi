using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundParameter : BoundNode, IBoundMember, IParameter
    {
        public BoundParameter(string name, IType type, ParameterSyntax parameterSyntax) : base(parameterSyntax)
        {
            Name = name;
            Type = type;
        }

        public IType Type { get; }
        public string Name { get; }
    }

    public interface IParameter
    {
        string Name { get; }
        IType Type { get; }
    }

    public class SpecialParameter : IParameter
    {
        public SpecialParameter(string name, IType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public IType Type { get; }
    }
}