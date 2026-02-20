# Production Deployment Checklist

**Last Updated:** February 20, 2026
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

- [ ] **1.** Verify/create Strapi DB password (dev):
  ```bash
  az keyvault secret show --vault-name kv-tm-dev-westus2 --name strapi-db-password || \
    az keyvault secret set --vault-name kv-tm-dev-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
  ```
- [ ] **2.** Verify/create Anthropic API key (dev):
  ```bash
  az keyvault secret show --vault-name kv-tm-dev-westus2 --name AnthropicApiKey || \
    az keyvault secret set --vault-name kv-tm-dev-westus2 --name AnthropicApiKey --value "<your-anthropic-api-key>"
  ```
  Get key from https://console.anthropic.com/settings/keys
- [ ] **3.** Verify/create Strapi DB password (prod):
  ```bash
  az keyvault secret show --vault-name kv-tm-pr-westus2 --name strapi-db-password || \
    az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
  ```
- [ ] **4.** Verify/create Anthropic API key (prod):
  ```bash
  az keyvault secret show --vault-name kv-tm-pr-westus2 --name AnthropicApiKey || \
    az keyvault secret set --vault-name kv-tm-pr-westus2 --name AnthropicApiKey --value "<your-anthropic-api-key>"
  ```

---

### A2. Infrastructure Deployments :hand:

- [ ] **5.** Deploy Application Insights Workbook (recommended):
  ```bash
  az account set --subscription "TrashMobProd"
  az deployment group create \
    --resource-group rg-trashmob-pr-westus2 \
    --template-file Deploy/appInsightsWorkbook.bicep \
    --parameters environment=pr region=westus2 \
      rgName=rg-trashmob-pr-westus2 \
      appInsightsName=ai-tm-pr-westus2
  ```
- [ ] **6.** Deploy Backup Alerts (if not already deployed):
  ```bash
  az deployment group create \
    --resource-group rg-trashmob-pr-westus2 \
    --template-file Deploy/backupAlerts.bicep \
    --parameters environment=pr region=westus2
  ```
- [ ] **7.** Deploy Billing Alerts action group:
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
- [ ] **9.** Create SendGrid alerts: https://app.sendgrid.com > Settings > Billing > alerts at 75% and 90% (recipient: joe@trashmob.eco)
- [ ] **10.** Create Google Maps API alerts: https://console.cloud.google.com > Billing > Budgets & alerts > "TrashMob Maps API" at $100/month with 50%, 90%, 100%
- [ ] **11.** Verify KeyVault RBAC is working (PR #2482 migrated from access policies to RBAC)

---

### A3. Strapi CMS (Optional — only if deploying to prod for first time) :hand:

- [ ] **12.** Create KeyVault secrets for Strapi:
  ```bash
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-admin-jwt-secret --value "$(openssl rand -base64 32)"
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-api-token-salt --value "$(openssl rand -base64 32)"
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-app-keys --value "$(openssl rand -base64 32),$(openssl rand -base64 32)"
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-transfer-token-salt --value "$(openssl rand -base64 32)"
  ```
- [ ] **13.** Deploy Strapi database:
  ```bash
  az deployment group create \
    --resource-group rg-trashmob-pr-westus2 \
    --template-file Deploy/sqlDatabaseStrapi.bicep \
    --parameters environment=pr region=westus2
  ```
- [ ] **14.** Create production workflow (copy from dev, update environment variables). Workflow auto-creates SQL user and deploys container.

---

### A4. Entra External ID — Auth Migration (Project 1) :hand:

This is a **downtime deployment** — B2C is fully replaced by Entra External ID. Complete ALL steps below before merging to release.

**Reference:** `Planning/Projects/Project_01_Auth_Revamp.md` and `Planning/TechnicalDesigns/Auth_Migration.md`

#### A4.1 Create Production Entra Tenant

- [ ] **15.** Azure Portal > Microsoft Entra External ID > Create **Customer** tenant
  - Tenant name: `TrashMobEco`
  - Domain: `trashmobeco.onmicrosoft.com`
  - CIAM domain: `trashmobeco.ciamlogin.com`
  - Location: United States
- [ ] **16.** Record the **Tenant ID** (GUID from Overview page)

#### A4.2 Register App Registrations

Login to the prod tenant first: `az login --tenant <prod-entra-tenant-id> --allow-no-subscriptions`

- [ ] **17.** **Web SPA (Frontend):** Name: `TrashMob Web`, Redirect URIs (SPA): `https://www.trashmob.eco`, `https://trashmob.eco`, check ID tokens, uncheck Access tokens. Record **Application (client) ID** → `FrontendClientId`
- [ ] **18.** **Backend API:** Name: `TrashMob API`, Expose an API → URI: `api://<client-id>`, Add scopes: `TrashMob.Read`, `TrashMob.Writes`. Record **Application (client) ID** → `ClientId`
- [ ] **19.** **Mobile App:** Name: `TrashMob Mobile`, Redirect URI (Public client): `eco.trashmob.trashmobmobile://auth`. Record **Application (client) ID**
- [ ] **20.** **Auth Extension (Layer 2):** Name: `TrashMob AuthExtension`. Record **Application (client) ID** → JWT audience validation
- [ ] **21.** **Grant API Permissions:** Web SPA + Mobile App → add `TrashMob.Read` + `TrashMob.Writes` → Grant admin consent

#### A4.3 Configure Optional Claims

- [ ] **22.** Run configure script:
  ```bash
  az login --tenant <prod-entra-tenant-id> --allow-no-subscriptions
  .\Deploy\configure-entra-apps.ps1 -Environment pr
  ```
  Sets: `email`, `given_name`, `family_name`, `preferred_username`, `acceptMappedClaims: true`, `isFallbackPublicClient: true` (mobile only)

#### A4.4 Configure Social Identity Providers

In Azure Portal > prod Entra tenant > External Identities > All identity providers:

- [ ] **23.** **Google:** Create OAuth 2.0 credentials in Google Cloud Console, add redirect URI `https://trashmobeco.ciamlogin.com/trashmobeco.onmicrosoft.com/federation/oauth2`, enter Client ID + secret in Azure
- [ ] **24.** **Facebook:** Add OAuth redirect URI in Facebook Developer Console, enter App ID + secret in Azure
- [ ] **25.** **Apple:**
  1. Apple Developer > Certificates, Identifiers & Profiles > Identifiers > **+** > **Services IDs**
  2. Register Service ID (e.g., `eco.trashmob.entra`) with **Sign In with Apple** enabled
  3. Configure return URL: `https://trashmobeco.ciamlogin.com/trashmobeco.onmicrosoft.com/federation/oidc/apple`
  4. If no `.p8` key file: create under Keys > **+** > check Sign in with Apple > download (one-time)
  5. Generate Apple client secret JWT using `d:/tools/Apple/generate-apple-secret.js` (update KEY_ID, SERVICE_ID, KEY_FILE for prod)
  6. Enter Service ID + generated JWT in Entra Apple IDP config
  7. **Set calendar reminder:** Client secret expires in 6 months — must regenerate
- [ ] **26.** **Microsoft:** Enabled by default — verify it's active

#### A4.5 Create User Flow

- [ ] **27.** User flows > New user flow > "Sign up and sign in" named `SignUpSignIn`
  - Identity providers: all configured (Google, Microsoft, Apple, Facebook, Email)
  - Attributes: Email (required), Given Name (required), Surname (required)
- [ ] **28.** Create custom attribute: External Identities > Custom user attributes > Add `dateOfBirth` (String type)
- [ ] **29.** Add `dateOfBirth` to the user flow's attribute collection page

#### A4.6 Configure Token Claims

- [ ] **30.** For each app registration (Web SPA, API, Mobile): Token configuration > Add optional claims (ID + Access tokens): `given_name`, `family_name`, `email` (built-in), `dateOfBirth` (directory schema extension)
- [ ] **31.** Verify `acceptMappedClaims: true` in each app's Manifest
- [ ] **32.** Test: sign in and decode JWT at https://jwt.ms to verify claims

#### A4.7 Configure Branding

- [ ] **33.** Company branding > Default sign-in experience:
  - Banner logo: TrashMob logo (260x36 px) — use `Planning/StoreAssets/HorizontalLogo_Source.svg` resized
  - Background image: TrashMob hero image (1920x1080 px) — use `Images/v1/TME_SignInBackground_1920x1080.png`
  - Background color: `#96ba00`
  - Sign-in text: "Welcome to TrashMob.eco — Join the movement to clean up the planet!"
  - Layout: Full-screen background template
- [ ] **34.** Test in incognito browser

#### A4.8 User Migration (B2C > Entra)

- [ ] **35.** Run migration script to export B2C users > import to Entra External ID (see `Deploy/migrate-b2c-users.ps1`)
- [ ] **36.** Verify migrated user count matches B2C
- [ ] **37.** Test sign-in with a few migrated accounts
- [ ] **38.** Confirm: existing users without DateOfBirth are grandfathered as adults

#### A4.9 Update Production Configuration

- [ ] **39.** Set backend config (Key Vault or env vars):
  ```
  AzureAdEntra__Instance=https://trashmobeco.ciamlogin.com/
  AzureAdEntra__ClientId=<API app client ID>
  AzureAdEntra__FrontendClientId=<Web SPA client ID>
  AzureAdEntra__Domain=trashmobeco.onmicrosoft.com
  AzureAdEntra__TenantId=<prod tenant ID>
  UseEntraExternalId=true
  ```
- [ ] **40.** Update `Deploy/containerApp.bicep` prod environment variables with prod Entra values
- [ ] **41.** Update `Deploy/configure-entra-apps.ps1` with prod app registration IDs

#### A4.10 Deploy Auth Extension Container App (Layer 2)

- [ ] **42.** Set GitHub Actions secrets (in `production` environment): `ENTRA_TENANT_ID`, `AUTH_EXTENSION_CLIENT_ID`
- [ ] **43.** Create production workflow: copy `.github/workflows/container_ca-authext-tm-dev-westus2.yml` > `release_ca-authext-tm-pr-westus2.yml`, update registry/container/RG, trigger on `release` branch
- [ ] **44.** Register Custom Authentication Extension in Entra portal:
  - External Identities > Custom authentication extensions > Create
  - Type: `OnAttributeCollectionSubmit`
  - Target URL: `https://ca-authext-tm-pr-westus2.<fqdn>/api/authext/attributecollectionsubmit`
  - Link to auth extension app registration
  - Assign to user flow's "When a user submits their information" event

#### A4.11 Mobile App Update

- [ ] **45.** Verify `AuthConstants.cs` has correct prod Entra values
- [ ] **46.** Build and test on Android emulator + iOS simulator with prod tenant
- [ ] **47.** Submit to Google Play Store and Apple App Store
- [ ] **48.** Consider force-update flow for users on old B2C version

#### A4.12 Pre-Cutover Verification (on dev.trashmob.eco) :test_tube:

- [ ] **49.** Web sign-in via email/password — succeeds, JWT contains expected claims
- [ ] **50.** Web sign-in via Google — succeeds, profile photo populated
- [ ] **51.** Web sign-in via Facebook — succeeds
- [ ] **52.** Web "Create Account" — shows age gate, blocks under-13, allows 13+
- [ ] **53.** Web "Sign In" — goes directly to Entra (no age gate)
- [ ] **54.** Web "Attend" (unauthenticated) — shows age gate before redirect
- [ ] **55.** Mobile sign-in — Entra External ID (not B2C)
- [ ] **56.** Mobile "Create Account" — AgeGatePage blocks under-13
- [ ] **57.** Auth extension — POST with under-13 DOB returns `showBlockPage`
- [ ] **58.** Profile edit — in-app edit works (name, photo upload)
- [ ] **59.** Account deletion — "Delete My Data" works with typed confirmation
- [ ] **60.** Auto-create user — new sign-up creates DB user from token claims
- [ ] **61.** Migrated user sign-in — existing B2C user signs in via Entra successfully

---

### A5. Apple Signing & API Key Renewal :hand:

- [ ] **62.** :gear: **AUTOMATED:** `scheduled_cert-expiry-check.yml` runs weekly (Mondays 9am UTC) and creates a GitHub issue if any certificate expires within 30 days. Update `Deploy/cert-expiry-dates.json` when renewing certificates. (See [Section R14](#r14-certificate-expiry-monitoring))
- [ ] **63.** :test_tube: Verify `Deploy/cert-expiry-dates.json` has current expiry dates for all certificates
- [ ] **64.** Check iOS distribution certificate expiry at https://developer.apple.com/account/resources/certificates/list
- [ ] **65.** Check App Store Connect API key status at https://appstoreconnect.apple.com/access/integrations/api — verify key used by `APPSTORE_KEY_ID` is "Active"
- [ ] **66.** If certificate expired or expiring — regenerate (see [Section R1](#r1-regenerate-ios-distribution-certificate))
- [ ] **67.** If API key expired or revoked — regenerate (see [Section R2](#r2-regenerate-app-store-connect-api-key))
- [ ] **68.** Verify Android keystore — `ANDROID_KEYSTORE` and `ANDROID_KEYSTORE_PASSWORD` secrets are set (see [Section R3](#r3-android-keystore-rotation) if rotation needed)

---

### A6. App Store Logos & Icons Review :hand:

Verify all store logos are current (v2 branding) and correctly sized. Source assets are in `Planning/StoreAssets/`.

**Required icon sizes:**

| Store | Asset | Required Size | Source File | Status |
|-------|-------|---------------|-------------|--------|
| Apple App Store | App Icon | 1024x1024 PNG (no transparency) | Resize from `AppIcon_2500x2500.png` | - [ ] Ready |
| Google Play | Hi-res Icon | 512x512 PNG (32-bit) | Resize from `AppIcon_2500x2500.png` | - [ ] Ready |
| Google Play | Feature Graphic | 1024x500 PNG/JPG | `GooglePlay_FeatureGraphic_1024x500.png` (v1 — may need refresh) | - [ ] Reviewed |
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
| `GooglePlay_FeatureGraphic_1024x500.png` | 1024x500 | Google Play feature graphic (v1 — review for refresh) |

Additional source files at `D:\data\images\v2\New TrashMob.eco files\New TrashMob.eco files\` (Illustrator, PDF, JPG, PNG, SVG formats).

- [ ] **69.** Generate all icon sizes — run `.\Planning\StoreAssets\generate-icons.ps1` (outputs to `Planning/StoreAssets/Generated/`):
  - `AppStore_1024x1024.png` — Apple App Store (no transparency, no rounded corners)
  - `GooglePlay_512x512.png` — Google Play (32-bit PNG)
  - Plus PWA, favicon, and Entra profile sizes
- [ ] **70.** Generate feature graphic — run `.\Planning\StoreAssets\generate-feature-graphic.ps1`:
  - Outputs `GooglePlay_FeatureGraphic_1024x500.png` with v2 logo on brand green background
  - Adjust `-Tagline`, `-BackgroundColor` parameters if needed
- [ ] **71.** Review generated images visually before uploading to stores
- [ ] **72.** Verify app icon in Apple App Store Connect matches v2 logo
- [ ] **73.** Verify app icon in Google Play Console matches v2 logo

---

### A7. App Store Listing & Screenshots :hand:

- [ ] **74.** Update app store listing copy (see [Section R4](#r4-app-store-listing-copy))
- [ ] **75.** Update release notes (see [Section R5](#r5-release-notes-template))
- [ ] **76.** Update keywords (Apple, see [Section R6](#r6-keywords))
- [ ] **77.** Capture new screenshots if significant UI changes (see [Section R7](#r7-screenshot-guide))
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

- [ ] **86.** Deploy to dev.trashmob.eco and verify all features work before proceeding to production
- [ ] **87.** Run through feature testing checklist (Part C below) on dev first

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

- [ ] **89.** :gear: **AUTOMATED:** Final B2C > Entra user migration — run `manual_b2c-to-entra-migration.yml` with `mode=full-migration` to catch any new users since last migration (see [Section R15](#r15-b2c-to-entra-migration-workflow))
- [ ] **90.** Verify frontend returns `authProvider: "entra"` from `/api/config` endpoint after deployment

---

### B3. Merge and Deploy Web App :hand: :gear:

- [ ] **91.** Merge main to release:
  ```bash
  git checkout release
  git pull origin release
  git merge origin/main
  git push origin release
  ```
- [ ] **92.** :gear: **AUTOMATED:** GitHub Actions builds and deploys web container to Azure Container Apps
- [ ] **93.** :gear: **AUTOMATED:** GitHub Actions builds and deploys background jobs (daily + hourly)
- [ ] **94.** Monitor GitHub Actions for success: https://github.com/TrashMob-eco/TrashMob/actions

---

### B4. Deploy Mobile Apps :hand: :gear:

- [ ] **95.** :gear: **AUTOMATED:** Push to `release` triggers `release_trashmobmobileapp.yml` which:
  - Builds Android AAB, signs with keystore, uploads to Google Play via GCP service account
  - Builds iOS IPA, signs with distribution cert, uploads to TestFlight via `xcrun altool`
- [ ] **96.** Monitor mobile build workflow for success
- [ ] **97.** :hand: **Apple:** Promote TestFlight build to App Store review in App Store Connect
- [ ] **98.** :hand: **Google:** Promote internal track build to production in Google Play Console (recommend staged rollout: 10% > 50% > 100%)

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

#### Auth Migration — Entra External ID (Project 1)
- [ ] **132.** Sign in via email/password works
- [ ] **133.** Sign in via Google works, profile photo auto-populated
- [ ] **134.** Sign in via Facebook works
- [ ] **135.** Sign in via Apple works
- [ ] **136.** "Create Account" shows age gate before Entra redirect
- [ ] **137.** Age gate blocks under-13 with friendly message
- [ ] **138.** "Sign In" goes directly to Entra (no age gate)
- [ ] **139.** Profile edit works in-app (name, photo upload)
- [ ] **140.** "Delete My Data" works with typed DELETE confirmation
- [ ] **141.** Migrated B2C user can sign in via Entra
- [ ] **142.** New user auto-created in DB on first sign-in
- [ ] **143.** Auth extension blocks under-13 sign-up server-side
- [ ] **144.** JWT contains expected claims: email, given_name, family_name, dateOfBirth
- [ ] **145.** No auth errors in Application Insights after 1 hour

---

### C3. Post-Launch Monitoring

- [ ] **146.** Monitor Application Insights for auth errors for 24 hours
- [ ] **147.** Monitor Sentry.io for mobile crashes
- [ ] **148.** Monitor Google Play pre-launch report for crashes
- [ ] **149.** After 1-week coexistence: decommission B2C tenant

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

### D2. Auth Rollback (B2C Fallback)

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
| Auth Migration (B2C > Entra) | Project 1 | Entra External ID sign-in, profile photos, social IDPs |
| Age Gate (COPPA) | Project 1/23 | Under-13 block (Layer 1 client + Layer 2 server), minor flagging |
| Auth Extension | Project 1 | Server-side age verification Container App for Entra |

---

## Automation Opportunities

The following steps are currently manual but could be automated to reduce errors and speed up future deployments:

| # | Current Manual Step | Automation Suggestion | Status |
|---|--------------------|-----------------------|--------|
| 1 | **Database migrations** (step 85) | `.github/workflows/release_db-migrations.yml` — auto-runs on release push when migration files change. Temporarily opens SQL firewall, retrieves connection string from Key Vault, applies migrations, cleans up. | **Done** |
| 2 | **App Store icon resizing** (steps 67-68) | `Planning/StoreAssets/generate-icons.ps1` — generates all required sizes (1024x1024, 512x512, 192x192, 32x32, 240x240) from the 2500x2500 source PNG. | **Done** |
| 3 | **Google Play Feature Graphic** (step 69) | `Planning/StoreAssets/generate-feature-graphic.ps1` — composites v2 logo onto brand green background at 1024x500 with configurable tagline. | **Done** |
| 4 | **Screenshot capture** (step 75) — manual emulator/simulator screenshots | Use [Fastlane Snapshot](https://docs.fastlane.tools/actions/snapshot/) (iOS) and [Fastlane Screengrab](https://docs.fastlane.tools/actions/screengrab/) (Android) to automate screenshot capture across device sizes. Requires UI test setup. | High |
| 5 | **Apple TestFlight > App Store promotion** (step 94) — manual click in App Store Connect | Use [Fastlane Deliver](https://docs.fastlane.tools/actions/deliver/) to automate App Store submission including metadata, screenshots, and build promotion. | Medium |
| 6 | **Google Play staged rollout** (step 95) — manual in Google Play Console | Use [Fastlane Supply](https://docs.fastlane.tools/actions/supply/) to automate Google Play uploads and rollout percentage management from CI. | Medium |
| 7 | **Post-deployment smoke tests** (step 99) — automated health checks | `.github/workflows/release_smoke-tests.yml` — runs automatically after container app deployment. Checks site HTTP status, `/health`, `/health/live`, `/api/config`, and Swagger. | **Done** |
| 8 | **Apple signing cert expiry monitoring** (step 62) — weekly automated check | `.github/workflows/scheduled_cert-expiry-check.yml` — runs weekly (Monday 9am UTC), reads `Deploy/cert-expiry-dates.json`, creates GitHub issue if any cert expires within 30 days. | **Done** |
| 9 | **B2C > Entra user migration** (step 89) — automated workflow | `.github/workflows/manual_b2c-to-entra-migration.yml` — `workflow_dispatch` with environment (dev/prod) and mode (dry-run/export-only/full-migration). Exports B2C users, imports to Entra via Graph API. | **Done** |
| 10 | **Data Safety / App Privacy form updates** (steps 81-83) — CI check | `Planning/PRIVACY_MANIFEST.md` tracks all data collection; `.github/workflows/ci_privacy-manifest-check.yml` adds PR comment when privacy-related files change. | **Done** |

All 10 automation opportunities have been implemented.

---

## Contacts

- **Engineering:** @JoeBeernink
- **Emergency:** Check #engineering in Slack

---

**Remember:** Test in dev first! https://dev.trashmob.eco
