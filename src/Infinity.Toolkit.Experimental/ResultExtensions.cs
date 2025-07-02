namespace Infinity.Toolkit.Experimental;

public static class ResultExtensions
{
    public static T? Value<T>(this Result<T> result)
    {
        return result switch
        {
            SuccessResult<T> success => success.Value,
            _ => default
        };
    }

    public static Result<T> Ok<T>(this Result<T> result, T value) => new SuccessResult<T>(value);

    public static Result<T2> Bind<T1, T2>(this Result<T1> result, Func<T1, Result<T2>> bind)
    {
        if (result.Succeeded)
        {
            return bind(result.Value);
        }

        return Result.Failure<T2>("");
    }

    /// <summary>
    /// Match the result with the provided functions which allows to execute different logic based on the result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">The function to execute when the result is successful.</param>
    /// <param name="onFailure">The function to execute when the result is a failure.</param>
    public static T Match<T>(this Result result, Func<T> onSuccess, Func<IReadOnlyCollection<Error>, T> onFailure)
    {
        return result.Succeeded ? onSuccess() : onFailure(result.Errors);
    }

    //public static TResult Match<T, TError, TResult>(this Result<T, TError> result, Func<T, TResult> onSuccess, Func<TError, TResult> onFailure)
    //    where TError : Error
    //{
    //    return result.Succeeded ? onSuccess(result.Value) : onFailure(result.Errors);
    //}

    public static Result<T> ToResult<T>(this T value) => new SuccessResult<T>(value);

    public static Result<T2> Map<T1, T2, TError>(this Result<T1> result, Func<T1, T2> mapFunc)
    {
        return result.Succeeded ? Result<T2>.Success(mapFunc(result.Value)) : new ErrorResult<T2>(result.Errors);
    }

}
