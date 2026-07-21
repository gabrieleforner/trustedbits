namespace Trustedbits.ApiServer.Infrastructure;

public class EnvLoader
{
    private const string DatabaseConnectionStringEnv = "DB_CONNECTION_URL";

    public static void Load(IConfiguration config)
    {
        var connectionString = Environment.GetEnvironmentVariable(DatabaseConnectionStringEnv);
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException($"You must set a valid DB connection string.", nameof(connectionString));
        config["ConnectionStrings:DbConnectionString"] = connectionString;
    }
}