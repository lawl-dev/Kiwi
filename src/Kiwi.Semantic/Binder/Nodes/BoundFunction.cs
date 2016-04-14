using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundFunction : BoundNode, IBoundMember, IFunction
    {
        public BoundFunction(string name, FunctionSyntax syntax) : base(syntax)
        {
            Name = name;
        }

        public List<BoundStatement> Statements { get; set; }

        public IType Type { get; set; }
        public string Name { get; }
        public IEnumerable<IParameter> Parameter { get; set; }
        public IType ReturnType { get; set; }
    }
}