using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class BasicSymbolService
    {
        public List<BoundCompilationUnit> CreateBasicModel(List<CompilationUnitSyntax> compilationUnitSyntax)
        {
            var compilationUnits = new List<BoundCompilationUnit>();
            foreach (var unitSyntax in compilationUnitSyntax)
            {
                var boundCompilationUnit = new BoundCompilationUnit(unitSyntax);
                foreach (var namespaceSyntax in unitSyntax.Namespaces)
                {
                    var boundNamespace = new BoundNamespace(namespaceSyntax.Name.Value, namespaceSyntax);
                    boundCompilationUnit.Namespaces.Add(boundNamespace);
                    foreach (var classSyntax in namespaceSyntax.Classes)
                    {
                        var boundType = new BoundType(classSyntax.Name.Value, classSyntax);
                        boundType.FieldsInternal.AddRange(
                            classSyntax.Fields.Select(x => new BoundField(x.Name.Value, x)));
                        boundType.FunctionsInternal.AddRange(
                            classSyntax.Functions.Select(x => new BoundFunction(x.Name.Value, x)));
                       boundNamespace.TypesInternal.Add(boundType);
                    }
                    boundNamespace.EnumsInternal.AddRange(
                        namespaceSyntax.Enums.Select(x => new BoundEnum(x.EnumName.Value, x)));
                }
                compilationUnits.Add(boundCompilationUnit);
            }
            return compilationUnits;
        }
    }
}