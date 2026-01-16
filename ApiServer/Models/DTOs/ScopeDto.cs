namespace ApiServer.Models.DTOs;

public class ScopeCreationDto
{
    public string? Name { get; set; }
    public string Value { get; set; } =  string.Empty;
    public bool? IsActive { get; set; }
    
    public List<ScopeActionCreationDto>? Actions { get; set; }
}

public class ScopeDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public List<ScopeActionDto> Actions { get; set; } = [];
}