namespace Trustedbits.ApiServer.Core.Patterns;

/// <summary>
/// Values that represent a generic error
/// reason. Used for resolve eventual error
/// codes (like HTTP)
/// </summary>
public enum ErrorType
{
    BadRequest,
    Conflict,
    NotFound,
    AlreadyExists,
    InsufficientPermissions,
}