namespace FakerCore.Generators;

public class FloatGenerator : IGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        return (float)(context.Random.NextDouble() * context.Random.Next(int.MinValue, int.MaxValue));
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(float);
    }
}