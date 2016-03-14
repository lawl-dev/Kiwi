using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ExpressionFunctionSyntax : FunctionSyntax
    {
        public ExpressionFunctionSyntax(
            Token functionName,
            List<ParameterSyntax> parameterList,
            ReturnStatementSyntax returnStatement)
            : base(functionName, parameterList, new List<IStatementSyntax>() { returnStatement })
        {
        }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}