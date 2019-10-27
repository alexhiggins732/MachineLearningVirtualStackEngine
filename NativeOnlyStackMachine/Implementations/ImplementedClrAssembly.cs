using System;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Policy;

namespace NativeOnlyStackMachine
{
    public class ImplementedClrAssembly : System.Runtime.InteropServices._Assembly
    {
        public string CodeBase => throw new NotImplementedException();

        public string EscapedCodeBase => throw new NotImplementedException();

        public string FullName => throw new NotImplementedException();

        public MethodInfo EntryPoint => throw new NotImplementedException();

        public string Location => throw new NotImplementedException();

        public Evidence Evidence => throw new NotImplementedException();

        public bool GlobalAssemblyCache => throw new NotImplementedException();

        public event ModuleResolveEventHandler ModuleResolve;

        public object CreateInstance(string typeName)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(string typeName, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
        {
            throw new NotImplementedException();
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public Type[] GetExportedTypes()
        {
            throw new NotImplementedException();
        }

        public FileStream GetFile(string name)
        {
            throw new NotImplementedException();
        }

        public FileStream[] GetFiles()
        {
            throw new NotImplementedException();
        }

        public FileStream[] GetFiles(bool getResourceModules)
        {
            throw new NotImplementedException();
        }

        public Module[] GetLoadedModules()
        {
            throw new NotImplementedException();
        }

        public Module[] GetLoadedModules(bool getResourceModules)
        {
            throw new NotImplementedException();
        }

        public ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            throw new NotImplementedException();
        }

        public string[] GetManifestResourceNames()
        {
            throw new NotImplementedException();
        }

        public Stream GetManifestResourceStream(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public Stream GetManifestResourceStream(string name)
        {
            throw new NotImplementedException();
        }

        public Module GetModule(string name)
        {
            throw new NotImplementedException();
        }

        public Module[] GetModules()
        {
            throw new NotImplementedException();
        }

        public Module[] GetModules(bool getResourceModules)
        {
            throw new NotImplementedException();
        }

        public AssemblyName GetName()
        {
            throw new NotImplementedException();
        }

        public AssemblyName GetName(bool copiedName)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public AssemblyName[] GetReferencedAssemblies()
        {
            throw new NotImplementedException();
        }

        public Assembly GetSatelliteAssembly(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
        {
            throw new NotImplementedException();
        }

        public Type GetType(string name)
        {
            throw new NotImplementedException();
        }

        public Type GetType(string name, bool throwOnError)
        {
            throw new NotImplementedException();
        }

        public Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        public Type[] GetTypes()
        {
            throw new NotImplementedException();
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public Module LoadModule(string moduleName, byte[] rawModule)
        {
            throw new NotImplementedException();
        }

        public Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
        {
            throw new NotImplementedException();
        }
    }
}
