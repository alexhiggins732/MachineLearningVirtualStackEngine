using System;
using System.Reflection;

namespace ILEngine
{

    public struct ILVariable
    {
        public int Index;
        private string name;
        public string Name { get { return name ?? (name = Index.ToString()); } set { name = value; } }
        public Type Type;
        public object Value;

        public ILVariable(LocalVariableInfo lcl) : this()
        {
            CopyFrom(lcl);
        }

        public override string ToString() => $"Loc.{Name} ({(Type?.Name ?? "")}) {{{(Value is null ? "null" : Value)}}}";

        public void CopyFrom(LocalVariableInfo localVariableInfo) => CopyFrom(localVariableInfo, false);


        public void CopyFrom(LocalVariableInfo localVariableInfo, bool initLocals)
        {
            Index = localVariableInfo.LocalIndex;
            Type = localVariableInfo.LocalType;
            if (initLocals && Type.IsValueType)
                Value = Activator.CreateInstance(Type);
        }
    }
}
