using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundBooleanExpression : BoundExpression
    {
        public BoundBooleanExpression(bool value, BooleanExpressionSyntax syntax) : base(syntax, new BoolCompilerGeneratedType())
        {
            Value = value;
        }

        public bool Value { get; }
    }
}