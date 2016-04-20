using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundFloatExpression : BoundExpression
    {
        public float Value { get; }

        public BoundFloatExpression(float value, FloatExpressionSyntax expressionSyntax) : base(expressionSyntax, new FloatCompilerGeneratedType())
        {
            Value = value;
        }
    }
}