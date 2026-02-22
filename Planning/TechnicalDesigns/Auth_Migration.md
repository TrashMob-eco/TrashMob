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
| **JWT token format** | Standard JWT (but see CIAM Token Differences below) |
| **Authorization policies** | Same custom authorization handlers (enhanced with OID + Graph API fallback) |
| **API scopes** | Same scope names (new tenant domain) |
| **`/api/config` pattern** | Same endpoint, updated values |

### CIAM Token Differences (Critical)

**The `email` claim behaves differently in CIAM vs B2C:**

| Token Type | B2C | CIAM (Entra External ID) |
|------------|-----|--------------------------|
| **id_token** | Contains `email` claim | **No `email` claim** — CIAM stores emails in `identities` collection, not `mail` property |
| **access_token** | Contains `email` claim | Contains `email` claim (populated from sign-in identity) |

**Impact:**
- **Frontend** reads `idTokenClaims` → email is unavailable → must fall back to `oid` for user lookup
- **Backend** reads claims from access_token → email IS available → can still use email-based lookup
- **Token validation** (`validateToken()` in `AuthStore.tsx`) must accept tokens with `oid` (not just `email`)
- **User identity resolution** changed from email-only to 4-step: email → ObjectId → Graph API → auto-create

**User Identity Resolution (Updated):**
```
Access Token (from CIAM)
    ↓
Step 1: Extract email claim → GetUserByEmailAsync(email)
    ↓ (if found, auto-link CIAM ObjectId for B2C→CIAM migration)
Step 2: If not found, extract oid → GetUserByObjectIdAsync(oid)
    ↓
Step 3: If still not found, call CiamGraphService.GetUserEmailAsync(oid)
    ↓ (Graph API resolves email from CIAM directory identities collection)
Step 4: If still not found, auto-create user from available claims
    ↓
Store UserId in HttpContext.Items["UserId"]
    ↓
Controllers access via base.UserId property
```

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

### Phase 0 Lessons

1. **App registrations must be added to the user flow.** After creating app registrations and a sign-up/sign-in user flow in the Entra External ID portal, you must explicitly add each app registration to the user flow. Without this step, the sign-in page will only show email/password sign-in — no sign-up option and no social identity providers will appear.

2. **Tenant naming must avoid existing B2C namespaces.** Our B2C tenants use `trashmobdev` and `trashmob` domain prefixes. Since these are already claimed, the Entra External ID tenants need different names. We used `TrashMobEcoDev` (domain: `trashmobecodev.ciamlogin.com`) for development and `TrashMobEcoPr` (domain: `trashmobecopr.ciamlogin.com`) for production.

3. **User flow attribute collection requires explicit configuration.** Custom attributes (e.g., `givenName`, `surname`, `dateOfBirth`) must be explicitly added to the user flow's attribute collection step in the portal. If attributes are not added, the sign-up form will only collect email and password — users won't be prompted for name or birthdate.

4. **Config section is `AzureAdEntra`, not `AzureAd`.** The actual implementation uses a dedicated `AzureAdEntra` config section (see `appsettings.json`) to allow both B2C and Entra configs to coexist during the feature-flag transition period (`UseEntraExternalId`).

5. **Built-in branding logo sizing.** The banner logo on the Entra External ID sign-in page displays at approximately 260x36 pixels. Standard logos may be hard to read at this size — consider creating a simplified or higher-contrast version specifically for the sign-in page.

### Phase 5 Lessons (Production Cutover — February 2026)

6. **CIAM id_tokens do NOT include email claims.** This was the biggest surprise during production cutover. CIAM stores sign-up emails in the `identities` collection (with `signInType: "emailAddress"`), not in the `mail` property. The `email` optional claim reads from `mail`, so it has nothing to emit. The email DOES appear in access_tokens. **Workaround:** Backend uses Microsoft Graph API (`User.Read.All` application permission) to resolve emails from the CIAM directory. Frontend falls back to ObjectId-based user lookup.

7. **Backend API app must be "Single tenant only."** Setting the backend API app registration to "Any Entra ID Tenant + Personal Microsoft accounts" (multi-tenant) causes `AADSTS500207: The account type can't be used for the resource you've requested`. Must use "Accounts in this organizational directory only" for CIAM tenants.

8. **Application ID URI format matters.** The Application ID URI for the backend API must use the CIAM domain format: `https://trashmobecopr.onmicrosoft.com/api` (not `api://<client-id>`). Using the wrong format causes `AADSTS500011: The resource principal named ... was not found`.

9. **Frontend token validation must not require email.** The original `validateToken()` function checked for `email` in `idTokenClaims`. Since CIAM id_tokens lack email, this rejected all tokens and caused `CanceledError: User not found!` on every protected API call. Fix: accept tokens with either `email` or `oid`.

10. **OID auto-linking enables zero-downtime B2C→CIAM migration.** Instead of bulk migrating users, existing B2C users are auto-linked to their CIAM identity on first sign-in. The auth handler finds the user by email (from access token) and updates their `ObjectId` to the new CIAM value. This eliminates the need for a migration window.

11. **Graph API requires a client secret in Key Vault.** The `CiamGraphService` uses `ClientSecretCredential` (app-only) to call Graph API. The secret must be stored as `AzureAdEntra--ClientSecret` in Key Vault. The service gracefully degrades when the secret is not configured — it simply returns null for email resolution.

12. **`isUserLoaded` check must handle empty GUIDs.** The frontend `UserData.id` defaults to `Guid.createEmpty()` (`00000000-0000-0000-0000-000000000000`), which is truthy in JavaScript. After removing the email-based check, `isUserLoaded` must explicitly exclude the empty GUID: `!!currentUser.id && currentUser.id !== EMPTY_GUID`.

### Known Limitations

1. **Custom CSS cutoff:** Entra External ID restricted custom CSS to tenants created before January 5, 2026. New tenants can only use built-in branding (logo, colors, background image, layout templates). For TrashMob, this is acceptable since all post-sign-in UX is in-app.

2. **No profile edit user flow:** Entra External ID does not offer a profile edit user flow. Profile management must be built in-app using Microsoft Graph API. This is actually an advantage — full control over the UX.

3. **JIT password migration:** B2C password hashes cannot be exported. The Microsoft migration tool supports Just-In-Time password migration where users re-authenticate on first login. Users who signed up via social providers (Google, Microsoft, etc.) migrate seamlessly.

4. **CIAM sign-out shows empty account picker.** After sign-out, the CIAM sign-out flow sometimes shows an empty "Pick an account" dialog. Workaround: clear session storage manually. Root cause under investigation.

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
| Config section | `AzureAdB2C` | `AzureAdEntra` |

### Files Changed

| File | What Changed |
|------|-------------|
| `TrashMob/appsettings.json` | Authority URL, client IDs, domain, added `ClientSecret` for Graph API |
| `TrashMob/appsettings.Development.json` | Dev tenant configuration |
| `TrashMob/Program.cs` | Config section name (`AzureAdB2C` → `AzureAdEntra`), DI for `CiamGraphService` |
| `TrashMob/Controllers/ConfigController.cs` | Config response format, remove policy references |
| `TrashMob/Controllers/UsersController.cs` | Added `GetUserByObjectId` endpoint for OID-based lookup |
| `TrashMob/Security/UserIsValidUserAuthHandler.cs` | 4-step user resolution: email → OID → Graph API → auto-create |
| `TrashMob/Services/CiamGraphService.cs` | **New** — Graph API email resolution from CIAM directory |
| `TrashMob/Services/ICiamGraphService.cs` | **New** — Interface for CIAM Graph service |
| `TrashMob/client-app/src/store/AuthStore.tsx` | Authority format, `validateToken` accepts `oid` (not just `email`) |
| `TrashMob/client-app/src/hooks/useLogin.ts` | OID-based user lookup fallback, `isUserLoaded` empty GUID check |
| `TrashMob/client-app/src/services/users.ts` | Added `GetUserByObjectId` service |
| `TrashMobMobile/Authentication/AuthConstants.cs` | Tenant domain, authority format, client IDs |
| `TrashMobMobile/Authentication/AuthService.cs` | Authority construction (remove `/tfp/` prefix) |

---

## Phase 0 Setup Commands

### Tenant Creation

Tenant creation is **portal-only** — Azure CLI does not support creating Entra External ID tenants.

1. Go to [Azure Portal → Microsoft Entra External ID](https://portal.azure.com/#view/Microsoft_AAD_IAM/CompanyBrandingBlade)
2. Create new external tenant
3. Record tenant name, domain, and tenant ID

### App Registrations (CLI)

After the tenant is created, app registrations can be created via Azure CLI:

```bash
# Switch to the new external tenant
az login --tenant <tenant-id>

# Web SPA app registration (dev)
az ad app create \
  --display-name "TrashMob Web Dev" \
  --sign-in-audience AzureADandPersonalMicrosoftAccount \
  --web-redirect-uris "http://localhost:3000" "https://dev.trashmob.eco" \
  --enable-id-token-issuance true \
  --enable-access-token-issuance false

# Backend API app registration (dev)
az ad app create \
  --display-name "TrashMob API Dev" \
  --sign-in-audience AzureADandPersonalMicrosoftAccount \
  --identifier-uris "api://<api-client-id>"

# Mobile app registration (dev)
az ad app create \
  --display-name "TrashMob Mobile Dev" \
  --sign-in-audience AzureADandPersonalMicrosoftAccount \
  --public-client-redirect-uris "eco.trashmob.trashmobmobile://auth"
```

### Social IDP Configuration

Social identity provider configuration (Google, Microsoft, Apple, Facebook) is **portal-only**:

1. Go to External ID tenant → External Identities → All identity providers
2. Add each social provider with client ID and secret from their developer consoles
3. Configure user flow to include social providers

### User Flow Configuration

User flow creation and configuration is **portal-only**:

1. Go to External ID tenant → User flows
2. Create sign-up/sign-in user flow
3. Configure custom attributes (givenName, surname, dateOfBirth)
4. Add Custom Authentication Extension for Privo age verification (Phase 3)

### Branding Configuration

Built-in branding (logo, colors, background image) is **portal-only**:

1. Go to External ID tenant → Company branding
2. Upload TrashMob logo, configure colors, set background image

---

**Last Updated:** February 22, 2026
**Status:** Living document — production cutover complete, CIAM token workarounds documented
**Next Update:** After Privo API integration (Phase 3)
