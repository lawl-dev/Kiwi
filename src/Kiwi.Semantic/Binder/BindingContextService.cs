using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class BindingContextService
    {
        private readonly Stack<Scope> _locals = new Stack<Scope>();
        private readonly Dictionary<string, BoundType> _types = new Dictionary<string, BoundType>();
        private BoundType _currentClass;
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
            switch ((StandardTypes)Enum.Parse(typeof(StandardTypes), value, true))
            {
                case StandardTypes.Bool:
                    return new BoolCompilerGeneratedType();
                case StandardTypes.Type:
                    return new TypeCompilerGeneratedType();
                case StandardTypes.Int:
                    return new IntCompilerGeneratedType();
                case StandardTypes.Float:
                    return new FloatCompilerGeneratedType();
                case StandardTypes.Void:
                    return new VoidCompilerGeneratedType();
                case StandardTypes.String:
                    return new StringCompilerGeneratedType();
                default:
                    throw new NotImplementedException();
            }
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

        public void EnterClass(BoundType @class)
        {
            _currentClass = @class;
        }

        public void ExitClass()
        {
            _currentClass = null;
        }

        public IEnumerable<IFunction> GetAvailableFunctions()
        {
            return _currentClass.Functions;
        }

        public IEnumerable<IField> GetAvailableFields()
        {
            return _currentClass.Fields;
        }
    }
}