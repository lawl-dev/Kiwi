using System;

namespace Kiwi.Common
{
    public class KiwiSyntaxException : Exception
    {
        public KiwiSyntaxException(string message) : base(message)
        {
        }
    }
}