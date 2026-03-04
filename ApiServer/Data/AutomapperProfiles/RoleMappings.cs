using AutoMapper;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;

namespace Trustedbits.ApiServer.Data.AutomapperProfiles;

public class RoleMappings : Profile
{
    public RoleMappings()
    {
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>();
    }    
}