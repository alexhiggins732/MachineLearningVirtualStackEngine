using System;
using System.Reflection;

namespace ILEngine
{

    public interface IILInstructionResolver
    {
        Module Module { get; }
        Func<int, FieldInfo> ResolveFieldToken { get; }
        Func<int, MemberInfo> ResolveMemberToken { get; }
        Func<int, MethodBase> ResolveMethodToken { get; }
        Func<int, byte[]> ResolveSignatureToken { get; }
        Func<int, string> ResolveStringToken { get; }
        Func<int, Type> ResolveTypeToken { get; }
    }
}
