using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class ScopeManager
    {
        private readonly Dictionary<string, IType> _types = new Dictionary<string, IType>();
        public Dictionary<string, IBoundMember> _locals = new Dictionary<string, IBoundMember>();

        public void Load(BoundNamespace boundNamespace)
        {
            foreach (var boundType in boundNamespace.TypesInternal)
            {
                _types.Add(boundType.Name.Value, boundType);
            }
        }

        public IType LookupType(string value)
        {
            return IsStandardType(value) ? GetStandardType(value) : _types[value];
        }

        private static IType GetStandardType(string value)
        {
            StandardTypes res;
            Enum.TryParse(value, true, out res);
            return new StandardType(res);
        }

        private static bool IsStandardType(string value)
        {
            StandardTypes res;
            return Enum.TryParse(value, true, out res);
        }

        public void AddLocal(string value, IBoundMember boundParameter)
        {
            _locals.Add(value, boundParameter);
        }
    }
}