using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Operations.Extentions
{
    public static class SortedListExtentions
    {
        public static IEnumerable<TValue> Where<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList, Func<TValue, bool> predicate)
        {
            return sortedList.Where(x => predicate.Invoke(x.Value)).Select(x => x.Value);
        }

        public static bool Any<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList, Func<TValue, bool> predicate)
        {
            return sortedList.Any(x => predicate.Invoke(x.Value));
        }

        public static int Count<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList, Func<TValue, bool> predicate)
        {
            return sortedList.Count(x => predicate.Invoke(x.Value));
        }

        public static int Sum<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList, Func<TValue, int> selector)
        {
            return sortedList.Sum(x => selector.Invoke(x.Value));
        }

        public static List<TValue> ToValueList<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList)
        {
            return sortedList.Select(x => x.Value).ToList();
        }

        public static TValue FirstOrDefaultValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList)
        {
            return sortedList.FirstOrDefault().Value;
        }

        public static TValue FirstOrDefaultValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList, Func<TValue, bool> predicate)
        {
            return sortedList.FirstOrDefault(x => predicate.Invoke(x.Value)).Value;
        }

        public static TValue LastOrDefaultValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList)
        {
            return sortedList.LastOrDefault().Value;
        }

        public static TValue LastOrDefaultValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sortedList, Func<TValue, bool> predicate)
        {
            return sortedList.LastOrDefault(x => predicate.Invoke(x.Value)).Value;
        }
    }
}
