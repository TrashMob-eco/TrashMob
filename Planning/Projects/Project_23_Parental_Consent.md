# Project 23 — Parental Consent for Minors (via Privo.com)

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in Progress (legal & Privo.com integration) |
| **Priority** | High |
| **Risk** | High |
| **Size** | Large |
| **Dependencies** | Project 1 (Auth Revamp), Legal Review |

---

## Business Rationale

Support parent-managed dependents or direct minor registration with age verification and protections. Expanding to minors (13+) significantly increases the volunteer base while maintaining safety and legal compliance.

---

## Objectives

### Primary Goals
- **Parent-managed dependents flow** for families
- **Direct minor registration** with age verification
- **Enhanced protections** (no DMs, adult presence requirements)
- **COPPA compliance** via Privo.com integration

### Secondary Goals
- Family accounts
- Minor activity reports for parents
- School/youth group integration
- Parental dashboard

---

## Scope

### Phase 1 - Age Verification
- ✅ Age gate during registration
- ✅ Privo.com integration for verification
- ✅ Under-13 block with explanation
- ✅ 13-17 minor flow trigger

### Phase 2 - Parental Consent
- ✅ Verifiable Parental Consent (VPC) via Privo
- ✅ Parent notification workflow
- ✅ Consent status tracking
- ✅ Consent artifact retention

### Phase 3 - Minor Protections
- ✅ No direct messaging for minors
- ✅ Adult presence enforcement at events
- ✅ Limited profile visibility
- ✅ Parent view of minor activity

### Phase 4 - Family Features
- ❓ Parent can manage multiple minors
- ❓ Family registration flow
- ❓ Parent approval for event registration
- ❓ Parent notification of event participation

---

## Out-of-Scope

- ❌ Under-13 registration (COPPA restriction)
- ❌ Minor-only events
- ❌ School/organization bulk registration
- ❌ Payment processing for families

---

## Success Metrics

### Quantitative
- **Minor signups:** Track adoption rate
- **Consent completion rate:** ≥ 80% of minor registrations
- **Parent approval time:** < 48 hours average
- **Compliance audit success:** Pass all audits

### Qualitative
- Parents trust platform with minor data
- Minors have positive experience
- No compliance incidents
- Easy parent/minor workflows

---

## Dependencies

### Blockers
- **Project 1 (Auth Revamp):** Integration with Entra External ID
- **Legal Review:** COPPA compliance sign-off
- **Privo.com Contract:** Vendor agreement

### Enables
- Youth volunteer participation
- Family engagement
- School/community partnerships

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **COPPA violation** | Low | Critical | Privo.com compliance; legal review; regular audits |
| **False age claims** | Medium | High | Privo age verification; periodic re-verification |
| **Parent consent fraud** | Low | High | Privo VPC process; consent validation |
| **Minor safety incident** | Low | Critical | Adult presence requirement; clear policies; training |
| **Complex implementation** | High | Medium | Phased rollout; Privo expertise; thorough testing |

---

## Privo.com Integration

### Services Used

**Age Verification:**
- Age gate with Privo verification
- Under-13 detection and block
- 13-17 minor identification
- Adult (18+) standard flow

**Verifiable Parental Consent (VPC):**
- Parent notification via email
- Consent collection methods (credit card, ID, video call)
- Consent verification and validation
- Consent storage and retrieval

**Compliance Documentation:**
- Audit trail for all consents
- COPPA compliance reports
- Consent revocation handling

### Integration Points

```csharp
// Privo service integration
public interface IPrivoService
{
    Task<AgeVerificationResult> VerifyAgeAsync(string birthDate, Guid userId);
    Task<ConsentRequest> RequestParentalConsentAsync(MinorConsentRequest request);
    Task<ConsentStatus> GetConsentStatusAsync(Guid consentRequestId);
    Task<bool> ValidateConsentAsync(Guid userId);
    Task RevokeConsentAsync(Guid userId, string reason);
}
```

---

## Implementation Plan

### Data Model Changes

```sql
-- Minor status tracking
ALTER TABLE Users
ADD DateOfBirth DATE NULL,
    IsMinor BIT NOT NULL DEFAULT 0,
    ParentUserId UNIQUEIDENTIFIER NULL,
    ConsentStatus NVARCHAR(50) NULL, -- Pending, Verified, Expired, Revoked
    ConsentVerifiedDate DATETIMEOFFSET NULL,
    PrivoUserId NVARCHAR(100) NULL,
    FOREIGN KEY (ParentUserId) REFERENCES Users(Id);

-- Parental consent records
CREATE TABLE ParentalConsents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MinorUserId UNIQUEIDENTIFIER NOT NULL,
    ParentEmail NVARCHAR(256) NOT NULL,
    ParentUserId UNIQUEIDENTIFIER NULL, -- If parent has account
    -- Privo integration
    PrivoConsentRequestId NVARCHAR(100) NOT NULL,
    PrivoConsentMethod NVARCHAR(50) NULL, -- CreditCard, ID, VideoCall
    -- Status
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Verified, Denied, Expired, Revoked
    RequestedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    VerifiedDate DATETIMEOFFSET NULL,
    ExpiresDate DATETIMEOFFSET NULL,
    RevokedDate DATETIMEOFFSET NULL,
    RevokedReason NVARCHAR(500) NULL,
    -- Audit
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (MinorUserId) REFERENCES Users(Id),
    FOREIGN KEY (ParentUserId) REFERENCES Users(Id)
);

-- Minor event attendance tracking
CREATE TABLE MinorEventParticipation (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    MinorUserId UNIQUEIDENTIFIER NOT NULL,
    SupervisingAdultUserId UNIQUEIDENTIFIER NULL, -- Adult present at event
    ParentApproved BIT NOT NULL DEFAULT 0,
    ParentApprovedDate DATETIMEOFFSET NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    FOREIGN KEY (MinorUserId) REFERENCES Users(Id),
    FOREIGN KEY (SupervisingAdultUserId) REFERENCES Users(Id)
);

CREATE INDEX IX_Users_IsMinor ON Users(IsMinor) WHERE IsMinor = 1;
CREATE INDEX IX_ParentalConsents_MinorUserId ON ParentalConsents(MinorUserId);
CREATE INDEX IX_ParentalConsents_Status ON ParentalConsents(Status);
```

### API Changes

```csharp
// Age verification during registration
[HttpPost("api/auth/verify-age")]
public async Task<ActionResult<AgeVerificationDto>> VerifyAge([FromBody] AgeVerificationRequest request)
{
    // Call Privo for age verification
    // Return result: Adult, Minor, UnderAge, VerificationRequired
}

// Request parental consent
[Authorize]
[HttpPost("api/consent/request")]
public async Task<ActionResult<ConsentRequestDto>> RequestParentalConsent([FromBody] ConsentRequest request)
{
    // Create consent request via Privo
    // Send parent notification
}

// Check consent status
[Authorize]
[HttpGet("api/consent/status")]
public async Task<ActionResult<ConsentStatusDto>> GetConsentStatus()
{
    // Return current consent status for minor
}

// Privo webhook for consent updates
[HttpPost("api/webhooks/privo")]
public async Task<ActionResult> ProcessPrivoWebhook([FromBody] PrivoEvent privoEvent)
{
    // Update consent status
    // Notify minor and parent
}

// Parent view of minor activity
[Authorize]
[HttpGet("api/parent/minors/{minorId}/activity")]
public async Task<ActionResult<MinorActivityDto>> GetMinorActivity(Guid minorId)
{
    // Validate parent relationship
    // Return minor's events and metrics
}

// Parent approval for event
[Authorize]
[HttpPost("api/parent/minors/{minorId}/events/{eventId}/approve")]
public async Task<ActionResult> ApproveEventParticipation(Guid minorId, Guid eventId)
{
    // Validate parent relationship
    // Mark event participation as approved
}
```

### Registration Flow

```
User clicks "Sign Up"
         │
         ▼
   Enter birthdate
         │
         ▼
┌────────┴────────┐
│   Privo Age     │
│   Verification  │
└────────┬────────┘
         │
    ┌────┴────┬────────┐
    ▼         ▼        ▼
 Under 13   13-17    18+
    │         │        │
    ▼         ▼        ▼
 BLOCKED   Minor    Standard
           Flow     Registration
             │
             ▼
    Enter parent email
             │
             ▼
    Privo VPC Request
             │
             ▼
    Pending status
    (limited access)
             │
    ┌────────┴────────┐
    ▼                 ▼
 Parent           Timeout
 Consents         (7 days)
    │                 │
    ▼                 ▼
 Full access      Account
 (with limits)    disabled
```

### Minor Protections

1. **Communication Restrictions:**
   - No in-app messaging (Project 12)
   - No direct contact with other users
   - All communications through parent

2. **Profile Visibility:**
   - Name shown as first name + last initial
   - No photo display to non-leads
   - No location sharing

3. **Event Participation:**
   - Adult must be present at event
   - Parent notification of registration
   - Optional parent approval required

4. **Data Retention:**
   - Consent artifacts per legal requirements
   - Regular consent re-verification
   - Right to deletion (with parent request)

---

## Implementation Phases

### Phase 1: Age Gate
- Privo integration setup
- Age verification during signup
- Under-13 blocking
- Minor flag in database

### Phase 2: Parental Consent
- VPC workflow implementation
- Parent notification
- Consent status tracking
- Pending account limitations

### Phase 3: Minor Protections
- Communication restrictions
- Profile visibility limits
- Adult presence tracking
- Parent notification system

### Phase 4: Family Features
- Parent dashboard
- Multiple minor management
- Event approval workflow
- Activity reporting

**Note:** Legal sign-off required before Phase 1 deployment.

---

## Open Questions

1. **Consent re-verification frequency?**
   **Recommendation:** Annual re-verification; Privo handles timing
   **Owner:** Legal + Product
   **Due:** Before Phase 2

2. **Minimum adult-to-minor ratio at events?**
   **Recommendation:** 1 adult per 5 minors; event lead discretion
   **Owner:** Legal + Product
   **Due:** Before Phase 3

3. **What if parent revokes consent?**
   **Recommendation:** Immediate account suspension; data retention per legal
   **Owner:** Legal
   **Due:** Before Phase 2

4. **School/organization bulk consent?**
   **Recommendation:** Out of scope for v1; evaluate demand
   **Owner:** Product Lead
   **Due:** Future

5. **Privo.com cost structure and timeline?**
   **Recommendation:** Complete contract negotiation
   **Owner:** Business Team
   **Due:** Before Phase 1

---

## Related Documents

- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** - Authentication integration
- **[Project 8 - Waivers V3](./Project_08_Waivers_V3.md)** - Minor waiver handling
- **[Privo.com](https://privo.com)** - Vendor documentation

---

**Last Updated:** January 24, 2026
**Owner:** Product Lead + Legal + Engineering
**Status:** Planning in Progress
**Next Review:** After Privo contract finalization
