namespace FakerCore.Generators;

public class ULongGenerator : IGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        return (ulong)context.Random.Next();
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(ulong);
    }
}