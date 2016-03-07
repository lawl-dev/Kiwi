using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FieldSyntax : ISyntaxBase
    {
        public Token FieldTypeQualifier { get; private set; }
        public Token FieldType { get; private set; }
        public Token FieldName { get; private set; }
        public IExpressionSyntax FieldInitializer { get; set; }

        public FieldSyntax(Token fieldTypeQualifier, Token fieldType, Token fieldName, IExpressionSyntax fieldInitializer)
        {
            FieldTypeQualifier = fieldTypeQualifier;
            FieldType = fieldType;
            FieldName = fieldName;
            FieldInitializer = fieldInitializer;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}