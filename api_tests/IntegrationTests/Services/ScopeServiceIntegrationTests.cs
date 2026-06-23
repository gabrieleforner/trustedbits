using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiServer.Core.Patterns;
using Trustedbits.ApiTests.TestSupport;

namespace Trustedbits.ApiTests.IntegrationTests.Services;

[TestClass]
public class ScopeServiceIntegrationTests
{
    [TestMethod]
    public async Task Create_ValidScope_ShouldPersistScopeAndReturnDto()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var service = TestData.CreateScopeService(dbContext);
        var dto = TestData.ScopeDto(id: Guid.NewGuid(), name: "Read Users", value: "Users:Read");

        // Act
        var result = await service.Create(dto);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreNotEqual(Guid.Empty, result.Data!.ScopeId);
        Assert.AreEqual("Read Users", result.Data.ScopeName);
        Assert.AreEqual("users:read", result.Data.ScopeValue);
        Assert.AreEqual(1, dbContext.Scopes.Count());
    }

    [TestMethod]
    public async Task Create_DuplicateName_ShouldReturnConflictAndNotPersistDuplicate()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var service = TestData.CreateScopeService(dbContext);
        await service.Create(TestData.ScopeDto(name: "Read Users", value: "users:read"));

        // Act
        var result = await service.Create(TestData.ScopeDto(id: Guid.NewGuid(), name: "READ USERS", value: "users:write"));

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual(ErrorType.Conflict, result.ErrorType);
        Assert.AreEqual(1, dbContext.Scopes.Count());
    }

    [TestMethod]
    public async Task Get_ExistingScope_ShouldReturnPersistedDto()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var service = TestData.CreateScopeService(dbContext);
        var created = await service.Create(TestData.ScopeDto(name: "Read Users", value: "users:read"));

        // Act
        var result = await service.Get(created.Data!.ScopeId!.Value);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreEqual("Read Users", result.Data!.ScopeName);
    }

    [TestMethod]
    public async Task Get_ValidPaging_ShouldReturnPersistedScopes()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var service = TestData.CreateScopeService(dbContext);
        await service.Create(TestData.ScopeDto(name: "Read Users", value: "users:read"));
        await service.Create(TestData.ScopeDto(id: Guid.NewGuid(), name: "Write Users", value: "users:write"));

        // Act
        var result = await service.Get(1, 10);
        var items = await TestData.ToListAsync(result.Data!);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreEqual(2, items.Count);
    }

    [TestMethod]
    public async Task Search_TermMatchesDescription_ShouldReturnMatchingScopes()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var service = TestData.CreateScopeService(dbContext);
        await service.Create(TestData.ScopeDto(name: "Read Users", value: "users:read", description: "people records"));
        await service.Create(TestData.ScopeDto(id: Guid.NewGuid(), name: "Read Roles", value: "roles:read", description: "Role records"));

        // Act
        var result = await service.Search("people", 1, 10);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreEqual(1, result.Data!.Count());
        Assert.AreEqual("Read Users", result.Data!.Single().ScopeName);
    }

    [TestMethod]
    public async Task Update_ExistingScope_ShouldPersistChanges()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var service = TestData.CreateScopeService(dbContext);
        var created = await service.Create(TestData.ScopeDto(name: "Read Users", value: "users:read"));
        var update = new ScopeDto { ScopeDescription = "Updated description" };
        dbContext.ChangeTracker.Clear();

        // Act
        var result = await service.Update(created.Data!.ScopeId!.Value, update);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.AreEqual("Updated description", result.Data!.ScopeDescription);
        Assert.AreEqual("Updated description", dbContext.Scopes.Single().Description);
    }

    [TestMethod]
    public async Task Delete_ExistingScope_ShouldRemoveScope()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var service = TestData.CreateScopeService(dbContext);
        var created = await service.Create(TestData.ScopeDto(name: "Read Users", value: "users:read"));
        dbContext.ChangeTracker.Clear();

        // Act
        var result = await service.Delete(created.Data!.ScopeId!.Value);

        // Assert
        Assert.IsFalse(result.IsFailed);
        Assert.IsTrue(result.Data);
        Assert.AreEqual(0, dbContext.Scopes.Count());
    }
}
