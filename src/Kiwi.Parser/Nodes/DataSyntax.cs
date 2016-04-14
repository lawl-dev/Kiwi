using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class DataSyntax : ISyntaxBase
    {
        public DataSyntax(Token typeName, List<ParameterSyntax> parameter)
        {
            TypeName = typeName;
            Parameter = parameter;
        }

        public Token TypeName { get; }
        public List<ParameterSyntax> Parameter { get; }
        public SyntaxType SyntaxType => SyntaxType.DataSyntax;

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