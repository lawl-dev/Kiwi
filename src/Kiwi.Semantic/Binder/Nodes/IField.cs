namespace Kiwi.Semantic.Binder.Nodes
{
    public interface IField : IBoundMember
    {
        string Name { get; }
        VariableQualifier Qualifier { get; }
    }
}