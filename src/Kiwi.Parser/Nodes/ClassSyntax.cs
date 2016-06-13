using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ClassSyntax : ISyntaxBase
    {
        public ClassSyntax(Token name, Token protocolName, List<ConstructorSyntax> constructors, List<FunctionSyntax> functions, List<FieldSyntax> fields)
        {
            Name = name;
            ProtocolName = protocolName;
            Constructors = constructors;
            Functions = functions;
            Fields = fields;
        }

        public Token Name { get; }
        public Token ProtocolName { get; }
        public List<ConstructorSyntax> Constructors { get; }
        public List<FunctionSyntax> Functions { get; }
        public List<FieldSyntax> Fields { get; }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}