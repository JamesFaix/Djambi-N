using System;

namespace Djambi.Model
{
    public class Option<T>
    {
        private readonly T _value;

        private readonly bool _hasValue;

        internal Option(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        public T Value =>
            _hasValue
                ? throw new InvalidOperationException($"Cannot get {nameof(Value)} from a {nameof(None)} {nameof(Option)}.")
                : _value;

        public bool HasValue => _hasValue;

        public override string ToString() =>
            _hasValue
                ? $"Some({_value})"
                : $"None";
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value) =>
            new Option<T>(value, true);

        public static Option<T> None<T>() =>
            new Option<T>(default(T), false);
    }
}
