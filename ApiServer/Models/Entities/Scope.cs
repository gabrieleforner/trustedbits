namespace ApiServer.Models.Entities;

public class Scope
{
    public Guid ParentTenantId { get; set; }
    public Tenant ParentTenant { get; set; }
    
    public string Name { get; set; }
    public string Value { get; set; }
    
    public List<ScopeAction> ScopeActions { get; set; }
    public bool IsActive { get; set; }
}

public class ScopeAction
{
    public Guid ParentScopeId { get; set; }
    public Scope ParentScope { get; set; }

    public string Name { get; set; }
    public string Value { get; set; }
    
    public bool IsActive { get; set; }

}