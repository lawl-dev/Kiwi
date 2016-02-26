using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class VariableAssignmentStatementSyntax : IStatetementSyntax
    {
        public Token VariableName { get; }
        public Token Operator { get; }
        public IExpressionSyntax ToAssign { get; }

        public VariableAssignmentStatementSyntax(Token variableName, Token @operator, IExpressionSyntax toAssign)
        {
            VariableName = variableName;
            Operator = @operator;
            ToAssign = toAssign;
        }
    }
}