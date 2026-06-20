using AutoMapper;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Domain.Entity;

namespace Trustedbits.ApiServer.Infrastructure.AutoMapperProfiles;

/// <summary>
/// AutoMapper profile that defines mappings between <see cref="UserEntity"/> and <see cref="UserDto"/>.
/// </summary>
public class UserMapProfile : Profile
{
    /// <summary>
    /// Initializes mapping configuration for scope domain/service DTO conversions.
    /// </summary>
    public UserMapProfile()
    {
        // Domain -> Service
        CreateMap<UserEntity, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.DisplayUsername))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.DisplayEmail));
        
        // Service -> Domain
        CreateMap<UserDto, UserEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.DisplayUsername, opt => opt.MapFrom(src => (src.Username ?? string.Empty).ToLower()))
            .ForMember(dest => dest.DisplayEmail, opt => opt.MapFrom(src => src.Email));
    }
}