using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.Utils
{
    public static class DictionaryExtension
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value = default(TValue);
            dict.TryGetValue(key, out value);
            return value;
        }
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> setValue)
        {
            TValue value = default(TValue);
            if (!dict.TryGetValue(key, out value))
            {
                value = setValue(key);
                dict.Add(key, value);
            }
            return value;
        }
    }
}
