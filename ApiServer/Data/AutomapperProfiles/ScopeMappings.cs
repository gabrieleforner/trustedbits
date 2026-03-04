using AutoMapper;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;

namespace Trustedbits.ApiServer.Data.AutomapperProfiles;

// Automapper profile for Scope entity and Scope DTO
public class ScopeMappings : Profile
{
    public ScopeMappings()
    {
        CreateMap<Scope, ScopeDto>()
            .ForMember(dest => dest.ScopeName,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ScopeValue,
                opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive));

        CreateMap<ScopeDto, Scope>()
            .ForMember(dest => dest.Value,
                opt => opt.MapFrom(src => src.ScopeValue))
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.ScopeName))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive));
    }        
}