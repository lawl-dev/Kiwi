using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundConstructor : BoundNode
    {
        public BoundConstructor(
            List<BoundParameter> boundParameters,
            List<BoundStatement> boundStatements,
            ConstructorSyntax syntax) : base(syntax)
        {
            Parameters = boundParameters;
            Statements = boundStatements;
        }

        public List<BoundParameter> Parameters { get; set; }
        public List<BoundStatement> Statements { get; set; }
    }
}