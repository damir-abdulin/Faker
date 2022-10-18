using System.Linq.Expressions;

namespace FakerCore;

public class FakerConfig
{
    private Dictionary<Type, Dictionary<string, IGenerator>> _generators;

    public void Add<TTypeName, TFieldType, TGenerator>(Expression<Func<TTypeName, TFieldType>> getField)
    {
        // Create or open key Type.
        // Get field name.
        // Create or open key field name.
        // Save generator to dictionary.

        throw new NotImplementedException();
    }

    public IGenerator GetGenerator(Type type, string fieldName)
    {
        throw new NotImplementedException();
    }
    
}