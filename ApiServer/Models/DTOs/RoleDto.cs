namespace Trustedbits.ApiServer.Models.DTOs;

public class RoleDto
{
    public String? RoleName = "";
    public String? RoleDescription = "";
    public List<String>? RoleScopes = new();
}