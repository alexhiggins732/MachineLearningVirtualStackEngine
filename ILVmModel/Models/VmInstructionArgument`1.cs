namespace ILVmModel.Models
{
    public class VmInstructionArgument<T> : VmInstructionArgument
    {
        public T Value { get; private set; }
        public override int ArgumentIndex { get; protected set; }
        public override int DataByteSize { get; protected set; }

        public VmInstructionArgument(T value, int index)
        {
            this.Value = value;
            this.ArgumentIndex = index;

        }
    }
}
