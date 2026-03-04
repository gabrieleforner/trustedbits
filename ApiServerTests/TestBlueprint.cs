using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Trustedbits.ApiServer.Data.AutomapperProfiles;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models.Entities;

namespace ApiServerTests;

public class TestBlueprint<TResource>
{
    protected Mock<IRepository<Scope>>? _scopeRepositoryMock;
    protected IMapper? _objectMapper;
    
    [TestInitialize]
    public void TestInitialize() 
    {
        _scopeRepositoryMock = new Mock<IRepository<Scope>>();
        var loggerFactory = LoggerFactory.Create(builder => { });
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ScopeMappings>();
        }, loggerFactory);
        
        _objectMapper = config.CreateMapper();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _objectMapper = null;        
    }
}