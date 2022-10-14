using System;

namespace FakerCore
{
    public class Faker
    {
        public T Create<T>()
        {
            return (T) GetDefaultValue(typeof(T));
        }

        private object Create(Type t)
        {
            return null;
        }

        private object GetDefaultValue(Type t)
        {
            object defaultValue = null;
            
            if (t.IsValueType)
                defaultValue = Activator.CreateInstance(t);
            
            return defaultValue;
        }
    }
}