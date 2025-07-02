namespace Infinity.Toolkit.Experimental;

public class SuccessResult : Result, Success
{
    public SuccessResult() : base()
    {
    }
}

public class SuccessResult<T> : Result<T>, Success
{
    public SuccessResult()
        : base(default!, []) => Succeeded = true;

    public SuccessResult(T data)
        : base(data, []) => Succeeded = true;
}

public class SuccessResult<T, TError> : Result<T, TError>, Success
    where TError : Error
{
    public SuccessResult(T data) : base(data, []) => Succeeded = true;
}
