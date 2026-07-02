# Project 23 — PRIVO Integration Test Scenarios

**Version:** 1.0
**Date:** March 27, 2026
**Environment:** PRIVO INT (`https://consent-svc-int.privo.com`)
**Service Identifier:** `trashmobservice`

---

## Prerequisites

- PRIVO INT credentials configured in Key Vault (or user-secrets for local dev)
- Webhook endpoint accessible from PRIVO INT (use ngrok or similar for local testing)
- Redirect URLs whitelisted with PRIVO: `https://dev.trashmob.eco`, `http://localhost:3000`
- Test email accounts (parent and child) that can receive email

---

## Flow 1: Adult Identity Verification

### 1.1 — Adult initiates identity verification (happy path)

**Preconditions:** Adult (18+) has a TrashMob account via Entra External ID.

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Adult navigates to profile/settings and clicks "Verify Identity" | Verify identity option visible for unverified adults |
| 2 | TrashMob backend calls `POST /token` (Section 1) | Access token returned, cached |
| 3 | TrashMob backend calls `POST /s2s/api/v1.0/trashmobservice/requests` (Section 2) with adult's name, DOB, email, EID (TrashMob User.Id) | Response contains: SiD, Consent Identifier, Consent URL |
| 4 | TrashMob stores SiD and Consent Identifier on user record | Database updated with PRIVO identifiers |
| 5 | Adult is redirected to PRIVO consent/verification URL | PRIVO verification widget loads with TrashMob branding |
| 6 | Adult completes identity verification | PRIVO processes verification |
| 7 | PRIVO sends webhook to TrashMob (`consent_request_created` or status update) | Webhook received and processed; user record updated |
| 8 | TrashMob calls `GET /s2s/api/v1.0/trashmobservice/accounts/sid/{sid}` (Section 4) | Returns verified data (name, DOB) |
| 9 | Adult returns to TrashMob | UI shows "Verified" status; "Add Child" option now available |

### 1.2 — Adult verification with direct widget URL (skip PRIVO pre-screens)

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | TrashMob creates consent request (Section 2) in background | SiD and Consent Identifier captured |
| 2 | TrashMob calls `POST .../consents/{consent_identifier}/verification/session?redirect_url=...` (Section 3) | Direct verification URL returned |
| 3 | Adult redirected to verification URL | Verification widget loads directly (no pre-screens) |
| 4 | After verification, adult redirected to `redirect_url` | Adult lands back on TrashMob with success parameter |

### 1.3 — Adult verification — token expired

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | TrashMob attempts API call with expired token | 401 response |
| 2 | TrashMob automatically refreshes token (Section 1) and retries | Retry succeeds with fresh token |

### 1.4 — Adult verification — user abandons

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Adult starts verification but closes browser/navigates away | No webhook received |
| 2 | Adult returns to TrashMob later | Status still shows "Unverified"; can re-initiate |
| 3 | TrashMob can check consent status via Section 7 | Returns current state of the pending request |

---

## Flow 2: Verified Adult Adds 13-17 Child

### 2.1 — Parent adds child (happy path)

**Preconditions:** Adult is identity-verified via PRIVO (Flow 1 complete). Adult has PRIVO SiD on record.

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Parent navigates to My Dependents and clicks "Add Child" | Add dependent form shown |
| 2 | Parent enters child info: first name, last name, DOB (13-17), email | Form validates: DOB in 13-17 range triggers PRIVO flow |
| 3 | TrashMob creates local Dependent record | Dependent record created with `IsActive = true` |
| 4 | TrashMob calls `POST /s2s/api/v1.0/trashmobservice/requests` (Section 5) with child data + parent reference (SiD or email) | Response contains: child SiD, Consent Identifier, Consent URL |
| 5 | TrashMob stores child SiD and Consent Identifier | Dependent record updated with PRIVO identifiers |
| 6 | Parent is presented with PRIVO consent page | Consent page shows: features list, Privacy Policy, ToS, Conduct Agreement |
| 7 | Parent clicks "I Agree" | Consent recorded in PRIVO |
| 8 | PRIVO sends webhook with consent completion | Webhook received; Dependent consent status updated to "Verified" |
| 9 | TrashMob generates Entra sign-up link and sends email to child | Child receives account creation email |
| 10 | Child clicks link, creates Entra account with the invited email | Account created; auto-linked to parent via existing `DependentInvitation` flow |
| 11 | Child logs in | `IsMinor = true`, `ParentUserId` set, minor protections active |

### 2.2 — Parent adds child under 13 (existing flow, no PRIVO)

**Preconditions:** Adult has a TrashMob account (PRIVO verification NOT required).

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Parent enters child info with DOB showing age < 13 | Standard dependent flow (no PRIVO) |
| 2 | Dependent created as parent-managed | No invitation option; no PRIVO calls; parent manages child entirely |

### 2.3 — Parent adds child exactly age 13

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Parent enters child DOB that makes them exactly 13 today | PRIVO consent flow triggered (not the under-13 flow) |

### 2.4 — Unverified parent tries to add 13-17 child

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Unverified adult tries to add dependent with DOB in 13-17 range | Blocked — message: "You must verify your identity before adding a child aged 13-17" |
| 2 | Link/button to start identity verification (Flow 1) | Redirects to verification flow |

### 2.5 — Parent adds child but does NOT complete consent

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Parent starts PRIVO consent but abandons | Dependent record exists with status "Pending" |
| 2 | Child cannot create account | No invitation email sent; no account linking |
| 3 | Parent can retry consent later | Re-initiates PRIVO consent flow for same dependent |

---

## Flow 3: Child-Initiated Account (Parent Exists)

### 3.1 — Child initiates, parent account exists and is verified (happy path)

**Preconditions:** Parent has a verified TrashMob account with PRIVO SiD.

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Child visits TrashMob and clicks "Sign Up" | Age gate shown |
| 2 | Child enters DOB (13-17) | System detects minor; shows "Parent account required" |
| 3 | Child enters parent's email | TrashMob checks if parent account exists — YES |
| 4 | Child provides first name and email | "Pending consent" message shown |
| 5 | TrashMob calls `POST .../requests` (Section 6) with parent email/SiD, child name, DOB, email | Response: child SiD, Consent Identifier |
| 6 | PRIVO sends consent email to parent | Parent receives consent request email |
| 7 | Parent opens email, clicks link | PRIVO consent page loads |
| 8 | Parent reviews features/policies, clicks "I Agree" | Consent recorded |
| 9 | PRIVO sends webhook to TrashMob | Consent status updated |
| 10 | TrashMob generates Entra sign-up link, sends to child email | Child receives account creation email |
| 11 | Child creates account via Entra | Account auto-linked to parent; `IsMinor = true` |

### 3.2 — Child initiates, parent account exists but is NOT verified

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Child enters parent email | Parent account found but not PRIVO-verified |
| 2 | System sends parent an email asking them to verify first | Parent receives "verify your identity" email with link |
| 3 | Parent completes verification (Flow 1) | Parent is now verified |
| 4 | Parent can then complete consent for child | Flow continues from step 6 of 3.1 |

### 3.3 — Child initiates, parent account does NOT exist (NO FLOW)

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Child enters parent email | TrashMob checks — no account found |
| 2 | Child can enter another email and try again | Option to try different email |
| 3 | Child clicks "Request Consent" | Child provides first name only |
| 4 | TrashMob sends email to parent requesting account creation | Parent receives email with link to create TrashMob account |
| 5 | Parent creates account (Entra sign-up) | New adult account created |
| 6 | Parent completes identity verification (Flow 1) | Parent now verified |
| 7 | Parent adds child (Flow 2) | Full consent flow completed |
| 8 | Child receives account creation email | Child can now create account |

### 3.4 — Child enters DOB under 13

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Child enters DOB showing age < 13 | Blocked: "You must be 13 or older to create an account" |
| 2 | No PII collected | No data sent to PRIVO; no records created |

### 3.5 — Child enters DOB 18+

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Child enters DOB showing age 18+ | Standard adult sign-up flow (no PRIVO consent needed) |

---

## Flow 4: Webhook Processing

### 4.1 — Consent granted webhook

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | PRIVO sends webhook with consent approval | Endpoint returns 200 |
| 2 | TrashMob matches `consent_identifiers` to stored record | Correct user/dependent found |
| 3 | Consent status updated | Database record set to "Verified" |
| 4 | If child-initiated: account creation email sent to child | Email dispatched |

### 4.2 — Consent denied webhook

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | PRIVO sends webhook with denial | Endpoint returns 200 |
| 2 | Consent status updated to "Denied" | Dependent record updated |
| 3 | If child had pending account, no email sent | No account creation triggered |
| 4 | Parent notified of denial | Informational email or dashboard update |

### 4.3 — Consent revoked webhook

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Parent revokes consent via PRIVO (or TrashMob calls Section 9) | Webhook received |
| 2 | Minor account suspended | `IsMinor` user can no longer log in or participate |
| 3 | Consent artifacts retained | Records kept per COPPA legal requirements |
| 4 | Minor's data no longer returned by PRIVO UserInfo | Confirmed via Section 4 call |

### 4.4 — Webhook with invalid API key

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Request arrives with wrong/missing `X-Api-Key` | 401 Unauthorized returned |
| 2 | No processing occurs | No database changes |
| 3 | Attempt logged | Security event logged to App Insights |

### 4.5 — Webhook with unknown consent_identifier

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Webhook contains consent_identifier not in our database | 200 returned (acknowledge receipt) |
| 2 | Warning logged | Log entry for investigation |
| 3 | No database changes | No records affected |

### 4.6 — Duplicate webhook delivery

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Same webhook payload delivered twice | Both return 200 |
| 2 | Second delivery is idempotent | No duplicate processing; same end state |

---

## Flow 5: Email Verification Sync

### 5.1 — Sync Entra-verified email to PRIVO

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | User creates Entra account (email verified by Entra) | Account created successfully |
| 2 | TrashMob calls Section 8 (PATCH .../attributes/{email_attr}/ial) with user's SiD | PRIVO acknowledges email verification |
| 3 | PRIVO UserInfo shows email as verified | No duplicate verification prompts from PRIVO |

---

## Flow 6: Consent Revocation (TrashMob-Initiated)

### 6.1 — Parent revokes consent from TrashMob

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Parent navigates to dependent management | Dependent listed with "Verified" consent |
| 2 | Parent clicks "Revoke Consent" | Confirmation dialog shown |
| 3 | TrashMob calls Section 9 (POST .../features/revoke) with parent SiD + child SiD | PRIVO revokes consent |
| 4 | Minor account suspended in TrashMob | `ConsentStatus = "Revoked"`, account disabled |
| 5 | PRIVO UserInfo returns empty data for revoked features | Confirmed |

---

## Flow 7: Token Management

### 7.1 — Token acquisition

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | First API call triggers token acquisition | `POST /token` called with INT credentials |
| 2 | Token cached in memory | Subsequent calls reuse cached token |
| 3 | Token used in Authorization header on all PRIVO API calls | API calls succeed |

### 7.2 — Token refresh on expiry

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Cached token expires (30 min) | Next API call gets 401 |
| 2 | Automatic retry with fresh token | New token acquired; original call retried and succeeds |

---

## Flow 8: Edge Cases & Error Handling

### 8.1 — PRIVO API unavailable

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | PRIVO INT endpoint returns 500 or times out | TrashMob shows user-friendly error |
| 2 | User can retry | Retry button available; no partial state left |
| 3 | Error logged | App Insights event with PRIVO error details |

### 8.2 — Duplicate EID (External ID)

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Same TrashMob User.Id sent to PRIVO twice for adult verification | PRIVO returns existing record (or error) |
| 2 | TrashMob handles gracefully | Uses existing SiD; no duplicate consent requests |

### 8.3 — Child email already exists in Entra

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Parent adds 13-17 child with email that already has an Entra account | Entra sign-up link detects existing account |
| 2 | Child signs in instead of signing up | Account linked to parent via invitation flow |

### 8.4 — Parent and child use same email

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Parent tries to add child with parent's own email | Validation error: child email must differ from parent |

### 8.5 — Minor turns 18

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Previously consented minor reaches 18th birthday | Minor protections can be lifted |
| 2 | User record updated | `IsMinor = false`; standard adult experience |
| 3 | PRIVO consent record remains for audit | Historical consent retained |

---

## Data to Capture & Verify in Each Flow

For every PRIVO API call, verify these are stored correctly:

| Field | Source | Storage Location |
|-------|--------|-----------------|
| PRIVO Access Token | Section 1 response | In-memory cache (not DB) |
| Adult SiD | Section 2 response | `User.PrivoSid` (new field) |
| Child SiD | Section 5/6 response | `Dependent.PrivoSid` (new field) or `User.PrivoSid` after account creation |
| Consent Identifier | Section 2/5/6 response | `ParentalConsent.PrivoConsentRequestId` |
| Consent URL | Section 2/5 response | Transient (used for redirect, not stored) |
| Verification URL | Section 3 response | Transient (used for redirect, not stored) |
| Webhook event_types | Section 10 payload | Logged; drives status updates |
| Granter SiD | Section 6 response / webhook | `ParentalConsent.PrivoGranterSid` (new field) |

---

## Test Environment Checklist

- [ ] PRIVO INT Client ID configured in user-secrets or Key Vault
- [ ] PRIVO INT Secret configured in user-secrets or Key Vault
- [ ] Webhook API key generated and shared with PRIVO for INT
- [ ] `https://dev.trashmob.eco` whitelisted as redirect URL in PRIVO
- [ ] `http://localhost:3000` whitelisted as redirect URL in PRIVO (local dev)
- [ ] ngrok or dev tunnel configured for local webhook testing
- [ ] Test parent email account accessible
- [ ] Test child email account accessible
- [ ] PRIVO INT Swagger available: https://consent-svc-int.privo.com/docs/s2s/

---

## Related Documents

- [Project 23 — PRIVO API Requirements](./Project_23_Privo_API_Requirements.md)
- [Project 23 — Parental Consent for Minors](./Project_23_Parental_Consent.md)
- [Project 23 — Privo Integration Package](./Project_23_Privo_Integration_Package.md)
- [Project 23 — Privo Proposed Flows](./Project_23_Privo_Proposed_Flows.md)
