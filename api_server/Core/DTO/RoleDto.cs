namespace Trustedbits.ApiServer.Core.Dto;

/// <summary>
/// DTO for describing an RBAC role.
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Unique identifier of the role.
     /// </summary>
    public Guid RoleId { get; set; }
    /// <summary>
    /// Name of the role.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
    /// <summary>
    /// Description of the role.
    /// </summary>
    public string RoleDescription { get; set; } = string.Empty;
}