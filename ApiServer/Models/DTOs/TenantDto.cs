using ApiServer.Models.Entities;

namespace ApiServer.Models.DTOs;

public class TenantCreateDto
{
    public string Name { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
}

public class TenantDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}