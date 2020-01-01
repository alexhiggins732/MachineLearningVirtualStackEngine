namespace ILEngine.CodeGenerator
{
    public interface ITypeRange
    {
        string TypeAlias { get; set; }
        object MinValueAsObject { get; }
        object DefaultValueAsObject { get; }
        object MaxValueAsObject { get; }
    }

}
