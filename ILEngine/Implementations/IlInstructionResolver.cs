using System;
using System.Reflection;

namespace ILEngine
{
    public class IlInstructionResolver : IIlInstructionResolver
    {
        public static IlInstructionResolver ExecutingAssemblyResolver = new IlInstructionResolver(Assembly.GetExecutingAssembly().ManifestModule);
        //public static IlInstructionResolver CallingAssemblyResolver = new IlInstructionResolver(Assembly.GetCallingAssembly().ManifestModule);
        //public static IlInstructionResolver EntryAssemblyResolver = new IlInstructionResolver(Assembly.GetEntryAssembly().ManifestModule);
        public Module Module { get; }
        public Func<int, FieldInfo> ResolveFieldToken { get; }
        public Func<int, MemberInfo> ResolveMemberToken { get; }
        public Func<int, MethodBase> ResolveMethodToken { get; }
        public Func<int, byte[]> ResolveSignatureToken { get; }
        public Func<int, string> ResolveStringToken { get; }
        public Func<int, Type> ResolveTypeToken { get; }

        public IlInstructionResolver(MethodInfo method) : this(method.DeclaringType.Assembly.ManifestModule) { }
        public IlInstructionResolver(Module manifestModule)
        {
            this.Module = manifestModule;
            //var module = System.Reflection.Assembly.GetExecutingAssembly().ManifestModule;
            this.ResolveFieldToken = (Func<int, System.Reflection.FieldInfo>)Module.ResolveField;
            this.ResolveMemberToken = (Func<int, System.Reflection.MemberInfo>)Module.ResolveMember;
            this.ResolveMethodToken = (Func<int, System.Reflection.MethodBase>)Module.ResolveMethod;
            this.ResolveSignatureToken = (Func<int, byte[]>)Module.ResolveSignature;
            this.ResolveStringToken = (Func<int, string>)Module.ResolveString;
            this.ResolveTypeToken = (Func<int, Type>)Module.ResolveType;

        }
    }
}
