
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

public class Error(string code, string details)
{
    public static readonly Error None = new("No error occurred.");

    public Error(string details)
        : this(string.Empty, details)
    {
    }

    public string Code { get; } = code ?? string.Empty;

    public string Details { get; } = details;
}

public class ExceptionError(string code, string details, Exception exception) : Error(code, details)
{
    public Exception Exception { get; } = exception ?? throw new ArgumentNullException(nameof(exception));

    public static implicit operator ExceptionError(Exception exception) => new(exception.GetType().Name, exception.Message, exception);
}
