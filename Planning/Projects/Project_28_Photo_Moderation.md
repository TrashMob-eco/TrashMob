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

**Add moderation properties to existing photo entities:**
```csharp
// Add to existing EventMedia and LitterImage entities (or create a shared interface)
#region Moderation Properties

/// <summary>
/// Gets or sets the moderation status (Pending, Approved, Rejected).
/// </summary>
public PhotoModerationStatus ModerationStatus { get; set; } = PhotoModerationStatus.Pending;

/// <summary>
/// Gets or sets the moderating admin's user identifier.
/// </summary>
public Guid? ModeratedByUserId { get; set; }

/// <summary>
/// Gets or sets when the photo was moderated.
/// </summary>
public DateTimeOffset? ModeratedDate { get; set; }

/// <summary>
/// Gets or sets the reason for moderation decision.
/// </summary>
public string ModerationReason { get; set; }

#endregion

// Navigation property
public virtual User ModeratedByUser { get; set; }
```

**New Entity: PhotoFlag**
```csharp
// New file: TrashMob.Models/PhotoFlag.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user flag/report of an inappropriate photo.
    /// </summary>
    public class PhotoFlag : KeyedModel
    {
        /// <summary>
        /// Gets or sets the photo identifier.
        /// </summary>
        public Guid PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the photo type (EventMedia or LitterImage).
        /// </summary>
        public string PhotoType { get; set; }

        /// <summary>
        /// Gets or sets the flagging user's identifier.
        /// </summary>
        public Guid FlaggedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the reason for flagging.
        /// </summary>
        public string FlagReason { get; set; }

        /// <summary>
        /// Gets or sets when the photo was flagged.
        /// </summary>
        public DateTimeOffset FlaggedDate { get; set; }

        /// <summary>
        /// Gets or sets when the flag was resolved.
        /// </summary>
        public DateTimeOffset? ResolvedDate { get; set; }

        /// <summary>
        /// Gets or sets the resolving admin's user identifier.
        /// </summary>
        public Guid? ResolvedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the resolution (Approved, Rejected, Dismissed).
        /// </summary>
        public string Resolution { get; set; }

        // Navigation properties
        public virtual User FlaggedByUser { get; set; }
        public virtual User ResolvedByUser { get; set; }
    }
}
```

**New Entity: PhotoModerationLog**
```csharp
// New file: TrashMob.Models/PhotoModerationLog.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Audit log entry for photo moderation actions.
    /// </summary>
    public class PhotoModerationLog : KeyedModel
    {
        /// <summary>
        /// Gets or sets the photo identifier.
        /// </summary>
        public Guid PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the photo type (EventMedia or LitterImage).
        /// </summary>
        public string PhotoType { get; set; }

        /// <summary>
        /// Gets or sets the action taken (Approved, Rejected, FlagDismissed).
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the reason for the action.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the admin who performed the action.
        /// </summary>
        public Guid PerformedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the action was performed.
        /// </summary>
        public DateTimeOffset PerformedDate { get; set; }

        // Navigation property
        public virtual User PerformedByUser { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<PhotoFlag>(entity =>
{
    entity.Property(e => e.PhotoType).HasMaxLength(50).IsRequired();
    entity.Property(e => e.FlagReason).HasMaxLength(500).IsRequired();
    entity.Property(e => e.Resolution).HasMaxLength(50);

    entity.HasOne(e => e.FlaggedByUser)
        .WithMany()
        .HasForeignKey(e => e.FlaggedByUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.ResolvedByUser)
        .WithMany()
        .HasForeignKey(e => e.ResolvedByUserId)
        .OnDelete(DeleteBehavior.NoAction);
});

modelBuilder.Entity<PhotoModerationLog>(entity =>
{
    entity.Property(e => e.PhotoType).HasMaxLength(50).IsRequired();
    entity.Property(e => e.Action).HasMaxLength(50).IsRequired();
    entity.Property(e => e.Reason).HasMaxLength(500);

    entity.HasOne(e => e.PerformedByUser)
        .WithMany()
        .HasForeignKey(e => e.PerformedByUserId)
        .OnDelete(DeleteBehavior.NoAction);
});
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
