﻿using System;

namespace FakerCore.Generators.ValueGenerators
{
    public class BoolGenerator : IGenerator
    {
        public object Generate(Type type, GeneratorContext context)
        {
            return context.Random.Next(0, 2) == 0;
        }

        public bool CanGenerate(Type type)
        {
            return type == typeof(bool);
        }
    }
}