using System.Linq;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    public class BasicSymbolService
    {
        public BoundCompilationUnit CreateBasicModel(CompilationUnitSyntax compilationUnitSyntax)
        {
            var boundCompilationUnit = new BoundCompilationUnit(compilationUnitSyntax);
            foreach (var namespaceSyntax in compilationUnitSyntax.NamespaceMember)
            {
                var boundNamespace = new BoundNamespace(namespaceSyntax.NamespaceName, namespaceSyntax);
                boundCompilationUnit.Namespaces.Add(boundNamespace);
                foreach (var classSyntax in namespaceSyntax.Classes)
                {
                    var boundType = new BoundType(classSyntax.ClassName, classSyntax);
                    boundType.FieldsInternal.AddRange(classSyntax.FieldMember.Select(x => new BoundField(x.FieldName, x)));
                    boundType.FunctionsInternal.AddRange(classSyntax.FunctionMember.Select(x => new BoundFunction(x.FunctionName, x)));
                    boundNamespace.TypesInternal.Add(boundType);
                }
                boundNamespace.EnumsInternal.AddRange(namespaceSyntax.Enums.Select(x => new BoundEnum(x.EnumName, x)));
            }
            return boundCompilationUnit;
        }
    }

    public class BoundEnum : BoundNode
    {
        public Token EnumName { get; set; }

        public BoundEnum(Token enumName, EnumSyntax enumSyntax) : base(enumSyntax)
        {
            EnumName = enumName;
        }
    }
}