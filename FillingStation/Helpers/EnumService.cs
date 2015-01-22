using System.Collections.Generic;

namespace FillingStation.Helpers
{
    public abstract class EnumService<T>
    {
        public abstract IEnumerable<T> AllItems();
        public abstract T ToEnum(object value);
        public abstract object ToObject(T value);
    }
}