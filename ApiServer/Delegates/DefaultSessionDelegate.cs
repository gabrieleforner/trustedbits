using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Trustedbits.ApiServer.Data.Repository;
using Trustedbits.ApiServer.Models.Entities;

namespace Trustedbits.ApiServer.Delegates;

public class DefaultSessionDelegate : ISessionDelegate
{
    private readonly IDistributedCache _cache;
    private readonly IRepository<TenantSettings> _tenantSettingsRepo;

    public DefaultSessionDelegate(IDistributedCache cache, IRepository<TenantSettings> tenantSettingsRepo)
    {
        _cache = cache;
        _tenantSettingsRepo = tenantSettingsRepo;
    }
    
    public async Task<string> CreateIdPSession(Guid tenantId, string email)
    {
        var tenantSettings = (await _tenantSettingsRepo
                .GetAsync(ts => ts.ParentTenantId == tenantId))
                .FirstOrDefault();
        if(tenantSettings == null)
            return string.Empty;

        var keyRandomBytes = new byte[tenantSettings.SaltBytesCount];
        RandomNumberGenerator.Fill(keyRandomBytes);
        
        var sessionIdKey = $"trustedbits:{tenantId}:idp_session:{email}";
        var sessionId = Convert.ToBase64String(keyRandomBytes);;

        var cachingOptions = new DistributedCacheEntryOptions();
        cachingOptions.AbsoluteExpirationRelativeToNow = tenantSettings.IdPSessionExpiry;
        await _cache.SetStringAsync(sessionIdKey, sessionId, cachingOptions);
        
        return sessionId;
    }

    public async Task<bool> VerifyIdPSessionExists(string sessionKey)
    {
        return await VerifySessionExists(sessionKey);
    }
    
    public async Task<bool> RevokeIdPSession(string sessionKey)
    {
        return await RevokeSession(sessionKey);
    }

    public async Task<string> CreateSession(Guid tenantId, string email)
    {
        var tenantSettings = (await _tenantSettingsRepo
                .GetAsync(ts => ts.ParentTenantId == tenantId))
            .FirstOrDefault();
        if(tenantSettings == null)
            return string.Empty;

        var keyRandomBytes = new byte[tenantSettings.SaltBytesCount];
        RandomNumberGenerator.Fill(keyRandomBytes);
        
        var sessionIdKey = $"trustedbits:{tenantId}:refresh_key:{email}";
        var sessionId = Convert.ToBase64String(keyRandomBytes);;

        var cachingOptions = new DistributedCacheEntryOptions();
        cachingOptions.AbsoluteExpirationRelativeToNow = tenantSettings.OidcSessionExpiry;
        await _cache.SetStringAsync(sessionIdKey, sessionId, cachingOptions);
        
        return sessionId;
    }

    public async Task<bool> VerifySessionExists(string sessionKey)
    {
        var sessionId = await _cache.GetStringAsync(sessionKey);
        if(sessionId.IsNullOrEmpty())
            return false;
        return true;

    }

    public async Task<bool> RevokeSession(string sessionKey)
    {
        if (await VerifySessionExists(sessionKey))
            return false;
        await _cache.RemoveAsync(sessionKey);
        return true;
    }
}