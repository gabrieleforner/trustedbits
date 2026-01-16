namespace ApiServer.Models.Entities;

public class ScopeAction
{
    public Guid ParentScopeId { get; set; }
    public Scope ParentScope { get; set; } = new();

    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }

}