using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.CompilerGeneratedNodes
{
    public class CompilerGeneratedField : IField
    {
        public CompilerGeneratedField(string name, VariableQualifier qualifier, IType type)
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