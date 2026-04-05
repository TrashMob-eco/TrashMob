# Project 23 — Parental Consent for Minors (via Privo.com)

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phases 0-3 Complete — PRIVO INT verified; Flow 3 child-initiated + production deployment remaining) |
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

The PRIVO/parental consent work is implemented as part of the [Project 1 — Auth Revamp](./Project_01_Auth_Revamp.md) phased rollout:

| Project 23 Phase | Implemented In | Requires PRIVO? | Description |
|-----------------|----------------|-----------------|-------------|
| Phase 0 (Parent-First Flow) ✅ | **Standalone** | **No** | Parent invites minor to create account, account linking via parent email, invitation system |
| Phase 1 (Age Verification) ✅ | **Standalone** | **Yes** | Age gate (web + mobile), PRIVO adult identity verification, Section 2/3 widget flow |
| Phase 2 (Parental Consent) ✅ | **Standalone** | **Yes** | PRIVO consent workflow (Flow 2: parent adds child), webhook processing, feature permissions, consent status tracking |
| Phase 3 (Minor Protections) ✅ | **Project 1, Phase 7** | **No** | Communication restrictions, limited profile visibility, adult presence enforcement, PRIVO feature permission gating |
| Phase 4 (Family Features) | **Deferred** | **No** | Parent dashboard, multiple minor management (future project) |

This combined approach minimizes risk by building PRIVO integration directly into the new Entra External ID auth system rather than retrofitting it into B2C.

---

## Objectives

### Primary Goals
- **Direct minor registration** with age verification via Privo.com
- **COPPA compliance** via Privo.com Verifiable Parental Consent (VPC)
- **Enhanced protections** (no DMs, limited profile visibility, adult presence requirements)
- **Document** the Entra External ID + PRIVO integration process (sponsorship deliverable)

### Secondary Goals
- Parent-managed dependents flow for families
- Minor activity reports for parents
- School/youth group integration
- Parental dashboard

---

## Scope

### Phase 1 — Age Verification & Adult Identity Verification ✅

**Completed April 2026.** Implemented as a standalone PRIVO integration (not via Custom Authentication Extension).

#### Layer 1: In-App Age Gate (Web + Mobile)
- [x] **Web (React):** AgeGateDialog with month/year dropdown DatePicker, DOB validation
- [x] **Mobile (MAUI):** AgeGateViewModel with DOB input, 13+ validation
- [x] Under-13 blocked with friendly message (COPPA: no PII collected)
- [x] 13-17 shows "Parental Consent Required" with option to navigate to child-signup flow
- [x] 18+ proceeds to standard Entra sign-up

#### Adult Identity Verification via PRIVO
- [x] `PrivoService` — HTTP client for all 10 PRIVO API sections, token caching (25-min), structured logging
- [x] `PrivoConsentManager` — orchestrates verification flow, stores `ParentalConsent` records
- [x] `PrivoConsentV2Controller` — `POST /v2/privo/verify` initiates Flow 1, redirects to PRIVO widget (Section 3)
- [x] `VerifyIdentityCard` — dashboard component shows verification status, "Verify My Identity" button
- [x] PRIVO callback page — handles redirect back from PRIVO verification widget
- [x] Webhook handler — receives PRIVO events, polls Section 7 for authoritative state, auto-updates verification
- [x] "Check Status" button — manual polling fallback when webhooks are delayed
- [x] User model: `IsIdentityVerified`, `IdentityVerifiedDate`, `PrivoSid` fields
- [x] Verified end-to-end on PRIVO INT environment (April 2026)

### Phase 2 — Parental Consent ✅

**Completed April 2026.** Full PRIVO consent workflow for 13-17 dependents.

#### Flow 2: Verified Parent Adds 13-17 Child
- [x] Parent adds dependent with optional email field
- [x] Shield button on 13+ dependents triggers PRIVO consent (Section 5)
- [x] PRIVO sends consent email to parent → parent approves on PRIVO site
- [x] Webhook fires → auto-polls PRIVO → consent status updates to "Approved"
- [x] Consent status badges: "Needs Consent", "Consent Pending", "Consent Approved"
- [x] After consent approved, Mail icon appears to send account invitation to child
- [x] Child creates Entra account → auto-linked to parent via `TryAcceptByEmailAsync`

#### Flow 3: Child-Initiated Signup (partially complete)
- [x] `/child-signup` page — child enters parent email, name, DOB
- [x] Backend: `InitiateChildConsentAsync` — checks parent exists, calls PRIVO Section 6
- [ ] End-to-end testing of child-initiated flow on INT

#### PRIVO Feature Permissions
- [x] `GET /v2/privo/permissions` — fetches PRIVO feature states by EID (Section 4), cached 1 hour
- [x] `usePrivoPermissions` React hook — `isFeatureEnabled(featureId)` for web gating
- [x] `IPrivoPermissionService` — mobile permission cache with `IsFeatureEnabled()` helper
- [x] Web: 7 gating points (leaderboards, newsletter, routes, sharing, teams, photos, registration)
- [x] Mobile: 5 ViewModels gated (ViewEvent, ViewTeam, Leaderboards, Newsletter, CreateLitterReport)
- [x] Mobile: pre-fetch permissions on login, clear cache on logout
- [x] Minor Account card — minors see restricted dashboard (no Add Dependent, no identity verification)

#### Consent Infrastructure
- [x] `ParentalConsent` entity with PRIVO identifiers, consent type/status enums
- [x] Webhook handler polls PRIVO on `consent_updated`/`account_feature_updated` events
- [x] Consent revocation endpoint (`POST /v2/privo/consent/{id}/revoke`)
- [x] 16 Playwright E2E tests covering API, UI, webhooks, callback page, age gate

### Phase 3 — Minor Protections (→ Project 1, Phase 7) ✅
- [x] Communication restrictions for minors (no direct messaging — no DM system exists; MessageRequestController is admin-only)
- [x] Limited profile visibility (first name + last initial via `ToDisplayUser()` name masking)
- [x] Adult presence enforcement at events (parent must be registered attendee; auto-unregister under-13 dependents on parent cancel)
- [x] Parent notification system (email sent when dependents registered for events)
- [x] Minor-specific UI indicators (Minor badges on web attendee table and mobile attendee list)
- [x] PRIVO feature permission gating (8 features gated on web + mobile based on parent-approved permissions)
- [x] Minor Account dashboard card (minors see restricted UI — no Add Dependent, no identity verification)
- [ ] Complete PRIVO documentation package (sponsorship deliverable — in progress)

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
| **False age claims** | Medium | High | PRIVO age verification; periodic re-verification |
| **Parent consent fraud** | Low | High | Privo VPC process; consent validation |
| **Minor safety incident** | Low | Critical | Adult presence requirement; clear policies; training |
| **Complex implementation** | High | Medium | Phased rollout; PRIVO expertise; thorough testing |

---

## Privo.com Integration

### Integration Architecture

The PRIVO integration uses a **Hybrid Age Gate (Option C)** — a two-layer approach combining an in-app pre-screen with Entra External ID Custom Authentication Extensions.

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

### PRIVO API Documentation

**Received March 24, 2026** — PRIVO delivered a 10-section API integration specification. See [Project 23 — PRIVO API Requirements](./Project_23_Privo_API_Requirements.md) for the complete reference including:
- Environment endpoints and credentials (INT + PROD)
- Feature and attribute identifiers
- All 10 API sections (token, consent requests, verification URLs, userinfo, revocation, webhooks)
- Webhook payload format and security requirements

### Privo Onboarding Requirements

**Before Privo can begin integration work, TrashMob must provide:**

| Requirement | Status | Owner |
|-------------|--------|-------|
| **Swim lane diagram** | ✅ Delivered | Engineering |
| **Feature set for parental consent** | ✅ Delivered | Product |
| **Business information** (name, address, website, privacy policy) | ✅ Delivered | Business |
| **Swimlane flow documentation** | ✅ Delivered | Engineering |
| **Branding assets** (logos, colors, etc.) | ❌ Pending | Marketing |
| **PRIVO API integration doc received** | ✅ Received March 24, 2026 | PRIVO |
| **Webhook endpoint configured (INT)** | ✅ Configured April 2026 | Engineering + PRIVO |
| **Redirect URLs whitelisted (INT)** | ✅ `dev.trashmob.eco` whitelisted April 2026 | Engineering + PRIVO |
| **Webhook API key exchanged (INT)** | ✅ Generated and shared April 2026 | Engineering |
| **INT environment verified end-to-end** | ✅ Adult verification + child consent April 2026 | Engineering |
| **Production credentials** | ❌ Pending PRIVO sign-off | PRIVO |

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

### Core Principle — REVISED (March 2026)

**Parent/guardian TrashMob accounts are required for minor participation.** The original design allowed Privo-only approval without a parent account, but this is insufficient for community-specific waiver workflows.

### Why Parent Accounts Are Required

**Community waiver constraint:** TrashMob's waiver system (Project 8) supports both Global and Community-scoped waivers. Communities (cities, counties, nonprofits) can require their own liability waivers, photo releases, or other legal documents. These community-specific waivers are only triggered when a minor registers for an event within that community's jurisdiction.

Privo has no mechanism to handle this because:
1. Privo doesn't know which community's waivers apply until the minor signs up for a specific event
2. Community waivers can change independently of the global platform waiver
3. New community waivers can be added at any time by community admins
4. There is no way to route a post-event-signup community waiver through Privo's VPC flow

This means the parent/guardian must have an authenticated TrashMob account to:
- Receive notification when their minor registers for an event requiring new waivers
- Review and sign community-specific waivers on behalf of the minor
- Re-sign when waiver versions change
- Participate in Event Check-In validation (Project 55)

### Registration Flows

Two registration paths lead to the same outcome: a minor User account linked to a parent User account via `ParentUserId`, with the parent's email stored for account matching.

#### Flow A: Parent-First (No Privo Required)

The parent already has a TrashMob account and initiates the process.

| Step | What Happens | Privo? |
|------|-------------|--------|
| 1. Parent adds dependent | Parent creates Dependent record (name, DOB, relationship) in My Dependents | No |
| 2. Parent invites minor | Parent clicks "Invite to create account" for a 13+ dependent; enters minor's email | No |
| 3. System sends invite | Secure token-based invite email sent to minor | No |
| 4. Minor creates account | Minor follows link → creates Entra External ID account | No |
| 5. System links accounts | Minor's User record linked to parent via `ParentUserId` and to Dependent via `DependentId` | No |
| 6. Parent signs waivers | Global + community waivers signed in-app on behalf of minor | No |
| 7. Minor registers for events | Community-specific waivers routed to parent in-app | No |

**This flow can be built today without Privo.** The trust chain is: authenticated parent → explicitly adds dependent → invites them. The gap is that without Privo, there's no verification the adult is actually the legal parent (self-attestation only).

#### Flow B: Minor-First (Requires PRIVO)

The minor initiates registration independently.

| Step | What Happens | Privo? |
|------|-------------|--------|
| 1. Minor age-gated at sign-up | DOB pre-screen blocks under-13; Privo verifies age (13-17) | **Yes** |
| 2. Minor provides parent's email | Stored on User record as `ParentEmail` for future linking | No |
| 3. Privo VPC verifies parent identity | Privo contacts parent via email; verifies identity (credit card, ID, video call) | **Yes** |
| 4. Minor account created (pending) | Account active but limited until parent completes setup | No |
| 5. Parent creates TrashMob account | Parent signs up using the same email the minor provided | No |
| 6. System auto-links accounts | System detects `ParentEmail` match → prompts parent to claim minor | No |
| 7. Parent signs waivers | Global + community waivers signed in-app on behalf of minor | No |
| 8. Minor registers for events | Community-specific waivers routed to parent in-app | No |

**This flow requires Privo** for age verification (step 1) and parent identity verification (step 3).

#### Account Linking via Parent Email

Both flows converge on the same linking mechanism. The parent's email is the key:

- **Flow A:** Parent's email is already known (they have an account). When minor accepts invite, `User.ParentUserId` is set directly.
- **Flow B:** Minor provides parent's email at sign-up. When a user later creates an account with that email, the system detects unlinked minors and prompts the parent to claim them.

```
Minor signs up → provides parent email → Privo VPC → minor account created (pending)
                                                            │
Parent creates account (matching email) ──────────────────►│
                                                            ▼
                                              System prompts: "A minor listed you
                                              as their parent. Claim this account?"
                                                            │
                                                   Parent confirms
                                                            │
                                                            ▼
                                              User.ParentUserId = parent.Id
                                              Dependent record auto-created
                                              Parent signs waivers
```

**Security considerations for auto-linking:**
- Parent must explicitly confirm the link (no silent auto-linking)
- Minor is notified when a parent claims their account
- Only one parent can be linked at a time (primary guardian)
- Parent email match is necessary but not sufficient — confirmation required from both sides

### Capability Matrix (Revised — Account Required)

| Capability | How It Works |
|------------|-------------|
| **Minor Registration** | Parent account required; Privo verifies parent identity |
| **Consent Approval** | Privo VPC for initial platform consent |
| **Consent Revocation** | Via Privo OR TrashMob app — immediate account suspension |
| **Global Waiver Signing** | In-app, from parent's My Dependents page |
| **Community Waiver Signing** | In-app notification when minor registers for event in new community |
| **Event Registration Notifications** | Email + in-app push notifications |
| **View Minor's Activity** | Parent dashboard |
| **Manage Multiple Minors** | Single dashboard for all dependents |
| **Event Check-In (Project 55)** | Parent confirms attendance; system validates waiver compliance |
| **Approve Event Participation** | In-app approval (Phase 4) |

### Non-Parent Guardian Scenarios

Not all adults bringing minors to events are their legal parents. Common scenarios:

| Scenario | Guardian Type | Challenges |
|----------|-------------|------------|
| **Scout troop cleanup** | Scout master / troop leader | 15+ kids, each with different parents. Scout master has organizational authority but not legal custody. |
| **School field trip** | Teacher / chaperone | School permission slips exist but are separate from TrashMob waivers. Teacher can't sign legal waivers. |
| **Church youth group** | Youth pastor / volunteer leader | Similar to scouts — organizational authority, not legal custody. |
| **Sports team** | Coach / assistant coach | Coach may have general parental permission forms but can't sign liability waivers. |
| **After-school program** | Program director | May have blanket parental consent for activities but not specific to TrashMob. |
| **Grandparent / family friend** | Informal guardian | May bring kids regularly but isn't the legal parent. |
| **Foster care** | Foster parent / caseworker | Legal authority varies by jurisdiction and court order. |
| **Divorced/separated families** | Non-custodial parent | May bring child to events but custody agreement may limit consent authority. |

**Key constraint:** Only a legal parent/guardian can sign waivers on behalf of a minor. A coach or teacher bringing a group of kids cannot sign waivers for children who are not their own dependents.

**Possible approaches for group scenarios:**

1. **Each parent signs in advance:** Group leader tells parents to create accounts and sign waivers before the event. Leader brings kids but doesn't sign anything.
2. **Paper waiver fallback:** Group leader brings paper waivers pre-signed by parents (existing Project 8 feature — `PaperWaiverUploadRequest`). Event lead uploads them.
3. **Delegated authority role (future):** A new "Group Leader" concept where a parent explicitly authorizes a specific adult (coach, teacher) to register and check in their child for events. Would need legal review.
4. **Organizational bulk consent (future):** Partner organizations (scout troops, schools) establish a relationship with TrashMob where the organization's existing consent framework is accepted. Complex legally.

> **Decision needed:** For v1, approach #1 (each parent signs individually) with #2 (paper fallback) is the safest. Approaches #3 and #4 require legal review and are deferred.

```csharp
// Account linking logic — triggered when a new user signs up
// Checks if any minors listed this email as their parent's email
public async Task<List<User>> FindUnlinkedMinorsAsync(string parentEmail, CancellationToken cancellationToken)
{
    return await _context.Users
        .Where(u => u.IsMinor &&
                    u.ParentUserId == null &&
                    u.ParentEmail == parentEmail)
        .ToListAsync(cancellationToken);
}

// Parent explicitly confirms the link (not auto-linked for security)
public async Task LinkParentToMinorAsync(Guid parentUserId, Guid minorUserId, CancellationToken cancellationToken)
{
    var minor = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == minorUserId && u.IsMinor && u.ParentUserId == null, cancellationToken);

    if (minor == null) return;

    minor.ParentUserId = parentUserId;

    // Auto-create Dependent record if one doesn't exist
    var existingDependent = await _context.Dependents
        .FirstOrDefaultAsync(d => d.ParentUserId == parentUserId &&
                                   d.FirstName == minor.FirstName &&
                                   d.DateOfBirth == minor.DateOfBirth, cancellationToken);

    if (existingDependent == null)
    {
        var dependent = new Dependent
        {
            ParentUserId = parentUserId,
            FirstName = minor.FirstName,
            LastName = minor.LastName,
            DateOfBirth = minor.DateOfBirth ?? DateOnly.MinValue,
            Relationship = "parent",
            IsActive = true,
        };
        _context.Dependents.Add(dependent);
        minor.DependentId = dependent.Id;
    }
    else
    {
        minor.DependentId = existingDependent.Id;
    }

    await _context.SaveChangesAsync(cancellationToken);
}
```

### Notifications to Parents

**Parents with accounts** receive notifications via email + in-app:

| Notification | Delivery Method |
|--------------|-----------------|
| Minor registered for event | Email + in-app |
| Community waiver required | Email + in-app (with sign action) |
| Event reminder (day before) | Email + in-app |
| Event completed | Email + in-app |
| Consent expiring (annual) | Email via Privo |

**Parents without accounts** (minor-first flow, before parent signs up):

| Notification | Delivery Method |
|--------------|-----------------|
| Your child listed you as parent | Email (with account creation link) |
| Privo VPC consent request | Email via Privo |
| Consent expiring (annual) | Email via Privo |

---

## Implementation Phases

Phases are restructured to separate Privo-independent work (can start now) from Privo-dependent work (blocked on Privo API).

### Phase 0: Parent-First Flow & Account Linking (No Privo Required)

**Can be implemented immediately.** Builds the infrastructure that both Flow A and Flow B will use.

#### Data Model ✅
- [x] Add `IsMinor` (bool), `ParentUserId` (Guid?), `ParentEmail` (string?) to User model
- [x] Add `DependentId` (Guid?) to User model — links minor's User to their Dependent record
- [x] Create `DependentInvitation` entity (token, expiry, status, DependentId, ParentUserId)
- [x] EF migration for all schema changes
- [x] DbContext configuration (indexes, FK relationships, `User.ParentUser` navigation)

#### Backend ✅
- [x] `DependentInvitationManager` — token generation (cryptographic, 32+ byte), expiry enforcement, invitation CRUD
- [x] `DependentInvitationsController` — invite creation, token verification, acceptance
- [x] Account linking service — match parent email, prompt for confirmation, link `User.ParentUserId`
- [x] Minor flag logic — set `IsMinor = true` based on Dependent.DateOfBirth (13-17)
- [x] Email template for invite notification

#### Web (React) ✅
- [x] "Invite to create account" button on My Dependents card for 13+ dependents
- [x] Invitation status display (Pending, Accepted, Expired)
- [x] Resend / cancel invitation actions
- [x] Accept-invite page (minor follows link → guided account creation)
- [x] Auto-link prompt when parent creates account and unlinked minors exist

#### Mobile (MAUI) ✅
- [x] Invite flow from My Dependents page
- [x] Invitation status display

### Phase 1: Age Gate (→ Project 1, Phase 3 — Requires PRIVO)
- In-app DOB pre-screen on **both web (React) and mobile (MAUI)** — blocks under-13s before Entra redirect (no PII collected)
- Custom Authentication Extension (Azure Function) on `OnAttributeCollectionSubmit` — server-side defense-in-depth
- Privo API integration for age verification (13-17 triggers minor flow)
- Under-13 blocking at both layers
- Minor flag in database

### Phase 2: Parental Consent (→ Project 1, Phase 3 — Requires PRIVO)
- Privo VPC workflow implementation
- Parent notification via Privo
- Consent status tracking (ParentalConsent entity)
- Pending account limitations
- Privo webhook for consent status updates
- Minor-first flow: minor provides parent email → Privo VPC → account linking when parent signs up

### Phase 3: Minor Protections (No Privo Required) ✅

**Complete.** These protections apply to all minor accounts regardless of how they were created.

- [x] Communication restrictions (no DMs for minors — no DM system exists; MessageRequestController is admin-only)
- [x] Profile visibility limits (first name + last initial via `ToDisplayUser()` and leaderboard cache masking)
- [x] Adult presence enforcement at events (parent must be registered attendee; auto-unregister under-13 dependents on parent cancel)
- [x] Parent notification system (email sent when dependents registered for events)
- [x] Minor-specific UI indicators ("Minor" badge on web event attendee table and mobile attendee list; `IsMinor` in `DisplayUser` DTO)

### Phase 4: Family Features (Deferred)
- Parent dashboard — view minor's events, stats, activity (future project)
- Multiple minor management (future project)
- Event approval workflow (future project)
- Activity reporting (future project)

**Note:** Legal sign-off required before Phase 1 deployment (Privo). Phase 0 and Phase 3 can proceed with product/legal review but don't require Privo contract or API.

### Sponsorship Documentation Deliverables

The following documentation must be produced as part of the Privo sponsorship agreement:

1. **Entra External ID tenant setup guide** — How to create and configure an external tenant for PRIVO integration (Project 1, Phase 0)
2. **Custom Authentication Extension guide** — How to build the Azure Function and wire it to the sign-up user flow (Phase 1)
3. **Privo VPC integration guide** — How to implement the Verifiable Parental Consent webhook and consent tracking (Phase 2)
4. **Complete integration package** — End-to-end guide combining all of the above (Phase 3)

---

## Privo Value Assessment (March 2026)

With the decision to require parent accounts for community waiver workflows, Privo's role narrows but remains important. Here's what Privo provides that TrashMob accounts alone cannot:

### What Privo Provides

| Capability | Without Privo (Account Only) | With Privo |
|------------|------------------------------|------------|
| **Age verification** | Self-reported DOB — a 10-year-old can claim to be 13 | Privo verifies age through authoritative methods |
| **Parent identity verification** | Self-attestation — the minor could create a fake "parent" account and approve themselves | Privo VPC uses FTC-approved methods (credit card, government ID, video call) to verify the adult is actually the parent |
| **COPPA safe harbor** | TrashMob bears full legal liability for any COPPA violation | Privo is an FTC-approved COPPA safe harbor program — using Privo provides legal protection if there's ever a COPPA complaint |
| **Consent artifact retention** | TrashMob must build and maintain its own legally compliant audit trail | Privo maintains consent artifacts per COPPA requirements |
| **Consent revocation** | Must build own revocation flow | Privo provides standardized revocation with legal compliance |
| **Annual re-verification** | Must build own re-verification reminder system | Privo handles re-verification timing automatically |

### Privo's Remaining Value — Is It Worth It?

**Strong case for keeping Privo:**

1. **Identity verification is the critical gap.** A TrashMob account proves someone *has an account*, not that they *are the parent*. Without Privo, a 15-year-old could create a parent account with a disposable email and approve their own participation. This is exactly the scenario COPPA is designed to prevent.

2. **COPPA safe harbor is significant legal protection.** If a parent files a COPPA complaint with the FTC, Privo's safe harbor status means TrashMob followed an FTC-approved process. Without it, TrashMob would need to defend its own verification methods — expensive and risky for a small nonprofit.

3. **Liability reduction for communities.** Cities and counties partnering with TrashMob will want assurance that minor participation is legally compliant. "We use an FTC-approved COPPA safe harbor provider" is a much stronger answer than "parents check a box on our website."

**Case for not using Privo:**

1. **Cost and complexity.** Even with the sponsorship, integrating Privo adds engineering complexity and an external dependency.
2. **The dependent model already works.** Adults already add dependents and sign waivers for them. The self-attestation model is standard for most volunteer organizations.
3. **Most youth volunteer programs don't use COPPA-grade verification.** Park cleanups, Habitat for Humanity, etc. typically just collect a paper permission slip.

### Recommendation

**Keep Privo for initial registration (age + parent identity verification) but handle all ongoing waiver management through TrashMob parent accounts.** This gives you:

- FTC-approved identity verification at the front door (Privo)
- Full control over community waiver workflows after that (TrashMob)
- COPPA safe harbor legal protection
- No dependency on Privo for day-to-day operations (waivers, event registration, check-in)

Privo is a one-time gate at registration. Everything after that flows through the parent's TrashMob account.

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

5. **Are parent accounts required?**
   **Decision:** Yes — community-specific waivers cannot be routed through Privo's VPC flow. Parent/guardian must have a TrashMob account to sign community waivers when their minor registers for events. (March 2026)

6. **Can we support both parent-first and minor-first registration?**
   **Decision:** Yes. Both flows use parent email as the linking key. Flow A (parent-first): parent invites minor — no Privo needed. Flow B (minor-first): minor provides parent email at sign-up, Privo verifies parent identity, accounts linked when parent creates account. Phase 0 implements Flow A infrastructure; Phases 1-2 add Flow B via Privo. (March 2026)

## Open Questions

1. ~~**Privo.com cost structure and timeline?**~~
   **Decision:** Privo is sponsoring the integration — TrashMob is the first Entra External ID + Privo partner. Cost covered by sponsorship agreement.
   **Status:** ✅ Resolved (February 2026)

2. ~~**When will Privo provide their API documentation and test environment?**~~
   **Decision:** PRIVO delivered 10-section API integration doc on March 24, 2026. INT environment credentials included. See [PRIVO API Requirements](./Project_23_Privo_API_Requirements.md).
   **Status:** ✅ Resolved (March 2026)

3. **How should group leaders (coaches, scout masters, teachers) handle minor participation?**
   Group leaders cannot sign waivers for children who are not their legal dependents. For v1, each parent must sign individually with paper waiver fallback. Future: consider delegated authority role where a parent explicitly authorizes a specific adult for their child. Requires legal review.
   **Status:** Deferred — not needed for v1
   **Owner:** Product + Legal

4. **How do we handle divorced/separated families and custody disputes?**
   If both parents have accounts, who has authority to sign/revoke waivers? Should we accept either parent's signature, or only the custodial parent? What if one parent revokes consent the other parent granted?
   **Status:** Open
   **Owner:** Legal

5. **Should foster parents and non-traditional guardians be able to sign waivers?**
   The `Dependent.Relationship` field supports "Guardian" but legal authority varies by jurisdiction. Foster parents may have limited consent authority depending on court orders. Need legal guidance on what documentation (if any) to require for non-parent guardians.
   **Status:** Open
   **Owner:** Legal

6. ~~**What happens when a parent account is required but the parent refuses to create one?**~~
   **Decision:** Minor cannot participate. Parent account and PRIVO verification are required.
   **Status:** ✅ Resolved (March 2026)

7. ~~**Should Privo identity verification be required for all parent accounts, or only for parents of minors?**~~
   **Decision:** Yes — parent must verify through PRIVO when approving any dependent aged 13-17. The current "add a dependent" flow only handles under-13 (no account needed). The 13-17 flow requires PRIVO verification and will be a distinct path. Need to distinguish between under-13 dependents (no PRIVO, no account) and 13-17 dependents (PRIVO consent required, child gets their own account).
   **Status:** ✅ Resolved (March 2026)

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

**Last Updated:** March 7, 2026
**Owner:** Product Lead + Legal + Engineering
**Status:** In Progress — Phase 0 (parent-first flow) and Phase 3 (minor protections) complete. Privo API integration (Phases 1-2) blocked on Privo onboarding.
**Next Review:** Privo API documentation availability; Phase 1 age verification implementation
