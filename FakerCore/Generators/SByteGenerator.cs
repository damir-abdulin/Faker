namespace FakerCore.Generators;

public class SByteGenerator : IGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        return context.Random.Next(sbyte.MinValue, sbyte.MaxValue);
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(sbyte);
    }
}
