using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Semantic.Binder.LanguageTypes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class BindingContextService
    {
        private readonly Stack<Scope> _locals = new Stack<Scope>();
        private readonly Dictionary<string, BoundType> _types = new Dictionary<string, BoundType>();
        private Scope CurrentScope => _locals.Any() ? _locals.Peek() : null;

        public void Load(BoundNamespace boundNamespace)
        {
            foreach (var boundType in boundNamespace.TypesInternal)
            {
                _types.Add(boundType.Name, boundType);
            }
        }

        public void Unload(BoundNamespace boundNamespace)
        {
            foreach (var boundType in boundNamespace.TypesInternal)
            {
                _types.Remove(boundType.Name);
            }
        }

        public IType LookupType(string value)
        {
            return IsStandardType(value) ? GetStandardType(value) : _types[value];
        }

        private IType GetStandardType(string value)
        {
            StandardTypes res;
            Enum.TryParse(value, true, out res);
            return GetStandardType(res);
        }

        public IType GetStandardType(StandardTypes type)
        {
            switch (type)
            {
                case StandardTypes.Bool:
                    return new BoolSpecialType();
                case StandardTypes.Int:
                    return new IntSpecialType();
            }
            throw new NotImplementedException();
        }

        private static bool IsStandardType(string value)
        {
            StandardTypes res;
            return Enum.TryParse(value, true, out res);
        }

        public void AddLocal(string value, IBoundMember boundParameter)
        {
            CurrentScope.Add(value, boundParameter);
        }

        public void EnterScope()
        {
            _locals.Push(new Scope(CurrentScope));
        }

        public void ExitScope()
        {
            _locals.Pop();
        }

        public IBoundMember GetLocal(string value)
        {
            return CurrentScope.Get(value);
        }
    }
}