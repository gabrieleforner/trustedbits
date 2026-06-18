namespace Trustedbits.ApiServer.Core.Patterns;

/// <summary>
/// Values that represent a generic error
/// reason. Used for resolve eventual error
/// codes (like HTTP)
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Bad request format.
    /// </summary>
    BadRequest,
    /// <summary>
    /// Conflicting value(s).
    /// </summary>
    Conflict,
    /// <summary>
    /// Resource not found.
    /// </summary>
    NotFound,
    /// <summary>
    /// Resource already exists.
    /// </summary>
    AlreadyExists,
    /// <summary>
    /// Insufficient permissions to access a certain resource.
    /// </summary>
    InsufficientPermissions,
}