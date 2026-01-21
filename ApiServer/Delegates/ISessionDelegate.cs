namespace Trustedbits.ApiServer.Delegates;

/// <summary>
/// This struct defines what methods must be exposed by a class
/// that provides session management features
/// </summary>
public interface ISessionDelegate
{
    Task<string> CreateIdPSession(Guid tenantId, string email);
    Task<bool> VerifyIdPSessionExists(string sessionKey);
    Task<bool> RevokeIdPSession(string sessionKey);
    
    Task<string> CreateSession(Guid tenantId, string email);
    Task<bool> VerifySessionExists(string sessionKey);
    Task<bool> RevokeSession(string sessionKey);
}