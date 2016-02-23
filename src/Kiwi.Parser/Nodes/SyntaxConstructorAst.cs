using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxConstructorAst : SyntaxAstBase
    {
        public List<SyntaxAstBase> ArgList { get; set; }
        public List<SyntaxAstBase> InnerSyntax { get; set; }

        public SyntaxConstructorAst(List<SyntaxAstBase> argList, List<SyntaxAstBase> innerSyntax)
        {
            ArgList = argList;
            InnerSyntax = innerSyntax;
        }
    }
}