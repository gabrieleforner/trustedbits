using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Trustedbits.ApiServer.Domain.Repository.impl;
using Trustedbits.ApiServer.Infrastructure.EFCore;
using Trustedbits.ApiTests.TestSupport;

namespace Trustedbits.ApiTests.IntegrationTests.Repositories;

[TestClass]
public class ScopeRepositoryIntegrationTests
{
    [TestMethod]
    public async Task CreateAsync_ValidScope_ShouldPersistScope()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        var scope = TestData.ScopeEntity();

        // Act
        var created = await repository.CreateAsync(scope);

        // Assert
        Assert.AreEqual(scope.Id, created.Id);
        Assert.AreEqual(1, await dbContext.Scopes.CountAsync());
    }

    [TestMethod]
    public async Task GetByIdAsync_UntrackedByDefault_ShouldReturnDetachedScope()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity());
        dbContext.ChangeTracker.Clear();

        // Act
        var result = await repository.GetByIdAsync(TestData.ScopeId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, dbContext.ChangeTracker.Entries().Count());
    }

    [TestMethod]
    public async Task GetByIdAsync_TrackedRequested_ShouldReturnTrackedScope()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity());
        dbContext.ChangeTracker.Clear();

        // Act
        var result = await repository.GetByIdAsync(TestData.ScopeId, isTracked: true);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, dbContext.ChangeTracker.Entries().Count());
    }

    [TestMethod]
    public async Task GetByNameAsync_MixedCaseName_ShouldNormalizeAndReturnScope()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity());

        // Act
        var result = await repository.GetByNameAsync("READ USERS");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestData.ScopeId, result.Id);
    }

    [TestMethod]
    public async Task GetByValueAsync_MixedCaseValue_ShouldNormalizeAndReturnScope()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity());

        // Act
        var result = await repository.GetByValueAsync("USERS:READ");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(TestData.ScopeId, result.Id);
    }

    [TestMethod]
    public async Task GetAllAsync_SecondPage_ShouldReturnRequestedPage()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity(Guid.NewGuid(), "One", "one", "one"));
        await repository.CreateAsync(TestData.ScopeEntity(Guid.NewGuid(), "Two", "two", "two"));
        await repository.CreateAsync(TestData.ScopeEntity(Guid.NewGuid(), "Three", "three", "three"));

        // Act
        var result = (await repository.GetAllAsync(2, 1)).ToList();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Two", result[0].DisplayName);
    }

    [TestMethod]
    public async Task SearchAsync_TermMatchesAnyFieldCaseInsensitive_ShouldReturnMatches()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity(Guid.NewGuid(), "Read Users", "read users", "users:read", "People records"));
        await repository.CreateAsync(TestData.ScopeEntity(Guid.NewGuid(), "Read Roles", "read roles", "roles:read", "Role records"));

        // Act
        var result = (await repository.SearchAsync("PEOPLE", 1, 10)).ToList();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Read Users", result[0].DisplayName);
    }

    [TestMethod]
    public async Task UpdateAsync_DetachedScope_ShouldPersistChanges()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity());
        dbContext.ChangeTracker.Clear();
        var detached = TestData.ScopeEntity(description: "Updated");

        // Act
        await repository.UpdateAsync(detached);

        // Assert
        Assert.AreEqual("Updated", dbContext.Scopes.AsNoTracking().Single().Description);
    }

    [TestMethod]
    public async Task UpdateAsync_TrackedModifiedScope_ShouldSaveChanges()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity());
        dbContext.ChangeTracker.Clear();
        var tracked = await repository.GetByIdAsync(TestData.ScopeId, isTracked: true);
        tracked!.Description = "Tracked update";

        // Act
        await repository.UpdateAsync(tracked);

        // Assert
        Assert.AreEqual("Tracked update", dbContext.Scopes.AsNoTracking().Single().Description);
    }

    [TestMethod]
    public async Task DeleteAsync_ExistingScope_ShouldRemoveScope()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        var created = await repository.CreateAsync(TestData.ScopeEntity());
        dbContext.ChangeTracker.Clear();

        // Act
        await repository.DeleteAsync(created);

        // Assert
        Assert.AreEqual(0, await dbContext.Scopes.CountAsync());
    }

    [TestMethod]
    public async Task SaveChanges_ModifiedTrackedScope_ShouldPersistChange()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var repository = CreateRepository(dbContext);
        await repository.CreateAsync(TestData.ScopeEntity());
        var tracked = dbContext.Scopes.Single();
        tracked.Description = "Saved";

        // Act
        await repository.SaveChanges();

        // Assert
        Assert.AreEqual("Saved", dbContext.Scopes.AsNoTracking().Single().Description);
    }

    private static ScopeRepositoryImpl CreateRepository(ServerDbContext dbContext)
    {
        return new ScopeRepositoryImpl(dbContext);
    }
}
