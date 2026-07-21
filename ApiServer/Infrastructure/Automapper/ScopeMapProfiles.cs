using AutoMapper;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Services.Schemas;

namespace Trustedbits.ApiServer.Infrastructure.Automapper;

public class ScopeMapProfiles : Profile
{
    public ScopeMapProfiles()
    {
        // Map usign constructor to ensure proper validation when creating the entity object
        CreateMap<CreateScopeRequest, Scope>()
            .ConstructUsing(src => new Scope(Guid.NewGuid(), src.Name, src.Value, src.Description));

        // Attribute names are identical
        CreateMap<Scope, CreateScopeResponse>();
    }
}