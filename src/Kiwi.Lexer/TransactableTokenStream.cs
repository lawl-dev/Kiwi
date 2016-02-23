using System.Globalization;
using System.Linq;
using Kiwi.Common;

namespace Kiwi.Lexer
{
    internal sealed class TransactableTokenStream : TransactableStreamBase<string>
    {
        public TransactableTokenStream(string source)
            : base(() => source.ToCharArray().Select(c => c.ToString(CultureInfo.InvariantCulture)).ToList())
        {
        }
    }
}