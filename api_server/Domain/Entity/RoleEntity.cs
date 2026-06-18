namespace Trustedbits.ApiServer.Domain.Entity;

/// <summary>
/// Implementation-agnostic representation of an
/// RBAC role.
/// </summary>
public class RoleEntity
{
    /// <summary>
    /// Unique identifier of  the role
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Normalized (full lowercase) name of the role
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;
    /// <summary>
    /// Name of the role used when representing it outside the domain.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>
    /// Description of the role
    /// </summary>
    public string Description { get; set; } = string.Empty;
}