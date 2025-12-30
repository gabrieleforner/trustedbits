using Microsoft.Extensions.Configuration;
using TrustedbitsApiServer;
using Moq;

namespace ApiServerTests;

public class TestConnectionStringSetup
{
    Mock<IConfiguration> configMock;
    
    [SetUp]
    public void Setup()
    {
        configMock = new Mock<IConfiguration>();
    }

    [TearDown]
    public void TearDown()
    {
        configMock.Reset();
    }

    
    /// <summary>
    /// Test the behavior of the method when the server hostname
    /// is null/empty
    /// </summary>
    [Test]
    public void TestNetworkSettingsValidation_InvalidHostname()
    {
        configMock
            .Setup(c => c["SQL_SERVER_HOST"])
            .Returns("");
        Assert.Throws<ArgumentException>(() => Program.GetConnectionString(configMock.Object));
    }
    
    /// <summary>
    /// Test the behavior of the method when the server port
    /// is null/empty
    /// </summary>
    [Test]
    public void TestNetworkSettingsValidation_InvalidPort()
    {
        // Test with empty port
        configMock
            .Setup(c => c["SQL_SERVER_HOST"])
            .Returns("generic.sql.instance");
        configMock
            .Setup(c => c["SQL_SERVER_PORT"])
            .Returns("");
        Assert.Throws<ArgumentException>(() => Program.GetConnectionString(configMock.Object));
        
        
        // Test with alphanumeric port
        configMock
            .Setup(c => c["SQL_SERVER_PORT"])
            .Returns("1433rh");
        Assert.Throws<ArgumentException>(() => Program.GetConnectionString(configMock.Object));

        // Test with negative port
        configMock
            .Setup(c => c["SQL_SERVER_PORT"])
            .Returns("-1");
        Assert.Throws<ArgumentException>(() => Program.GetConnectionString(configMock.Object));
        
        // Test with overflowing port
        configMock
            .Setup(c => c["SQL_SERVER_PORT"])
            .Returns("65537");
        Assert.Throws<ArgumentException>(() => Program.GetConnectionString(configMock.Object));
    }

    /// <summary>
    /// Test the behavior of the method when the service account username
    /// is null/empty
    /// </summary>
    [Test]
    public void TestServiceAccountSettingsValidation_InvalidUsername()
    {
        configMock
            .Setup(c => c["SQL_USERNAME"])
            .Returns("");
        Assert.Throws<ArgumentException>(() => Program.GetConnectionString(configMock.Object));
    }
    
    /// <summary>
    /// Test the behavior of the method when the service account password
    /// is null/empty
    /// </summary>
    [Test]
    public void TestServiceAccountSettingsValidation_InvalidPassword()
    {
        configMock
            .Setup(c => c["SQL_USERNAME"])
            .Returns("mockuser");
        configMock
            .Setup(c => c["SQL_PASSWORD"])
            .Returns("");
        Assert.Throws<ArgumentException>(() => Program.GetConnectionString(configMock.Object));
    }
    
    [Test]
    public void TestSettingsValidation_AllValid()
    {
        configMock
            .Setup(c => c["SQL_USER"])
            .Returns("mockuser");
        
        configMock
            .Setup(c => c["SQL_PASSWORD"])
            .Returns("mockpassword");
        
        configMock
            .Setup(c => c["SQL_SERVER_HOST"])
            .Returns("generic.sql.instance");
        configMock
            .Setup(c => c["SQL_SERVER_PORT"])
            .Returns("1433");
        Assert.DoesNotThrow(() => Program.GetConnectionString(configMock.Object));
    }
}