using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FunctionSyntax : ISyntaxBase
    {
        public Token FunctionName { get; private set; }
        public List<ParameterSyntax> ParameterList { get; private set; }
        public List<ISyntaxBase> Member { get;  }
        public List<IStatetementSyntax> StatementMember => Member.OfType<IStatetementSyntax>().ToList();

        public FunctionSyntax(
            Token functionName,
            List<ParameterSyntax> parameterList,
            List<ISyntaxBase> member)
        {
            FunctionName = functionName;
            ParameterList = parameterList;
            Member = member;
        }
    }

    public class DataClassFunctionSyntax : FunctionSyntax
    {
        public DataSyntax DataClassSyntax { get; }

        public DataClassFunctionSyntax(Token functionName, List<ParameterSyntax> parameter, List<ISyntaxBase> member, DataSyntax dataClassSyntax) : base(functionName, parameter, member)
        {
            DataClassSyntax = dataClassSyntax;
        }
    }

    public class ReturnFunctionSyntax : FunctionSyntax
    {
        public Token ReturnType { get; }

        public ReturnFunctionSyntax(Token functionName, List<ParameterSyntax> parameterList, List<ISyntaxBase> member, Token returnType) : base(functionName, parameterList, member)
        {
            ReturnType = returnType;
        }
    }
}