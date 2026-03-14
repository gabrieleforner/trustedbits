using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Trustedbits.ApiServer.Data;
using Trustedbits.ApiServer.Data.AutomapperProfiles;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models.Entities;

namespace ApiServerTests;

public class TestBlueprint<TResource> where TResource : class
{
    protected Mock<IRepository<TResource>> _resourceRepositoryMock;
    protected IRepository<TResource> _resourceRepository;
    protected IMapper _objectMapper;
    
    [TestInitialize]
    public void TestInitialize() 
    {
        DbContextOptions options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _resourceRepository = new EFCoreRepository<TResource>(new AppDbContext(options));
        
        _resourceRepositoryMock = new Mock<IRepository<TResource>>();
        var loggerFactory = LoggerFactory.Create(builder => { });
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ScopeMappings>();
            cfg.AddProfile<RoleMappings>();
        }, loggerFactory);
        
        _objectMapper = config.CreateMapper();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _objectMapper = null;    
        _resourceRepositoryMock = null;
        _resourceRepository = null;
    }
}