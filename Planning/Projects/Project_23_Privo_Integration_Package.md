# TrashMob.eco — PRIVO Integration Package

**Document Version:** 3.0
**Date:** April 5, 2026
**Prepared for:** PRIVO Integration Team
**Status:** Integration complete on INT — awaiting production credentials

---

## 1. Business Information

| Field | Value |
|-------|-------|
| **Company Name** | TrashMob.eco |
| **Website** | https://www.trashmob.eco |
| **Business Type** | Non-profit volunteer coordination platform (501(c)(3)) |
| **Primary Service** | Connecting volunteers with community cleanup events |
| **Target Audience** | Adults (18+) and minors (13-17) with parental consent |
| **Privacy Policy** | https://www.trashmob.eco/privacypolicy |
| **Terms of Service** | https://www.trashmob.eco/termsofservice |

---

## 2. Implemented Flows

### Flow 1: Adult Identity Verification

The parent verifies their identity before they can add 13-17 dependents.

```
   Parent                  TrashMob                    PRIVO
     │                        │                          │
     │  1. Click "Verify      │                          │
     │     My Identity"       │                          │
     │───────────────────────>│                          │
     │                        │                          │
     │                        │  2. POST /token           │
     │                        │─────────────────────────>│
     │                        │  3. Access token          │
     │                        │<─────────────────────────│
     │                        │                          │
     │                        │  4. POST /requests        │
     │                        │  (Section 2: adult data,  │
     │                        │   EID=User.Id)            │
     │                        │─────────────────────────>│
     │                        │  5. SiD, ConsentId        │
     │                        │<─────────────────────────│
     │                        │                          │
     │                        │  6. POST /verification/   │
     │                        │  session (Section 3)      │
     │                        │─────────────────────────>│
     │                        │  7. Verification URL      │
     │                        │<─────────────────────────│
     │                        │                          │
     │  8. Redirect to        │                          │
     │     PRIVO widget ──────────────────────────────> │
     │                        │                          │
     │  9. Complete ID        │                          │
     │     verification ─────────────────────────────> │
     │                        │                          │
     │  10. Redirect back     │                          │
     │      to TrashMob <────────────────────────────── │
     │                        │                          │
     │                        │  11. Webhook:             │
     │                        │  consent_updated          │
     │                        │<─────────────────────────│
     │                        │                          │
     │                        │  12. GET /consents/{id}   │
     │                        │  (Section 7: verify       │
     │                        │   state = "approved")     │
     │                        │─────────────────────────>│
     │                        │  13. State confirmed      │
     │                        │<─────────────────────────│
     │                        │                          │
     │  14. "Identity         │                          │
     │      Verified" shown   │                          │
     │<───────────────────────│                          │
```

**Key implementation details:**
- TrashMob uses `User.Id` (GUID) as the PRIVO External ID (EID)
- Token cached 25 minutes (5-min buffer under 30-min PRIVO expiry) with semaphore-protected refresh
- `consent_request_email` suppressed for adult self-verification (parent redirects directly to widget)
- `consent_approved_email` enabled so parent gets confirmation after verification
- Webhook handler polls Section 7 for authoritative state (does not rely on event type names)
- "Check Status" button available as manual polling fallback

### Flow 2: Verified Parent Adds 13-17 Child

After identity verification, the parent adds a child as a dependent and approves PRIVO consent.

```
   Parent                  TrashMob                    PRIVO
     │                        │                          │
     │  1. Add dependent      │                          │
     │  (name, DOB, email)    │                          │
     │───────────────────────>│                          │
     │                        │  2. Create Dependent      │
     │                        │     record locally        │
     │                        │                          │
     │  3. Click shield icon  │                          │
     │  (start consent)       │                          │
     │───────────────────────>│                          │
     │                        │                          │
     │                        │  4. POST /requests        │
     │                        │  (Section 5: child data,  │
     │                        │   granter=parent SiD,     │
     │                        │   EID=Dependent.Id)       │
     │                        │─────────────────────────>│
     │                        │  5. Child SiD, ConsentId  │
     │                        │<─────────────────────────│
     │                        │                          │
     │                        │                     6. PRIVO sends
     │                        │                        consent email
     │  7. "Check email"      │                        to parent
     │     toast shown        │                          │
     │<───────────────────────│                          │
     │                        │                          │
     │  8. Parent clicks      │                          │
     │     email link ────────────────────────────────> │
     │                        │                          │
     │  9. Review features,   │                          │
     │     click "I Agree" ──────────────────────────> │
     │                        │                          │
     │                        │  10. Webhook:             │
     │                        │  consent_updated +        │
     │                        │  account_feature_updated  │
     │                        │<─────────────────────────│
     │                        │                          │
     │                        │  11. Poll Section 7       │
     │                        │  → state = "approved"     │
     │                        │                          │
     │  12. "Consent Approved"│                          │
     │      badge shown       │                          │
     │<───────────────────────│                          │
     │                        │                          │
     │  13. Send invitation   │                          │
     │      email to child    │                          │
     │───────────────────────>│                          │
     │                        │  14. Email sent to child  │
     │                        │      with account link    │
     │                        │                          │
     │                    CHILD                          │
     │                        │                          │
     │  15. Child creates     │                          │
     │      Entra account     │                          │
     │      using invite link │                          │
     │                        │  16. Auto-link to parent  │
     │                        │  (TryAcceptByEmailAsync)  │
     │                        │  IsMinor=true,            │
     │                        │  ParentUserId set         │
```

**Key implementation details:**
- Child email is optional on the Dependent record — omitted from PRIVO request if empty
- PRIVO sends the consent email (not TrashMob) — `consent_request_email: true` for child flow
- Consent status tracked per-dependent: "Needs Consent" → "Consent Pending" → "Consent Approved"
- Invitation email uses secure token (SHA-256 hashed, 30-day expiry)
- Child account auto-linked to parent when email matches pending invitation

### Post-Registration: Minor Event Attendance

When a minor with their own account registers for an event:

```
   Minor                   TrashMob                    Parent
     │                        │                          │
     │  1. Click "Attend"     │                          │
     │───────────────────────>│                          │
     │                        │                          │
     │                        │  2. Check PRIVO           │
     │                        │  permissions              │
     │                        │  (Account feature)        │
     │                        │                          │
     │                        │  3. Check DependentWaivers│
     │                        │  (Global + Community      │
     │                        │   waivers for event)      │
     │                        │                          │
     │                     IF WAIVERS MISSING:           │
     │                        │                          │
     │                        │  4. Create registration   │
     │                        │  with WaiverPendingDate   │
     │                        │                          │
     │  5. "Waiting for       │  6. Send email:           │
     │     parent to sign     │  "Sign waivers for        │
     │     waivers"           │   [child] attending       │
     │<───────────────────────│   [event]"                │
     │                        │─────────────────────────>│
     │                        │                          │
     │                        │  7. Parent signs          │
     │                        │  DependentWaivers         │
     │                        │<─────────────────────────│
     │                        │                          │
     │                        │  8. Auto-complete:        │
     │                        │  Clear WaiverPendingDate  │
     │                        │                          │
     │                     IF ALL WAIVERS SIGNED:        │
     │                        │                          │
     │                        │  9. Register normally     │
     │                        │                          │
     │                        │  10. Notify parent:       │
     │                        │  "[child] registered      │
     │                        │   for [event]"            │
     │                        │─────────────────────────>│
```

---

## 3. PRIVO Feature Permissions

PRIVO controls which features a minor can access. Parents approve features during the consent flow.

### Feature Identifiers

| Identifier | Feature | Gated On |
|------------|---------|----------|
| `trashmobservice_account` | Event registration | Web: RegisterBtn, Mobile: ViewEventViewModel |
| `trashmobservice_leaderboard` | Public leaderboards | Web: LeaderboardsPage, Mobile: LeaderboardsViewModel |
| `trashmobservice_social` | Social media sharing | Web: ShareDialog, Team detail |
| `trashmobservice_newsletter` | Newsletter subscriptions | Web: MyDashboard, Mobile: NewsletterPreferencesViewModel |
| `trashmobservice_notifications` | In-app notifications | Placeholder (not fully implemented) |
| `trashmobservice_geolocation` | Route tracking / GPS | Web: MyRoutesCard, Mobile: ViewEventViewModel |
| `trashmobservice_team` | Join / create teams | Web: TeamsPage, Team detail, Mobile: ViewTeamViewModel |
| `trashmobservice_photo_uploads` | Photo uploads | Web: EventPhotoUploader, Mobile: ViewEventViewModel, CreateLitterReportViewModel |

### How Permissions Are Enforced

1. **Backend**: `GET /v2/privo/permissions` calls PRIVO Section 4 (`/accounts/eid/{eid}`), returns feature states, cached 1 hour server-side
2. **Web**: `usePrivoPermissions` React hook with 1-hour `staleTime` — `isFeatureEnabled(featureId)` returns `true` for adults, checks cache for minors
3. **Mobile**: `IPrivoPermissionService` singleton with 1-hour client-side cache — `IsFeatureEnabled(featureId)`, pre-fetched on login, cleared on logout
4. **Fail-closed**: Missing permission keys treated as disabled

---

## 4. Minor Protection Rules

### Profile Visibility
- Name displayed as **first name + last initial only** (e.g., "John S.")
- Full name visible only to event leads for check-in purposes

### Communication Restrictions
- **No direct messaging** (no DM system exists; blocked by design)
- Parent receives email when minor registers for events

### Event Participation
- At least **one adult must be present** at any event with minor participants
- Under-13 dependents auto-unregistered if parent cancels (13-17 minors with own accounts are not)
- Parent notified via email on every minor event registration

### Waiver Handling
- **Minors cannot sign their own waivers** — blocked at API level (`POST /v2/waivers/accept` returns 400 for minors)
- All waivers (TrashMob global + community-specific) route to parent for signature
- Registration held in "waiver pending" status until parent signs all required waivers
- Auto-completes when parent signs last required `DependentWaiver`
- Parent sees red alert on dashboard when waivers need signing

### Minor Dashboard
- Minors see "Minor Account" card instead of dependents management
- No "Add Dependent" button, no identity verification prompt
- Feature access controlled by PRIVO permission gating

---

## 5. Technical Architecture

### Backend Components

| Component | Location | Purpose |
|-----------|----------|---------|
| `PrivoService` | `TrashMob/Services/PrivoService.cs` | HTTP client for all 10 PRIVO API sections, OAuth token caching (25-min, semaphore) |
| `PrivoConsentManager` | `TrashMob.Shared/Managers/PrivoConsentManager.cs` | Orchestrates verification/consent workflows, webhook processing, status polling |
| `PrivoConsentV2Controller` | `TrashMob/Controllers/V2/PrivoConsentV2Controller.cs` | API: verify, consent, status, refresh, permissions, revoke |
| `PrivoWebhooksV2Controller` | `TrashMob/Controllers/V2/PrivoWebhooksV2Controller.cs` | Webhook handler with `X-Api-Key` auth via `PrivoApiKeyAuthenticationFilter` |
| `DependentWaiverManager` | `TrashMob.Shared/Managers/DependentWaiverManager.cs` | Dependent waiver checks (Global + Community), auto-complete pending registrations |
| `ParentalConsent` entity | `TrashMob.Models/ParentalConsent.cs` | Stores PRIVO consent records (SiD, ConsentId, status, type) |

### Frontend Components

| Component | Platform | Purpose |
|-----------|----------|---------|
| `VerifyIdentityCard` | Web | Dashboard card: verify button, pending status, check status |
| `MyDependentsCard` | Web | Consent badges, shield button, invite flow |
| `PendingWaiverAlertsCard` | Web | Red alert banner for parent waiver signing |
| `RegisterBtn` | Web | Minor path: skip waiver dialog, show pending message |
| `usePrivoPermissions` | Web | React hook for feature gating |
| `PrivoPermissionService` | Mobile | Singleton permission cache |
| `PrivoConsentRestService` | Mobile | REST client for PRIVO API endpoints |

### API Endpoints

| Method | Route | Auth | Purpose |
|--------|-------|------|---------|
| `POST` | `/v2/privo/verify` | User | Initiate adult identity verification (Flow 1) |
| `POST` | `/v2/privo/consent/child/{dependentId}` | User | Initiate child consent (Flow 2) |
| `POST` | `/v2/privo/consent/child-initiated` | Anonymous | Child-initiated consent (Flow 3) |
| `GET` | `/v2/privo/status` | User | Current verification/consent status |
| `POST` | `/v2/privo/status/refresh` | User | Poll PRIVO Section 7 and update local record |
| `GET` | `/v2/privo/permissions` | User | PRIVO feature permissions (cached 1 hr) |
| `GET` | `/v2/privo/enabled` | Public | Feature flag check |
| `POST` | `/v2/privo/consent/{id}/revoke` | User | Revoke consent |
| `POST` | `/v2/webhooks/privo/consent` | API Key | PRIVO webhook receiver |

### Webhook Processing

TrashMob receives webhooks at `POST /v2/webhooks/privo/consent` with `X-Api-Key` header authentication.

**Processing logic:**
1. Validate API key against Key Vault secret `Privo-ApiKey`
2. Match `consent_identifiers` to local `ParentalConsent` record
3. Skip pure creation events (`consent_request_created`, `consent_notice_delivered`)
4. For substantive events (`consent_updated`, `account_feature_updated`): poll Section 7 for authoritative state
5. Update local record based on polled state (`approved` → Verified, `denied` → Denied)
6. For adult verification: set `User.IsIdentityVerified = true`

---

## 6. PRIVO API Usage

### Sections Used

| Section | Used For | TrashMob Implementation |
|---------|----------|------------------------|
| 1 (Token) | OAuth client credentials | Cached 25 min, auto-refresh on 401 |
| 2 (Adult Consent Request) | Adult identity verification | EID = User.Id, granter = principal (same person) |
| 3 (Verification URL) | Direct widget redirect | Skip pre-verification UI |
| 4 (UserInfo) | Feature permissions | Query by EID, cached 1 hour |
| 5 (Parent Child Consent) | Parent adds 13-17 dependent | Granter by SiD, principal EID = Dependent.Id |
| 7 (Consent Status) | Poll for state changes | Used by webhook handler and Check Status button |
| 9 (Revoke) | Consent revocation | Available via API, not yet exposed in UI |
| 10 (Webhooks) | Real-time event notifications | `X-Api-Key` authentication |

### Sections Not Currently Used

| Section | Reason |
|---------|--------|
| 6 (Child-Initiated) | Backend built, pending E2E testing |
| 8 (Email IAL) | `email_verified: true` sent in consent request; IAL value not yet provided by PRIVO |

### Request Payload Format (Adult Verification)

```json
{
  "granter": {
    "email": "parent@example.com",
    "notifications": [
      { "is_on": false, "notification_type": "consent_request_email" },
      { "is_on": true, "notification_type": "consent_approved_email" }
    ]
  },
  "locale": "en-US",
  "principal": {
    "given_name": "Joe",
    "birthdate": "19900101",
    "birthdate_precision": "yyyymmdd",
    "email": "parent@example.com",
    "email_verified": true,
    "eid": "ee1b2ad7-fd41-40b5-a056-d157e815c2d8",
    "attributes": [
      {
        "attribute_identifier": "trashmobservice_att_granter_family_name",
        "value": ["Beernink"]
      }
    ]
  }
}
```

### Request Payload Format (Parent-Child Consent)

```json
{
  "granter": {
    "email": "parent@example.com",
    "sid": "c315f9b7-5a9c-485b-b670-e83d84be8d9d",
    "notifications": [
      { "is_on": true, "notification_type": "consent_request_email" }
    ]
  },
  "locale": "en-US",
  "principal": {
    "given_name": "ChildFirstName",
    "birthdate": "20130101",
    "birthdate_precision": "yyyymmdd",
    "eid": "acbf334e-ee32-4134-9c9b-9489aa83659a",
    "email": "child@example.com"
  }
}
```

---

## 7. Technical Environment

| Environment | Web URL | API Base | Webhook Endpoint |
|-------------|---------|----------|-----------------|
| **Development** | https://dev.trashmob.eco | https://dev.trashmob.eco/api | https://dev.trashmob.eco/api/v2/webhooks/privo/consent |
| **Production** | https://www.trashmob.eco | https://www.trashmob.eco/api | https://www.trashmob.eco/api/v2/webhooks/privo/consent |

### Authentication Provider
- **Microsoft Entra External ID (CIAM)** — production since February 22, 2026
- PRIVO integration occurs **after** IdP authentication (not during sign-up)
- Age gate is in-app (React `AgeGateDialog` / MAUI `AgeGateViewModel`), not via Custom Authentication Extension

### Key Vault Secrets

| Secret | Purpose | Environment |
|--------|---------|-------------|
| `Privo-ClientId` | OAuth client ID | Per environment |
| `Privo-ClientSecret` | OAuth client secret | Per environment |
| `Privo-ApiKey` | Webhook authentication key | Per environment |
| `Privo--Enabled` | Feature flag | `true` for dev, `false` for prod (until go-live) |

---

## 8. Branding

### Brand Colors
| Color | Hex | Usage |
|-------|-----|-------|
| **Primary Green** | #005B4B | Headers, primary actions |
| **Lime** | #ABC313 | Accents, highlights |
| **White** | #FFFFFF | Backgrounds |
| **Dark Gray** | #333333 | Body text |

### PRIVO on TrashMob
- Home page "Trusted By" section displays PRIVO 25th anniversary logo with link to privo.com
- FAQ page includes "What is PRIVO?" question with link
- Help page explains PRIVO's role in parental consent flow
- News article: "TrashMob Partners with PRIVO to Protect Young Volunteers"

---

## 9. Testing

### Automated Tests (16 Playwright E2E scenarios)

| Category | Tests | What's Covered |
|----------|-------|----------------|
| API Endpoints | 4 | enabled, status, permissions, refresh |
| Adult Verification UI | 2 | Card visibility, button initiation |
| Dependent Consent UI | 2 | Status display, shield button |
| Minor Account Restrictions | 1 | Correct card by user type |
| Callback Page | 2 | Renders with/without status param |
| Webhook Security | 2 | Rejects missing/wrong API key |
| Child Signup Page | 1 | Form renders |
| Age Gate Dialog | 2 | 13-17 consent message, under-13 block |

### Manual Test Scenarios

See [Project 23 — PRIVO Test Scenarios](./Project_23_Privo_Test_Scenarios.md) for 25 detailed test scenarios across 8 flow categories.

---

## 10. Production Deployment Checklist

- [ ] PRIVO provides production credentials (Client ID, Client Secret)
- [ ] Set `Privo-ClientId` and `Privo-ClientSecret` in prod Key Vault (`kv-tm-pr-westus2`)
- [ ] Generate and set `Privo-ApiKey` in prod Key Vault, share with PRIVO
- [ ] Set `Privo--Enabled` = `true` in prod Key Vault
- [ ] PRIVO whitelists `https://www.trashmob.eco` as redirect URL
- [ ] PRIVO configures production webhook endpoint: `https://www.trashmob.eco/api/v2/webhooks/privo/consent`
- [ ] Verify token acquisition against production endpoint
- [ ] Test adult verification end-to-end on production
- [ ] Test parent-child consent end-to-end on production

---

## 11. Open Questions

1. **Webhook retry policy:** How does PRIVO handle webhook delivery failures? (Fallback: Check Status polling works)
2. **Section 8 IAL value:** What IAL value should TrashMob use when syncing email verification status?

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | January 31, 2026 | TrashMob Engineering | Initial draft |
| 1.1 | February 22, 2026 | TrashMob Engineering | Updated auth provider to Entra External ID |
| 1.2 | March 2, 2026 | TrashMob Engineering | Added custom authentication extension architecture |
| 1.3 | March 2, 2026 | TrashMob Engineering | Documented CIAM OIDC endpoint signing key differences |
| 1.4 | March 27, 2026 | TrashMob Engineering | Added PRIVO API Requirements reference |
| 2.0 | April 5, 2026 | TrashMob Engineering | Integration complete on INT |
| 3.0 | April 5, 2026 | TrashMob Engineering | Complete rewrite to reflect actual implementation: accurate flow diagrams, API usage, feature permissions, waiver routing, production checklist |

---

**Related Documents:**
- [Project 23 — Parental Consent for Minors](./Project_23_Parental_Consent.md)
- [Project 23 — PRIVO API Requirements](./Project_23_Privo_API_Requirements.md)
- [Project 23 — PRIVO Test Scenarios](./Project_23_Privo_Test_Scenarios.md)
- [Project 23 — PRIVO Proposed Flows](./Project_23_Privo_Proposed_Flows.md)
- [Project 8 — Waivers V3](./Project_08_Waivers_V3.md)
