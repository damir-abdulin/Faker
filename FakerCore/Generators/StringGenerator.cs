namespace FakerCore.Generators;

public class StringGenerator : IGenerator
{
    private const int MinLength = 1;
    private const int MaxLength = 20;
    
    public object Generate(Type type, GeneratorContext context)
    {
        var length = context.Random.Next(MinLength, MaxLength + 1);

        Span<char> buffer = stackalloc char[length];

        
        return (uint)context.Random.Next();
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(string);
    }
}