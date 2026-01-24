# Project 18 — Before & After Event Pictures

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in Progress |
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

### Phase 1 - Photo Upload
- ✅ Upload photos during/after event
- ✅ Associate photos with events
- ✅ Mark as before, during, or after
- ✅ Add captions/descriptions

### Phase 2 - Display
- ✅ Photo gallery on event detail page
- ✅ Before/after comparison view
- ✅ Photo lightbox/viewer
- ✅ Download original images

### Phase 3 - Moderation
- ✅ Report inappropriate photo
- ✅ Admin moderation queue
- ✅ Remove photo with notification
- ✅ Appeal process

### Phase 4 - Albums
- ❓ Team photo albums
- ❓ Community photo galleries
- ❓ Featured photos on home page

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

```sql
-- Event photos
CREATE TABLE EventPhotos (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    UploadedByUserId UNIQUEIDENTIFIER NOT NULL,
    -- Image storage
    ImageUrl NVARCHAR(500) NOT NULL,
    ThumbnailUrl NVARCHAR(500) NULL,
    -- Metadata
    PhotoType NVARCHAR(20) NOT NULL DEFAULT 'During', -- Before, During, After
    Caption NVARCHAR(500) NULL,
    TakenAt DATETIMEOFFSET NULL,
    -- Status
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Flagged, Removed
    -- Audit
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    FOREIGN KEY (UploadedByUserId) REFERENCES Users(Id)
);

-- Photo reports
CREATE TABLE PhotoReports (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PhotoId UNIQUEIDENTIFIER NOT NULL,
    ReporterUserId UNIQUEIDENTIFIER NOT NULL,
    Reason NVARCHAR(50) NOT NULL, -- Inappropriate, Copyright, Privacy, Other
    Description NVARCHAR(500) NULL,
    ReportDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    -- Review
    ReviewedByUserId UNIQUEIDENTIFIER NULL,
    ReviewedDate DATETIMEOFFSET NULL,
    ReviewOutcome NVARCHAR(50) NULL, -- Dismissed, Removed, Warning
    ReviewNotes NVARCHAR(500) NULL,
    FOREIGN KEY (PhotoId) REFERENCES EventPhotos(Id),
    FOREIGN KEY (ReporterUserId) REFERENCES Users(Id),
    FOREIGN KEY (ReviewedByUserId) REFERENCES Users(Id)
);

CREATE INDEX IX_EventPhotos_EventId ON EventPhotos(EventId);
CREATE INDEX IX_EventPhotos_Status ON EventPhotos(Status);
CREATE INDEX IX_EventPhotos_PhotoType ON EventPhotos(PhotoType);
CREATE INDEX IX_PhotoReports_ReviewedDate ON PhotoReports(ReviewedDate) WHERE ReviewedDate IS NULL;
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

1. **Photo size limits?**
   **Recommendation:** 10MB per photo; 20 photos per event
   **Owner:** Engineering
   **Due:** Before Phase 1

2. **Automatic face detection/blur?**
   **Recommendation:** Not for v1; consider Azure Cognitive Services later
   **Owner:** Product Lead
   **Due:** Future

3. **Photo retention policy?**
   **Recommendation:** Keep indefinitely unless removed
   **Owner:** Legal + Product
   **Due:** Before Phase 1

4. **Copyright assignment?**
   **Recommendation:** License to TrashMob in ToS; uploader retains ownership
   **Owner:** Legal
   **Due:** Before Phase 1

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Team photo galleries
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community photos
- **[Project 14 - Social Media](./Project_14_Social_Media.md)** - Photo sharing

---

**Last Updated:** January 24, 2026
**Owner:** Web Team
**Status:** Planning in Progress
**Next Review:** When prioritized
