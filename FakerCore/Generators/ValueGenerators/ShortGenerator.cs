using System;

namespace FakerCore.Generators.ValueGenerators
{
    public class ShortGenerator : IGenerator
    {
        public object Generate(Type type, GeneratorContext context)
        {
            return context.Random.Next(short.MinValue, short.MaxValue);
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(short);
        }
    }
}