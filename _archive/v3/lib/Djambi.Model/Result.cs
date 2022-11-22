﻿using System;

namespace Djambi.Model
{
    public class Result<T>
    {
        private readonly T _value;

        private readonly Exception _error;

        internal Result(T value, Exception error)
        {
            _value = value;
            _error = error;
        }

        public T Value =>
            _error != null
                ? throw new InvalidOperationException($"Cannot get {nameof(Value)} from an {nameof(Error)} {nameof(Result)}.")
                : _value;

        public Exception Error =>
            _error == null
                ? throw new InvalidOperationException($"Cannot get {nameof(Error)} from a {nameof(Value)} {nameof(Result)}.")
                : _error;

        public bool HasValue => _error == null;

        public override string ToString() =>
            _error != null
                ? $"Value({_value})"
                : $"Error({_error})";
    }

    public static class Result
    { 
        public static Result<T> Value<T>(T value) =>
            new Result<T>(value, null);

        public static Result<T> Error<T>(Exception error) =>
            new Result<T>(default(T), error);
    }
}
