using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ReturnFunctionSyntax : FunctionSyntax
    {
        public TypeSyntax ReturnType { get; }

        public ReturnFunctionSyntax(Token functionName, List<ParameterSyntax> parameterList, List<ISyntaxBase> member, TypeSyntax returnType) : base(functionName, parameterList, member)
        {
            ReturnType = returnType;
        }
    }
}