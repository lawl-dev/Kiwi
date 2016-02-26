using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class InvocationExpressionSyntax : IExpressionSyntax
    {
        public IExpressionSyntax ToInvoke { get; }
        public List<IExpressionSyntax> Parameter { get; }

        public InvocationExpressionSyntax(IExpressionSyntax toInvoke, List<IExpressionSyntax> parameter)
        {
            ToInvoke = toInvoke;
            Parameter = parameter;
        }
    }
}