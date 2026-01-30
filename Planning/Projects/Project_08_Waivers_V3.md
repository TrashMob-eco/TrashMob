# Project 8 � Liability Waivers V3

| Attribute | Value |
|-----------|-------|
| **Status** | Requirements & Legal Review |
| **Priority** | High |
| **Risk** | Very High |
| **Size** | Very Large |
| **Dependencies** | Project 23 (Parental Consent) |

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

- Waivers are valid for a period specified when created (e.g., 1 year)
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

### Phase 1 - Waiver Management Infrastructure
- ☐ Database schema for flexible waivers (versioned, with validity periods)
- ☐ Waiver upload and version control
- ☐ Effective/expiry date management
- ☐ Scheduled activation for future waivers
- ☐ Waiver assignment to communities
- ☐ Immutable document storage (Azure Blob with legal hold)

### Phase 2 - User Workflows (E-Signature)
- ☐ Website click-to-accept flow (checkbox + typed legal name)
- ☐ Mobile app click-to-accept flow
- ☐ Multi-waiver signing (TrashMob + community in one flow)
- ☐ Waiver text snapshot storage at signing time
- ☐ Server-side PDF generation with waiver text, signature, audit trail
- ☐ PDF download for signed waivers
- ☐ Waiver viewing and printing
- ☐ Re-consent when waiver updates
- ☐ Email notifications for expiring waivers

### Phase 3 - Manual Upload Workflow
- ☐ Paper waiver upload by authorized users (staff, community mgr, team lead, event lead)
- ☐ Image/PDF upload with metadata (signer name, date, event)
- ☐ Validation workflow for uploaded documents
- ☐ Link uploaded waiver to user account
- ☐ Event page admin panel for waiver management (upload, view status, bulk actions)

### Phase 4 - Event Integration
- ☐ Event registration waiver checks
- ☐ Determine which waivers required (TrashMob + community)
- ☐ Block registration if waiver missing/expired
- ☐ Event lead view of attendee waiver status
- ☐ Day-of check-in waiver verification

### Phase 5 - Minors Support
- ☐ Guardian consent workflow
- ☐ Minor actions based on parental consent scope (e.g., event creation, team leadership)
- ☐ Minors require adult presence at events
- ☐ Age verification integration (with Project 23)

### Phase 6 - Admin & Viewing Tools
- ☐ Admin dashboard for waiver compliance
- ☐ View waivers by authorized party (signer, community lead, team lead, staff)
- ☐ Bulk export for legal review
- ☐ Audit logs and reporting
- ☐ Exception handling workflow

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
- **Legal review:** Must approve approach before development
- **Project 23 (Parental Consent):** Privo.com integration for minors

### Related (Non-Blocking)
- **Project 1 (Auth):** Minors support with age verification

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
- `/waivers` - List of waivers user has signed (viewable by signer)
- `/waivers/{id}` - View/print specific signed waiver
- `/admin/waivers` - Admin dashboard for all waivers
- `/admin/waivers/create` - Create new waiver
- `/communities/{id}/waivers` - Manage community waivers

**My Dashboard - Waivers Section:**
- New "My Waivers" card/section on user dashboard (`/mydashboard`)
- Shows list of signed waivers with status (valid, expiring soon, expired)
- Quick view of waiver expiry dates
- Link to view/download each signed waiver PDF
- Alert banner for waivers expiring within 30 days
- "View All Waivers" link to `/waivers` page

**Event Lead View:**
- Attendee list with waiver status indicator
- ✓ Valid, ⚠ Expiring soon, ✗ Missing/Expired
- Export button for waiver report

**Event Page Admin Panel (for Event Lead/Community Mgr/Team Lead/Staff):**
- "Manage Waivers" section visible to authorized users on event detail page
- Upload signed paper waiver images/PDFs for attendees
- Bulk upload option for multiple waivers
- View all attendee waiver statuses at a glance
- Quick actions: Upload waiver, Send reminder, Mark exception
- Audit log of all waiver uploads for this event

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

### Phase 4: Minors Support (Depends on Project 1 & 23)
- Guardian consent workflow with configurable action permissions
- Age verification integration
- Enforce parental consent scope (what actions minor is allowed to perform)
- Adult presence validation at events

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

2. **What's the validity period for waivers?**
   **Recommendation:** 1 year default; configurable per waiver
   **Owner:** Legal team + Product
   **Due:** Before Phase 1

3. **How do we handle minors who turn 18 during waiver validity?**
   **Recommendation:** Require re-consent on birthday; automated check
   **Owner:** Legal team
   **Due:** Before Phase 5

4. **Can guardians sign for multiple minors at once?**
   **Recommendation:** Yes, with clear UI showing each minor
   **Owner:** Product team
   **Due:** Before Phase 5

5. ~~**What happens if community changes its waiver mid-year?**~~
   **Decision:** New waiver version invalidates all previous signatures. All users must re-sign the new waiver before attending events requiring it. No grandfathering.
   **Status:** ✅ Resolved

6. **What immutable storage solution for signed waivers?**
   **Recommendation:** Azure Blob Storage with immutability policies (legal hold or time-based retention)
   **Owner:** Engineering
   **Due:** Before Phase 1

7. **Who can upload manually-signed paper waivers?**
   **Decision:** TrashMob staff, Community Manager, Team Leader, Event Lead
   **Status:** ✅ Resolved

8. **What PDF generation library for server-side waiver PDFs?**
   **Recommendation:** QuestPDF (modern, .NET native, free for open source) or iTextSharp
   **Owner:** Engineering
   **Due:** Before Phase 2

9. ~~**E-signature implementation approach?**~~
   **Decision:** Native click-to-accept implementation (checkbox + typed legal name + audit trail). No third-party services (DocuSign/Adobe Sign). Legally valid under ESIGN Act and UETA.
   **Status:** ✅ Resolved

---

## Related Documents

- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** - Minors authentication
- **[Project 23 - Parental Consent](./Project_23_Parental_Consent.md)** - Privo.com integration
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community-specific waivers
- **Legal Requirements:** (To be created after legal review)

---

**Last Updated:** January 29, 2026
**Owner:** Product Lead + Legal Counsel
**Status:** Requirements & Legal Review
**Next Review:** After legal team approval

**?? CRITICAL:** No development work begins until legal team provides written approval of approach.
