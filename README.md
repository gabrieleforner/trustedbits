# Trustedbits

## Overview
Trustedbits is a tiny and compact identity provider project that i decided to start  
in order to have something that i can use for managing identities and that i can spin  
up easily and quickly.

## Model
Trustedbits relies on a multitenacy model, where each resource has **always** a tenant  
UUID value to identfy to which tenant it belongs to.
```
Tenant
├── Users
│   └── Roles
│       └──Scopes
```
The API endpoints (except for login/signup endpoints) they generally contain the tenant  
ID in the route, following an example:
```http request
http://instance.trustedbits.io/d60a1c04-b587-4eeb-b2dc-539a60d2fde9/saml2/sso
```
Where, as you can imagine, `d60a1c04-b587-4eeb-b2dc-539a60d2fde9` it's the tenant ID.

## Tech stack
-  **ASP.NET Core:** Foudation framework the API server relies on
- **Entity Framework Core**: Used as ORM to manage entities with the SQL database
- **AutoMapper**: Automates DTO to Entity/Entity to DTO conversions

