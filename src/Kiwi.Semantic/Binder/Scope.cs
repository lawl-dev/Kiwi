using System.Collections.Generic;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class Scope
    {
        private readonly Dictionary<string, IBoundMember> _locals = new Dictionary<string, IBoundMember>();
        private readonly Scope _parent;

        public Scope(Scope parent)
        {
            _parent = parent;
        }

        public void Add(string value, IBoundMember member)
        {
            if (Get(value) != null)
            {
                throw new KiwiSemanticException($"{value} already defined.");
            }
            _locals.Add(value, member);
        }

        public IBoundMember Get(string value)
        {
            var boundMember = _parent?.Get(value);
            if (boundMember != null)
            {
                return boundMember;
            }

            _locals.TryGetValue(value, out boundMember);
            return boundMember;
        }
    }
}