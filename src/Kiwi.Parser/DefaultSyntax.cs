using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class DefaultSyntax : ISyntaxBase
    {
        public List<ISyntaxBase> Body { get; }

        public DefaultSyntax(List<ISyntaxBase> body)
        {
            Body = body;
        }
    }
}