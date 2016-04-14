using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class NamespaceSyntax : ISyntaxBase
    {
        public NamespaceSyntax(
            Token namespaceName,
            List<ClassSyntax> classes,
            List<DataSyntax> datas,
            List<EnumSyntax> enums)
        {
            NamespaceName = namespaceName;
            Classes = classes;
            Datas = datas;
            Enums = enums;
        }

        public Token NamespaceName { get; private set; }
        public List<ClassSyntax> Classes { get; private set; }
        public List<DataSyntax> Datas { get; private set; }
        public List<EnumSyntax> Enums { get; private set; }
        public SyntaxType SyntaxType => SyntaxType.NamespaceSyntax;

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