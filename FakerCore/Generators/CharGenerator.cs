namespace FakerCore.Generators;

public class CharGenerator : IGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        return (char)context.Random.Next();
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(float);
    }
}