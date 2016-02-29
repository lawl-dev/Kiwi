namespace Kiwi.Parser.Nodes
{
    public interface ISyntaxBase
    {
        void Accept(ISyntaxVisitor visitor);
    }
}