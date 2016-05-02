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
            List<BoundExpression> parameter,
            ArrayCreationExpressionSyntax expressionSyntax) : base(expressionSyntax, new ArrayCompilerGeneratedType(type, dimension))
        {
            Parameter = parameter;
        }

        public List<BoundExpression> Parameter { get; set; }
    }
}