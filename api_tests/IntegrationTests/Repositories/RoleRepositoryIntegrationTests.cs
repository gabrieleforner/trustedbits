using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Repository.impl;
using Trustedbits.ApiServer.Infrastructure.EFCore;
using Trustedbits.ApiTests.TestSupport;

namespace Trustedbits.ApiTests.IntegrationTests.Repositories;

[TestClass]
public class RoleRepositoryIntegrationTests
{
    [TestMethod]
    public async Task RoleRepository_BasicMethods_ShouldPersistAndQueryRoles()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        var role = TestData.RoleEntity();

        // Act
        var created = await repository.CreateAsync(role);
        var byId = await repository.GetByIdAsync(role.Id);
        var byName = await repository.GetByNameAsync("ADMINISTRATOR");
        var all = (await repository.GetAllAsync(1, 10)).ToList();
        created.Description = "Updated";
        await repository.UpdateAsync(created);
        await repository.DeleteAsync(created);

        // Assert
        Assert.AreEqual(role.Id, created.Id);
        Assert.IsNotNull(byId);
        Assert.IsNotNull(byName);
        Assert.AreEqual(1, all.Count);
        Assert.AreEqual(0, (await repository.GetAllAsync(1, 10)).Count());
    }

    [TestMethod]
    public async Task SearchAsync_TermMatchesNameOrDescription_ShouldReturnMatches()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.RoleEntity(description: "Full access"));
        await repository.CreateAsync(TestData.RoleEntity(Guid.NewGuid(), "Viewer", "viewer", "Read only"));

        // Act
        var result = (await repository.SearchAsync("admin", 1, 10)).ToList();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Administrator", result[0].DisplayName);
    }

    private static RoleRepositoryImpl CreateRepository(ServerDbContext dbContext)
    {
        return new RoleRepositoryImpl(dbContext);
    }
}
