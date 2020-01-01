namespace ILEngine.CodeGenerator
{
    public class TypeRanges
    {
        public static TypeRange<sbyte> I8 = new TypeRange<sbyte>
        {
            TypeAlias = nameof(I8),
            MinValue = sbyte.MinValue,
            DefaultValue = default(sbyte),
            MaxValue = sbyte.MaxValue
        };
        public static TypeRange<short> I16 = new TypeRange<short>
        {
            TypeAlias = nameof(I16),
            MinValue = short.MinValue,
            DefaultValue = default(short),
            MaxValue = short.MaxValue
        };
        public static TypeRange<int> I32 = new TypeRange<int>
        {
            TypeAlias = nameof(I32),
            MinValue = int.MinValue,
            DefaultValue = default(int),
            MaxValue = int.MaxValue
        };
        public static TypeRange<long> I64 = new TypeRange<long>
        {
            TypeAlias = nameof(I32),
            MinValue = long.MinValue,
            DefaultValue = default(long),
            MaxValue = long.MaxValue
        };

        public static TypeRange<byte> U8 = new TypeRange<byte>
        {
            TypeAlias = nameof(U8),
            MinValue = byte.MinValue,
            DefaultValue = default(byte),
            MaxValue = byte.MaxValue
        };
        public static TypeRange<ushort> U16 = new TypeRange<ushort>
        {
            TypeAlias = nameof(U16),
            MinValue = ushort.MinValue,
            DefaultValue = default(ushort),
            MaxValue = ushort.MaxValue
        };
        public static TypeRange<uint> U32 = new TypeRange<uint>
        {
            TypeAlias = nameof(U32),
            MinValue = uint.MinValue,
            DefaultValue = default(uint),
            MaxValue = uint.MaxValue
        };
        public static TypeRange<ulong> U64 = new TypeRange<ulong>
        {
            TypeAlias = nameof(U32),
            MinValue = ulong.MinValue,
            DefaultValue = default(ulong),
            MaxValue = ulong.MaxValue
        };
        public static TypeRange<float> R4 = new TypeRange<float>
        {
            TypeAlias = nameof(R4),
            MinValue = float.MinValue,
            DefaultValue = default(float),
            MaxValue = float.MaxValue
        };
        public static TypeRange<double> R8 = new TypeRange<double>
        {
            TypeAlias = nameof(R8),
            MinValue = double.MinValue,
            DefaultValue = default(double),
            MaxValue = double.MaxValue
        };
        public static TypeRange<decimal> D = new TypeRange<decimal>
        {
            TypeAlias = nameof(R8),
            MinValue = decimal.MinValue,
            DefaultValue = default(decimal),
            MaxValue = decimal.MaxValue
        };

        public static TypeRange[] All = new TypeRange[]
        {
            I8, I16, I32, I64, U8, U16, U32, R4, R8, D
        };
    }

}
