# Integrating PRIVO with Microsoft Entra External ID

**A practical guide for organizations migrating from Azure AD B2C**

**Author:** TrashMob.eco Engineering
**Date:** April 2026
**Audience:** Development teams at organizations that use Azure AD B2C and want to integrate PRIVO for children's privacy compliance

---

## Who This Guide Is For

You're an organization that:
- Currently uses **Azure AD B2C** for customer identity
- Needs to add **COPPA-compliant parental consent** for users aged 13-17
- Wants to integrate **PRIVO** for identity verification and consent management
- Is considering (or has been told to consider) migrating to **Microsoft Entra External ID**

TrashMob.eco was the first organization to complete this integration. This guide documents what we learned so your team doesn't have to figure it out from scratch.

---

## Part 1: Migrating from Azure AD B2C to Entra External ID

### Why Migrate?

Azure AD B2C is being superseded by **Entra External ID** (also called CIAM — Customer Identity and Access Management). Key differences:

| Aspect | Azure AD B2C | Entra External ID |
|--------|-------------|-------------------|
| **Custom policies** | XML-based Identity Experience Framework (IEF) | Custom Authentication Extensions (code-based) |
| **Token format** | v1 or v2 tokens | v2 tokens only |
| **Admin portal** | Separate B2C portal | Integrated into Entra admin center |
| **User flows** | Built-in + custom policies | Built-in + custom auth extensions |
| **OIDC discovery** | Standard endpoint | Two endpoints with different signing keys (see below) |
| **Status** | Legacy (still supported) | Active development, recommended for new projects |

### Migration Overview

The migration is primarily a configuration change, not a code rewrite. Your application code (MSAL calls, token validation) stays largely the same.

**High-level steps:**

1. **Create a CIAM tenant** in the Entra admin center
2. **Register your application** in the new tenant
3. **Configure user flows** (sign-up, sign-in) in the new tenant
4. **Migrate user data** from B2C to the new tenant (Microsoft provides migration tools)
5. **Update MSAL configuration** in your application to point to the new tenant
6. **Test** thoroughly — especially token claims and user resolution
7. **Cut over** production traffic

### Critical: CIAM OIDC Discovery Endpoints

This is the single most important thing to know about Entra External ID that isn't obvious from the documentation.

CIAM tenants have **two** OIDC discovery endpoints with **different signing keys**:

```
Standard:  https://login.microsoftonline.com/{tenantId}/v2.0
CIAM:      https://{tenantId}.ciamlogin.com/{tenantId}/v2.0
```

**The CIAM endpoint has a superset of signing keys.** Entra may sign tokens with keys that only exist in the CIAM discovery document. If your application validates tokens using the standard `login.microsoftonline.com` endpoint, you will get `IDX10503` signature validation failures for some users.

**Fix:** Always use the `ciamlogin.com` endpoint as your JWT Authority:

```csharp
// ASP.NET Core
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{tenantId}.ciamlogin.com/{tenantId}/v2.0";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuers = new[]
            {
                $"https://login.microsoftonline.com/{tenantId}/v2.0",
                $"https://{tenantId}.ciamlogin.com/{tenantId}/v2.0",
            },
        };
    });
```

Accept tokens from **both** issuers since the `iss` claim in the token can use either format.

### CIAM Token Differences

**id_tokens may not include an `email` claim.** This is different from B2C where email was typically available. You have two options:

1. **Use the `oid` claim** (Object ID) as the primary identifier instead of email
2. **Fetch email via Microsoft Graph API** using `User.Read.All` application permission

We use a 4-step resolution: email lookup → ObjectId lookup → Graph API email resolution → auto-create user.

### Custom Authentication Extensions

B2C used XML-based custom policies (IEF) for custom logic during sign-up/sign-in. Entra External ID uses **Custom Authentication Extensions** — Azure Functions or Container Apps that Entra calls at specific points in the user flow.

Available extension points:
- `OnTokenIssuanceStart` — modify token claims
- `OnAttributeCollectionStart` — pre-populate attributes
- `OnAttributeCollectionSubmit` — validate/transform submitted attributes

For PRIVO integration, you might consider using `OnAttributeCollectionSubmit` to verify age during sign-up. However, **we chose not to do this** — instead, we implemented the age gate in-app (before the MSAL login redirect) and integrated PRIVO post-authentication. This is simpler and doesn't require maintaining a Custom Authentication Extension.

---

## Part 2: Integrating PRIVO

### Architecture Decision: Where Does PRIVO Fit?

There are two approaches to integrating PRIVO with Entra External ID:

**Option A: During sign-up (Custom Authentication Extension)**
- Entra calls your extension during sign-up
- Extension calls PRIVO for age verification
- Extension blocks or modifies the sign-up based on age
- **Pro:** Server-side, can't be bypassed
- **Con:** Complex to set up, maintain, and debug; tightly coupled to Entra

**Option B: Post-authentication (recommended)**
- User completes Entra sign-up/sign-in normally
- Your application checks age and initiates PRIVO flows
- Age gate is in-app (before or after authentication)
- **Pro:** Simpler, easier to debug, decoupled from identity provider
- **Con:** Age gate can technically be bypassed (but PRIVO consent still required for feature access)

**We chose Option B.** The PRIVO integration is entirely in our application layer — Entra handles authentication, PRIVO handles consent. They don't need to talk to each other.

### PRIVO Integration Steps

#### Step 1: Get PRIVO Credentials

PRIVO will provide:
- **INT (Integration) environment** credentials for testing
- **Service identifier** for your application
- **Feature identifiers** for each feature requiring parental consent
- **Attribute identifiers** for user data fields

#### Step 2: Implement Token Management

PRIVO uses OAuth 2.0 client credentials. Tokens expire after 30 minutes.

```csharp
// POST https://consent-svc-int.privo.com/token
// Content-Type: application/x-www-form-urlencoded
// Body: client_id=...&client_secret=...&scope=openid&grant_type=client_credentials

public class PrivoTokenManager
{
    private string cachedToken;
    private DateTime tokenExpiry;
    private readonly SemaphoreSlim semaphore = new(1, 1);

    public async Task<string> GetTokenAsync()
    {
        if (cachedToken != null && DateTime.UtcNow < tokenExpiry)
            return cachedToken;

        await semaphore.WaitAsync();
        try
        {
            if (cachedToken != null && DateTime.UtcNow < tokenExpiry)
                return cachedToken;

            // Fetch new token from PRIVO
            var response = await httpClient.PostAsync("/token", formContent);
            var json = await response.Content.ReadAsStringAsync();
            cachedToken = ParseAccessToken(json);
            tokenExpiry = DateTime.UtcNow.AddMinutes(25); // 5-min buffer
            return cachedToken;
        }
        finally
        {
            semaphore.Release();
        }
    }
}
```

**Key lesson:** Cache tokens with a buffer before expiry. Use a semaphore to prevent concurrent token acquisition requests.

#### Step 3: Implement Adult Identity Verification (PRIVO Section 2 + 3)

Before a parent can add children, they must verify their identity.

**Flow:**
1. Create a consent request (Section 2) with the parent's data
2. Get a direct verification URL (Section 3) to skip PRIVO's pre-screens
3. Redirect the parent to the verification widget
4. Receive a webhook when verification completes
5. Poll Section 7 to confirm the state

**Request payload for adult verification:**

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
    "given_name": "ParentFirstName",
    "birthdate": "19900101",
    "birthdate_precision": "yyyymmdd",
    "email": "parent@example.com",
    "email_verified": true,
    "eid": "your-internal-user-id",
    "attributes": [
      {
        "attribute_identifier": "yourservice_att_granter_family_name",
        "value": ["ParentLastName"]
      }
    ]
  }
}
```

**Important notes from our experience:**
- Set `consent_request_email` to `false` for adult self-verification — otherwise PRIVO sends a consent email to the parent asking them to approve their own verification, which is confusing
- Set `consent_approved_email` to `true` so the parent gets a confirmation
- Use your internal user ID as the `eid` (External ID) — this links the PRIVO record to your user
- `birthdate` format is `yyyyMMdd` (no separators) — `yyyy-MM-dd` returns a "Bad Date format" error
- The `granter` object is required even for adult self-verification

#### Step 4: Implement Parental Consent for Children (PRIVO Section 5)

After a parent is verified, they can add children.

**Request payload for child consent:**

```json
{
  "granter": {
    "email": "parent@example.com",
    "sid": "parent-privo-sid-from-step-3",
    "notifications": [
      { "is_on": true, "notification_type": "consent_request_email" }
    ]
  },
  "locale": "en-US",
  "principal": {
    "given_name": "ChildFirstName",
    "birthdate": "20130101",
    "birthdate_precision": "yyyymmdd",
    "eid": "your-internal-child-id"
  }
}
```

**Key differences from adult verification:**
- `consent_request_email` is `true` — PRIVO emails the parent a consent link
- The parent's `sid` (from their verification) links them as the granter
- Child email is optional — include it only if you have it
- No direct widget redirect — the parent receives an email from PRIVO instead

#### Step 5: Handle Webhooks

PRIVO sends webhooks when consent events occur. Secure them with an API key header.

**Webhook payload format:**

```json
{
  "id": "webhook-uuid",
  "timestamp": "2026-04-01T03:25:16Z",
  "sid": "user-privo-sid",
  "event_types": ["consent_updated", "account_feature_updated"],
  "granter_sid": ["parent-privo-sid"],
  "consent_identifiers": ["consent-uuid"]
}
```

**Critical lesson:** Don't try to determine consent status from the `event_types` array. PRIVO sends events like `consent_updated` and `account_feature_updated` — not `consent_approved` as you might expect. Instead, when you receive any non-creation webhook, **poll Section 7** (`GET /consents/{id}`) for the authoritative state:

```csharp
// Don't do this:
if (eventTypes.Contains("consent_approved")) { /* won't work */ }

// Do this instead:
if (!IsCreationEvent(eventTypes))
{
    var status = await privoService.GetConsentStatusAsync(consentId);
    if (status == "approved") { /* update your records */ }
}
```

#### Step 6: Implement Feature Permission Gating

PRIVO returns which features the parent approved. Query Section 4 (UserInfo) by your external ID:

```
GET /s2s/api/v1.0/{service}/accounts/eid/{your-internal-id}
```

Response includes a `features` array with each feature's state (`on`, `off`, `pending`). Cache this for 1 hour per user and gate your UI accordingly.

**Fail-closed:** If a feature key is missing from the response, treat it as disabled. This is safer for COPPA compliance.

---

## Part 3: Lessons Learned

### Things That Tripped Us Up

1. **PRIVO date format is `yyyyMMdd`** — not `yyyy-MM-dd`. The error message ("Bad Date format") doesn't tell you what format is expected.

2. **The `granter` object is required for adult verification** — even though the adult is both principal and granter. Without it, you get "Granter info is required."

3. **Section 3 (verification URL) returns 403 if your redirect URL isn't whitelisted** — PRIVO must whitelist your hostname. Only the base URL needs whitelisting; path and query parameters can be set freely.

4. **Don't rely on webhook event types for status** — Poll Section 7 instead. The event type names don't map obviously to consent states.

5. **PRIVO's INT environment processes verification in real-time** — but only after they've configured your service. If consent status stays at "processing" for more than a few minutes, something is misconfigured on PRIVO's side.

6. **Creating multiple consent requests for the same EID can cause 403 errors** — If you need to restart testing, delete the PRIVO account first (`DELETE /accounts/eid/{id}`), then create a fresh consent request.

7. **Token caching is essential** — PRIVO suggests maintaining a pool of tokens. At minimum, cache with a semaphore to prevent concurrent refresh races.

8. **CIAM id_tokens lack email claims** — You'll need Microsoft Graph API (`User.Read.All` application permission) to resolve emails from Object IDs. This is a CIAM-specific behavior, not a PRIVO issue.

### Architecture Recommendations

1. **Keep PRIVO decoupled from your identity provider.** Don't integrate PRIVO into Custom Authentication Extensions. It's simpler and more maintainable to handle PRIVO in your application layer.

2. **Use your internal IDs as PRIVO External IDs (EIDs).** This gives you a clean mapping between your user records and PRIVO records without storing PRIVO-specific IDs everywhere.

3. **Store PRIVO credentials in a secret manager** (Key Vault, AWS Secrets Manager, etc.) with a feature flag to enable/disable the integration per environment.

4. **Build a "Check Status" polling endpoint** as a fallback for webhooks. Webhooks may be delayed or fail, and having a manual polling option prevents support tickets.

5. **Gate features on the client side, not just the server.** Fetch permissions once on login, cache for an hour, and hide/show UI elements based on what the parent approved. This gives a better user experience than showing features and then blocking with an error.

---

## Part 4: Quick Reference

### PRIVO Environments

| Environment | Base URL |
|-------------|----------|
| Integration (INT) | `https://consent-svc-int.privo.com` |
| Production | `https://consent-svc.privo.com` |

### API Sections Used

| Section | Method | Endpoint | Purpose |
|---------|--------|----------|---------|
| 1 | POST | `/token` | Get access token |
| 2 | POST | `/s2s/api/v1.0/{svc}/requests` | Create consent request |
| 3 | POST | `/s2s/api/v1.0/{svc}/consents/{id}/verification/session` | Get verification widget URL |
| 4 | GET | `/s2s/api/v1.0/{svc}/accounts/eid/{eid}` | Get user info / feature states |
| 5 | POST | `/s2s/api/v1.0/{svc}/requests` | Parent-initiated child consent |
| 7 | GET | `/s2s/api/v1.0/{svc}/consents/{id}` | Check consent status |
| 9 | POST | `/s2s/api/v1.0/{svc}/accounts/sid/{sid}/{granter}/features/revoke` | Revoke consent |

### Swagger Documentation

- **INT:** https://consent-svc-int.privo.com/docs/s2s/
- **Production:** https://consent-svc.privo.com/docs/s2s/

---

*This guide was written by the TrashMob.eco engineering team based on our experience as the first organization to integrate Microsoft Entra External ID with PRIVO. For questions or corrections, contact us at https://www.trashmob.eco/contactus.*
