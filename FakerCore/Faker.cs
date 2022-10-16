using System.Reflection;

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

        public object Create(Type t)
        {
            var newObject = _generators.Where(g => g.CanGenerate(t)).
                Select(g => g.Generate(t, _context)).FirstOrDefault();

            if (newObject is null)
            {
                // Generate object.
                newObject = FillClass(t);
            }
            
            return newObject ?? GetDefaultValue(t);
        }

        private void GetGenerators()
        {
            _generators = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IGenerator)))
                .Select(t => (IGenerator)Activator.CreateInstance(t));
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

        private object CreateClass(Type t)
        {
            var constructorsInfo = t.GetConstructors(BindingFlags.DeclaredOnly | 
                                                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            var paramsInfo = constructorsInfo[0].GetParameters();
            var args = new object[paramsInfo.Length];

            for (var i = 0; i < args.Length; i++)
            {
                args[i] = Create(paramsInfo[i].ParameterType);
            }

            return constructorsInfo[0].Invoke(args);
        }

        private void FillProperties(object obj, IReflect t)
        {
            var propertiesInfo = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertiesInfo)
            {
                property.SetValue(obj, Create(property.PropertyType)); 
            }
        }

        private void FillFields(object obj, IReflect t)
        {
            var fieldsInfo = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var field in fieldsInfo)
            {
                field.SetValue(obj, Create(field.FieldType));
            }
        }
    }
}