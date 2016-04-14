using System;

namespace Kiwi.Semantic.Binder
{
    public class KiwiSemanticException : Exception
    {
        public KiwiSemanticException(string message) : base(message)
        {
        }
    }
}