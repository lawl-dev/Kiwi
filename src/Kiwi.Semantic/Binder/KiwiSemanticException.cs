using System;

namespace Kiwi.Semantic.Binder
{
    internal class KiwiSemanticException : Exception
    {
        public KiwiSemanticException(string message) : base(message)
        {
        }
    }
}