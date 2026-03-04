namespace Trustedbits.ApiServer.Models.DTOs;

public class TenantDto
{
    public Guid? TenantId { get; set; } = Guid.Empty;
    public string? TenantName { get; set; } = "";
    public string? TenantDescription { get; set; } = "";
    public bool? IsActive { get; set; } = false;
}