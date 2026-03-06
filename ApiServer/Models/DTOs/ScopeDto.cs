using Trustedbits.ApiServer.Services.Interfaces;

namespace Trustedbits.ApiServer.Models.DTOs;

/// <summary>
/// Contain information about a scope
/// </summary>
public class ScopeDto
{
    /// <summary>
    /// The name of the scope (e.g. "k8s_read_pods")
    /// </summary>
    public string? ScopeName = "";
    /// <summary>
    /// Description of what this scope grants
    /// </summary>
    public string? ScopeDescription = "";
    
    /// <summary>
    /// Actual scope string (e.g. "kubernets_pod:read")
    /// </summary>
    public string? ScopeValue = "";
    
    /// <summary>
    /// Indicates wheter this scope must be considered "valid" or not
    /// </summary>
    public bool? IsActive = true;
}

/// <summary>
/// Describes what fields to use in order to query a scope (<see cref="IScopeService"/>
/// </summary>
public class ScopeQueryDto
{
    /// <summary>
    /// Keyword contained in scope name 
    /// </summary>
    public string ContainsInName = string.Empty;
    
    /// <summary>
    /// Keyword contained in scope value 
    /// </summary>
    public string ContainsInValue = string.Empty;
    
    /// <summary>
    /// If the scope is active or not
    /// </summary>
    public bool IsActive = true;
}