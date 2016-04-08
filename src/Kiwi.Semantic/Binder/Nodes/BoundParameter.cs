using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundParameter : BoundNode, IBoundMember
    {
        public Token ParameterName { get; private set; }
        
        public BoundParameter(Token parameterName, IType type, ParameterSyntax parameterSyntax) : base(parameterSyntax)
        {
            ParameterName = parameterName;
            Type = type;
        }

        public IType Type { get; set; }
    }
}