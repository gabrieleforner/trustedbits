namespace Trustedbits.ApiServer.Models.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    
    public TenantSettings Settings { get; set; }
    
    public List<User> Users { get; set; }
    public List<Role> Roles { get; set; }
    
    public List<Scope> Scopes { get; set; }
}

public class TenantSettings
{
    public Tenant ParentTenant { get; set; }
    public Guid ParentTenantId { get; set; }
}