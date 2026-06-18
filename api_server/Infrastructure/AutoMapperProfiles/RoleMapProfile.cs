using AutoMapper;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Infrastructure.AutoMapperProfiles;

/// <summary>
/// AutoMapper profile that defines mappings between <see cref="RoleEntity"/> and <see cref="RoleDto"/>.
/// </summary>
public class RoleMapProfile : Profile
{
    /// <summary>
    /// Initializes mapping configuration for role domain/service DTO conversions.
    /// </summary>
    public RoleMapProfile()
    {
        // Domain -> Service
        CreateMap<RoleEntity, RoleDto>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Description));

        // Service -> Domain
        CreateMap<RoleDto, RoleEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoleId))
            .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.RoleName.ToLower()))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.RoleName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.RoleDescription));
    }
}