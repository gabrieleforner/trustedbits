using System.Security.Claims;

namespace TrustedbitsApiServer.Models.Entities;

/// <summary>
/// Contain all the settings of a tenant.
/// </summary>
public class TenantSettings : ITenantAssociated
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }

    // Session & token lifetimes
    public TimeSpan IdPSessionDuration { get; set; }
    public TimeSpan OidcIdTokenDuration { get; set; }
    public TimeSpan OidcAccessTokenDuration { get; set; }
    public TimeSpan OidcRefreshTokenDuration { get; set; }

    // OIDC core
    public string Issuer { get; set; }
    public string Audience { get; set; }

    // Crypto
    public string IdTokenSigningAlg { get; set; } = "RS256";
    public string AccessTokenSigningAlg { get; set; } = "RS256";
    public string SigningKeyId { get; set; }

    // Scopes & claims
    public ICollection<string> AllowedScopes { get; set; }
}
