using TrustedbitsApiServer.Models.Entities;

namespace TrustedbitsApiServer.Models;

/// <summary>
/// This class represents an application that may interface with the IdP, and it contains
/// data that is mainly required by the OIDC/OUAuth2 protocols.
/// </summary>
public class Application : ITenantAssociated
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }

    public string Name { get; set; }
    public string ClientId { get; set; }
    public string Scope { get; set; }
}