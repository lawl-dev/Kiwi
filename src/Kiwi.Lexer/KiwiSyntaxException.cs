using System;

namespace Kiwi.Lexer
{
    internal class KiwiSyntaxException : Exception
    {
        public KiwiSyntaxException(string message) : base(message)
        {
        }
    }
}