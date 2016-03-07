using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class DefaultSyntax : ISyntaxBase
    {
        public List<ISyntaxBase> Body { get; }

        public DefaultSyntax(List<ISyntaxBase> body)
        {
            Body = body;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}