# Project 11 — Adopt-A-Location

| Attribute | Value |
|-----------|-------|
| **Status** | ✅ Complete |
| **Priority** | Medium |
| **Risk** | High |
| **Size** | Very Large |
| **Dependencies** | Project 9 (Teams), Project 10 (Community Pages) |

---

## Business Rationale

Model adoptable areas (roads, parks, trails) with availability and safety rules; manage team applications; reporting and reminders. This is a key differentiator for community partnerships and a revenue driver for city subscriptions.

---

## Objectives

### Primary Goals
- **Create and manage adoptable areas** with geographic boundaries
- **Team applications** for adoption with approval workflow
- **Public visibility rules** (show team name on signs, maps)
- **Delinquency reports and reminders** for teams not meeting commitments
- **Event linkage** to track cleanup activity

### Secondary Goals
- Integration with physical signage programs
- Automated certificate generation
- Multi-team shared adoptions
- Historical adoption records

---

## Scope

### Phase 1 - Adoptable Area Management
- ✅ Define adoptable areas with polygons or routes
- ✅ AI-assisted polygon lookup (user types "Central Park" → auto-fetch boundary from OSM/Azure Maps)
- ✅ Manual polygon editing/adjustment after auto-fetch
- ✅ Set availability (open, adopted, unavailable)
- ✅ Define cleanup frequency requirements
- ✅ Safety rules and guidelines per area
- ✅ Area categories (highway, park, trail, waterway, spot)

### Phase 2 - Team Applications
- ✅ Teams can browse available areas
- ✅ Submit adoption application
- ✅ Application review workflow
- ✅ Approval/rejection with notifications
- ✅ Adoption agreement acknowledgment

### Phase 3 - Adoption Management
- ✅ Track adoption start/end dates
- ✅ Link events to adopted areas
- ✅ Track compliance (events per period)
- ✅ Renewal reminders
- ✅ Termination workflow

### Phase 4 - Reporting
- ✅ Delinquency reports for community admins
- ✅ Team compliance dashboards
- ✅ Area activity reports
- ✅ Litter report correlation (count of litter reports within adopted area boundaries)
- ✅ Export for physical signage updates

---

## Out-of-Scope

- ❌ Physical sign manufacturing/installation
- ❌ Insurance/liability management
- ❌ Payment processing for adoption fees
- ❌ Integration with government GIS systems
- ❌ Route planning/optimization

---

## Success Metrics

### Quantitative
- **Areas defined:** 50+ areas in pilot communities
- **Adoption rate:** ≥ 70% of available areas adopted
- **Compliance rate:** ≥ 80% of teams meet cleanup frequency
- **Renewal rate:** ≥ 75% of adoptions renew

### Qualitative
- Community admins find area management intuitive
- Teams report clear understanding of commitments
- Reduction in delinquent adoptions

---

## Dependencies

### Blockers
- **Project 9 (Teams):** Teams are the adopting entity
- **Project 10 (Community Pages):** Communities define programs

### Enables
- Community subscription revenue
- Physical signage integration (future)
- Corporate sponsorship of areas

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Complex polygon/route editing** | High | Medium | AI-assisted lookup from OSM for known places; manual editing with vertex simplification |
| **Teams not meeting commitments** | Medium | High | Clear reminders; grace periods; easy termination |
| **Community-specific requirements** | High | Medium | Configurable rules per program; templates |
| **Liability concerns** | Medium | High | Clear agreements; waiver integration |

---

## Implementation Plan

### Data Model Changes

**New Entity: AdoptableArea**
```csharp
// New file: TrashMob.Models/AdoptableArea.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a geographic area available for team adoption (highway, park, trail, etc.).
    /// </summary>
    public class AdoptableArea : KeyedModel
    {
        /// <summary>
        /// Gets or sets the community program this area belongs to.
        /// </summary>
        public Guid CommunityProgramId { get; set; }

        /// <summary>
        /// Gets or sets the area name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the area description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of area (Highway, Park, Trail, Waterway, Street, Spot).
        /// </summary>
        public string AreaType { get; set; }

        #region Geographic Definition

        /// <summary>
        /// Gets or sets the GeoJSON representation of the area boundary/route.
        /// </summary>
        public string GeoJson { get; set; }

        /// <summary>
        /// Gets or sets the start point latitude (for linear areas).
        /// </summary>
        public double? StartLatitude { get; set; }

        /// <summary>
        /// Gets or sets the start point longitude (for linear areas).
        /// </summary>
        public double? StartLongitude { get; set; }

        /// <summary>
        /// Gets or sets the end point latitude (for linear areas).
        /// </summary>
        public double? EndLatitude { get; set; }

        /// <summary>
        /// Gets or sets the end point longitude (for linear areas).
        /// </summary>
        public double? EndLongitude { get; set; }

        #endregion

        #region Requirements

        /// <summary>
        /// Gets or sets how often cleanup is required (in days).
        /// </summary>
        public int CleanupFrequencyDays { get; set; } = 90;

        /// <summary>
        /// Gets or sets the minimum events required per year.
        /// </summary>
        public int MinEventsPerYear { get; set; } = 4;

        /// <summary>
        /// Gets or sets safety requirements and guidelines.
        /// </summary>
        public string SafetyRequirements { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the adoption status (Available, Adopted, Unavailable).
        /// </summary>
        public string Status { get; set; } = "Available";

        /// <summary>
        /// Gets or sets whether this area is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual CommunityProgram CommunityProgram { get; set; }
        public virtual ICollection<TeamAdoption> Adoptions { get; set; }
    }
}
```

**New Entity: TeamAdoption**
```csharp
// New file: TrashMob.Models/TeamAdoption.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a team's adoption of an area, including application and compliance tracking.
    /// </summary>
    public class TeamAdoption : KeyedModel
    {
        /// <summary>
        /// Gets or sets the adopting team identifier.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets the adopted area identifier.
        /// </summary>
        public Guid AdoptableAreaId { get; set; }

        #region Application

        /// <summary>
        /// Gets or sets the date of the adoption application.
        /// </summary>
        public DateTimeOffset ApplicationDate { get; set; }

        /// <summary>
        /// Gets or sets the application status (Pending, Approved, Rejected).
        /// </summary>
        public string ApplicationStatus { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the identifier of the user who reviewed the application.
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the application was reviewed.
        /// </summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets the reason for rejection (if applicable).
        /// </summary>
        public string RejectionReason { get; set; }

        #endregion

        #region Adoption Period

        /// <summary>
        /// Gets or sets the adoption start date.
        /// </summary>
        public DateOnly? AdoptionStartDate { get; set; }

        /// <summary>
        /// Gets or sets the adoption end date.
        /// </summary>
        public DateOnly? AdoptionEndDate { get; set; }

        #endregion

        #region Compliance Tracking

        /// <summary>
        /// Gets or sets the date of the last cleanup event.
        /// </summary>
        public DateOnly? LastEventDate { get; set; }

        /// <summary>
        /// Gets or sets the count of cleanup events for this adoption.
        /// </summary>
        public int EventCount { get; set; }

        /// <summary>
        /// Gets or sets whether the team is compliant with requirements.
        /// </summary>
        public bool IsCompliant { get; set; } = true;

        #endregion

        // Navigation properties
        public virtual Team Team { get; set; }
        public virtual AdoptableArea AdoptableArea { get; set; }
        public virtual User ReviewedByUser { get; set; }
        public virtual ICollection<TeamAdoptionEvent> AdoptionEvents { get; set; }
    }
}
```

**New Entity: TeamAdoptionEvent**
```csharp
// New file: TrashMob.Models/TeamAdoptionEvent.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Links a cleanup event to a team adoption for compliance tracking.
    /// </summary>
    public class TeamAdoptionEvent : KeyedModel
    {
        /// <summary>
        /// Gets or sets the team adoption identifier.
        /// </summary>
        public Guid TeamAdoptionId { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        // Navigation properties
        public virtual TeamAdoption TeamAdoption { get; set; }
        public virtual Event Event { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<AdoptableArea>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
    entity.Property(e => e.AreaType).HasMaxLength(50).IsRequired();
    entity.Property(e => e.Status).HasMaxLength(20);

    entity.HasOne(e => e.CommunityProgram)
        .WithMany(cp => cp.AdoptableAreas)
        .HasForeignKey(e => e.CommunityProgramId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasIndex(e => e.CommunityProgramId);
    entity.HasIndex(e => e.Status);
});

modelBuilder.Entity<TeamAdoption>(entity =>
{
    entity.Property(e => e.ApplicationStatus).HasMaxLength(20);
    entity.Property(e => e.RejectionReason).HasMaxLength(500);

    entity.HasOne(e => e.Team)
        .WithMany()
        .HasForeignKey(e => e.TeamId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.AdoptableArea)
        .WithMany(a => a.Adoptions)
        .HasForeignKey(e => e.AdoptableAreaId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.ReviewedByUser)
        .WithMany()
        .HasForeignKey(e => e.ReviewedByUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.TeamId);
    entity.HasIndex(e => e.AdoptableAreaId);
    entity.HasIndex(e => e.ApplicationStatus);
});

modelBuilder.Entity<TeamAdoptionEvent>(entity =>
{
    entity.HasOne(e => e.TeamAdoption)
        .WithMany(ta => ta.AdoptionEvents)
        .HasForeignKey(e => e.TeamAdoptionId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Event)
        .WithMany()
        .HasForeignKey(e => e.EventId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => new { e.TeamAdoptionId, e.EventId }).IsUnique();
});
```

### API Changes

```csharp
// Adoptable areas
[HttpGet("api/communities/{communityId}/programs/{programId}/areas")]
public async Task<ActionResult<IEnumerable<AdoptableAreaDto>>> GetAdoptableAreas(
    Guid communityId, Guid programId, [FromQuery] AreaFilter filter)
{
    // Get areas with availability status
}

[Authorize(Policy = "CommunityAdmin")]
[HttpPost("api/communities/{communityId}/programs/{programId}/areas")]
public async Task<ActionResult<AdoptableAreaDto>> CreateAdoptableArea(
    Guid communityId, Guid programId, [FromBody] CreateAreaRequest request)
{
    // Create new adoptable area
}

// Team adoption applications
[Authorize]
[HttpPost("api/areas/{areaId}/adopt")]
public async Task<ActionResult<TeamAdoptionDto>> ApplyForAdoption(
    Guid areaId, [FromBody] AdoptionApplicationRequest request)
{
    // Submit adoption application for team
}

[Authorize(Policy = "CommunityAdmin")]
[HttpPut("api/adoptions/{adoptionId}/review")]
public async Task<ActionResult> ReviewAdoption(
    Guid adoptionId, [FromBody] AdoptionReviewRequest request)
{
    // Approve or reject adoption
}

// Adoption management
[HttpGet("api/teams/{teamId}/adoptions")]
public async Task<ActionResult<IEnumerable<TeamAdoptionDto>>> GetTeamAdoptions(Guid teamId)
{
    // Get team's adoptions with compliance status
}

[Authorize]
[HttpPost("api/adoptions/{adoptionId}/events/{eventId}")]
public async Task<ActionResult> LinkEventToAdoption(Guid adoptionId, Guid eventId)
{
    // Link completed event to adoption
}

// Reports
[Authorize(Policy = "CommunityAdmin")]
[HttpGet("api/communities/{communityId}/adoptions/delinquent")]
public async Task<ActionResult<IEnumerable<DelinquentAdoptionDto>>> GetDelinquentAdoptions(Guid communityId)
{
    // Get adoptions not meeting requirements
}
```

### Web UX Changes

**New Pages:**

1. `/communities/{id}/adopt` - Browse Adoptable Areas
   - Map showing available areas
   - Filter by area type, availability
   - Apply for adoption button

2. `/areas/{id}` - Area Detail Page
   - Area description and map
   - Requirements and safety info
   - Current adopting team (if adopted)
   - Apply button (if available)

3. `/communities/{id}/admin/areas` - Manage Adoptable Areas
   - List of all areas
   - Create/edit areas
   - View adoption applications
   - Compliance dashboard

4. `/teams/{id}/adoptions` - Team's Adopted Areas
   - List of adopted areas
   - Compliance status
   - Link events to adoptions
   - Renewal reminders

### Mobile App Changes

- Browse adoptable areas on map
- View adoption details
- Link events to adoptions
- Compliance notifications

---

## Implementation Phases

### Phase 1: Area Definition
- Database schema
- Admin UI for creating areas
- AI-assisted polygon lookup (see below)
- Basic area listing

**AI-Assisted Polygon Lookup:**
The area creation UI includes a "Search for location" feature that auto-fetches boundaries:

1. **User Flow:**
   - Admin types location name (e.g., "Central Park, New York")
   - System queries OpenStreetMap Nominatim API with `polygon_geojson=1`
   - If boundary found, polygon is rendered on map
   - Admin can adjust vertices if needed, then save
   - If no boundary found, admin draws polygon manually

2. **Technical Implementation:**
   ```typescript
   // Frontend service for polygon lookup
   async function fetchLocationBoundary(query: string): Promise<GeoJSON | null> {
     // Primary: OpenStreetMap Nominatim (free, good park/trail coverage)
     const osmUrl = `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&polygon_geojson=1`;
     const response = await fetch(osmUrl);
     const results = await response.json();

     if (results[0]?.geojson) {
       return results[0].geojson;
     }

     // Fallback: Azure Maps (already used by TrashMob)
     // Returns point, but could be combined with radius for simple areas
     return null;
   }
   ```

3. **Data Sources:**
   - **Parks/Trails:** OpenStreetMap has excellent coverage
   - **Highways:** May need manual definition (OSM has routes but not "adoptable segments")
   - **Waterways:** OSM has good coverage for rivers, streams, lake shores

### Phase 2: Applications
- Application workflow
- Review process
- Notifications

### Phase 3: Compliance Tracking
- Event linkage
- Compliance calculation
- Delinquency reports

### Phase 4: Public Features
- Public adoption display
- Team dashboards
- Renewal workflow

**Note:** Requires Project 9 (Teams) and Project 10 (Community Pages) completion.

---

## Open Questions

1. ~~**Standard adoption period length?**~~
   **Decision:** Community-configurable; community admin selects from pick list (1, 2, 3, 4, or 5 years) when creating the program
   **Status:** ✅ Resolved

2. ~~**Grace period for delinquency?**~~
   **Decision:** 30 days past due before escalation to community admin
   **Status:** ✅ Resolved

3. ~~**Can areas be co-adopted by multiple teams?**~~
   **Decision:** Yes, as configuration option per area (community admin sets when creating the area)
   **Status:** ✅ Resolved

4. ~~**Integration with physical signage?**~~
   **Decision:** Future feature after community feedback; not in initial release. When implemented, may need to track: sign content, order date, posting date, removal date, inspection due date
   **Status:** ✅ Resolved (deferred)

5. ~~**How are events linked to adoptions?**~~
   **Decision:** Manual at event creation time; if the event lead's team has adopted areas, show a dropdown to optionally select which adopted area this event is for. Selection is optional (events can exist without adoption linkage).
   **Status:** ✅ Resolved

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2233](https://github.com/trashmob/TrashMob/issues/2233)** - Project 11: Adopt-a-Location (tracking issue)

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Adopting entity
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Program management
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Tracking impact

---

**Last Updated:** January 31, 2026
**Owner:** Product Lead + Web Team
**Status:** Planning in Progress
**Next Review:** When Project 9 & 10 near completion
