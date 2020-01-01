namespace ILOpcodeMeta
{
    public class OperandType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
        public int ByteSize { get; set; }
        public int BitSize { get; set; }
        public bool IsFloatingPoint { get; set; }
        public string SystemType { get; set; }

        public OperandType() { }
        public OperandType(int id, string name, int value, string description, int byteSize, int bitSize, bool isFloatingPoint, string systemType)
        {
            Id = id;
            Name = name;
            Value = value;
            Description = description;
            ByteSize = byteSize;
            BitSize = bitSize;
            IsFloatingPoint = isFloatingPoint;
            SystemType = systemType;
        }
    }
}
