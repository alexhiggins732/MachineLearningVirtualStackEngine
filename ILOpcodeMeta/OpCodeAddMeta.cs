using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILOpcodeMeta
{
    public class ILType
    {
        public string Name;
        public Type Type;
        public string TypeName => Type.Name;
        public string TypeAlias;
        public bool Unsigned;
        public int Bits;
        public bool FloatingPoint;

    }
    public class ILTypes
    {
        public static readonly ILType I8 = new ILType { Name = nameof(I8), Type = typeof(SByte), TypeAlias = "sbyte", Unsigned = false, Bits = 8 };
        public static readonly ILType I16 = new ILType { Name = nameof(I16), Type = typeof(Int16), TypeAlias = "short", Unsigned = false, Bits = 16 };
        public static readonly ILType I32 = new ILType { Name = nameof(I32), Type = typeof(Int32), TypeAlias = "int", Unsigned = false, Bits = 32 };
        public static readonly ILType I64 = new ILType { Name = nameof(I64), Type = typeof(Int64), TypeAlias = "long", Unsigned = false, Bits = 64 };
        public static readonly ILType U8 = new ILType { Name = nameof(U8), Type = typeof(Byte), TypeAlias = "byte", Unsigned = true, Bits = 8 };
        public static readonly ILType U16 = new ILType { Name = nameof(U16), Type = typeof(UInt16), TypeAlias = "ushort", Unsigned = true, Bits = 16 };
        public static readonly ILType U32 = new ILType { Name = nameof(U32), Type = typeof(UInt32), TypeAlias = "uint", Unsigned = true, Bits = 32 };
        public static readonly ILType U64 = new ILType { Name = nameof(U64), Type = typeof(UInt64), TypeAlias = "ulong", Unsigned = true, Bits = 64 };
        public static readonly ILType R4 = new ILType { Name = nameof(R4), Type = typeof(float), TypeAlias = "float", Unsigned = false, Bits = 32, FloatingPoint = true };
        public static readonly ILType R8 = new ILType { Name = nameof(R8), Type = typeof(double), TypeAlias = "double", Unsigned = false, Bits = 64, FloatingPoint = true };
        public static ILType[] AllTypes = new[]
        {
            I8,I16,I32, I64, U8, U16,U32,U64, R4, R8
        };
        public static string GetOpAddReturnType(ILType a, ILType b)
        {
            if (a.Bits > b.Bits) return a.TypeAlias;
            return b.TypeAlias;

            //bool opIsUnsigned = a.Unsigned | b.Unsigned;
            //if (!opIsUnsigned)
            //{
            //    if (a.Bits > b.Bits) return a.TypeAlias;
            //    return b.TypeAlias;
            //}
            //else
            //{
            //    if (a.Unsigned & b.Unsigned)
            //    {
            //        if (a.Bits > b.Bits) return a.TypeAlias;
            //        return b.TypeAlias;
            //    } else
            //    {
            //        if (a.Unsigned)
            //    }
            //}
            //var typeA = a.Type;
            //var typeB = b.Type;
            //switch (typeA)
            //{
            //    default:
            //        return nameof(Int32);
            //}
        }
    }
    class OpCodeAddMeta
    {
    }
}
