using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class ExpressionOperatorChain
    {
        private readonly List<object> _flatList = new List<object>();

        public void Add(IExpressionSyntax expression)
        {
            _flatList.Add(expression);
        }

        public void Add(Token @operator)
        {
            _flatList.Add(@operator);
        }

        public BinaryExpressionSyntax SolveOperatorPrecendence(TokenType[] precedence)
        {
            foreach (var @operator in precedence)
            {
                int opIndex;
                while ((opIndex = GetOperatorIndex(@operator)) != 0)
                {
                    var expression = GetBinaryExpression(opIndex);
                    ReplaceInFlatList(expression, opIndex);
                }
            }

            Debug.Assert(_flatList.Count == 1);
            return (BinaryExpressionSyntax)_flatList[0];
        }

        private void ReplaceInFlatList(BinaryExpressionSyntax expression, int opIndex)
        {
            _flatList.RemoveAt(opIndex - 1);
            _flatList.RemoveAt(opIndex - 1);
            _flatList[opIndex - 1] = expression;
        }

        private int GetOperatorIndex(TokenType @operator)
        {
            var next = _flatList.OfType<Token>().FirstOrDefault(x => x.Type == @operator);
            if (next == null)
            {
                return 0;
            }
            return _flatList.IndexOf(next);
        }

        private BinaryExpressionSyntax GetBinaryExpression(int index)
        {
            var leftExpression = (IExpressionSyntax)_flatList[index - 1];
            var op = (Token)_flatList[index];
            var rightExpression = (IExpressionSyntax)_flatList[index + 1];
            return new BinaryExpressionSyntax(leftExpression, rightExpression, op);
        }
    }
}