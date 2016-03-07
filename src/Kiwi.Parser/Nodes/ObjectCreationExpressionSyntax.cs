using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ObjectCreationExpressionSyntax : IExpressionSyntax
    {
        public TypeSyntax Type { get; }
        public List<ISyntaxBase> Parameter { get; }

        public ObjectCreationExpressionSyntax(TypeSyntax type, List<ISyntaxBase> parameter)
        {
            Type = type;
            Parameter = parameter;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}