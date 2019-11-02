using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class OpCodeFilters
    {
        public static List<Func<OpCode, bool>> EmptyStackWithNoArgsLocalsAndNoInlineOperandFilters()
        {
            var filters = new List<Func<OpCode, bool>>();
            filters.Add(x => x.StackBehaviourPop == StackBehaviour.Pop0);
            filters.Add(x => x.OperandType == OperandType.InlineNone);
            filters.Add(x => x.FlowControl == FlowControl.Next || x.Name == "ret");
            filters.Add(x => x.Name != "arglist");
            filters.Add(x => !x.Name.StartsWith("ldloc") && !x.Name.StartsWith("ldarg"));
            return filters;
        }

        public static List<Func<OpCode, bool>> BuildEmptyStackWithArgsAndLocalsAndInlineOnlyOperandFilters(object[] args = null, ILVariable[] locals = null)
        {
            var filters = new List<Func<OpCode, bool>>();
            filters.Add(x => x.StackBehaviourPop == StackBehaviour.Pop0);
            filters.Add(x => x.OperandType == OperandType.InlineNone);
            filters.Add(x => x.FlowControl == FlowControl.Next || x.Name == "ret");
            filters.Add(x => x.Name != "arglist");
            if (locals == null || locals.Length == 0)
            {
                filters.Add(x => !x.Name.StartsWith("ldloc"));
            }
            else
            {
                if ( locals.Length < 4)
                {
                    List<string> allowedLocals = new List<string>();

                    if (locals.Length > 0)
                    {
                        allowedLocals.Add(ILOpCodeValueNativeNames.Ldloc_0);
                    }
                    if (locals.Length > 1)
                    {
                        allowedLocals.Add(ILOpCodeValueNativeNames.Ldloc_1);
                    }
                    if (locals.Length > 2)
                    {
                        allowedLocals.Add(ILOpCodeValueNativeNames.Ldloc_2);
                    }
                    if (locals.Length > 3)
                    {
                        allowedLocals.Add(ILOpCodeValueNativeNames.Ldloc_3);
                    }
                    filters.Add(x =>  allowedLocals.Contains(x.Name) || !x.Name.StartsWith("ldloc"));
                } else
                {
                    //if>4 then allow allo locals
                    //filters.Add(x => !x.Name.StartsWith("ldloc"));
                }


            }
            if (args == null || args.Length == 0)
            {
                filters.Add(x => !x.Name.StartsWith("ldarg"));
            } else
            {
                if (args.Length < 4)
                {
                    List<string> allowedArgs = new List<string>();

                    if (args.Length > 0)
                    {
                        allowedArgs.Add(ILOpCodeValueNativeNames.Ldarg_0);
                    }
                    if (args.Length > 1)
                    {
                        allowedArgs.Add(ILOpCodeValueNativeNames.Ldarg_1);
                    }
                    if (args.Length > 2)
                    {
                        allowedArgs.Add(ILOpCodeValueNativeNames.Ldarg_2);
                    }
                    if (args.Length > 3)
                    {
                        allowedArgs.Add(ILOpCodeValueNativeNames.Ldarg_2);
                    }
                    filters.Add(x => allowedArgs.Contains(x.Name) || !x.Name.StartsWith("ldarg"));
                }
            }


            return filters;
        }


        public static List<Func<OpCode, bool>> EmptyStackAndNoInlineOperandFilters()
        {
            var filters = new List<Func<OpCode, bool>>();
            filters.Add(x => x.StackBehaviourPop == StackBehaviour.Pop0);
            filters.Add(x => x.OperandType == OperandType.InlineNone);
            filters.Add(x => x.FlowControl == FlowControl.Next || x.Name == "ret");
            filters.Add(x => x.Name != "arglist");
            return filters;
        }
    }
}
