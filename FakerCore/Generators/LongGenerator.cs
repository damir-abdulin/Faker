namespace FakerCore.Generators;

public class LongGenerator : IGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        return (long)context.Random.Next(int.MinValue, int.MaxValue);
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(long);
    }
}
