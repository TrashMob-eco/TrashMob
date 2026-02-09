# Migrating from Azure AD B2C to Microsoft Entra External ID

## A Guide for Privo.com Developers and Partners

**Version:** 1.0
**Date:** February 8, 2026
**Author:** TrashMob.eco Engineering Team
**Audience:** Privo.com developers, organizations migrating from B2C, Entra External ID implementers

---

## Table of Contents

1. [Introduction](#introduction)
2. [Why Migrate?](#why-migrate)
3. [Architecture Overview: Before & After](#architecture-overview-before--after)
4. [Current Azure AD B2C Architecture](#current-azure-ad-b2c-architecture)
5. [Target Entra External ID Architecture](#target-entra-external-id-architecture)
6. [Migration Phases](#migration-phases)
7. [Privo.com Integration via Custom Authentication Extensions](#privocom-integration-via-custom-authentication-extensions)
8. [Lessons Learned](#lessons-learned)
9. [Reference: Configuration Details](#reference-configuration-details)

---

## Introduction

This document captures TrashMob.eco's experience migrating from Azure AD B2C to Microsoft Entra External ID, including the first-ever integration of Entra External ID with Privo.com for COPPA-compliant age verification and parental consent.

TrashMob.eco is a volunteer coordination platform for community cleanup events. The platform serves web (React SPA), mobile (MAUI), and backend (.NET 10 API) clients, all authenticating against a shared identity provider.

**Purpose of this guide:**
- Document the end-to-end migration process for other organizations considering the same move
- Provide a reference implementation for integrating Privo.com age verification with Entra External ID Custom Authentication Extensions
- Capture lessons learned and pitfalls to help the next team avoid them

---

## Why Migrate?

### B2C Pain Points

Azure AD B2C served TrashMob well for several years, but accumulated issues made migration necessary:

1. **Broken sign-up flow:** When a user tries to sign up with an email that already exists in the directory, the B2C custom policy hangs indefinitely. No error, no redirect — just a blank screen. This is a known B2C IEF bug with no resolution path.

2. **Broken profile edit:** The `B2C_1A_TM_PROFILEEDIT` custom policy stopped working. Debugging IEF XML policies is extremely difficult — the tooling is limited and error messages are opaque.

3. **B2C is end-of-life:** Microsoft stopped selling B2C to new customers in May 2025. No new features are being developed. Security patches only, with support guaranteed until at least May 2030. The writing is on the wall.

4. **IEF XML complexity:** B2C's Identity Experience Framework uses XML-based custom policies that are difficult to write, debug, and maintain. A single missing claim transformation can silently break a flow.

5. **Branding limitations:** Updating the B2C sign-in page branding requires editing custom HTML/CSS hosted in blob storage, then referencing it from the IEF policy XML. It's fragile and hard to iterate on.

### Entra External ID Benefits

1. **GUI-configured user flows** replace IEF XML — simpler to set up and maintain
2. **Custom Authentication Extensions** (Azure Functions) replace custom policies — real code instead of XML, with proper debugging, logging, and testing
3. **Microsoft Graph API** for profile management — replaces the broken profile edit policy with full programmatic control
4. **Built-in branding** — logo, colors, background image, and layout templates configured in the portal
5. **Active development** — Microsoft is investing in Entra External ID as the successor to B2C

### The Privo Opportunity

Privo.com provides COPPA-compliant age verification and Verifiable Parental Consent (VPC). TrashMob is the first organization to integrate Privo with Entra External ID, making the Custom Authentication Extensions approach the reference implementation for Privo's other customers.

---

## Architecture Overview: Before & After

### Before: Azure AD B2C

```
┌─────────────────────────────────────────────────────────────┐
│                    Azure AD B2C Tenant                       │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │        Identity Experience Framework (IEF)           │   │
│  │                                                     │   │
│  │  B2C_1A_TM_SIGNUP_SIGNIN  (custom XML policy)       │   │
│  │  B2C_1A_TM_PROFILEEDIT   (broken)                  │   │
│  │  B2C_1A_TM_DEREGISTER    (custom XML policy)       │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  Social IDPs: Google, Microsoft, Apple, Facebook           │
│  Custom HTML/CSS branding (blob storage)                    │
│                                                             │
│  App Registrations:                                        │
│    - Web SPA (dev/prod)                                    │
│    - Mobile (dev/prod)                                     │
│    - Backend API (dev/prod)                                │
└─────────────────────────────────────────────────────────────┘
         │                    │                    │
    ┌────┴────┐         ┌────┴────┐         ┌────┴────┐
    │  React  │         │  MAUI   │         │  .NET   │
    │   SPA   │         │ Mobile  │         │   API   │
    └─────────┘         └─────────┘         └─────────┘
```

### After: Entra External ID

```
┌─────────────────────────────────────────────────────────────┐
│              Entra External ID Tenant                        │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │           User Flow (GUI-configured)                 │   │
│  │                                                     │   │
│  │  Sign-up/Sign-in flow                               │   │
│  │    └── Custom Auth Extension ──► Azure Function     │   │
│  │                                    └── Privo API    │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  Social IDPs: Google, Microsoft, Apple, Facebook           │
│  Built-in branding (portal-configured)                      │
│                                                             │
│  In-app (replaces broken policies):                        │
│    - Profile edit via Microsoft Graph API                   │
│    - Account deletion via Microsoft Graph API               │
└─────────────────────────────────────────────────────────────┘
         │                    │                    │
    ┌────┴────┐         ┌────┴────┐         ┌────┴────┐
    │  React  │         │  MAUI   │         │  .NET   │
    │   SPA   │         │ Mobile  │         │   API   │
    └─────────┘         └─────────┘         └─────────┘
```

**Key architectural shift:** B2C's XML-based IEF policies are replaced by Entra External ID's GUI-configured user flows plus Azure Functions for custom logic. Profile management moves from a B2C policy to in-app code using Microsoft Graph API.

---

## Current Azure AD B2C Architecture

### Tenant Structure

TrashMob uses separate B2C tenants for development and production:

| Environment | Tenant Domain | B2C Login Domain |
|-------------|---------------|------------------|
| Development | `trashmobdev.onmicrosoft.com` | `trashmobdev.b2clogin.com` |
| Production | `trashmob.onmicrosoft.com` | `trashmob.b2clogin.com` |

### App Registrations (6 Total)

| Platform | Environment | Purpose |
|----------|-------------|---------|
| Web SPA | Dev | React single-page app (MSAL.js) |
| Web SPA | Prod | React single-page app (MSAL.js) |
| Mobile | Dev | MAUI cross-platform app (MSAL.NET) |
| Mobile | Prod | MAUI cross-platform app (MSAL.NET) |
| Backend API | Dev | ASP.NET Core API (Microsoft.Identity.Web) |
| Backend API | Prod | ASP.NET Core API (Microsoft.Identity.Web) |

### Custom Policies (IEF XML)

Three custom policies built on the Identity Experience Framework:

1. **`B2C_1A_TM_SIGNUP_SIGNIN`** — Primary sign-up and sign-in flow
   - Supports email/password and social login (Google, Microsoft, Apple, Facebook)
   - Custom claims: email
   - **Known bug:** Hangs when signing up with an existing email

2. **`B2C_1A_TM_PROFILEEDIT`** — Profile editing flow
   - **Status: BROKEN** — No longer functioning, root cause unclear in IEF XML
   - Cannot be easily debugged

3. **`B2C_1A_TM_DEREGISTER`** — Account deletion flow
   - Allows users to delete their account and associated data

### MSAL Usage Across Platforms

All three platforms use the Microsoft Authentication Library (MSAL):

**React SPA (`@azure/msal-browser`):**
```
MSAL PublicClientApplication
  → Authority: https://{b2cDomain}/{tenantDomain}/{policy}
  → Scopes: TrashMob.Read, TrashMob.Writes, email
  → Cache: sessionStorage
  → Redirect: window.location
```

**MAUI Mobile (`Microsoft.Identity.Client`):**
```
MSAL PublicClientApplication
  → Authority: https://{b2cDomain}/tfp/{tenantDomain}/{policy}
  → Scopes: TrashMob.Read, TrashMob.Writes, email
  → iOS: Keychain security group
  → Android: Parent activity context
  → Redirect: eco.trashmob.trashmobmobile://auth
```

**ASP.NET Core API (`Microsoft.Identity.Web`):**
```
AddMicrosoftIdentityWebApi
  → Bound to "AzureAdB2C" config section
  → NameClaimType: "name"
  → Audience validation: disabled
  → Custom OnChallenge event for JSON 401 responses
```

### User Identity Resolution

The critical data flow for user identity:

```
JWT Token (from B2C)
    ↓
Extract email claim (ClaimTypes.Email or "email")
    ↓
Database lookup: GetUserByEmailAsync(email)
    ↓
Store UserId in HttpContext.Items["UserId"]
    ↓
Controllers access via base.UserId property
```

This email-based lookup pattern is used identically on web, mobile, and backend. The `UserIsValidUserAuthHandler` authorization handler performs this lookup on every authenticated request.

### Dynamic Configuration

The backend serves B2C configuration to frontends via `GET /api/config`:

```json
{
  "azureAdB2C": {
    "clientId": "...",
    "authorityDomain": "trashmobdev.b2clogin.com",
    "policies": {
      "signUpSignIn": "B2C_1A_TM_SIGNUP_SIGNIN",
      "deleteUser": "B2C_1A_TM_DEREGISTER",
      "profileEdit": "B2C_1A_TM_PROFILEEDIT"
    },
    "authorities": {
      "signUpSignIn": "https://trashmobdev.b2clogin.com/trashmobdev.onmicrosoft.com/B2C_1A_TM_SIGNUP_SIGNIN"
    },
    "scopes": [
      "https://trashmobdev.onmicrosoft.com/api/TrashMob.Read",
      "https://trashmobdev.onmicrosoft.com/api/TrashMob.Writes",
      "email"
    ]
  }
}
```

The React SPA loads this at startup with a hardcoded fallback for local development. This pattern means **changing auth configuration requires only a backend config change** — no frontend redeployment needed.

### Authorization Policies

The backend uses custom authorization policies:

| Policy | Description |
|--------|-------------|
| `ValidUser` | User must exist in database (email from JWT → DB lookup) |
| `UserOwnsEntity` | User owns the requested resource |
| `UserOwnsEntityOrIsAdmin` | Owner or site admin |
| `UserIsAdmin` | Site admin only |
| `UserIsEventLead` | Event lead role |
| `UserIsPartnerUserOrIsAdmin` | Partner user or admin |

---

## Target Entra External ID Architecture

### What Changes

| Aspect | B2C | Entra External ID |
|--------|-----|-------------------|
| **Sign-up/Sign-in** | IEF XML custom policy | GUI-configured user flow |
| **Custom logic** | Claims transformations in XML | Custom Authentication Extensions (Azure Functions) |
| **Profile edit** | B2C policy (broken) | In-app via Microsoft Graph API |
| **Account deletion** | B2C policy | In-app via Microsoft Graph API |
| **Branding** | Custom HTML/CSS in blob storage | Built-in branding (portal) |
| **Social IDPs** | Configured in IEF XML | GUI-configured in portal |
| **Age verification** | Not supported | Custom Auth Extension → Privo API |

### What Stays the Same

| Aspect | Details |
|--------|---------|
| **MSAL libraries** | Same libraries on all platforms — config changes only |
| **JWT token format** | Standard JWT with email claim |
| **User identity resolution** | Email from JWT → database lookup (no change) |
| **Authorization policies** | Same custom authorization handlers |
| **API scopes** | Same scope names (new tenant domain) |
| **`/api/config` pattern** | Same endpoint, updated values |

### Key Insight: MSAL Compatibility

The most important architectural discovery is that **MSAL is compatible with both B2C and Entra External ID**. The migration is primarily a configuration change:

- **Authority URL format changes** (no more `/tfp/` prefix for mobile)
- **Tenant domain changes** (new external tenant)
- **Policy names disappear** (replaced by user flow, no policy in authority URL)
- **Scopes may change** (new tenant domain in scope URIs)

The MSAL API surface (`AcquireTokenInteractive`, `AcquireTokenSilent`, `PublicClientApplication`) remains identical.

### Profile Management via Graph API

Since Entra External ID does not offer a profile edit user flow, profile management moves to application code:

```
User clicks "Edit Profile" in app
    ↓
App shows in-app form (React / MAUI)
    ↓
Form submits to our API
    ↓
API calls Microsoft Graph API:
  PATCH /users/{userId}
    ↓
Updated fields: displayName, givenName, surname, etc.
```

**Advantage:** Full control over the profile edit UX. No more fragile B2C policy XML.

---

## Migration Phases

### Phase 0 — Tenant & Groundwork
- Create Entra External ID external tenant (development)
- Register app registrations (web SPA, mobile, backend API)
- Configure social identity providers (Google, Microsoft, Apple, Facebook)
- Configure built-in branding (logo, colors, background)
- Document tenant setup process

### Phase 1 — Sign-Up/Sign-In + Profile Photos
- Set up sign-up/sign-in user flow with custom attributes
- Configure MSAL on web (React) to point to new tenant
- Update backend JWT validation (authority URL, audience)
- Auto-populate profile photo URL from social provider `picture` claim
- Update `/api/config` endpoint with new tenant details

### Phase 2 — In-App Profile Edit + Photo Upload
- Build in-app profile edit page (replaces broken B2C policy)
- Update profile fields via Microsoft Graph API
- Add profile photo upload
- In-app account deletion (replaces B2C deregister policy)

### Phase 3 — Privo Age Verification
- Build Custom Authentication Extension (Azure Function) for age gate
- Integrate with Privo API on `OnAttributeCollectionSubmit` event
- Age-based routing: under-13 block, 13-17 minor flow, 18+ standard
- Implement Privo VPC webhook for parental consent

### Phase 4 — User Migration + Testing
- Run Microsoft migration tool to export B2C users
- Import users to Entra External ID
- Configure JIT (Just-In-Time) password migration
- Parallel operation of both systems during coexistence period

### Phase 5 — Production Cutover
- Create production Entra External ID tenant
- Register production app registrations
- Run production user migration
- Switch web and API traffic to new system
- Monitor for auth failures with hot rollback to B2C

### Phase 6 — Mobile App Update
- Update MAUI MSAL config for new tenant
- Push app updates to iOS and Android stores
- Force update flow for users on old version
- Decommission B2C after coexistence period

---

## Privo.com Integration via Custom Authentication Extensions

### Overview

Custom Authentication Extensions are Entra External ID's mechanism for injecting custom logic into user flows. They are Azure Functions triggered at specific points in the authentication process.

**Key Events:**

| Event | When It Fires | Use Case |
|-------|---------------|----------|
| `OnAttributeCollectionStart` | Before attribute collection page renders | Pre-populate fields, skip collection |
| `OnAttributeCollectionSubmit` | After user submits attributes | **Age verification via Privo** |
| `OnTokenIssuanceStart` | Before token is issued | Add custom claims to token |

### Age Verification Flow

```
User fills sign-up form (includes birthdate)
    ↓
Entra External ID triggers OnAttributeCollectionSubmit
    ↓
Azure Function receives birthdate + email
    ↓
Azure Function calls Privo API for age verification
    ↓
┌────────────────────┬──────────────────┬──────────────────┐
│     Under 13       │     13-17        │      18+         │
├────────────────────┼──────────────────┼──────────────────┤
│ Return:            │ Return:          │ Return:          │
│ ShowBlockPage      │ ModifyAttributes │ Continue         │
│ (registration      │ (set IsMinor=    │ (standard        │
│  denied)           │  true, trigger   │  registration)   │
│                    │  VPC flow)       │                  │
└────────────────────┴──────────────────┴──────────────────┘
```

### Custom Authentication Extension Response Format

The Azure Function returns a specific JSON format that Entra External ID interprets:

**Continue (18+ adult):**
```json
{
  "data": {
    "@odata.type": "microsoft.graph.onAttributeCollectionSubmitResponseData",
    "actions": [
      {
        "@odata.type": "microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior"
      }
    ]
  }
}
```

**Block (under 13):**
```json
{
  "data": {
    "@odata.type": "microsoft.graph.onAttributeCollectionSubmitResponseData",
    "actions": [
      {
        "@odata.type": "microsoft.graph.attributeCollectionSubmit.showBlockPage",
        "message": "You must be at least 13 years old to create an account."
      }
    ]
  }
}
```

**Modify attributes (13-17 minor):**
```json
{
  "data": {
    "@odata.type": "microsoft.graph.onAttributeCollectionSubmitResponseData",
    "actions": [
      {
        "@odata.type": "microsoft.graph.attributeCollectionSubmit.modifyAttributeValues",
        "attributes": {
          "isMinor": true
        }
      }
    ]
  }
}
```

### Privo VPC (Verifiable Parental Consent) Webhook

After a minor registers, Privo handles the parental consent flow. The application receives consent status updates via webhook:

```
Minor registers (13-17)
    ↓
Custom Auth Extension sets IsMinor flag
    ↓
Application creates ParentalConsent record (status: Pending)
    ↓
Application calls Privo API to initiate VPC
    ↓
Privo contacts parent via email
    ↓
Parent completes verification (credit card, ID, or video call)
    ↓
Privo calls our webhook: POST /api/webhooks/privo
    ↓
Application updates consent status → Verified
    ↓
Minor account upgraded from limited to full access
```

---

## Lessons Learned

*This section will be updated as the migration progresses.*

### Phase 0 Lessons

- *(To be documented during implementation)*

### Known Limitations

1. **Custom CSS cutoff:** Entra External ID restricted custom CSS to tenants created before January 5, 2026. New tenants can only use built-in branding (logo, colors, background image, layout templates). For TrashMob, this is acceptable since all post-sign-in UX is in-app.

2. **No profile edit user flow:** Entra External ID does not offer a profile edit user flow. Profile management must be built in-app using Microsoft Graph API. This is actually an advantage — full control over the UX.

3. **JIT password migration:** B2C password hashes cannot be exported. The Microsoft migration tool supports Just-In-Time password migration where users re-authenticate on first login. Users who signed up via social providers (Google, Microsoft, etc.) migrate seamlessly.

---

## Reference: Configuration Details

### MSAL Configuration Comparison

**React SPA:**

| Setting | B2C | Entra External ID |
|---------|-----|-------------------|
| Authority | `https://{b2c}.b2clogin.com/{tenant}/{policy}` | `https://{tenant}.ciamlogin.com/` |
| Known authorities | `["{b2c}.b2clogin.com"]` | `["{tenant}.ciamlogin.com"]` |
| Client ID | SPA app registration client ID | New SPA app registration client ID |
| Scopes | `https://{tenant}/api/TrashMob.Read` | `https://{tenant}/api/TrashMob.Read` |
| Cache location | `sessionStorage` | `sessionStorage` |

**MAUI Mobile:**

| Setting | B2C | Entra External ID |
|---------|-----|-------------------|
| Authority | `https://{b2c}.b2clogin.com/tfp/{tenant}/{policy}` | `https://{tenant}.ciamlogin.com/` |
| Client ID | Mobile app registration client ID | New mobile app registration client ID |
| Redirect URI | `eco.trashmob.trashmobmobile://auth` | `eco.trashmob.trashmobmobile://auth` |

**ASP.NET Core API:**

| Setting | B2C | Entra External ID |
|---------|-----|-------------------|
| Instance | `https://{b2c}.b2clogin.com/` | `https://{tenant}.ciamlogin.com/` |
| Domain | `{tenant}.onmicrosoft.com` | `{tenant}.onmicrosoft.com` |
| Client ID | API app registration client ID | New API app registration client ID |
| Config section | `AzureAdB2C` | `AzureAd` (or custom) |

### Files That Need to Change

| File | What Changes |
|------|-------------|
| `TrashMob/appsettings.json` | Authority URL, client IDs, domain |
| `TrashMob/appsettings.Development.json` | Dev tenant configuration |
| `TrashMob/Program.cs` | Config section name (`AzureAdB2C` → `AzureAd`) |
| `TrashMob/Controllers/ConfigController.cs` | Config response format, remove policy references |
| `TrashMob/client-app/src/store/AuthStore.tsx` | Authority format, remove policies, update fallback |
| `TrashMobMobile/Authentication/AuthConstants.cs` | Tenant domain, authority format, client IDs |
| `TrashMobMobile/Authentication/AuthService.cs` | Authority construction (remove `/tfp/` prefix) |

---

**Last Updated:** February 8, 2026
**Status:** Living document — updated as migration progresses
**Next Update:** After Phase 0 completion
