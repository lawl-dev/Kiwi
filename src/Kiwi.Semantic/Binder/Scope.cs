using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    internal class Scope
    {
        private readonly Dictionary<string, BoundType> _typeSymbols = new Dictionary<string, BoundType>(); 
        private readonly Dictionary<string, BoundField> _fieldSymbols = new Dictionary<string, BoundField>(); 
        private readonly Dictionary<string, BoundParameter> _parameterSymbols = new Dictionary<string, BoundParameter>(); 
        private readonly Dictionary<string, List<BoundFunction>> _functionSymbols = new Dictionary<string, List<BoundFunction>>(); 

        public void Add(string value, BoundType boundType)
        {
            _typeSymbols.Add(value, boundType);
        }

        public IBoundMember Lookup(string value)
        {
            if (_typeSymbols.ContainsKey(value))
            {
                return _typeSymbols[value];
            }
            if (_fieldSymbols.ContainsKey(value))
            {
                return _fieldSymbols[value];
            }
            if (_parameterSymbols.ContainsKey(value))
            {
                return _parameterSymbols[value];
            }
            return null;
        }

        public void Add(string value, BoundField boundField)
        {
            _fieldSymbols.Add(value, boundField);
        }

        public void Add(string value, BoundFunction boundFunction)
        {
            if (!_functionSymbols.ContainsKey(value))
            {
                _functionSymbols.Add(value, new List<BoundFunction>());
            }
            _functionSymbols[value].Add(boundFunction);
        }

        public BoundFunction LookupFunction(string value, List<IType> parameterTypes)
        {
            return _functionSymbols[value].SingleOrDefault(x=>MatchTypes(x.Parameter, parameterTypes));
        }

        private static bool MatchTypes(List<BoundParameter> parameter, List<IType> parameterTypes)
        {
            return parameterTypes.Count == parameter.Count
                   && parameterTypes.Select((x, i) => x == parameter[i].Type).All(x => x);
        }

        public void Add(string value, BoundParameter boundParameter)
        {
            _parameterSymbols.Add(value, boundParameter);
        }
    }
}