using System.Collections.Generic;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.LanguageTypes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundArrayCreationExpression : BoundExpression
    {
        public BoundArrayCreationExpression(
            BoundType type,
            int dimension,
            List<BoundExpression> boundParameter,
            ArrayCreationExpressionSyntax expressionSyntax) : base(expressionSyntax, new ArrayType(type, dimension))
        {
            BoundParameter = boundParameter;
        }

        public List<BoundExpression> BoundParameter { get; set; }
    }
}