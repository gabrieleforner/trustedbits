using AutoMapper;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;

namespace Trustedbits.ApiServer.Data.AutomapperProfiles;

public class RoleMappings : Profile
{
    public RoleMappings()
    {
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.RoleName,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.RoleDescription,
                opt => opt.MapFrom(src => src.RoleDescription))
            .ForMember(dest => dest.RoleScopes, opt => opt.MapFrom(src => src.RoleScopes));
        
        CreateMap<RoleDto, Role>()
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.RoleName))
            .ForMember(dest => dest.RoleDescription, 
                opt => opt.MapFrom(src => src.RoleDescription))
            .ForMember(dest => dest.RoleScopes, 
                opt => opt.MapFrom(src => src.RoleScopes))
;
    }    
}