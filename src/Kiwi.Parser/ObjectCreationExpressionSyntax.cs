using System.Collections.Generic;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    internal class ObjectCreationExpressionSyntax : IExpressionSyntax
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