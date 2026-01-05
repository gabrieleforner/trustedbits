using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrustedbitsApiServer.Models;
using TrustedbitsApiServer.Models.Entities;
using TrustedbitsApiServer.Services;

namespace ApiServerTests.InstanceSetupTests;

public class TestDefaultTenantSetup
{
    private Mock<ILogger> _mockLogger;
    private ServerDbContext _dbContext;
    private Mock<UserManager<User>> _mockUserManager;
    private Mock<RoleManager<Role>> _mockRoleManager;
    private InstanceSetup _instanceSetup;

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

        var roles = new List<Role>().AsQueryable();
        _mockRoleManager.Setup(x => x.Roles).Returns(roles);
        _mockRoleManager
            .Setup(x => x.CreateAsync(It.IsAny<Role>()))
            .ReturnsAsync(IdentityResult.Success);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Database.EnsureDeleted();
        _dbContext?.Dispose();
    }
    
    [Test]
    public void TestTenantSetup_ShouldCreateTenant()
    {
        // Arrange
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
        Assert.That(createdTenant.Id, Is.Not.EqualTo(Guid.Empty));
    }
    
    [Test]
    public void TestTenantSetup_ShouldNotCreateTenant()
    {
        // Arrange
        var existingTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = InstanceSetup.DefaultTenantName
        };
        _dbContext.Tenants.Add(existingTenant);
        _dbContext.SaveChanges();

        _instanceSetup = new InstanceSetup(
            _mockLogger.Object,
            _dbContext,
            _mockUserManager.Object,
            _mockRoleManager.Object);

        // Act
        _instanceSetup.SetupDefaultTenant();

        // Assert
        var tenantCount = _dbContext.Tenants
            .Count(t => t.Name == InstanceSetup.DefaultTenantName);
        
        Assert.That(tenantCount, Is.EqualTo(1), "Should not create duplicate tenant");
        
        var tenant = _dbContext.Tenants
            .First(t => t.Name == InstanceSetup.DefaultTenantName);
        Assert.That(tenant.Id, Is.EqualTo(existingTenant.Id), "Should use existing tenant");
    }
}