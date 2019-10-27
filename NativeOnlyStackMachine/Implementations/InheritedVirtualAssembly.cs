using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Policy;

namespace NativeOnlyStackMachine
{
    public class InheritedVirtualAssembly : System.Reflection.Assembly, IClrVirtualAssembly
    {
        public override string CodeBase => base.CodeBase;

        public override string EscapedCodeBase => base.EscapedCodeBase;

        public override string FullName => base.FullName;

        public override MethodInfo EntryPoint => base.EntryPoint;

        public override IEnumerable<Type> ExportedTypes => base.ExportedTypes;

        public override IEnumerable<TypeInfo> DefinedTypes => base.DefinedTypes;

        public override Evidence Evidence => base.Evidence;

        public override PermissionSet PermissionSet => base.PermissionSet;

        public override SecurityRuleSet SecurityRuleSet => base.SecurityRuleSet;

        public override Module ManifestModule => base.ManifestModule;

        public override IEnumerable<CustomAttributeData> CustomAttributes => base.CustomAttributes;

        public override bool ReflectionOnly => base.ReflectionOnly;

        public override IEnumerable<Module> Modules => base.Modules;

        public override string Location => base.Location;

        public override string ImageRuntimeVersion => base.ImageRuntimeVersion;

        public override bool GlobalAssemblyCache => base.GlobalAssemblyCache;

        public override long HostContext => base.HostContext;

        public override bool IsDynamic => base.IsDynamic;

        public override event ModuleResolveEventHandler ModuleResolve;

        public override object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
        {
            return base.CreateInstance(typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes);
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return base.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return base.GetCustomAttributes(attributeType, inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return base.GetCustomAttributesData();
        }

        public override Type[] GetExportedTypes()
        {
            return base.GetExportedTypes();
        }

        public override FileStream GetFile(string name)
        {
            return base.GetFile(name);
        }

        public override FileStream[] GetFiles()
        {
            return base.GetFiles();
        }

        public override FileStream[] GetFiles(bool getResourceModules)
        {
            return base.GetFiles(getResourceModules);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override Module[] GetLoadedModules(bool getResourceModules)
        {
            return base.GetLoadedModules(getResourceModules);
        }

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            return base.GetManifestResourceInfo(resourceName);
        }

        public override string[] GetManifestResourceNames()
        {
            return base.GetManifestResourceNames();
        }

        public override Stream GetManifestResourceStream(Type type, string name)
        {
            return base.GetManifestResourceStream(type, name);
        }

        public override Stream GetManifestResourceStream(string name)
        {
            return base.GetManifestResourceStream(name);
        }

        public override Module GetModule(string name)
        {
            return base.GetModule(name);
        }

        public override Module[] GetModules(bool getResourceModules)
        {
            return base.GetModules(getResourceModules);
        }

        public override AssemblyName GetName()
        {
            return base.GetName();
        }

        public override AssemblyName GetName(bool copiedName)
        {
            return base.GetName(copiedName);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override AssemblyName[] GetReferencedAssemblies()
        {
            return base.GetReferencedAssemblies();
        }

        public override Assembly GetSatelliteAssembly(CultureInfo culture)
        {
            return base.GetSatelliteAssembly(culture);
        }

        public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
        {
            return base.GetSatelliteAssembly(culture, version);
        }

        public override Type GetType(string name)
        {
            return base.GetType(name);
        }

        public override Type GetType(string name, bool throwOnError)
        {
            return base.GetType(name, throwOnError);
        }

        public override Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            return base.GetType(name, throwOnError, ignoreCase);
        }

        public override Type[] GetTypes()
        {
            return base.GetTypes();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return base.IsDefined(attributeType, inherit);
        }

        public override Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
        {
            return base.LoadModule(moduleName, rawModule, rawSymbolStore);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
