using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ImplicitParameterTypeAnonymousFunctionExpressionSyntax : IExpressionSyntax
    {
        public List<IExpressionSyntax> Parameter { get; }
        public List<IStatementSyntax> Statements { get; }

        public ImplicitParameterTypeAnonymousFunctionExpressionSyntax(List<IExpressionSyntax> parameter, List<IStatementSyntax> statements)
        {
            Parameter = parameter;
            Statements = statements;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}