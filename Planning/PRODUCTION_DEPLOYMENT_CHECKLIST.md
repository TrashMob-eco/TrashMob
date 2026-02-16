# Production Deployment Checklist

**Last Updated:** February 12, 2026
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

### 4. Entra External ID — Auth Migration (Project 1)

This is a **downtime deployment** — B2C will be fully replaced by Entra External ID in one push. All steps below must be completed before merging to release.

**Reference:** See `Planning/Projects/Project_01_Auth_Revamp.md` for full details and `Planning/TechnicalDesigns/Auth_Migration.md` for technical architecture.

#### 4.1 Create Production Entra Tenant

- [ ] Go to Azure Portal → Microsoft Entra External ID → Create a **Customer** tenant
  - Tenant name: `TrashMobEco`
  - Domain: `trashmobeco.onmicrosoft.com`
  - CIAM domain will be: `trashmobeco.ciamlogin.com`
  - Location: United States
- [ ] Record the **Tenant ID** (GUID from Overview page)

#### 4.2 Register App Registrations (in the prod Entra tenant)

Login to the prod tenant first:
```bash
az login --tenant <prod-entra-tenant-id> --allow-no-subscriptions
```

**a. Web SPA (Frontend):**
- [ ] Name: `TrashMob Web`
- [ ] Redirect URIs (SPA): `https://www.trashmob.eco`, `https://trashmob.eco`
- [ ] Authentication → check ID tokens, uncheck Access tokens
- [ ] Record **Application (client) ID** → this is `FrontendClientId`

**b. Backend API:**
- [ ] Name: `TrashMob API`
- [ ] Expose an API → Application ID URI: `api://<client-id>`
- [ ] Add scopes: `TrashMob.Read`, `TrashMob.Writes`
- [ ] Record **Application (client) ID** → this is `ClientId` in appsettings

**c. Mobile App:**
- [ ] Name: `TrashMob Mobile`
- [ ] Redirect URI (Public client): `eco.trashmob.trashmobmobile://auth`
- [ ] Record **Application (client) ID**

**d. Auth Extension (Layer 2):**
- [ ] Name: `TrashMob AuthExtension`
- [ ] Record **Application (client) ID** → used for JWT audience validation

**e. Grant API Permissions:**
- [ ] Web SPA → API permissions → add `TrashMob.Read` + `TrashMob.Writes` → Grant admin consent
- [ ] Mobile App → API permissions → add `TrashMob.Read` + `TrashMob.Writes` → Grant admin consent

#### 4.3 Configure Optional Claims

Run the configure script (update `configure-entra-apps.ps1` with prod values first):
```bash
az login --tenant <prod-entra-tenant-id> --allow-no-subscriptions
.\Deploy\configure-entra-apps.ps1 -Environment pr
```

This sets on all app registrations:
- Optional claims: `email`, `given_name`, `family_name`, `preferred_username`
- `acceptMappedClaims: true`
- `isFallbackPublicClient: true` (mobile only)

#### 4.4 Configure Social Identity Providers

In Azure Portal → prod Entra tenant → External Identities → All identity providers:

- [ ] **Google:** Create OAuth 2.0 credentials in Google Cloud Console, add redirect URI `https://trashmobeco.ciamlogin.com/trashmobeco.onmicrosoft.com/federation/oauth2`, enter Client ID + secret in Azure
- [ ] **Facebook:** Add OAuth redirect URI in Facebook Developer Console, enter App ID + secret in Azure
- [ ] **Apple:**
  1. Go to [Apple Developer](https://developer.apple.com/account/resources/identifiers/list/serviceId) → Certificates, Identifiers & Profiles → Identifiers → **+** → **Services IDs**
  2. Register a new Service ID (e.g., `eco.trashmob.entra`) with **Sign In with Apple** enabled
  3. Configure return URL: `https://trashmobeco.ciamlogin.com/trashmobeco.onmicrosoft.com/federation/oidc/apple`
  4. If you don't have a `.p8` key file, create a new key under **Keys** → **+** → check **Sign in with Apple** → download the `.p8` file immediately (one-time download)
  5. Generate the Apple client secret JWT using `d:/tools/Apple/generate-apple-secret.js` — update `KEY_ID`, `SERVICE_ID`, and `KEY_FILE` for prod values, then run `node generate-apple-secret.js`
  6. Enter the Service ID (as Client ID) and generated JWT (as Client Secret) in the Entra Apple IDP configuration
  7. **Note:** The client secret expires after 6 months — set a calendar reminder to regenerate it
- [ ] **Microsoft:** Enabled by default in Entra External ID — just verify it's active

#### 4.5 Create User Flow with dateOfBirth

- [ ] User flows → New user flow → "Sign up and sign in"
  - Name: `SignUpSignIn`
  - Identity providers: all configured (Google, Microsoft, Apple, Facebook, Email)
  - Attributes to collect: Email (required), Given Name (required), Surname (required)
- [ ] Create custom attribute: External Identities → Custom user attributes → Add `dateOfBirth` (String type)
- [ ] Add `dateOfBirth` to the user flow's attribute collection page

#### 4.6 Configure Token Claims

For each app registration (Web SPA, API, Mobile):
- [ ] Token configuration → Add optional claims (ID + Access tokens):
  - `given_name`, `family_name`, `email` (built-in)
  - `dateOfBirth` (directory schema extension source)
- [ ] Verify `acceptMappedClaims: true` in Manifest
- [ ] Test: sign in and decode JWT at https://jwt.ms to verify claims

#### 4.7 Configure Branding

- [ ] Company branding → Default sign-in experience:
  - Banner logo: TrashMob logo (260x36 px)
  - Background image: TrashMob hero image (1920x1080 px)
  - Background color: `#96ba00`
  - Sign-in text: "Welcome to TrashMob.eco — Join the movement to clean up the planet!"
  - Layout: Full-screen background template
- [ ] Test in incognito browser

#### 4.8 User Migration (B2C → Entra)

- [ ] Run migration script to export B2C users → import to Entra External ID
  - Reference: `Deploy/migrate-b2c-users.ps1` (dev version — update for prod)
- [ ] Verify migrated user count matches B2C
- [ ] Test sign-in with a few migrated accounts
- [ ] Existing users without `DateOfBirth` are grandfathered as adults (no migration needed for DOB)

#### 4.9 Update Production Configuration

**a. Backend config (Key Vault or environment variables):**
```
AzureAdEntra__Instance=https://trashmobeco.ciamlogin.com/
AzureAdEntra__ClientId=<API app client ID>
AzureAdEntra__FrontendClientId=<Web SPA client ID>
AzureAdEntra__Domain=trashmobeco.onmicrosoft.com
AzureAdEntra__TenantId=<prod tenant ID>
UseEntraExternalId=true
```

**b. Frontend config:** The `/api/config` endpoint returns auth config dynamically — verify it returns `authProvider: "entra"` with correct prod Entra values after deployment.

**c. Update `Deploy/containerApp.bicep`** prod environment variables with prod Entra values.

**d. Update `Deploy/configure-entra-apps.ps1`** with prod app registration IDs.

#### 4.10 Deploy Auth Extension Container App (Layer 2)

**a. Set GitHub Actions secrets** (in the `production` environment):
```
ENTRA_TENANT_ID=<prod-entra-tenant-id>
AUTH_EXTENSION_CLIENT_ID=<auth-extension-app-client-id>
```

**b. Create production workflow:**
- [ ] Copy `.github/workflows/container_ca-authext-tm-dev-westus2.yml` → `release_ca-authext-tm-pr-westus2.yml`
- [ ] Update environment variables: registry `acrtmprwestus2`, container `ca-authext-tm-pr-westus2`, resource group `rg-trashmob-pr-westus2`
- [ ] Trigger on push to `release` branch

**c. Register Custom Authentication Extension in Entra portal:**
- [ ] External Identities → Custom authentication extensions → Create
- [ ] Type: `OnAttributeCollectionSubmit`
- [ ] Target URL: `https://ca-authext-tm-pr-westus2.<fqdn>/api/authext/attributecollectionsubmit`
- [ ] Link to auth extension app registration
- [ ] Assign to user flow's "When a user submits their information" event

#### 4.11 Mobile App Update

- [ ] Verify `AuthConstants.cs` has correct prod Entra values (or uses config-driven approach)
- [ ] Build and test on Android emulator + iOS simulator with prod tenant
- [ ] Submit to Google Play Store and Apple App Store
- [ ] Consider force-update flow for users on old B2C version

#### 4.12 Pre-Cutover Verification (on dev.trashmob.eco)

- [ ] **Web sign-in** via email/password → succeeds, JWT contains expected claims
- [ ] **Web sign-in** via Google → succeeds, profile photo populated
- [ ] **Web sign-in** via Facebook → succeeds
- [ ] **Web "Create Account"** → shows age gate, blocks under-13, allows 13+
- [ ] **Web "Sign In"** → goes directly to Entra (no age gate)
- [ ] **Web "Attend" (unauthenticated)** → shows age gate before redirect
- [ ] **Mobile sign-in** → Entra External ID (not B2C)
- [ ] **Mobile "Create Account"** → AgeGatePage → blocks under-13
- [ ] **Auth extension** → POST with under-13 DOB returns `showBlockPage`
- [ ] **Profile edit** → in-app edit works (name, photo upload)
- [ ] **Account deletion** → "Delete My Data" works with typed confirmation
- [ ] **Auto-create user** → new sign-up creates DB user from token claims
- [ ] **Migrated user sign-in** → existing B2C user signs in via Entra successfully

---

## Deployment Steps

### Auth Migration Cutover Window

**Timing:** Schedule a maintenance window (low-traffic period). Communicate to users in advance.

**Cutover sequence:**
1. Complete all pre-deployment tasks above (sections 0-4)
2. Final B2C → Entra user migration (catch any new users since last migration)
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
