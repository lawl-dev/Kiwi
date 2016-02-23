using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxClassAst : SyntaxAstBase
    {
        public Token ClassName { get; set; }
        public bool Inherit { get; set; }
        public Token DescriptorName { get; set; }
        public List<SyntaxAstBase> Inner { get; set; }

        public SyntaxClassAst(Token className, bool inherit, Token descriptorName, List<SyntaxAstBase> inner)
        {
            ClassName = className;
            Inherit = inherit;
            DescriptorName = descriptorName;
            Inner = inner;
        }
    }
}