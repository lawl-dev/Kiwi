using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class VariableDeclarationStatementSyntax : IStatetementSyntax
    {
        public Token VariableQualifier { get; }
        public List<Token> VariableNames { get; }
        public IExpressionSyntax Initializer { get; }

        public VariableDeclarationStatementSyntax(Token variableQualifier, List<Token> variableNames, IExpressionSyntax initializer)
        {
            VariableQualifier = variableQualifier;
            VariableNames = variableNames;
            Initializer = initializer;
        }
    }
}