# TrustedBits

## Overview
TrustedBits is a little Identity Platform (IdP) that provides a easy and yet powerful way to manage different identity  
domains across different applications with the most common protocols for SSO (Single Sign-On), it provides both a modern   
dashboard for administrators and an API that allows to both let the services interface with the IdP and eventually  
to build on top of this program applications to streamline operations like setups _et simila_.

It relies on **tenants**, so each application/identity domain it's isolated and has its own settings (protocols, registered  
applications/clients, ...) and RBAC (Role-Based Access Control) to manage authorization. In terms of authentication and  
authorization protocols, the following are supported:
- OIDC (OpenID Connect)
- OAuth 2.0


## Tech Stack
### Tools
- **Redis**: Session caching database
- **SQL Server**: Persistent storage for tenants, roles, users
- **ASP.NET (Minimal API)**: Framework that powers the API server
- **Blazor**: Framework that powers the web dashboard.

### Libraries
- **Newtonsoft.Json**: JSON Parsing library
- **EntityFramework**: Easier DB interaction
- **Identity Core**: Streamlined way to manage users.

## Project Structure
```
trustedbits/
│
├── api-server/
│   ├── Services
│   ├── Controllers
│   └── Models
│       ├── DTOs
│       └── Entities
└── dashboard/
```

**Services**: Contain all the business logic regardless of the caller endpoint controller.  
**Controllers**: Contain all the API controllers.  
**Models/Entities**: Contain classes that extends Identity Core entities to support tenancy and the tenant entity.  
**Models/DTOs**: Contain classes that define formats to request/response resources between application layers and 
external clients.