# TrashMob.eco — Privo Integration Package

**Document Version:** 1.0
**Date:** January 31, 2026
**Prepared for:** Privo.com Integration Team
**Contact:** *(Contact addresses stored in secure location - not in public repo)*

---

## 1. Business Information

| Field | Value |
|-------|-------|
| **Company Name** | TrashMob.eco |
| **Website** | https://www.trashmob.eco |
| **Business Type** | Non-profit volunteer coordination platform |
| **Primary Service** | Connecting volunteers with community cleanup events |
| **Target Audience** | Adults (18+) and minors (13-17) with parental consent |
| **Privacy Policy** | https://www.trashmob.eco/privacypolicy |
| **Terms of Service** | https://www.trashmob.eco/termsofservice |

### Company Address
*(To be provided by Business Team)*

### Primary Contacts
*(To be provided by Business Team)*

---

## 2. Authentication Flow — Swim Lane Diagram

### Current Architecture: Microsoft Entra External ID (CIAM)
> **Note:** TrashMob completed migration from Azure AD B2C to Microsoft Entra External ID in February 2026 (Project 1).
> The Privo integration points remain the same — Privo hooks into the Custom Authentication Extension on the sign-up user flow.

```
┌─────────────────────────────────────────────────────────────────────────────────────────────────────┐
│                              MINOR REGISTRATION FLOW WITH PRIVO                                      │
└─────────────────────────────────────────────────────────────────────────────────────────────────────┘

   USER (Minor)          TrashMob Web/Mobile       Entra External ID      Privo.com            Parent
        │                        │                      │                     │                   │
        │  1. Click "Sign Up"    │                      │                     │                   │
        │───────────────────────>│                      │                     │                   │
        │                        │                      │                     │                   │
        │                        │  2. Redirect to CIAM │                     │                   │
        │                        │─────────────────────>│                     │                   │
        │                        │                      │                     │                   │
        │  3. Enter Email/Password                      │                     │                   │
        │<─────────────────────────────────────────────>│                     │                   │
        │                        │                      │                     │                   │
        │                        │  4. Auth Success     │                     │                   │
        │                        │<─────────────────────│                     │                   │
        │                        │                      │                     │                   │
        │  5. Enter Date of Birth│                      │                     │                   │
        │<───────────────────────│                      │                     │                   │
        │───────────────────────>│                      │                     │                   │
        │                        │                      │                     │                   │
        │                        │  6. Verify Age       │                     │                   │
        │                        │───────────────────────────────────────────>│                   │
        │                        │                      │                     │                   │
        │                        │  7. Age Result       │                     │                   │
        │                        │<───────────────────────────────────────────│                   │
        │                        │                      │                     │                   │
        │                        │                      │                     │                   │
┌───────┴────────────────────────┴──────────────────────┴─────────────────────┴───────────────────┴───┐
│                                    AGE VERIFICATION RESULTS                                          │
├─────────────────────────────────────────────────────────────────────────────────────────────────────┤
│  UNDER 13: Block registration, show explanation, suggest parent creates account for them            │
│  13-17 (MINOR): Continue to Parental Consent Flow (below)                                           │
│  18+ (ADULT): Continue to standard registration, skip Privo consent                                 │
└─────────────────────────────────────────────────────────────────────────────────────────────────────┘
        │                        │                      │                     │                   │
        │                        │                      │                     │                   │
═══════════════════════════════════════════════════════════════════════════════════════════════════════
                              PARENTAL CONSENT FLOW (13-17 MINORS ONLY)
═══════════════════════════════════════════════════════════════════════════════════════════════════════
        │                        │                      │                     │                   │
        │  8. Enter Parent Email │                      │                     │                   │
        │<───────────────────────│                      │                     │                   │
        │───────────────────────>│                      │                     │                   │
        │                        │                      │                     │                   │
        │                        │  9. Request VPC      │                     │                   │
        │                        │───────────────────────────────────────────>│                   │
        │                        │                      │                     │                   │
        │                        │  10. VPC Request ID  │                     │                   │
        │                        │<───────────────────────────────────────────│                   │
        │                        │                      │                     │                   │
        │  11. "Pending" Status  │                      │                     │                   │
        │      Limited Access    │                      │                     │                   │
        │<───────────────────────│                      │                     │                   │
        │                        │                      │                     │                   │
        │                        │                      │  12. Send Consent   │                   │
        │                        │                      │      Email          │                   │
        │                        │                      │     ─────────────────────────────────────>│
        │                        │                      │                     │                   │
        │                        │                      │                     │  13. Parent       │
        │                        │                      │                     │      Clicks Link  │
        │                        │                      │                     │<──────────────────│
        │                        │                      │                     │                   │
        │                        │                      │                     │  14. Verify       │
        │                        │                      │                     │      Identity     │
        │                        │                      │                     │      (CC/ID/Video)│
        │                        │                      │                     │<─────────────────>│
        │                        │                      │                     │                   │
        │                        │  15. Webhook:        │                     │                   │
        │                        │      Consent Status  │                     │                   │
        │                        │<───────────────────────────────────────────│                   │
        │                        │                      │                     │                   │
┌───────┴────────────────────────┴──────────────────────┴─────────────────────┴───────────────────┴───┐
│                                    CONSENT RESULTS                                                   │
├─────────────────────────────────────────────────────────────────────────────────────────────────────┤
│  VERIFIED: Minor account fully activated with protections (no DMs, limited profile visibility)      │
│  DENIED: Minor account disabled, data deleted per COPPA                                             │
│  TIMEOUT (7 days): Minor account disabled, can retry with new parent email                          │
│  REVOKED (later): Immediate account suspension, data retained per legal requirements                │
└─────────────────────────────────────────────────────────────────────────────────────────────────────┘
        │                        │                      │                     │                   │
        │  16. Full Access       │                      │                     │                   │
        │      (with protections)│                      │                     │                   │
        │<───────────────────────│                      │                     │                   │
        │                        │                      │                     │                   │
        │                        │  17. Send Location   │                     │                   │
        │                        │      to Privo        │                     │                   │
        │                        │───────────────────────────────────────────>│                   │
        │                        │                      │                     │                   │
```

---

## 3. Feature Set Requiring Parental Consent

The following features require Verifiable Parental Consent (VPC) for users aged 13-17:

### Core Platform Features

| Feature | Description | Data Collected |
|---------|-------------|----------------|
| **Newsletter subscriptions** | Email communications about events and updates | Email address |
| **In-app notifications** | Push and in-app alerts about events | Device tokens |
| **Event sign-up** | Register to attend cleanup events | Name, email, attendance |
| **Create an event** | Organize cleanup events | Name, location, contact |
| **Photo uploads** | Upload before/during/after event photos | Images, metadata |
| **Profile photo** | Set a profile picture | Image |
| **Join a team** | Become member of a cleanup team | Membership data |

### Location & Tracking Features (Sensitive)

| Feature | Description | Data Collected |
|---------|-------------|----------------|
| **Geolocation sharing** | Share location for nearby events | GPS coordinates |
| **Route tracing** | Track cleanup route during events | GPS path, timestamps |
| **Litter report submission** | Report litter locations | GPS coordinates, photos |

### Public Visibility Features

| Feature | Description | Data Collected |
|---------|-------------|----------------|
| **Public leaderboards** | Appear on volunteer rankings | Name (first + last initial), stats |
| **Attendee metrics** | Individual contributions visible | Bags, weight, time |
| **Social media sharing** | Share events to social platforms | Name, event participation |

### Communication Features

| Feature | Description | Data Collected |
|---------|-------------|----------------|
| **Contact info to event leads** | Email shared with event organizers | Email address |
| **Instant messaging** | **BLOCKED for minors** | N/A |

### Legal Features

| Feature | Description | Data Collected |
|---------|-------------|----------------|
| **Waiver signing** | Parent signs liability waiver on behalf of minor | Digital signature, consent record |

---

## 4. Minor Protection Rules

Once parental consent is verified, minors have access with the following protections:

### Profile Visibility
- Name displayed as **first name + last initial only** (e.g., "John S.")
- Profile photo **not visible** to other users (only to event leads)
- No location sharing in profile

### Communication Restrictions
- **No direct messaging** (instant messaging blocked entirely)
- **No direct contact** with other users
- All communications routed through parent (notifications CC'd to parent email)

### Event Participation
- At least **one adult must be present** at any event with minor participants
- Parent receives notification when minor registers for an event
- Event leads can see full name for check-in purposes only

### Data Handling
- Annual consent re-verification (Privo handles timing)
- Right to deletion upon parent request
- Consent artifacts retained per COPPA legal requirements

### Parent Account Requirements

**Important:** Parents do NOT need a TrashMob account to approve their minor's registration. Privo email approval is sufficient.

| Scenario | Parent Account Required? | Notes |
|----------|-------------------------|-------|
| Minor registration approval | ❌ No | Privo email link sufficient |
| Consent revocation | ❌ No | Can revoke via Privo |
| View minor's activity | ✅ Yes | Requires parent dashboard |
| Approve event participation | ✅ Yes | Future feature (Phase 4) |
| Receive event notifications | ❌ No | Email sent regardless |

If parent later creates a TrashMob account, it can be linked to their minor's account via email match, unlocking additional features like activity monitoring.

---

## 5. Integration Data Flows

### Data Sent to Privo

| Trigger | Data | Purpose |
|---------|------|---------|
| **Registration** | Date of birth, user ID | Age verification |
| **Minor detected** | Minor user ID, parent email | VPC request |
| **Post-signup** | User location (city, region) | Compliance jurisdiction |
| **Consent check** | User ID, consent request ID | Validate active consent |

### Data Received from Privo

| Event | Data | Action |
|-------|------|--------|
| **Age verification result** | Age category (Under13, Minor, Adult) | Route registration flow |
| **VPC request created** | Consent request ID | Store for tracking |
| **Consent status webhook** | Status (Verified, Denied, Revoked) | Update user access |
| **Consent expiration** | Expiration warning | Trigger re-verification |

---

## 6. API Integration Points

> **Detailed API documentation received from PRIVO on March 24, 2026.** See [Project 23 — PRIVO API Requirements](./Project_23_Privo_API_Requirements.md) for the complete 10-section API specification including endpoints, credentials, feature/attribute identifiers, and webhook payload format.

### Key API Sections (Summary)

| Section | Endpoint | Purpose |
|---------|----------|---------|
| 1 | `POST /token` | Client access token (30 min expiry, extendable to 12 hrs) |
| 2 | `POST /s2s/api/v1.0/{svc}/requests` | Adult identity verification request |
| 3 | `POST /s2s/api/v1.0/{svc}/consents/{id}/verification/session` | Direct verification URL (skip PRIVO pre-screens) |
| 4 | `GET /s2s/api/v1.0/{svc}/accounts/sid/{sid}` | User info / feature states |
| 5 | `POST /s2s/api/v1.0/{svc}/requests` | Parent-initiated child consent |
| 6 | `POST /s2s/api/v1.0/{svc}/requests` | Child-initiated consent |
| 7 | `GET /s2s/api/v1.0/{svc}/consents/{id}` | Consent status check |
| 8 | `PATCH /s2s/api/v1.0/{svc}/accounts/sid/{sid}/attributes/{attr}/ial` | Email verification sync |
| 9 | `POST /s2s/api/v1.0/{svc}/accounts/sid/{sid}/{granter_sid}/features/revoke` | Revoke consent |
| 10 | Webhook | Real-time consent event notifications |

### Privo → TrashMob (Webhooks)

```json
// Webhook payload format (from PRIVO docs)
{
  "id": "3c5bb850-8b65-45f3-a31b-80c5k27d5514",
  "timestamp": "2024-09-11T13:23:34.325581013Z",
  "sid": "818g84dd-04d7-4793-9342-50c209316c95",
  "event_types": ["consent_request_created"],
  "granter_sid": ["ded3cad0-m557-4716-bce5-48098395bd74"],
  "consent_identifiers": ["1710c4c7-0694-42b7-8096-328f99844aad"]
}
```

---

## 7. Branding Assets

### Logo Files
*(To be provided by Marketing Team)*
- Primary logo (SVG, PNG)
- Icon/favicon (SVG, PNG)
- White/dark variants

### Brand Colors
| Color | Hex | Usage |
|-------|-----|-------|
| **Primary Green** | #00a651 | Primary actions, headers |
| **Dark Green** | #006633 | Secondary elements |
| **White** | #FFFFFF | Backgrounds |
| **Dark Gray** | #333333 | Text |

### Typography
- **Headings:** System fonts (Arial, Helvetica, sans-serif)
- **Body:** System fonts

### Consent Page Customization
TrashMob requests the following customizations on Privo's consent collection pages:
- TrashMob logo in header
- Brand colors on buttons and accents
- Clear explanation of what the minor can do on TrashMob
- Link back to https://www.trashmob.eco

---

## 8. Technical Environment

| Environment | URL | Purpose |
|-------------|-----|---------|
| **Production** | https://www.trashmob.eco | Live users |
| **Development** | https://dev.trashmob.eco | Testing & integration |

### Webhook Endpoints
- **Production:** `https://api.trashmob.eco/webhooks/privo`
- **Development:** `https://dev-api.trashmob.eco/webhooks/privo`

### Authentication Provider
- **Current:** Microsoft Entra External ID (CIAM) — production cutover completed February 22, 2026
- **Previous:** Azure AD B2C (decommissioned)
- **Impact on Privo:** None — Privo integration occurs after IdP authentication
- **CIAM Note:** CIAM id_tokens do not include an `email` claim. The backend resolves emails via Microsoft Graph API (`User.Read.All` application permission). The auth handler uses a 4-step user resolution: email lookup → ObjectId lookup → Graph API email resolution → auto-create.

### Current Age Gate: Custom Authentication Extension

TrashMob currently uses an Entra External ID **Custom Authentication Extension** (`OnAttributeCollectionSubmit`) to enforce an under-13 age gate during sign-up. This is the integration point that Privo would replace/extend.

**Architecture:**
- **Container App:** `ca-authext-tm-pr-westus2` (Azure Container Apps, production)
- **Source code:** `TrashMob.AuthExtension/` — minimal ASP.NET Core API
- **Endpoint:** `POST /api/authext/attributecollectionsubmit`
- **Behavior:** Parses `dateOfBirth` from the sign-up attributes, calculates age, and returns `continueWithDefaultBehavior` (13+) or `showBlockPage` (under 13)
- **Bicep template:** `Deploy/containerAppAuthExtension.bicep`
- **GitHub Actions:** `.github/workflows/release_ca-authext-tm-pr-westus2.yml`

**Entra Configuration (CIAM tenant `b5fc8717-29eb-496e-8e09-cf90d344ce9f`):**

The custom extension requires precise Entra configuration. The following was validated during a production incident investigation (March 2026):

| Component | Value | Notes |
|-----------|-------|-------|
| **Custom Extension ID** | `71e35239-0b1e-4a19-9f13-4a3c77417306` | Registered under Identity > Custom Extensions |
| **Extension Type** | `onAttributeCollectionSubmitCustomExtension` | Fires during sign-up attribute collection |
| **App Registration** | `TrashMob Eco Prod Auth` (`e11e65ba-e457-4a95-8a54-c96582ebb837`) | The app registration the extension authenticates as |
| **Application ID URI** | `api://ca-authext-tm-pr-westus2.greenground-fd8fc385.westus2.azurecontainerapps.io/e11e65ba-...` | Must match `authenticationConfiguration.resourceId` on the custom extension |
| **accessTokenAcceptedVersion** | `2` | Required for custom auth extensions |
| **App Role** | `CustomAuthenticationExtension.Receive.Payload` (value: `customauthenticationextension.api.endpoint`) | Must be defined on the app registration |
| **Role Assignment** | Azure AD Auth Extensions SP (`99045fe1-7639-4a75-9d4a-577b6ca3810f`) → app role on `e11e65ba` SP | Required for Entra to acquire a token to call the endpoint |
| **AllowedAppId** | `99045fe1-7639-4a75-9d4a-577b6ca3810f` | First-party Microsoft SP; validated via `azp` claim in JWT |

**Container App Environment Variables:**

| Variable | Value | Source |
|----------|-------|--------|
| `AuthExtension__TenantId` | CIAM tenant ID (`b5fc8717-...`) | GitHub secret `ENTRA_TENANT_ID` |
| `AuthExtension__ClientId` | App registration client ID (`e11e65ba-...`) | GitHub secret `AUTH_EXTENSION_CLIENT_ID` |
| `AuthExtension__AllowedAppId` | `99045fe1-7639-4a75-9d4a-577b6ca3810f` | Hardcoded in Bicep and `Program.cs` |

**OIDC Authority — CIAM vs Standard Endpoint:**

CIAM tenants have **two** OIDC discovery endpoints with **different signing keys**:
- `https://login.microsoftonline.com/{tenantId}/v2.0` — standard Azure AD endpoint; has a subset of signing keys
- `https://{tenantId}.ciamlogin.com/{tenantId}/v2.0` — CIAM-specific endpoint; has the **complete** set of signing keys

The JWT `Authority` in `Program.cs` **must** use the `ciamlogin.com` endpoint. Entra signs tokens for custom auth extensions with keys that may only exist in the CIAM OIDC discovery document (e.g., kid `z6-fLv223PW4n6R3gxvdvXigZXk` in the prod tenant). Using `login.microsoftonline.com` as the Authority causes `IDX10503` signature validation failures because the app fetches keys from the wrong JWKS endpoint.

The `ValidIssuers` array should accept tokens from **both** issuers since the actual `iss` claim in the token can use either format.

**Troubleshooting Notes (March 2026 Incident):**
- **AADSTS1003021** (`CustomExtensionPermissionNotGrantedToServicePrincipal`): The `CustomAuthenticationExtension.Receive.Payload` permission was not granted on the correct app registration. The custom extension's `authenticationConfiguration.resourceId` determines which app registration is used — verify this matches the app that has the permission granted.
- **AADSTS1100001** (non-retryable error from custom extension API): Multiple causes found:
  1. The app registration was missing the required app role definition (`customauthenticationextension.api.endpoint`), and the Azure AD Auth Extensions SP (`99045fe1`) had no role assignment. Both must be configured: (1) define the app role on the app registration, (2) assign that role to the `99045fe1` service principal.
  2. The JWT `Authority` was set to `login.microsoftonline.com` which does not expose all CIAM signing keys. Tokens signed with CIAM-only keys failed signature validation (`IDX10503`), returning 401 to Entra, which surfaced as AADSTS1100001 to the user. Fix: use `{tenantId}.ciamlogin.com` as the Authority.
- **Container App receives no requests:** If Entra cannot acquire a token (due to missing app role or role assignment), it fails internally and never makes the HTTP call to the endpoint. Container App logs will show zero requests even though the extension is configured.
- **Container App returns 401 but no logs visible:** The default `appsettings.json` sets `Microsoft.AspNetCore` log level to `Warning`, which suppresses request-level logs. Add env vars `Logging__LogLevel__Microsoft.AspNetCore=Information` and `Logging__LogLevel__Microsoft.AspNetCore.Authentication=Debug` to see JWT validation details. The `OnAuthenticationFailed` event in `Program.cs` also logs the specific error.

---

## 9. Timeline & Milestones

| Milestone | Target Date | Owner |
|-----------|-------------|-------|
| Privo contract signed | TBD | Business Team |
| Branding assets delivered | TBD | Marketing |
| Development environment ready | TBD | Engineering |
| Integration testing complete | TBD | Engineering + Privo |
| Production go-live | TBD | All |

---

## 10. Open Questions for Privo

1. ~~**Consent collection methods:** Which methods will be available?~~ — Handled by PRIVO verification widget (see [API Requirements](./Project_23_Privo_API_Requirements.md))
2. **Webhook retry policy:** How does Privo handle webhook delivery failures?
3. ~~**Consent expiration:** How far in advance does Privo notify about expiring consents?~~ — Not a concern; waivers expire yearly regardless.
4. ~~**Testing environment:** Does Privo provide a sandbox for integration testing?~~ — INT environment provided (see [API Requirements](./Project_23_Privo_API_Requirements.md))
5. ~~**Bulk consent verification:** Can we verify consent status for multiple users in one call?~~ — Not needed for v1; single-user checks sufficient.

---

## Appendix A: Figma Access

Please provide Figma access to the Privo team members for UI/UX review.

*(Contact addresses stored in secure location - not in public repo)*

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | January 31, 2026 | TrashMob Engineering | Initial draft |
| 1.1 | February 22, 2026 | TrashMob Engineering | Updated auth provider to Entra External ID (CIAM migration complete) |
| 1.2 | March 2, 2026 | TrashMob Engineering | Added custom authentication extension architecture, Entra configuration details, and troubleshooting notes from production incident |
| 1.3 | March 2, 2026 | TrashMob Engineering | Documented CIAM vs standard OIDC endpoint signing key differences, IDX10503 root cause, and logging troubleshooting tips |
| 1.4 | March 27, 2026 | TrashMob Engineering | Updated API section with PRIVO API Requirements doc reference (received March 24, 2026); resolved open questions on consent methods and testing environment |
| 2.0 | April 5, 2026 | TrashMob Engineering | Integration complete on INT: adult verification (Flow 1), parent-child consent (Flow 2), webhook processing, feature permissions, waiver routing, E2E tests. First Entra External ID + PRIVO integration verified. |

---

## 11. Integration Completion Summary

**Completed April 2026.** TrashMob is the first organization to integrate Microsoft Entra External ID with PRIVO.

### What Was Built

| Component | Description |
|-----------|-------------|
| `PrivoService` | HTTP client for all 10 PRIVO API sections with OAuth token caching |
| `PrivoConsentManager` | Orchestrates verification and consent workflows |
| `PrivoConsentV2Controller` | REST API endpoints for verification, consent, status, permissions |
| `PrivoWebhooksV2Controller` | Webhook handler with API key auth, polls Section 7 for state |
| `usePrivoPermissions` | React hook for client-side feature gating |
| `IPrivoPermissionService` | Mobile permission cache with 1-hour TTL |
| 16 E2E Tests | Playwright tests covering API, UI, webhooks, callback, age gate |

### Verified Flows

| Flow | Status | Notes |
|------|--------|-------|
| Adult Identity Verification | Verified on INT | Widget redirect, webhook auto-update |
| Parent Adds 13-17 Child | Verified on INT | Consent email, webhook, account linking |
| Child-Initiated Signup | Built, pending E2E test | Backend + UI complete |
| Feature Permission Gating | Verified | 8 features gated on web + mobile |
| Minor Waiver Routing | Built | Waivers route to parent, auto-complete on sign |

### Key Vault Secrets (INT)

| Secret | Purpose |
|--------|---------|
| `Privo-ClientId` | PRIVO INT OAuth client ID |
| `Privo-ClientSecret` | PRIVO INT OAuth client secret |
| `Privo-ApiKey` | Webhook authentication key |
| `Privo--Enabled` | Feature flag (true for dev) |

### Production Deployment Checklist

- [ ] PRIVO provides production credentials
- [ ] Set `Privo-ClientId`, `Privo-ClientSecret`, `Privo-ApiKey` in prod Key Vault
- [ ] Set `Privo--Enabled` = true in prod Key Vault
- [ ] Whitelist `https://www.trashmob.eco` as redirect URL with PRIVO
- [ ] Configure production webhook endpoint with PRIVO
- [ ] Verify token acquisition against production endpoint
- [ ] Test adult verification end-to-end on production

---

**Related Documents:**
- [Project 23 — Parental Consent for Minors](./Project_23_Parental_Consent.md)
- [Project 1 — Auth Revamp](./Project_01_Auth_Revamp.md)
- [Project 8 — Waivers V3](./Project_08_Waivers_V3.md)
