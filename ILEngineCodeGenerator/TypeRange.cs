namespace ILEngine.CodeGenerator
{
    public abstract class TypeRange : ITypeRange
    {
        public string TypeAlias { get; set; }
        public abstract object MinValueAsObject { get; }
        public abstract object DefaultValueAsObject { get; }
        public abstract object MaxValueAsObject { get; }
    }

}
