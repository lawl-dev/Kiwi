using System.Collections.Generic;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundFunction : BoundNode, IBoundMember
    {
        public Token Name { get; private set; }
        public List<BoundParameter> Parameter { get; set; }
        public IType ReturnType { get; set; }
        public List<BoundStatement> Statements { get; set; }

        public BoundFunction(Token name, FunctionSyntax syntax) : base(syntax)
        {
            Name = name;
        }

        public IType Type { get; set; }
    }
}