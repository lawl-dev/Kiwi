namespace Kiwi.Semantic.Binder.Nodes
{
    public interface IField
    {
        string Name { get; }
        VariableQualifier Qualifier { get; }
        IType Type { get; }
    }
}