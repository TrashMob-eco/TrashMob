# Project 28 — Photo Moderation Admin Page

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

User-generated content (event photos, litter report images) requires moderation to ensure compliance with Terms of Service and prevent inappropriate content. Site admins need a centralized interface to review flagged photos, approve or remove content, and notify uploaders of violations.

**Current State:**
- Photos can be uploaded for events and litter reports
- No admin interface exists for reviewing or moderating photos
- No flagging mechanism for users to report inappropriate content
- No notification workflow when photos are removed

---

## Objectives

### Primary Goals
- Create admin page to view all photos pending moderation
- Enable admin approval/rejection workflow with reason codes
- Implement user flagging mechanism for inappropriate content
- Send email notifications to uploaders when photos are removed
- Maintain audit log of moderation actions

### Secondary Goals (Nice-to-Have)
- Batch moderation actions (approve/reject multiple photos at once)
- Photo moderation statistics dashboard
- Automated content moderation using AI/ML (future phase)

---

## Scope

### Phase 1 - Data Model & API
- ✅ Add PhotoModerationStatus enum and fields to photo tables
- ✅ Create PhotoFlag table for user reports
- ✅ Create PhotoModerationLog audit table
- ✅ Implement admin moderation API endpoints
- ✅ Implement user flagging API endpoint

### Phase 2 - Admin UI
- ✅ Site Admin > Photo Moderation page
- ✅ Pending/Flagged/Recently Moderated tabs
- ✅ Photo detail modal with context (event/litter report info)
- ✅ Approve/Reject actions with reason codes

### Phase 3 - User-Facing & Notifications
- ✅ Add "Report Photo" option on public photo views
- ✅ Email notification on photo removal
- ✅ Documentation and admin training

---

## Out-of-Scope

- ❌ Automated content moderation (AI/ML) - future phase
- ❌ Partner/community-level moderation (admins only for now)
- ❌ Video moderation (photos only)
- ❌ Real-time moderation queue (batch processing is sufficient)

---

## Success Metrics

### Quantitative
- **Resolution Time:** Average time from flag to resolution < 24 hours
- **Notification Rate:** 100% of removed photos have notification sent to uploader
- **Queue Coverage:** Zero inappropriate photos remaining after admin review

### Qualitative
- Site admins can efficiently review flagged content
- Users feel empowered to report inappropriate content
- Clear audit trail for all moderation decisions

---

## Dependencies

### Blockers (Must be complete before this project starts)
- None (existing photo infrastructure is sufficient)

### Enablers for Other Projects (What this unlocks)
- Future AI/ML content moderation integration
- Community-level moderation delegation

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **High volume of flagged content** | Low | Medium | Batch processing UI, consider AI pre-screening in future |
| **False positives from user flags** | Medium | Low | Require reason codes, log all actions, no penalty for reported users |
| **Admin response time SLA** | Low | Medium | Dashboard alerts for aging items in queue |

---

## Implementation Plan

### Data Model Changes

**PhotoModerationStatus enum:**
```csharp
public enum PhotoModerationStatus
{
    Pending = 0,    // Newly uploaded, not yet reviewed
    Approved = 1,   // Reviewed and approved
    Rejected = 2    // Reviewed and removed
}
```

**Add to EventMedia and LitterImage tables:**
```sql
ALTER TABLE EventMedia ADD
    ModerationStatus INT NOT NULL DEFAULT 0,
    ModeratedByUserId UNIQUEIDENTIFIER NULL,
    ModeratedDate DATETIMEOFFSET NULL,
    ModerationReason NVARCHAR(500) NULL;

ALTER TABLE LitterImages ADD
    ModerationStatus INT NOT NULL DEFAULT 0,
    ModeratedByUserId UNIQUEIDENTIFIER NULL,
    ModeratedDate DATETIMEOFFSET NULL,
    ModerationReason NVARCHAR(500) NULL;
```

**PhotoFlag table:**
```sql
CREATE TABLE PhotoFlags (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PhotoId UNIQUEIDENTIFIER NOT NULL,
    PhotoType NVARCHAR(50) NOT NULL, -- 'EventMedia' or 'LitterImage'
    FlaggedByUserId UNIQUEIDENTIFIER NOT NULL,
    FlagReason NVARCHAR(500) NOT NULL,
    FlaggedDate DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    ResolvedDate DATETIMEOFFSET NULL,
    ResolvedByUserId UNIQUEIDENTIFIER NULL,
    Resolution NVARCHAR(50) NULL -- 'Approved', 'Rejected', 'Dismissed'
);
```

**PhotoModerationLog audit table:**
```sql
CREATE TABLE PhotoModerationLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PhotoId UNIQUEIDENTIFIER NOT NULL,
    PhotoType NVARCHAR(50) NOT NULL,
    Action NVARCHAR(50) NOT NULL, -- 'Approved', 'Rejected', 'FlagDismissed'
    Reason NVARCHAR(500) NULL,
    PerformedByUserId UNIQUEIDENTIFIER NOT NULL,
    PerformedDate DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE()
);
```

### API Changes

**Admin endpoints:**
```csharp
[Authorize(Policy = "SiteAdmin")]
[HttpGet("api/admin/photos/pending")]
public async Task<ActionResult<PagedResult<PhotoModerationItem>>> GetPendingPhotos(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50)

[Authorize(Policy = "SiteAdmin")]
[HttpGet("api/admin/photos/flagged")]
public async Task<ActionResult<PagedResult<FlaggedPhotoItem>>> GetFlaggedPhotos(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50)

[Authorize(Policy = "SiteAdmin")]
[HttpPost("api/admin/photos/{photoType}/{id}/approve")]
public async Task<ActionResult> ApprovePhoto(
    string photoType,
    Guid id)

[Authorize(Policy = "SiteAdmin")]
[HttpPost("api/admin/photos/{photoType}/{id}/reject")]
public async Task<ActionResult> RejectPhoto(
    string photoType,
    Guid id,
    [FromBody] RejectPhotoRequest request)
```

**User flagging endpoint:**
```csharp
[Authorize]
[HttpPost("api/photos/{photoType}/{id}/flag")]
public async Task<ActionResult> FlagPhoto(
    string photoType,
    Guid id,
    [FromBody] FlagPhotoRequest request)
```

### Web UX Changes

**Site Admin > Photo Moderation page:**
- Location: `/siteadmin/photomoderation`
- Tabs: Pending Review, Flagged by Users, Recently Moderated
- Photo thumbnail grid (50 items per page) with quick actions
- Click thumbnail to open detail modal showing:
  - Full-size photo
  - Uploader info
  - Associated event or litter report context
  - Flag history (if flagged)
- Approve button (one-click)
- Reject button opens reason dropdown:
  - Inappropriate content
  - Spam/advertising
  - Copyright violation
  - Other (free text)

**Public photo views:**
- Add "Report Photo" option (flag icon or link)
- Opens modal with reason selection:
  - Inappropriate content
  - Spam/advertising
  - Copyright violation
  - Other (free text)
- Confirmation message after submission

### Email Templates

**Photo removal notification:**
- Subject: "Your photo has been removed from TrashMob"
- Body: Explains which photo was removed, the reason, and how to appeal
- Links to Terms of Service

---

## Implementation Phases

### Phase 1: Data Model & API
- Create database migrations
- Implement entity models in TrashMob.Models
- Create manager classes in TrashMob.Shared
- Implement API endpoints in TrashMob

### Phase 2: Admin UI
- Create PhotoModeration page component
- Implement photo grid with pagination
- Add detail modal with moderation actions
- Wire up API calls

### Phase 3: User-Facing & Notifications
- Add "Report Photo" to public photo views
- Implement email notification on rejection
- Test end-to-end workflow
- Update admin documentation

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Rollout Plan

1. Deploy admin moderation page (admin-only, internal testing)
2. Enable user flagging feature on web
3. Monitor moderation queue and adjust workflow as needed
4. Consider mobile app flagging support in future phase

---

## Open Questions

1. **Should photos be hidden immediately when flagged?**
   **Recommendation:** No, keep visible until admin decision to avoid abuse of flag system
   **Owner:** Product
   **Due:** Before Phase 3 starts

2. **How long to retain rejected photos?**
   **Recommendation:** 30 days for potential appeals, then permanently delete
   **Owner:** Legal/Product
   **Due:** Before Phase 1 starts

---

## Related Documents

- **[Project 18 - Before/After Photos](./Project_18_Before_After_Photos.md)** - Additional photo features
- **[TrashMob.Models PRD](../../TrashMob.Models/TrashMob.Models.prd)** - Domain model documentation
- **[Site Admin Layout](../../TrashMob/client-app/src/pages/siteadmin/_layout.tsx)** - Admin UI patterns

---

**Last Updated:** January 24, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** Q2 2026
