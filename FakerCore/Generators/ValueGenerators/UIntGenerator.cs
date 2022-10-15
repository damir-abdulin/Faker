using System;

namespace FakerCore.Generators.ValueGenerators
{
    public class UIntGenerator : IGenerator
    {
        public object Generate(Type type, GeneratorContext context)
        {
            return (uint)context.Random.Next();
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(uint);
        }
    }
}