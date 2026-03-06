using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Interfaces;

namespace ApiServerTests.ScopeService;

[TestClass]
public class GetAllScopesTests : TestBlueprint<Scope>
{
    IScopeService _scopeService;

    [TestMethod]
    public async Task TestPageSizeNegative()
    {
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);
        var result = await _scopeService.GetAllScopes(Guid.NewGuid(), 1, -1);
        
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("ERR_INVALID_PAGE_SIZE", result.ErrorData.ErrorString);
        Assert.AreEqual(ScopeErrors.ScopeInvalidData, result.Error);
    }
    
    [TestMethod]
    public async Task TestPageNumberNegative()
    {
        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);
        var result = await _scopeService.GetAllScopes(Guid.NewGuid(), -1, 100);
        
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("ERR_INVALID_PAGE_NUM", result.ErrorData.ErrorString);
        Assert.AreEqual(ScopeErrors.ScopeInvalidData, result.Error);
    }
    
    [TestMethod]
    public async Task TestDbFail()
    {
        _resourceRepositoryMock
            .Setup(x => x.Get(It.IsAny<Expression<Func<Scope, bool>>>()))
            .Throws<DbUpdateException>();

        _scopeService = new Trustedbits.ApiServer.Services.ScopeService(_resourceRepositoryMock.Object, _objectMapper);
        var result = await _scopeService.GetAllScopes(Guid.NewGuid(), 1, 100);
        
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("INTERNAL_ERROR", result.ErrorData.ErrorString);
        Assert.AreEqual(ScopeErrors.ServerError, result.Error);
    }
}