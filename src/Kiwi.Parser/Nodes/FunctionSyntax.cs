using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FunctionSyntax : ISyntaxBase
    {
        public Token FunctionName { get; private set; }
        public List<ISyntaxBase> ParameterList { get; private set; }
        public ISyntaxBase DataClassDeclarationSyntax { get; private set; }
        public Token ReturnTypeName { get; private set; }
        public List<ISyntaxBase> Member { get;  }
        public List<IStatetementSyntax> StatementMember => Member.OfType<IStatetementSyntax>().ToList();

        public FunctionSyntax(
            Token functionName,
            List<ISyntaxBase> parameterList,
            ISyntaxBase dataClassDeclarationSyntax,
            Token returnTypeName,
            List<ISyntaxBase> member)
        {
            FunctionName = functionName;
            ParameterList = parameterList;
            DataClassDeclarationSyntax = dataClassDeclarationSyntax;
            ReturnTypeName = returnTypeName;
            Member = member;
        }
    }
}