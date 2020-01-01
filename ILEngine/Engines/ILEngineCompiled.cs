using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ILEngine
{
    //TODO: Unit test NotImplemented for null frame
    //TODO: Unit test stelem strongly typed array casting, to handle casting such as char[] -> short[] for stelem_I2
    public class ILEngineCompiled : ILEngineBase, IILEngine, IOpCodeEngine
    {

        public override void ExecuteFrame(IILStackFrame frame)
        {
            this.frame = frame;
            this.flowControlTarget = ILStackFrameFlowControlTarget.MoveNext;
            frame.Reset();
            goto Inc;

            ReadNext:
            frame.ReadNext();

            short opCodeValue = frame.Code.Value;
            this.flowControlTarget = ILStackFrameFlowControlTarget.Inc;
            ExecuteOpCode(opCodeValue);

            switch (flowControlTarget)
            {
                case ILStackFrameFlowControlTarget.ReadNext:
                    goto ReadNext;
                case ILStackFrameFlowControlTarget.Inc:
                    goto Inc;
                case ILStackFrameFlowControlTarget.Ret:
                    goto Ret;
                case ILStackFrameFlowControlTarget.MoveNext:
                    goto MoveNext;
            }

            Inc:
            frame.Inc();

            MoveNext:
            if (frame.MoveNext())
                goto ReadNext;

            frame.Exception = new InvalidInstructionsException("End of instructions reached without a Ret statement");

            Ret:
            if (frame.Exception != null && ThrowOnException) throw (frame.Exception);
            frame.ReturnResult = (frame.Stack.Count > 0 && frame.Exception == null) ? frame.Stack.Pop() : null;
        }
        //TODO: 40 blocks not covered due to break not hit after not implemented exception. 
        //TODO: Implement remaining candidates, eg rethrow, finall, cpyobj, etc.
        //TODO: Merge/remove final not implemented candidates, so default not implemented case is thrown.
        public void ExecuteOpCode(short opCodeValue)
        {
            switch (opCodeValue)
            {
                case unchecked((short)ILOpCodeValues.Nop):
                    Nop();
                    break;
                case unchecked((short)ILOpCodeValues.Break):
                    Break();
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg_0):
                    Ldarg_0();
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg_1):
                    Ldarg_1();
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg_2):
                    Ldarg_2();
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg_3):
                    Ldarg_3();
                    break;
                case unchecked((short)ILOpCodeValues.Ldloc_0):
                    Ldloc_0();
                    break;
                case unchecked((short)ILOpCodeValues.Ldloc_1):
                    Ldloc_1();
                    break;
                case unchecked((short)ILOpCodeValues.Ldloc_2):
                    Ldloc_2();
                    break;
                case unchecked((short)ILOpCodeValues.Ldloc_3):
                    Ldloc_3();
                    break;
                case unchecked((short)ILOpCodeValues.Stloc_0):
                    Stloc_0();
                    break;
                case unchecked((short)ILOpCodeValues.Stloc_1):
                    Stloc_1();
                    break;
                case unchecked((short)ILOpCodeValues.Stloc_2):
                    Stloc_2();
                    break;
                case unchecked((short)ILOpCodeValues.Stloc_3):
                    Stloc_3();
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg_S):
                    Ldarg_S();
                    break;
                case unchecked((short)ILOpCodeValues.Ldarga_S):
                    Ldarga_S();
                    break;
                case unchecked((short)ILOpCodeValues.Starg_S):
                    Starg_S();
                    break;
                case unchecked((short)ILOpCodeValues.Ldloc_S):
                    Ldloc_S();
                    break;
                case unchecked((short)ILOpCodeValues.Ldloca_S):
                    Ldloca_S();
                    break;
                case unchecked((short)ILOpCodeValues.Stloc_S):
                    Stloc_S();
                    break;
                case unchecked((short)ILOpCodeValues.Ldnull):
                    Ldnull();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_M1):
                    Ldc_I4_M1();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_0):
                    Ldc_I4_0();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_1):
                    Ldc_I4_1();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_2):
                    Ldc_I4_2();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_3):
                    Ldc_I4_3();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_4):
                    Ldc_I4_4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_5):
                    Ldc_I4_5();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_6):
                    Ldc_I4_6();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_7):
                    Ldc_I4_7();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_8):
                    Ldc_I4_8();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4_S):
                    Ldc_I4_S();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I4):
                    Ldc_I4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_I8):
                    Ldc_I8();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_R4):
                    Ldc_R4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldc_R8):
                    Ldc_R8();
                    break;
                case unchecked((short)ILOpCodeValues.Dup):
                    Dup();
                    break;
                case unchecked((short)ILOpCodeValues.Pop):
                    Pop();
                    break;
                case unchecked((short)ILOpCodeValues.Jmp):
                    Jmp();
                    break;
                case unchecked((short)ILOpCodeValues.Call):
                    Call();
                    break;
                case unchecked((short)ILOpCodeValues.Calli):
                    Calli();
                    break;
                case unchecked((short)ILOpCodeValues.Ret):
                    Ret();
                    break;
                case unchecked((short)ILOpCodeValues.Br_S):
                    Br_S();
                    break;
                case unchecked((short)ILOpCodeValues.Brfalse_S):
                    Brfalse_S();
                    break;
                case unchecked((short)ILOpCodeValues.Brtrue_S):
                    Brtrue_S();
                    break;
                case unchecked((short)ILOpCodeValues.Beq_S):
                    Beq_S();
                    break;
                case unchecked((short)ILOpCodeValues.Bge_S):
                    Bge_S();
                    break;
                case unchecked((short)ILOpCodeValues.Bgt_S):
                    Bgt_S();
                    break;
                case unchecked((short)ILOpCodeValues.Ble_S):
                    Ble_S();
                    break;
                case unchecked((short)ILOpCodeValues.Blt_S):
                    Blt_S();
                    break;
                case unchecked((short)ILOpCodeValues.Bne_Un_S):
                    Bne_Un_S();
                    break;
                case unchecked((short)ILOpCodeValues.Bge_Un_S):
                    Bge_Un_S();
                    break;
                case unchecked((short)ILOpCodeValues.Bgt_Un_S):
                    Bgt_Un_S();
                    break;
                case unchecked((short)ILOpCodeValues.Ble_Un_S):
                    Ble_Un_S();
                    break;
                case unchecked((short)ILOpCodeValues.Blt_Un_S):
                    Blt_Un_S();
                    break;
                case unchecked((short)ILOpCodeValues.Br):
                    Br();
                    break;
                case unchecked((short)ILOpCodeValues.Brfalse):
                    Brfalse();
                    break;
                case unchecked((short)ILOpCodeValues.Brtrue):
                    Brtrue();
                    break;
                case unchecked((short)ILOpCodeValues.Beq):
                    Beq();
                    break;
                case unchecked((short)ILOpCodeValues.Bge):
                    Bge();
                    break;
                case unchecked((short)ILOpCodeValues.Bgt):
                    Bgt();
                    break;
                case unchecked((short)ILOpCodeValues.Ble):
                    Ble();
                    break;
                case unchecked((short)ILOpCodeValues.Blt):
                    Blt();
                    break;
                case unchecked((short)ILOpCodeValues.Bne_Un):
                    Bne_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Bge_Un):
                    Bge_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Bgt_Un):
                    Bgt_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Ble_Un):
                    Ble_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Blt_Un):
                    Blt_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Switch):
                    Switch();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_I1):
                    Ldind_I1();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_U1):
                    Ldind_U1();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_I2):
                    Ldind_I2();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_U2):
                    Ldind_U2();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_I4):
                    Ldind_I4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_U4):
                    Ldind_U4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_I8):
                    Ldind_I8();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_I):
                    Ldind_I();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_R4):
                    Ldind_R4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_R8):
                    Ldind_R8();
                    break;
                case unchecked((short)ILOpCodeValues.Ldind_Ref):
                    Ldind_Ref();
                    break;
                case unchecked((short)ILOpCodeValues.Stind_Ref):
                    Stind_Ref();
                    break;
                case unchecked((short)ILOpCodeValues.Stind_I1):
                    Stind_I1();
                    break;
                case unchecked((short)ILOpCodeValues.Stind_I2):
                    Stind_I2();
                    break;
                case unchecked((short)ILOpCodeValues.Stind_I4):
                    Stind_I4();
                    break;
                case unchecked((short)ILOpCodeValues.Stind_I8):
                    Stind_I8();
                    break;
                case unchecked((short)ILOpCodeValues.Stind_R4):
                    Stind_R4();
                    break;
                case unchecked((short)ILOpCodeValues.Stind_R8):
                    Stind_R8();
                    break;
                case unchecked((short)ILOpCodeValues.Add):
                    Add();
                    break;
                case unchecked((short)ILOpCodeValues.Sub):
                    Sub();
                    break;
                case unchecked((short)ILOpCodeValues.Mul):
                    Mul();
                    break;
                case unchecked((short)ILOpCodeValues.Div):
                    Div();
                    break;
                case unchecked((short)ILOpCodeValues.Div_Un):
                    Div_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Rem):
                    Rem();
                    break;
                case unchecked((short)ILOpCodeValues.Rem_Un):
                    Rem_Un();
                    break;
                case unchecked((short)ILOpCodeValues.And):
                    And();
                    break;
                case unchecked((short)ILOpCodeValues.Or):
                    Or();
                    break;
                case unchecked((short)ILOpCodeValues.Xor):
                    Xor();
                    break;
                case unchecked((short)ILOpCodeValues.Shl):
                    Shl();
                    break;
                case unchecked((short)ILOpCodeValues.Shr):
                    Shr();
                    break;
                case unchecked((short)ILOpCodeValues.Shr_Un):
                    Shr_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Neg):
                    Neg();
                    break;
                case unchecked((short)ILOpCodeValues.Not):
                    Not();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_I1):
                    Conv_I1();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_I2):
                    Conv_I2();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_I4):
                    Conv_I4();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_I8):
                    Conv_I8();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_R4):
                    Conv_R4();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_R8):
                    Conv_R8();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_U4):
                    Conv_U4();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_U8):
                    Conv_U8();
                    break;
                case unchecked((short)ILOpCodeValues.Callvirt):
                    Callvirt();
                    break;
                case unchecked((short)ILOpCodeValues.Cpobj):
                    Cpobj();
                    break;
                case unchecked((short)ILOpCodeValues.Ldobj):
                    Ldobj();
                    break;
                case unchecked((short)ILOpCodeValues.Ldstr):
                    Ldstr();
                    break;
                case unchecked((short)ILOpCodeValues.Newobj):
                    Newobj();
                    break;
                case unchecked((short)ILOpCodeValues.Castclass):
                    Castclass();
                    break;
                case unchecked((short)ILOpCodeValues.Isinst):
                    Isinst();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_R_Un):
                    Conv_R_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Unbox):
                    Unbox();
                    break;
                case unchecked((short)ILOpCodeValues.Throw):
                    Throw();
                    break;
                case unchecked((short)ILOpCodeValues.Ldfld):
                    Ldfld();
                    break;
                case unchecked((short)ILOpCodeValues.Ldflda):
                    Ldflda();
                    break;
                case unchecked((short)ILOpCodeValues.Stfld):
                    Stfld();
                    break;
                case unchecked((short)ILOpCodeValues.Ldsfld):
                    Ldsfld();
                    break;
                case unchecked((short)ILOpCodeValues.Ldsflda):
                    Ldsflda();
                    break;
                case unchecked((short)ILOpCodeValues.Stsfld):
                    Stsfld();
                    break;
                case unchecked((short)ILOpCodeValues.Stobj):
                    Stobj();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I1_Un):
                    Conv_Ovf_I1_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I2_Un):
                    Conv_Ovf_I2_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I4_Un):
                    Conv_Ovf_I4_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I8_Un):
                    Conv_Ovf_I8_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U1_Un):
                    Conv_Ovf_U1_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U2_Un):
                    Conv_Ovf_U2_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U4_Un):
                    Conv_Ovf_U4_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U8_Un):
                    Conv_Ovf_U8_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I_Un):
                    Conv_Ovf_I_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U_Un):
                    Conv_Ovf_U_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Box):
                    Box();
                    break;
                case unchecked((short)ILOpCodeValues.Newarr):
                    Newarr();
                    break;
                case unchecked((short)ILOpCodeValues.Ldlen):
                    Ldlen();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelema):
                    Ldelema();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_I1):
                    Ldelem_I1();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_U1):
                    Ldelem_U1();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_I2):
                    Ldelem_I2();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_U2):
                    Ldelem_U2();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_I4):
                    Ldelem_I4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_U4):
                    Ldelem_U4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_I8):
                    Ldelem_I8();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_I):
                    Ldelem_I();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_R4):
                    Ldelem_R4();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_R8):
                    Ldelem_R8();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem_Ref):
                    Ldelem_Ref();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem_I):
                    Stelem_I();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem_I1):
                    Stelem_I1();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem_I2):
                    Stelem_I2();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem_I4):
                    Stelem_I4();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem_I8):
                    Stelem_I8();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem_R4):
                    Stelem_R4();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem_R8):
                    Stelem_R8();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem_Ref):
                    Stelem_Ref();
                    break;
                case unchecked((short)ILOpCodeValues.Ldelem):
                    Ldelem();
                    break;
                case unchecked((short)ILOpCodeValues.Stelem):
                    Stelem();
                    break;
                case unchecked((short)ILOpCodeValues.Unbox_Any):
                    Unbox_Any();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I1):
                    Conv_Ovf_I1();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U1):
                    Conv_Ovf_U1();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I2):
                    Conv_Ovf_I2();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U2):
                    Conv_Ovf_U2();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I4):
                    Conv_Ovf_I4();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U4):
                    Conv_Ovf_U4();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I8):
                    Conv_Ovf_I8();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U8):
                    Conv_Ovf_U8();
                    break;
                case unchecked((short)ILOpCodeValues.Refanyval):
                    Refanyval();
                    break;
                case unchecked((short)ILOpCodeValues.Ckfinite):
                    Ckfinite();
                    break;
                case unchecked((short)ILOpCodeValues.Mkrefany):
                    Mkrefany();
                    break;
                case unchecked((short)ILOpCodeValues.Ldtoken):
                    Ldtoken();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_U2):
                    Conv_U2();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_U1):
                    Conv_U1();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_I):
                    Conv_I();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_I):
                    Conv_Ovf_I();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_Ovf_U):
                    Conv_Ovf_U();
                    break;
                case unchecked((short)ILOpCodeValues.Add_Ovf):
                    Add_Ovf();
                    break;
                case unchecked((short)ILOpCodeValues.Add_Ovf_Un):
                    Add_Ovf_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Mul_Ovf):
                    Mul_Ovf();
                    break;
                case unchecked((short)ILOpCodeValues.Mul_Ovf_Un):
                    Mul_Ovf_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Sub_Ovf):
                    Sub_Ovf();
                    break;
                case unchecked((short)ILOpCodeValues.Sub_Ovf_Un):
                    Sub_Ovf_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Endfinally):
                    Endfinally();
                    break;
                case unchecked((short)ILOpCodeValues.Leave):
                    Leave();
                    break;
                case unchecked((short)ILOpCodeValues.Leave_S):
                    Leave_S();
                    break;
                case unchecked((short)ILOpCodeValues.Stind_I):
                    Stind_I();
                    break;
                case unchecked((short)ILOpCodeValues.Conv_U):
                    Conv_U();
                    break;
                case unchecked((short)ILOpCodeValues.Prefix7):
                    Prefix7();
                    break;
                case unchecked((short)ILOpCodeValues.Prefix6):
                    Prefix6();
                    break;
                case unchecked((short)ILOpCodeValues.Prefix5):
                    Prefix5();
                    break;
                case unchecked((short)ILOpCodeValues.Prefix4):
                    Prefix4();
                    break;
                case unchecked((short)ILOpCodeValues.Prefix3):
                    Prefix3();
                    break;
                case unchecked((short)ILOpCodeValues.Prefix2):
                    Prefix2();
                    break;
                case unchecked((short)ILOpCodeValues.Prefix1):
                    Prefix1();
                    break;
                case unchecked((short)ILOpCodeValues.Prefixref):
                    Prefixref();
                    break;
                case unchecked((short)ILOpCodeValues.Arglist):
                    Arglist();
                    break;
                case unchecked((short)ILOpCodeValues.Ceq):
                    Ceq();
                    break;
                case unchecked((short)ILOpCodeValues.Cgt):
                    Cgt();
                    break;
                case unchecked((short)ILOpCodeValues.Cgt_Un):
                    Cgt_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Clt):
                    Clt();
                    break;
                case unchecked((short)ILOpCodeValues.Clt_Un):
                    Clt_Un();
                    break;
                case unchecked((short)ILOpCodeValues.Ldftn):
                    Ldftn();
                    break;
                case unchecked((short)ILOpCodeValues.Ldvirtftn):
                    Ldvirtftn();
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg):
                    Ldarg();
                    break;
                case unchecked((short)ILOpCodeValues.Ldarga):
                    Ldarga();
                    break;
                case unchecked((short)ILOpCodeValues.Starg):
                    Starg();
                    break;
                case unchecked((short)ILOpCodeValues.Ldloc):
                    Ldloc();
                    break;
                case unchecked((short)ILOpCodeValues.Ldloca):
                    Ldloca();
                    break;
                case unchecked((short)ILOpCodeValues.Stloc):
                    Stloc();
                    break;
                case unchecked((short)ILOpCodeValues.Localloc):
                    Localloc();
                    break;
                case unchecked((short)ILOpCodeValues.Endfilter):
                    Endfilter();
                    break;
                case unchecked((short)ILOpCodeValues.Unaligned):
                    Unaligned();
                    break;
                case unchecked((short)ILOpCodeValues.Volatile):
                    Volatile();
                    break;
                case unchecked((short)ILOpCodeValues.Tailcall):
                    Tailcall();
                    break;
                case unchecked((short)ILOpCodeValues.Initobj):
                    Initobj();
                    break;
                case unchecked((short)ILOpCodeValues.Constrained):
                    Constrained();
                    break;
                case unchecked((short)ILOpCodeValues.Cpblk):
                    Cpblk();
                    break;
                case unchecked((short)ILOpCodeValues.Initblk):
                    Initblk();
                    break;
                case unchecked((short)ILOpCodeValues.Rethrow):
                    Rethrow();
                    break;
                case unchecked((short)ILOpCodeValues.Sizeof):
                    Sizeof();
                    break;
                case unchecked((short)ILOpCodeValues.Refanytype):
                    Refanytype();
                    break;
                case unchecked((short)ILOpCodeValues.Readonly):
                    Readonly();
                    break;
                case unchecked((short)ILOpCodeValues.Exec_MSIL_I):
                    Exec_MSIL_I();
                    break;
                case unchecked((short)ILOpCodeValues.Exec_MSIL_S):
                    Exec_MSIL_S();
                    break;
                default:
                    NotImplemented();
                    break;
            }
        }

        public void Add()
        {
            //handle Add
            //: //add
            frame.Stack.Push(unchecked((dynamic)frame.Stack.Pop() + (dynamic)frame.Stack.Pop()));

        }
        public void Add_Ovf_Un()
        {
            //handle Add_Ovf_Un
            // 215: //add.ovf.un
            frame.Stack.Push(checked(((dynamic)frame.Stack.Pop()) + ((dynamic)frame.Stack.Pop())));

        }
        public void Add_Ovf()
        {
            //handle Add_Ovf
            // 214: //add.ovf
            frame.Stack.Push(checked(((dynamic)frame.Stack.Pop()) + ((dynamic)frame.Stack.Pop())));

        }
        public void And()
        {
            //handle And
            //95: //and
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) & ((dynamic)frame.Stack.Pop()));

        }
        public void Arglist()
        {
            // Missing: Arglist
            throw new OpCodeNotImplementedException(-512);

        }
        public void Beq_S()
        {

            //handle Beq_S

            var opa = frame.Stack.Pop();
            var opb = frame.Stack.Pop();
            if (opa is IConvertible && opb is IConvertible)
            {
                var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                frame.Stack.Push(compc == 0 ? 1 : 0);
            }
            else
            {
                frame.Stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
            }
            Brtrue_S();


        }
        public void Beq()
        {
            var opb = frame.Stack.Pop();
            var opa = frame.Stack.Pop();
            if (opa is IConvertible && opb is IConvertible)
            {
                var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                frame.Stack.Push(compc == 0 ? 1 : 0);
            }
            else
            {
                frame.Stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
            }
            Brtrue();
        }
        public void Bge_S()
        {
            Bge();
        }
        public void Bge()
        {
            var opb = frame.Stack.Pop();
            var opa = frame.Stack.Pop();
            if (opa is IConvertible && opb is IConvertible)
            {
                var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                frame.Stack.Push(compc >= 0 ? 1 : 0);
            }
            else
            {
                frame.Stack.Push(Convert.ToInt32(((dynamic)opa) >= ((dynamic)opb)));
            }
            Brtrue();
        }
        public void Bge_Un_S()
        {
            Bge();
        }
        public void Bge_Un()
        {
            Bge();
            //var opb = frame.Stack.Pop();
            //var opa = frame.Stack.Pop();
            //if (opa is IConvertible && opb is IConvertible)
            //{
            //    var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
            //    frame.Stack.Push(compc >= 0 ? 1 : 0);
            //}
            //else
            //{
            //    frame.Stack.Push(Convert.ToInt32(((dynamic)opa) >= ((dynamic)opb)));
            //}
            //Brtrue();
        }
        public void Bgt_S()
        {
            Bgt();
        }
        public void Bgt()
        {
            var opb = frame.Stack.Pop();
            var opa = frame.Stack.Pop();
            if (opa is IConvertible && opb is IConvertible)
            {
                var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                frame.Stack.Push(compc > 0 ? 1 : 0);
            }
            else
            {
                frame.Stack.Push(Convert.ToInt32(((dynamic)opa) > ((dynamic)opb)));
            }
            Brtrue();
        }
        public void Bgt_Un_S()
        {
            Bgt();
        }
        public void Bgt_Un()
        {
            Bgt();
        }
        public void Ble_S()
        {
            Ble();
        }
        public void Ble()
        {
            var opb = frame.Stack.Pop();
            var opa = frame.Stack.Pop();
            if (opa is IConvertible && opb is IConvertible)
            {
                var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                frame.Stack.Push(compc <= 0 ? 1 : 0);
            }
            else
            {
                frame.Stack.Push(Convert.ToInt32(((dynamic)opa) <= ((dynamic)opb)));
            }
            Brtrue();
        }
        public void Ble_Un_S()
        {
            Ble();
        }
        public void Ble_Un()
        {
            Ble();
        }
        public void Blt_S()
        {
            Blt();
        }
        public void Blt()
        {
            var opb = frame.Stack.Pop();
            var opa = frame.Stack.Pop();
            if (opa is IConvertible && opb is IConvertible)
            {
                var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                frame.Stack.Push(compc < 0 ? 1 : 0);
            }
            else
            {
                frame.Stack.Push(Convert.ToInt32(((dynamic)opa) < ((dynamic)opb)));
            }
            Brtrue();
        }
        public void Blt_Un_S()
        {
            Blt();
        }
        public void Blt_Un()
        {
            Blt();
        }
        public void Bne_Un_S()
        {
            Bne_Un();
        }
        public void Bne_Un()
        {
            var opb = frame.Stack.Pop();
            var opa = frame.Stack.Pop();
            if (opa is IConvertible && opb is IConvertible)
            {
                var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                frame.Stack.Push(compc != 0 ? 1 : 0);
            }
            else
            {
                frame.Stack.Push(Convert.ToInt32(((dynamic)opa) != ((dynamic)opb)));
            }
            Brtrue();
        }
        public void Box()
        {
            //handle Box
            // 140: //box
            object objectReference = (object)frame.Stack.Pop();
            frame.Stack.Push(objectReference);

        }
        public void Br_S()
        {
            //handle Br_S
            //0x2b:

            var delta = (int)(sbyte)Convert.ToByte(frame.Current.Arg);
            var directpos = (int)frame.Stream[frame.Position + 1].ByteIndex + delta;
            frame.Position = frame.JumpTable[directpos];
            //goto MoveNext;

            FlowControlTarget = ILStackFrameFlowControlTarget.MoveNext;
        }
        public void Br()
        {
            //handle Br
            // 0x38:
            {
                var delta = Convert.ToInt32(frame.Current.Arg);
                var directpos = (int)frame.Stream[frame.Position + 1].ByteIndex + delta;
                frame.Position = frame.JumpTable[directpos];
                //goto MoveNext;
            }
            FlowControlTarget = ILStackFrameFlowControlTarget.ReadNext;
        }

        //can not be tested from unit test due to requiring debugger to be attached.
        [ExcludeFromCodeCoverage]
        public void Break()
        {
            //handle Break
            if (frame.TriggerBreak || this.BreakOnDebug)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }

        }
        public void Brfalse_S()
        {
            //handle Brfalse_S
            {
                var chk = frame.Stack.Pop();
                int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                var condtarget = -1;
                if (cond == 0)
                {
                    condtarget = (int)frame.Stream[frame.Position + 1].ByteIndex + (int)(sbyte)Convert.ToByte(frame.Current.Arg);
                    frame.Position = frame.JumpTable[condtarget];
                    FlowControlTarget = ILStackFrameFlowControlTarget.MoveNext;

                }
            }

        }
        public void Brfalse()
        {
            //handle Brfalse

            var chk = frame.Stack.Pop();
            int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
            var condtarget = -1;
            if (cond == 0)
            {
                condtarget = (int)frame.Stream[frame.Position + 1].ByteIndex + Convert.ToInt32(frame.Current.Arg);
                frame.Position = frame.JumpTable[condtarget];
                FlowControlTarget = ILStackFrameFlowControlTarget.MoveNext;
            }
        }
        public void Brtrue_S()
        {

            //handle Brtrue_S

            var chk = frame.Stack.Pop();
            int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
            var condtarget = -1;
            if (cond == 1)
            {
                condtarget = (int)frame.Stream[frame.Position + 1].ByteIndex + (int)(sbyte)Convert.ToByte(frame.Current.Arg);
                frame.Position = frame.JumpTable[condtarget];
                FlowControlTarget = ILStackFrameFlowControlTarget.MoveNext;
            }
        }
        public void Brtrue()
        {

            //handle Brtrue

            var chk = frame.Stack.Pop();
            int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
            var condtarget = -1;
            if (cond == 1)
            {
                condtarget = (int)frame.Stream[frame.Position + 1].ByteIndex + Convert.ToInt32(frame.Current.Arg);
                frame.Position = frame.JumpTable[condtarget];
                FlowControlTarget = ILStackFrameFlowControlTarget.MoveNext;
            }



        }

        //TODO: Deprecate IL variable or implement.
        //TODO: Implement nested method call test. It should exist in another, older engine.
        public void Callvirt()
        {
            //handle Callvirt
            System.Reflection.MethodBase method = null;
            object resolved = null;
            if (frame.Current.Arg is System.Reflection.MethodInfo)
                resolved = (System.Reflection.MethodInfo)frame.Current.Arg;
            else
                resolved = frame.ResolveMethodToken((int)frame.Current.Arg);
            if (resolved is ConstructorInfo)
            {
                method = (ConstructorInfo)resolved;
            }
            else
            {
                method = (System.Reflection.MethodInfo)resolved;
            }
            var parameters = method.GetParameters();
            var methodArgs = new object[parameters.Length];
            for (var i = methodArgs.Length - 1; i >= 0; i--)
            {
                var val = frame.Stack.Pop();
                if (val is ILVariable)
                    methodArgs[i] = ((ILVariable)(val)).Value;
                else
                    methodArgs[i] = val;
            }
            object methodTarget = null;
            if (!method.IsStatic && !method.IsConstructor)
                methodTarget = frame.Stack.Pop();
            if (methodTarget is ILVariable)
                methodTarget = ((ILVariable)methodTarget).Value;
            //var t = default(RuntimeTypeHandle);
            //var t1 = default(ArgIterator);
            //var tobject = new object[] { t, t };
            //var del = Delegate.CreateDelegate()
            //((ConstructorInfo)method).Invoke( new object[]{ t});
            for (var i = methodArgs.Length - 1; i >= 0; i--)
                if (methodArgs[i] is IConvertible)
                    methodArgs[i] = Convert.ChangeType(methodArgs[i], parameters[i].ParameterType);
            //if the current method is invoking another method then convert the arguments for the inner method.
            if (methodTarget is MethodBase && methodArgs.Length == 2 && methodArgs[1] is Array)
            {
                var invokeArgs = (Array)methodArgs[1];
                var invokeParameters = ((MethodInfo)methodTarget).GetParameters();
                for (var i = invokeArgs.Length - 1; i >= 0; i--)
                {
                    var arg = invokeArgs.GetValue(i);
                    if (arg is IConvertible)
                        invokeArgs.SetValue(Convert.ChangeType(arg, invokeParameters[i].ParameterType), i);
                }
                //if (invokeArgs.GetValue(i) is IConvertible)
                //    invokeArgs.SetValue(Convert.ChangeType(invokeArgs[i], invokeParameters[i].ParameterType));
            }
            // Roadblock here: Int.CompareTo(object value) -> argument value must be of type int but there is no way to programatically determine the expected destination type.
            var methodresult = (method is MethodInfo) ? method.Invoke(methodTarget, methodArgs) : ((ConstructorInfo)method).Invoke(methodArgs);
            if (frame.Code.StackBehaviourPush == StackBehaviour.Varpush)
                if ((method as MethodInfo)?.ReturnType != typeof(void) || method.IsConstructor)
                    frame.Stack.Push(methodresult);

        }
        public void Call()
        {
            //redirect Call => Callvirt()
            Callvirt();

        }
        public void Calli()
        {
            //redirect Calli => Callvirt()
            Callvirt();

        }
        public void Castclass()
        {

            //handle Castclass
            var targetClassToken = (int)frame.Current.Arg;
            var targetType = frame.ResolveTypeToken(targetClassToken);
            var src = frame.Stack.Pop();
            var srcType = src.GetType();

            //var t= typeof(CastHelper<, >).MakeGenericType(new[] {src.GetType(), targetType });
            //var castMethod = t.GetMethod(nameof(CastHelper<string, string>.Cast));
            //var result = castMethod.Invoke(null, new object[] { src });



            var listType = typeof(List<>).MakeGenericType(new[] { targetType });
            var instance = Activator.CreateInstance(listType);
            var prop = listType.GetProperty("Item");


            var add = listType.GetMethod("Add");
            add.Invoke(instance, new[] { src });
            var get = listType.GetMethod("get_Item");
            var listresult = get.Invoke(instance, new object[] { 0 });
            frame.Stack.Push(listresult);

        }
        public void Ceq()
        {
            //handle Ceq
            //: //ceq
            {
                var opa = frame.Stack.Pop();
                var opb = frame.Stack.Pop();
                if (opa is IConvertible && opb is IConvertible)
                {
                    var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                    frame.Stack.Push(compc == 0 ? 1 : 0);
                }
                else
                {
                    var compc = (dynamic)opa == (dynamic)opb;
                    frame.Stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
                }
            }

        }
        public void Cgt()
        {
            //handle Cgt
            // -510: //cgt
            var b = frame.Stack.Pop();
            var a = frame.Stack.Pop();
            var comp = (dynamic)a > (dynamic)b;
            var result = Convert.ToInt32(comp);
            frame.Stack.Push(result);

        }
        public void Cgt_Un()
        {
            var b = frame.Stack.Pop();
            var a = frame.Stack.Pop();
            var comp = (dynamic)a > (dynamic)b;
            var result = Convert.ToInt32(comp);
            frame.Stack.Push(result);

        }
        public void Ckfinite()
        {
            //handle Ckfinite
            // 195: //ckfinite
            var ckval = frame.Stack.Pop(); frame.Stack.Push((ckval is float ? float.IsInfinity((float)ckval) : double.IsInfinity((double)ckval)));

        }
        public void Clt()
        {
            var b = frame.Stack.Pop();
            var a = frame.Stack.Pop();
            var comp = (dynamic)a < (dynamic)b;
            var result = Convert.ToInt32(comp);
            frame.Stack.Push(result);

        }
        public void Clt_Un()
        {
            var b = frame.Stack.Pop();
            var a = frame.Stack.Pop();
            var comp = (dynamic)a < (dynamic)b;
            var result = Convert.ToInt32(comp);
            frame.Stack.Push(result);

        }
        public void Constrained()
        {
            // Missing: Constrained
            throw new OpCodeNotImplementedException(-490);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        // TODO: Test long and ulong
        public void Conv_I()
        {
            //handle Conv_I
            // 103: //conv.i1
            var conv8 = (frame.Stack.Peek() is long || frame.Stack.Peek() is ulong);
            if (conv8)
            {
                Conv_I8();
            }
            else
            {
                Conv_I4();
            }
        }
        public void Conv_Ovf_I()
        {
            Conv_Ovf_I4();

        }
        public void Conv_Ovf_U()
        {
            // Missing: Conv_Ovf_U
            Conv_Ovf_U4();

        }

        public void Conv_I1()
        {
            //handle Conv_I1
            // 103: //conv.i1
            frame.Stack.Push(unchecked((sbyte)Convert.ToInt64(frame.Stack.Pop())));

        }
        public void Conv_I2()
        {
            //handle Conv_I2
            //104: //conv.i2
            frame.Stack.Push(unchecked((short)Convert.ToInt64(frame.Stack.Pop())));

        }
        public void Conv_I4()
        {
            //handle Conv_I4
            //105: //conv.i4
            frame.Stack.Push(unchecked((int)Convert.ToInt64(frame.Stack.Pop())));

        }
        public void Conv_I8()
        {
            //handle Conv_I8
            //106: //conv.i8
            var arg = frame.Stack.Pop();
            try
            {
                frame.Stack.Push(unchecked(Convert.ToInt64(arg)));
            }
            catch (OverflowException)
            {
                frame.Stack.Push(unchecked((long)(Convert.ToUInt64(arg))));
            }


        }
        public void Conv_Ovf_I_Un()
        {
            //redirect Conv_Ovf_I_Un => Conv_Ovf_I()
            Conv_Ovf_I();

        }
        public void Conv_Ovf_I1()
        {
            //handle Conv_Ovf_I1
            //179: //conv.ovf.i1
            frame.Stack.Push(Convert.ToSByte(frame.Stack.Pop()));

        }
        public void Conv_Ovf_U1()
        {
            //handle Conv_Ovf_U1
            //180: //conv.ovf.u1
            frame.Stack.Push(Convert.ToByte(frame.Stack.Pop()));

        }
        public void Conv_Ovf_I2()
        {
            //handle Conv_Ovf_I2
            //181: //conv.ovf.i2
            frame.Stack.Push(Convert.ToInt16(frame.Stack.Pop()));

        }
        public void Conv_Ovf_U2()
        {
            //handle Conv_Ovf_U2
            //182: //conv.ovf.u2
            frame.Stack.Push(Convert.ToUInt16(frame.Stack.Pop()));

        }
        public void Conv_Ovf_I4()
        {
            //handle Conv_Ovf_I4
            //183: //conv.ovf.i4
            frame.Stack.Push(Convert.ToInt32(frame.Stack.Pop()));

        }
        public void Conv_Ovf_U4()
        {
            //handle Conv_Ovf_U4
            //184: //conv.ovf.u4
            frame.Stack.Push(Convert.ToUInt32(frame.Stack.Pop()));

        }
        public void Conv_Ovf_I8()
        {
            //handle Conv_Ovf_I8
            //185: //conv.ovf.i8
            frame.Stack.Push(Convert.ToInt64(frame.Stack.Pop()));

        }
        public void Conv_Ovf_U8()
        {
            //handle Conv_Ovf_U8
            //186: //conv.ovf.u8
            frame.Stack.Push(Convert.ToUInt64(frame.Stack.Pop()));

        }
        public void Conv_Ovf_I1_Un()
        {
            //handle Conv_Ovf_I1_Un
            //130: //conv.ovf.i1.un
            frame.Stack.Push(Convert.ToSByte(frame.Stack.Pop()));

        }
        public void Conv_Ovf_I2_Un()
        {
            //handle Conv_Ovf_I2_Un
            //131: //conv.ovf.i2.un
            frame.Stack.Push(Convert.ToInt16(frame.Stack.Pop()));

        }
        public void Conv_Ovf_I4_Un()
        {
            //handle Conv_Ovf_I4_Un
            //132: //conv.ovf.i4.un
            frame.Stack.Push(Convert.ToInt32(frame.Stack.Pop()));

        }
        public void Conv_Ovf_I8_Un()
        {
            //handle Conv_Ovf_I8_Un
            //133: //conv.ovf.i8.un
            frame.Stack.Push(Convert.ToInt64(frame.Stack.Pop()));

        }
        public void Conv_Ovf_U_Un()
        {
            // Missing: Conv_Ovf_U_Un
            Conv_Ovf_U4();

        }
        public void Conv_Ovf_U1_Un()
        {
            //handle Conv_Ovf_U1_Un
            //134: //conv.ovf.u1.un
            frame.Stack.Push(Convert.ToByte(frame.Stack.Pop()));

        }
        public void Conv_Ovf_U2_Un()
        {
            //handle Conv_Ovf_U2_Un
            //135: //conv.ovf.u2.un
            frame.Stack.Push(Convert.ToUInt16(frame.Stack.Pop()));

        }
        public void Conv_Ovf_U4_Un()
        {
            //handle Conv_Ovf_U4_Un
            //136: //conv.ovf.u4.un
            frame.Stack.Push(Convert.ToUInt32(frame.Stack.Pop()));

        }
        public void Conv_Ovf_U8_Un()
        {
            //handle Conv_Ovf_U8_Un
            //137: //conv.ovf.u8.un
            frame.Stack.Push(Convert.ToUInt64(frame.Stack.Pop()));

        }
        public void Conv_R_Un()
        {
            //handle Conv_R_Un
            //118: //conv.r.un
            frame.Stack.Push(Convert.ToSingle(frame.Stack.Pop()));

        }
        public void Conv_R4()
        {
            //handle Conv_R4
            //107: //conv.r4
            frame.Stack.Push(Convert.ToSingle(frame.Stack.Pop()));

        }
        public void Conv_R8()
        {
            //handle Conv_R8
            //108: //conv.r8
            frame.Stack.Push(Convert.ToDouble(frame.Stack.Pop()));

        }
        public void Conv_U()
        {
            //handle Conv_U
            //TODO: CIL explicit conversion to 8 byte integer is required for Conv_U/CO
            var conv8 = (frame.Stack.Peek() is long || frame.Stack.Peek() is ulong);
            if (conv8)
            {
                Conv_U8();
            }
            else
            {
                Conv_U4();
            }

        }
        public void Conv_U1()
        {
            //handle Conv_U1
            //210: //conv.u1
            Conv_U8();
            frame.Stack.Push(unchecked((byte)(ulong)frame.Stack.Pop()));
        }
        public void Conv_U2()
        {
            Conv_U8();
            frame.Stack.Push(unchecked((ushort)(ulong)frame.Stack.Pop()));
        }
        public void Conv_U4()
        {
            //handle Conv_U4
            //109: //conv.u4
            frame.Stack.Push(unchecked((uint)(Convert.ToInt64(frame.Stack.Pop()))));

        }
        public void Conv_U8()
        {
            //handle Conv_U8
            //110: //conv.u8
            Conv_I8();
            frame.Stack.Push(unchecked((ulong)(long)frame.Stack.Pop()));

        }
        public void Cpblk()
        {
            // Missing: Cpblk
            throw new OpCodeNotImplementedException(-489);

        }
        public void Cpobj()
        {
            // Missing: Cpobj
            throw new OpCodeNotImplementedException(112);

        }
        public void Div()
        {
            //handle Div
            // 91: //div
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) / ((dynamic)frame.Stack.Pop()));

        }
        public void Div_Un()
        {
            //handle Div_Un
            // 92: //div.un
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) / ((dynamic)frame.Stack.Pop()));

        }
        public void Dup()
        {
            //handle Dup
            frame.Stack.Push(frame.Stack.Peek());
            //break;
            //TODO: Implemented scopre validation for branch: (EG, inside try, catch, finally,etc)

        }
        public void Endfilter()
        {
            // Missing: Endfilter
            throw new OpCodeNotImplementedException(-495);

        }
        public void Endfinally()
        {
            // Missing: Endfinally
            throw new OpCodeNotImplementedException(220);

        }
        public void Exec_MSIL_I()
        {
            // Missing: Exec_MSIL_I
            throw new OpCodeNotImplementedException(36);

        }
        public void Exec_MSIL_S()
        {
            // Missing: Exec_MSIL_S
            throw new OpCodeNotImplementedException(119);

        }
        public void Initblk()
        {
            // Missing: Initblk
            throw new OpCodeNotImplementedException(-488);

        }
        public void Initobj()
        {
            //TODO: Implement ILOpCodeValues.Initobj
            /*
             *  //handle Initobj
            var newObj = Activator.CreateInstance(frame.ResolveTypeToken((int)frame.Current.Arg));
            var inst = frame.Stack.Pop();
            if (inst is ILVariable ilvar)
            {
                ilvar.Value = newObj;
                frame.Locals[ilvar.Index] = ilvar;
            }
            else
                inst = newObj;*/
            throw new OpCodeNotImplementedException(ILOpCodeValues.Initobj);


        }
        public void Isinst()
        {
            // Missing: Isinst
            var targetClassToken = (int)frame.Current.Arg;
            var targetType = frame.ResolveTypeToken(targetClassToken);
            var source = frame.Stack.Pop();
            bool isInstance = targetType.IsInstanceOfType(source);
            frame.Stack.Push(isInstance);

        }
        public void Jmp()
        {
            // Missing: Jmp
            throw new OpCodeNotImplementedException(39);
        }
        public void Ldarg()
        {
            //handle Ldarg
            frame.Stack.Push(frame.Args[(int)frame.Current.Arg]);

        }
        public void Ldarga()
        {
            //handle Ldarga
            frame.Stack.Push(frame.Args[(int)frame.Current.Arg]);

        }
        public void Ldarg_0()
        {
            //handle Ldarg_0
            frame.Stack.Push(frame.Args[0]);

        }
        public void Ldarg_1()
        {
            //handle Ldarg_1
            frame.Stack.Push(frame.Args[1]);

        }
        public void Ldarg_2()
        {
            //handle Ldarg_2
            frame.Stack.Push(frame.Args[2]);

        }
        public void Ldarg_3()
        {
            //handle Ldarg_3
            frame.Stack.Push(frame.Args[3]);

        }
        public void Ldarg_S()
        {
            //handle Ldarg_S
            frame.Stack.Push(frame.Args[(byte)Convert.ToInt32(frame.Current.Arg)]);

        }
        public void Ldarga_S()
        {
            //handle Ldarga_S
            frame.Stack.Push(frame.Args[(byte)Convert.ToInt32(frame.Current.Arg)]);

        }
        public void Ldc_I4_0()
        {
            //handle Ldc_I4_0
            frame.Stack.Push(0);

        }
        public void Ldc_I4_1()
        {
            //handle Ldc_I4_1
            frame.Stack.Push(1);

        }
        public void Ldc_I4_2()
        {
            //handle Ldc_I4_2
            frame.Stack.Push(2);

        }
        public void Ldc_I4_3()
        {
            //handle Ldc_I4_3
            frame.Stack.Push(3);

        }
        public void Ldc_I4_4()
        {
            //handle Ldc_I4_4
            frame.Stack.Push(4);

        }
        public void Ldc_I4_5()
        {
            //handle Ldc_I4_5
            frame.Stack.Push(5);

        }
        public void Ldc_I4_6()
        {
            //handle Ldc_I4_6
            frame.Stack.Push(6);

        }
        public void Ldc_I4_7()
        {
            //handle Ldc_I4_7
            frame.Stack.Push(7);

        }
        public void Ldc_I4_8()
        {
            //handle Ldc_I4_8
            frame.Stack.Push(8);

        }
        public void Ldc_I4_M1()
        {
            //handle Ldc_I4_M1
            frame.Stack.Push(-1);

        }
        public void Ldc_I4_S()
        {
            //handle Ldc_I4_S
            frame.Stack.Push(Convert.ToInt32(frame.Current.Arg));

        }
        public void Ldc_I4()
        {
            //handle Ldc_I4
            frame.Stack.Push(unchecked((int)frame.Current.Arg));

        }
        public void Ldc_I8()
        {
            //handle Ldc_I8
            frame.Stack.Push(Convert.ToInt64(frame.Current.Arg));

        }
        public void Ldc_R4()
        {
            //handle Ldc_R4
            frame.Stack.Push(Convert.ToSingle(frame.Current.Arg));

        }
        public void Ldc_R8()
        {
            //handle Ldc_R8
            frame.Stack.Push(Convert.ToDouble(frame.Current.Arg));

        }
        public void Ldftn()
        {
            //handle Ldftn
            var ftnToken = (int)frame.Current.Arg;
            var ftnMethod = frame.ResolveMethodToken(ftnToken);
            frame.Stack.Push(ftnMethod.MethodHandle.GetFunctionPointer());

        }
        public void Ldelem()
        {
            //redirect Ldelem => Ldelema()
            Ldelema();

        }
        public void Ldelema()
        {
            //redirect Ldelema => Ldelem_I()
            Ldelem_I();

        }
        public void Ldelem_I1()
        {
            //handle Ldelem_I1
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = (sbyte)Convert.ToInt32(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_U1()
        {
            //handle Ldelem_U1
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = (byte)Convert.ToUInt32(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_I2()
        {
            //handle Ldelem_I2
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = (short)Convert.ToInt32(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_U2()
        {
            //handle Ldelem_U2
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = (ushort)Convert.ToUInt32(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_I4()
        {
            //handle Ldelem_I4
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = Convert.ToInt32(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_U4()
        {
            //handle Ldelem_U4
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = Convert.ToUInt32(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_I8()
        {
            //handle Ldelem_I8
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = Convert.ToInt64(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_I()
        {
            //redirect Ldelem_I => Ldelem_Ref()
            Ldelem_Ref();

        }
        public void Ldelem_R4()
        {
            //handle Ldelem_R4
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = Convert.ToSingle(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_R8()
        {
            //handle Ldelem_R8
            {
                var idx = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                var val = array.GetValue(idx);
                var target = Convert.ToDouble(val);
                frame.Stack.Push(target);
                //break;
            }

        }
        public void Ldelem_Ref()
        {
            //handle Ldelem_Ref

            Conv_I4();
            var idx = (int)frame.Stack.Pop();
            var array = (Array)frame.Stack.Pop();
            var val = array.GetValue(idx);
            frame.Stack.Push(val);
            //break;


        }
        public void Ldfld()
        {
            //handle Ldfld
            FieldInfo field = null;
            if (frame.Current.Arg is int fieldMetadataToken)
            {
                field = frame.ResolveFieldToken(fieldMetadataToken);
            }
            else if (frame.Current.Arg is FieldInfo field2)
            {
                field = field2;
            }

            var target = frame.Stack.Pop();
            var value = field.GetValue(target);
            frame.Stack.Push(value);
            //break;


        }
        public void Ldflda()
        {
            // Missing: Ldflda
            Ldfld();

        }
        public void Ldind_I1()
        {
            Conv_I1();
        }
        public void Ldind_I2()
        {
            Conv_I2();
        }
        public void Ldind_I4()
        {
            Conv_I4();
        }
        public void Ldind_I8()
        {
            Conv_I8();
        }
        public void Ldind_I()
        {
            Conv_I();
        }
        public void Ldind_R4()
        {
            Conv_R4();
        }
        public void Ldind_R8()
        {
            Conv_R8();
        }
        public void Ldind_Ref()
        {
            //nothing to do here: but should probably assure the stack is not empty or execute anyway to ensure the stack is not empty.
            frame.Stack.Push(frame.Stack.Pop());
        }
        public void Ldind_U1()
        {
            Conv_U1();
        }
        public void Ldind_U2()
        {
            Conv_U2();
        }
        public void Ldind_U4()
        {
            Conv_U4();
        }
        public void Ldlen()
        {
            //handle Ldlen
            {
                var array = frame.Stack.Pop();
                var arr = (Array)array;
                frame.Stack.Push(arr.Length);
                //break;
            }

        }
        public void Ldloc()
        {
            //handle Ldloc
            frame.Stack.Push(frame.Locals[Convert.ToInt32(frame.Current.Arg)].Value);

        }
        public void Ldloca()
        {
            //handle Ldloca
            frame.Stack.Push(frame.Locals[Convert.ToInt32(frame.Current.Arg)].Value);

        }
        public void Ldloc_0()
        {
            //handle Ldloc_0
            frame.Stack.Push(frame.Locals[0].Value);

        }
        public void Ldloc_1()
        {
            //handle Ldloc_1
            frame.Stack.Push(frame.Locals[1].Value);

        }
        public void Ldloc_2()
        {
            //handle Ldloc_2
            frame.Stack.Push(frame.Locals[2].Value);

        }
        public void Ldloc_3()
        {
            //handle Ldloc_3
            frame.Stack.Push(frame.Locals[3].Value);

        }
        public void Ldloc_S()
        {
            //handle Ldloc_S
            frame.Stack.Push(frame.Locals[(byte)Convert.ToInt32(frame.Current.Arg)].Value);

        }
        public void Ldloca_S()
        {
            //handle Ldloca_S
            frame.Stack.Push(frame.Locals[(byte)Convert.ToInt32(frame.Current.Arg)].Value);

        }
        public void Ldnull()
        {
            //handle Ldnull
            frame.Stack.Push(null);

        }
        public void Ldobj()
        {

            var arg = frame.Current.Arg;
            frame.Stack.Push(arg);

        }
        public void Ldsfld()
        {
            FieldInfo field = null;
            if (frame.Current.Arg is int fieldMetadataToken)
            {
                field = frame.ResolveFieldToken(fieldMetadataToken);
            }
            else if (frame.Current.Arg is FieldInfo field2)
            {
                field = field2;
            }

            object target = null;
            var value = field.GetValue(target);
            frame.Stack.Push(value);
        }
        public void Ldsflda()
        {
            // Missing: Ldsflda
            Ldsfld();

        }
        public void Ldstr()
        {
            //handle Ldstr

            if (frame.Current.Arg is string)
                frame.Stack.Push((string)frame.Current.Arg);
            else
                frame.Stack.Push(frame.ResolveStringToken((int)frame.Current.Arg));
            //break;


        }
        public void Ldtoken()
        {
            //handle Ldtoken
            var metaToken = (int)frame.Current.Arg;

            var memToken = frame.ResolveMemberToken(metaToken);
            var tokenType = memToken.GetType();
            switch (tokenType.Name)
            {
                case "RtFieldInfo":
                    {
                        var fieldInfo = frame.ResolveFieldToken(metaToken);
                        var handle = (FieldInfo)fieldInfo;
                        frame.Stack.Push(handle.FieldHandle);
                    }
                    break;
                case "RuntimeType":
                    {
                        var type = frame.ResolveTypeToken(metaToken);
                        var handle = (Type)type;
                        frame.Stack.Push(handle.TypeHandle);
                    }

                    break;
                default:
                    frame.Exception = new NotImplementedException($"{nameof(OpCode)} {frame.Code} token {tokenType.Name}");
                    flowControlTarget = ILStackFrameFlowControlTarget.Ret;
                    break;
                    //throw new NotImplementedException();
            }


        }
        public void Ldvirtftn()
        {
            // Missing: Ldvirtftn
            Ldftn();

        }
        public void Leave()
        {
            //redirect Leave => Br()
            //Br();
            //FlowControlTarget = ILStackFrameFlowControlTarget.ReadNext;
            throw new OpCodeNotImplementedException(ILOpCodeValues.Leave);
        }
        public void Leave_S()
        {
            //redirect Leave_S => Br_S()
            //Br_S();
            //FlowControlTarget = ILStackFrameFlowControlTarget.ReadNext;
            throw new OpCodeNotImplementedException(ILOpCodeValues.Leave_S);
        }
        public void Localloc()
        {
            // Missing: Localloc
            throw new OpCodeNotImplementedException(-497);

        }
        public void Mkrefany()
        {
            // Missing: Mkrefany
            throw new OpCodeNotImplementedException(198);

        }
        public void Mul()
        {
            //handle Mul
            // 90: //mul
            frame.Stack.Push(unchecked(((dynamic)frame.Stack.Pop()) * ((dynamic)frame.Stack.Pop())));

        }
        public void Mul_Ovf()
        {
            //handle Mul_Ovf
            // 216: //mul.ovf
            dynamic a = frame.Stack.Pop();
            dynamic b = frame.Stack.Pop();
            dynamic result = checked(a * b);
            frame.Stack.Push(result);

        }
        public void Mul_Ovf_Un()
        {
            //handle Mul_Ovf_Un
            // 217: //mul.ovf.un
            dynamic a = frame.Stack.Pop();
            dynamic b = frame.Stack.Pop();
            dynamic result = checked(a * b);
            frame.Stack.Push(result);

        }
        public void Neg()
        {
            //handle Neg
            //101: //neg
            frame.Stack.Push(-((dynamic)frame.Stack.Pop()));

        }
        public void Newarr()
        {
            //handle Newarr
            //141: //newarr
            Type arrayType = ((frame.Current.Arg is int) ? frame.ResolveTypeToken((int)frame.Current.Arg) : (Type)frame.Current.Arg);
            frame.Stack.Push(Array.CreateInstance(arrayType, (int)frame.Stack.Pop()));

        }
        public void Newobj()
        {
            //handle Newobj
            {
                var ctor = (System.Reflection.ConstructorInfo)frame.ResolveMethodToken((int)frame.Current.Arg);
                var ctorArgs = new object[ctor.GetParameters().Length];
                for (var i = ctorArgs.Length - 1; i > -1; i--)
                    ctorArgs[i] = frame.Stack.Pop();
                //var reversed = ctorArgs.Reverse().ToArray();
                var ctorResult = ctor.Invoke(ctorArgs);
                frame.Stack.Push(ctorResult);
                //break;
            }

        }
        public void Nop()
        {
            //handle Nop
        }
        public void Not()
        {
            //handle Not
            //102: //not
            frame.Stack.Push(~((dynamic)frame.Stack.Pop()));

        }

        //TODO: Unit test for null Frame code.
        public void NotImplemented()
        {
            var opCodeValue = frame?.Code.Value ?? 0;
            frame.Exception = new OpCodeNotImplementedException(opCodeValue);
            flowControlTarget = ILStackFrameFlowControlTarget.Ret;
        }

        public void Ret()
        {
            //handle Ret
            //goto Ret
            FlowControlTarget = ILStackFrameFlowControlTarget.Ret;
        }
        public void Starg_S()
        {
            //handle Starg_S
            frame.Args[(byte)Convert.ToInt32(frame.Current.Arg)] = frame.Stack.Pop();

        }
        public void Stloc_0()
        {
            //handle Stloc_0
            frame.Locals[0].Value = frame.Stack.Pop();

        }
        public void Stloc_1()
        {
            //handle Stloc_1
            frame.Locals[1].Value = frame.Stack.Pop();

        }
        public void Stloc_2()
        {
            //handle Stloc_2
            frame.Locals[2].Value = frame.Stack.Pop();

        }
        public void Stloc_3()
        {
            //handle Stloc_3
            frame.Locals[3].Value = frame.Stack.Pop();

        }
        public void Stloc_S()
        {
            //handle Stloc_S

            //Compiled code will have the arg index as a byte and C# chokes on (int)(object=byte)
            int index = (byte)(Convert.ToInt32(frame.Current.Arg));
            frame.Locals[index].Value = frame.Stack.Pop();

        }
        public void Or()
        {
            //handle Or
            // 96: //or
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) | ((dynamic)frame.Stack.Pop()));

        }
        public void Prefix7()
        {
            // Missing: Prefix7
            throw new OpCodeNotImplementedException(248);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Prefix6()
        {
            // Missing: Prefix6
            throw new OpCodeNotImplementedException(249);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Prefix5()
        {
            // Missing: Prefix5
            throw new OpCodeNotImplementedException(250);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Prefix4()
        {
            // Missing: Prefix4
            throw new OpCodeNotImplementedException(251);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Prefix3()
        {
            // Missing: Prefix3
            throw new OpCodeNotImplementedException(252);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Prefix2()
        {
            // Missing: Prefix2
            throw new OpCodeNotImplementedException(253);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Prefix1()
        {
            // Missing: Prefix1
            throw new OpCodeNotImplementedException(254);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Prefixref()
        {
            // Missing: Prefixref
            throw new OpCodeNotImplementedException(255);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Pop()
        {
            //handle Pop
            //38: //pop
            frame.Stack.Pop();// no push

        }
        public void Readonly()
        {
            // Missing: Readonly
            throw new OpCodeNotImplementedException(-482);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Refanytype()
        {
            // Missing: Refanytype
            throw new OpCodeNotImplementedException(-483);

        }
        public void Refanyval()
        {
            // Missing: Refanyval
            throw new OpCodeNotImplementedException(194);

        }
        public void Rem()
        {
            //handle Rem
            // 93: //rem
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) % ((dynamic)frame.Stack.Pop()));
        }
        public void Rem_Un()
        {
            //handle Rem_Un
            // 94: //rem.un
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) % ((dynamic)frame.Stack.Pop()));

        }
        public void Rethrow()
        {
            // Missing: Rethrow
            throw new OpCodeNotImplementedException(-486);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }

        //TODO: Unit test for meta data token
        public void Sizeof()
        {
            if(!(frame.Current.Arg is Type typeArg))
            {
                var typeToken = Convert.ToInt32(frame.Current.Arg);
                typeArg = frame.ResolveTypeToken(typeToken);
            }
            var sizeOf = System.Runtime.InteropServices.Marshal.SizeOf(typeArg);
            frame.Stack.Push(sizeOf);

        }
        public void Shl()
        {
            //handle Shl
            //98: //shl
            int shift = (int)frame.Stack.Pop();
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) << shift);

        }
        public void Shr()
        {
            //handle Shr
            //99: //shr
            int shift = (int)frame.Stack.Pop();
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) >> shift);

        }
        public void Shr_Un()
        {
            //handle Shr_Un
            //100: //shr.un
            int shift = (int)frame.Stack.Pop();
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) >> shift);

        }
        public void Starg()
        {
            //handle Starg
            frame.Args[(int)frame.Current.Arg] = frame.Stack.Pop();

        }
        public void Stelem()
        {
            //handle Stelem
            {
                object el = frame.Stack.Pop();
                int index = (int)frame.Stack.Pop();
                var array = (Array)frame.Stack.Pop();
                array.GetType()
                .GetMethod("SetValue", new[] { typeof(object), typeof(int) })
                .Invoke(array, new object[] { el, index });
                //break;
            }

        }
        public void Stelem_I()
        {
            Stelem_I4();

        }
        //TODO: Unit test typed array conversions.
        public void Stelem_I1()
        {
            //handle Stelem_I1
            {
                object el = frame.Stack.Pop();
                int index = (int)frame.Stack.Pop();
                var array = frame.Stack.Pop();
                var arrType = array.GetType();
                var arrElType = arrType.GetElementType();
                var elType = el.GetType();
                if (elType == arrElType) ((Array)array).SetValue(el, index);
                else if (arrElType == typeof(sbyte)) ((sbyte[])array)[index] = (sbyte)(int)el;
                else ((byte[])array)[index] = (byte)(int)el;
                //break;
            }

        }

        //TODO: Unit test typed array conversions.
        public void Stelem_I2()
        {
            //handle Stelem_I2
            {
                object el = frame.Stack.Pop();
                int index = (int)frame.Stack.Pop();
                var array = frame.Stack.Pop();
                var arrType = array.GetType();
                var arrElType = arrType.GetElementType();
                var elType = el.GetType();
                if (elType == arrElType) ((Array)array).SetValue(el, index);
                else if (arrElType == typeof(short)) ((Array)array).SetValue((short)(int)el, index);
                else if (arrElType == typeof(ushort)) ((Array)array).SetValue((ushort)(int)el, index);
                else ((Array)array).SetValue(Convert.ChangeType(el, arrElType), index);
                //break;
            }

        }

        //TODO: Unit test typed array conversions.
        public void Stelem_I4()
        {
            //handle Stelem_I4
            {
                object el = frame.Stack.Pop();
                int index = (int)frame.Stack.Pop();
                var array = frame.Stack.Pop();
                var arrType = array.GetType();
                var arrElType = arrType.GetElementType();
                var elType = el.GetType();
                if (elType == arrElType) ((Array)array).SetValue(el, index);
                else if (arrElType == typeof(int)) ((int[])array)[index] = (int)el;
                else ((uint[])array)[index] = (uint)el;
                //break;
            }

        }

        //TODO: Unit test typed array conversions.
        public void Stelem_I8()
        {
            //handle Stelem_I8
            {
                object el = frame.Stack.Pop();
                int index = (int)frame.Stack.Pop();
                var array = frame.Stack.Pop();
                var arrType = array.GetType();
                var arrElType = arrType.GetElementType();
                var elType = el.GetType();
                if (elType == arrElType) ((Array)array).SetValue(el, index);
                else if (arrElType == typeof(long)) ((long[])array)[index] = (long)el;
                else ((ulong[])array)[index] = (ulong)el;
                //break;
            }

        }

        //TODO: Unit test typed array conversions.
        public void Stelem_R4()
        {
            //handle Stelem_R4
            {
                object el = frame.Stack.Pop();
                int index = (int)frame.Stack.Pop();
                var array = frame.Stack.Pop();
                var arrType = array.GetType();
                var arrElType = arrType.GetElementType();
                var elType = el.GetType();
                if (elType == arrElType) ((Array)array).SetValue(el, index);
                else if (arrElType == typeof(float)) ((float[])array)[index] = (float)el;
                //else ((ulong[])array)[index] = (ulong)el;
                //break;
            }

        }

        //TODO: Unit test typed array conversions.
        public void Stelem_R8()
        {
            //handle Stelem_R8
            {
                object el = frame.Stack.Pop();
                int index = (int)frame.Stack.Pop();
                var array = frame.Stack.Pop();
                var arrType = array.GetType();
                var arrElType = arrType.GetElementType();
                var elType = el.GetType();
                if (elType == arrElType) ((Array)array).SetValue(el, index);
                else if (arrElType == typeof(double)) ((double[])array)[index] = (double)el;
                //else ((ulong[])array)[index] = (ulong)el;
                //break;
            }

        }

        //TODO: Unit test typed array conversions.
        public void Stelem_Ref()
        {
            //handle Stelem_Ref
            {
                object val = frame.Stack.Pop();
                int index = (int)frame.Stack.Pop();
                var array = frame.Stack.Pop();
                ((Array)array).SetValue(val, index);
                //break;
            }

        }
        public void Stind_I()
        {
            // Missing: Stind_I
            throw new OpCodeNotImplementedException(223);

        }
        public void Stind_I1()
        {
            // Missing: Stind_I1
            throw new OpCodeNotImplementedException(82);

        }
        public void Stind_I2()
        {
            // Missing: Stind_I2
            throw new OpCodeNotImplementedException(83);

        }
        public void Stind_I4()
        {
            // Missing: Stind_I4
            throw new OpCodeNotImplementedException(84);

        }
        public void Stind_I8()
        {
            // Missing: Stind_I8
            throw new OpCodeNotImplementedException(85);

        }
        public void Stind_R4()
        {
            // Missing: Stind_R4
            throw new OpCodeNotImplementedException(86);

        }
        public void Stind_R8()
        {
            // Missing: Stind_R8
            throw new OpCodeNotImplementedException(87);

        }
        public void Stind_Ref()
        {
            // Missing: Stind_Ref
            throw new OpCodeNotImplementedException(81);

        }
        public void Stloc()
        {
            //handle Stloc
            frame.Locals[Convert.ToInt32(frame.Current.Arg)].Value = frame.Stack.Pop();

        }
        public void Stfld()
        {
            //handle Stfld
            FieldInfo field = null;
            if (frame.Current.Arg is int fieldMetadataToken)
            {
                field = frame.ResolveFieldToken(fieldMetadataToken);
            }
            else if (frame.Current.Arg is FieldInfo field2)
            {
                field = field2;
            }


            var fo = frame.Stack.Pop();
            var target = frame.Stack.Pop();
            field.SetValue(target, fo);
            //frame.Stack.Push(value);
            //break;


        }
        public void Stsfld()
        {
            FieldInfo field = null;
            if (frame.Current.Arg is int fieldMetadataToken)
            {
                field = frame.ResolveFieldToken(fieldMetadataToken);
            }
            else if (frame.Current.Arg is FieldInfo field2)
            {
                field = field2;
            }


            var fo = frame.Stack.Pop();
            object target = null;
            field.SetValue(target, fo);

        }
        public void Stobj()
        {
            //handle Stobj
            throw new OpCodeNotImplementedException(ILOpCodeValues.Stobj);

        }

        //TODO: Unit test invalid argument and IL_[Address] argument
        public void Switch()
        {

            var switchArg = frame.Stack.Pop();
            if (switchArg is int arg)
            {
                int[] args = (int[])frame.Current.Arg;
                if (arg < args.Length)
                {
                    var delta = args[arg];
                    var directpos = (int)frame.Stream[frame.Position + 1].ByteIndex + delta;
                    frame.Position = frame.JumpTable[directpos];
                    FlowControlTarget = ILStackFrameFlowControlTarget.ReadNext;
                }
            }
            else
                throw new ILInstructionArgumentException("Switch argument must be an integer");
            //move to next;
            // Missing: Switch
            //throw new OpCodeNotImplementedException(69);
        }
        public void Sub()
        {
            //handle Sub
            // 89: //sub
            var b = (dynamic)frame.Stack.Pop();
            var a = (dynamic)frame.Stack.Pop();
            frame.Stack.Push(unchecked(a - b));

        }
        public void Sub_Ovf()
        {
            //handle Sub_Ovf
            // 218: //sub.ovf
            dynamic a = frame.Stack.Pop();
            dynamic b = frame.Stack.Pop();
            dynamic result = checked(a - b);
            frame.Stack.Push(result);

        }
        public void Sub_Ovf_Un()
        {
            //handle Sub_Ovf_Un
            // 219: //sub.ovf.un
            dynamic a = frame.Stack.Pop();
            dynamic b = frame.Stack.Pop();
            dynamic result = checked(a - b);
            frame.Stack.Push(result);

        }
        public void Tailcall()
        {
            // Missing: Tailcall
            throw new OpCodeNotImplementedException(-492);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Throw()
        {
            // Missing: Throw
            var exception = frame.Stack.Pop();
            if (exception == null)
            {
                exception = new NullReferenceException();
            }
            frame.Exception = (Exception)exception;
            //TODO: Implement catch logic
            FlowControlTarget = ILStackFrameFlowControlTarget.Ret;

            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Unaligned()
        {
            // Missing: Unaligned
            throw new OpCodeNotImplementedException(-494);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Unbox()
        {
            //handle Unbox
            //121: //unbox
            frame.Stack.Push(Convert.ChangeType(frame.Stack.Pop(), frame.ResolveTypeToken((int)frame.Current.Arg)));

        }
        public void Unbox_Any()
        {
            //handle Unbox_Any
            //165: //unbox.any
            frame.Stack.Push(Convert.ChangeType(frame.Stack.Pop(), frame.ResolveTypeToken((int)frame.Current.Arg)));

        }
        public void Volatile()
        {
            // Missing: Volatile
            throw new OpCodeNotImplementedException(-493);
            //TODO: FlowControlTarget = IlStackFrameFlowControlTarget.Inc;
        }
        public void Xor()
        {
            //handle Xor
            // 97: //xor
            frame.Stack.Push(((dynamic)frame.Stack.Pop()) ^ ((dynamic)frame.Stack.Pop()));

        }

    }
}
