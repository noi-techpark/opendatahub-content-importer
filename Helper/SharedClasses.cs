// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Helper
{
    public class RabbitNotifyMessage
    {
        public string? id { get; set; }
        public string? db { get; set; }
        public string? collection { get; set; }
    }

    public class TestObject
    {
        public string Name { get; set; }
        public string Property { get; set; }
        public string Source { get; set; }
    }

    public class RabbitIngressMessage
    {
        public string id { get; set; }
        public string provider { get; set; }
        public string timestamp { get; set; }
        //public object Rawdata { get; set; }
        public string rawdata { get; set; }
    }

    public static class DictionaryExtensions
    {
        //public static Dictionary<string, T> TryAddOrUpdate<T>(this Dictionary<string, T> dict, string key, T val)
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
    }
}
