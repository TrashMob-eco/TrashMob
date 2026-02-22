# Production Deployment Checklist

**Last Updated:** February 22, 2026
**Commits Since Last Release:** ~100+ commits from main

> **Legend:**
> - :gear: **AUTOMATED** — Handled by CI/CD pipeline (GitHub Actions). Verify it succeeds; no manual action needed.
> - :hand: **MANUAL** — Requires a human to perform in a portal, CLI, or console.
> - :test_tube: **VERIFY** — Manual verification / testing step.

---

## Part A — Pre-Deployment (Do These First)

Everything in this section can and should be completed **before** merging to release.

---

### A1. Key Vault Secrets :hand:

Ensure required secrets exist in both environments. These are idempotent — safe to re-run.

- [x] **1.** Verify/create Strapi DB password (dev):
  ```bash
  az keyvault secret show --vault-name kv-tm-dev-westus2 --name strapi-db-password || \
    az keyvault secret set --vault-name kv-tm-dev-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
  ```
- [x] **2.** Verify/create Anthropic API key (dev):
  ```bash
  az keyvault secret show --vault-name kv-tm-dev-westus2 --name AnthropicApiKey || \
    az keyvault secret set --vault-name kv-tm-dev-westus2 --name AnthropicApiKey --value "<your-anthropic-api-key>"
  ```
  Get key from https://console.anthropic.com/settings/keys
- [x] **3.** Verify/create Strapi DB password (prod):
  ```bash
  az keyvault secret show --vault-name kv-tm-pr-westus2 --name strapi-db-password || \
    az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
  ```
- [x] **4.** Verify/create Anthropic API key (prod):
  ```bash
  az keyvault secret show --vault-name kv-tm-pr-westus2 --name AnthropicApiKey || \
    az keyvault secret set --vault-name kv-tm-pr-westus2 --name AnthropicApiKey --value "<your-anthropic-api-key>"
  ```

---

### A2. Infrastructure Deployments :hand:

- [x] **5.** Deploy Application Insights Workbook (recommended):
  ```bash
  az account set --subscription "TrashMobProd"
  az deployment group create \
    --resource-group rg-trashmob-pr-westus2 \
    --template-file Deploy/appInsightsWorkbook.bicep \
    --parameters environment=pr region=westus2 \
      rgName=rg-trashmob-pr-westus2 \
      appInsightsName=ai-tm-pr-westus2
  ```
- [x] **6.** Deploy Backup Alerts (if not already deployed):
  ```bash
  az deployment group create \
    --resource-group rg-trashmob-pr-westus2 \
    --template-file Deploy/backupAlerts.bicep \
    --parameters environment=pr region=westus2
  ```
- [x] **7.** Deploy Billing Alerts action group:
  ```bash
  az deployment group create \
    --resource-group rg-trashmob-pr-westus2 \
    --template-file Deploy/billingAlerts.bicep \
    --parameters environment=pr region=westus2
  ```
- [ ] **8.** Create Azure budgets manually (budget APIs don't support sponsorship subscriptions):
  - Azure Portal > Cost Management > Budgets — see `Deploy/COST_ALERT_RUNBOOK.md`
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

### A3. Strapi CMS (Optional — only if deploying to prod for first time) :hand:

- [x] **12.** Create KeyVault secrets for Strapi: — **Already exist in prod KV (verified 2026-02-20)**
  ```bash
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-admin-jwt-secret --value "$(openssl rand -base64 32)"
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-api-token-salt --value "$(openssl rand -base64 32)"
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-app-keys --value "$(openssl rand -base64 32),$(openssl rand -base64 32)"
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-transfer-token-salt --value "$(openssl rand -base64 32)"
  ```
- [x] **13.** Deploy Strapi database:
  ```bash
  az deployment group create \
    --resource-group rg-trashmob-pr-westus2 \
    --template-file Deploy/sqlDatabaseStrapi.bicep \
    --parameters environment=pr region=westus2
  ```
- [x] **14.** Create production workflow (copy from dev, update environment variables). Workflow auto-creates SQL user and deploys container. — **Created `.github/workflows/release_strapi-tm-pr-westus2.yml`**

---

### A4. Entra External ID — Auth Migration (Project 1) :hand:

This is a **downtime deployment** — B2C is fully replaced by Entra External ID. Complete ALL steps below before merging to release.

**Reference:** `Planning/Projects/Project_01_Auth_Revamp.md` and `Planning/TechnicalDesigns/Auth_Migration.md`

#### A4.1 Create Production Entra Tenant

- [x] **15.** Azure Portal > Microsoft Entra External ID > Create **Customer** tenant
  - Tenant name: `TrashMobEcoPr`
  - Domain: `trashmobecopr.onmicrosoft.com`
  - CIAM domain: `trashmobecopr.ciamlogin.com`
  - Location: United States
- [x] **16.** Record the **Tenant ID**: `b5fc8717-29eb-496e-8e09-cf90d344ce9f`

#### A4.2 Register App Registrations

Login to the prod tenant first: `az login --tenant b5fc8717-29eb-496e-8e09-cf90d344ce9f --allow-no-subscriptions`

- [x] **17.** **Web SPA (Frontend):** Name: `TrashMob Web`, Redirect URIs (SPA): `https://www.trashmob.eco`, `https://trashmob.eco`, check ID tokens, uncheck Access tokens. **FrontendClientId: `0604ef02-6b84-450f-b5d5-2196e96f3b48`**
- [x] **18.** **Backend API:** Name: `TrashMob API`, Expose an API → URI: `api://dc09e17b-bce4-4af9-82ab-f7b12af586b4`, Scopes: `TrashMob.Read`, `TrashMob.Writes`. **ClientId: `dc09e17b-bce4-4af9-82ab-f7b12af586b4`**
- [x] **19.** **Mobile App:** Name: `TrashMob Mobile`, Redirect URI (Public client): `eco.trashmob.trashmobmobile://auth`. **AppId: `9fce4b6e-9df5-4e41-a425-75535ba99fbe`**
- [x] **20.** **Auth Extension (Layer 2):** Name: `TrashMob AuthExtension`. **AppId: `261e358b-ccf7-4691-89a8-0690262bcc52`**
- [x] **21.** **Grant API Permissions:** Web SPA + Mobile App → `TrashMob.Read` + `TrashMob.Writes` → Admin consent granted

#### A4.3 Configure Optional Claims

- [x] **22.** Run configure script:
  ```bash
  az login --tenant b5fc8717-29eb-496e-8e09-cf90d344ce9f --allow-no-subscriptions
  .\Deploy\configure-entra-apps.ps1 -Environment pr
  ```
  Sets: `email`, `given_name`, `family_name`, `preferred_username`, `acceptMappedClaims: true`, `isFallbackPublicClient: true` (mobile only)

#### A4.4 Configure Social Identity Providers

In Azure Portal > prod Entra tenant > External Identities > All identity providers:

- [x] **23.** **Google:**
  1. Go to [Google Cloud Console](https://console.cloud.google.com) > select the TrashMob project
  2. APIs & Services > Credentials > **+ Create Credentials** > **OAuth client ID**
  3. Application type: **Web application**
  4. Name: `TrashMob Entra Prod`
  5. Authorized redirect URIs > **+ Add URI**:
     ```
     https://trashmobecopr.ciamlogin.com/trashmobecopr.onmicrosoft.com/federation/oauth2
     ```
  6. Click **Create** — copy the **Client ID** and **Client secret**
  7. In Azure Portal > prod Entra tenant (`trashmobecopr.onmicrosoft.com`) > External Identities > All identity providers > **+ Google**
  8. Paste the Client ID and Client secret > Save
- [x] **24.** **Facebook:**
  1. Go to [Facebook for Developers](https://developers.facebook.com) > My Apps > select the TrashMob app (or create one: Add New App > Consumer)
  2. Settings > Basic — copy the **App ID** and **App Secret**
  3. In the left nav: Use cases > Authentication and Account Creation > Customize > Settings
  4. Under Valid OAuth Redirect URIs > **+** add:
     ```
     https://trashmobecopr.ciamlogin.com/trashmobecopr.onmicrosoft.com/federation/oauth2
     ```
  5. Save Changes
  6. If the app is in Development mode: App Review > switch to **Live** mode (required for non-developer users to sign in)
  7. In Azure Portal > prod Entra tenant > External Identities > All identity providers > **+ Facebook**
  8. Paste the App ID and App Secret > Save
- [x] **25.** **Apple:**
  1. Apple Developer > Certificates, Identifiers & Profiles > Identifiers > **+** > **Services IDs**
  2. Register Service ID (e.g., `eco.trashmob.entra`) with **Sign In with Apple** enabled
  3. Configure return URL: `https://trashmobecopr.ciamlogin.com/trashmobecopr.onmicrosoft.com/federation/oidc/apple`
  4. If no `.p8` key file: create under Keys > **+** > check Sign in with Apple > download (one-time)
  5. Generate Apple client secret JWT using `d:/tools/Apple/generate-apple-secret.js` (update KEY_ID, SERVICE_ID, KEY_FILE for prod)
  6. Enter Service ID + generated JWT in Entra Apple IDP config
  7. **Set calendar reminder:** Client secret expires in 6 months — must regenerate
- [x] **26.** **Microsoft:** Enabled by default — verify it's active

#### A4.5 Create User Flow

- [x] **27.** User flows > New user flow > "Sign up and sign in" named `SignUpSignIn`
  - Identity providers: all configured (Google, Microsoft, Apple, Facebook, Email)
  - Attributes: Email (required), Given Name (required), Surname (required)
- [x] **28.** Create custom attribute: External Identities > Custom user attributes > Add `dateOfBirth` (String type)
- [x] **29.** Add `dateOfBirth` to the user flow's attribute collection page
- [x] **29a.** Add applications to the user flow: User flows > `SignUpSignIn` > Applications > Add application — select Web SPA, API, and Mobile app registrations so they use this flow for sign-in

#### A4.6 Configure Token Claims

- [x] **30.** For each app registration (Web SPA, API, Mobile): Token configuration > Add optional claims (ID + Access tokens): `given_name`, `family_name`, `email`, `preferred_username` — done via A4.3 `az rest` commands. Note: `dateOfBirth` is a custom attribute and must be added via user flow **Application claims**, not here.
- [x] **31.** Verify `acceptMappedClaims: true` in each app's Manifest — done via A4.3
- [ ] **32.** Test: sign in and decode JWT at https://jwt.ms to verify claims

#### A4.7 Configure Branding

- [x] **33.** Company branding > Default sign-in experience:
  - Banner logo: TrashMob logo (260x36 px) — use `Planning/StoreAssets/HorizontalLogo_Source.svg` resized
  - Background image: TrashMob hero image (1920x1080 px) — use `Images/v1/TME_SignInBackground_1920x1080.png`
  - Background color: `#f3f4f6`
  - Custom CSS: Upload `Deploy/entra-signin-branding.css`
  - Sign-in text: "Welcome to TrashMob.eco — Join the movement to clean up the planet!"
  - Layout: Full-screen background template
- [x] **34.** Test in incognito browser

#### A4.8 User Migration (B2C > Entra)

- [x] **35.** Run migration script to export B2C users > import to Entra External ID (see `Deploy/migrate-b2c-users-prod.ps1`)
- [ ] **36.** Verify migrated user count matches B2C
- [ ] **37.** Test sign-in with a few migrated accounts
- [ ] **38.** Confirm: existing users without DateOfBirth are grandfathered as adults

#### A4.9 Update Production Configuration

- [x] **39.** Set backend config (Key Vault or env vars):
  ```
  AzureAdEntra__Instance=https://trashmobecopr.ciamlogin.com/
  AzureAdEntra__ClientId=dc09e17b-bce4-4af9-82ab-f7b12af586b4
  AzureAdEntra__FrontendClientId=0604ef02-6b84-450f-b5d5-2196e96f3b48
  AzureAdEntra__Domain=trashmobecopr.onmicrosoft.com
  AzureAdEntra__TenantId=b5fc8717-29eb-496e-8e09-cf90d344ce9f
  UseEntraExternalId=true  (flip during B2 auth cutover — currently false in containerApp.bicep for prod)
  ```
- [x] **40.** Update `Deploy/containerApp.bicep` prod environment variables with prod Entra values
- [x] **41.** Update `Deploy/configure-entra-apps.ps1` with prod app registration IDs

#### A4.10 Deploy Auth Extension Container App (Layer 2)

- [x] **42.** Set GitHub Actions secrets (in `production` environment): `ENTRA_TENANT_ID`, `AUTH_EXTENSION_CLIENT_ID`
- [x] **43.** Create production workflow: `release_ca-authext-tm-pr-westus2.yml` — triggers on `release`, uses `acrtmprwestus2` registry, `production` environment — **Note: Initial workflow had wrong registry name `crtmprwestus2`; fixed to `acrtmprwestus2` in PR #2835. Same fix needed for `release_strapi-tm-pr-westus2.yml`.**
- [x] **44.** Register Custom Authentication Extension in Entra portal:
  - External Identities > Custom authentication extensions > Create
  - Type: `OnAttributeCollectionSubmit`
  - Target URL: `https://ca-authext-tm-pr-westus2.greenground-fd8fc385.westus2.azurecontainerapps.io/api/authext/attributecollectionsubmit`
  - Link to auth extension app registration
  - Assign to user flow's "When a user submits their information" event

  **Deployment Notes (2026-02-21):**
  > **Important lessons learned:**
  >
  > 1. **Do NOT try to select an existing app registration** when creating the custom auth extension. The portal throws `"The resourceId is not found in the IdentifierUris property of the app"` errors regardless of what Identifier URI format you set (`api://{appId}`, `api://{ciamDomain}/{appId}`, `api://{tenantId}/{appId}`).
  >
  > 2. **Use "Create new app registration"** option in the portal wizard instead. This creates a dedicated app registration with the correct Identifier URI and `CustomAuthenticationExtensions.Receive.Payload` permission pre-configured.
  >
  > 3. **Steps that worked:**
  >    - Go to Entra portal > External Identities > Custom authentication extensions > **+ Create**
  >    - Select `OnAttributeCollectionSubmit` event type
  >    - Enter the Target URL (Container App FQDN + `/api/authext/attributecollectionsubmit`)
  >    - On the API Authentication step, choose **"Create new app registration"** (NOT "Select existing")
  >    - Give it a name (e.g., `TrashMob AuthExtension API`)
  >    - Complete the wizard
  >    - Then go to User flows > `SignUpSignIn` > Custom authentication extensions > assign the new extension to "When a user submits their information"
  >
  > 4. **The original app registration** (AppId `261e358b-ccf7-4691-89a8-0690262bcc52` from step 20) is separate from the one the portal wizard creates. The wizard-created registration is specifically for the auth extension API authentication.
  >
  > 5. **Graph API alternative** was blocked by insufficient permissions in the CIAM tenant. Portal wizard is the reliable path.

#### A4.11 Mobile App Update

- [x] **45.** Verify `AuthConstants.cs` has correct prod Entra values — **Updated: ClientId `9fce4b6e`, TenantName `trashmobecopr`, TenantDomain `trashmobecopr.onmicrosoft.com`**
- [ ] **46.** Build and test on Android emulator + iOS simulator with prod tenant
- [ ] **47.** Submit to Google Play Store and Apple App Store
- [ ] **48.** Consider force-update flow for users on old B2C version

#### A4.12 Pre-Cutover Verification (on dev.trashmob.eco) :test_tube:

- [x] **49.** Web sign-in via email/password — succeeds, JWT contains expected claims (verified on dev)
- [x] **50.** Web sign-in via Google — succeeds, profile photo populated (verified on dev)
- [x] **51.** Web sign-in via Facebook — succeeds (verified on dev)
- [x] **52.** Web "Create Account" — shows age gate, blocks under-13, allows 13+ (verified on dev)
- [x] **53.** Web "Sign In" — goes directly to Entra (no age gate) (verified on dev)
- [ ] **54.** Web "Attend" (unauthenticated) — shows age gate before redirect
- [x] **55.** Mobile sign-in — Entra External ID (not B2C) (verified on dev)
- [x] **56.** Mobile "Create Account" — AgeGatePage blocks under-13 (verified on dev)
- [ ] **57.** Auth extension — POST with under-13 DOB returns `showBlockPage`
- [ ] **58.** Profile edit — in-app edit works (name, photo upload)
- [ ] **59.** Account deletion — "Delete My Data" works with typed confirmation
- [ ] **60.** Auto-create user — new sign-up creates DB user from token claims
- [x] **61.** Migrated user sign-in — existing B2C user signs in via Entra successfully (verified on dev)

---

### A5. Apple Signing & API Key Renewal :hand:

- [x] **62.** :gear: **AUTOMATED:** `scheduled_cert-expiry-check.yml` runs weekly (Mondays 9am UTC) and creates a GitHub issue if any certificate expires within 30 days. Update `Deploy/cert-expiry-dates.json` when renewing certificates. (See [Section R14](#r14-certificate-expiry-monitoring))
- [x] **63.** :test_tube: Verify `Deploy/cert-expiry-dates.json` has current expiry dates for all certificates — **Updated iOS cert expiry to 2027-02-21**
- [x] **64.** Check iOS distribution certificate expiry at https://developer.apple.com/account/resources/certificates/list — **Expired; regenerated 2026-02-21 via `openssl` CSR (macOS Sequoia lacks Keychain Certificate Assistant)**
- [x] **65.** Check App Store Connect API key status at https://appstoreconnect.apple.com/access/integrations/api — verify key used by `APPSTORE_KEY_ID` is "Active" — **`TrashMobProdApiKey` (L8R272VZ58) is Active (created 2026-02-15)**
- [x] **66.** If certificate expired or expiring — regenerate (see [Section R1](#r1-regenerate-ios-distribution-certificate)) — **Done. New cert created, .p12 exported, `IOS_CERTIFICATES_P12` and `IOS_CERTIFICATES_P12_PASSWORD` GitHub secrets updated.**
- [x] **67.** If API key expired or revoked — regenerate (see [Section R2](#r2-regenerate-app-store-connect-api-key)) — **New prod key `96BK5UTL5D` created. Set in `production` GitHub environment only.**

  **Apple API Key Separation — Lesson Learned (2026-02-21):**
  > **Dev and prod use DIFFERENT App Store Connect API keys.** Do NOT set the same key in both `test` and `production` environments.
  >
  > | Environment | Key ID | Key Name | Purpose |
  > |-------------|--------|----------|---------|
  > | `test` (dev builds) | `X8J78CDY98` | TrashMobDevApiKey | Dev app (`eco.trashmobdev.trashmobmobile`) |
  > | `production` (release builds) | `96BK5UTL5D` | TrashMobProdApiKey | Prod app (`eco.trashmob`) |
  >
  > Initially the prod key was mistakenly set in BOTH environments, causing dev iOS builds to fail with `error:1E08010C:DECODER routines::unsupported` when downloading provisioning profiles (wrong key for the dev app). Fix: Restored dev key `X8J78CDY98` to `test` environment.
  >
  > **Secrets affected:** `APPSTORE_KEY_ID`, `APPSTORE_PRIVATE_KEY` (and possibly `APPSTORE_ISSUER_ID` — same issuer for both keys since they're on the same account).
- [x] **68.** Verify Android keystore — `ANDROID_KEYSTORE` and `ANDROID_KEYSTORE_PASSWORD` secrets are set (see [Section R3](#r3-android-keystore-rotation) if rotation needed) — **Both secrets exist in `test` (2024-05-26) and `production` (2022-09-10) environments. Keystore valid until 2036.**

  **Dev vs Prod Android Bundle ID — Lesson Learned (2026-02-21):**
  > The dev workflow (`main_trashmobmobileapp.yml`) had `ANDROID_BUNDLE_ID: 'eco.trashmob.trashmobmobileapp'` (the prod value), causing Google Play upload failures due to signing key mismatch. The correct dev Android bundle ID is `eco.trashmobdev.trashmobmobile`. Fixed in PR #2834.
  >
  > | Environment | Android Bundle ID | iOS Bundle ID |
  > |-------------|-------------------|---------------|
  > | Dev (`test`) | `eco.trashmobdev.trashmobmobile` | `eco.trashmobdev.trashmobmobile` |
  > | Prod (`production`) | `eco.trashmob.trashmobmobileapp` | `eco.trashmob` |
  >
  > **Google Maps note:** The dev Android bundle ID must be registered in Google Cloud Console API key restrictions (package name + SHA-1 fingerprint) for Google Maps to work on dev Android builds.

---

### A6. App Store Logos & Icons Review :hand:

Verify all store logos are current (v2 branding) and correctly sized. Source assets are in `Planning/StoreAssets/`.

**Required icon sizes:**

| Store | Asset | Required Size | Source File | Status |
|-------|-------|---------------|-------------|--------|
| Apple App Store | App Icon | 1024x1024 PNG (no transparency) | Resize from `AppIcon_2500x2500.png` | - [ ] Ready |
| Google Play | Hi-res Icon | 512x512 PNG (32-bit) | Resize from `AppIcon_2500x2500.png` | - [ ] Ready |
| Google Play | Feature Graphic | 1024x500 PNG/JPG | Generate with `generate-feature-graphic.ps1` → `Generated/GooglePlay_FeatureGraphic_1024x500.png` | - [ ] Reviewed |
| Apple App Store | App Store Banner | 1024x1024 (same as icon) | Same as App Icon | - [ ] Ready |
| Entra Sign-In | Banner Logo | 260x36 PNG | Resize from `HorizontalLogo_2259x588.png` | - [ ] Ready |
| Entra Sign-In | Background | 1920x1080 PNG | `Images/v1/TME_SignInBackground_1920x1080.png` | - [ ] Ready |

**Store assets folder:** `Planning/StoreAssets/`

| File | Dimensions | Description |
|------|------------|-------------|
| `AppIcon_2500x2500.png` | 2500x2500 | V2 logo symbol — resize for Apple (1024x1024) and Google (512x512) |
| `AppIcon_Source.svg` | Vector | Source SVG for logo symbol |
| `HorizontalLogo_2259x588.png` | 2259x588 | V2 horizontal logo with tagline |
| `HorizontalLogo_Source.svg` | Vector | Source SVG for horizontal logo |

Additional source files at `D:\data\images\v2\New TrashMob.eco files\New TrashMob.eco files\` (Illustrator, PDF, JPG, PNG, SVG formats).

- [x] **69.** Generate all icon sizes — run `.\Planning\StoreAssets\generate-icons.ps1` (outputs to `Planning/StoreAssets/Generated/`):
  - `AppStore_1024x1024.png` — Apple App Store (no transparency, no rounded corners)
  - `GooglePlay_512x512.png` — Google Play (32-bit PNG)
  - Plus PWA, favicon, and Entra profile sizes
- [x] **70.** Generate feature graphic — run `.\Planning\StoreAssets\generate-feature-graphic.ps1`:
  - Outputs `GooglePlay_FeatureGraphic_1024x500.png` with v2 logo on brand green background
  - Adjust `-Tagline`, `-BackgroundColor` parameters if needed
- [x] **71.** Review generated images visually before uploading to stores
- [x] **72.** Verify app icon in Apple App Store Connect matches v2 logo
- [x] **73.** Verify app icon in Google Play Console matches v2 logo

---

### A7. App Store Listing & Screenshots :hand:

- [ ] **74.** Update app store listing copy (see [Section R4](#r4-app-store-listing-copy))
- [ ] **75.** Update release notes (see [Section R5](#r5-release-notes-template))
- [ ] **76.** Update keywords (Apple, see [Section R6](#r6-keywords))
- [ ] **77.** Capture new screenshots if significant UI changes — run `gh workflow run "Capture App Screenshots"` or capture manually (see [Section R7](#r7-screenshot-guide) and [Section R19](#r19-screenshot-capture))
- [ ] **78.** Verify privacy policy URL is accessible: https://www.trashmob.eco/privacypolicy
- [ ] **79.** Verify support URL is accessible: https://www.trashmob.eco/contactus
- [ ] **80.** Update content rating if needed (user-generated content, location)
- [ ] **81.** Update Data Safety (Google) if new data types collected — check `Planning/PRIVACY_MANIFEST.md` for current declarations (see [Section R16](#r16-privacy-manifest-ci-check))
- [ ] **82.** Update App Privacy (Apple) if new data types collected — check `Planning/PRIVACY_MANIFEST.md` for current declarations (see [Section R16](#r16-privacy-manifest-ci-check))
- [ ] **83.** :gear: **AUTOMATED:** `ci_privacy-manifest-check.yml` adds a PR comment when privacy-related files change, reminding to update store forms
- [ ] **84.** Verify COPPA compliance — age gate blocks under-13
- [ ] **85.** Verify location permission strings are accurate and specific

---

### A8. Dev Environment Smoke Test :test_tube:

- [x] **86.** Deploy to dev.trashmob.eco and verify all features work before proceeding to production
- [x] **87.** Run through feature testing checklist (Part C below) on dev first

---

## Part B — Deployment

---

### B1. Database Migrations :gear:

Run all 31 pending migrations. EF Core applies only unapplied migrations automatically.

- [ ] **88.** :gear: **AUTOMATED:** `release_db-migrations.yml` runs automatically when migration files change on `release` push. It:
  - Temporarily adds the GitHub runner IP to the Azure SQL firewall
  - Retrieves the connection string from Key Vault
  - Lists pending migrations, then applies them via `dotnet ef database update`
  - Removes the firewall rule (always, even on failure)
  - Can also be triggered manually via `workflow_dispatch`

<details>
<summary><strong>Full migration list (31 migrations)</strong></summary>

#### Weight & Event Basics
1. `20260107022919_AddPickupWeightToEventSummary` - Adds PickedWeight (int) and PickedWeightUnitId to EventSummaries; creates WeightUnits table with None/lb/kg
2. `20260131222334_AddEventCoLeads` - Adds IsEventLead column to EventAttendees; **backfills event creators as leads**
3. `20260201013924_ChangePickedWeightToDecimal` - Converts EventSummaries.PickedWeight from int to decimal(10,1)

#### Teams & Feedback
4. `20260201030122_AddTeamsFeature` - Creates Teams, TeamMembers, TeamEvents, TeamJoinRequests, TeamPhotos tables
5. `20260201164845_AddUserFeedback` - Creates UserFeedback table for in-app feedback with status, screenshots, GitHub issue linking

#### Community Pages (Project 10)
6. `20260201181911_AddCommunityHomePageFieldsToPartner` - Adds 12 columns to Partners: BannerImageUrl, BrandingColors, City, Country, Lat/Lng, Slug, Tagline, HomePage fields
7. `20260202000411_AddCommunityPhase2Fields` - Adds ContactEmail, ContactPhone, LogoUrl, PhysicalAddress to Partners

#### Photo Moderation & Adoptable Areas
8. `20260202004834_AddPhotoModeration` - Adds moderation fields to TeamPhotos and LitterImages; creates PhotoFlags and PhotoModerationLogs tables
9. `20260202030639_AddAdoptableArea` - Creates AdoptableAreas table with GeoJSON, cleanup frequency, and co-adoption settings
10. `20260202042553_AddTeamAdoption` - Creates TeamAdoptions table linking teams to adoptable areas with application/review workflow

#### Waivers V3 (Project 8)
11. `20260202053435_AddWaiverVersioning` - Creates WaiverVersions, CommunityWaivers, UserWaivers tables with versioning, minor consent, guardian info

#### Adoption Events & Attendee Metrics
12. `20260202062028_AddTeamAdoptionEvent` - Adds adoption tracking columns to TeamAdoptions; creates TeamAdoptionEvents join table
13. `20260203024754_AddEventAttendeeMetrics` - Creates EventAttendeeMetrics table for per-attendee bags, weight, duration tracking

#### Engagement & Gamification
14. `20260203042212_AddEmailInvites` - Creates EmailInviteBatches and EmailInvites tables for invite tracking and conversion
15. `20260203050953_AddLeaderboardCacheAndGamificationPreferences` - Adds ShowOnLeaderboards and AchievementNotificationsEnabled to Users; creates LeaderboardCaches table
16. `20260203150803_AddAchievements` - Creates AchievementTypes and UserAchievements tables; seeds 7 achievement types
17. `20260203151205_AddNewsletterSupport` - Creates NewsletterCategories, NewsletterTemplates, Newsletters, UserNewsletterPreferences; seeds defaults

#### Photos
18. `20260204030038_AddEventPhotos` - Creates EventPhotos table with moderation support
19. `20260204040000_AddPartnerPhoto` - Creates PartnerPhotos table with moderation support

#### Route Tracking (Project 4)
20. `20260206171539_AddRouteTracingProperties` - Adds 9 columns to EventAttendeeRoutes (BagsCollected, DurationMinutes, WeightCollected, PrivacyLevel, etc.); creates RoutePoints table for GPS coordinates

#### Community Prospects (Project 40)
21. `20260207140209_AddCommunityProspects` - Creates CommunityProspects and ProspectActivities tables with pipeline stage and fit scoring
22. `20260207191850_AddProspectOutreachEmails` - Creates ProspectOutreachEmails table for outreach cadence with open/click tracking

#### Regional Communities & Partner Docs
23. `20260208181514_AddRegionalCommunityFields` - Adds BoundsNorth/South/East/West, CountyName, RegionType to Partners
24. `20260208182003_AddPartnerDocumentStorage` - Adds BlobStoragePath, ContentType, DocumentTypeId, ExpirationDate, FileSizeBytes to PartnerDocuments
25. `20260208222433_AddCommunityAreaDefaults` - Adds default adoptable area settings to Partners (co-adoption, frequency, min events, safety)

#### Professional/Sponsored Adoptions
26. `20260208233634_AddSponsoredAdoptions` - Creates ProfessionalCompanies, Sponsors, SponsoredAdoptions, ProfessionalCompanyUsers, ProfessionalCleanupLogs tables

#### User Profile & Auth
27. `20260209005001_AddUserProfileFields` - Adds DateOfBirth, GivenName, Surname, ProfilePhotoUrl to Users

#### Community & Event Enhancements
28. `20260210020832_AddCommunityPartnerTypeAndIsFeatured` - Adds IsFeatured boolean to Partners; inserts Community partner type
29. `20260214204051_AddEventVisibility` - Replaces IsEventPublic with EventVisibilityId (Public/TeamOnly/Private); adds TeamId FK to Events

#### AI Area Generation (Project 44)
30. `20260216045129_AddAreaGeneration` - Creates AreaGenerationBatches and StagedAdoptableAreas tables for AI-powered area discovery
31. `20260217150209_AddBoundaryGeoJson` - Adds BoundaryGeoJson column to Partners for storing GeoJSON boundary polygons

**Notes:**
- Migration #2 includes data backfill SQL that marks all event creators as event leads
- Migration #29 converts the boolean IsEventPublic to an enum-style EventVisibilityId — existing events are migrated to Public

</details>

---

### B2. Auth Migration Cutover Window :hand:

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

### B3. Merge and Deploy Web App :hand: :gear:

- [x] **91.** Merge main to release:
  ```bash
  git checkout release
  git pull origin release
  git merge origin/main
  git push origin release
  ```
  **Note (2026-02-21):** Merge had 739 commits and 37 merge conflicts. All resolved using `--theirs` strategy (accepting main's version). Three issues found after merge:
  - **Registry name:** `release_ca-authext-tm-pr-westus2.yml` and `release_strapi-tm-pr-westus2.yml` had `crtmprwestus2` instead of `acrtmprwestus2` — fixed directly on release, PR #2835 created for main
  - **Bicep duplicates:** `Deploy/containerApp.bicep` had duplicate `param`/`var` declarations from merge conflict resolution (`customDomainName`, `managedCertificateName`, `managedCertificateId`) — removed duplicates directly on release, needs fixing on main too
- [x] **92.** :gear: **AUTOMATED:** GitHub Actions builds and deploys web container to Azure Container Apps — **Succeeded after fixing registry names and Bicep duplicates**
- [x] **93.** :gear: **AUTOMATED:** GitHub Actions builds and deploys background jobs (daily + hourly) — **Succeeded**
- [x] **94.** Monitor GitHub Actions for success: https://github.com/TrashMob-eco/TrashMob/actions — **All backend services deployed successfully. Auth extension, Strapi, and Container App required fixes (see step 91 notes).**

---

### B4. Deploy Mobile Apps :hand: :gear:

- [x] **95.** :gear: **AUTOMATED:** Push to `release` triggers `release_trashmobmobileapp.yml` which:
  - Builds Android AAB, signs with keystore, uploads to Google Play internal track via GCP service account
  - Builds iOS IPA, signs with distribution cert, uploads to TestFlight via `xcrun altool` (see [Section R18](#r18-app-store-promotion))

  **Deployment Notes (2026-02-21):**
  > - **iOS Build:** Succeeded. Used `xcrun altool` (not Fastlane Pilot — see R18 note).
  > - **iOS Deploy:** Failed — version `2.11.597` already uploaded to TestFlight by dev build. Next release commit will get a higher version number.
  > - **Android Build:** Succeeded.
  > - **Android Deploy:** Failed — Google Play rejected upload because the app now uses `FOREGROUND_SERVICE_LOCATION` permission, which requires a foreground service declaration in Google Play Console (see step 98 notes). **Workaround:** Manually uploaded the signed AAB via Google Play Console after completing the declaration.
- [x] **96.** Monitor mobile build workflow for success — **Builds succeeded; deploys had issues (see step 95 notes)**
- [ ] **97.** :hand: **Apple:** Promote TestFlight build to App Store review — either manually in App Store Connect, or run `gh workflow run "iOS - Submit for App Store Review"` (see [Section R18](#r18-app-store-promotion)) — **Blocked: need a new version build (see step 95). Will succeed on next release push.**
- [ ] **98.** :hand: **Google:** Promote internal track to production with staged rollout — either manually in Google Play Console, or run `gh workflow run "Android - Adjust Production Rollout" -f rollout_percentage=10` then increase (see [Section R17](#r17-google-play-staged-rollout))

  **Google Play Foreground Service Declaration (2026-02-21):**
  > This release adds `FOREGROUND_SERVICE_LOCATION` permission for background route tracking. Google Play now requires:
  >
  > 1. **Permission declaration:** Google Play Console > App content > Sensitive app permissions > Foreground service
  >    - Task type: **Background location updates** > **User-initiated location sharing**
  >    - Justification: "TrashMob tracks the user's cleanup route during litter pickup events. The user explicitly starts route recording from the event details screen. A foreground service with a persistent notification ('Recording your cleanup route...') keeps GPS location updates active when the user switches apps or locks their screen, so their full route is captured. Recording stops when the user returns to the app and taps Stop, or when they leave the event."
  >
  > 2. **Video demonstration required:** Record a 30-60 second screen recording showing:
  >    - Open app → navigate to event → Routes tab → Start route tracking
  >    - Foreground notification appears ("Recording your cleanup route...")
  >    - Switch to another app (background usage)
  >    - Pull down notification shade showing the notification is still active
  >    - Return to app → Stop route tracking → notification disappears
  >    - Record using: device screen recorder, emulator toolbar, or `adb shell screenrecord /sdcard/demo.mp4`
  >
  > 3. **Must complete declaration before uploading AAB** — otherwise Google Play rejects the upload.
  >
  > See also: GitHub issues #2836 (deobfuscation file) and #2837 (debug symbols) for post-launch cleanup items.

---

## Part C — Post-Deployment Verification :test_tube:

---

### C1. Smoke Test

- [ ] **99.** :gear: **AUTOMATED:** `release_smoke-tests.yml` runs automatically after the container app deployment completes. It checks: site HTTP status, `/health`, `/health/live`, `/api/config` (valid JSON), and Swagger endpoint. (See [Section R13](#r13-post-deployment-smoke-tests))
- [ ] **100.** :test_tube: Verify smoke test workflow passed in GitHub Actions
- [ ] **101.** :hand: Test login/logout functionality manually
- [ ] **102.** Check Application Insights for errors (monitor for 1 hour)

---

### C2. Feature Testing Checklist

#### Teams Feature (Project 9)
- [ ] **103.** Create a new team
- [ ] **104.** Join an existing team
- [ ] **105.** View Teams map
- [ ] **106.** Upload team photo
- [ ] **107.** Upload team logo
- [ ] **108.** Associate event with team

#### User Feedback (Project 34)
- [ ] **109.** Feedback widget visible in bottom-right corner
- [ ] **110.** Submit test feedback
- [ ] **111.** Admin can view feedback at /siteadmin/feedback

#### Event Co-Leads (Project 21)
- [ ] **112.** Event creator marked as lead
- [ ] **113.** Can add co-leads to events

#### Weight Tracking (Project 7)
- [ ] **114.** Event summary accepts decimal weights
- [ ] **115.** Weight units dropdown works

#### Litter Reports (Project 3)
- [ ] **116.** Create litter report with photos
- [ ] **117.** Litter reports appear on home map
- [ ] **118.** Admin litter reports page works

#### Feature Metrics (Project 29)
- [ ] **119.** Login/logout events tracked
- [ ] **120.** Event creation tracked
- [ ] **121.** Attendance registration tracked

#### Community Pages (Project 10)
- [ ] **122.** Communities discovery page at /communities
- [ ] **123.** Community detail page at /communities/{slug}
- [ ] **124.** Community map shows events, teams, litter reports
- [ ] **125.** Stats widget shows community impact metrics
- [ ] **126.** Contact card displays email/phone/address
- [ ] **127.** Events and Teams sections display nearby data

#### Job Opportunities Markdown Editor (Issue #2215)
- [ ] **128.** Admin can create/edit job opportunities with markdown
- [ ] **129.** Preview toggle works in admin forms
- [ ] **130.** Convert existing job listings to markdown format:
  - Go to /siteadmin/job-opportunities, edit each active listing
  - `<strong>` > `**bold**`, `<em>` > `*italic*`, `<ul><li>` > `- item`, `<h2>` > `## heading`, `<br>` > blank line, `<p>` > text with blank lines
  - Use Preview toggle to verify before saving
- [ ] **131.** Volunteer opportunities page renders markdown correctly

#### Route Tracking (Project 4)
- [ ] **132.** Start route tracking during an event on mobile
- [ ] **133.** Route appears as colored polyline on event map
- [ ] **134.** Route tracking works offline and syncs when connected
- [ ] **135.** Route stops recording when user taps stop or event ends

#### Area Map Editor (Project 44)
- [ ] **136.** Generate areas via AI (interchanges, city blocks, highway sections)
- [ ] **137.** Search and filter adoptable areas
- [ ] **138.** Clear all areas (hard-delete for unadopted areas)
- [ ] **139.** View area boundaries on map (polygon rendering)
- [ ] **140.** Community boundary GeoJSON displays correctly

#### Event Visibility
- [ ] **141.** Create event with Public/Team-Only/Private visibility
- [ ] **142.** Team-Only events visible only to team members
- [ ] **143.** Private events visible only to invited attendees

#### Event Photos
- [ ] **144.** Upload photos to an event
- [ ] **145.** Photos display in event details
- [ ] **146.** Photo moderation flags work for admins

#### Auth Migration — Entra External ID (Project 1)
- [ ] **147.** Sign in via email/password works
- [ ] **148.** Sign in via Google works, profile photo auto-populated
- [ ] **149.** Sign in via Facebook works
- [ ] **150.** Sign in via Apple works
- [ ] **151.** "Create Account" shows age gate before Entra redirect
- [ ] **152.** Age gate blocks under-13 with friendly message
- [ ] **153.** "Sign In" goes directly to Entra (no age gate)
- [ ] **154.** Profile edit works in-app (name, photo upload)
- [ ] **155.** "Delete My Data" works with typed DELETE confirmation
- [ ] **156.** Migrated B2C user can sign in via Entra
- [ ] **157.** New user auto-created in DB on first sign-in
- [ ] **158.** Auth extension blocks under-13 sign-up server-side
- [ ] **159.** JWT contains expected claims: email, given_name, family_name, dateOfBirth
- [ ] **160.** No auth errors in Application Insights after 1 hour

---

### C3. Post-Launch Monitoring

- [ ] **161.** Monitor Application Insights for auth errors for 24 hours
- [ ] **162.** Monitor Sentry.io for mobile crashes
- [ ] **163.** Monitor Google Play pre-launch report for crashes
- [ ] **164.** After 1-week coexistence: decommission B2C tenant

---

## Part D — Rollback Plan

### D1. Quick Rollback (< 5 min) :hand:

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

### D3. Database Rollback

Database migrations do NOT have automatic rollback. If critical issues:
1. Restore from backup (Azure SQL automatic backups — 14-day retention)
2. Or manually run `Down()` migration scripts

---

## Reference Sections

---

### R1. Regenerate iOS Distribution Certificate

<a id="r1-regenerate-ios-distribution-certificate"></a>

1. Open Keychain Access on a Mac > Certificate Assistant > Request a Certificate from a CA
   - Email: joe@trashmob.eco, Common Name: TrashMob Distribution, Save to disk
2. Go to [Apple Developer Certificates](https://developer.apple.com/account/resources/certificates/add) > Type: **Apple Distribution** > Upload CSR > Download `.cer`
3. Double-click `.cer` to install in Keychain Access
4. In Keychain Access, find certificate > right-click > **Export** > save as `.p12` with a strong password
5. Base64-encode: `base64 -i Certificates.p12 -o Certificates.p12.base64`
6. Update GitHub secrets: `IOS_CERTIFICATES_P12` (base64 content), `IOS_CERTIFICATES_P12_PASSWORD` (password)
7. Regenerate provisioning profile: [Provisioning Profiles](https://developer.apple.com/account/resources/profiles/list) > Edit App Store profile for `eco.trashmob` > Select new cert > Download

### R2. Regenerate App Store Connect API Key

<a id="r2-regenerate-app-store-connect-api-key"></a>

1. Go to [App Store Connect > Integrations > API](https://appstoreconnect.apple.com/access/integrations/api)
2. Click **+** > Name: `TrashMob CI/CD` > Access: **App Manager**
3. Download `.p8` private key immediately (one-time download)
4. Note **Key ID** and **Issuer ID**
5. Update GitHub secrets (in both `test` and `production` environments):
   - `APPSTORE_ISSUER_ID`, `APPSTORE_KEY_ID`, `APPSTORE_PRIVATE_KEY` (entire `.p8` contents)
6. Trigger a test build to verify: `gh workflow run "TrashMobMobileApp - Dev"`

### R3. Android Keystore Rotation

<a id="r3-android-keystore-rotation"></a>

Rarely needed — only if compromised. Managed via Google Play App Signing:
1. `keytool -genkeypair -v -keystore upload.jks -keyalg RSA -keysize 2048 -validity 10000 -alias upload`
2. `keytool -export -rfc -keystore upload.jks -alias upload -file upload_certificate.pem`
3. Google Play Console > App > Setup > App signing > Request upload key reset > Upload `upload_certificate.pem`
4. Update secrets: `ANDROID_KEYSTORE` (base64-encoded jks), `ANDROID_KEYSTORE_PASSWORD`

### R4. App Store Listing Copy

<a id="r4-app-store-listing-copy"></a>

**Name:** TrashMob
**Subtitle (Apple, 30 chars max):** Clean Up Your Community
**Short Description (Google, 80 chars max):** Join local cleanup events. Report litter. Track your environmental impact.

**Full Description:**

> **Make a real difference in your neighborhood.**
>
> TrashMob connects you with community cleanup events happening near you. Whether you want to organize a cleanup or join one, TrashMob makes it easy to take action against litter and build a cleaner world.
>
> **Find and join events**
> Browse cleanup events on an interactive map. See what's happening near you today, this week, or this month. One tap to RSVP and get directions.
>
> **Report litter**
> Spot a mess? Snap a photo to create a litter report. Your report appears on the community map so others can see problem areas and organize cleanups where they're needed most.
>
> **Track your route**
> Turn on route tracking during a cleanup to see exactly where you covered. Your path shows on the event map so the team can see the area cleaned.
>
> **See your impact**
> Your personal dashboard shows your stats: events attended, hours volunteered, bags collected, and litter reports submitted. Watch your impact grow with every cleanup.
>
> **Build a team**
> Create or join a team to track collective impact. Compete on leaderboards, coordinate events, and celebrate milestones together.
>
> **Pickup coordination**
> After a cleanup, drop pins where bags of trash are waiting. Hauling partners can see pickup locations and collect them — no bags left behind.
>
> **Why TrashMob?**
> - 100% free, no ads, open source
> - Events in cities across the United States and Canada
> - Works offline for route tracking
> - Available on iOS and Android
>
> Join thousands of volunteers making their communities cleaner. Download TrashMob and start your first cleanup today.

### R5. Release Notes Template

<a id="r5-release-notes-template"></a>

> - Redesigned Create Event and Edit Event pages with improved date/time and duration controls
> - Route tracking during cleanup events — see your walking path on the map
> - Team support — create or join a team, track collective impact
> - Improved litter report flow with photo support
> - Better form validation with inline error messages
> - Performance and stability improvements

### R6. Keywords

<a id="r6-keywords"></a>

Apple (100 chars max):
```
cleanup,litter,volunteer,community,environment,trash,recycle,pickup,green,eco,team,event,map,route
```

### R7. Screenshot Guide

<a id="r7-screenshot-guide"></a>

#### Required Sizes

**Apple App Store:**

| Device | Size (pixels) | Required |
|--------|---------------|----------|
| iPhone 6.9" (16 Pro Max) | 1320 x 2868 | Yes |
| iPhone 6.7" (15 Plus/Pro Max) | 1290 x 2796 | Yes |
| iPhone 6.5" (11 Pro Max) | 1284 x 2778 | Yes (if supporting older models) |
| iPad Pro 13" | 2064 x 2752 | Yes (if iPad supported) |

**Google Play Store:**

| Type | Size (pixels) | Required |
|------|---------------|----------|
| Phone | Min 320px, max 3840px, 16:9 or 9:16 | Yes (2-8 screenshots) |
| Tablet 7" | 1024 x 500 or similar | Recommended |
| Tablet 10" | 1920 x 1200 or similar | Recommended |

#### Recommended Screenshots (in order)

1. **Home/Map View** — Shows nearby events and litter reports on the map
2. **Event Details** — A well-populated event with date, location, attendees
3. **Litter Report** — Creating a litter report with photo
4. **Route Tracking** — Active cleanup route on the map
5. **Dashboard** — User stats, upcoming events, impact metrics
6. **Teams** — Team page with members and collective impact

#### Capture Tips

**Android:** Use Android Studio emulator with Pixel 8 Pro profile. `adb exec-out screencap -p > screenshot.png`

**iOS:** Use Xcode Simulator. `xcrun simctl boot "iPhone 16 Pro Max"` then `xcrun simctl io booted screenshot screenshot.png`

**Professional polish:** Use [screenshots.pro](https://screenshots.pro), [Previewed](https://previewed.app), or [AppMockUp](https://app-mockup.com) to add device frames and captions. Use brand green `#96ba00` as background. Keep captions to 3-5 words.

### R8. Location Permission Justification

#### Apple App Review Response

> **Background Location Usage:**
> TrashMob uses background location exclusively for the "Route Tracking" feature during active cleanup events. When a user starts a cleanup, they can optionally enable route tracking to record their walking path. This path is displayed on the event map to show the area they covered. Background location is only active when the user explicitly enables route tracking, and it stops when the event ends or the user turns it off. A visible indicator is shown whenever background location is in use. The app never collects location data when the user is not actively using the route tracking feature.

**Tips to avoid rejection:**
- Include a demo account or clear instructions for the reviewer to test route tracking
- Show a visible indicator when background location is active
- Ensure the app works without location (graceful degradation)

#### Google Play Data Safety

| Category | Data type | Collected | Shared | Purpose |
|----------|-----------|-----------|--------|---------|
| Location | Approximate location | Yes | No | Show nearby events |
| Location | Precise location | Yes | No | Litter report pins, pickup locations, route tracking |
| Personal info | Name, Email | Yes | No | User profile, event registration |
| Photos/Videos | Photos | Yes | No | Litter report photos, event photos |

- **Encrypted in transit:** Yes (HTTPS only)
- **Deletion mechanism:** Yes ("Delete My Data" in-app or contact info@trashmob.eco)
- **Required for app to function:** Location is optional

### R9. Submission & Certification Tips

**Apple:**
- Review time: 24-48 hours (expedited for critical fixes at [reportaproblem.apple.com](https://reportaproblem.apple.com))
- Provide a demo account in App Review Information
- Explain route tracking and location usage in App Review notes
- Common rejections: vague location strings, no visible background location indicator, crashes on launch, missing privacy policy, no test account

**Google:**
- Review time: hours to 7 days (updates faster than new apps)
- Use internal testing track first (no review), then closed testing (review + limited audience), then production
- Staged rollout recommended: 10% > monitor crashes > 100%
- Check Firebase Test Lab pre-launch report for crashes
- Data safety form must be accurate — Google flags discrepancies

**Both:**
- Test the upgrade path (old version > new version), not just fresh installs
- Test with location permission denied — app should degrade gracefully
- Test with no network — route tracking should work offline, sync later
- Keep release notes user-friendly — no developer jargon

### R10. Automated Database Migrations Workflow

<a id="r10-automated-database-migrations"></a>

**Workflow:** `.github/workflows/release_db-migrations.yml`

#### How it works

1. **Trigger:** Runs automatically on push to `release` when files in `TrashMob.Shared/Migrations/` or `MobDbContext.cs` change. Also supports `workflow_dispatch` for manual runs.
2. **Environment:** Requires `production` environment approval in GitHub (same approval gate as the container app deployment).
3. **Process:**
   - Checks out code, installs .NET 10 SDK and `dotnet-ef` tool
   - Logs into Azure via OIDC (same service principal as other prod workflows)
   - Gets the GitHub runner's public IP and creates a temporary SQL firewall rule
   - Retrieves the `TMDBServerConnectionString` secret from Key Vault (`kv-tm-pr-westus2`)
   - Lists pending migrations, then applies them with `dotnet ef database update --verbose`
   - **Always** removes the temporary firewall rule (even on failure)
4. **Concurrency:** `cancel-in-progress: false` — migrations are never interrupted mid-run

#### Prerequisites

These GitHub secrets must exist in the `production` environment (already configured for other prod workflows):

| Secret | Purpose |
|--------|---------|
| `AZURE_CLIENT_ID` | Service principal for OIDC login |
| `AZURE_TENANT_ID` | Azure AD tenant |
| `AZURE_SUBSCRIPTION_ID` | Azure subscription |

The service principal needs these permissions:
- `Key Vault Secrets User` on `kv-tm-pr-westus2` (to read `TMDBServerConnectionString`)
- `SQL Server Contributor` or equivalent on `sql-tm-pr-westus2` (to create/delete firewall rules)
- The connection string itself must include credentials with DDL permissions on the database

#### Manual trigger

```bash
gh workflow run "TrashMobProd - Database Migrations"
```

#### Troubleshooting

- **Firewall rule not cleaned up:** If the workflow crashes hard (GitHub outage), manually remove the rule:
  ```bash
  az sql server firewall-rule list --server sql-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 -o table
  az sql server firewall-rule delete --server sql-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --name "github-actions-runner-<run-id>" --yes
  ```
- **Migration fails:** The workflow will fail but the firewall rule is still cleaned up. Fix the migration code, push to release, and the workflow re-runs.
- **No pending migrations:** The workflow detects this and skips the `database update` step (no-op).

### R11. Icon Generation Script

<a id="r11-icon-generation-script"></a>

**Script:** `Planning/StoreAssets/generate-icons.ps1`

Generates all required app store and platform icon sizes from the 2500x2500 source PNG using .NET `System.Drawing` (no external tools needed on Windows).

#### Usage

```powershell
# Default: uses AppIcon_2500x2500.png, outputs to Planning/StoreAssets/Generated/
.\Planning\StoreAssets\generate-icons.ps1

# Custom source image
.\Planning\StoreAssets\generate-icons.ps1 -SourceImage ".\path\to\icon.png"

# Custom output directory
.\Planning\StoreAssets\generate-icons.ps1 -OutputDir ".\my-output"
```

#### Generated files

| File | Size | Use |
|------|------|-----|
| `AppStore_1024x1024.png` | 1024x1024 | Apple App Store icon (no transparency, no rounded corners — Apple adds those) |
| `GooglePlay_512x512.png` | 512x512 | Google Play Store hi-res icon (32-bit PNG) |
| `PWA_512x512.png` | 512x512 | Progressive Web App manifest icon (large) |
| `PWA_192x192.png` | 192x192 | Progressive Web App manifest icon (small) |
| `Favicon_32x32.png` | 32x32 | Browser favicon |
| `EntraProfile_240x240.png` | 240x240 | Entra External ID / Azure AD profile image |

#### Source asset

The source icon is `Planning/StoreAssets/AppIcon_2500x2500.png` (v2 logo symbol, copied from `D:\data\images\v2\New TrashMob.eco files`). The SVG source is `AppIcon_Source.svg`.

To update the icon: replace `AppIcon_2500x2500.png` with the new design at 2500x2500 or larger, then re-run the script.

### R12. Feature Graphic Generation Script

<a id="r12-feature-graphic-generation-script"></a>

**Script:** `Planning/StoreAssets/generate-feature-graphic.ps1`

Generates the Google Play Feature Graphic (1024x500) by compositing the horizontal logo onto a branded background with configurable tagline.

#### Usage

```powershell
# Default: TrashMob brand green with "Clean Up Your Community" tagline
.\Planning\StoreAssets\generate-feature-graphic.ps1

# Custom tagline
.\Planning\StoreAssets\generate-feature-graphic.ps1 -Tagline "Join the Movement"

# Custom colors
.\Planning\StoreAssets\generate-feature-graphic.ps1 -BackgroundColor "#005B4B" -TaglineColor "#96ba00"

# No tagline
.\Planning\StoreAssets\generate-feature-graphic.ps1 -Tagline ""
```

#### Parameters

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-LogoImage` | `HorizontalLogo_2259x588.png` | Source logo (horizontal with tagline) |
| `-OutputDir` | `Generated/` | Output directory |
| `-BackgroundColor` | `#96ba00` | TrashMob brand green |
| `-Tagline` | `Clean Up Your Community` | Text below logo |
| `-TaglineColor` | `#FFFFFF` | Tagline text color |
| `-Width` | `1024` | Output width (px) |
| `-Height` | `500` | Output height (px) |

#### Output

`Planning/StoreAssets/Generated/GooglePlay_FeatureGraphic_1024x500.png`

Upload to Google Play Console > Store listing > Feature graphic.

### R13. Post-Deployment Smoke Tests

<a id="r13-post-deployment-smoke-tests"></a>

**Workflow:** `.github/workflows/release_smoke-tests.yml`

#### How it works

1. **Trigger:** Runs automatically after `TrashMobProd - Container App` workflow completes successfully. Also supports `workflow_dispatch` with optional `base_url` input.
2. **Wait:** Pauses 60 seconds for the deployment to stabilize.
3. **Checks:**
   - HTTP status code from the base URL (must be 2xx-3xx)
   - `/health` endpoint returns "Healthy"
   - `/health/live` endpoint returns "Healthy"
   - `/api/config` returns valid JSON
   - Swagger endpoint (informational — never fails, since Swagger may be disabled in prod)
4. **Summary:** Reports pass/fail for each check.

#### Manual trigger

```bash
# Test against production
gh workflow run "TrashMobProd - Smoke Tests"

# Test against a custom URL
gh workflow run "TrashMobProd - Smoke Tests" -f base_url=https://dev.trashmob.eco
```

#### Troubleshooting

- **Health check fails:** Check Application Insights for startup errors. Common causes: missing environment variables, database connection issues.
- **Workflow doesn't trigger:** Verify the `workflow_run` trigger references the exact workflow name (`TrashMobProd - Container App`). Check that the deployment workflow completed (not just started).

### R14. Certificate Expiry Monitoring

<a id="r14-certificate-expiry-monitoring"></a>

**Workflow:** `.github/workflows/scheduled_cert-expiry-check.yml`
**Config:** `Deploy/cert-expiry-dates.json`

#### How it works

1. **Schedule:** Runs every Monday at 9:00 UTC via cron schedule.
2. **Process:** Reads `Deploy/cert-expiry-dates.json` which contains certificate names, expiry dates, and renewal URLs.
3. **Alerts:** If any certificate expires within 30 days, creates (or updates) a GitHub issue with:
   - `cert-expiry` and `ops` labels
   - List of expired and expiring certificates with renewal links
   - Links to the deployment checklist renewal procedures (R1, R2)
4. **Dedup:** If an open issue with the `cert-expiry` label already exists, it updates that issue instead of creating a new one.

#### Config file format

```json
{
  "certificates": [
    {
      "name": "iOS Distribution Certificate",
      "expiryDate": "2027-01-15",
      "renewUrl": "https://developer.apple.com/account/resources/certificates/list",
      "notes": "Update this date after regenerating the .p12 certificate."
    }
  ]
}
```

#### Maintaining the config

**When you renew a certificate:** Update the `expiryDate` in `Deploy/cert-expiry-dates.json` and commit the change. The weekly check will use the new date.

**Current certificates tracked:**

| Certificate | Expiry | Notes |
|-------------|--------|-------|
| iOS Distribution Certificate | 2027-01-15 | Regenerate .p12 (see R1) |
| App Store Connect API Key | 2099-12-31 | Doesn't expire unless revoked |
| Apple Sign-In Client Secret | 2026-08-20 | Expires every 6 months |
| Android Upload Keystore | 2036-01-15 | 10000-day validity |

#### Manual trigger

```bash
gh workflow run "Check Certificate Expiry Dates"
```

### R15. B2C to Entra User Migration Workflow

<a id="r15-b2c-to-entra-migration-workflow"></a>

**Workflow:** `.github/workflows/manual_b2c-to-entra-migration.yml`
**Script (interactive version):** `Deploy/migrate-b2c-users.ps1`

#### How it works

1. **Trigger:** `workflow_dispatch` only — never runs automatically.
2. **Inputs:**
   - `environment`: `dev` or `production` (determines which GitHub environment and secrets to use)
   - `mode`: `dry-run`, `export-only`, or `full-migration`
   - `export_file`: Path to save/load the export JSON
3. **Phase 1 (Export):** Authenticates to B2C tenant via client credentials, exports all users via Graph API with pagination, classifies into email/password vs. social-only.
4. **Phase 2 (Import):** Authenticates to Entra tenant, creates email/password users with random passwords. Social-only users are skipped (they self-migrate on first sign-in).
5. **Artifacts:** The export JSON is uploaded as a GitHub Actions artifact (retained 30 days).

#### Prerequisites

The following secrets must be configured in the GitHub environment (`dev` or `production`):

| Secret | Purpose |
|--------|---------|
| `B2C_TENANT_DOMAIN` | B2C tenant domain (e.g., `trashmobdev.onmicrosoft.com`) |
| `B2C_CLIENT_ID` | App registration client ID in B2C tenant |
| `B2C_CLIENT_SECRET` | App registration client secret in B2C tenant |
| `ENTRA_TENANT_ID` | Target Entra External ID tenant GUID |
| `ENTRA_TENANT_DOMAIN` | Entra tenant domain (e.g., `trashmobecodev.onmicrosoft.com`) |
| `ENTRA_CLIENT_ID` | App registration client ID in Entra tenant |
| `ENTRA_CLIENT_SECRET` | App registration client secret in Entra tenant |

Both app registrations need `User.ReadWrite.All` application permission (not delegated) with admin consent.

#### Usage

```bash
# Dry run (see what would happen)
gh workflow run "B2C to Entra User Migration" -f environment=dev -f mode=dry-run

# Export only (review the data)
gh workflow run "B2C to Entra User Migration" -f environment=dev -f mode=export-only

# Full migration
gh workflow run "B2C to Entra User Migration" -f environment=production -f mode=full-migration
```

#### Post-migration steps

1. Verify users in [Entra admin center](https://entra.microsoft.com)
2. Confirm `/api/config` returns `authProvider: "entra"`
3. Test email sign-in (user clicks "Forgot password" to set new password)
4. Test social sign-in (Google/Facebook/Microsoft buttons)

### R16. Privacy Manifest CI Check

<a id="r16-privacy-manifest-ci-check"></a>

**Workflow:** `.github/workflows/ci_privacy-manifest-check.yml`
**Manifest:** `Planning/PRIVACY_MANIFEST.md`

#### How it works

1. **Trigger:** Runs on pull requests that modify any of these files:
   - `TrashMobMobile/Platforms/iOS/PrivacyInfo.xcprivacy`
   - `TrashMobMobile/Platforms/iOS/Info.plist`
   - `TrashMobMobile/Platforms/MacCatalyst/PrivacyInfo.xcprivacy`
   - `TrashMobMobile/Platforms/Android/AndroidManifest.xml`
   - `Planning/PRIVACY_MANIFEST.md`
   - `TrashMobMobile/TrashMobMobile.csproj` (new NuGet packages)
2. **Detection:** Analyzes the diff to identify privacy-impacting changes (new permissions, API categories, NuGet packages).
3. **Notification:** Adds a PR comment listing what changed and reminding to update App Store / Play Store privacy forms.
4. **Dedup:** Updates existing comment instead of creating duplicates.

#### Privacy Manifest document

`Planning/PRIVACY_MANIFEST.md` is the single source of truth for what data the app collects. It includes:

- iOS `PrivacyInfo.xcprivacy` API declarations with reason codes
- Data collection inventory (personal info, location, usage data, UGC)
- iOS and Android permission lists
- Third-party SDK data flows
- Apple App Privacy and Google Play Data Safety quick reference
- Change log for tracking when data collection changes

#### When to update

Update `Planning/PRIVACY_MANIFEST.md` whenever you:
- Add a new SDK that collects user data
- Add new iOS/Android permissions
- Start collecting a new type of user data
- Change how existing data is shared with third parties

After updating the manifest, update the corresponding store forms:
- **Apple:** [App Store Connect > App Privacy](https://appstoreconnect.apple.com)
- **Google:** [Play Console > Data Safety](https://play.google.com/console)

### R17. Google Play Staged Rollout (Fastlane Supply)

<a id="r17-google-play-staged-rollout"></a>

**Workflow (auto):** `.github/workflows/publish-android.yml` (optional `promote_to_production` input)
**Workflow (manual):** `.github/workflows/manual_android-rollout.yml`
**Fastlane lane:** `android_promote` / `android_rollout` in `fastlane/Fastfile`

#### How it works

1. **During release:** The `publish-android.yml` workflow uploads the AAB to the Google Play internal track via `r0adkll/upload-google-play`. If `promote_to_production: true` is passed, it then uses Fastlane Supply to promote from `internal` → `production` at the specified rollout percentage (default 10%).
2. **Manual rollout adjustment:** Use `manual_android-rollout.yml` to change the rollout percentage (10% → 25% → 50% → 100%) without rebuilding or redeploying.

#### Prerequisites

| Secret | Purpose |
|--------|---------|
| `GCP_SERVICE_ACCOUNT` | Google Cloud service account JSON key with Google Play Developer API access |

The service account must have "Release manager" permissions in Google Play Console.

#### Usage

```bash
# Adjust rollout to 50%
gh workflow run "Android - Adjust Production Rollout" -f rollout_percentage=50

# Full rollout (100%)
gh workflow run "Android - Adjust Production Rollout" -f rollout_percentage=100
```

#### Monitoring rollout

Check rollout status in [Google Play Console](https://play.google.com/console) > Release > Production > Release dashboard. Watch crash rates and ANR rates before increasing rollout percentage.

### R18. App Store Promotion (altool Upload & Fastlane Deliver)

<a id="r18-app-store-promotion"></a>

**Workflow (upload):** `.github/workflows/publish-ios.yml` (uses `xcrun altool`)
**Workflow (submit):** `.github/workflows/manual_ios-submit.yml`
**Fastlane lanes:** `ios_submit` in `fastlane/Fastfile`
**Metadata:** `fastlane/metadata/en-US/`

#### How it works

1. **TestFlight upload:** The `publish-ios.yml` workflow uses `xcrun altool --upload-app` with App Store Connect API key authentication to upload the IPA to TestFlight.
2. **App Store submission:** The `manual_ios-submit.yml` workflow uses `fastlane deliver` to submit the latest TestFlight build for App Store review, including metadata from `fastlane/metadata/en-US/`.

> **Note (2026-02-21):** We attempted to migrate from `xcrun altool` to `fastlane pilot upload` but reverted due to a `string contains null byte` error. Fastlane's Ruby OpenSSL layer could not parse the `.p8` API private key when stored as a multiline GitHub Actions secret. The `xcrun altool` approach works reliably by writing the key to a temp file (`~/private_keys/AuthKey_${API_KEY_ID}.p8`) and cleaning up after upload. PR #2834 reverted this change.

#### Prerequisites

| Secret | Purpose |
|--------|---------|
| `APPSTORE_KEY_ID` | App Store Connect API key ID |
| `APPSTORE_ISSUER_ID` | App Store Connect API issuer ID |
| `APPSTORE_PRIVATE_KEY` | App Store Connect API private key (.p8 content, base64-encoded) |

#### Metadata files

| File | Purpose |
|------|---------|
| `description.txt` | Full app description |
| `keywords.txt` | Search keywords (comma-separated, max 100 chars) |
| `release_notes.txt` | What's New text for this version |
| `privacy_url.txt` | Privacy policy URL |
| `support_url.txt` | Support/contact URL |
| `subtitle.txt` | App subtitle (max 30 chars) |
| `name.txt` | App name |

#### Usage

```bash
# Submit latest TestFlight build for review
gh workflow run "iOS - Submit for App Store Review"

# Submit a specific build number
gh workflow run "iOS - Submit for App Store Review" -f build_number=42
```

#### Updating metadata

Edit the files in `fastlane/metadata/en-US/`, commit, and push. The next `ios_submit` run will use the updated metadata. Apple validates metadata during review — check App Store Connect for any rejections.

### R19. Screenshot Capture (Appium)

<a id="r19-screenshot-capture"></a>

**Workflow:** `.github/workflows/manual_capture-screenshots.yml`
**Tests:** `TrashMobMobile.UITests/Tests/ScreenshotTests.cs`

#### How it works

1. **Trigger:** `workflow_dispatch` with optional API level and authenticated screenshot toggle.
2. **Build:** Publishes the MAUI app as an Android APK.
3. **Emulator:** Starts an Android emulator (pixel_6 profile) via `reactivecircus/android-emulator-runner`.
4. **Appium:** Installs Appium + UiAutomator2 driver, starts the server.
5. **Capture:** Runs `dotnet test` with `Category=Screenshots` filter. Each test navigates to a screen and calls `CaptureScreenshot()`.
6. **Artifacts:** Screenshots are uploaded as a GitHub Actions artifact (retained 30 days).

#### Screenshot tests

| Test | Screen | Filename |
|------|--------|----------|
| `CaptureWelcomeScreen` | Welcome/landing | `01_Welcome.png` |
| `CaptureGetStartedScreen` | Get Started page | `02_GetStarted.png` |
| `CaptureEventsMapScreen` | Events map (main) | `03_EventsMap.png` |
| `CaptureDashboardScreen` | User dashboard | `04_Dashboard.png` |
| `CaptureTeamsScreen` | Teams listing | `05_Teams.png` |
| `CaptureCreateEventScreen` | Create event form | `06_CreateEvent.png` |

Tests marked `[Trait("Category", "Authenticated")]` require a logged-in session. By default these are skipped; enable with `include_authenticated: true`.

#### Usage

```bash
# Capture unauthenticated screenshots (default)
gh workflow run "Capture App Screenshots"

# Include authenticated screens
gh workflow run "Capture App Screenshots" -f include_authenticated=true

# Use a different API level
gh workflow run "Capture App Screenshots" -f emulator_api_level=35
```

#### Adding new screenshots

1. Add a new test method to `ScreenshotTests.cs` with `[Trait("Category", "Screenshots")]`.
2. Use the Appium driver to navigate to the target screen.
3. Call `CaptureScreenshot("07_ScreenName")` with the next number in sequence.
4. If the screen requires authentication, also add `[Trait("Category", "Authenticated")]`.

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
| Route Tracking | Project 4 | GPS route tracing during cleanups, colored polylines on map |
| Area Map Editor | Project 44 | AI area generation (Overpass API), interchanges, city blocks, highway sections, boundary editor |
| Adoptable Areas | Project 44 | Area adoption system with team assignments, co-adoption, cleanup frequency |
| Event Visibility | — | Public/Team-Only/Private event visibility settings |
| Event Photos | — | Photo uploads for events with moderation support |
| Waivers V3 | Project 8 | Community waivers, minor consent, versioning, guardian info |
| Achievements | — | Gamification with 7 achievement types, leaderboard caching |
| Sponsored Adoptions | — | Professional companies, sponsors, sponsored area cleanups |
| Community Prospects | Project 40 | Pipeline management, fit scoring, outreach email cadence |
| Feature Metrics | Project 29 | Application Insights event tracking |
| OpenTelemetry | Project 27 | Migrated from App Insights SDK |
| KeyVault RBAC | Project 26 | Migrated from access policies |
| Database Backups | Project 32 | Configured retention policies |
| Billing Alerts | Project 30 | Azure budget caps, grant monitor, cost runbook |
| Job Opportunities Markdown | Issue #2215 | Markdown editor for job listings admin |
| Auth Migration (B2C > Entra) | Project 1 | Entra External ID sign-in, profile photos, social IDPs |
| Age Gate (COPPA) | Project 1/23 | Under-13 block (Layer 1 client + Layer 2 server), minor flagging |
| Auth Extension | Project 1 | Server-side age verification Container App for Entra |
| Fastlane CI/CD | — | Pilot (TestFlight), Deliver (App Store review), Supply (Google Play rollout) |

---

## Automation Opportunities

The following steps are currently manual but could be automated to reduce errors and speed up future deployments:

| # | Current Manual Step | Automation Suggestion | Status |
|---|--------------------|-----------------------|--------|
| 1 | **Database migrations** (step 88) | `.github/workflows/release_db-migrations.yml` — auto-runs on release push when migration files change. Temporarily opens SQL firewall, retrieves connection string from Key Vault, applies migrations, cleans up. | **Done** |
| 2 | **App Store icon resizing** (steps 69-70) | `Planning/StoreAssets/generate-icons.ps1` — generates all required sizes (1024x1024, 512x512, 192x192, 32x32, 240x240) from the 2500x2500 source PNG. | **Done** |
| 3 | **Google Play Feature Graphic** (step 69) | `Planning/StoreAssets/generate-feature-graphic.ps1` — composites v2 logo onto brand green background at 1024x500 with configurable tagline. | **Done** |
| 4 | **Screenshot capture** (step 77) — automated via Appium | `.github/workflows/manual_capture-screenshots.yml` — `workflow_dispatch` runs Appium UI tests on Android emulator, captures screenshots of key screens, uploads as artifacts. `TrashMobMobile.UITests/Tests/ScreenshotTests.cs` has the test class. | **Done** |
| 5 | **Apple TestFlight > App Store promotion** (step 97) — Fastlane Deliver | `.github/workflows/manual_ios-submit.yml` — `workflow_dispatch` submits latest TestFlight build for App Store review using Fastlane Deliver with metadata from `fastlane/metadata/en-US/`. iOS publish workflow uses `xcrun altool` for TestFlight upload (Fastlane Pilot reverted due to null byte error with multiline secrets — see R18). | **Done** |
| 6 | **Google Play staged rollout** (step 98) — Fastlane Supply | `.github/workflows/manual_android-rollout.yml` — `workflow_dispatch` to adjust production rollout (10%/25%/50%/100%). Android publish workflow updated with optional `promote_to_production` input using Fastlane Supply. | **Done** |
| 7 | **Post-deployment smoke tests** (step 99) — automated health checks | `.github/workflows/release_smoke-tests.yml` — runs automatically after container app deployment. Checks site HTTP status, `/health`, `/health/live`, `/api/config`, and Swagger. | **Done** |
| 8 | **Apple signing cert expiry monitoring** (step 62) — weekly automated check | `.github/workflows/scheduled_cert-expiry-check.yml` — runs weekly (Monday 9am UTC), reads `Deploy/cert-expiry-dates.json`, creates GitHub issue if any cert expires within 30 days. | **Done** |
| 9 | **B2C > Entra user migration** (step 89) — automated workflow | `.github/workflows/manual_b2c-to-entra-migration.yml` — `workflow_dispatch` with environment (dev/prod) and mode (dry-run/export-only/full-migration). Exports B2C users, imports to Entra via Graph API. | **Done** |
| 10 | **Data Safety / App Privacy form updates** (steps 81-83) — CI check | `Planning/PRIVACY_MANIFEST.md` tracks all data collection; `.github/workflows/ci_privacy-manifest-check.yml` adds PR comment when privacy-related files change. | **Done** |

All 10 automation opportunities have been implemented.

---

## Part E — Post-Cutover Cleanup :hand:

After the production deployment is stable and the B2C coexistence window has ended (1 week post-launch), clean up old credentials and services that are no longer needed.

---

### E1. Google Cloud Console Cleanup

- [ ] **165.** Go to [Google Cloud Console](https://console.cloud.google.com) > select the TrashMob project > APIs & Services > Credentials
- [ ] **166.** Identify old OAuth client IDs that were used for Azure B2C (the redirect URI will point to `*.b2clogin.com`)
- [ ] **167.** Delete old B2C OAuth client IDs — they are no longer needed since sign-in now goes through Entra External ID
- [ ] **168.** Verify the new Entra prod OAuth client ID (`TrashMob Entra Prod`) still works — test Google sign-in on www.trashmob.eco
- [ ] **169.** Review API key restrictions: ensure the Maps SDK key is restricted to the correct bundle IDs and referrer URLs
- [ ] **170.** Review enabled APIs — disable any that are no longer used (e.g., if B2C required a specific API)

---

### E2. Facebook Developer Console Cleanup

- [ ] **171.** Go to [Facebook for Developers](https://developers.facebook.com) > My Apps > select the TrashMob app
- [ ] **172.** Settings > Basic — review the Valid OAuth Redirect URIs
- [ ] **173.** Remove old B2C redirect URIs (any pointing to `*.b2clogin.com`) — these are no longer used
- [ ] **174.** Verify only the Entra prod redirect URI remains:
  ```
  https://trashmobecopr.ciamlogin.com/trashmobecopr.onmicrosoft.com/federation/oauth2
  ```
- [ ] **175.** Also keep the Entra dev redirect URI if you want dev to keep working:
  ```
  https://trashmobecodev.ciamlogin.com/trashmobecodev.onmicrosoft.com/federation/oauth2
  ```
- [ ] **176.** Test Facebook sign-in on www.trashmob.eco after removing old URIs
- [ ] **177.** Review App Roles — remove any test users that are no longer needed

---

### E3. Apple Developer Console Cleanup

- [ ] **178.** Go to [Apple Developer](https://developer.apple.com/account/resources/identifiers/list/serviceId) > Certificates, Identifiers & Profiles > Identifiers > Service IDs
- [ ] **179.** Identify old Service IDs that were used for Azure B2C sign-in (return URLs pointing to `*.b2clogin.com`)
- [ ] **180.** Delete or disable old B2C Service IDs — they are no longer needed
- [ ] **181.** Verify the new Entra prod Service ID (`eco.trashmob.entra` or similar) has the correct return URL:
  ```
  https://trashmobecopr.ciamlogin.com/trashmobecopr.onmicrosoft.com/federation/oidc/apple
  ```
- [ ] **182.** Keys: Go to [Keys](https://developer.apple.com/account/resources/authkeys/list) — if you created a new Sign in with Apple key for Entra, verify the old B2C key is no longer referenced anywhere, then revoke it
  - **Caution:** Do NOT revoke the key if it's shared between B2C and Entra. Only revoke if separate keys were created.
- [ ] **183.** Regenerate the Apple client secret JWT (expires every 6 months) and update the calendar reminder:
  - Script: `d:/tools/Apple/generate-apple-secret.js`
  - Update `Deploy/cert-expiry-dates.json` with the new expiry date
- [ ] **184.** Test Apple sign-in on www.trashmob.eco after cleanup

---

### E4. Azure B2C Tenant Decommission

- [ ] **185.** Confirm all users have been migrated to Entra (compare user counts)
- [ ] **186.** Confirm no traffic is going to the B2C tenant (check B2C audit logs for 1 week)
- [ ] **187.** Remove B2C-related secrets from Key Vault (`kv-tm-pr-westus2`):
  - `AzureAdB2C--*` prefixed secrets (if any)
  - Any B2C client secrets
- [ ] **188.** Remove B2C configuration from `Deploy/containerApp.bicep` environment variables
- [ ] **189.** Remove B2C-related code from the codebase (controllers, middleware, config classes)
- [ ] **190.** Delete B2C app registrations in the B2C tenant
- [ ] **191.** Schedule B2C tenant deletion (Azure Portal > Azure AD B2C > Overview > Delete tenant)
  - **Warning:** Tenant deletion is irreversible. Ensure all user data is migrated and verified first.

---

### E5. GitHub Secrets Cleanup

- [ ] **192.** Review GitHub Actions secrets in both `test` and `production` environments
- [ ] **193.** Remove B2C-related secrets that are no longer needed:
  - `B2C_TENANT_DOMAIN`, `B2C_CLIENT_ID`, `B2C_CLIENT_SECRET` (if the migration workflow is no longer needed)
- [ ] **194.** Verify all Entra-related secrets are correctly set for production
- [ ] **195.** Remove any temporary secrets created during the migration

---

### E6. Custom Auth Domain (Post-Launch Enhancement)

Replace `trashmobecopr.ciamlogin.com` with a branded domain (e.g., `auth.trashmob.eco`) so users see your domain during sign-in instead of a Microsoft domain.

- [ ] **196.** Add DNS CNAME record: `auth.trashmob.eco` → `trashmobecopr.ciamlogin.com`
- [ ] **197.** Configure custom URL domain in Entra portal: External Identities > Custom URL domains > Add `auth.trashmob.eco`
- [ ] **198.** Update redirect URIs in all 3 app registrations (Web SPA, API, Mobile) to use `auth.trashmob.eco`
- [ ] **199.** Update social IDP redirect URIs (Google, Facebook, Apple) to use `auth.trashmob.eco`
- [ ] **200.** Update `Deploy/containerApp.bicep` `entraInstance` to `https://auth.trashmob.eco/`
- [ ] **201.** Update mobile `AuthConstants.cs` with new auth domain
- [ ] **202.** Test sign-in on web and mobile with the new domain

---

## Contacts

- **Engineering:** @JoeBeernink
- **Emergency:** Check #engineering in Slack

---

**Remember:** Test in dev first! https://dev.trashmob.eco
