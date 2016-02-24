using Kiwi.Lexer;

namespace Kiwi.Parser
{
    internal class VariableDeclarationStatementSyntax : IStatetementSyntax
    {
        public Token VariableQualifier { get; }
        public Token VariableName { get; }
        public IExpressionSyntax Initializer { get; }

        public VariableDeclarationStatementSyntax(Token variableQualifier, Token variableName, IExpressionSyntax initializer)
        {
            VariableQualifier = variableQualifier;
            VariableName = variableName;
            Initializer = initializer;
        }
    }
}