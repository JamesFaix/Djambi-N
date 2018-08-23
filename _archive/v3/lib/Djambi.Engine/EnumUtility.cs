using System;
using System.Collections.Generic;
using System.Linq;

namespace Djambi.Engine
{
    public static class EnumUtility
    {
        public static IEnumerable<T> GetValues<T>()
            where T : struct, IConvertible =>
            Enum.GetValues(typeof(T))
                .OfType<object>()
                .Select(x => (T)x);
    }
}
