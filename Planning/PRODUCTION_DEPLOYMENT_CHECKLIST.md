# Production Deployment Checklist

**Last Updated:** February 22, 2026
**Commits Since Last Release:** ~100+ commits from main

---

## Pre-Deployment Tasks

### 0. Key Vault Secrets (If Not Already Created)

Ensure required secrets exist before deployments:

**Dev environment:**
```bash
# Strapi database password (required for Strapi Azure SQL)
az keyvault secret show --vault-name kv-tm-dev-westus2 --name strapi-db-password || \
  az keyvault secret set --vault-name kv-tm-dev-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"

# Anthropic API key (required for AI prospect discovery - Project 40 Phase 2)
# Get key from https://console.anthropic.com/settings/keys
az keyvault secret show --vault-name kv-tm-dev-westus2 --name AnthropicApiKey || \
  az keyvault secret set --vault-name kv-tm-dev-westus2 --name AnthropicApiKey --value "<your-anthropic-api-key>"
```

**Production environment:**
```bash
# Strapi database password (required for Strapi Azure SQL)
az keyvault secret show --vault-name kv-tm-pr-westus2 --name strapi-db-password || \
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"

# Anthropic API key (required for AI prospect discovery - Project 40 Phase 2)
# Get key from https://console.anthropic.com/settings/keys
az keyvault secret show --vault-name kv-tm-pr-westus2 --name AnthropicApiKey || \
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name AnthropicApiKey --value "<your-anthropic-api-key>"
```

### 1. Database Migrations (REQUIRED)

Run all 4 pending migrations in order:

```bash
# From TrashMob folder, connected to production database
dotnet ef database update
```

**Migrations to apply:**
1. `20260131222334_AddEventCoLeads` - Adds IsEventLead column to EventAttendees, backfills event creators as leads
2. `20260201013924_ChangePickedWeightToDecimal` - Changes EventSummaries.PickedWeight from int to decimal(10,1)
3. `20260201030122_AddTeamsFeature` - Creates Teams, TeamMembers, TeamEvents, TeamJoinRequests, TeamPhotos tables
4. `20260201164845_AddUserFeedback` - Creates UserFeedback table for in-app feedback
5. `20260202000411_AddCommunityPhase2Fields` - Adds LogoUrl, ContactEmail, ContactPhone, PhysicalAddress to Partner (Community)

**Note:** Migration #1 includes data backfill SQL that marks all event creators as event leads.

### 2. Infrastructure Deployments

#### 2.1 Application Insights Workbook (Recommended)
```bash
az account set --subscription "TrashMobProd"

az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/appInsightsWorkbook.bicep \
  --parameters environment=pr region=westus2 \
    rgName=rg-trashmob-pr-westus2 \
    appInsightsName=ai-tm-pr-westus2
```

#### 2.2 Backup Alerts (If not already deployed)
```bash
az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/backupAlerts.bicep \
  --parameters environment=pr region=westus2
```

#### 2.3 Billing Alerts & Budget Caps (Project 30)

**a. Deploy action group (Bicep):**
```bash
az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/billingAlerts.bicep \
  --parameters environment=pr region=westus2
```

**b. Manual steps (one-time — budget APIs don't support sponsorship subscriptions):**

- [ ] **Azure budgets:** Create budgets manually in Azure Portal > Cost Management > Budgets (see `Deploy/COST_ALERT_RUNBOOK.md` for step-by-step):
  - Monthly budget ($500) with alerts at 50%, 75%, 90%, 100%
  - Annual grant monitor ($1) to detect grant expiration
- [ ] **SendGrid alerts:** Log into https://app.sendgrid.com > Settings > Billing > Add alerts at 75% and 90% of monthly email limit (recipient: joe@trashmob.eco)
- [ ] **Google Maps API alerts:** Log into https://console.cloud.google.com > Billing > Budgets & alerts > Create budget "TrashMob Maps API" at $100/month with 50%, 90%, 100% thresholds

See `Deploy/COST_ALERT_RUNBOOK.md` for full response procedures and monthly review checklist.

#### 2.4 KeyVault RBAC (Already Completed)
PR #2482 migrated KeyVault from access policies to RBAC. Verify this is working correctly.

### 3. Strapi CMS (Optional)

If deploying Strapi to production for the first time:

**a. Create KeyVault secrets:**
```bash
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-admin-jwt-secret --value "$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-api-token-salt --value "$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-app-keys --value "$(openssl rand -base64 32),$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-transfer-token-salt --value "$(openssl rand -base64 32)"
```

**b. Deploy Strapi database:**
```bash
az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/sqlDatabaseStrapi.bicep \
  --parameters environment=pr region=westus2
```

**c. Create production workflow** (copy from dev and update environment variables)

**Note:** The workflow automatically creates the SQL user and deploys the container.

### 4. Entra External ID — Auth Migration (Project 1) -- COMPLETED

Production cutover completed February 22, 2026. All steps below document the completed migration for reference.

**Reference:** See `Planning/Projects/Project_01_Auth_Revamp.md` for full details and `Planning/TechnicalDesigns/Auth_Migration.md` for technical architecture.

#### 4.1 Create Production Entra Tenant -- COMPLETED

- [x] Created Entra External ID **Customer** tenant
  - Tenant name: `TrashMobEcoPr`
  - Domain: `trashmobecopr.onmicrosoft.com`
  - CIAM domain: `trashmobecopr.ciamlogin.com`
  - Location: United States
- [x] **Tenant ID:** `b5fc8717-29eb-496e-8e09-cf90d344ce9f`

#### 4.2 Register App Registrations (in the prod Entra tenant) -- COMPLETED

Login to the prod tenant first:
```bash
az login --tenant b5fc8717-29eb-496e-8e09-cf90d344ce9f --allow-no-subscriptions
```

**a. Web SPA (Frontend):**
- [x] Name: `TrashMob Web`
- [x] Redirect URIs (SPA): `https://www.trashmob.eco`, `https://trashmob.eco`
- [x] Authentication: check ID tokens, uncheck Access tokens
- [x] **FrontendClientId:** `0604ef02-6b84-450f-b5d5-2196e96f3b48`

**b. Backend API:**
- [x] Name: `TrashMob API`
- [x] Expose an API: Application ID URI: `https://trashmobecopr.onmicrosoft.com/api`
- [x] Add scopes: `TrashMob.Read`, `TrashMob.Writes`
- [x] **ClientId:** `dc09e17b-bce4-4af9-82ab-f7b12af586b4`

**CRITICAL:** Supported account types must be **"Single tenant only"** (Accounts in this organizational directory only). Using multi-tenant causes `AADSTS500207` errors during token validation.

**c. Mobile App:**
- [x] Name: `TrashMob Mobile`
- [x] Redirect URI (Public client): `eco.trashmob.trashmobmobile://auth`
- [x] Recorded Application (client) ID

**d. Auth Extension (Layer 2):**
- [x] Name: `TrashMob AuthExtension`
- [x] Recorded Application (client) ID for JWT audience validation

**e. Grant API Permissions:**
- [x] Web SPA: `TrashMob.Read` + `TrashMob.Writes` with admin consent
- [x] Mobile App: `TrashMob.Read` + `TrashMob.Writes` with admin consent

#### 4.3 Configure Optional Claims -- COMPLETED

Ran the configure script with prod values:
```bash
az login --tenant b5fc8717-29eb-496e-8e09-cf90d344ce9f --allow-no-subscriptions
.\Deploy\configure-entra-apps.ps1 -Environment pr
```

This sets on all app registrations:
- Optional claims: `email`, `given_name`, `family_name`, `preferred_username`
- `acceptMappedClaims: true`
- `isFallbackPublicClient: true` (mobile only)

**CRITICAL -- CIAM Token Claim Behavior:**

The `email` claim behaves differently in Entra External ID (CIAM) compared to standard Azure AD:

| Context | Email Claim Available? | Reason |
|---------|----------------------|--------|
| **id_token** | NO | CIAM stores email in the `identities` collection, not the `mail` property. The `email` optional claim reads from `mail`, so it emits as empty/missing. |
| **access_token** | YES | Access tokens include email from the identity provider claims. |
| **Frontend (idTokenClaims)** | NO | MSAL.js reads from `idTokenClaims`, so email is unavailable on the frontend. |
| **Backend (access token)** | YES | The API validates access tokens, so email IS available in backend claims. |

**Impact:** The frontend must fall back to `oid` (Object ID) for user identification instead of email. The backend can resolve users via email from the access token. See sections 4.10 and 4.11 for the Graph API and OID-based resolution patterns that address this.

#### 4.4 Configure Social Identity Providers -- COMPLETED

In Azure Portal, prod Entra tenant, External Identities, All identity providers:

- [x] **Google:** OAuth 2.0 credentials configured with redirect URI `https://trashmobecopr.ciamlogin.com/trashmobecopr.onmicrosoft.com/federation/oauth2`
- [x] **Facebook:** OAuth redirect URI configured
- [x] **Apple:** Service ID with Sign In with Apple, return URL `https://trashmobecopr.ciamlogin.com/trashmobecopr.onmicrosoft.com/federation/oidc/apple`, client secret JWT generated (expires after 6 months -- set calendar reminder)
- [x] **Microsoft:** Enabled by default in Entra External ID

#### 4.5 Create User Flow with dateOfBirth -- COMPLETED

- [x] User flow: "Sign up and sign in" named `SignUpSignIn`
  - Identity providers: all configured (Google, Microsoft, Apple, Facebook, Email)
  - Attributes to collect: Email (required), Given Name (required), Surname (required)
- [x] Custom attribute: `dateOfBirth` (String type) created and added to user flow

#### 4.6 Configure Token Claims -- COMPLETED

For each app registration (Web SPA, API, Mobile):
- [x] Token configuration: optional claims (ID + Access tokens): `given_name`, `family_name`, `email`, `dateOfBirth`
- [x] `acceptMappedClaims: true` verified in Manifest
- [x] Tested: sign in and decoded JWT at https://jwt.ms to verify claims (see 4.3 for email claim caveats)

#### 4.7 Configure Branding -- COMPLETED

- [x] Company branding configured:
  - Banner logo: TrashMob logo (260x36 px)
  - Background image: TrashMob hero image (1920x1080 px)
  - Background color: `#96ba00`
  - Sign-in text: "Welcome to TrashMob.eco -- Join the movement to clean up the planet!"
  - Layout: Full-screen background template
- [x] Tested in incognito browser

#### 4.8 User Migration (B2C to Entra) -- PARTIAL

- [ ] Bulk migration script not run -- users migrate automatically on first CIAM sign-in via OID auto-linking (see section 4.11)
- [x] Existing users without `DateOfBirth` are grandfathered as adults (no migration needed for DOB)
- [x] OID auto-linking: when a user signs in via CIAM for the first time, the auth handler looks up their email, finds the existing DB user, and writes the CIAM OID to the user record for future lookups

#### 4.9 Update Production Configuration -- COMPLETED

**a. Backend config (Key Vault or environment variables):**
```
AzureAdEntra__Instance=https://trashmobecopr.ciamlogin.com/
AzureAdEntra__ClientId=dc09e17b-bce4-4af9-82ab-f7b12af586b4
AzureAdEntra__FrontendClientId=0604ef02-6b84-450f-b5d5-2196e96f3b48
AzureAdEntra__Domain=trashmobecopr.onmicrosoft.com
AzureAdEntra__TenantId=b5fc8717-29eb-496e-8e09-cf90d344ce9f
UseEntraExternalId=true
```

**b. Frontend config:** The `/api/config` endpoint returns auth config dynamically -- verified it returns `authProvider: "entra"` with correct prod Entra values after deployment.

**c. Updated `Deploy/containerApp.bicep`** prod environment variables with prod Entra values.

**d. Updated `Deploy/configure-entra-apps.ps1`** with prod app registration IDs.

#### 4.10 CIAM Graph API Service -- COMPLETED

Because CIAM does not emit the `email` claim in id_tokens (see section 4.3), a Graph API service was implemented to resolve user emails server-side. This service queries Microsoft Graph for user profile information including email addresses.

**Setup commands (run in prod Entra tenant):**
```bash
# Login to the CIAM tenant
az login --tenant b5fc8717-29eb-496e-8e09-cf90d344ce9f --allow-no-subscriptions

# Grant Microsoft Graph User.Read.All application permission to the Backend API app
az ad app permission add \
  --id dc09e17b-bce4-4af9-82ab-f7b12af586b4 \
  --api 00000003-0000-0000-c000-000000000000 \
  --api-permissions df021288-bdef-4463-88db-98f22de89214=Role

# Grant admin consent for the permission
az ad app permission admin-consent --id dc09e17b-bce4-4af9-82ab-f7b12af586b4

# Create a client secret for the Backend API app (used by CiamGraphService)
az ad app credential reset \
  --id dc09e17b-bce4-4af9-82ab-f7b12af586b4 \
  --append \
  --display-name "CiamGraphService" \
  --years 2

# Switch back to the Azure subscription context
az login --tenant f0062da2-b273-427c-b99e-6e85f75b23eb
az account set --subscription "TrashMobProd"

# Store the client secret in Key Vault
az keyvault secret set \
  --vault-name kv-tm-pr-westus2 \
  --name "AzureAdEntra--ClientSecret" \
  --value "<secret>"
```

**Implementation:** `CiamGraphService.cs` queries Microsoft Graph to extract the user's email from one of three locations:
1. The `mail` property
2. The `otherMails` collection
3. The `identities` collection (where CIAM stores email sign-up addresses)

**Graceful degradation:** When `AzureAdEntra:ClientSecret` is not configured (e.g., in local development), the Graph API service is gracefully disabled and the auth handler falls back to token-only claims for user resolution.

#### 4.11 Auth Handler -- OID-Based User Resolution -- COMPLETED

Because email is unavailable in CIAM id_tokens on the frontend, the auth handler implements a 4-step user resolution process:

**Backend (AuthorizationHandler / UserManager):**

1. **Email lookup + OID auto-linking:** Look up user by email from the access token. If found and the user's OID field is empty, write the CIAM OID to the user record for future lookups. This handles the first sign-in for migrated B2C users.
2. **OID lookup:** Look up user by Object ID (`oid` claim). This handles subsequent sign-ins after the OID has been linked.
3. **Graph API email resolution:** If neither email nor OID matches, call CiamGraphService to resolve the user's email from Microsoft Graph, then retry the email lookup. This handles edge cases where the access token email claim is also missing.
4. **Auto-create:** If no existing user is found by any method, create a new user record from the token claims (email, name, OID).

**Frontend changes (`useLogin.ts`):**

- `fetchUser`: Attempts to fetch the current user by email first; if that returns 404, falls back to fetching by OID (`oid` claim from `idTokenClaims`)
- `isUserLoaded`: Uses `id !== EMPTY_GUID` instead of checking email, since email may be unavailable from CIAM id_tokens
- `validateToken`: Accepts either email or OID as a valid user identifier (previously required email)

#### 4.12 Production Cutover Verification

**Completed:**
- [x] Web sign-in via email/password works
- [x] New user auto-created in DB on first sign-in (via OID-based resolution)

**Remaining to test:**
- [ ] Web sign-in via Google works, profile photo auto-populated
- [ ] Web sign-in via Facebook works
- [ ] Web sign-in via Apple works
- [ ] "Create Account" shows age gate before Entra redirect
- [ ] Age gate blocks under-13 with friendly message
- [ ] "Sign In" goes directly to Entra (no age gate)
- [ ] Profile edit works in-app (name, photo upload)
- [ ] "Delete My Data" works with typed DELETE confirmation
- [ ] Migrated B2C user can sign in via Entra (OID auto-linking)
- [ ] Auth extension blocks under-13 sign-up server-side
- [ ] JWT contains expected claims: given_name, family_name, dateOfBirth (email in access_token only, see 4.3)
- [ ] No auth errors in Application Insights after 1 hour
- [ ] Mobile sign-in via Entra External ID

#### 4.13 Mobile App Update

- [ ] Verify `AuthConstants.cs` has correct prod Entra values (or uses config-driven approach)
- [ ] Build and test on Android emulator + iOS simulator with prod tenant
- [ ] Submit to Google Play Store and Apple App Store
- [ ] Consider force-update flow for users on old B2C version

---

## Deployment Steps

### Auth Migration Cutover Window

**Timing:** Schedule a maintenance window (low-traffic period). Communicate to users in advance.

**Note:** Production cutover was completed on February 22, 2026. The steps below are retained for reference.

**Cutover sequence:**
1. Complete all pre-deployment tasks above (sections 0-4)
2. Final B2C to Entra user migration (catch any new users since last migration)
3. Merge main to release (triggers deployment)
4. Verify Entra sign-in works on www.trashmob.eco
5. Monitor Application Insights for auth errors for 24 hours
6. If critical issues: rollback (see below)
7. After 1-week coexistence: decommission B2C tenant

---

## Deployment Steps

### 1. Merge main to release
```bash
git checkout release
git pull origin release
git merge origin/main
git push origin release
```

### 2. Wait for CI/CD Pipeline
- Container builds will trigger automatically
- Monitor GitHub Actions for success

### 3. Verify Deployment
- Check https://www.trashmob.eco is accessible
- Test login/logout functionality
- Verify Teams feature is working
- Verify Feedback widget is visible
- Check Application Insights for errors

---

## Post-Deployment Verification

### Feature Testing Checklist

- [ ] **Teams Feature (Project 9)**
  - [ ] Create a new team
  - [ ] Join an existing team
  - [ ] View Teams map
  - [ ] Upload team photo
  - [ ] Upload team logo
  - [ ] Associate event with team

- [ ] **User Feedback (Project 34)**
  - [ ] Feedback widget visible in bottom-right corner
  - [ ] Submit test feedback
  - [ ] Admin can view feedback at /siteadmin/feedback

- [ ] **Event Co-Leads (Project 21)**
  - [ ] Event creator marked as lead
  - [ ] Can add co-leads to events

- [ ] **Weight Tracking (Project 7)**
  - [ ] Event summary accepts decimal weights
  - [ ] Weight units dropdown works

- [ ] **Litter Reports (Project 3)**
  - [ ] Create litter report with photos
  - [ ] Litter reports appear on home map
  - [ ] Admin litter reports page works

- [ ] **Feature Metrics (Project 29)**
  - [ ] Login/logout events tracked
  - [ ] Event creation tracked
  - [ ] Attendance registration tracked

- [ ] **Community Pages (Project 10)**
  - [ ] Communities discovery page at /communities
  - [ ] Community detail page at /communities/{slug}
  - [ ] Community map shows events, teams, litter reports
  - [ ] Stats widget shows community impact metrics
  - [ ] Contact card displays email/phone/address
  - [ ] Events and Teams sections display nearby data

- [ ] **Job Opportunities Markdown Editor (Issue #2215)**
  - [ ] Admin can create/edit job opportunities with markdown
  - [ ] Preview toggle works in admin forms
  - [ ] **Convert existing job listings to markdown format:**
    - Go to /siteadmin/job-opportunities
    - Edit each active job opportunity
    - Convert HTML to markdown:
      - `<strong>text</strong>` or `<b>text</b>` → `**text**`
      - `<em>text</em>` or `<i>text</i>` → `*text*`
      - `<ul><li>item</li></ul>` → `- item`
      - `<ol><li>item</li></ol>` → `1. item`
      - `<h2>heading</h2>` → `## heading`
      - `<br>` or `<br/>` → blank line
      - `<p>text</p>` → text with blank line before/after
    - Use Preview toggle to verify formatting before saving
  - [ ] Volunteer opportunities page renders markdown correctly

- [ ] **Auth Migration — Entra External ID (Project 1)**
  - [ ] Sign in via email/password works
  - [ ] Sign in via Google works, profile photo auto-populated
  - [ ] Sign in via Facebook works
  - [ ] Sign in via Apple works
  - [ ] "Create Account" shows age gate before Entra redirect
  - [ ] Age gate blocks under-13 with friendly message
  - [ ] "Sign In" goes directly to Entra (no age gate)
  - [ ] Profile edit works in-app (name, photo upload)
  - [ ] "Delete My Data" works with typed DELETE confirmation
  - [ ] Migrated B2C user can sign in via Entra
  - [ ] New user auto-created in DB on first sign-in
  - [ ] Auth extension blocks under-13 sign-up server-side (test by navigating directly to Entra sign-up URL)
  - [ ] JWT contains expected claims: email, given_name, family_name, dateOfBirth
  - [ ] No auth errors in Application Insights after 1 hour

---

## Rollback Plan

If issues occur:

### Quick Rollback (< 5 min)
```bash
# List available revisions
az containerapp revision list \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --output table

# Activate previous revision
az containerapp revision activate \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --revision <previous-revision-name>

# Route traffic to previous revision
az containerapp ingress traffic set \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --revision-weight <previous-revision-name>=100
```

### Auth Rollback (B2C Fallback)
If Entra External ID has critical issues after cutover:

**Note:** As of February 22, 2026, B2C has been replaced by Entra External ID in production. Rolling back would require setting `UseEntraExternalId=false` in Container App environment variables to revert to the B2C auth handler. This should only be done as a last resort, as users who have already signed in via CIAM and had their OIDs linked would need to re-authenticate through B2C.

1. Set `UseEntraExternalId=false` in Container App env vars (reverts to B2C)
2. Redeploy Container App with B2C config
3. Mobile users on old app version still use B2C — no action needed
4. Mobile users on new app version cannot fallback — must wait for fix or app store update

### Database Rollback (If needed)
Database migrations do NOT have automatic rollback. If critical issues:
1. Restore from backup (Azure SQL automatic backups)
2. Or manually run Down() migration scripts

---

## New Features Summary

| Feature | Project | Description |
|---------|---------|-------------|
| Teams | Project 9 | User-created teams with membership, photos, events |
| Community Pages | Project 10 | Discovery, detail pages with map, stats, events, teams |
| User Feedback | Project 34 | In-app feedback widget + admin dashboard |
| Event Co-Leads | Project 21 | Multiple leads per event |
| Weight Tracking | Project 7 | Decimal weights, weight units |
| Litter Reports Web | Project 3 | Full web parity for litter reporting |
| Feature Metrics | Project 29 | Application Insights event tracking |
| OpenTelemetry | Project 27 | Migrated from App Insights SDK |
| KeyVault RBAC | Project 26 | Migrated from access policies |
| Database Backups | Project 32 | Configured retention policies |
| Billing Alerts | Project 30 | Azure budget caps, grant monitor, cost runbook |
| Job Opportunities Markdown | Issue #2215 | Markdown editor for job listings admin |
| Auth Migration (B2C → Entra) | Project 1 | Entra External ID sign-in, profile photos, social IDPs |
| Age Gate (COPPA) | Project 1/23 | Under-13 block (Layer 1 client + Layer 2 server), minor flagging |
| Auth Extension | Project 1 | Server-side age verification Container App for Entra |

---

## Contacts

- **Engineering:** @JoeBergeron
- **Emergency:** Check #engineering in Slack

---

**Remember:** Test in dev first! https://dev.trashmob.eco
