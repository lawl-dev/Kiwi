using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.LanguageTypes
{
    public class SpecialField : IField
    {
        public SpecialField(string name, VariableQualifier qualifier, IType type)
        {
            Name = name;
            Qualifier = qualifier;
            Type = type;
        }

        public string Name { get; }
        public VariableQualifier Qualifier { get; }
        public IType Type { get; internal set; }
    }
}