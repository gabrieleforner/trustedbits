using Moq;
using System.Data;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Interfaces;

namespace ApiServerTests.ScopeService;

[TestClass]
public class CreateScopeTests : TestBlueprint<Scope>
{
    [TestMethod]
    public async Task TestEmptyScopeValue()
    {
        var scopeService = new Trustedbits.ApiServer.Services.ScopeService(_scopeRepositoryMock.Object, _objectMapper);
        var result = await scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeValue = ""
        });
        
        Assert.AreEqual(ScopeErrors.ScopeInvalidData, result.Error);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("ERR_INVALID_SCOPE_VALUE", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestBlankScopeValue()
    {
        var scopeService = new Trustedbits.ApiServer.Services.ScopeService(_scopeRepositoryMock.Object, _objectMapper);
        var result = await scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeValue = "   "
        });
        
        Assert.AreEqual(ScopeErrors.ScopeInvalidData, result.Error);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("ERR_INVALID_SCOPE_VALUE", result.ErrorData.ErrorString);
    }
    
    
    [TestMethod]
    public async Task TestEmptyScopeName()
    {
        _scopeRepositoryMock.Setup(r => r.Create(It.IsAny<Scope>()))
            .ReturnsAsync((object src) => src as Scope);
        
        var scopeService = new Trustedbits.ApiServer.Services.ScopeService(_scopeRepositoryMock.Object, _objectMapper);
        var result = await scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
        {
            ScopeName = "",
            ScopeValue = "scope:value"
        });
        
        Assert.AreEqual(false, result.IsFailed);
        Assert.IsNull(result.ErrorData);
        Assert.AreEqual("scope:value", result.Success.ScopeName);
    }
    
    [TestMethod]
    public async Task TestBlankScopeName()
    {
        _scopeRepositoryMock.Setup(r => r.Create(It.IsAny<Scope>()))
            .ReturnsAsync((object src) => src as Scope);
        
        var scopeService = new Trustedbits.ApiServer.Services.ScopeService(_scopeRepositoryMock.Object, _objectMapper);
        var result = await scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
        {
            ScopeName = "    ",
            ScopeValue = "scope:value"
        });
        
        Assert.AreEqual(false, result.IsFailed);
        Assert.IsNull(result.ErrorData);
        Assert.AreEqual("scope:value", result.Success.ScopeName);
    }
    
    [TestMethod]
    public async Task TestDbFail()
    {

        _scopeRepositoryMock = new Mock<IRepository<Scope>>();
        _scopeRepositoryMock.Setup(r => r.Create(It.IsAny<Scope>()))
            .Throws<DBConcurrencyException>();
        
        var scopeService = new Trustedbits.ApiServer.Services.ScopeService(_scopeRepositoryMock.Object, _objectMapper);
        var result = await scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
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