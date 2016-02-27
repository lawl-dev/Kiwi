using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ArrayAccessExpression : IExpressionSyntax
    {
        public IExpressionSyntax Owner { get; }
        public List<IExpressionSyntax> Parameter { get; }

        public ArrayAccessExpression(IExpressionSyntax owner, List<IExpressionSyntax> parameter)
        {
            Owner = owner;
            Parameter = parameter;
        }
    }
}