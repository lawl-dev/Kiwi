using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class VariableDeclarationStatementSyntax : IStatementSyntax
    {
        public VariableDeclarationStatementSyntax(
            Token variableQualifier,
            Token identifier,
            IExpressionSyntax initExpression)
        {
            VariableQualifier = variableQualifier;
            Identifier = identifier;
            InitExpression = initExpression;
        }

        public Token VariableQualifier { get; }
        public Token Identifier { get; }
        public IExpressionSyntax InitExpression { get; }

        public SyntaxType SyntaxType => SyntaxType.VariableDeclarationStatementSyntax;

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