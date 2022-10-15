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
            var newObject = _generators.Where(g => g.CanGenerate(t)).
                Select(g => g.Generate(t, _context)).FirstOrDefault();

            return newObject ?? GetDefaultValue(t);
        }

        private void GetGenerators()
        {
            _generators = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IGenerator)))
                .Select(t => (IGenerator)Activator.CreateInstance(t));
        }
        
        private static object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}
