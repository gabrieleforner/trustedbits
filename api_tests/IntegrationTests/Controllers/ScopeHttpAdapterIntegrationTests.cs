using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustedbits.ApiServer.Adapters.Http;
using Trustedbits.ApiServer.Core.Dto;
using Trustedbits.ApiTests.TestSupport;

namespace Trustedbits.ApiTests.IntegrationTests.Controllers;

[TestClass]
public class ScopeHttpAdapterIntegrationTests
{
    [TestMethod]
    public async Task CreateGetUpdateDelete_ControllerServiceRepositoryFlow_ShouldCompleteSuccessfully()
    {
        // Arrange
        var databaseName = Guid.NewGuid().ToString();
        await using var createContext = TestData.CreateDbContext(databaseName);
        var controller = new ScopeHttpAdapter(TestData.CreateScopeService(createContext));
        var createDto = TestData.ScopeDto(name: "Read Users", value: "users:read");

        // Act
        var createResult = await controller.CreateAsync(createDto);
        var created = (ScopeDto)((CreatedAtActionResult)createResult).Value!;

        await using var getContext = TestData.CreateDbContext(databaseName);
        controller = new ScopeHttpAdapter(TestData.CreateScopeService(getContext));
        var getResult = await controller.GetById(created.ScopeId!.Value);

        await using var updateContext = TestData.CreateDbContext(databaseName);
        controller = new ScopeHttpAdapter(TestData.CreateScopeService(updateContext));
        var updateResult = await controller.UpdateAsync(created.ScopeId.Value, new ScopeDto { ScopeDescription = "Updated" });

        await using var deleteContext = TestData.CreateDbContext(databaseName);
        controller = new ScopeHttpAdapter(TestData.CreateScopeService(deleteContext));
        var deleteResult = await controller.DeleteAsync(created.ScopeId.Value);

        await using var afterDeleteContext = TestData.CreateDbContext(databaseName);
        controller = new ScopeHttpAdapter(TestData.CreateScopeService(afterDeleteContext));
        var getAfterDeleteResult = await controller.GetById(created.ScopeId.Value);

        // Assert
        Assert.IsInstanceOfType<CreatedAtActionResult>(createResult);
        Assert.IsInstanceOfType<OkObjectResult>(getResult);
        Assert.AreEqual("Updated", ((ScopeDto)((OkObjectResult)updateResult).Value!).ScopeDescription);
        Assert.AreEqual(true, ((OkObjectResult)deleteResult).Value);
        Assert.IsInstanceOfType<NotFoundObjectResult>(getAfterDeleteResult);
    }

    [TestMethod]
    public async Task GetAll_ControllerServiceRepositoryFlow_ShouldReturnPersistedScopes()
    {
        // Arrange
        await using var dbContext = TestData.CreateDbContext();
        var controller = new ScopeHttpAdapter(TestData.CreateScopeService(dbContext));
        await controller.CreateAsync(TestData.ScopeDto(name: "Read Users", value: "users:read"));
        await controller.CreateAsync(TestData.ScopeDto(id: Guid.NewGuid(), name: "Write Users", value: "users:write"));

        // Act
        var result = await controller.GetAll(1, 10);
        var data = (IAsyncEnumerable<ScopeDto>)((OkObjectResult)result).Value!;
        var items = await TestData.ToListAsync(data);

        // Assert
        Assert.AreEqual(2, items.Count);
    }
}
