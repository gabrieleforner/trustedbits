using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Interfaces;

namespace ApiServerTests.ScopeService;

[TestClass]
public class DeleteScopeTests : TestBlueprint<Scope>
{
    IScopeService _scopeService;

    [TestMethod]
    public async Task TestEmptyScopeName()
    {
        var tenantId = Guid.NewGuid();
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);
        var result = await _scopeService.DeleteScope(tenantId, "");
        
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(false, result.IsSuccess);
        Assert.AreEqual(ScopeErrors.ScopeInvalidData, result.Error);
        Assert.AreEqual("ERR_MISSING_DATA", result.ErrorData.ErrorString);
    }

    [TestMethod]
    public async Task TestScopeNotFound()
    {
        var tenantId = Guid.NewGuid();
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);

        await _scopeService.CreateScope(tenantId, new ScopeDto
        {
            ScopeName = "ExampleScope",
            ScopeDescription = "Example Scope Description",
            ScopeValue =  "scope:value"
        });
        
        var result = await _scopeService.DeleteScope(tenantId, "TestScope");
        
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(false, result.IsSuccess);
        Assert.AreEqual(ScopeErrors.ScopeNotFound, result.Error);
        Assert.AreEqual("ERR_SCOPE_NOT_FOUND", result.ErrorData.ErrorString);
    }

    
    [TestMethod]
    public async Task TestDbFail()
    {
        _resourceRepositoryMock
            .Setup(i => i.FirstOrDefault(It.IsAny<Expression<Func<Scope, bool>>>()))
            .Throws<DbUpdateException>();
        
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);
        var result = await _scopeService.DeleteScope(Guid.NewGuid(), "ExampleScope");
        
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(false, result.IsSuccess);
        Assert.AreEqual(ScopeErrors.ServerError, result.Error);
        Assert.AreEqual("INTERNAL_ERROR", result.ErrorData.ErrorString);
    }
}