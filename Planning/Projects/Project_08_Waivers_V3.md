# Project 8 � Liability Waivers V3

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phases 1-4, 6 Complete; Phase 5 Ready for Development) |
| **Priority** | High |
| **Risk** | Very High |
| **Size** | Very Large |
| **Dependencies** | None (Project 23 only needed for self-registered minor accounts, not dependent waivers) |

---

## Business Rationale

Support flexible waiver model allowing both TrashMob default waivers and community-specific waivers with validity periods, minors coverage, admin visibility, and print/export capabilities. Current waiver system is rigid and doesn't support community customization or proper legal workflows.

**Critical:** Legal review and sign-off required before any development begins.

---

## Objectives

### Primary Goals
- **Upload and manage waivers** with effective/expiry dates
- **List and print waivers** by user, event, and community
- **Event-time waiver checks** to ensure current valid waivers
- **Minor/guardian workflows** for parental consent
- **Audit trail** for all waiver acceptances

### Secondary Goals
- Bulk waiver export for legal purposes
- Waiver version history
- Multi-language waiver support
- Digital signature integration

---

## Scope

### Waiver Signing Methods

Three methods of signing waivers must be supported:

1. **E-signature on website** - Click-to-accept during event registration flow
2. **E-signature on mobile app** - Same click-to-accept flow within the mobile application
3. **Manual paper upload** - Physically signed waivers uploaded as images or PDFs by:
   - TrashMob staff
   - Community Manager
   - Team Leader
   - Event Lead

### E-Signature Implementation (Click-to-Accept)

Native implementation using click-to-accept pattern (no third-party services like DocuSign):

**User Flow:**
1. Display full waiver text (scrollable, must scroll to bottom)
2. Checkbox: "I have read and agree to the above waiver"
3. Typed full legal name field
4. "Sign Waiver" button (disabled until checkbox checked and name entered)

**Audit Trail Captured:**
- Authenticated user ID
- Typed legal name
- Timestamp (UTC)
- IP address
- User agent (browser/device info)
- Waiver version ID

**Legal Validity:** Click-to-accept with strong audit trail is legally valid under ESIGN Act and UETA for liability waivers.

### Document Storage Requirements

- **Immutable storage** - Signed waivers must be stored immutably (no modifications after signing)
- **Waiver text snapshot** - Store the exact waiver text at time of signing (not just version ID)
- **Viewable by authorized parties:**
  - The person who signed the waiver
  - Community Lead (for community events)
  - Team Lead (for team events)
  - TrashMob staff (all waivers)

### PDF Download Requirement

Users must be able to download their signed waivers as PDF documents containing:

1. **Original waiver text** - The exact version they signed (snapshot, not current version)
2. **Signature details:**
   - Signer's typed legal name
   - Date and time signed (user's local timezone + UTC)
   - For paper uploads: scanned signature image
3. **Audit trail:**
   - IP address at signing
   - Device/browser information
   - Waiver version identifier
4. **TrashMob branding** - Logo, document title, legal footer

**Implementation:** Server-side PDF generation (not client-side) to ensure consistency and prevent tampering. Store generated PDF in immutable blob storage.

### Waiver Validity Rules

- Waivers are valid for a **calendar year** (expires December 31)
- Users must re-sign at their first event registration in a new calendar year
- Waivers can be uploaded and scheduled for future activation date
- A new waiver version invalidates previous signatures (re-consent required)
- Users without a current valid waiver for an event **must** sign before attending

### Waiver Requirements Per Event

| Event Context | Required Waivers |
|---------------|------------------|
| Event in community **with** custom waiver | TrashMob waiver + Community waiver (both required) |
| Event in community **without** custom waiver | TrashMob waiver only |

**Community Waiver Matching:** An event is considered "in a community" when the event's **city and state match** the community's city and state. The system checks this location match to determine which community waiver (if any) applies.

**Note:** Users may need to sign multiple waivers (e.g., both TrashMob and community waivers). Team events use the same waiver requirements as regular events (no team-specific waivers).

---

### Phase 1 - Waiver Management Infrastructure ✅ Complete
- ✅ Database schema for flexible waivers (versioned, with validity periods)
- ✅ Waiver upload and version control
- ✅ Effective/expiry date management
- ✅ Scheduled activation for future waivers
- ✅ Waiver assignment to communities
- ✅ Immutable document storage (Azure Blob with legal hold)

### Phase 2 - User Workflows (E-Signature) ✅ Complete
- ✅ Website click-to-accept flow (checkbox + typed legal name)
- ✅ Mobile app multi-waiver flow with styled popup (PR #2752)
- ✅ Multi-waiver signing (TrashMob + community in one flow)
- ✅ Waiver text snapshot storage at signing time
- ✅ Server-side PDF generation with waiver text, signature, audit trail
- ✅ PDF download for signed waivers
- ✅ Waiver viewing and printing (My Dashboard)
- ✅ Redesign waiver signing flow with hub/list view (PR #2745)
- ✅ Re-consent when waiver updates
- ✅ Email notifications for expiring waivers

### Phase 3 - Manual Upload Workflow ✅ Complete
- ✅ Paper waiver upload by authorized users (staff, community mgr, team lead, event lead)
- ✅ Image/PDF upload with metadata (signer name, date, event)
- ✅ Validation workflow for uploaded documents
- ✅ Link uploaded waiver to user account
- ✅ Event page admin panel for waiver management (upload, view status, bulk actions)

### Phase 4 - Event Integration ✅ Complete
- ✅ Event registration waiver checks
- ✅ Determine which waivers required (TrashMob + community)
- ✅ Block registration if waiver missing/expired
- ✅ Add waiver signing check before event creation (PR #2748)
- ✅ Add waiver check before creating event from litter report (PR #2744)
- ✅ Fix waiver bypass on create event flow (PR #2756)
- ✅ Navigate to home when waiver is dismissed on create event (PR #2757)
- ✅ Fix waiver timing and display issues (PR #2753)
- ✅ Event lead view of attendee waiver status
- ✅ Day-of check-in waiver verification

### Phase 5 - Dependent Minors & Guardian Waivers

Covers the scenario where a registered adult brings children to events — children who may be under 13 (no account), 13-17 (may or may not have an account), relatives, scout group members, or one-time participants. The adult may bring different children to different events.

**Design Principles:**
- **Zero friction for users without kids** — the dependent system is opt-in; users who never bring children see no additional UI
- **No accounts for children under 13** — sidesteps COPPA entirely; child data lives on the parent's profile
- **Sign once per year, select per event** — annual waiver renewal per dependent, not per-event signing
- **Covers non-parent guardians** — scout leaders, neighbors, grandparents can add dependents with "Authorized Supervisor" relationship

**Legal Context:**
- Parental waivers for minors are unenforceable in 17 US states (TX, IL, PA, WA, etc.) and unclear in 21 more
- Organizations should still collect them: evidence of informed consent, emergency medical authorization, and claim deterrence
- Minor waiver must include: assumption of risk, liability release, medical authorization, photo/media release, statement of guardian authority
- Follow COPPA spirit even if legally exempt as a nonprofit: never collect data directly from children under 13

#### Phase 5a - Data Model & API

**New Entity: `Dependent`** — a minor linked to a registered adult's account

| Field | Type | Required | Purpose |
|-------|------|----------|---------|
| Id | Guid | Yes | Primary key |
| ParentUserId | Guid | Yes | FK to User (the responsible adult) |
| FirstName | string(100) | Yes | Identification at event |
| LastName | string(100) | Yes | Waiver legal requirement |
| DateOfBirth | DateOnly | Yes | Age verification, tier enforcement |
| Relationship | string(50) | Yes | parent, legal guardian, grandparent, authorized supervisor, other |
| MedicalNotes | string(500) | No | Allergies, conditions, medications |
| EmergencyContactPhone | string(20) | No | If different from parent's phone |
| IsActive | bool | Yes | Soft-delete for dependents no longer brought |
| CreatedDate | DateTimeOffset | Yes | Audit |
| LastUpdatedDate | DateTimeOffset | Yes | Audit |

**New Entity: `EventDependent`** — which dependents attend which events

| Field | Type | Required | Purpose |
|-------|------|----------|---------|
| Id | Guid | Yes | Primary key |
| EventId | Guid | Yes | FK to Event |
| DependentId | Guid | Yes | FK to Dependent |
| ParentUserId | Guid | Yes | FK to User (for easy querying) |
| DependentWaiverId | Guid | Yes | FK to DependentWaiver (the waiver covering this dependent) |
| CreatedDate | DateTimeOffset | Yes | When registered |

**New Entity: `DependentWaiver`** — waiver signed by adult covering a specific dependent

| Field | Type | Required | Purpose |
|-------|------|----------|---------|
| Id | Guid | Yes | Primary key |
| DependentId | Guid | Yes | FK to Dependent |
| WaiverVersionId | Guid | Yes | FK to WaiverVersion |
| SignedByUserId | Guid | Yes | FK to User (the adult who signed) |
| TypedLegalName | string | Yes | Adult's legal name |
| WaiverTextSnapshot | string | Yes | Exact waiver text at signing |
| AcceptedDate | DateTimeOffset | Yes | When signed |
| ExpiryDate | DateTimeOffset | Yes | End of calendar year |
| DocumentUrl | string | No | PDF in immutable blob storage |
| IPAddress | string(50) | No | Audit trail |
| UserAgent | string(500) | No | Audit trail |
| CreatedDate | DateTimeOffset | Yes | Audit |

**Age Tiers:**

| Age | Account | Waiver Flow | At Event |
|-----|---------|-------------|----------|
| Under 13 | No account; Dependent on parent's profile | Parent signs DependentWaiver | Parent must be physically present |
| 13-17 | Optional own account (future Project 23) OR Dependent on parent's profile | Parent signs DependentWaiver or UserWaiver with guardian fields | Authorized adult must be present |
| 18+ | Full account | Signs own UserWaiver | Independent |

**API Endpoints:**

| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| GET | /users/{userId}/dependents | ValidUser | List user's dependents |
| POST | /users/{userId}/dependents | ValidUser | Add a dependent |
| PUT | /users/{userId}/dependents/{dependentId} | ValidUser | Update dependent info |
| DELETE | /users/{userId}/dependents/{dependentId} | ValidUser | Soft-delete dependent |
| POST | /dependents/{dependentId}/waiver | ValidUser | Sign waiver for a dependent |
| GET | /dependents/{dependentId}/waiver | ValidUser | Get current waiver status |
| POST | /events/{eventId}/dependents | ValidUser | Register dependents for event |
| GET | /events/{eventId}/dependents | EventLead | List dependents attending event |
| GET | /events/{eventId}/dependent-count | ValidUser | Count of dependents registered |

- ☐ Create Dependent model, repository, manager
- ☐ Create EventDependent model, repository, manager
- ☐ Create DependentWaiver model, repository, manager
- ☐ Add migration for new tables with proper indexes and FKs
- ☐ Create API controllers with authorization
- ☐ Create minor-specific waiver text (add to WaiverVersion with a MinorWaiver flag or separate scope)

#### Phase 5b - Minor Waiver Content

The minor waiver must contain language beyond the standard adult waiver:

1. **Statement of authority:** "I, [parent/guardian name], am the [relationship] of [minor's name], born [DOB]. I have legal authority to consent to [minor's name]'s participation."
2. **Assumption of risk:** Specific to outdoor cleanup — sharp objects, uneven terrain, traffic, weather, contact with hazardous materials
3. **Liability release and indemnification:** Standard waiver language naming the minor
4. **Medical authorization:** "I authorize TrashMob.eco and event organizers to arrange emergency medical treatment for [minor's name] at my expense in the event of injury or illness."
5. **Photo/media release:** Consent for the minor to appear in event photos and social media
6. **Supervision acknowledgment:** "I understand that I am responsible for supervising [minor's name] during this activity" (for parent/guardian) or "I confirm I have been authorized by the parent/legal guardian of [minor's name] to supervise this child" (for authorized supervisors)
7. **Activity acknowledgment:** "I understand that this activity involves outdoor litter cleanup which may include walking on uneven surfaces, handling litter and debris, exposure to weather conditions, and proximity to traffic"

- ☐ Draft minor waiver text (requires legal review)
- ☐ Draft "authorized supervisor" variant waiver text
- ☐ Create WaiverVersion record with minor scope
- ☐ PDF generation for dependent waivers (extend existing QuestPDF template)

#### Phase 5c - Web UX

**Profile > "My Dependents" Page:**
- Accessible from user dashboard (not prominently featured — opt-in discovery)
- List of dependents with name, age, relationship, waiver status badge (valid/expiring/expired/none)
- "Add Dependent" button → inline form: first name, last name, DOB, relationship dropdown, medical notes, emergency phone
- Edit/remove existing dependents
- "Sign Waiver" button next to dependents without valid waivers → opens minor waiver signing dialog

**Event Registration Flow (Modified):**
1. User registers themselves (existing flow, unchanged)
2. **New optional section appears after self-registration:** "Will you be bringing any dependents?"
   - If user has no dependents on file → "Add a dependent" link (goes to profile dependents page or inline add)
   - If user has dependents → checkboxes to select who is attending this event
   - Each dependent shows waiver status badge
   - Dependents with expired/missing waivers → inline "Sign Waiver" before completing
   - "Add another dependent" link for new children
3. Confirmation shows attendee + dependents registered

**Event Lead View (Extended):**
- Existing attendee waiver status list extended with "Dependents" column
- Shows count of dependents per attendee
- Expandable row to see dependent names, ages, waiver status
- Export includes dependent data

- ☐ Create "My Dependents" page component
- ☐ Add dependents section to user dashboard
- ☐ Modify event registration flow with optional dependent selection
- ☐ Create minor waiver signing dialog (extend WaiverSigningDialog)
- ☐ Extend event lead attendee view with dependent information
- ☐ Extend compliance export with dependent waivers

#### Phase 5d - Mobile UX

**Profile > Dependents:**
- "My Dependents" section in profile/settings
- Add/edit/remove dependents
- Sign minor waivers via existing mobile waiver signing popup pattern

**Event Registration:**
- After self-registration, optional "Bringing dependents?" prompt
- Select from existing dependents or add new inline
- Sign waiver inline if needed (extend existing multi-waiver popup flow)

- ☐ Add dependents management to mobile profile
- ☐ Extend mobile event registration with dependent selection
- ☐ Extend mobile waiver popup for minor waiver signing

#### Phase 5e - Event Headcount & Safety

- ☐ Include dependent count in event attendee totals (for capacity planning)
- ☐ Event lead can see total headcount: registered adults + dependents
- ☐ Dependent age display for event leads (safety planning — knowing how many young children)
- ☐ Admin compliance dashboard extended: dependent waiver counts, guardian relationship breakdown

**Note:** Phase 5 no longer requires Project 23 (Privo.com) for the common case of adults bringing children. Project 23 remains relevant for the separate case of 13-17 year-olds who want their own user accounts with age verification and parental consent.

### Phase 6 - Admin & Viewing Tools ✅ Complete
- ✅ Admin dashboard for waiver compliance (summary stats, filtering, pagination)
- ✅ View waivers by admin (all signed waivers with user details)
- ✅ Bulk export for legal review (CSV export with all audit data)
- ✅ Audit logs in export (IP, user agent, signing method, timestamps)
- ☐ Exception handling workflow (future: manual override for edge cases)

---

## Out-of-Scope

- ❌ Third-party e-signature integration (DocuSign/Adobe Sign) - use native implementation
- ❌ Automated legal compliance checking
- ❌ Insurance integration
- ❌ International legal variations (focus on US initially)

---

## Success Metrics

### Quantitative
- **Waiver compliance:** 100% of event attendees have valid waivers
- **Waiver expiry warnings:** 90% of users re-consent before expiry
- **Admin audit time:** Reduce from 2 hours to 15 minutes per month
- **Legal review time:** Reduce by 50% with proper exports

### Qualitative
- Legal team sign-off on implementation
- Zero waiver-related legal incidents
- Positive feedback from community partners
- Clear, user-friendly waiver workflow

---

## Dependencies

### Blockers
- **Legal review:** Must approve minor waiver text before deployment

### Related (Non-Blocking)
- **Project 23 (Parental Consent):** Privo.com integration — only needed for 13-17 year-olds who want their own user accounts; not required for dependent waiver system
- **Project 1 (Auth):** Minors support with age verification — same scope as Project 23

### Enables
- **Project 10 (Community Pages):** Community-specific waivers
- **Event registration:** Proper legal protection

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Legal non-compliance** | Low | Critical | Mandatory legal review gate; professional counsel |
| **Complex minor workflows** | High | High | Phased approach; extensive testing; clear UX |
| **Performance issues** | Medium | Medium | Optimize queries; cache waiver status; index properly |
| **User confusion** | High | Medium | Clear messaging; help documentation; onboarding flow |
| **Waiver acceptance fraud** | Low | High | Audit trail; IP logging; timestamp verification |

---

## Implementation Plan

### Data Model Changes

> **Note:** Existing `Waiver` entity in `TrashMob.Models/Waiver.cs` handles basic waiver info.
> These new entities extend the waiver system for V3 features.

**New Entity: WaiverVersion (replaces or extends Waiver)**
```csharp
// New file: TrashMob.Models/WaiverVersion.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a versioned waiver document with effective dates.
    /// </summary>
    public class WaiverVersion : KeyedModel
    {
        /// <summary>
        /// Gets or sets the waiver name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version string (e.g., "2.0").
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the full waiver text content.
        /// </summary>
        public string WaiverText { get; set; }

        /// <summary>
        /// Gets or sets when this waiver version becomes effective.
        /// </summary>
        public DateTimeOffset EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets when this waiver version expires (null = no expiry).
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets how long user acceptance is valid (null = indefinite).
        /// </summary>
        public int? ValidityPeriodDays { get; set; }

        /// <summary>
        /// Gets or sets whether this waiver version is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets scope of the waiver (All, Community, Event).
        /// </summary>
        public string AppliesTo { get; set; }

        // Navigation properties
        public virtual ICollection<CommunityWaiver> CommunityWaivers { get; set; }
        public virtual ICollection<UserWaiver> UserWaivers { get; set; }
    }
}
```

**New Entity: CommunityWaiver**
```csharp
// New file: TrashMob.Models/CommunityWaiver.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Associates a waiver with a community (partner).
    /// </summary>
    public class CommunityWaiver : KeyedModel
    {
        /// <summary>
        /// Gets or sets the community (partner) identifier.
        /// </summary>
        public Guid CommunityId { get; set; }

        /// <summary>
        /// Gets or sets the waiver identifier.
        /// </summary>
        public Guid WaiverId { get; set; }

        /// <summary>
        /// Gets or sets whether this waiver is required for the community.
        /// </summary>
        public bool IsRequired { get; set; } = true;

        // Navigation properties
        public virtual Partner Community { get; set; }
        public virtual WaiverVersion Waiver { get; set; }
    }
}
```

**New Entity: UserWaiver**
```csharp
// New file: TrashMob.Models/UserWaiver.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Records a user's acceptance of a waiver with audit trail.
    /// </summary>
    public class UserWaiver : KeyedModel
    {
        /// <summary>
        /// Gets or sets the user who accepted the waiver.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the waiver that was accepted.
        /// </summary>
        public Guid WaiverId { get; set; }

        /// <summary>
        /// Gets or sets when the waiver was accepted/signed.
        /// </summary>
        public DateTimeOffset AcceptedDate { get; set; }

        /// <summary>
        /// Gets or sets when this acceptance expires.
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the typed legal name entered by the signer.
        /// </summary>
        public string TypedLegalName { get; set; }

        /// <summary>
        /// Gets or sets the full waiver text at time of signing (snapshot for PDF generation).
        /// </summary>
        public string WaiverTextSnapshot { get; set; }

        #region Signing Method

        /// <summary>
        /// Gets or sets how the waiver was signed (ESignatureWeb, ESignatureMobile, PaperUpload).
        /// </summary>
        public string SigningMethod { get; set; }

        /// <summary>
        /// Gets or sets the URL to the generated PDF in immutable blob storage.
        /// </summary>
        public string DocumentUrl { get; set; }

        /// <summary>
        /// Gets or sets who uploaded the paper waiver (null for e-signatures).
        /// </summary>
        public Guid? UploadedByUserId { get; set; }

        #endregion

        #region Audit Trail

        /// <summary>
        /// Gets or sets the IP address at time of acceptance.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent at time of acceptance.
        /// </summary>
        public string UserAgent { get; set; }

        #endregion

        #region Minor Support

        /// <summary>
        /// Gets or sets whether the user was a minor at time of acceptance.
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the guardian's user ID (if minor).
        /// </summary>
        public Guid? GuardianUserId { get; set; }

        /// <summary>
        /// Gets or sets the guardian's name (if minor and guardian not a user).
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// Gets or sets the guardian's relationship to the minor.
        /// </summary>
        public string GuardianRelationship { get; set; }

        #endregion

        // Navigation properties
        public virtual User User { get; set; }
        public virtual User UploadedByUser { get; set; }
        public virtual WaiverVersion Waiver { get; set; }
        public virtual User GuardianUser { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<WaiverVersion>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
    entity.Property(e => e.Version).HasMaxLength(50).IsRequired();
    entity.Property(e => e.AppliesTo).HasMaxLength(50).IsRequired();
});

modelBuilder.Entity<CommunityWaiver>(entity =>
{
    entity.HasOne(e => e.Community)
        .WithMany()
        .HasForeignKey(e => e.CommunityId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Waiver)
        .WithMany(w => w.CommunityWaivers)
        .HasForeignKey(e => e.WaiverId)
        .OnDelete(DeleteBehavior.Cascade);
});

modelBuilder.Entity<UserWaiver>(entity =>
{
    entity.Property(e => e.IPAddress).HasMaxLength(50);
    entity.Property(e => e.UserAgent).HasMaxLength(500);
    entity.Property(e => e.GuardianName).HasMaxLength(200);
    entity.Property(e => e.GuardianRelationship).HasMaxLength(100);

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Waiver)
        .WithMany(w => w.UserWaivers)
        .HasForeignKey(e => e.WaiverId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.GuardianUser)
        .WithMany()
        .HasForeignKey(e => e.GuardianUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => e.WaiverId);
    entity.HasIndex(e => e.ExpiryDate);
});
```

### API Changes

```csharp
// Waiver management (Admin only)
[Authorize(Roles = "Admin")]
[HttpPost("api/waivers")]
public async Task<ActionResult<WaiverDto>> CreateWaiver([FromBody] CreateWaiverRequest request)
{
    // Create new waiver version
}

[Authorize(Roles = "Admin")]
[HttpPost("api/communities/{communityId}/waivers")]
public async Task<ActionResult> AssignWaiverToCommunity(
    Guid communityId,
    [FromBody] AssignWaiverRequest request)
{
    // Assign waiver to community
}

// User waiver acceptance
[Authorize]
[HttpGet("api/users/{userId}/waivers/required")]
public async Task<ActionResult<IEnumerable<WaiverDto>>> GetRequiredWaivers(Guid userId)
{
    // Get waivers user needs to sign
}

[Authorize]
[HttpPost("api/users/{userId}/waivers/{waiverId}/accept")]
public async Task<ActionResult<UserWaiverDto>> AcceptWaiver(
    Guid userId,
    Guid waiverId,
    [FromBody] AcceptWaiverRequest request)
{
    // Record waiver acceptance with audit trail
}

// Event waiver checks
[HttpGet("api/events/{eventId}/waiver-status/{userId}")]
public async Task<ActionResult<WaiverStatusDto>> CheckEventWaiverStatus(
    Guid eventId,
    Guid userId)
{
    // Check if user has all required waivers for event
    // Returns: { hasAllWaivers: true/false, missingWaivers: [...] }
}
```

### Web UX Changes

**Event Attendee Registration Flow (E-Signature):**

> **Important:** Waiver signing occurs during **event attendee registration**, not when creating a user account.

1. User clicks "Register for Event" on event page
2. System determines required waivers:
   - TrashMob waiver (always required)
   - Community waiver (if event city/state matches a community with custom waiver)
3. System checks if user has valid signatures for all required waivers
4. If any waivers missing or expired:
   - Display waiver(s) with checkboxes in multi-step form
   - User reads and accepts each waiver
   - System records acceptance with timestamp/IP/user agent
5. Complete event registration
6. User receives confirmation with links to view signed waivers

**Paper Waiver Upload Flow:**
1. Event Lead/Community Mgr/Team Lead/Staff navigates to event attendee list
2. Clicks "Upload Paper Waiver" for attendee
3. Uploads signed waiver image/PDF
4. Enters metadata: signer name, date signed
5. System stores document immutably and links to user account

**Waiver Management Pages:**
- `/admin/waivers` - Admin dashboard for all waivers
- `/admin/waivers/create` - Create new waiver
- `/communities/{id}/waivers` - Manage community waivers

**My Dashboard - Waivers Section:**
- New "My Waivers" card/section on user dashboard (`/mydashboard`)
- Shows list of signed waivers with status (valid, expiring soon, expired)
- Quick view of waiver expiry dates
- Link to view/download each signed waiver PDF
- Alert banner for waivers expiring within 30 days

**Note:** No separate `/waivers` page needed - users view their waivers directly on their dashboard.

**Event Lead View:**
- Attendee list with waiver status indicator
- ✓ Valid, ⚠ Expiring soon, ✗ Missing/Expired
- Export button for waiver report

**Event Page Admin Panel (for Event Lead/Community Mgr/Team Lead/Staff):**
- "Manage Waivers" section visible to authorized users on event detail page
- Upload signed paper waiver images/PDFs for attendees (one at a time)
- View all attendee waiver statuses at a glance
- Quick actions: Upload waiver, Send reminder, Mark exception
- Audit log of all waiver uploads for this event

**Note:** Bulk upload of paper waivers is out of scope for V3.

### Mobile App Changes

- Waiver acceptance within registration flow
- View accepted waivers in profile
- Push notifications for expiring waivers

---

## Implementation Phases

### Phase 1: Infrastructure (Critical)
- Database schema
- Admin UI for waiver management
- Upload and versioning system
- **Gate:** Legal review and approval

### Phase 2: User Workflows
- Waiver acceptance flow
- View/print waivers
- Re-consent workflow
- Email notifications

### Phase 3: Event Integration
- Event registration checks
- Determine required waivers
- Block flow if non-compliant
- Event lead dashboard

### Phase 4: Dependent Minors & Guardian Waivers
- Dependent profiles on adult accounts (no child accounts needed)
- Guardian waiver signing for each dependent (annual)
- Per-event dependent selection during registration
- Event lead headcount and dependent waiver compliance
- Web and mobile UX for dependent management
- **Gate:** Legal review of minor waiver text

### Phase 5: Admin & Reporting
- Compliance dashboard
- Audit reports
- Bulk export
- Exception handling

**Note:** Phase 1 MUST have legal approval before proceeding.

---

## Legal Review Requirements

### Must Address:
1. **Enforceability:** Are digital waivers legally binding?
2. **Minor consent:** What constitutes valid parental consent?
3. **Retention:** How long must we keep waiver records?
4. **Proof:** What audit trail is sufficient for court?
5. **Updates:** How to handle waiver version changes?
6. **Jurisdiction:** State-specific requirements?

### Deliverables for Legal Review:
- Database schema documentation
- User flow diagrams
- Sample waiver acceptance screens
- Audit trail specifications
- Data retention policy

---

## Open Questions

1. ~~**Do we need both TrashMob AND community waivers, or can community waiver replace TrashMob?**~~
   **Decision:** Yes, both required. TrashMob waiver covers platform liability; community waiver covers local/partner requirements. Users may need to sign 2 waivers for community events.
   **Status:** ✅ Resolved

2. ~~**What's the validity period for waivers?**~~
   **Decision:** Waivers are valid for a calendar year. Users must re-sign at their first event in a new calendar year.
   **Status:** ✅ Resolved

3. ~~**How do we handle minors who turn 18 during waiver validity?**~~
   **Decision:** Require re-consent on birthday; automated check triggers new waiver signing
   **Status:** ✅ Resolved

4. ~~**Can guardians sign for multiple minors at once?**~~
   **Decision:** Yes, with clear UI showing each minor
   **Status:** ✅ Resolved

5. ~~**What happens if community changes its waiver mid-year?**~~
   **Decision:** New waiver version invalidates all previous signatures. All users must re-sign the new waiver before attending events requiring it. No grandfathering.
   **Status:** ✅ Resolved

6. ~~**What immutable storage solution for signed waivers?**~~
   **Decision:** Azure Blob Storage with immutability policies (legal hold or time-based retention)
   **Status:** ✅ Resolved

7. **Who can upload manually-signed paper waivers?**
   **Decision:** TrashMob staff, Community Manager, Team Leader, Event Lead
   **Status:** ✅ Resolved

8. ~~**What PDF generation library for server-side waiver PDFs?**~~
   **Decision:** QuestPDF (modern, .NET native, free for open source)
   **Status:** ✅ Resolved

9. ~~**E-signature implementation approach?**~~
   **Decision:** Native click-to-accept implementation (checkbox + typed legal name + audit trail). No third-party services (DocuSign/Adobe Sign). Legally valid under ESIGN Act and UETA.
   **Status:** ✅ Resolved

10. ~~**How do community admins compare waiver versions to see what changed?**~~
    **Decision:** Provide diff view showing added/removed/changed text between versions; highlight changes clearly. Low priority feature.
    **Status:** ✅ Resolved

11. ~~**What if a user registers for events in multiple communities on the same day?**~~
    **Decision:** Waivers are per community, not per event. User must sign each community's waiver once (valid for calendar year). Same waiver covers all events in that community.
    **Status:** ✅ Resolved

12. ~~**How do we handle team events that span community boundaries?**~~
    **Decision:** Event location (city/state) determines the community; single community waiver applies based on event's registered location
    **Status:** ✅ Resolved

13. ~~**What accessibility accommodations must waivers support?**~~
    **Decision:** Screen reader accessible (proper semantic HTML); plain text version available for download; large print / high contrast options; keyboard-navigable. Low priority.
    **Status:** ✅ Resolved

14. ~~**How do we handle adults bringing unregistered children to events?**~~
    **Decision:** "Dependent Profiles" model — adults manage a list of dependents on their account. Children under 13 never have their own accounts (sidesteps COPPA). Adult signs a minor waiver per dependent per year. At event registration, adult optionally selects which dependents are attending. Zero friction for users without children.
    **Status:** ✅ Resolved

15. ~~**What if an adult brings different children to different events?**~~
    **Decision:** Dependents are stored on the adult's profile, not tied to any event. Adult can add new dependents at any time (including during event registration). Per-event, the adult just checks boxes for which dependents are coming. Waiver is per-dependent per-year, not per-event.
    **Status:** ✅ Resolved

16. ~~**What about scout leaders or non-parent guardians?**~~
    **Decision:** Relationship field supports "Authorized Supervisor" in addition to parent/guardian. Waiver text includes: "I confirm I have been authorized by the parent/legal guardian of [name] to supervise this child and consent to emergency medical treatment on their behalf." This shifts some liability but is not a substitute for actual parental consent in states that reject parental waivers.
    **Status:** ✅ Resolved

17. ~~**Should we require parental presence for younger children?**~~
    **Decision:** Following Friends of the Urban Forest model — under 14 requires the responsible adult to be physically present (1:1). 14-17 can attend with any authorized adult supervisor. This is policy, enforced by event leads at check-in, not by the software.
    **Status:** ✅ Resolved

18. **Should the minor waiver be a separate WaiverVersion or a flag on the existing waiver?**
    **Decision:** TBD — could be a new WaiverVersion with `Scope = Minor`, or add a `IsMinorWaiver` flag. Separate version allows different text for minors vs adults.
    **Status:** Open

19. **Do we need to capture the child's own signature/assent for 13-17 year-olds?**
    **Decision:** TBD — some organizations require both parent signature AND minor assent for teens. Legal review needed.
    **Status:** Open

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2230](https://github.com/trashmob/TrashMob/issues/2230)** - Project 8: Liability Waivers V3 (tracking issue)
- **[#1032](https://github.com/trashmob/TrashMob/issues/1032)** - Move waiver versions to database instead of hardcoding them
- **[#938](https://github.com/trashmob/TrashMob/issues/938)** - Allow admin to view a report of when users have signed waivers
- **[#2146](https://github.com/trashmob/TrashMob/issues/2146)** - Waiver pops up briefly, then automatically registers without giving chance to read

---

## Related Documents

- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** - Minors authentication
- **[Project 23 - Parental Consent](./Project_23_Parental_Consent.md)** - Privo.com integration
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community-specific waivers
- **Legal Requirements:** (To be created after legal review)

---

**Last Updated:** March 3, 2026
**Owner:** Product Lead + Legal Counsel
**Status:** In Progress (Phases 1-4, 6 Complete; Phase 5 Ready for Development — no longer blocked on Project 23)
**Next Review:** After legal review of minor waiver text

**?? CRITICAL:** No development work begins until legal team provides written approval of approach.
