namespace Trustedbits.ApiServer.Models.DTOs;

public class ScopeDto
{
    public string? ScopeName = "";
    public string? ScopeDescription = "";
    public string? ScopeValue = "";
    public bool? IsActive = true;
}

public class ScopeQueryDto
{
    public string ContainsInName = string.Empty;
    public string ContainsInValue = string.Empty;
    public bool IsActive = true;
}