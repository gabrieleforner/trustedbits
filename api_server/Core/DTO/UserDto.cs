namespace Trustedbits.ApiServer.Core.Dto;

/// <summary>
/// DTO for describing a user account
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier of the account
    /// </summary>
    public Guid UserId { get; set; } = Guid.Empty;
    /// <summary>
    /// Email associated with the account
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Username associated with the account
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Date and time of the last access
    /// </summary>
    public DateTime LastAccess { get; set; } = DateTime.Now;
    /// <summary>
    /// Date and time when the account has been created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}