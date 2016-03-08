using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ObjectCreationExpressionSyntax : IExpressionSyntax
    {
        public TypeSyntax Type { get; }
        public List<IExpressionSyntax> Parameter { get; }

        public ObjectCreationExpressionSyntax(TypeSyntax type, List<IExpressionSyntax> parameter)
        {
            Type = type;
            Parameter = parameter;
        }

        public virtual void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}