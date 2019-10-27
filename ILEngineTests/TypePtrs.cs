using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngineTests
{

    public struct I1
    {
        public sbyte value; public override string ToString() => $"{value}";
        public static implicit operator sbyte(I1 value) => value.value;
        public static implicit operator I1(sbyte value) => new I1 { value = value };
    }
    public struct I2 { public short value; public override string ToString() => $"{value}"; }
    public struct I4 { public int value; public override string ToString() => $"{value}"; }
    public struct I8 { public long value; public override string ToString() => $"{value}"; }
    public struct U1 { public byte value; public override string ToString() => $"{value}"; }
    public struct U2 { public short value; public override string ToString() => $"{value}"; }
    public struct U4 { public uint value; public override string ToString() => $"{value}"; }
    public struct U8 { public ulong value; public override string ToString() => $"{value}"; }
    public struct R32 { public float value; public override string ToString() => $"{value}"; }
    public struct R64 { public float value; public override string ToString() => $"{value}"; }
    public struct D { public decimal value; public override string ToString() => $"{value}"; }

    public class TypePtrConvert
    {
        public static I2 ToI2(I1 value)
        {
            unsafe { return *(I2*)&value; }
        }
        public static I4 ToI4(I1 value)
        {
            unsafe { return *(I4*)&value; }
        }
        public static I8 ToI8(I1 value)
        {
            unsafe { return *(I8*)&value; }
        }
        public static U1 ToU1(I1 value)
        {
            unsafe { return *(U1*)&value; }
        }
        public static U2 ToU2(I1 value)
        {
            unsafe { return *(U2*)&value; }
        }
        public static U4 ToU4(I1 value)
        {
            unsafe { return *(U4*)&value; }
        }
        public static U8 ToU8(I1 value)
        {
            unsafe { return *(U8*)&value; }
        }
        public static R32 ToR32(I1 value)
        {
            return new R32 { value = value.value };
        }
        public static R64 ToR64(I1 value)
        {
            return new R64 { value = value.value };
        }
        public static D ToD(I1 value)
        {
            return new D { value = value.value };
        }

        public static I1 ToI1(R32 value)
        {
            unsafe { return *(I1*)&value; }
        }


        public static I8 ToI8(R32 value)
        {
            unsafe { return *(I8*)&value; }
        }
    }

}
