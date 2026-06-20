namespace Trustedbits.ApiServer.Domain.Entity;

/// <summary>
/// Implementation-agnostic representation of a user.
/// </summary>
public class UserEntity
{
    /// <summary>
    /// Unique identifier of the user
    /// </summary>
    public Guid Id { get; set; } = Guid.Empty;
    /// <summary>
    /// Email associated with the account 
    /// </summary>
    public string DisplayEmail { get; set; } = string.Empty;

    /// <summary>
    /// Normalized account email (all lowercase)
    /// </summary>
    public string NormalizedEmail { get; set; } = string.Empty;

    /// <summary>
    /// Username of the account (defaults to account email)
    /// </summary>
    public string DisplayUsername { get; set; } = string.Empty;
    /// <summary>
    /// Normalized username of the account
    /// </summary>
    public string NormalizedUsername { get; set; } = string.Empty;

    /// <summary>
    /// Salt for Argon2 password
    /// </summary>
    public string PasswordHashSalt { get; set; } = string.Empty;
    /// <summary>
    /// Argon2-hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Date and time of the last access
    /// </summary>
    public DateTime LastAccess { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Date and time when the account has been created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Navigation property to the roles associated with the account
    /// </summary>
    public ICollection<RoleEntity> AssociatedRoles { get; set; } = new List<RoleEntity>();
}