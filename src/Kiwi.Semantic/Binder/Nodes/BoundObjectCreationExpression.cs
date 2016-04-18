using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundObjectCreationExpression : BoundExpression
    {
        public BoundObjectCreationExpression(
            IType type,
            IConstructor boundConstructor,
            List<BoundExpression> boundParameter,
            ObjectCreationExpressionSyntax expressionSyntax) : base(expressionSyntax, type)
        {
            BoundConstructor = boundConstructor;
            BoundParameter = boundParameter;
        }

        public IConstructor BoundConstructor { get; set; }
        public List<BoundExpression> BoundParameter { get; set; }
    }
}