# Project 1 — Auth Revamp (Azure B2C → Entra External ID)

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phase 0) |
| **Priority** | Critical |
| **Risk** | High |
| **Size** | Large |
| **Dependencies** | None (Project 23 depends on this) |

---

## Business Rationale

Migrate from Azure B2C to Microsoft Entra External ID to fix critical auth issues, modernize the sign-in experience, and enable Privo.com age verification for minors (Project 23).

**Why now (not later):**
- **B2C is broken:** Sign-up hangs for existing emails (#2321), profile edit not working, branding outdated
- **B2C is end-of-life:** Microsoft stopped selling B2C to new customers (May 2025). No new features, security patches only. Supported until at least May 2030.
- **Privo sponsorship:** TrashMob is the first organization to integrate Entra External ID with Privo.com. Privo is asking us to generate documentation on the process for sharing with their other customers.
- **Profile photos:** Users want profile photos; social providers already have them — migration gives us this for free

**Decision: Entra External ID** (resolved February 2026)

---

## Objectives

### Primary Goals
- **Migrate** from Azure B2C to Entra External ID with minimal user disruption
- **Fix** broken sign-up flow (existing email hangs), profile edit, and branding
- **Add** profile photo support (auto-populate from social IDP + manual upload)
- **Enable** Privo.com age verification via Custom Authentication Extensions (Project 23)
- **Document** the Entra External ID + Privo integration process (sponsorship deliverable)

### Secondary Goals
- Set home location during sign-up flow
- ToS/Privacy consent with versioning and re-consent support
- Optional partner SSO for community administrators
- Reduce auth-related support tickets
- Improve signup completion rate

---

## Current B2C Implementation (What Must Be Replaced)

### Known Issues
- **Sign-up for existing email hangs** (#2321) — B2C custom policy bug, no resolution path
- **Profile edit not working** — B2C_1A_TM_PROFILEEDIT policy broken
- **Branding outdated** — Custom HTML/CSS pages need refresh
- **B2C maintenance mode** — No new features from Microsoft

### Current Architecture
- **3 custom policies (IEF XML):** B2C_1A_TM_SIGNUP_SIGNIN, B2C_1A_TM_DEREGISTER, B2C_1A_TM_PROFILEEDIT
- **6 client IDs:** 2 web (dev/prod), 2 mobile (dev/prod), 2 backend (dev/prod)
- **User lookup by email** — auth handlers extract email from JWT, query database
- **MSAL on all platforms** — React (@azure/msal-react), MAUI (Microsoft.Identity.Client), Backend (Microsoft.Identity.Web)

### Entra External ID Replacements

| B2C Feature | Entra External ID Replacement |
|-------------|-------------------------------|
| B2C_1A_TM_SIGNUP_SIGNIN (IEF policy) | User flow (GUI-configured) + Custom Authentication Extension for Privo |
| B2C_1A_TM_PROFILEEDIT (broken) | In-app profile edit via Microsoft Graph API (more control) |
| B2C_1A_TM_DEREGISTER | In-app account deletion via Graph API |
| Custom HTML/CSS branding | Built-in branding (logo, colors, background, layout) |

### Branding Limitation
Custom CSS is **not available** — Entra External ID restricted custom CSS to tenants created before January 5, 2026, and our tenant has not been created yet. Built-in branding options (logo, colors, background image, layout templates) cover the sign-in page. All other UX is in-app and fully controlled by us.

**Mitigation:** Request exception from Microsoft given Privo sponsorship. Fall back to built-in branding if denied — it covers 90% of sign-in page needs.

---

## Scope

### Phase 0 — Tenant & Groundwork

#### Completed (Code — PR #2620)
- [x] Add `GivenName`, `Surname`, `DateOfBirth`, `ProfilePhotoUrl` to User model + EF migration
- [x] Add fields to frontend `UserData` model
- [x] Display profile photos in UserNav when available
- [x] Add `UseEntraExternalId` feature flag + `AzureAdEntra` config section
- [x] Update `Program.cs` for feature-flag-aware auth config binding
- [x] Split `ConfigController` into B2C and Entra config builders
- [x] Update frontend `AuthStore` and config types for dual auth
- [x] Add Entra guards in UserNav and DeleteMyData
- [x] Document Phase 0 CLI commands in Auth_Migration.md

#### Completed (Manual — Azure Portal, Dev)
- [x] Create Entra External ID external tenant (dev: `TrashMobEcoDev`, domain `trashmobecodev.ciamlogin.com`) — see [Step-by-Step: Tenant Setup](#step-by-step-tenant-setup)
- [x] Register app registrations (web, mobile, backend) for dev — see [Step-by-Step: App Registrations](#step-by-step-app-registrations)
- [x] Configure social identity providers: Google, Facebook — see [Step-by-Step: Social IDPs](#step-by-step-social-identity-providers)
- [x] Create sign-up/sign-in user flow with custom attributes — see [Step-by-Step: User Flow](#step-by-step-user-flow)
- [x] Configure built-in branding (logo, colors, background) — see [Step-by-Step: Branding](#step-by-step-branding)
- [x] Fill in `AzureAdEntra` config values in appsettings.Development.json
- [x] Test sign-in with `UseEntraExternalId: true` on dev — sign-in, sign-up, Google, and Facebook all working

#### Remaining (Manual — Azure Portal, Dev)
- [ ] Add custom attributes (givenName, surname, dateOfBirth) to user flow attribute collection — currently sign-up only collects email/password
- [ ] Configure social identity provider: Microsoft account
- [ ] Configure social identity provider: Apple (requires Apple Developer setup)
- [ ] Improve banner logo for sign-in page — current logo is hard to read at the small display size (260x36px); consider a simplified/higher-contrast version
- [ ] Request custom CSS exception from Microsoft (given Privo sponsorship)
- [ ] Fill in `AzureAdEntra` config values in Key Vault (for deployed dev environment)
- [ ] Document tenant setup process (Privo documentation deliverable)

#### Remaining (Manual — Azure Portal, Prod)
- [ ] Create Entra External ID external tenant (prod: `TrashMobEco`, domain `trashmobeco.ciamlogin.com`)
- [ ] Register app registrations (web, mobile, backend) for prod
- [ ] Configure social identity providers: Google, Facebook, Microsoft, Apple
- [ ] Create sign-up/sign-in user flow with custom attributes
- [ ] Configure built-in branding (logo, colors, background)
- [ ] Fill in `AzureAdEntra` config values in Key Vault (for deployed prod environment)

### Phase 1 — Sign-Up/Sign-In + Profile Photos
- [ ] Upgrade MSAL packages: `@azure/msal-browser` v2→v5 and `@azure/msal-react` v1→v5 (close Renovate PR #2036)
- [ ] Configure MSAL on web (React) to point to new tenant
- [ ] Update backend JWT validation (authority URL, audience)
- [ ] Auto-populate `ProfilePhotoUrl` from social provider `picture` claim on sign-up
- [ ] Display profile photos in event attendees, leaderboards
- [ ] Document sign-up flow for Privo

### Phase 2 — In-App Profile Edit + Photo Upload
- [ ] Build in-app profile edit page (replaces broken B2C policy)
- [ ] Update profile fields via Microsoft Graph API
- [ ] Add profile photo upload to Azure Blob Storage
- [ ] Photo display across web and mobile (avatars, attendee lists, etc.)
- [ ] In-app account deletion (replaces B2C deregister policy)
- [ ] Home location capture during sign-up or profile edit

### Phase 3 — Privo Age Verification (→ Project 23 Phase 1-2)
- [ ] Build Custom Authentication Extension (Azure Function) for age gate
- [ ] Integrate with Privo API on `OnAttributeCollectionSubmit` event
- [ ] Under-13 block, 13-17 minor flow trigger, 18+ standard flow
- [ ] Implement Privo VPC (Verifiable Parental Consent) webhook
- [ ] Consent status tracking in database (ParentalConsent entity)
- [ ] Pending account limitations for minors awaiting consent
- [ ] Document Custom Authentication Extension + Privo integration (sponsorship deliverable)

### Phase 4 — User Migration + Testing
- [ ] Run MS migration tool to export B2C users → import to Entra External ID
- [ ] Configure JIT (Just-In-Time) password migration for first-login
- [ ] Run both systems in parallel (coexistence period)
- [ ] Smoke test with real accounts on dev
- [ ] Security audit of new configuration
- [ ] Load testing

### Phase 5 — Production Cutover (Web + Backend)
- [ ] Create production Entra External ID tenant
- [ ] Register production app registrations
- [ ] Run production user migration
- [ ] Update production MSAL config + backend JWT validation
- [ ] Switch all web traffic to new system
- [ ] Monitor for auth failures, have hot rollback to B2C
- [ ] 24/7 monitoring during migration window

### Phase 6 — Mobile App Update
- [ ] Update MAUI MSAL config (AuthConstants.cs) for new tenant
- [ ] Test sign-in/sign-up on iOS and Android
- [ ] Push app update to stores
- [ ] Force update flow for users on old version
- [ ] Monitor crash-free rate
- [ ] Decommission B2C (after coexistence period)

### Phase 7 — Minor Protections (→ Project 23 Phase 3)
- [ ] Communication restrictions for minors
- [ ] Limited profile visibility (first name + last initial)
- [ ] Adult presence enforcement at events
- [ ] Parent notification system
- [ ] Complete Privo documentation package

---

## Out-of-Scope

- Automated billing for community subscriptions (manual for 2026)
- Multi-factor authentication (future enhancement)
- Social login consolidation (link Google + Microsoft accounts)
- Enterprise directory sync for large partners
- Custom CSS on hosted sign-in pages (cutoff passed; use built-in branding)

---

## Success Metrics

### Quantitative
- **Signup completion rate:** Increase by 15% (fix broken existing-email hang)
- **Auth-related support tickets:** Decrease by 40%
- **Migration success rate:** 99%+ of B2C users migrated without issues
- **Rollback events:** 0 (successful migration without needing rollback)
- **Profile photo adoption:** 30%+ of users with social IDP photos within 3 months

### Qualitative
- Mobile app store updates published and approved
- Zero security incidents related to auth changes
- Positive user feedback on new sign-in experience
- Privo documentation package delivered to sponsor

---

## Dependencies

### Blockers
- **Privo.com contract:** Required before Phase 3 (age verification)
- **Microsoft Entra External ID tenant:** Must be created (Phase 0)

### Enablers for Other Projects
- **Project 23 (Parental Consent):** Phases 3 and 7 of this project implement Project 23's core work
- **Project 10 (Community Pages):** Partner SSO is nice-to-have
- **Project 8 (Waivers V3):** Benefits from improved auth flows

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Scarce Entra External ID expertise** | Medium | High | TrashMob is Privo's first Entra External ID partner; document everything; lean on Microsoft and Privo support |
| **Custom CSS not available** | Confirmed | Low | Built-in branding covers sign-in page; all other UX is in-app; request MS exception via Privo sponsorship |
| **User confusion during migration** | High | Medium | Clear communication; in-app notifications; help center updates |
| **Mobile app store approval delays** | Medium | Medium | Submit updates early; have rollback plan |
| **Data loss during migration** | Low | Critical | Comprehensive backups; dry-run migrations; validation scripts |
| **Privo integration delays** | Medium | Medium | Phases 0-2 can proceed without Privo; Phase 3 is gated on Privo contract |
| **JIT password migration failures** | Low | High | Pre-migration dry runs; fallback to password reset flow |

---

## Implementation Plan

### Data Model Changes (Phase 0 — Complete)
- **User table additions:**
  - `GivenName` (string, nullable, max 64) — first name for display and minor protections
  - `Surname` (string, nullable, max 64) — last name for display and minor protections
  - `DateOfBirth` (DateTimeOffset, nullable) — used for Privo age verification
  - `ProfilePhotoUrl` (string, nullable, max 500) — auto-populated from social IDP `picture` claim

### API Changes
- **Config endpoint (Phase 0 — Complete):** `/api/config` returns B2C or Entra config based on `UseEntraExternalId` flag
- **Auth binding (Phase 0 — Complete):** `Program.cs` binds to `AzureAdB2C` or `AzureAdEntra` based on feature flag
- **Auth endpoints:** Update JWT validation (authority URL, audience, claims mapping) — Phase 1
- **Profile photo:** `/api/auth/profile-photo` — Upload/delete profile photo — Phase 2
- **Profile edit:** In-app profile update via Microsoft Graph API — Phase 2
- **Account deletion:** In-app via Graph API (replaces B2C deregister policy) — Phase 2

### Web UX Changes
- **Profile photos (Phase 0 — Complete):** Display in UserNav trigger button and hover card
- **Auth guards (Phase 0 — Complete):** UserNav profile edit and DeleteMyData guarded for Entra mode
- **Sign-in page:** Uses Entra External ID built-in branding (logo, colors, background) — Phase 1
- **Profile photos in lists:** Event attendees, leaderboards — Phase 1
- **Profile edit page:** New in-app page replacing broken B2C policy — Phase 2
- **Photo upload:** Azure Blob Storage upload with avatar cropping — Phase 2

### Mobile App Changes
- Update MAUI MSAL config (`AuthConstants.cs`) for new Entra External ID tenant — Phase 6
- Handle profile photo display in app — Phase 6
- Coordinate app store updates with web cutover — Phase 6

---

## Open Questions

1. ~~**Does Entra External ID support all required social login providers?**~~
   **Decision:** Yes — Google, Microsoft, Apple, and Facebook are all supported via built-in social identity provider configuration.
   **Status:** ✅ Resolved

2. ~~**Entra External ID vs B2C final decision?**~~
   **Decision:** Entra External ID — B2C is maintenance-mode, multiple B2C policies are broken, and Privo sponsorship specifically targets Entra External ID.
   **Status:** ✅ Resolved (February 2026)

3. ~~**Can we grandfather existing users or require re-consent for ToS?**~~
   **Decision:** No grandfathering — new ToS will be required anyway, so all users must re-consent during or after migration.
   **Status:** ✅ Resolved (February 2026)

4. ~~**Do we need multi-factor auth for admins?**~~
   **Decision:** Not for initial launch — can be added later as Entra External ID supports MFA natively.
   **Status:** ✅ Resolved

5. ~~**How do we handle existing user sessions during migration cutover?**~~
   **Decision:** Not a concern — traffic volume is low enough that session disruption during migration is acceptable.
   **Status:** ✅ Resolved

6. ~~**What is the strategy for users with duplicate accounts across identity providers?**~~
   **Decision:** Detect during migration; prompt users to link accounts or choose primary; preserve data from both.
   **Status:** ✅ Resolved (February 2026)

7. ~~**How will auth changes impact API authentication for potential third-party integrations?**~~
   **Decision:** Not a concern — there are no third-party consumer apps using the API.
   **Status:** ✅ Resolved

8. ~~**Should we implement device/session management (view active sessions, logout all devices)?**~~
   **Decision:** Not needed — user base is not large enough to warrant this feature.
   **Status:** ✅ Resolved

9. ~~**Can we get custom CSS for the Entra External ID sign-in page?**~~
   **Decision:** Custom CSS cutoff was January 5, 2026; our tenant hasn't been created yet. Request exception from Microsoft given Privo sponsorship. Fall back to built-in branding if denied.
   **Status:** ✅ Resolved (workaround identified)

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2242](https://github.com/trashmob/TrashMob/issues/2242)** - Project 1: Revamp Sign Up / Sign-In Process (tracking issue)
- **[#2253](https://github.com/trashmob/TrashMob/issues/2253)** - Investigate Converting from AzureB2C to Entra External ID
- **[#2254](https://github.com/trashmob/TrashMob/issues/2254)** - Update the Graphics on the Sign In / Sign Up page
- **[#2255](https://github.com/trashmob/TrashMob/issues/2255)** - Add/Test current 3rd party integrations to EEID interface
- **[#2256](https://github.com/trashmob/TrashMob/issues/2256)** - Add integration with existing TrashMob APIs to Sign In Process
- **[#2257](https://github.com/trashmob/TrashMob/issues/2257)** - User can select home location during sign-up process
- **[#2258](https://github.com/trashmob/TrashMob/issues/2258)** - User can allow TrashMob to use their profile photo from 3rd party IDP
- **[#2259](https://github.com/trashmob/TrashMob/issues/2259)** - Allow user to upload a profile photo to TrashMob
- **[#2260](https://github.com/trashmob/TrashMob/issues/2260)** - Allow user to update/delete profile photo from TrashMob
- **[#2261](https://github.com/trashmob/TrashMob/issues/2261)** - Migrate current users from B2C to EEID
- **[#2262](https://github.com/trashmob/TrashMob/issues/2262)** - Allow TrashMob to set up Single Sign On with Partners
- **[#677](https://github.com/trashmob/TrashMob/issues/677)** - Add User Profile Images
- **[#948](https://github.com/trashmob/TrashMob/issues/948)** - Clean up B2C Redirects in prod
- **[#1015](https://github.com/trashmob/TrashMob/issues/1015)** - Convert to use Azure Graph for User Details
- **[#2321](https://github.com/trashmob/TrashMob/issues/2321)** - When trying to sign up with an id that already exists, the workflow hangs

---

## Related Documents

- **[Project 23 - Parental Consent](./Project_23_Parental_Consent.md)** - Age verification and minor protections (Phases 3 & 7 of this project)
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Partner SSO (nice-to-have)
- **[Project 8 - Waivers V3](./Project_08_Waivers_V3.md)** - Minor waiver handling
- **[Auth Migration Technical Design](../TechnicalDesigns/Auth_Migration.md)** - B2C → Entra External ID migration guide (Privo deliverable)

---

## Step-by-Step: Manual Portal Setup

### Step-by-Step: Tenant Setup

1. Go to [Azure Portal](https://portal.azure.com) → **Microsoft Entra External ID**
2. Click **+ Create a tenant** → select **Customer** tenant type
3. Configure:
   - **Tenant name:** `trashmobdev` (dev) or `trashmob` (prod)
   - **Domain name:** `trashmobdev.onmicrosoft.com`
   - **Location:** United States
4. Click **Create** — this takes 2-3 minutes
5. Record:
   - **Tenant ID** (GUID from Overview page)
   - **Primary domain** (e.g., `trashmobdev.onmicrosoft.com`)
   - **CIAM domain** (e.g., `trashmobdev.ciamlogin.com`)

### Step-by-Step: App Registrations

Create three app registrations in the new external tenant:

#### 1. Web SPA (Frontend)
1. Go to **App registrations** → **+ New registration**
2. **Name:** `TrashMob Web Dev`
3. **Supported account types:** Accounts in this organizational directory only
4. **Redirect URI:** Single-page application (SPA)
   - `http://localhost:3000` (local dev)
   - `https://dev.trashmob.eco` (dev environment)
5. Click **Register**
6. Go to **Authentication** → under **Implicit grant and hybrid flows**:
   - Check **ID tokens**
   - Uncheck **Access tokens**
7. Record the **Application (client) ID** — this is the `FrontendClientId`

#### 2. Backend API
1. **Name:** `TrashMob API Dev`
2. **Supported account types:** Accounts in this organizational directory only
3. **Redirect URI:** Leave blank (API doesn't redirect)
4. Click **Register**
5. Go to **Expose an API**:
   - Click **+ Add a scope**
   - Set **Application ID URI** to `api://<client-id>`
   - Add scope: `TrashMob.Read` (Admins and users)
   - Add scope: `TrashMob.Writes` (Admins and users)
6. Record the **Application (client) ID** — this is the `ClientId` in appsettings

#### 3. Mobile App
1. **Name:** `TrashMob Mobile Dev`
2. **Supported account types:** Accounts in this organizational directory only
3. **Redirect URI:** Public client/native (mobile & desktop)
   - `eco.trashmob.trashmobmobile://auth`
4. Click **Register**
5. Record the **Application (client) ID**

#### Grant API Permissions
For both the Web SPA and Mobile app registrations:
1. Go to **API permissions** → **+ Add a permission**
2. Select **My APIs** → select `TrashMob API Dev`
3. Check both `TrashMob.Read` and `TrashMob.Writes`
4. Click **Grant admin consent**

#### Update Config Values
After all registrations are complete, update the config:
- **`appsettings.Development.json`** → fill in `AzureAdEntra` section:
  - `Instance`: `https://trashmobdev.ciamlogin.com/`
  - `ClientId`: API app registration client ID
  - `FrontendClientId`: Web SPA app registration client ID
  - `Domain`: `trashmobdev.onmicrosoft.com`
  - `TenantId`: Tenant ID from Overview page
- **Key Vault** (`kv-tm-dev-westus2`): Store the same values as secrets for production use

### Step-by-Step: Social Identity Providers

For each provider (Google, Microsoft, Apple, Facebook):

#### Google
1. Go to [Google Cloud Console](https://console.cloud.google.com) → **APIs & Services** → **Credentials**
2. Create or use existing **OAuth 2.0 Client ID**
3. Add authorized redirect URI: `https://trashmobdev.ciamlogin.com/trashmobdev.onmicrosoft.com/federation/oauth2`
4. In Azure Portal → External tenant → **External Identities** → **All identity providers**
5. Click **+ Google** → enter Client ID and Client secret from Google

#### Microsoft
1. In Azure Portal → External tenant → **External Identities** → **All identity providers**
2. Microsoft account is available by default — just enable it

#### Apple
1. Go to [Apple Developer](https://developer.apple.com) → **Certificates, Identifiers & Profiles**
2. Create a **Services ID** for Sign in with Apple
3. Configure the return URL: `https://trashmobdev.ciamlogin.com/trashmobdev.onmicrosoft.com/federation/oauth2`
4. In Azure Portal → **External Identities** → **All identity providers** → **+ Apple**
5. Enter Apple Service ID, Team ID, Key ID, and private key

#### Facebook
1. Go to [Facebook Developers](https://developers.facebook.com) → create or use existing app
2. Under **Facebook Login** → **Settings**, add valid OAuth redirect URI:
   `https://trashmobdev.ciamlogin.com/trashmobdev.onmicrosoft.com/federation/oauth2`
3. In Azure Portal → **External Identities** → **All identity providers** → **+ Facebook**
4. Enter App ID and App secret from Facebook

### Step-by-Step: User Flow

1. Go to External tenant → **User flows** → **+ New user flow**
2. Select **Sign up and sign in**
3. **Name:** `SignUpSignIn` (the full name will be `B2C_1_SignUpSignIn`)
4. **Identity providers:** Select all configured providers (Google, Microsoft, Apple, Facebook, Email)
5. **User attributes to collect during sign-up:**
   - Email Address (required)
   - Given Name (required)
   - Surname (required)
   - Display Name (optional — maps to UserName)
6. **Custom attributes** (create if not present):
   - `dateOfBirth` (String) — collected during sign-up for age verification
7. Click **Create**
8. After creation, go to **Properties** → configure:
   - Token lifetime: 1 hour (access token), 24 hours (refresh token)
   - Session behavior: match current B2C settings

### Step-by-Step: Branding

1. Go to External tenant → **Company branding** → **Default sign-in experience**
2. Configure:
   - **Banner logo:** Upload TrashMob logo (260x36 px recommended, PNG/SVG)
   - **Background image:** Upload TrashMob hero image (1920x1080 px)
   - **Background color:** `#96ba00` (TrashMob green) as fallback
   - **Sign-in page text:** "Welcome to TrashMob.eco — Join the movement to clean up the planet!"
3. Under **Layout**:
   - Select **Full-screen background** template
4. Click **Save**
5. Test by navigating to the sign-in URL in an incognito browser

---

**Last Updated:** February 9, 2026
**Owner:** Security Engineer + Product Lead
**Status:** In Progress (Phase 0 — code complete, portal setup remaining)
**Next Review:** After Phase 0 tenant setup
