namespace FakerCore.Generators;

public class ShortGenerator : IGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        return (short)context.Random.Next(short.MinValue, short.MaxValue);
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(short);
    }
}
