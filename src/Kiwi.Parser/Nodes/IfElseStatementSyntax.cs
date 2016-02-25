using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class IfElseStatementSyntax : IfStatementSyntax
    {
        public List<IStatetementSyntax> ElseBody { get; }

        public IfElseStatementSyntax(
            List<ISyntaxBase> condition,
            List<IStatetementSyntax> body,
            List<IStatetementSyntax> elseBody) : base(condition, body)
        {
            ElseBody = elseBody;
        }
    }
}