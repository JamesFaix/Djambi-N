export type Func<T, TResult> = (t : T) => TResult;

export function memoize<TKey, TValue>(f : Func<TKey, TValue>) : Func<TKey, TValue> {
    const cache = new Map<TKey, TValue>();
    return function (key : TKey) : TValue {
        let value = cache.get(key);
        if (value === undefined) {
            value = f(key);
            cache.set(key, value);
        }
        return value;
    };
}