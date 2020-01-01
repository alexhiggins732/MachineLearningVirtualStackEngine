using System;
using System.Collections.Generic;

namespace ILEngine
{
    public class DefaultOpCodeActionCodeProvider : IOpCodeActionProvider
    {
        private Dictionary<short, Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget>> actions
            = new Dictionary<short, Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget>>();
        public Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> GetOpCodeAction(short opCodeValue)
        {
            if (actions.ContainsKey(opCodeValue))
            {
                return actions[opCodeValue];
            }
            return (Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget>)(x => ILStackFrameFlowControlTarget.NotImplemented);
        }

        public static Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> Nop
            => (x => ILStackFrameFlowControlTarget.Ignore);
        public static Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> Ret
            => (x => ILStackFrameFlowControlTarget.Ret);
        public static Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> Stloc_0
            => (frame => { frame.Locals[0].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; });

        public static Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> Stloc_1
            => (frame => { frame.Locals[1].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; });

        public static Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> Stloc_2
            => (frame => { frame.Locals[2].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; });
        public static Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> Stloc_3
            => (frame => { frame.Locals[3].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; });
        public static Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> Stloc
            => (frame => { frame.Locals[(int)frame.Current.Arg].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; });


        private void test()
        {
            var result = new Dictionary<short, Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget>>
            {
                {(short)ILOpCodeValues.Nop, (frame => { frame.Locals[(int)frame.Current.Arg].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; }) }
                ,{(short)ILOpCodeValues.Stloc_0, (frame => { frame.Locals[0].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; }) }
                ,{(short)ILOpCodeValues.Stloc_1, (frame => { frame.Locals[1].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; }) }
                ,{(short)ILOpCodeValues.Stloc_2, (frame => { frame.Locals[2].Value = frame.Stack.Pop(); return ILStackFrameFlowControlTarget.Inc; }) }

            };
        }
    }
}
