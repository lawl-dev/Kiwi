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
}