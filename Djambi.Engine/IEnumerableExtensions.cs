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

        public static IEnumerable<TResult> LeftOuterJoin<TLeft, TRight, TKey, TResult>(
            this IEnumerable<TLeft> first,
            IEnumerable<TRight> second,
            Func<TLeft, TKey> getLeftKey,
            Func<TRight, TKey> getRightKey,
            Func<TLeft, TRight, TResult> getResult,
            Func<TLeft, TResult> getDefaultResult)
        {
            IEnumerable<TResult> getGroupResults (TLeft left, IEnumerable<TRight> rights)
            {
                rights = rights.ToList();
                return rights.Any()
                    ? rights.Select(r => getResult(left, r))
                    : Enumerable.Repeat(getDefaultResult(left), 1);
            }

            return first
                .GroupJoin(second, getLeftKey, getRightKey, getGroupResults)
                .SelectMany(seq => seq);
        }
    }
}
