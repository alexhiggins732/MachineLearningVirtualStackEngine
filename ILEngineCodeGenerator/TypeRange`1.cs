namespace ILEngine.CodeGenerator
{
    public class TypeRange<T> : TypeRange
    {
        public T MinValue;
        public T DefaultValue;
        public T MaxValue;

        public override object MinValueAsObject => MinValue;

        public override object DefaultValueAsObject => DefaultValue;

        public override object MaxValueAsObject => MaxValue;
    }

}
