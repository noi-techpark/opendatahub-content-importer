using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LTSAPI.Utils
{
    public static class DictionaryExtensions
    {
        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            if (dict == null)
                return defaultValue;

            TV value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static void RemoveNullValues(this IDictionary<string, string>? dict)
        {
            if (dict != null)
            {
                var keysWithNullValues = dict
                    .Where(kv => kv.Value == null)
                    .Select(kv => kv.Key)
                    .ToList();

                foreach (var key in keysWithNullValues)
                {
                    dict.Remove(key);
                }
            }
        }
    }
}
