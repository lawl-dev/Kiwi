namespace Kiwi.Parser.Nodes
{
    public interface ISyntaxBase
    {
        SyntaxType SyntaxType { get; }
        void Accept(ISyntaxVisitor visitor);
        TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor);
    }
}