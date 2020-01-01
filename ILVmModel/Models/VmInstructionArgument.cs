namespace ILVmModel.Models
{
    public abstract class VmInstructionArgument
    {
        public abstract int DataByteSize { get; protected set; }
        public abstract int ArgumentIndex { get; protected set; }
    }
}
