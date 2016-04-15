using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class DataClassFunctionSyntax : FunctionSyntax
    {
        public DataClassFunctionSyntax(
            Token functionName,
            List<ParameterSyntax> parameter,
            List<IStatementSyntax> statements,
            DataSyntax dataClassSyntax) : base(functionName, parameter, statements)
        {
            DataClassSyntax = dataClassSyntax;
        }

        public DataSyntax DataClassSyntax { get; }

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}