using System.Globalization;
using System.Reflection;

namespace FakerCore
{
    public class Faker
    {
        private const string DllName = "Generators";
        private readonly string[] _generatorsFromDllNames =  { "Generators.CharGenerator", "Generators.ShortGenerator"};
        
        private readonly FakerConfig _fakerConfig;
        private readonly List<Type> _generatedTypes;
        private readonly GeneratorContext _context;
        
        private IEnumerable<IGenerator> _generators;

        public Faker(FakerConfig fakerConfig)
        {
            _fakerConfig = fakerConfig;
            GetGenerators();
            _context = new GeneratorContext(this, new Random());

            _generatedTypes = new List<Type>();
        }
        
        public Faker() : this(new FakerConfig())
        { }
        
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
            var assembly = Assembly.LoadFrom(DllName);

            foreach (var generatorName in _generatorsFromDllNames)
            {
                var genType = assembly.GetType(generatorName);

                var methodInfoCanGenerate = genType?.GetMethod("CanGenerate",  BindingFlags.NonPublic | BindingFlags.Static);

                var param = new object[] { t };
                if (methodInfoCanGenerate is not null && (bool?)methodInfoCanGenerate.Invoke(null, param) == true)
                {
                    var methodInfoGenerate = genType.GetMethod("Generate",  BindingFlags.NonPublic | BindingFlags.Static);
                    result = methodInfoGenerate?.Invoke(null, param);
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
            FillPropertiesWithUserGenerators(newObject, t);
            FillFields(newObject, t);
            FillFieldsWithUserGenerators(newObject, t);

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

        private object[] GetArguments(Type t, ParameterInfo[] paramsInfo)
        {
            var args = new object[paramsInfo.Length];

            var textInfo = new CultureInfo("en-US",false).TextInfo;
            for (var i = 0; i < args.Length; i++)
            {

                if (paramsInfo[i].Name is not null)
                {
                    var fieldName = textInfo.ToTitleCase(paramsInfo[i].Name);
                    if (_fakerConfig.HasGenerator(t, fieldName))
                    {
                        var fieldInfo = t.GetField(fieldName, 
                            BindingFlags.Public | BindingFlags.Instance);
                        var propertyInfo = t.GetProperty(fieldName, 
                            BindingFlags.Public | BindingFlags.Instance);

                        if (fieldInfo is not null && fieldInfo.FieldType == paramsInfo[i].ParameterType ||
                            propertyInfo is not null && propertyInfo.PropertyType == paramsInfo[i].ParameterType)
                        {
                            var generator = _fakerConfig.GetGenerator(t, fieldName);
                            args[i] = generator.Generate(paramsInfo[i].ParameterType, _context);
                            continue;
                        }
                    }
                }
                
                args[i] = Create(paramsInfo[i].ParameterType);
            }

            return args;
        }

        private object CreateClass(Type t)
        {
            var constructor = GetConstructorForInvoke(t);
            
            var paramsInfo = constructor.GetParameters();
            var args = GetArguments(t, paramsInfo);
            
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

        private void FillPropertiesWithUserGenerators(object obj, Type t)
        {
            var propertiesInfo = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertiesInfo)
            {
                var propertyType = property.PropertyType;
                if (_fakerConfig.HasGenerator(t, property.Name) && property.CanWrite)
                {
                    var generator = _fakerConfig.GetGenerator(t, property.Name);

                    if (generator.CanGenerate(propertyType))
                        property.SetValue(obj, generator.Generate(propertyType, _context));
                }
            }
        }
        
        private void FillFieldsWithUserGenerators(object obj, Type t)
        {
            var fieldsInfo = t.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fieldsInfo)
            {
                if (_fakerConfig.HasGenerator(t, field.Name))
                {
                    var generator = _fakerConfig.GetGenerator(t, field.Name);
                    field.SetValue(obj, generator.Generate(field.FieldType, _context));
                }
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