using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NativeOnlyStackMachine
{
    /*byte code {
     .main:
         113 ([0]{instruction.name=push},[1]{instruction.arity=1},{instruction.args={3}})
         114 ([0]{instruction.name=push},[1]{instruction.arity=1},{instruction.args={4}})
         81 ([0]{instruction.name=add},[1]{instruction.arity=0},{instruction.args={4}})
        -> stack.push(stack.pop+stack.pop);
     .add:
        stack.pop @0;
        stack.pop @1;
        call "add"
        ret;

     */
    public class StackFrame
    {
        public int Ip;
        public StackFrame(int ip)
        {
            this.Ip = ip;
        }
    }
    public class MachineStackEngine
    {
        public int Ip;
        public int Length;
        public Stack<StackFrame> CallStack;
        public MachineStackEngine()
        {
            CallStack = new Stack<StackFrame>();
            CallStack.Push(new StackFrame(0));
        }
        public static void RunTest() => new MachineStackEngine().Run();


        public void Run()
        {
            while (Ip != Length)
            {
                var frame = CallStack.Pop();
                Ip++;
               
            }
        }
        public void RunArgTest()
        {
            var stack = new MachineStack();
            int a = 1;
            int b = 2;
            double c = 0.5;
            double e = 1.4;
            DateTime now = DateTime.Now;
            var p = anonHelper.build(a, b, c, e, now);
            //var p2 = anonHelper.build(() => a, () => b, () => c, () => e, () => now);
            stack.InitLocals(() => new { a, b, c, e, now });
        }
        private void test(dynamic[] dynamics)
        {
            var stack = new MachineStack();
            stack.InitLocals(() => dynamics);
        }
    }
    public class anonHelper
    {
        public static dynamic build(params dynamic[] args)
        {
            return new { args };
        }
        public static dynamic build(params Func<dynamic>[] args)
        {
            var parameters = args.Select(arg =>
            {
                return MemberInfoGetting.GetParameters(arg());
            }).ToList();
            return new { args };
        }
    }
    public class MachineStack
    {
        public object[] locals;
        public dynamic[] localargs;
        public MachineStack()
        {

        }

        internal void InitLocals(Func<dynamic> args)
        {
            //var args = MemberInfoGetting.GetMemberName(() => args);
            //var args2 = MemberInfoGetting.GetMemberName(() => args());
            //var info = MemberInfoGetting.GetParameterInfo1(args());
            var arguments = args();
            var parameters = MemberInfoGetting.GetParameters(arguments);
            List<dynamic> locals = new List<dynamic>();
            for (var i = 0; i < arguments.Length; i++)
            {
                locals.Add(arguments[i]);
            }
            localargs = locals.ToArray();
            //throw new NotImplementedException();
        }
        //public void InitLocals(params dynamic[] args)
        //{
        //    for (var i = 0; i < args.Length; i++)
        //    {
        //        var tInfo = MemberInfoGetting.GetParameterInfo1(new { args[i] });
        //        string nameOfParam1 = MemberInfoGetting.GetMemberName(() => args[i]);()=
        //        Console.WriteLine(nameOfParam1);
        //    }
        //    locals = args;
        //}
    }
    public static class MemberInfoGetting
    {
        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {

            dynamic expressionBody = memberExpression.Body;
            //MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return (expressionBody as MemberExpression)?.Member.Name;
        }


        public static ParameterInfo[] GetStructParameters<T>(T item) where T : struct
        {
            return new ParameterInfo[] { };
        }
        public static ParameterInfo[] GetParameters<T>(T item) where T : class
        {
            if (item == null)
                return new ParameterInfo[] { };

            var props = typeof(T).GetProperties();
            var param = item.ToString().TrimStart('{').TrimEnd('}').Split('=');
            var l = new List<ParameterInfo>();

            for (var k = 0; k < props.Length; k++)
            {

                var name = props[k].Name;
                var type = props[k].PropertyType;
                l.Add(new ParameterInfo
                {
                    Label = name,
                    Value = props[k].GetValue(item),
                    Type = type

                });
            }
            return l.ToArray();
            //return "Parameter: '" + param[0].Trim() +
            //       "' = " + param[1].Trim();
        }
        public static string GetParameterInfo1<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

            var param = item.ToString().TrimStart('{').TrimEnd('}').Split('=');
            return "Parameter: '" + param[0].Trim() +
                   "' = " + param[1].Trim();
        }

        public class ParameterInfo
        {
            public string Label;
            public Type Type;
            public dynamic Value;
        }
    }
}
