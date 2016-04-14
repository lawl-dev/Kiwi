using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundObjectCreationExpression : BoundExpression
    {
        public BoundObjectCreationExpression(
            BoundType type,
            BoundConstructor boundConstructor,
            List<BoundExpression> boundParameter,
            ObjectCreationExpressionSyntax expressionSyntax) : base(expressionSyntax, type)
        {
            BoundConstructor = boundConstructor;
            BoundParameter = boundParameter;
        }

        public BoundConstructor BoundConstructor { get; set; }
        public List<BoundExpression> BoundParameter { get; set; }
    }
}