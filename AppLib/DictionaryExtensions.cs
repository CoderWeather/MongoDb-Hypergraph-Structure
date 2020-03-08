using System.Collections.Generic;

namespace AppLib
{
    public static class DictionaryExtensions
    {
        public static void AddOrModify<T>(this IDictionary<T, int> dict, T key, int value = 1)
        {
            if (dict.TryAdd(key, value) is false)
                dict[key] += value;
        }

        public static void AddOrModify<T>(this IDictionary<T, int> dict, IEnumerable<KeyValuePair<T, int>> pairs)
        {
            foreach (var (key, value) in pairs)
                dict.AddOrModify(key, value);
        }
    }
}