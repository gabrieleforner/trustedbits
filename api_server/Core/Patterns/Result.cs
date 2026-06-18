using Trustedbits.ApiServer.Core.Dto;

namespace Trustedbits.ApiServer.Core.Patterns;

/// <summary>
/// Represents a generic result from a service use case. It already returns
/// the errors at a service level.
/// </summary>
/// <see cref="ErrorDto"/>
/// <typeparam name="T">Type to return on success</typeparam>
public class Result<T>
{
    /// <summary>
    /// Data to return in case of success.
    /// </summary>
    public T? Data { get; }
    /// <summary>
    /// Flag that indicates whether it's failed.
    /// </summary>
    public bool IsFailed { get; }
    /// <summary>
    /// Error DTO.
    /// </summary>
    public ErrorDto? Error { get; }
    /// <summary>
    /// Generic error description.
    /// </summary>
    public ErrorType ErrorType { get; }

    /// <summary>
    /// On-success constructor.
    /// </summary>
    /// <param name="data">Data to return</param>
    public Result(T? data)
    {
        Data = data;
        IsFailed = false;
    }

    /// <summary>
    /// On-failure constructor.
    /// </summary> 
    public Result(ErrorDto? error, ErrorType errorType)
    {
        Error = error;
        ErrorType = errorType;
        IsFailed = true;
    }
}