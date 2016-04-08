using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class DataSyntax : ISyntaxBase
    {
        public Token TypeName { get; }
        public List<ParameterSyntax> Parameter { get; }
        public SyntaxType SyntaxType => SyntaxType.DataSyntax;

        public DataSyntax(Token typeName, List<ParameterSyntax> parameter)
        {
            TypeName = typeName;
            Parameter = parameter;
        }
        
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