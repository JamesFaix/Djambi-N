using System;
using System.Collections.Generic;
using System.Linq;

namespace Djambi.Engine
{
    public static class IEnumerableExtensions
    {
        private static Random _random = new Random();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> @this)
        {
            var list = @this.ToList();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                var k = _random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
