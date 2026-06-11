using Trustedbits.ApiServer.Core.Dto;

namespace Trustedbits.ApiServer.Core.Patterns;

/// <summary>
/// Represents a generic result from a service use case. It already returns
/// the errors at a service level.
/// </summary>
/// <see cref="ErrorDto"/>
/// <typeparam name="T">Type to return on success</typeparam>
public class GenericResult<T>
{
    public T? Data { get; }
    public bool IsFailed { get; }
    public ErrorDto? Error { get; }
    public ErrorType ErrorType { get; }

    // On-success constructor
    public GenericResult(T? data)
    {
        Data = data;
        IsFailed = false;
    }

    // On-failure constructor
    public GenericResult(ErrorDto? error, ErrorType errorType)
    {
        Error = error;
        ErrorType = errorType;
        IsFailed = true;
    }
}