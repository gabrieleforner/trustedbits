using Moq;
using System.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Interfaces;

namespace ApiServerTests.ScopeService;

[TestClass]
public class CreateScopeTests : TestBlueprint<Scope>
{
    IScopeService _scopeService;
    
    [TestMethod]
    public async Task TestEmptyScopeValue()
    {
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper, NullLogger.Instance);
        var result = await _scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
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
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper, NullLogger.Instance);
        var result = await _scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeValue = "   "
        });
        
        Assert.AreEqual(ScopeErrors.ScopeInvalidData, result.Error);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("ERR_INVALID_SCOPE_VALUE", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestRegexScopeValue()
    {
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper, NullLogger.Instance);
        var result = await _scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeValue = "resource.verb"
        });
        
        Assert.AreEqual(ScopeErrors.ScopeInvalidData, result.Error);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("ERR_INVALID_SCOPE_VALUE", result.ErrorData.ErrorString);
    }

    [TestMethod]
    public async Task TestScopeNameDuplicated()
    {
        var tenantId = Guid.NewGuid();
        var creationDto = new ScopeDto
        {
            ScopeName = "TestScope",
            ScopeValue = "scope:value",
        };
        
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepository, _objectMapper, NullLogger.Instance);
        await _scopeService.CreateScope(tenantId, creationDto);
        
        var result = await _scopeService.CreateScope(tenantId, creationDto);
        
        Assert.AreEqual(false, result.IsSuccess);
        Assert.IsNotNull(result.ErrorData);
        Assert.IsNull(result.Success);
        Assert.AreEqual(ScopeErrors.ScopeAlreadyExists, result.Error);
        Assert.AreEqual("SCOPE_ALREADY_EXISTS", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestEmptyScopeName()
    {
        _resourceRepositoryMock.Setup(r => r.Create(It.IsAny<Scope>()))
            .ReturnsAsync((object src) => src as Scope);
        
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper, NullLogger.Instance);
        var result = await _scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
        {
            ScopeName = "",
            ScopeValue = "scope:value"
        });
        
        Assert.AreEqual(false, result.IsFailed);
        Assert.IsNull(result.ErrorData);
        Assert.IsNotNull(result.Success);
        Assert.AreEqual("scope:value", result.Success.ScopeName);
    }
    
    [TestMethod]
    public async Task TestBlankScopeName()
    {
        _resourceRepositoryMock.Setup(r => r.Create(It.IsAny<Scope>()))
            .ReturnsAsync((object src) => src as Scope);
        
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper, NullLogger.Instance);
        var result = await _scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
        {
            ScopeName = "    ",
            ScopeValue = "scope:value"
        });
        
        Assert.AreEqual(false, result.IsFailed);
        Assert.IsNull(result.ErrorData);
        Assert.IsNotNull(result.Success);
        Assert.AreEqual("scope:value", result.Success.ScopeName);
    }
    
    [TestMethod]
    public async Task TestDbFail()
    {
        _resourceRepositoryMock = new Mock<IRepository<Scope>>();
        _resourceRepositoryMock.Setup(r => r.Create(It.IsAny<Scope>()))
            .Throws<DBConcurrencyException>();
        
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper, NullLogger.Instance);
        var result = await _scopeService.CreateScope(Guid.NewGuid(), new ScopeDto
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