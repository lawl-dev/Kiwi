using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class NamespaceSyntax : ISyntaxBase
    {
        public NamespaceSyntax(
            Token name,
            List<ClassSyntax> classes,
            List<DataSyntax> datas,
            List<EnumSyntax> enums)
        {
            Name = name;
            Classes = classes;
            Datas = datas;
            Enums = enums;
        }

        public Token Name { get; }
        public List<ClassSyntax> Classes { get; }
        public List<DataSyntax> Datas { get; }
        public List<EnumSyntax> Enums { get; }

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