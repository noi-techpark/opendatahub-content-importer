using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TValue> TryAddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val)
        {
            if (dict.ContainsKey(key))
                dict[key] = val;
            else
                dict.Add(key, val);

            return dict;
        }

        public static IDictionary<string, string> ConvertToLowercase(this IDictionary<string, string> dict, bool keys, bool values)
        {
            var newdict = new Dictionary<string, string>();

            foreach (var kvp in dict)
            {
                newdict.Add(keys ? kvp.Key.ToLower() : kvp.Key, values ? kvp.Value.ToLower() : kvp.Value);
            }

            return newdict;
        }

        public static bool ContentEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> otherDictionary)
        {
            return (otherDictionary ?? new Dictionary<TKey, TValue>())
                .OrderBy(kvp => kvp.Key)
                .SequenceEqual((dictionary ?? new Dictionary<TKey, TValue>())
                                   .OrderBy(kvp => kvp.Key));
        }
    }
}
