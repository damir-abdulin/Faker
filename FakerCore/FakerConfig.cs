using System.Linq.Expressions;

namespace FakerCore;

public class FakerConfig
{
    private readonly Dictionary<Type, Dictionary<string, IGenerator>> _generators;

    public FakerConfig()
    {
        _generators = new Dictionary<Type, Dictionary<string, IGenerator>>();
    }
    
    public void Add<TTypeName, TFieldType, TGenerator>(Expression<Func<TTypeName, TFieldType>> getField)
    {
        if (!_generators.ContainsKey(typeof(TTypeName)))
        {
            _generators.Add(typeof(TTypeName), new Dictionary<string, IGenerator>());
        }
        
        var member = getField.Body as MemberExpression ?? throw new ArgumentException("Invalid expression");
        var fieldName = member.Member.Name;
        var generator = (IGenerator)Activator.CreateInstance(typeof(TGenerator));
        
        _generators[typeof(TTypeName)].Add(fieldName, generator);
    }

    public bool HasGenerator(Type type, string fieldName)
    {
        return _generators.ContainsKey(type) && _generators[type].ContainsKey(fieldName);
    }

    public IGenerator GetGenerator(Type type, string fieldName)
    {
        return HasGenerator(type, fieldName) ? _generators[type][fieldName] : null;
    }
    
}