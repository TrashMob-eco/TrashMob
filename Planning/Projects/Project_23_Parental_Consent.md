# Project 23 — Parental Consent for Minors (via Privo.com)

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in Progress (combined with Project 1; auth migration complete, Privo integration remaining) |
| **Priority** | High |
| **Risk** | High |
| **Size** | Large |
| **Dependencies** | Project 1 (Auth Revamp — Phases 3 & 7), Legal Review, Privo.com Contract |

---

## Business Rationale

Support parent-managed dependents or direct minor registration with age verification and protections. Expanding to minors (13+) significantly increases the volunteer base while maintaining safety and legal compliance.

**Privo Sponsorship:** TrashMob is the first organization to integrate Microsoft Entra External ID with Privo.com. Privo is sponsoring the integration and asking TrashMob to generate documentation on the process for sharing with their other customers. This documentation is a key deliverable alongside the technical integration.

---

## Combined Approach with Project 1

The Privo/parental consent work is implemented as part of the [Project 1 — Auth Revamp](./Project_01_Auth_Revamp.md) phased rollout:

| Project 23 Phase | Implemented In | Description |
|-----------------|----------------|-------------|
| Phase 1 (Age Verification) | **Project 1, Phase 3** | Custom Authentication Extension for age gate via Privo API |
| Phase 2 (Parental Consent) | **Project 1, Phase 3** | Privo VPC workflow, consent tracking, pending account limitations |
| Phase 3 (Minor Protections) | **Project 1, Phase 7** | Communication restrictions, limited profile visibility, adult presence enforcement |
| Phase 4 (Family Features) | **Deferred** | Parent dashboard, multiple minor management (future project) |

This combined approach minimizes risk by building Privo integration directly into the new Entra External ID auth system rather than retrofitting it into B2C.

---

## Objectives

### Primary Goals
- **Direct minor registration** with age verification via Privo.com
- **COPPA compliance** via Privo.com Verifiable Parental Consent (VPC)
- **Enhanced protections** (no DMs, limited profile visibility, adult presence requirements)
- **Document** the Entra External ID + Privo integration process (sponsorship deliverable)

### Secondary Goals
- Parent-managed dependents flow for families
- Minor activity reports for parents
- School/youth group integration
- Parental dashboard

---

## Scope

### Phase 1 — Age Verification (→ Project 1, Phase 3)

**Architecture: Hybrid Age Gate (two-layer verification)**

#### Layer 1: In-App Pre-Screen (Web + Mobile, before Entra redirect)
- [ ] **Web (React):** DOB input component shown when user clicks "Sign Up" — before `loginRedirect()`
- [ ] **Mobile (MAUI):** DOB input page/modal shown before `AcquireTokenInteractive()` in `AuthService`
- [ ] Under-13 blocked immediately with friendly message (COPPA: no PII collected from children)
- [ ] 13-17 flagged as minor, DOB passed to Entra sign-up via MSAL `extraQueryParameters` or `state`
- [ ] 18+ proceeds to standard Entra sign-up

#### Layer 2: Custom Authentication Extension (Azure Function, server-side)
- [ ] `OnAttributeCollectionSubmit` re-verifies DOB (defense-in-depth — can't be bypassed)
- [ ] Integrates with Privo API for age verification
- [ ] Under-13 → `showBlockPage`, 13-17 → set `isMinor` flag, 18+ → continue
- [ ] Document Custom Authentication Extension setup (sponsorship deliverable)

#### Pre-requisite: Verify Token Claims & Profile Completeness
See **Project 1, Phase 3 Investigation** for full task list. Key questions:
- Are `given_name`/`family_name` optional claims working for all sign-in methods?
- How are existing users without `DateOfBirth` handled? (Plan: grandfather as adults)
- Should profile page prompt for missing fields?

### Phase 2 — Parental Consent (→ Project 1, Phase 3)
- [ ] Implement Privo VPC (Verifiable Parental Consent) webhook
- [ ] Parent notification workflow via Privo
- [ ] Consent status tracking in database (ParentalConsent entity)
- [ ] Pending account limitations for minors awaiting consent
- [ ] Consent artifact retention per COPPA requirements
- [ ] Document Privo VPC integration (sponsorship deliverable)

### Phase 3 — Minor Protections (→ Project 1, Phase 7)
- [ ] Communication restrictions for minors (no direct messaging)
- [ ] Limited profile visibility (first name + last initial)
- [ ] Adult presence enforcement at events
- [ ] Parent notification system
- [ ] Complete Privo documentation package (sponsorship deliverable)

### Phase 4 — Family Features (Deferred)
- Parent can manage multiple minors (future)
- Family registration flow (future)
- Parent approval for event registration (future)
- Parent notification of event participation (future)

> **Note:** Phase 4 deferred to a future project. Focus on Phases 1-3 for initial minor support.

---

## Out-of-Scope

- Under-13 registration (COPPA restriction)
- Minor-only events
- School/organization bulk registration
- Payment processing for families

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
- ~~**Project 1 (Auth Revamp):** Integration with Entra External ID~~ ✅ Complete — Entra External ID live on production (Feb 22, 2026)
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

### Integration Architecture

The Privo integration uses a **Hybrid Age Gate (Option C)** — a two-layer approach combining an in-app pre-screen with Entra External ID Custom Authentication Extensions.

**Layer 1 (In-App Pre-Screen):** React DOB input shown before `loginRedirect()`. Blocks under-13s immediately without collecting any PII (COPPA compliance). Passes DOB context to Entra via MSAL `extraQueryParameters` or `state`.

**Layer 2 (Custom Authentication Extension):** `OnAttributeCollectionSubmit` event in the sign-up user flow triggers an Azure Function that re-verifies DOB and calls the Privo API. This server-side layer provides defense-in-depth — users cannot bypass age verification by navigating directly to the Entra sign-up URL.

This replaces the B2C IEF custom policy approach and is the first-ever implementation of Entra External ID + Privo.

### Services Used

**Age Verification:**
- Age gate with Privo verification (via Custom Authentication Extension)
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
// Custom Authentication Extension (Azure Function)
// Triggered by OnAttributeCollectionSubmit during sign-up
public class AgeVerificationExtension
{
    // Receives: birthDate, email from sign-up form
    // Calls: Privo API for age verification
    // Returns: ContinueWithDefaultBehavior (18+),
    //          ShowBlockPage (under 13),
    //          ModifyAttributeValues (13-17, set IsMinor flag)
}

// Privo service integration (backend)
public interface IPrivoService
{
    Task<AgeVerificationResult> VerifyAgeAsync(string birthDate, Guid userId);
    Task<ConsentRequest> RequestParentalConsentAsync(MinorConsentRequest request);
    Task<ConsentStatus> GetConsentStatusAsync(Guid consentRequestId);
    Task<bool> ValidateConsentAsync(Guid userId);
    Task RevokeConsentAsync(Guid userId, string reason);
}
```

### Privo Onboarding Requirements

**Before Privo can begin integration work, TrashMob must provide:**

| Requirement | Status | Owner |
|-------------|--------|-------|
| **Swim lane diagram** | ❌ Pending | Engineering |
| **Feature set for parental consent** | ❌ Pending | Product |
| **Business information** (name, address, website, privacy policy) | ❌ Pending | Business |
| **Swimlane flow documentation** | ❌ Pending | Engineering |
| **Branding assets** (logos, colors, etc.) | ❌ Pending | Marketing |

**Features requiring parental consent:**

*Core features:*
- Newsletter subscriptions
- In-app notifications
- Event sign-up
- Instant messaging (blocked for minors per Phase 3)
- Geolocation sharing
- Create an event
- Photo uploads
- Profile photo
- Join a team

*Location & tracking (sensitive):*
- Route tracing during events (Project 15)
- Litter report submission (includes location)

*Public visibility:*
- Appear on public leaderboards (Project 20)
- Individual attendee metrics visible (Project 22)
- Social media sharing with name/info (Project 14)

*Communication:*
- Contact information shared with event leads

*Legal:*
- Waiver signing (parent signs on behalf of minor - Project 8)

**Integration data flows:**
- Send user location to Privo after sign-up
- Send parental waiver to parent on signup

**Figma access required for Privo team:**
*(Contact addresses stored in secure location - not in public repo)*

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

### Registration Flow (Hybrid Age Gate — Option C)

```
User clicks "Sign Up"
         │
         ▼
┌─────────────────────┐
│ LAYER 1: In-App     │  ← React UI (before Entra redirect)
│ DOB Pre-Screen      │     No PII collected yet
└────────┬────────────┘
         │
    ┌────┴────┬────────┐
    ▼         ▼        ▼
 Under 13   13-17    18+
    │         │        │
    ▼         │        │
 BLOCKED      │        │
 (friendly    ▼        ▼
  message)  ┌─────────────────────┐
            │ Entra Sign-Up Flow  │  ← Redirect to Entra External ID
            │ (email, name, DOB,  │     Collects all attributes
            │  password/social)   │
            └────────┬────────────┘
                     │
            ┌────────────────────┐
            │ LAYER 2: Custom    │  ← Azure Function (server-side)
            │ Auth Extension     │     OnAttributeCollectionSubmit
            │ (re-verify DOB +   │     Defense-in-depth
            │  Privo API call)   │
            └────────┬───────────┘
                     │
                ┌────┴────┐
                ▼         ▼
             13-17      18+
                │         │
                ▼         ▼
           Set isMinor  Standard
           flag         Registration
                │         │
                ▼         ▼
         ┌──────────┐  Account
         │ Post-Reg │  Active
         │ Privo    │
         │ VPC Flow │
         └────┬─────┘
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

**Why two layers:**
1. **Layer 1 (in-app)** — Best UX: blocks under-13s instantly before any PII is collected (COPPA compliance). No wasted form-filling.
2. **Layer 2 (server-side)** — Security: prevents bypass (e.g., direct URL to Entra sign-up, API manipulation). Privo API call provides authoritative age verification.

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
   - At least one adult must be present at any event with minors (no specific ratio)
   - Parent notification of registration
   - Parent approval for events: deferred to Phase 4 (future)

4. **Data Retention:**
   - Consent artifacts per legal requirements
   - Regular consent re-verification
   - Right to deletion (with parent request)

---

## Parent Account Requirements

### Core Principle
**Privo approval is sufficient for minor registration.** Parents do NOT need a TrashMob account to approve their child's participation. However, parents who create accounts unlock additional features.

### Registration Scenarios

| Scenario | Flow | Minor Status |
|----------|------|--------------|
| **Parent has no TrashMob account** | Minor registers → Parent approves via Privo email → Minor active | ✅ Fully functional |
| **Parent creates account later** | Minor already active → Parent registers → Links to minor via email match | ✅ Enhanced features unlocked |
| **Parent registers first** | Parent creates account → Adds minor as dependent → Minor registers with parent's email | ✅ Streamlined flow |
| **Minor registers before parent approves** | Minor in "Pending" status → Limited access until Privo approval | ⏳ Waiting for approval |

### Capability Matrix: Parent Account Requirements

| Capability | No Parent Account | With Parent Account |
|------------|-------------------|---------------------|
| **Minor Registration** | ✅ Privo email approval sufficient | ✅ Can initiate from parent dashboard |
| **Consent Approval** | ✅ Via Privo interface | ✅ Via Privo interface |
| **Consent Revocation** | ✅ Via Privo interface | ✅ Via Privo OR TrashMob app |
| **Event Registration Notifications** | ✅ Email only | ✅ Email + in-app notifications |
| **View Minor's Activity** | ❌ Not available | ✅ Parent dashboard |
| **View Minor's Event History** | ❌ Not available | ✅ Parent dashboard |
| **View Minor's Metrics/Stats** | ❌ Not available | ✅ Parent dashboard |
| **Approve Event Participation** | ❌ Not available (Phase 4) | ✅ In-app approval (Phase 4) |
| **Manage Multiple Minors** | ❌ Not available | ✅ Single dashboard for all dependents |
| **Update Minor's Profile** | ❌ Not available | ✅ Can edit on behalf of minor |
| **Delete Minor's Account** | ✅ Via Privo (revoke consent) | ✅ Via Privo OR TrashMob app |
| **Receive Minor's Achievements** | ❌ Not available | ✅ Notified of badges earned |
| **Sign Waiver for Minor** | ✅ Via email link | ✅ Via TrashMob app |

### Account Linking

When a parent creates a TrashMob account after their minor is already registered:

1. **Email Match:** System detects parent email matches a minor's `ParentEmail` field
2. **Verification:** Parent confirms they are the parent of the listed minor(s)
3. **Linking:** `User.ParentUserId` set on minor's account
4. **Dashboard:** Parent dashboard shows linked minor(s)

```csharp
// Account linking logic
public async Task LinkParentToMinors(Guid parentUserId, string parentEmail)
{
    var unlinkedMinors = await _context.Users
        .Where(u => u.IsMinor &&
                    u.ParentUserId == null &&
                    u.ParentalConsents.Any(pc => pc.ParentEmail == parentEmail &&
                                                  pc.Status == "Verified"))
        .ToListAsync();

    foreach (var minor in unlinkedMinors)
    {
        minor.ParentUserId = parentUserId;
    }
}
```

### Notifications to Non-Account Parents

Parents without TrashMob accounts still receive critical notifications via email:

| Notification | Delivery Method |
|--------------|-----------------|
| Minor registered for event | Email |
| Event reminder (day before) | Email |
| Event completed | Email |
| Consent expiring (annual) | Email via Privo |
| Waiver required | Email with sign link |

---

## Implementation Phases

> **Note:** Phases 1-2 are implemented as Project 1 Phase 3, and Phase 3 is implemented as Project 1 Phase 7. See [Project 1 — Auth Revamp](./Project_01_Auth_Revamp.md) for the full implementation timeline.

### Phase 1: Age Gate (→ Project 1, Phase 3)
- In-app DOB pre-screen on **both web (React) and mobile (MAUI)** — blocks under-13s before Entra redirect (no PII collected)
- Custom Authentication Extension (Azure Function) on `OnAttributeCollectionSubmit` — server-side defense-in-depth
- Privo API integration for age verification (13-17 triggers minor flow)
- Under-13 blocking at both layers
- Minor flag in database

### Phase 2: Parental Consent (→ Project 1, Phase 3)
- Privo VPC workflow implementation
- Parent notification via Privo
- Consent status tracking (ParentalConsent entity)
- Pending account limitations
- Privo webhook for consent status updates

### Phase 3: Minor Protections (→ Project 1, Phase 7)
- Communication restrictions (no DMs for minors)
- Profile visibility limits (first name + last initial)
- Adult presence enforcement at events
- Parent notification system

### Phase 4: Family Features (Deferred)
- Parent dashboard (future project)
- Multiple minor management (future project)
- Event approval workflow (future project)
- Activity reporting (future project)

**Note:** Legal sign-off required before Phase 1 deployment. Phase 4 deferred to focus on core minor safety features first.

### Sponsorship Documentation Deliverables

The following documentation must be produced as part of the Privo sponsorship agreement:

1. **Entra External ID tenant setup guide** — How to create and configure an external tenant for Privo integration (Project 1, Phase 0)
2. **Custom Authentication Extension guide** — How to build the Azure Function and wire it to the sign-up user flow (Phase 1)
3. **Privo VPC integration guide** — How to implement the Verifiable Parental Consent webhook and consent tracking (Phase 2)
4. **Complete integration package** — End-to-end guide combining all of the above (Phase 3)

---

## Resolved Questions

1. **Consent re-verification frequency?**
   **Decision:** Annual re-verification; Privo handles timing automatically

2. **Minimum adult-to-minor ratio at events?**
   **Decision:** No specific ratio enforcement; require at least 1 adult present at any event with minors

3. **What if parent revokes consent?**
   **Decision:** Immediate account suspension; data retained per legal requirements (COPPA compliance)

4. **School/organization bulk consent?**
   **Decision:** Out of scope for v1; evaluate demand in future

## Open Questions

1. ~~**Privo.com cost structure and timeline?**~~
   **Decision:** Privo is sponsoring the integration — TrashMob is the first Entra External ID + Privo partner. Cost covered by sponsorship agreement.
   **Status:** ✅ Resolved (February 2026)

2. **When will Privo provide their API documentation and test environment?**
   **Status:** Pending — awaiting onboarding requirements completion
   **Owner:** Engineering + Privo
   **Due:** Before Project 1 Phase 3

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#956](https://github.com/trashmob/TrashMob/issues/956)** - Project 23: Parental Consent for Minors (tracking issue)

---

## Related Documents

- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** - Auth migration (Phases 3 & 7 implement this project's work)
- **[Privo Integration Package](./Project_23_Privo_Integration_Package.md)** - Document to send to Privo
- **[Project 8 - Waivers V3](./Project_08_Waivers_V3.md)** - Minor waiver handling
- **[Privo.com](https://privo.com)** - Vendor documentation

---

**Last Updated:** February 22, 2026
**Owner:** Product Lead + Legal + Engineering
**Status:** Planning in Progress — Auth migration complete (Project 1 Phases 0-5 live on production). Privo API integration and parental consent workflow remaining.
**Next Review:** After Privo API documentation and test environment available
