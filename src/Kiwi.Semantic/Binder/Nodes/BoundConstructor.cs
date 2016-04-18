using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundConstructor : BoundNode, IConstructor
    {
        public BoundConstructor(
            List<BoundParameter> boundParameters,
            BoundScopeStatement boundStatements,
            ConstructorSyntax syntax) : base(syntax)
        {
            Parameter = boundParameters;
            Statements = boundStatements;
        }

        public IEnumerable<IParameter> Parameter { get; }
        public BoundScopeStatement Statements { get; }
    }
}