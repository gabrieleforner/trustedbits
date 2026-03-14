using System.Data;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Interfaces;

namespace ApiServerTests.RoleService;

[TestClass]
public class CreateRoleTests : TestBlueprint<Role>
{
    IRoleService _roleService;

    [TestMethod]
    public async Task TestEmptyRoleName()
    {
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepository, _objectMapper,  NullLogger.Instance);
        var tenantId = Guid.NewGuid();
        var roleDto = new RoleDto
        {
            RoleName = "",
            RoleDescription = "I am a test role"
        };
        
        var result = await _roleService.CreateRole(tenantId, roleDto);
         
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(RoleErrors.RoleInvalidData, result.Error);
        Assert.AreEqual("ERR_INVALID_ROLE_VALUE", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestBlankRoleName()
    {
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepository, _objectMapper,  NullLogger.Instance);
        var tenantId = Guid.NewGuid();
        var roleDto = new RoleDto
        {
            RoleName = "   ",
            RoleDescription = "I am a test role"
        };
        
        var result = await _roleService.CreateRole(tenantId, roleDto);
         
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(RoleErrors.RoleInvalidData, result.Error);
        Assert.AreEqual("ERR_INVALID_ROLE_VALUE", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestRoleAlreadyExists()
    {
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepository, _objectMapper,  NullLogger.Instance);
         var tenantId = Guid.NewGuid();
         var roleDto = new RoleDto
         {
             RoleName = "TestRole",
             RoleDescription = "I am a test role"
         };
         
         await _roleService.CreateRole(tenantId, roleDto);
         var result = await _roleService.CreateRole(tenantId, roleDto);
         
         Assert.IsNull(result.Success);
         Assert.IsNotNull(result.ErrorData);
         Assert.AreEqual(RoleErrors.RoleAlreadyExists, result.Error);
         Assert.AreEqual("ERR_ROLE_ALREADY_EXISTS", result.ErrorData.ErrorString);
    }

    [TestMethod]
    public async Task TestDbFail()
    {
        _resourceRepositoryMock
            .Setup(x => x.Get(It.IsAny<Expression<Func<Role, bool>>>()))
            .Throws<DBConcurrencyException>();
        
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepositoryMock.Object,
            _objectMapper,
            NullLogger.Instance);
        
        var result = await _roleService.CreateRole(Guid.NewGuid(), new RoleDto
        {
            RoleName = "TestRole",
            RoleDescription = "I am a test role"
        });
        
        Assert.AreEqual(RoleErrors.ServerError, result.Error);
        Assert.IsNotNull(result.ErrorData);
        Assert.IsNull(result.Success);
        Assert.AreEqual("INTERNAL_ERROR", result.ErrorData.ErrorString);
    }
}