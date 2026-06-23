using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Trustedbits.ApiServer.Core;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Domain.Entity;
using Trustedbits.ApiServer.Domain.Repository.impl;
using Trustedbits.ApiServer.Infrastructure.AutoMapperProfiles;
using Trustedbits.ApiServer.Infrastructure.EFCore;

namespace Trustedbits.ApiTests.TestSupport;

internal static class TestData
{
    public static readonly Guid ScopeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid OtherScopeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid RoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static ScopeDto ScopeDto(
        Guid? id = null,
        string? name = "Read Users",
        string? value = "users:read",
        string? description = "Read user records")
    {
        return new ScopeDto
        {
            ScopeId = id ?? ScopeId,
            ScopeName = name,
            ScopeValue = value,
            ScopeDescription = description
        };
    }

    public static ScopeEntity ScopeEntity(
        Guid? id = null,
        string displayName = "Read Users",
        string normalizedName = "read users",
        string value = "users:read",
        string description = "Read user records")
    {
        return new ScopeEntity
        {
            Id = id ?? ScopeId,
            DisplayName = displayName,
            NormalizedName = normalizedName,
            Value = value,
            Description = description
        };
    }

    public static RoleEntity RoleEntity(
        Guid? id = null,
        string displayName = "Administrator",
        string normalizedName = "administrator",
        string description = "Platform administrator")
    {
        return new RoleEntity
        {
            Id = id ?? RoleId,
            DisplayName = displayName,
            NormalizedName = normalizedName,
            Description = description
        };
    }

    public static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ScopeMapProfile>();
            cfg.AddProfile<RoleMapProfile>();
        }, NullLoggerFactory.Instance);

        return configuration.CreateMapper();
    }

    public static ServerDbContext CreateDbContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ServerDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ServerDbContext(options);
    }

    public static ScopeService CreateScopeService(
        ServerDbContext dbContext,
        IMapper? mapper = null,
        ILogger<ScopeService>? logger = null)
    {
        var scopeRepository = new ScopeRepositoryImpl(dbContext);
        return new ScopeService(
            mapper ?? CreateMapper(),
            scopeRepository,
            logger ?? Mock.Of<ILogger<ScopeService>>());
    }

    public static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var result = new List<T>();
        await foreach (var item in source)
        {
            result.Add(item);
        }

        return result;
    }
}
