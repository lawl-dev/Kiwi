using System.Collections.Generic;
using System.Linq;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxNamespaceAst : SyntaxAstBase
    {
        public List<SyntaxAstBase> ClassSyntax { get; private set; }
        public List<SyntaxAstBase> DataSyntax { get; private set; }
        public List<SyntaxAstBase> EnumSyntax { get; private set; }

        public SyntaxNamespaceAst(List<SyntaxAstBase> innerSyntax)
        {
            ClassSyntax = innerSyntax.Where(x => x is SyntaxClassAst).ToList();
            DataSyntax = innerSyntax.Where(x => x is SyntaxDataAst).ToList();
            EnumSyntax = innerSyntax.Where(x => x is SyntaxEnumAst).ToList();
        }
    }
}