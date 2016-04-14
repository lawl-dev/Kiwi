using System.Collections.Generic;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.LanguageTypes
{
    public class SpecialFunction : IFunction
    {
        public SpecialFunction(string name, List<SpecialParameter> parameters, IType returnType)
        {
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public List<SpecialParameter> Parameters { get; set; }

        public string Name { get; }
        public IEnumerable<IParameter> Parameter { get; set; }
        public IType ReturnType { get; set; }
    }
}