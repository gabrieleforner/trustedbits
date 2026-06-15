namespace Trustedbits.ApiServer.Domain.Entity;

public class RoleEntity
{
    public Guid Id { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}