using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class DataClassFunctionSyntax : FunctionSyntax
    {
        public DataSyntax DataClassSyntax { get; }

        public DataClassFunctionSyntax(Token functionName, List<ParameterSyntax> parameter, List<ISyntaxBase> member, DataSyntax dataClassSyntax) : base(functionName, parameter, member)
        {
            DataClassSyntax = dataClassSyntax;
        }
    }
}