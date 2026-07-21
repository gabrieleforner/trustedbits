using Trustedbits.ApiServer.Services.Schemas;

namespace Trustedbits.ApiServer.Services;

public class Result<T>
{
    public T? Data { get; init; }
    public ErrorSchema? Error { get; init; }
    public bool Failed { get; init; }

    public Result(ErrorSchema? error)
    {
        Error = error;
        Failed = true;
    }

    public Result(T data)
    {
        Data = data; 
        Failed = false;
    }
}