# Project 23 — PRIVO API Integration Requirements

**Document Version:** 1.0
**Date Received:** March 24, 2026
**From:** Thorn Tayloe, V.P. Product & Engineering, PRIVO
**Contact:** ttayloe@privo.com | 571.297.1796

---

## High-Level Overview

TrashMob will utilize PRIVO consent management and identity verification via PRIVO API calls. Three primary flows are supported:

1. **Adult (Parent) Identity Verification** — Adults verify their identity through PRIVO after creating a TrashMob account
2. **Verified Adult Adds Child(ren)** — Verified parents add child accounts and complete consent in one flow
3. **Child-Initiated Account** — Child (13-17) provides parent email, PRIVO handles consent collection from parent

---

## Environment & Credentials

### PRIVO API Endpoints

| Environment | URL |
|-------------|-----|
| Integration (INT) | `https://consent-svc-int.privo.com` |
| Production (PROD) | `https://consent-svc.privo.com` |

**Swagger Documentation:** https://consent-svc-int.privo.com/docs/s2s/ (INT environment)

### OAuth Scopes

| Environment | Scope |
|-------------|-------|
| INT & PROD | `openid` |

### Partner Credentials

| Environment | Client ID | Secret |
|-------------|-----------|--------|
| Integration (INT) | *Stored in dev Key Vault as `Privo-ClientId`* | *Stored in dev Key Vault as `Privo-ClientSecret`* |
| Production (PROD) | *Stored in prod Key Vault as `Privo-ClientId`* | *Stored in prod Key Vault as `Privo-ClientSecret`* |

> **Note:** INT credentials received from PRIVO on March 24, 2026. Added to dev Key Vault — do not commit to source control.

### Service Identifier

All API requests require a `service_identifier`:

| Environment | Value |
|-------------|-------|
| INT & PROD | `trashmobservice` |

---

## Feature Names & Identifiers

These are the PRIVO feature identifiers for consent-gated features:

| Feature | Identifier |
|---------|------------|
| Adult Identity Verification | `trashmobservice_adult_identity_verification` |
| Register and Participate in Events | `trashmobservice_account` |
| Public Leaderboards | `trashmobservice_leaderboard` |
| Social Media Sharing | `trashmobservice_social` |
| Newsletter Subscriptions | `trashmobservice_newsletter` |
| In-App Notifications | `trashmobservice_notifications` |
| Geolocation & Tracking | `trashmobservice_geolocation` |
| Join a Team | `trashmobservice_team` |
| Photo Uploads | `trashmobservice_photo_uploads` |

---

## Attribute Names & Identifiers

| Attribute | Identifier |
|-----------|------------|
| Principal (Child) Given Name | `trashmobservice_att_principal_given_name` |
| Principal (Child) Family Name | `trashmobservice_att_principal_family_name` |
| Principal (Child) Email | `trashmobservice_att_principal_email` |
| Principal (Child) Birth Date | `trashmobservice_att_principal_birthdate` |
| Adult (Granter) Birth Date | `trashmobservice_att_granter_birthdate` |
| Adult (Granter) Email | `trashmobservice_att_granter_email` |
| Adult (Granter) Given Name | `trashmobservice_att_granter_given_name` |
| Adult (Granter) Family Name | `trashmobservice_att_granter_family_name` |

---

## API Sections

### Section 1 — Client Access Token (POST)

Obtain a client API access token. Tokens expire after 30 minutes (can be extended up to 12 hours by request).

```
POST {{env_endpoint}}/token
```

**Suggestion from PRIVO:** Maintain a pool of tokens to avoid creating new ones on demand. Best achieved if token expiration is extended to 12 hours.

---

### Section 2 — Adult Consent (Verification) Request (POST)

Creates a consent request for an 18+ adult who wants to verify their identity. Returns SiD (user identifier), Consent Identifier, and Consent URL.

```
POST {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/requests
```

**Request data includes:** First Name, Last Name, Birthdate, Email, optional External ID (EID).

**IMPORTANT:** Capture from response:
- **Consent Identifier** — represents the consent request
- **Principal (adult) SiD** — user identifier in PRIVO
- **Consent URL** — redirect URL for verification flow

**Note on EID:** If provided, must be unique per user. Email and EID can be reused for a Granter (parent) when registering additional children.

---

### Section 3 — Get Verification URL (POST)

**Optional.** Use when skipping PRIVO's pre-verification UI/UX and sending the user directly to the identity verification widget.

```
POST {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/consents/{{consent_identifier}}/verification/session?redirect_url={{some_whitelisted_url}}/someUserParameter
```

**Flow:**
1. Create consent request (Section 2) in the background
2. Capture SiDs and Consent Identifier
3. Call this endpoint to get a direct verification URL
4. Redirect user to the URL
5. After verification, user is redirected back to `redirect_url`

**Important:** The `redirect_url` hostname must be whitelisted in the PRIVO system. Only the base URL needs whitelisting — path and query parameters can be set freely.

**URLs to whitelist:**
- `https://www.trashmob.eco`
- `https://dev.trashmob.eco`

---

### Section 4 — User Information / UserInfo (GET)

Retrieve feature states and approved data for a given user. Used after consent is in place or verification is complete.

```
# By PRIVO SiD
GET {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/accounts/sid/{{sid}}

# By External ID (EID)
GET {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/accounts/eid/{{eid}}
```

Returns all feature states and approved data related to permissioned features. For adult verification, returns the verified adult data (e.g., verified name vs. claimed name).

---

### Section 5 — Parent-Initiated Child Consent Record (POST)

When a verified parent wants to add a child to their account.

```
POST {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/requests
```

**Request must include:** Reference to the verified adult (email, SiD, or EID).

**IMPORTANT:** Capture from response:
- **Consent Identifier**
- **Principal (child) SiD**

Partners must also provide attribute and feature identifiers on subsequent requests to complete the consent receipt cycle.

**Note on EID:** Must be unique per user. Email and EID can be reused for a Granter when registering additional children.

---

### Section 6 — Child-Initiated Consent Record (POST)

When a 13-17-year-old user initiates the consent request themselves.

```
POST {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/requests
```

**Request data includes:** Parent email (or SiD, or EID), child first name, birth date, email address.

**IMPORTANT:** Capture from response:
- **Consent Identifier**
- **Principal (child) SiD**
- **Granter (parent) SiD**

---

### Section 7 — Consent Status (GET)

Check the consent status of a given user.

```
GET {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/consents/{{consent_identifier}}
```

Returns the `state` of the consent request.

**Note:** For general use only. Do NOT rely on this for feature state or data retrieval — use Section 4 (UserInfo) for that.

---

### Section 8 — Email Attribute Verification (PATCH)

When TrashMob has independently verified a user's email (e.g., via Entra), update PRIVO with that verification status.

```
PATCH {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/accounts/sid/{{sid}}/attributes/{{attribute_identifier}}/ial
```

Updates the Identity Assurance Level (IAL) for a given attribute. IAL value will be provided by PRIVO during integration.

**Relevant for TrashMob:** Since Entra External ID verifies email during account creation, we should call this to sync email verification status to PRIVO.

---

### Section 9 — Revoke Consent (POST)

Manually revoke consent for a user (e.g., parent request via customer service or parent dashboard).

```
POST {{env_endpoint}}/s2s/api/v1.0/{{service_identifier}}/accounts/sid/{{sid}}/{{granter_sid}}/features/revoke
```

Requires both Granter (parent) and Principal (child) SiDs.

**IMPORTANT:** All data associated with revoked features will no longer be shared with TrashMob. UserInfo requests will return empty data for revoked features.

---

### Section 10 — Event Webhooks

PRIVO sends real-time consent and account notifications via secure webhook endpoints.

#### Security Requirements

- Separate secure endpoints for INT and PROD (multiple endpoints per environment allowed)
- Separate API keys for each endpoint
- Header name can be shared across environments

#### HTTP Request Format

```
POST https://api.trashmob.eco/webhooks/privo HTTP/1.1
Host: api.trashmob.eco
x-api-key: <api-key-value>
content-type: application/json
```

#### Webhook Payload

```json
{
  "id": "3c5bb850-8b65-45f3-a31b-80c5k27d5514",
  "timestamp": "2024-09-11T13:23:34.325581013Z",
  "sid": "818g84dd-04d7-4793-9342-50c209316c95",
  "event_types": [
    "consent_request_created"
  ],
  "granter_sid": [
    "ded3cad0-m557-4716-bce5-48098395bd74"
  ],
  "consent_identifiers": [
    "1710c4c7-0694-42b7-8096-328f99844aad"
  ]
}
```

#### TrashMob Webhook Endpoints

| Environment | Endpoint |
|-------------|----------|
| Integration | `https://dev-api.trashmob.eco/webhooks/privo` |
| Production | `https://api.trashmob.eco/webhooks/privo` |

---

## Flow Details

### Flow 1: Adult (Parent) Identity Verification

1. Adult creates TrashMob account via Entra External ID (provides birthdate 18+)
2. Adult chooses to verify their identity
3. TrashMob calls Section 1 (get token), then Section 2 (create consent/verification request)
   - Data sent: First Name, Last Name, Birthdate, Email, EID (TrashMob user GUID)
4. TrashMob captures SiD, Consent Identifier, Consent URL
5. **Option A:** Redirect to PRIVO's consent URL (includes pre-verification UI/UX)
6. **Option B:** Call Section 3 to get direct verification URL (skip PRIVO pre-screens)
7. Adult completes identity verification via approved methods
8. PRIVO sends webhook (Section 10) with consent status
9. TrashMob calls Section 4 (UserInfo) to retrieve verified data
10. Adult is now verified and can add children

### Flow 2: Verified Adult Adds Child(ren)

1. Verified parent elects to add child account(s) — can batch multiple children
2. Parent provides child info: name, birthdate, email, etc.
3. TrashMob calls Section 5 (parent-initiated consent request)
   - Must include reference to verified adult (email, SiD, or EID)
4. TrashMob captures child SiD, Consent Identifier, Consent URL
5. Parent is presented with consent page: features, Privacy Policy, ToS, Conduct Agreement
6. Parent clicks "I Agree"
7. Parent views account summary, can manage consent or process additional children
8. PRIVO sends webhook on consent completion
9. TrashMob generates Entra account link, sends email to child
10. Child creates account and is linked to parent

### Flow 3: Child-Initiated Account

1. Child visits TrashMob, enters birthdate (13-17)
2. TrashMob requires parent account — child provides parent email

**If parent account exists:**
3. Child provides first name and email
4. TrashMob calls Section 6 (child-initiated consent request)
   - Data: parent email/SiD/EID, child first name, birth date, email
5. PRIVO sends consent email to parent
6. Parent opens email, reviews features/policies, clicks "I Agree"
7. PRIVO sends webhook on consent completion
8. TrashMob generates Entra account link, sends email to child
9. Child creates account

**If parent account does NOT exist (NO FLOW):**
3. Child provides first name only
4. TrashMob sends email to parent requesting they create an account
5. Parent creates adult account (Flow 1), then adds child (Flow 2)

---

## Data Mapping: TrashMob → PRIVO

| TrashMob Field | PRIVO Attribute | Notes |
|----------------|-----------------|-------|
| `User.Id` (GUID) | EID (External ID) | Unique per user |
| `User.GivenName` | `trashmobservice_att_principal_given_name` | Or `granter_given_name` for adults |
| `User.SurName` | `trashmobservice_att_principal_family_name` | Or `granter_family_name` for adults |
| `User.Email` | `trashmobservice_att_principal_email` | Or `granter_email` for adults |
| `User.DateOfBirth` | `trashmobservice_att_principal_birthdate` | Or `granter_birthdate` for adults |
| `ParentalConsent.PrivoConsentRequestId` | Consent Identifier | From Sections 2, 5, 6 |
| `ParentalConsent.PrivoUserId` | SiD (Service Identifier) | From Sections 2, 5, 6 |

---

## Implementation Notes

### Token Management
- Tokens expire after 30 minutes by default
- Request extended expiration (up to 12 hours) from PRIVO
- Implement token pool/cache to avoid on-demand token creation
- Store in memory cache with sliding expiration

### Secrets Management
- Store INT/PROD Client ID and Secret in Azure Key Vault
- Store webhook API keys in Key Vault
- Never commit credentials to source control
- INT credentials in this document are for development only

### Webhook Security
- Define HTTP header name (e.g., `x-api-key`) and provide API key value to PRIVO
- Validate header on every incoming webhook request
- Separate API keys for INT and PROD endpoints
- Log all webhook events for audit trail

### Email Verification Sync
- Since Entra verifies email at account creation, call Section 8 after user creation to sync email verification status to PRIVO
- IAL value to be confirmed by PRIVO during integration

### Consent Revocation Handling
- When consent is revoked (Section 9), all associated feature data becomes inaccessible via UserInfo
- TrashMob must handle: disable minor account, retain consent artifacts per COPPA, notify parent

---

## Open Questions

1. ~~Consent collection methods available?~~ — Handled by PRIVO verification widget
2. ~~Webhook retry policy?~~ — Need to confirm with PRIVO
3. ~~Testing environment?~~ — INT environment provided (see credentials above)
4. **What IAL value should TrashMob use for email verification sync (Section 8)?** — Awaiting PRIVO guidance
5. ~~**What is the token expiration we should request?**~~ — Use default 30 min; not a concern for v1.
6. **What event_types can appear in webhooks?** — `consent_request_created` shown; need full list
7. **Webhook retry/failure policy?** — What happens if our endpoint is down?
8. ~~**Can consent requests be batched?**~~ — Deferred. Single-child flow only for v1; batch is a rare edge case.

---

## Related Documents

- [Project 23 — Parental Consent for Minors](./Project_23_Parental_Consent.md)
- [Project 23 — Privo Integration Package](./Project_23_Privo_Integration_Package.md)
- [Project 23 — Privo Proposed Flows](./Project_23_Privo_Proposed_Flows.md)
- [Project 1 — Auth Revamp](./Project_01_Auth_Revamp.md)
