using System.Collections.Generic;
using System.Linq;

namespace Kiwi.Parser.Nodes
{
    public class ConstructorSyntax : ISyntaxBase
    {
        public List<ISyntaxBase> ArgList { get; private set; }
        public List<ISyntaxBase> Member { get; }
        public List<IStatetementSyntax> StatementMember => Member.OfType<IStatetementSyntax>().ToList();

        public ConstructorSyntax(List<ISyntaxBase> argList, List<ISyntaxBase> member)
        {
            ArgList = argList;
            Member = member;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}