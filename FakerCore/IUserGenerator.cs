namespace FakerCore;

public interface IUserGenerator
{
    object Generate(Type type, GeneratorContext context);
    bool CanGenerate(Type type);
}