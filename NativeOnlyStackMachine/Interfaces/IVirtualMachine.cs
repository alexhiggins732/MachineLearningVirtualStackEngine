using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeOnlyStackMachine
{
    /// <summary>
    /// The machine
    /// </summary>
    public interface IMachine
    {
    }

    //Memory
    public interface IMemory
    {

    }
    /// <summary>
    /// RAM. Possibly also used for program storage
    /// </summary>
    public interface INonPersistantMemory : IMemory
    {

    }
    /// <summary>
    /// Persisted Memory, EG: File, Database, Etc.
    /// </summary>
    public interface PersistantMemory
    {

    }

    public interface IProgram
    {
        //Source Image
    }

    public interface IAssembly
    {

    }
    public interface INamespace
    {
        IAssembly Assembly { get; set; }
        List<INamespace> Namespaces { get; set; }
    }

    public interface ISymbol
    {

    }
    public interface IType
    {

    }
    public interface IMethodArguments
    {

    }
    public interface IMethodBody
    {
        IType Parent { get; set; }
    }
    public interface IMethodSignature { }
    public interface IMethod
    {
        string Label { get; set; }
        MethodType MethodType { get; set; }
        IMethodBody Body { get; set; }
        IMethodArguments Arguments { get; set; }
        IMethodSignature Signature { get; set; }
    }

    public enum MethodType
    {
        Void,
        Function
    }
}
