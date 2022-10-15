namespace FakerCore.Generators;

public class DoubleGenerator : IGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        return (double)(context.Random.NextDouble() * context.Random.Next(int.MinValue, int.MaxValue));
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(double);
    }
}