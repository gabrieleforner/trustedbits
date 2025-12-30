using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrustedbitsApiServer.Models;
using TrustedbitsApiServer.Services;

namespace ApiServerTests.InstanceSetupTests;

public class TestAdminRoleSetup
{
    private Mock<ILogger> _mockLogger;
    private ServerDbContext _dbContext;
    private Mock<UserManager<User>> _mockUserManager;
    private Mock<RoleManager<Role>> _mockRoleManager;
    private InstanceSetup _instanceSetup;
    private Guid _defaultTenantId;

    [SetUp]
    public void Setup()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ServerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ServerDbContext(options);

        // Setup mocks
        _mockLogger = new Mock<ILogger>();
        
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        var roleStore = new Mock<IRoleStore<Role>>();
        _mockRoleManager = new Mock<RoleManager<Role>>(
            roleStore.Object, null, null, null, null);

        // Create default tenant
        _defaultTenantId = Guid.NewGuid();
        var defaultTenant = new Tenant
        {
            Id = _defaultTenantId,
            Name = InstanceSetup.DefaultTenantName
        };
        _dbContext.Tenants.Add(defaultTenant);
        _dbContext.SaveChanges();

        // Setup RoleManager.Roles to return queryable collection
        var roles = new List<Role>().AsQueryable();
        _mockRoleManager.Setup(x => x.Roles).Returns(roles);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Database.EnsureDeleted();
        _dbContext?.Dispose();
    }
    
    [Test]
    public void TestTenantSetup_ShouldCreateRole()
    {
        // Arrange
        _mockRoleManager
            .Setup(x => x.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);

        _instanceSetup = new InstanceSetup(
            _mockLogger.Object,
            _dbContext,
            _mockUserManager.Object,
            _mockRoleManager.Object);

        // Act
        _instanceSetup.SetupDefaultTenant();

        // Assert
        _mockRoleManager.Verify(
            x => x.CreateAsync(It.Is<Role>(r => 
                r.Name == InstanceSetup.DefaultAdminRoleName && 
                r.TenantId == _defaultTenantId)),
            Times.Once,
            "Should create default admin role when it doesn't exist");
    }
    
    [Test]
    public void TestTenantSetup_ShouldNotCreateRole()
    {
        // Arrange
        var existingRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = InstanceSetup.DefaultAdminRoleName,
            TenantId = _defaultTenantId
        };

        var rolesWithExisting = new List<Role> { existingRole }.AsQueryable();
        _mockRoleManager.Setup(x => x.Roles).Returns(rolesWithExisting);

        _instanceSetup = new InstanceSetup(
            _mockLogger.Object,
            _dbContext,
            _mockUserManager.Object,
            _mockRoleManager.Object);

        // Act
        _instanceSetup.SetupDefaultTenant();

        // Assert
        _mockRoleManager.Verify(
            x => x.CreateAsync(It.IsAny<Role>()),
            Times.Never,
            "Should not create default admin role when it already exists");
    }

    [Test]
    public void TestTenantSetup_ShouldHandleTenantCreation()
    {
        // Arrange - Clear existing tenant
        _dbContext.Tenants.RemoveRange(_dbContext.Tenants);
        _dbContext.SaveChanges();

        _mockRoleManager
            .Setup(x => x.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);

        _instanceSetup = new InstanceSetup(
            _mockLogger.Object,
            _dbContext,
            _mockUserManager.Object,
            _mockRoleManager.Object);

        // Act
        _instanceSetup.SetupDefaultTenant();

        // Assert
        var createdTenant = _dbContext.Tenants
            .FirstOrDefault(t => t.Name == InstanceSetup.DefaultTenantName);
        
        Assert.That(createdTenant, Is.Not.Null, "Should create default tenant");
        Assert.That(createdTenant.Name, Is.EqualTo(InstanceSetup.DefaultTenantName));
    }
}