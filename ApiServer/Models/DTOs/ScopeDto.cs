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
    public string? ScopeName { get; set; } = "";
    /// <summary>
    /// Description of what this scope grants
    /// </summary>
    public string? ScopeDescription { get; set; } = "";
    
    /// <summary>
    /// Actual scope string (e.g. "kubernets_pod:read")
    /// </summary>
    public string? ScopeValue { get; set; } = "";
    
    /// <summary>
    /// Indicates wheter this scope must be considered "valid" or not
    /// </summary>
    public bool? IsActive { get; set; } = true;
}

/// <summary>
/// Describes what fields to use in order to query a scope (<see cref="IScopeService"/>
/// </summary>
public class ScopeQueryDto
{
    /// <summary>
    /// Keywords contained in scope name 
    /// </summary>
    public string KeywordInName { get; set; } = "";
    
    /// <summary>
    /// Keywords contained in scope value 
    /// </summary>
    public string KeywordInValue { get; set; } = "";
    
    /// <summary>
    /// If the scope is active or not
    /// </summary>
    public bool? IsActive = true;
}