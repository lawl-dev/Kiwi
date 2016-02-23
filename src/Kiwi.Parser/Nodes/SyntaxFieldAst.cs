using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxFieldAst : SyntaxAstBase
    {
        public Token FieldTypeQualifier { get; set; }
        public Token FieldType { get; set; }
        public Token FieldName { get; set; }
        public SyntaxAstBase FieldInitializer { get; set; }

        public SyntaxFieldAst(Token fieldTypeQualifier, Token fieldType, Token fieldName, SyntaxAstBase fieldInitializer)
        {
            FieldTypeQualifier = fieldTypeQualifier;
            FieldType = fieldType;
            FieldName = fieldName;
            FieldInitializer = fieldInitializer;
        }
    }
}