namespace FakerCore.UserGenerators;

public class AgeGenerator : IUserGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        var age = context.Random.Next(18, 60);

        return age;
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(int);
    }
}