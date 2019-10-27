using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FX
{
    public class TypeTree
    {
        public List<AssemblyType> AssemblyTypes;
    }
    public class AssemblyType
    {
        //exported types
        // modules
        // types => { Id, Name, Assembly }=>  [Members,Fields,Methods,Events, NestedTypes]
        //  Give each an Id, Name
        //      Where applicable, parameters, type, return type, MSIL
        // Stack machine => Set Instructions
        //      Execute ->
        //          1) Static entry point (static void main(string[] args)
        //          2) Loaded externally with code called form external libary.
        // Features:
        //  Type Table - Resolve internal and external types.
        //      Methods, Fields, Properties (method wrappers), events, delegates, nested types)
        //  Stacks: Operand stack, Instruction (Instruction more of a random access stream than a stack with each instruction have flow control)
        //  Heap: Memory storage -> Local(instance) and Global (static)
        //  Instructions: Can implement MSIL instead of reinventing wheel. Allow interop with 3rd party libaries 
        //      by importing and exporting MSIL.
        //
        //  options:
        //      a) Nice to have => CSParser that builds MSIL from CS string.
        //      b) Compile CS string with Reflection.Emit, requires injecting MSIL.
        //      c) Build instruction API and get MSIL out of method bodies.
        //      d) Load assembly and read instruction from existing binary
        //      e) Parse MSIL file (appear to be incomplete).
        //      
        //      Dependencies:
        //          a) can use existing tooling, even given MSIL need references to types, members, methods, etc.
        //              can be resolved via reflection, metadatatokens poing to token in local module and not the 
        //              the module where an external method is defined (Probably a binding redirect in the local module).
    }
    internal class TypeExplorer
    {
        public TypeExplorer()
        {
        }

        internal void Explore(Type type)
        {
            var typeName = type.Name;
            var typeFullName = type.FullName;
            var typeNameSpace = type.Namespace;
            var members = type.GetMembers(BindingFlags.Static| BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //need to be able to get type members:See MemberInfo and interface  System.Runtime.InteropServices._MemberInfo
            var memberGroupings = members.GroupBy(member => member.MemberType);
            var add = type.GetMethod("op_Addition", BindingFlags.Public | BindingFlags.Static);
            MethodInfo mi = typeof(int).GetMethod("op_Addition",
    BindingFlags.Static | BindingFlags.Public);
            var memberData = new Dictionary<MemberTypes, dynamic>();

            foreach (var memberGrouping in memberGroupings)
            {
                var memberType = memberGrouping.Key;


                if (!memberData.ContainsKey(memberType))
                {
                    memberData.Add(memberType, new List<dynamic>());
                }
                
                var memberTypeDict = memberGrouping.OrderBy(x=> x.ToString()).ToDictionary(x => x.ToString(), x=>x);
                memberData[memberType] = memberTypeDict;

            }


            foreach (MemberInfo member in members)
            {
                var memberName = member.Name;

            }
        }
    }
}