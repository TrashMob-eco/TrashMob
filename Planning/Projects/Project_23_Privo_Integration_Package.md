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

### TrashMob → Privo

```
POST /api/privo/verify-age
Request: { dateOfBirth: "2012-05-15", userId: "guid" }
Response: { category: "Minor", privoUserId: "privo-123" }

POST /api/privo/request-consent
Request: {
  minorUserId: "guid",
  privoUserId: "privo-123",
  parentEmail: "parent@email.com",
  features: ["newsletter", "events", "photos", "leaderboards", ...],
  redirectUrl: "https://trashmob.eco/consent-complete"
}
Response: { consentRequestId: "vpc-456", status: "Pending" }

GET /api/privo/consent-status/{consentRequestId}
Response: { status: "Verified", verifiedDate: "2026-01-31T...", method: "CreditCard" }
```

### Privo → TrashMob (Webhooks)

```
POST https://api.trashmob.eco/webhooks/privo
{
  "event": "consent_status_changed",
  "consentRequestId": "vpc-456",
  "status": "Verified",
  "timestamp": "2026-01-31T15:30:00Z"
}

POST https://api.trashmob.eco/webhooks/privo
{
  "event": "consent_revoked",
  "userId": "privo-123",
  "reason": "Parent requested",
  "timestamp": "2026-02-15T10:00:00Z"
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

**Troubleshooting Notes (March 2026 Incident):**
- **AADSTS1003021** (`CustomExtensionPermissionNotGrantedToServicePrincipal`): The `CustomAuthenticationExtension.Receive.Payload` permission was not granted on the correct app registration. The custom extension's `authenticationConfiguration.resourceId` determines which app registration is used — verify this matches the app that has the permission granted.
- **AADSTS1100001** (non-retryable error from custom extension API): The app registration was missing the required app role definition (`customauthenticationextension.api.endpoint`), and the Azure AD Auth Extensions SP (`99045fe1`) had no role assignment. Both must be configured: (1) define the app role on the app registration, (2) assign that role to the `99045fe1` service principal.
- **Container App receives no requests:** If Entra cannot acquire a token (due to missing app role or role assignment), it fails internally and never makes the HTTP call to the endpoint. Container App logs will show zero requests even though the extension is configured.

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

1. **Consent collection methods:** Which methods will be available? (Credit card, ID, video call?)
2. **Webhook retry policy:** How does Privo handle webhook delivery failures?
3. **Consent expiration:** How far in advance does Privo notify about expiring consents?
4. **Testing environment:** Does Privo provide a sandbox for integration testing?
5. **Bulk consent verification:** Can we verify consent status for multiple users in one call?

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

---

**Related Documents:**
- [Project 23 — Parental Consent for Minors](./Project_23_Parental_Consent.md)
- [Project 1 — Auth Revamp](./Project_01_Auth_Revamp.md)
- [Project 8 — Waivers V3](./Project_08_Waivers_V3.md)
