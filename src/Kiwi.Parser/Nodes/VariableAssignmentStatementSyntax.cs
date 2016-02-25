using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class VariableAssignmentStatementSyntax : IStatetementSyntax
    {
        public Token VariableName { get; }
        public Token Operator { get; }
        public IExpressionSyntax Intializer { get; }

        public VariableAssignmentStatementSyntax(Token variableName, Token @operator, IExpressionSyntax intializer)
        {
            VariableName = variableName;
            Operator = @operator;
            Intializer = intializer;
        }
    }
}