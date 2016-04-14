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
                foreach (var namespaceSyntax in unitSyntax.NamespaceMember)
                {
                    var boundNamespace = new BoundNamespace(namespaceSyntax.NamespaceName.Value, namespaceSyntax);
                    boundCompilationUnit.Namespaces.Add(boundNamespace);
                    foreach (var classSyntax in namespaceSyntax.Classes)
                    {
                        var boundType = new BoundType(classSyntax.ClassName.Value, classSyntax);
                        boundType.FieldsInternal.AddRange(
                            classSyntax.FieldMember.Select(x => new BoundField(x.FieldName.Value, x)));
                        boundType.FunctionsInternal.AddRange(
                            classSyntax.FunctionMember.Select(x => new BoundFunction(x.FunctionName.Value, x)));
                        boundNamespace.TypesInternal.Add(boundType);
                    }
                    boundNamespace.EnumsInternal.AddRange(
                        namespaceSyntax.Enums.Select(x => new BoundEnum(x.EnumName, x)));
                }
                compilationUnits.Add(boundCompilationUnit);
            }
            return compilationUnits;
        }
    }

    public class BoundEnum : BoundNode
    {
        public BoundEnum(Token enumName, EnumSyntax enumSyntax) : base(enumSyntax)
        {
            EnumName = enumName;
        }

        public Token EnumName { get; set; }
    }
}