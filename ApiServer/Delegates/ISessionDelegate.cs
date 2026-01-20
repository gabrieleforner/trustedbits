using Trustedbits.ApiServer.Models.Entities;

namespace Trustedbits.ApiServer.Delegates;

/// <summary>
/// This struct defines what methods must be exposed by a class
/// that provides session management features
/// </summary>
public interface ISessionDelegate
{
    Task<string> InitializeIdPSession<TLoginRequestDto>(TLoginRequestDto loginDto, TenantSettings tenantSettings);
}