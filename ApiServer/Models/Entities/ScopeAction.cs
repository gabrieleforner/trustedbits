namespace Trustedbits.ApiServer.Models.Entities;

public class ScopeAction
{
    public Guid Id { get; set; } 
    public Guid ParentScopeId { get; set; }
    public Scope ParentScope { get; set; }

    public string Name { get; set; }
    public string Value { get; set; }
    
    public bool IsActive { get; set; }

}