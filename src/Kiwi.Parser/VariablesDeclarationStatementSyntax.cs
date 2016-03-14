using System;
using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    internal class VariablesDeclarationStatementSyntax : IStatementSyntax
    {
        public List<VariableDeclarationStatementSyntax> Declarations { get; set; }

        public VariablesDeclarationStatementSyntax(List<VariableDeclarationStatementSyntax> declarations)
        {
            Declarations = declarations;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}