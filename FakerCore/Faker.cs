using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using FakerCore.Generators;

namespace FakerCore
{
    public class Faker
    {
        private IEnumerable<IGenerator> _generators;
        private GeneratorContext _context;

        public Faker()
        {
            GetGenerators();
            _context = new GeneratorContext(this, new Random());
        }
        
        public T Create<T>()
        {
            return (T) Create(typeof(T));
        }

        private object Create(Type t)
        {
            foreach (var generator in _generators)
            {
                if (generator.CanGenerate(t))
                    return generator.Generate(t, _context);
            }

            return null;
        }

        private void GetGenerators()
        {
            _generators = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IGenerator)))
                .Select(t => (IGenerator)Activator.CreateInstance(t));
        }
    }
}
