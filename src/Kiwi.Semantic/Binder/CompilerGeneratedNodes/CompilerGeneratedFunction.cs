using System.Collections.Generic;
using System.Linq;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.CompilerGeneratedNodes
{
    public class CompilerGeneratedFunction : IFunction
    {
        public CompilerGeneratedFunction(string name, List<SpecialParameter> parameters, IType returnType)
        {
            Name = name;
            Parameter = parameters;
            ReturnType = returnType;
        }
        

        public string Name { get; }
        public IEnumerable<IParameter> Parameter { get; set; }
        public IType ReturnType { get; set; }
        public IType Type => new FunctionCompilerGeneratedType(Parameter.Select(x=>x.Type).ToList(), ReturnType);
    }
}