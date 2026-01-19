namespace Trustedbits.ApiServer.Models.Entities;

public class Scope
{
    public Guid Id { get; set; }
    public Guid ParentTenantId { get; set; }
    public Tenant ParentTenant { get; set; }
    
    public string Name { get; set; }
    public string Value { get; set; }
    
    public List<ScopeAction> ScopeActions { get; set; }
    public bool IsActive { get; set; }
    
    public List<RoleScope<Guid>> RoleScopes { get; set; }
}