using System;
using Djambi.Model;

namespace Djambi.Engine.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> ToResult<T>(this T @this) =>
            Result.Value(@this);

        public static Result<T> ToErrorResult<T>(this Exception @this) =>
            Result.Error<T>(@this);

        public static Result<T2> Bind<T1, T2>(this Result<T1> @this, Func<T1, Result<T2>> projection) =>
            @this.HasValue ? projection(@this.Value) : Result.Error<T2>(@this.Error);

        public static Result<T2> Map<T1, T2>(this Result<T1> @this, Func<T1, T2> projection) =>
            @this.HasValue ? Result.Value(projection(@this.Value)) : Result.Error<T2>(@this.Error);

        public static Result<T> Assert<T>(this Result<T> @this, Func<T, bool> condition, Func<Exception> getError) =>
            @this.HasValue && condition(@this.Value) ? @this : Result.Error<T>(getError());

        public static Result<T> OnValue<T>(this Result<T> @this, Action<T> action)
        {
            if (@this.HasValue)
            {
                action(@this.Value);
            }

            return @this;
        }

        public static Result<T> OnError<T>(this Result<T> @this, Action<Exception> action)
        {
            if (!@this.HasValue)
            {
                action(@this.Error);
            }

            return @this;
        }
    }
}
