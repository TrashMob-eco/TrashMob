# Project 18 — Before & After Event Pictures

| Attribute | Value |
|-----------|-------|
| **Status** | Phase 4 Complete |
| **Priority** | Low |
| **Risk** | Moderate |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Empower leads to document impact visually with before and after photos. Visual proof of cleanup impact is compelling for volunteers, sponsors, and communities. Requires admin moderation and TOS enforcement.

---

## Objectives

### Primary Goals
- **Upload and manage photos** for events
- **Mark photos as before/after** for comparison
- **Admin moderation queue** for inappropriate content
- **Notification to uploader** on removal

### Secondary Goals
- Photo galleries on event pages
- Community/team photo albums
- Photo sharing to social media
- AI-powered before/after matching

---

## Scope

### Phase 1 - Photo Upload ✅ Complete
- ✅ Upload photos during/after event (API ready)
- ✅ Associate photos with events (EventPhoto model)
- ✅ Mark as before, during, or after (EventPhotoType enum)
- ✅ Add captions/descriptions (Caption field)
- ✅ Frontend upload UI (EventPhotoUploader component)

### Phase 2 - Display ✅ Complete
- ✅ Photo gallery on event detail page (EventPhotoGallery component)
- ⏳ Before/after comparison view (future enhancement)
- ✅ Photo lightbox/viewer (Dialog-based with navigation)
- ⏳ Download original images (future enhancement)

### Phase 3 - Moderation ✅ Complete
- ✅ Report inappropriate photo (FlagPhoto API)
- ✅ Admin moderation queue (PhotoModerationManager supports all photo types)
- ✅ Remove photo with notification (email notification on reject)
- ⏳ Appeal process (not needed per user request)

### Phase 4 - Albums ✅ Complete
- ✅ Team photo albums (TeamPhoto feature)
- ✅ Community photo galleries (PartnerPhoto feature)
- ⏳ Featured photos on home page (moved to Project 2 - Home Page Redesign)

---

## Out-of-Scope

- ❌ Video upload/hosting
- ❌ Photo editing tools
- ❌ Automatic face blurring
- ❌ Geotagging from EXIF (privacy)
- ❌ Professional photography booking

---

## Success Metrics

### Quantitative
- **Events with photos:** ≥ 50% of completed events
- **Before/after pairs:** ≥ 30% of events with photos
- **Moderation volume:** < 1% of photos flagged
- **Removal appeals:** < 10% of removals appealed

### Qualitative
- Photos enhance event storytelling
- No inappropriate content incidents
- Positive volunteer feedback

---

## Dependencies

### Blockers
None - independent feature

### Enables
- **Project 20 (Gamification):** Photo-based achievements
- **Marketing:** Visual content for campaigns
- **Sponsors:** Impact documentation

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Inappropriate content** | Medium | High | Moderation queue; report function; clear ToS |
| **Storage costs** | Medium | Medium | Image compression; size limits; CDN caching |
| **Privacy (people in photos)** | Medium | High | Clear photo policy; blur request process |
| **Copyright issues** | Low | Medium | Uploader agreement; DMCA process |

---

## Implementation Plan

### Data Model Changes

**New Entity: EventPhoto**
```csharp
// New file: TrashMob.Models/EventPhoto.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a photo uploaded for an event (before, during, or after).
    /// </summary>
    public class EventPhoto : KeyedModel
    {
        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the uploader's user identifier.
        /// </summary>
        public Guid UploadedByUserId { get; set; }

        #region Image Storage

        /// <summary>
        /// Gets or sets the URL of the full-size image.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL of the thumbnail image.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        #endregion

        #region Metadata

        /// <summary>
        /// Gets or sets the photo type (Before, During, After).
        /// </summary>
        public string PhotoType { get; set; } = "During";

        /// <summary>
        /// Gets or sets the photo caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets when the photo was taken.
        /// </summary>
        public DateTimeOffset? TakenAt { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the status (Active, Flagged, Removed).
        /// </summary>
        public string Status { get; set; } = "Active";

        // Navigation properties
        public virtual Event Event { get; set; }
        public virtual User UploadedByUser { get; set; }
        public virtual ICollection<PhotoReport> Reports { get; set; }
    }
}
```

**New Entity: PhotoReport**
```csharp
// New file: TrashMob.Models/PhotoReport.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user report of an inappropriate photo.
    /// </summary>
    public class PhotoReport : KeyedModel
    {
        /// <summary>
        /// Gets or sets the reported photo identifier.
        /// </summary>
        public Guid PhotoId { get; set; }

        /// <summary>
        /// Gets or sets the reporting user's identifier.
        /// </summary>
        public Guid ReporterUserId { get; set; }

        /// <summary>
        /// Gets or sets the reason (Inappropriate, Copyright, Privacy, Other).
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets additional description of the issue.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets when the report was submitted.
        /// </summary>
        public DateTimeOffset ReportDate { get; set; }

        #region Review

        /// <summary>
        /// Gets or sets the reviewing admin's user identifier.
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the report was reviewed.
        /// </summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets the outcome (Dismissed, Removed, Warning).
        /// </summary>
        public string ReviewOutcome { get; set; }

        /// <summary>
        /// Gets or sets notes from the reviewer.
        /// </summary>
        public string ReviewNotes { get; set; }

        #endregion

        // Navigation properties
        public virtual EventPhoto Photo { get; set; }
        public virtual User ReporterUser { get; set; }
        public virtual User ReviewedByUser { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<EventPhoto>(entity =>
{
    entity.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
    entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
    entity.Property(e => e.PhotoType).HasMaxLength(20);
    entity.Property(e => e.Caption).HasMaxLength(500);
    entity.Property(e => e.Status).HasMaxLength(20);

    entity.HasOne(e => e.Event)
        .WithMany()
        .HasForeignKey(e => e.EventId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.UploadedByUser)
        .WithMany()
        .HasForeignKey(e => e.UploadedByUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.EventId);
    entity.HasIndex(e => e.Status);
    entity.HasIndex(e => e.PhotoType);
});

modelBuilder.Entity<PhotoReport>(entity =>
{
    entity.Property(e => e.Reason).HasMaxLength(50).IsRequired();
    entity.Property(e => e.Description).HasMaxLength(500);
    entity.Property(e => e.ReviewOutcome).HasMaxLength(50);
    entity.Property(e => e.ReviewNotes).HasMaxLength(500);

    entity.HasOne(e => e.Photo)
        .WithMany(p => p.Reports)
        .HasForeignKey(e => e.PhotoId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.ReporterUser)
        .WithMany()
        .HasForeignKey(e => e.ReporterUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.ReviewedByUser)
        .WithMany()
        .HasForeignKey(e => e.ReviewedByUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.ReviewedDate)
        .HasFilter("[ReviewedDate] IS NULL");
});
```

### API Changes

```csharp
// Upload photo
[Authorize]
[HttpPost("api/events/{eventId}/photos")]
public async Task<ActionResult<EventPhotoDto>> UploadPhoto(
    Guid eventId, [FromForm] UploadPhotoRequest request)
{
    // Validate user is attendee/lead
    // Process and store image
    // Generate thumbnail
}

// Get event photos
[HttpGet("api/events/{eventId}/photos")]
public async Task<ActionResult<IEnumerable<EventPhotoDto>>> GetEventPhotos(
    Guid eventId, [FromQuery] PhotoFilter filter)
{
    // Return photos (Active status only)
}

// Update photo metadata
[Authorize]
[HttpPut("api/photos/{photoId}")]
public async Task<ActionResult<EventPhotoDto>> UpdatePhoto(
    Guid photoId, [FromBody] UpdatePhotoRequest request)
{
    // Validate ownership
    // Update caption, type
}

// Delete photo
[Authorize]
[HttpDelete("api/photos/{photoId}")]
public async Task<ActionResult> DeletePhoto(Guid photoId)
{
    // Validate ownership or admin
    // Soft delete
}

// Report photo
[Authorize]
[HttpPost("api/photos/{photoId}/report")]
public async Task<ActionResult> ReportPhoto(
    Guid photoId, [FromBody] ReportPhotoRequest request)
{
    // Create report, flag photo if threshold reached
}

// Admin moderation
[Authorize(Roles = "SiteAdmin")]
[HttpGet("api/admin/photos/flagged")]
public async Task<ActionResult<IEnumerable<FlaggedPhotoDto>>> GetFlaggedPhotos()
{
    // Get photos pending review
}

[Authorize(Roles = "SiteAdmin")]
[HttpPut("api/admin/photos/{photoId}/review")]
public async Task<ActionResult> ReviewPhoto(
    Guid photoId, [FromBody] ReviewPhotoRequest request)
{
    // Process review, notify uploader if removed
}
```

### Web UX Changes

**Event Photo Upload:**
- Upload button on event page (attendees/leads)
- Drag-and-drop interface
- Photo type selector (before/during/after)
- Caption input
- Progress indicator

**Event Photo Gallery:**
- Grid view of all photos
- Filter by type
- Lightbox viewer
- Before/after comparison slider
- Download button

**Admin Moderation:**
- Flagged photos queue
- Photo details with reports
- Approve/remove actions
- Notification preview

### Mobile App Changes

- Camera integration for direct upload
- Photo gallery viewer
- Before/after marking
- Report function

---

## Implementation Phases

### Phase 1: Upload & Storage
- Azure Blob storage setup
- Upload API
- Thumbnail generation
- Basic display

### Phase 2: Gallery & Display
- Photo gallery component
- Before/after comparison
- Lightbox viewer

### Phase 3: Moderation
- Report function
- Admin queue
- Removal workflow
- Notifications

### Phase 4: Extensions
- Team/community albums
- Featured photos
- Social sharing

---

## Photo Storage

**Azure Blob Storage:**
```
trashmob-photos/
├── events/
│   └── {eventId}/
│       ├── originals/
│       │   └── {photoId}.jpg
│       └── thumbnails/
│           └── {photoId}_thumb.jpg
```

**Image Processing:**
- Resize originals to max 2000px
- Generate 400px thumbnails
- Strip EXIF location data
- Compress for web delivery

---

## Open Questions

1. ~~**Photo size limits?**~~
   **Decision:** 10MB per photo; 20 photos per event - balances quality and storage costs
   **Status:** ✅ Resolved

2. ~~**Automatic face detection/blur?**~~
   **Decision:** Not for v1; consider Azure Cognitive Services in future version if needed
   **Status:** ✅ Resolved

3. ~~**Photo retention policy?**~~
   **Decision:** Keep indefinitely unless manually removed by user or admin
   **Status:** ✅ Resolved

4. ~~**Copyright assignment?**~~
   **Decision:** License to TrashMob in ToS; uploader retains ownership but grants TrashMob rights to use photos for marketing and platform display
   **Status:** ✅ Resolved

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#202](https://github.com/trashmob/TrashMob/issues/202)** - Project 18: Allow event creators to post "before" and "after" photos (tracking issue)
- **[#1170](https://github.com/trashmob/TrashMob/issues/1170)** - Website: Allow photo uploads for pickup location

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Team photo galleries
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community photos
- **[Project 14 - Social Media](./Project_14_Social_Media.md)** - Photo sharing

---

**Last Updated:** February 3, 2026
**Owner:** Web Team
**Status:** Phase 4 Complete
**Next Review:** Project complete - no further phases planned
