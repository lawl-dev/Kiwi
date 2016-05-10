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
            List<EnumSyntax> enums,
            List<FunctionSyntax> functions)
        {
            Name = name;
            Classes = classes;
            Datas = datas;
            Enums = enums;
            Functions = functions;
        }

        public Token Name { get; }
        public List<ClassSyntax> Classes { get; }
        public List<DataSyntax> Datas { get; }
        public List<EnumSyntax> Enums { get; }
        public List<FunctionSyntax> Functions { get; }

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