using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustedbits.ApiTests.TestSupport;

namespace Trustedbits.ApiTests.UnitTests.Mapping;

[TestClass]
public class AutoMapperProfileTests
{
    [TestMethod]
    public void ScopeMapProfile_EntityToDto_ShouldMapAllFields()
    {
        // Arrange
        var mapper = TestData.CreateMapper();
        var entity = TestData.ScopeEntity();

        // Act
        var dto = mapper.Map<Trustedbits.ApiServer.Core.Dto.ScopeDto>(entity);

        // Assert
        Assert.AreEqual(entity.Id, dto.ScopeId);
        Assert.AreEqual(entity.DisplayName, dto.ScopeName);
        Assert.AreEqual(entity.Value, dto.ScopeValue);
        Assert.AreEqual(entity.Description, dto.ScopeDescription);
    }

    [TestMethod]
    public void ScopeMapProfile_DtoToEntity_ShouldNormalizeNameAndValue()
    {
        // Arrange
        var mapper = TestData.CreateMapper();
        var dto = TestData.ScopeDto(name: "Read Users", value: "Users:Read");

        // Act
        var entity = mapper.Map<Trustedbits.ApiServer.Domain.Entity.ScopeEntity>(dto);

        // Assert
        Assert.AreEqual(dto.ScopeId, entity.Id);
        Assert.AreEqual("read users", entity.NormalizedName);
        Assert.AreEqual("users:read", entity.Value);
        Assert.AreEqual(dto.ScopeName, entity.DisplayName);
    }

    [TestMethod]
    public void ScopeMapProfile_NullDtoStrings_ShouldMapToEmptyNormalizedFields()
    {
        // Arrange
        var mapper = TestData.CreateMapper();
        var dto = TestData.ScopeDto(name: null, value: null, description: null);

        // Act
        var entity = mapper.Map<Trustedbits.ApiServer.Domain.Entity.ScopeEntity>(dto);

        // Assert
        Assert.AreEqual(string.Empty, entity.NormalizedName);
        Assert.AreEqual(string.Empty, entity.Value);
        Assert.IsNull(entity.DisplayName);
        Assert.IsNull(entity.Description);
    }

    [TestMethod]
    public void RoleMapProfile_EntityToDto_ShouldMapAllFields()
    {
        // Arrange
        var mapper = TestData.CreateMapper();
        var entity = TestData.RoleEntity();

        // Act
        var dto = mapper.Map<Trustedbits.ApiServer.Core.Dto.RoleDto>(entity);

        // Assert
        Assert.AreEqual(entity.Id, dto.RoleId);
        Assert.AreEqual(entity.DisplayName, dto.RoleName);
        Assert.AreEqual(entity.Description, dto.RoleDescription);
    }

    [TestMethod]
    public void RoleMapProfile_DtoToEntity_ShouldNormalizeName()
    {
        // Arrange
        var mapper = TestData.CreateMapper();
        var dto = new Trustedbits.ApiServer.Core.Dto.RoleDto
        {
            RoleId = TestData.RoleId,
            RoleName = "Administrator",
            RoleDescription = "Platform administrator"
        };

        // Act
        var entity = mapper.Map<Trustedbits.ApiServer.Domain.Entity.RoleEntity>(dto);

        // Assert
        Assert.AreEqual(dto.RoleId, entity.Id);
        Assert.AreEqual("administrator", entity.NormalizedName);
        Assert.AreEqual(dto.RoleName, entity.DisplayName);
        Assert.AreEqual(dto.RoleDescription, entity.Description);
    }
}
