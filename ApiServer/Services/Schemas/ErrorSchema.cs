namespace Trustedbits.ApiServer.Services.Schemas;

public enum ErrorType
{
    NotFound,
    BadRequest,
    ServerError,
    AlreadyExists,
}

public record ErrorSchema
{
    public required string Message { get; init; }
    public required ErrorType Type { get; init; }
}