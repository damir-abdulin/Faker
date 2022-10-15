using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using FakerCore.Generators;

namespace FakerCore
{
    public class Faker
    {
        private List<IGenerator> _generators;
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

        public void GetGenerators()
        {
            _generators = new List<IGenerator>();
            
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var filter = new TypeFilter(InterfaceFilter);

            foreach (var type in types)
            {
                var myInterfaces = type.FindInterfaces(filter, "FakerCore.Generators.IGenerator");
                
                if (myInterfaces.Length > 0)
                {
                    var generator = (IGenerator)Activator.CreateInstance(type);
                    _generators.Add(generator);
                }
            }
            
        }
        
        private bool InterfaceFilter(Type typeObj, Object criteriaObj)
        {
            return typeObj.ToString() == criteriaObj.ToString();
        }
    }
}
