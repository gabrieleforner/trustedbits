namespace api_server.Services;

/// <summary>
/// Define all the methods that a valid caching system must expose
/// </summary>
/// <typeparam name="T">Type of the value (of the key-value pair)</typeparam>
public interface ICachingSystem<T>
{
    public Task<T> CreateValue(string key, T value, double ttlMilliseconds);
    public Task<T?> GetValue(string key);
    public Task<T?> UpdateValue(string key, T newValue);
    public Task<T?> DeleteValue(string key);
}