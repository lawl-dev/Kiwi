using System.Collections.Generic;
using System.Runtime.Remoting.Services;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundArrayCreationExpression : BoundExpression
    {
        public BoundArrayCreationExpression(
            IType type,
            int dimension,
            List<BoundExpression> boundParameter,
            ArrayCreationExpressionSyntax expressionSyntax) : base(expressionSyntax, new ArrayCompilerGeneratedType(type, dimension))
        {
            BoundParameter = boundParameter;
        }

        public List<BoundExpression> BoundParameter { get; set; }
    }
}