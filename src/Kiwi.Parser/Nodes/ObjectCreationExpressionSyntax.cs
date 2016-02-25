using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ObjectCreationExpressionSyntax : IExpressionSyntax
    {
        public Token TypeName { get; }
        public List<ISyntaxBase> Parameter { get; }

        public ObjectCreationExpressionSyntax(Token typeName, List<ISyntaxBase> parameter)
        {
            TypeName = typeName;
            Parameter = parameter;
        }
    }
}