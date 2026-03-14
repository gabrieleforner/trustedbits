using System.Data;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Trustedbits.ApiServer.Models.DTOs;
using Trustedbits.ApiServer.Models.Entities;
using Trustedbits.ApiServer.Services.Interfaces;

namespace ApiServerTests.RoleService;

[TestClass]
public class EditRoleTests : TestBlueprint<Role>
{
    IRoleService _roleService;

    [TestMethod]
    public async Task TestDbFail()
    {
        _resourceRepositoryMock
            .Setup(x => x.GetTracked(It.IsAny<Expression<Func<Role, bool>>>()))
            .Throws<DBConcurrencyException>();
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepositoryMock.Object,
            _objectMapper,
            NullLogger.Instance);
        
        var result = await _roleService.EditRole(Guid.Empty, "GenericRole", new RoleDto
        {
            RoleName = "TestRoleName",
            RoleDescription = "Role Description",
        });

        
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(RoleErrors.ServerError, result.Error);
        Assert.AreEqual("INTERNAL_ERROR", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestRoleNotExist()
    {
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepository, _objectMapper, NullLogger.Instance);
        var result = await _roleService.EditRole(Guid.NewGuid(), "RoleNameHere", new RoleDto
        {
            RoleName = "TestRoleName",
            RoleDescription = "Role Description",
        });
        
        
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(RoleErrors.RoleNotFound, result.Error);
        Assert.AreEqual("ERR_ROLE_NOT_FOUND", result.ErrorData.ErrorString);
    }
    
    [TestMethod]
    public async Task TestNewRoleNameAlreadyExists()
    {
        var tenantId = Guid.NewGuid();
        var commonRoleName = "CommonRoleName";
        var roleName = "HelloWorldRole";

        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepository, _objectMapper, NullLogger.Instance);
        
        // Role that will be edited
        await _roleService.CreateRole(tenantId, new RoleDto { RoleName = roleName});
        // Role with the same name as the one provided in the editing DTO
        await _roleService.CreateRole(tenantId, new RoleDto { RoleName = commonRoleName });

        var result = await _roleService.EditRole(tenantId, roleName, new RoleDto { RoleName = commonRoleName });
        
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(RoleErrors.RoleAlreadyExists, result.Error);
        Assert.AreEqual("ERR_ROLE_NAME_ALREADY_EXISTS", result.ErrorData.ErrorString);
    }

    [TestMethod]
    public async Task TestNewRoleNameEmpty()
    {
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepository, _objectMapper, NullLogger.Instance);
        var tenantId = Guid.NewGuid();
        
        await _roleService.CreateRole(tenantId, new RoleDto { RoleName = "TestRoleName" });
        var result = await _roleService.EditRole(tenantId, "TestRoleName", new RoleDto
        {
            RoleName = "",
            RoleDescription = "Role Description",
        });
        
        
        Assert.IsNotNull(result.Success);
        Assert.IsNull(result.ErrorData);
        Assert.AreEqual("TestRoleName", result.Success.RoleName);
        Assert.AreEqual("Role Description", result.Success.RoleDescription);
    }
    
    [TestMethod]
    public async Task TestNewRoleDescEmpty()
    {
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepository, _objectMapper, NullLogger.Instance);
        var tenantId = Guid.NewGuid();
        
        await _roleService.CreateRole(tenantId, new RoleDto {
            RoleName = "TestRoleName",
            RoleDescription =  "Role Description" 
        });
        
        var result = await _roleService.EditRole(tenantId, "TestRoleName", new RoleDto
        {
            RoleName = "AnotherRoleName",
            RoleDescription = "",
        });
        
        Assert.IsNotNull(result.Success);
        Assert.IsNull(result.ErrorData);
        Assert.AreEqual("AnotherRoleName", result.Success.RoleName);
        Assert.AreEqual("Role Description", result.Success.RoleDescription);
    }
    
    [TestMethod]
    public async Task TestRoleNameEmpty()
    {
        _roleService = new Trustedbits.ApiServer.Services.RoleService(_resourceRepository, _objectMapper, NullLogger.Instance);
        var result = await _roleService.EditRole(Guid.NewGuid(), "", new RoleDto
        {
            RoleName = "TestRoleName",
            RoleDescription = "Role Description",
        });
        
        Assert.IsNull(result.Success);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual(RoleErrors.RoleInvalidData, result.Error);
        Assert.AreEqual("ERR_INVALID_ROLE_NAME", result.ErrorData.ErrorString);
    }
}