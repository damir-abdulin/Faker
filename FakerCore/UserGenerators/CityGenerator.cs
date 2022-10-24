namespace FakerCore.UserGenerators;

public class CityGenerator : IUserGenerator
{
    private readonly string[] _cities = { "Minsk", "Paris", "Brest", "London" };

    public object Generate(Type type, GeneratorContext context)
    {
        var cityId = context.Random.Next(0, _cities.Length);

        return _cities[cityId];
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(string);
    }
}