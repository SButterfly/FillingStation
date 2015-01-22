using System.Collections.Generic;
using System.Linq;

namespace FillingStation.Helpers
{
    public class DoubleDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public TKey GetKey(TValue value)
        {
            //TODO improve this
            return this.FirstOrDefault(kv => Equals(kv.Value, value)).Key;
        }

        public TValue GetValue(TKey key)
        {
            return this[key];
        }
    }
}