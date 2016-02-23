using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxFunctionAst : SyntaxAstBase
    {
        public Token FunctionName { get; set; }
        public List<SyntaxAstBase> ParameterList { get; set; }
        public bool IsVoid { get; set; }
        public SyntaxAstBase DataClassDeclarationSyntax { get; set; }
        public Token ReturnTypeName { get; set; }
        public List<SyntaxAstBase> InnerSyntax { get; set; }

        public SyntaxFunctionAst(Token functionName, List<SyntaxAstBase> parameterList, bool isVoid, SyntaxAstBase dataClassDeclarationSyntax, Token returnTypeName, List<SyntaxAstBase> innerSyntax)
        {
            FunctionName = functionName;
            ParameterList = parameterList;
            IsVoid = isVoid;
            DataClassDeclarationSyntax = dataClassDeclarationSyntax;
            ReturnTypeName = returnTypeName;
            InnerSyntax = innerSyntax;
        }
    }
}