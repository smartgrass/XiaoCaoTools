using System.Collections.Generic;

namespace GG.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Change the key of a dictionary entry
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            TKey oldKey, TKey newKey)
        {
            if (!dict.TryGetValue(oldKey, out TValue value))
                return false;

            dict.Remove(oldKey); // do not change order
            dict[newKey] = value; // or dict.Add(newKey, value) depending on ur comfort
            return true;
        }

        /// <summary>
        /// Checks if a dictionary value exists, if not creates it, if so updates it
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue newValue)
        {
            if (dic.TryGetValue(key, out TValue val))
            {
                // yay, value exists!
                dic[key] = newValue;
            }
            else
            {
                // darn, lets add the value
                dic.Add(key, newValue);
            }
        }
    }
}
