using AutoMapper;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;

namespace Trustedbits.ApiServer.Data.AutomapperProfiles;

public class TenantMappings : Profile
{
    public TenantMappings()
    {
        CreateMap<Tenant, TenantDto>();
        CreateMap<Tenant, TenantDto>();
    }
}