using System.Data;
using System.Linq.Expressions;
using Moq;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Interfaces;

namespace ApiServerTests.ScopeService;

[TestClass]
public class EditScopeTests : TestBlueprint<Scope>
{
    IScopeService _scopeService;
    
    [TestMethod]
    public async Task TestMissingScopeName()
    {
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);
        var tenantId = Guid.NewGuid();
        var scopeDto = new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeValue = "scope:value"
        };
        await _scopeService.CreateScope(tenantId, scopeDto);
        
        var result = await _scopeService.EditScope(tenantId, "", scopeDto);
        
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(false, result.IsSuccess);
        Assert.AreEqual("ERR_MISSING_DATA", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestEmptyScopeDto()
    {
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepository, _objectMapper);
        var tenantId = Guid.NewGuid();
        var scopeDto = new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeDescription = "An example scope description.",
            ScopeValue = "scope:value"
        };
        await _scopeService.CreateScope(tenantId, scopeDto);

        scopeDto.ScopeName = null;
        scopeDto.ScopeValue = null;
        scopeDto.ScopeDescription = null;
        
        var result = await _scopeService.EditScope(tenantId, "TestScope", scopeDto);
        
        Assert.IsNotNull(result.Success);
        Assert.AreEqual(true, result.IsSuccess);
        Assert.AreEqual("TestScope", result.Success.ScopeName);
        Assert.AreEqual("An example scope description.", result.Success.ScopeDescription);
        Assert.AreEqual("scope:value", result.Success.ScopeValue);
    }

    [TestMethod]
    public async Task TestScopeNotFound()
    {
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepository, _objectMapper);
        var tenantId = Guid.NewGuid();
        var scopeDto = new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeDescription = "An example scope description.",
            ScopeValue = "scope:value"
        };

        scopeDto.ScopeName = null;
        scopeDto.ScopeValue = null;
        scopeDto.ScopeDescription = null;
        
        var result = await _scopeService.EditScope(tenantId, "TestScope", scopeDto);
        
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(true, result.IsFailed);
        Assert.AreEqual(ScopeErrors.ScopeNotFound, result.Error);
        Assert.AreEqual("ERR_SCOPE_NOT_FOUND", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestDbFail()
    {
        _resourceRepositoryMock.Setup(r => r.FirstOrDefault(It.IsAny<Expression<Func<Scope, bool>>>()))
            .Throws<DBConcurrencyException>();
        
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);
        var result = await _scopeService.EditScope(Guid.NewGuid(), "GenericScope", new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeValue = "scope:value",
            IsActive = true
        });
        
        Assert.AreEqual(ScopeErrors.ServerError, result.Error);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("INTERNAL_ERROR", result.ErrorData.ErrorString);
    }
}