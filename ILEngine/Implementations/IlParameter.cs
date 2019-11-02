using System;
namespace ILEngine
{
    //TODO: Implement or remove
    public struct IlParameter
    {
        public string Name;
        public Type Type;
        public object Value;
        public override string ToString() => $"{(Type?.Name ?? "")} {(Name ?? "")} = {(Value is null ? "null" : Value)}";

    }
}
