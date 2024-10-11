using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericHelper
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

    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static void TryAddOrUpdateOnList(this ICollection<string> smgtags, ICollection<string> tagsToAdd)
        {
            foreach (var tag in tagsToAdd)
            {
                smgtags.TryAddOrUpdateOnList(tag);
            }
        }

        public static void TryRemoveOnList(this ICollection<string> smgtags, string tagToRemove)
        {
            if (smgtags.Contains(tagToRemove))
                smgtags.Remove(tagToRemove);
        }

        public static void TryRemoveOnList(this ICollection<string> smgtags, ICollection<string> tagsToRemove)
        {
            foreach (var tag in tagsToRemove)
            {
                smgtags.TryRemoveOnList(tag);
            }
        }

        public static void TryAddOrUpdateOnList(this ICollection<string> smgtags, string tagToAdd)
        {
            if (!smgtags.Contains(tagToAdd))
                smgtags.Add(tagToAdd);
        }

        public static IEnumerable<string> UnionIfNotNull(this ICollection<string>? sourceunion, ICollection<string>? listtounion)
        {
            if (sourceunion != null && listtounion != null)
                return sourceunion.Union(listtounion);
            else if (sourceunion == null && listtounion != null)
                return listtounion;
            else if (sourceunion != null && listtounion == null)
                return sourceunion;
            else
                return new List<string>();
        }

        public static ICollection<string>? ConverListToLowerCase(this ICollection<string>? smgtags)
        {
            if (smgtags != null && smgtags.Count > 0)
                return smgtags.Select(d => d.ToLower()).ToList();
            else
                return smgtags;
        }
    }
}
