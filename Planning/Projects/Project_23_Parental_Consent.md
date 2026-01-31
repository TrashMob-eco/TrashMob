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
- **Project 19 (Newsletter):** First name personalization (FirstName field added to User model)

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

**Modification: User (add minor-related properties and optional name)**
```csharp
// Add to existing TrashMob.Models/User.cs
#region Profile (Optional Name)

/// <summary>
/// Gets or sets the user's first name (optional, for personalization).
/// </summary>
public string FirstName { get; set; }

/// <summary>
/// Gets or sets the user's last name (optional, required for minors per Privo).
/// </summary>
public string LastName { get; set; }

#endregion

#region Minor Support (Privo.com Integration)

/// <summary>
/// Gets or sets the user's date of birth for age verification.
/// </summary>
public DateOnly? DateOfBirth { get; set; }

/// <summary>
/// Gets or sets whether this user is a minor (13-17).
/// </summary>
public bool IsMinor { get; set; }

/// <summary>
/// Gets or sets the parent's user ID (if minor has parent account).
/// </summary>
public Guid? ParentUserId { get; set; }

/// <summary>
/// Gets or sets the consent status (Pending, Verified, Expired, Revoked).
/// </summary>
public string ConsentStatus { get; set; }

/// <summary>
/// Gets or sets when parental consent was verified.
/// </summary>
public DateTimeOffset? ConsentVerifiedDate { get; set; }

/// <summary>
/// Gets or sets the Privo.com user identifier.
/// </summary>
public string PrivoUserId { get; set; }

#endregion

// Navigation property
public virtual User ParentUser { get; set; }
public virtual ICollection<User> Dependents { get; set; }
```

**New Entity: ParentalConsent**
```csharp
// New file: TrashMob.Models/ParentalConsent.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Records parental consent for a minor user via Privo.com.
    /// </summary>
    public class ParentalConsent : KeyedModel
    {
        /// <summary>
        /// Gets or sets the minor user's identifier.
        /// </summary>
        public Guid MinorUserId { get; set; }

        /// <summary>
        /// Gets or sets the parent's email address.
        /// </summary>
        public string ParentEmail { get; set; }

        /// <summary>
        /// Gets or sets the parent's user ID (if they have an account).
        /// </summary>
        public Guid? ParentUserId { get; set; }

        #region Privo Integration

        /// <summary>
        /// Gets or sets the Privo consent request identifier.
        /// </summary>
        public string PrivoConsentRequestId { get; set; }

        /// <summary>
        /// Gets or sets the consent method used (CreditCard, ID, VideoCall).
        /// </summary>
        public string PrivoConsentMethod { get; set; }

        #endregion

        #region Status

        /// <summary>
        /// Gets or sets the consent status (Pending, Verified, Denied, Expired, Revoked).
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets when consent was requested.
        /// </summary>
        public DateTimeOffset RequestedDate { get; set; }

        /// <summary>
        /// Gets or sets when consent was verified.
        /// </summary>
        public DateTimeOffset? VerifiedDate { get; set; }

        /// <summary>
        /// Gets or sets when consent expires.
        /// </summary>
        public DateTimeOffset? ExpiresDate { get; set; }

        /// <summary>
        /// Gets or sets when consent was revoked.
        /// </summary>
        public DateTimeOffset? RevokedDate { get; set; }

        /// <summary>
        /// Gets or sets the reason for revocation.
        /// </summary>
        public string RevokedReason { get; set; }

        #endregion

        // Navigation properties
        public virtual User MinorUser { get; set; }
        public virtual User ParentUser { get; set; }
    }
}
```

**New Entity: MinorEventParticipation**
```csharp
// New file: TrashMob.Models/MinorEventParticipation.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Tracks a minor's participation in an event with parental approval and supervision.
    /// </summary>
    public class MinorEventParticipation : KeyedModel
    {
        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the minor user's identifier.
        /// </summary>
        public Guid MinorUserId { get; set; }

        /// <summary>
        /// Gets or sets the supervising adult's user identifier.
        /// </summary>
        public Guid? SupervisingAdultUserId { get; set; }

        /// <summary>
        /// Gets or sets whether the parent approved participation.
        /// </summary>
        public bool ParentApproved { get; set; }

        /// <summary>
        /// Gets or sets when the parent approved participation.
        /// </summary>
        public DateTimeOffset? ParentApprovedDate { get; set; }

        // Navigation properties
        public virtual Event Event { get; set; }
        public virtual User MinorUser { get; set; }
        public virtual User SupervisingAdultUser { get; set; }
    }
}
```

**New Entity: UserConsent (ToS/Privacy versioning)**
```csharp
// New file: TrashMob.Models/UserConsent.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Records user consent for Terms of Service, Privacy Policy, and other agreements.
    /// </summary>
    public class UserConsent : KeyedModel
    {
        /// <summary>
        /// Gets or sets the user's identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of consent (ToS, Privacy, Parental).
        /// </summary>
        public string ConsentType { get; set; }

        /// <summary>
        /// Gets or sets the version of the document accepted.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets when the consent was accepted.
        /// </summary>
        public DateTimeOffset AcceptedDate { get; set; }

        /// <summary>
        /// Gets or sets the IP address at time of consent.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent at time of consent.
        /// </summary>
        public string UserAgent { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<User>(entity =>
{
    // Add to existing User configuration
    entity.Property(e => e.FirstName).HasMaxLength(100);
    entity.Property(e => e.LastName).HasMaxLength(100);
    entity.Property(e => e.ConsentStatus).HasMaxLength(50);
    entity.Property(e => e.PrivoUserId).HasMaxLength(100);

    entity.HasOne(e => e.ParentUser)
        .WithMany(p => p.Dependents)
        .HasForeignKey(e => e.ParentUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.IsMinor)
        .HasFilter("[IsMinor] = 1");
});

modelBuilder.Entity<ParentalConsent>(entity =>
{
    entity.Property(e => e.ParentEmail).HasMaxLength(256).IsRequired();
    entity.Property(e => e.PrivoConsentRequestId).HasMaxLength(100).IsRequired();
    entity.Property(e => e.PrivoConsentMethod).HasMaxLength(50);
    entity.Property(e => e.Status).HasMaxLength(50);
    entity.Property(e => e.RevokedReason).HasMaxLength(500);

    entity.HasOne(e => e.MinorUser)
        .WithMany()
        .HasForeignKey(e => e.MinorUserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.ParentUser)
        .WithMany()
        .HasForeignKey(e => e.ParentUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.MinorUserId);
    entity.HasIndex(e => e.Status);
});

modelBuilder.Entity<MinorEventParticipation>(entity =>
{
    entity.HasOne(e => e.Event)
        .WithMany()
        .HasForeignKey(e => e.EventId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.MinorUser)
        .WithMany()
        .HasForeignKey(e => e.MinorUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.SupervisingAdultUser)
        .WithMany()
        .HasForeignKey(e => e.SupervisingAdultUserId)
        .OnDelete(DeleteBehavior.NoAction);
});

modelBuilder.Entity<UserConsent>(entity =>
{
    entity.Property(e => e.ConsentType).HasMaxLength(50).IsRequired();
    entity.Property(e => e.Version).HasMaxLength(50).IsRequired();
    entity.Property(e => e.IPAddress).HasMaxLength(45);
    entity.Property(e => e.UserAgent).HasMaxLength(500);

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasIndex(e => new { e.UserId, e.ConsentType });
});
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

**Last Updated:** January 31, 2026
**Owner:** Product Lead + Legal + Engineering
**Status:** Planning in Progress
**Next Review:** After Privo contract finalization
