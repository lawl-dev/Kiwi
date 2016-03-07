using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class DataSyntax : ISyntaxBase
    {
        public Token TypeName { get; }
        public List<ParameterSyntax> Parameter { get; }

        public DataSyntax(Token typeName, List<ParameterSyntax> parameter)
        {
            TypeName = typeName;
            Parameter = parameter;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}