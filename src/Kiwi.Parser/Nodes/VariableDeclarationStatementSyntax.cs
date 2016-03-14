using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class VariableDeclarationStatementSyntax : IStatementSyntax
    {
        public Token VariableQualifier { get; }
        public Token Identifier { get; }
        public IExpressionSyntax InitExpression { get; }

        public VariableDeclarationStatementSyntax(Token variableQualifier, Token identifier, IExpressionSyntax initExpression)
        {
            VariableQualifier = variableQualifier;
            Identifier = identifier;
            InitExpression = initExpression;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}