using ILEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Reflection;

namespace ILEngine.Tests
{
    [TestClass()]
    public class OpCodeTests
    {
        private ILInstructionEngine engine = new ILEngine.ILInstructionEngine();


        public OpCodeTests()
        {

        }
        [TestMethod()]
        public void TestNop()
        {

            var Instructions = new List<ILInstruction>();
            int idx = 0;
            var instruction = new ILInstruction { OpCode = OpCodes.Nop, ByteIndex = idx };
            var result = engine.ExecuteTyped(Instructions, null, null);
            Assert.IsNull(result);
        }

        public static object LocalsAndArgsTestMethod(
            char arg_a, bool arg_b, sbyte arg_c, byte arg_d,
            short arg_e, ushort arg_f, int arg_g, uint arg_h,
            long arg_i, ulong arg_j, float arg_k, double arg_l,
            decimal arg_m, BigInteger arg_n, int ret)
        {
            var a = arg_a;
            var b = arg_b;
            var c = arg_c;
            var d = arg_d;
            var e = arg_e;
            var f = arg_f;
            var g = arg_g;
            var h = arg_h;
            var i = arg_i;
            var j = arg_j;
            var k = arg_k;
            var l = arg_l;
            var m = arg_m;
            var n = arg_n;
            if (ret == 0) return a;
            if (ret == 1) return b;
            if (ret == 2) return c;
            if (ret == 3) return d;
            if (ret == 4) return e;
            if (ret == 5) return f;
            if (ret == 6) return g;
            if (ret == 7) return h;
            if (ret == 8) return i;
            if (ret == 9) return j;
            if (ret == 10) return k;
            if (ret == 11) return l;
            if (ret == 12) return m;
            if (ret == 13) return n;
            return ret;

        }

        [TestMethod()]
        public void TestLocalsAndArgs()
        {
            var method = typeof(OpCodeTests).GetMethod(nameof(LocalsAndArgsTestMethod));
            var args = new object[] {
                    '0',
                    true,
                    (sbyte)2,
                    (byte)3,
                    (short)4,
                    (ushort)5,
                    (int)6,
                    (uint)7,
                    (long)8,
                    (ulong)9,
                    (float)10,
                    (double)11,
                    (decimal)12,
                    (BigInteger)13,
                    0
            };


            for (var i = 0; i < args.Length - 1; i++)
            {
                var expected = LocalsAndArgsTestMethod(
                    (char)args[0],
                    (bool)args[1],
                    (sbyte)args[2],
                    (byte)args[3],
                    (short)args[4],
                    (ushort)args[5],
                    (int)args[6],
                    (uint)args[7],
                    (long)args[8],
                    (ulong)args[9],
                    (float)args[10],
                    (double)args[11],
                    (decimal)args[12],
                    (BigInteger)args[13],
                    i);
                Assert.IsTrue(((IComparable)args[i]).CompareTo((IComparable)expected) == 0);
                args[14] = i;
                var actual = engine.ExecuteTyped(method, args);
                Assert.IsTrue(((IComparable)args[i]).CompareTo((IComparable)actual) == 0);

            }

        }


        private delegate int OpCodeDelegate(string msg, int ret);
        [TestMethod]

        private byte[] CompileMethod(OpCode op)
        {
            var testName = "IlUnitTest" + op.Value.ToString();
            var asmName = new AssemblyName(testName);
            AppDomain domain = AppDomain.CurrentDomain;

            AssemblyBuilder wrapperAssembly =
                domain.DefineDynamicAssembly(asmName,
                    AssemblyBuilderAccess.RunAndSave);

            var assemblyPath = asmName.Name + ".dll";

            ModuleBuilder wrapperModule =
                wrapperAssembly.DefineDynamicModule(asmName.Name,
                   assemblyPath);

            // Define a type to contain the method.
            TypeBuilder typeBuilder =
                wrapperModule.DefineType("testName", TypeAttributes.Public);

            var mb = typeBuilder.DefineMethod("test", MethodAttributes.Public | MethodAttributes.Static);
            var gen = mb.GetILGenerator();


         
            switch (op.OperandType)
            {

                //     The operand is a 32-bit integer branch target.
                case OperandType.InlineBrTarget: // = 0,
                    //instruction.Arg = br.ReadInt32();
                    gen.Emit(op, 0);
                    break;
                //     The operand is a 32-bit metadata token.
                case OperandType.InlineField: // = 1,
                    //instruction.Arg = br.ReadInt32();
                    gen.Emit(op, 1);
                    break;
                //     The operand is a 32-bit integer.
                case OperandType.InlineI: // = 2,
                    //instruction.Arg = br.ReadInt32();
                    gen.Emit(op, 0);
                    break;
                //     The operand is a 64-bit integer.
                case OperandType.InlineI8: // = 3,
                    //instruction.Arg = br.ReadInt64();
                    gen.Emit(op, (long)0);
                    break;
                //     The operand is a 32-bit metadata token.
                case OperandType.InlineMethod: // = 4,
                                               //instruction.Arg = br.ReadInt32();
                    gen.Emit(op, 0);
                    break;
                //     No operand.
                case OperandType.InlineNone: // = 5,
                    if (op.Value == 254)
                        gen.Emit(op, (byte)254);
                    else
                        gen.Emit(op);
                    break;
                //     The operand is reserved and should not be used.
#pragma warning disable CS0618 // Type or member is obsolete
                case OperandType.InlinePhi: // = 6,
#pragma warning restore CS0618 // Type or member is obsolete
                    throw new NotImplementedException();
                //     The operand is a 64-bit IEEE floating point number.
                case OperandType.InlineR: // = 7,
                    gen.Emit(op, (double)0);
                    //instruction.Arg = br.ReadDouble();
                    break;
                //     The operand is a 32-bit metadata signature token.
                case OperandType.InlineSig: // = 9,
                    gen.Emit(op, 0);
                    //instruction.Arg = br.ReadInt32();
                    break;
                //     The operand is a 32-bit metadata string token.
                case OperandType.InlineString: // = 10,
                    gen.Emit(op, 0);
                    //instruction.Arg = br.ReadInt32();
                    //throw new NotImplementedException();
                    break;
                //     The operand is the 32-bit integer argument to a switch instruction.
                case OperandType.InlineSwitch: // = 11,
                    gen.Emit(op, 0);
                    //instruction.Arg = br.ReadInt32();
                    break;
                //     The operand is a FieldRef, MethodRef, or TypeRef token.
                case OperandType.InlineTok: // = 12,
                    gen.Emit(op, 0);
                    //instruction.Arg = br.ReadInt32();
                    //throw new NotImplementedException();
                    break;
                //     The operand is a 32-bit metadata token.
                case OperandType.InlineType: // = 13,
                    gen.Emit(op, 0);
                    //instruction.Arg = br.ReadInt32();
                    break;
                //     The operand is 16-bit integer containing the ordinal of a local variable or an
                //     argument.
                case OperandType.InlineVar: // = 14,
                    gen.Emit(op, (short)0);
                    //instruction.Arg = br.ReadInt16();
                    break;
                //     The operand is an 8-bit integer branch target.
                case OperandType.ShortInlineBrTarget: // = 15,
                    gen.Emit(op, (byte)0);
                    //instruction.Arg = br.ReadByte();
                    break;
                //     The operand is an 8-bit integer.
                case OperandType.ShortInlineI: // = 16,
                    gen.Emit(op, (byte)0);
                    //instruction.Arg = br.ReadByte();
                    break;
                //     The operand is a 32-bit IEEE floating point number.
                case OperandType.ShortInlineR: // = 17,
                    gen.Emit(op, (float)0);
                    //instruction.Arg = br.ReadSingle();
                    break;
                //     The operand is an 8-bit integer containing the ordinal of a local variable or
                //     an argumenta.
                case OperandType.ShortInlineVar: // = 18
                    gen.Emit(op, (byte)0);
                    //instruction.Arg = br.ReadByte();
                    break;
                default:
                    gen.Emit(op);
                    break;
                    //throw new NotImplementedException();
            }


            gen.Emit(OpCodes.Ret);
            var type = typeBuilder.CreateType();
            var t = typeBuilder.GetMethod("test");
            var tbody = t.GetMethodBody();
            var il = tbody.GetILAsByteArray();
            return il;


        }
        public void TestIlInstructionReader()
        {
            Type[] args = { typeof(string), typeof(int) };


            var opCodeFields = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);

            ILOpCodeValues parseResult = ILOpCodeValues.Nop;
            foreach (var field in opCodeFields)
            {
                var OpCode = (OpCode)field.GetValue(null);
               
                var name = field.Name;
                var lookup = Enum.TryParse(name, out parseResult);
                var shortValue = unchecked((short)parseResult);
                if (shortValue == 254) continue; //Prefix 1 is used to indicate a two byte instruction.

                Assert.IsTrue(OpCode.Value == shortValue);

                var il = CompileMethod(OpCode);
                var ilStream = ILInstructionReader.FromByteCode(il);
                var first = ilStream.First();
                Assert.IsTrue(first.OpCode == OpCode);
                Assert.IsTrue(first.OpCode.Value == shortValue);
            }


        }

    }
}
