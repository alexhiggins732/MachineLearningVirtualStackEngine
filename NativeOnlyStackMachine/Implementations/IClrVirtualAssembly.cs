using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Policy;

namespace NativeOnlyStackMachine
{
    public interface IClrVirtualAssembly
    {
        string CodeBase { get; }
        IEnumerable<CustomAttributeData> CustomAttributes { get; }
        IEnumerable<TypeInfo> DefinedTypes { get; }
        MethodInfo EntryPoint { get; }
        string EscapedCodeBase { get; }
        Evidence Evidence { get; }
        IEnumerable<Type> ExportedTypes { get; }
        string FullName { get; }
        bool GlobalAssemblyCache { get; }
        long HostContext { get; }
        string ImageRuntimeVersion { get; }
        bool IsDynamic { get; }
        string Location { get; }
        Module ManifestModule { get; }
        IEnumerable<Module> Modules { get; }
        PermissionSet PermissionSet { get; }
        bool ReflectionOnly { get; }
        SecurityRuleSet SecurityRuleSet { get; }

        event ModuleResolveEventHandler ModuleResolve;

        object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes);
        bool Equals(object o);
        object[] GetCustomAttributes(bool inherit);
        object[] GetCustomAttributes(Type attributeType, bool inherit);
        IList<CustomAttributeData> GetCustomAttributesData();
        Type[] GetExportedTypes();
        FileStream GetFile(string name);
        FileStream[] GetFiles();
        FileStream[] GetFiles(bool getResourceModules);
        int GetHashCode();
        Module[] GetLoadedModules(bool getResourceModules);
        ManifestResourceInfo GetManifestResourceInfo(string resourceName);
        string[] GetManifestResourceNames();
        Stream GetManifestResourceStream(string name);
        Stream GetManifestResourceStream(Type type, string name);
        Module GetModule(string name);
        Module[] GetModules(bool getResourceModules);
        AssemblyName GetName();
        AssemblyName GetName(bool copiedName);
        void GetObjectData(SerializationInfo info, StreamingContext context);
        AssemblyName[] GetReferencedAssemblies();
        Assembly GetSatelliteAssembly(CultureInfo culture);
        Assembly GetSatelliteAssembly(CultureInfo culture, Version version);
        Type GetType(string name);
        Type GetType(string name, bool throwOnError);
        Type GetType(string name, bool throwOnError, bool ignoreCase);
        Type[] GetTypes();
        bool IsDefined(Type attributeType, bool inherit);
        Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore);
        string ToString();
    }
}