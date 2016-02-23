using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class SyntaxCompilationUnitAst : SyntaxAstBase
    {
        public List<SyntaxAstBase> InnerSyntax { get; private set; }

        public SyntaxCompilationUnitAst(List<SyntaxAstBase> innerSyntax)
        {
            InnerSyntax = innerSyntax;
        }
    }
}