using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundFunction : BoundNode, IFunction
    {
        public BoundFunction(string name, FunctionSyntax syntax) : base(syntax)
        {
            Name = name;
        }

        public BoundStatement Statements { get; internal set; }

        public IType Type { get; internal set; }
        public string Name { get; }
        public IEnumerable<IParameter> Parameter { get; internal set; }
        public IType ReturnType { get; internal set; }
    }
}