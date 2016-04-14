using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.LanguageTypes
{
    public class IntSpecialType : IType
    {
        public ReadOnlyCollection<IField> Fields
            =>
                new ReadOnlyCollection<IField>(
                    new List<IField> { new SpecialField("Max", VariableQualifier.Const, new IntSpecialType()) });

        public IReadOnlyCollection<IFunction> Functions { get; }
        IReadOnlyCollection<IField> IType.Fields => Fields;
    }
}