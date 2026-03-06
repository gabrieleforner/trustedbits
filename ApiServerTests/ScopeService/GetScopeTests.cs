using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Interfaces;

namespace ApiServerTests.ScopeService;

[TestClass]
public class GetScopeTests : TestBlueprint<Scope>
{
    IScopeService _scopeService;

    [TestMethod]
    public async Task TestWrongTenantId() 
    {
        var tenantId = Guid.NewGuid();
        var wrongTenantId = Guid.NewGuid();
        
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepository, _objectMapper);
        await _scopeService.CreateScope(tenantId, new ScopeDto
        {
            ScopeName = "Hello",
            ScopeDescription = "World",
            ScopeValue = "hello:world",
            IsActive = true,
        });

        var result = await _scopeService.GetScope(wrongTenantId, new ScopeQueryDto
        {
            ContainsInValue = "hello"
        });
        
        Assert.IsNull(result.ErrorData);
        Assert.IsNotNull(result.Success);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Success.Count);
    }
    
    [TestMethod]
    public async Task TestDbFail()
    {
        _resourceRepositoryMock
            .Setup(i => i.Get(It.IsAny<Expression<Func<Scope, bool>>>()))
            .Throws<DbUpdateException>();
        
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);
        var result = await _scopeService.GetScope(Guid.NewGuid(), new ScopeQueryDto
        {
            ContainsInValue = "Hello"
        });
        
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(false, result.IsSuccess);
        Assert.AreEqual(ScopeErrors.ServerError, result.Error);
        Assert.AreEqual("INTERNAL_ERROR", result.ErrorData.ErrorString);
    }
}