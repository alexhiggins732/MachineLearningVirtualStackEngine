using System;
using System.Diagnostics.CodeAnalysis;

namespace ILEngine
{
    //TODO: Implement or remove
    [ExcludeFromCodeCoverage]
    public struct ILParameter
    {
        public string Name;
        public Type Type;
        public object Value;
        public override string ToString() => $"{(Type?.Name ?? "")} {(Name ?? "")} = {(Value is null ? "null" : Value)}";

    }
}
