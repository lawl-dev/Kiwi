using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FieldSyntax : ISyntaxBase
    {
        public FieldSyntax(Token fieldTypeQualifier, Token fieldName, IExpressionSyntax fieldInitializer)
        {
            FieldTypeQualifier = fieldTypeQualifier;
            FieldName = fieldName;
            FieldInitializer = fieldInitializer;
        }

        public Token FieldTypeQualifier { get; private set; }
        public Token FieldName { get; private set; }
        public IExpressionSyntax FieldInitializer { get; set; }
        public SyntaxType SyntaxType => SyntaxType.FieldSyntax;

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}