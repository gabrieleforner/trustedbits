using AutoMapper;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Infrastructure.AutoMapperProfiles;

/// <summary>
/// AutoMapper profile that defines mappings between <see cref="ScopeEntity"/> and <see cref="ScopeDto"/>.
/// </summary>
public class ScopeMapProfile : Profile
{
    /// <summary>
    /// Initializes mapping configuration for scope domain/service DTO conversions.
    /// </summary>
    public ScopeMapProfile()
    {
        // Domain -> Service
        CreateMap<ScopeEntity, ScopeDto>()
            .ForMember(dest => dest.ScopeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ScopeName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.ScopeDescription, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ScopeValue, opt => opt.MapFrom(src => src.Value));
        
        // Service -> Domain
        CreateMap<ScopeDto, ScopeEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ScopeId))
            .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => (src.ScopeName ?? string.Empty).ToLower()))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.ScopeName))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => (src.ScopeValue ?? string.Empty).ToLower()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ScopeDescription));
    }
}