﻿using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ClassSyntax : ISyntaxBase
    {
        public Token ClassName { get; private set; }
        public Token DescriptorName { get; private set; }
        public List<ISyntaxBase> Member { get; set; }
        public List<FieldSyntax> FieldMember => Member.OfType<FieldSyntax>().ToList();
        public List<ConstructorSyntax> ConstructorMember => Member.OfType<ConstructorSyntax>().ToList();
        public List<FunctionSyntax> FunctionMember => Member.OfType<FunctionSyntax>().ToList();
        public SyntaxType SyntaxType => SyntaxType.ClassSyntax;

        public ClassSyntax(Token className, Token descriptorName, List<ISyntaxBase> member)
        {
            ClassName = className;
            DescriptorName = descriptorName;
            Member = member;
        }
        
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