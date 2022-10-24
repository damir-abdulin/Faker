﻿using System.Diagnostics;
using System.Reflection;

namespace FakerCore
{
    public class Faker
    {
        private readonly string[] _dllNames =  new string[] { "CharGenerator", "ShortGenerator"};
        
        private FakerConfig _fakerConfig;
        
        private List<Type> _generatedTypes = new List<Type>();
        private IEnumerable<IGenerator> _generators;
        private IEnumerable<IGenerator> _dynamicGenerators;
        private GeneratorContext _context;

        public Faker(FakerConfig fakerConfig)
        {
            _fakerConfig = fakerConfig;
        }
        
        public Faker() : this(new FakerConfig())
        {
            GetGenerators();
            _context = new GeneratorContext(this, new Random());
        }
        
        public T Create<T>()
        {
            return (T) Create(typeof(T));
        }

        public object Create(Type t)
        {
            object newObject;

            try
            {
                newObject = GenerateViaDll(t);
            }
            catch (Exception)
            {
                newObject = null;
            }
            
            newObject ??= _generators.Where(g => g.CanGenerate(t)).
                    Select(g => g.Generate(t, _context)).FirstOrDefault();

            if (newObject is null && IsDto(t) && !IsGenerated(t))
            {
                _generatedTypes.Add(t);
                newObject = FillClass(t);
                _generatedTypes.Remove(t);
            }

            return newObject ?? GetDefaultValue(t);
        }

        private void GetGenerators()
        {
            _generators = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IGenerator)))
                .Select(t => (IGenerator)Activator.CreateInstance(t));
        }

        private object GenerateViaDll(Type t)
        {
            object result = null;
            
            foreach (var generatorName in _dllNames)
            {
                var assembly = Assembly.Load(generatorName);
                var gen = (IGenerator)assembly.CreateInstance(generatorName);

                if (gen != null && gen.CanGenerate(t))
                {
                    result = gen.Generate(t, _context);
                    break;
                }
            }

            return result;
        }
        
        private object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
        
        private object FillClass(Type t)
        {
            var newObject = CreateClass(t);
            FillProperties(newObject, t);
            FillFields(newObject, t);

            return newObject;
        }

        private ConstructorInfo GetConstructorForInvoke(Type t)
        {
            var constructorsInfo = t.GetConstructors(BindingFlags.DeclaredOnly |
                                                     BindingFlags.Instance | BindingFlags.Public |
                                                     BindingFlags.NonPublic);

            var maxParamsCount = constructorsInfo[0].GetParameters().Length;
            var constructorWithMaxParams = 0;
            for (var i = 1; i < constructorsInfo.Length; i++)
            {
                var currParamsCount = constructorsInfo[i].GetParameters().Length;
                if (currParamsCount > maxParamsCount)
                {
                    constructorWithMaxParams = i;
                    maxParamsCount = currParamsCount;
                }
            }   

            return constructorsInfo[constructorWithMaxParams];
        }

        private object[] GetArguments(ParameterInfo[] paramsInfo)
        {
            var args = new object[paramsInfo.Length];

            for (var i = 0; i < args.Length; i++)
            {
                args[i] = Create(paramsInfo[i].ParameterType);
            }

            return args;
        }
        
        private object CreateClass(Type t)
        {
            var constructor = GetConstructorForInvoke(t);
            
            var paramsInfo = constructor.GetParameters();
            var args = GetArguments(paramsInfo);
            
            return constructor.Invoke(args);
        }

        private void FillProperties(object obj, IReflect t)
        {
            var propertiesInfo = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertiesInfo)
            {
                if (property.SetMethod is not null && property.SetMethod.IsPublic)
                {
                    var getMethod = property.GetMethod?.Invoke(obj, null);
                    var defValue = GetDefaultValue(property.PropertyType);
                    if (getMethod is null || getMethod.Equals(defValue))
                        property.SetValue(obj, Create(property.PropertyType));
                }
            }
        }

        private void FillFields(object obj, IReflect t)
        {
            var fieldsInfo = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var field in fieldsInfo)
            {   
                var getMethod = field.GetValue(obj);
                
                if (getMethod is null || getMethod.Equals(GetDefaultValue(field.FieldType)))
                    field.SetValue(obj, Create(field.FieldType));
            }
        }

        private bool IsDto(IReflect t)
        {
            var methods = t.GetMethods(BindingFlags.DeclaredOnly |
                                       BindingFlags.Instance | BindingFlags.Public);
            var methodsCount = methods.Length;

            var properties = t.GetProperties(BindingFlags.DeclaredOnly |
                                             BindingFlags.Instance | BindingFlags.Public);
            var propertiesCount = 0;
            foreach (var property in properties)
            {
                if (property.GetMethod is not null && property.GetMethod.IsPublic)
                    propertiesCount += 1;
                if (property.SetMethod is not null && property.SetMethod.IsPublic)
                    propertiesCount += 1;
            }

            return methodsCount - propertiesCount == 0;
        }

        private bool IsGenerated(Type t)
        {
            return _generatedTypes.Exists(genT => genT == t);
        }
    }
}