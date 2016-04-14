using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class VariablesDeclarationStatementSyntax : IStatementSyntax
    {
        public VariablesDeclarationStatementSyntax(List<VariableDeclarationStatementSyntax> declarations)
        {
            Declarations = declarations;
        }

        public List<VariableDeclarationStatementSyntax> Declarations { get; set; }

        public SyntaxType SyntaxType => SyntaxType.VariablesDeclarationStatementSyntax;

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