﻿namespace FakerCore.Generators;

public class ByteGenerator : IGenerator
{
    public object Generate(Type type, GeneratorContext context)
    {
        return context.Random.Next(byte.MinValue, byte.MaxValue);
    }

    public bool CanGenerate(Type type)
    {
        return type == typeof(byte);
    }
}
