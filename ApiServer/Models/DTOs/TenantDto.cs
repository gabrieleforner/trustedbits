namespace Trustedbits.ApiServer.Models.DTOs;

/// <summary>
/// Describe what fields must be provided
/// in order to create a tenant.
/// </summary>
public class TenantCreationDto
{
    public string TenantName { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Describe what fields must be provided
/// in order to represent a tenant.
/// </summary>
public class TenantDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; }
    public bool IsActive { get; set; }
}