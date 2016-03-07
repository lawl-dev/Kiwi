using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class IfStatementSyntax : IStatetementSyntax
    {
        public List<ISyntaxBase> Condition { get; }
        public List<IStatetementSyntax> Body { get; } 

        public IfStatementSyntax(List<ISyntaxBase> condition, List<IStatetementSyntax> body)
        {
            Condition = condition;
            Body = body;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}