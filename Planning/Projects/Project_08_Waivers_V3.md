# Project 8 — Liability Waivers V3

| Attribute | Value |
|-----------|-------|
| **Status** | Requirements & Legal Review |
| **Priority** | High |
| **Risk** | Very High |
| **Size** | Very Large |
| **Dependencies** | Project 1 (Auth - minors support) |

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

### Phase 1 - Waiver Management Infrastructure
- ? Database schema for flexible waivers
- ? Waiver upload and version control
- ? Effective/expiry date management
- ? Waiver assignment to communities

### Phase 2 - User Workflows
- ? User waiver acceptance flow
- ? Waiver viewing and printing
- ? Re-consent when waiver updates
- ? Email notifications for expiring waivers

### Phase 3 - Event Integration
- ? Event registration waiver checks
- ? Determine which waivers required (TrashMob + community)
- ? Block registration if waiver missing/expired
- ? Event lead view of attendee waiver status

### Phase 4 - Minors Support
- ? Guardian consent workflow
- ? Minors can't create events
- ? Minors require adult presence at events
- ? Age verification integration (with Project 23)

### Phase 5 - Admin Tools
- ? Admin dashboard for waiver compliance
- ? Bulk export for legal review
- ? Audit logs and reporting
- ? Exception handling workflow

---

## Out-of-Scope

- ? E-signature integration (Phase 2 - evaluate DocuSign/Adobe Sign)
- ? Automated legal compliance checking
- ? Insurance integration
- ? International legal variations (focus on US initially)

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
- **Project 1 (Auth):** Minors support with age verification
- **Project 23 (Parental Consent):** Privo.com integration for minors

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

```sql
-- Waiver template/document
CREATE TABLE Waivers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Version NVARCHAR(50) NOT NULL, -- e.g., "2.0"
    WaiverText NVARCHAR(MAX) NOT NULL,
    EffectiveDate DATETIMEOFFSET NOT NULL,
    ExpiryDate DATETIMEOFFSET NULL,
    ValidityPeriodDays INT NULL, -- How long acceptance is valid
    IsActive BIT NOT NULL DEFAULT 1,
    AppliesTo NVARCHAR(50) NOT NULL, -- 'All', 'Community', 'Event'
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    LastUpdatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);

-- Community-specific waivers
CREATE TABLE CommunityWaivers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CommunityId UNIQUEIDENTIFIER NOT NULL,
    WaiverId UNIQUEIDENTIFIER NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (CommunityId) REFERENCES Partners(Id),
    FOREIGN KEY (WaiverId) REFERENCES Waivers(Id)
);

-- User waiver acceptances
CREATE TABLE UserWaivers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    WaiverId UNIQUEIDENTIFIER NOT NULL,
    AcceptedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    ExpiryDate DATETIMEOFFSET NULL,
    IPAddress NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,
    IsMinor BIT NOT NULL DEFAULT 0,
    GuardianUserId UNIQUEIDENTIFIER NULL, -- If minor
    GuardianName NVARCHAR(200) NULL,
    GuardianRelationship NVARCHAR(100) NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (WaiverId) REFERENCES Waivers(Id),
    FOREIGN KEY (GuardianUserId) REFERENCES Users(Id)
);

CREATE INDEX IX_UserWaivers_UserId ON UserWaivers(UserId);
CREATE INDEX IX_UserWaivers_WaiverId ON UserWaivers(WaiverId);
CREATE INDEX IX_UserWaivers_ExpiryDate ON UserWaivers(ExpiryDate);
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

**Waiver Acceptance Flow:**
1. User registers for event
2. System checks required waivers (TrashMob + community)
3. If missing/expired, redirect to waiver page
4. Display waiver(s) with checkboxes
5. User reads and accepts
6. System records acceptance with timestamp/IP
7. Continue to event registration

**Waiver Management Pages:**
- `/waivers` - List of waivers user has signed
- `/waivers/{id}` - View/print specific waiver
- `/admin/waivers` - Admin dashboard
- `/admin/waivers/create` - Create new waiver
- `/communities/{id}/waivers` - Manage community waivers

**Event Lead View:**
- Attendee list with waiver status indicator
- ? Valid, ?? Expiring soon, ? Missing/Expired
- Export button for waiver report

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
- Guardian consent workflow
- Age verification integration
- Minor-specific restrictions
- Adult presence validation

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

1. **Do we need both TrashMob AND community waivers, or can community waiver replace TrashMob?**  
   **Recommendation:** Both required; TrashMob covers platform, community covers local  
   **Owner:** Legal team  
   **Due:** Before Phase 1

2. **What's the validity period for waivers?**  
   **Recommendation:** 1 year default; configurable per waiver  
   **Owner:** Legal team + Product  
   **Due:** Before Phase 1

3. **How do we handle minors who turn 18 during waiver validity?**  
   **Recommendation:** Require re-consent on birthday; automated check  
   **Owner:** Legal team  
   **Due:** Before Phase 4

4. **Can guardians sign for multiple minors at once?**  
   **Recommendation:** Yes, with clear UI showing each minor  
   **Owner:** Product team  
   **Due:** Before Phase 4

5. **What happens if community changes its waiver mid-year?**  
   **Recommendation:** Grandfather existing registrations; new users see new waiver  
   **Owner:** Product + Legal  
   **Due:** Before Phase 3

---

## Related Documents

- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** - Minors authentication
- **[Project 23 - Parental Consent](./Project_23_Parental_Consent.md)** - Privo.com integration
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community-specific waivers
- **Legal Requirements:** (To be created after legal review)

---

**Last Updated:** January 24, 2026  
**Owner:** Product Lead + Legal Counsel  
**Status:** Requirements & Legal Review  
**Next Review:** After legal team approval

**?? CRITICAL:** No development work begins until legal team provides written approval of approach.
