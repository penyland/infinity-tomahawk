
namespace Infinity.Toolkit.Experimental;

public class ErrorResult : Result, Failure
{
    public ErrorResult(Error error)
        : this(error.Details, [error])
    {
        Succeeded = false;
    }

    public ErrorResult(string message)
        : this(message, [new(message)])
    {
    }

    public ErrorResult(string message, IReadOnlyCollection<Error> errors)
    {
        Message = message;
        Succeeded = false;
        Errors = errors ?? [];
    }

    public ErrorResult(IReadOnlyCollection<Error> errors)
        : this(string.Empty, errors)
    {
    }

    public ErrorResult(Exception exception)
        : this(exception.Message, [new ExceptionError(string.Empty, exception.Message, exception)])
    {
    }

    public string Message { get; }
}

public class ErrorResult<T> : Result<T>, Failure
{
    public ErrorResult(T value) 
        : base(value, [])
    {
        Succeeded = false;
    }

    public ErrorResult(Error error)
        : base(default!, [error])
    {
    }

    public ErrorResult(string message)
        : base(default!, [new Error(message)])
    {
        Message = message;
    }

    public ErrorResult(string message, IReadOnlyCollection<Error> errors)
        : base(default!, errors)
    {
        Message = message;
        Succeeded = false;
        Errors = errors ?? [];
    }

    public ErrorResult(IReadOnlyCollection<Error> errors)
        : base(default!, errors)
    {
        Succeeded = false;
        Errors = errors;
    }

    public ErrorResult(Exception exception)
        : base(default!, [new ExceptionError(exception.GetType().Name, exception.Message, exception)])
    {
        Succeeded = false;
        Message = exception.Message;
    }

    public string Message { get; }
}

public class ErrorResult<T, TError> : ErrorResult<T>, Failure
    where TError : Error
{
    public ErrorResult(TError error)
        : base([error])
    {
        Succeeded = false;
    }

    //public ErrorResult(TError error)
    //    : base(default!, [error])
    //{
    //}
    //public ErrorResult(string message, IReadOnlyCollection<TError> errors)
    //    : base(default!, errors)
    //{
    //    Message = message;
    //    Succeeded = false;
    //    Errors = errors ?? [];
    //}
    //public string Message { get; }
}
