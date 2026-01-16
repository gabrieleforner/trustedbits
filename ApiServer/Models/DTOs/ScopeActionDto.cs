namespace ApiServer.Models.DTOs;


public class ScopeActionCreationDto
{
    public string? Name { get; set; }
    public string? Value { get; set; }
    public bool? IsActive { get; set; }
}

public class ScopeActionDto
{
    public string Name { get; set; }  = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}