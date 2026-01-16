namespace ApiServer.Models.Entities;


public class TenantSettings
{
    public Tenant ParentTenant { get; set; } = new Tenant();
    public Guid ParentTenantId { get; set; }
}