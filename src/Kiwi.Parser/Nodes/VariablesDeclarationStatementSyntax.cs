using System;
using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class VariablesDeclarationStatementSyntax : IStatementSyntax
    {
        public List<VariableDeclarationStatementSyntax> Declarations { get; set; }

        public VariablesDeclarationStatementSyntax(List<VariableDeclarationStatementSyntax> declarations)
        {
            Declarations = declarations;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}