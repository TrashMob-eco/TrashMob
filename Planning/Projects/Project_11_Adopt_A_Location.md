# Project 11 — Adopt-A-Location

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in Progress |
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
- ✅ Set availability (open, adopted, unavailable)
- ✅ Define cleanup frequency requirements
- ✅ Safety rules and guidelines per area
- ✅ Area categories (highway, park, trail, waterway)

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
| **Complex polygon/route editing** | High | Medium | Use existing mapping libraries; simplify to key points |
| **Teams not meeting commitments** | Medium | High | Clear reminders; grace periods; easy termination |
| **Community-specific requirements** | High | Medium | Configurable rules per program; templates |
| **Liability concerns** | Medium | High | Clear agreements; waiver integration |

---

## Implementation Plan

### Data Model Changes

```sql
-- Adoptable areas (defined by communities)
CREATE TABLE AdoptableAreas (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CommunityProgramId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    AreaType NVARCHAR(50) NOT NULL, -- Highway, Park, Trail, Waterway, Street
    -- Geographic definition (simplified to route/polygon points)
    GeoJson NVARCHAR(MAX) NULL, -- GeoJSON for polygon/line
    StartLatitude DECIMAL(9,6) NULL,
    StartLongitude DECIMAL(9,6) NULL,
    EndLatitude DECIMAL(9,6) NULL,
    EndLongitude DECIMAL(9,6) NULL,
    -- Requirements
    CleanupFrequencyDays INT NOT NULL DEFAULT 90, -- e.g., every 90 days
    MinEventsPerYear INT NOT NULL DEFAULT 4,
    SafetyRequirements NVARCHAR(MAX) NULL,
    -- Status
    Status NVARCHAR(20) NOT NULL DEFAULT 'Available', -- Available, Adopted, Unavailable
    IsActive BIT NOT NULL DEFAULT 1,
    -- Audit
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    LastUpdatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (CommunityProgramId) REFERENCES CommunityPrograms(Id),
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

-- Team adoptions
CREATE TABLE TeamAdoptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TeamId UNIQUEIDENTIFIER NOT NULL,
    AdoptableAreaId UNIQUEIDENTIFIER NOT NULL,
    -- Application
    ApplicationDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    ApplicationStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected
    ReviewedByUserId UNIQUEIDENTIFIER NULL,
    ReviewedDate DATETIMEOFFSET NULL,
    RejectionReason NVARCHAR(500) NULL,
    -- Adoption period
    AdoptionStartDate DATE NULL,
    AdoptionEndDate DATE NULL,
    -- Compliance
    LastEventDate DATE NULL,
    EventCount INT NOT NULL DEFAULT 0,
    IsCompliant BIT NOT NULL DEFAULT 1,
    -- Audit
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (TeamId) REFERENCES Teams(Id),
    FOREIGN KEY (AdoptableAreaId) REFERENCES AdoptableAreas(Id),
    FOREIGN KEY (ReviewedByUserId) REFERENCES Users(Id)
);

-- Link events to adoptions
CREATE TABLE TeamAdoptionEvents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TeamAdoptionId UNIQUEIDENTIFIER NOT NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (TeamAdoptionId) REFERENCES TeamAdoptions(Id),
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    UNIQUE (TeamAdoptionId, EventId)
);

CREATE INDEX IX_AdoptableAreas_CommunityProgramId ON AdoptableAreas(CommunityProgramId);
CREATE INDEX IX_AdoptableAreas_Status ON AdoptableAreas(Status);
CREATE INDEX IX_TeamAdoptions_TeamId ON TeamAdoptions(TeamId);
CREATE INDEX IX_TeamAdoptions_AdoptableAreaId ON TeamAdoptions(AdoptableAreaId);
CREATE INDEX IX_TeamAdoptions_ApplicationStatus ON TeamAdoptions(ApplicationStatus);
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
- Basic area listing

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

1. **Standard adoption period length?**
   **Recommendation:** 1 year with auto-renewal option
   **Owner:** Product Lead
   **Due:** Before Phase 2

2. **Grace period for delinquency?**
   **Recommendation:** 30 days past due before escalation
   **Owner:** Product Lead
   **Due:** Before Phase 3

3. **Can areas be co-adopted by multiple teams?**
   **Recommendation:** Yes, as configuration option per area
   **Owner:** Product Lead
   **Due:** Before Phase 2

4. **Integration with physical signage?**
   **Recommendation:** Export capability only; physical process manual
   **Owner:** Business Team
   **Due:** Before pilot

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Adopting entity
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Program management
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Tracking impact

---

**Last Updated:** January 24, 2026
**Owner:** Product Lead + Web Team
**Status:** Planning in Progress
**Next Review:** When Project 9 & 10 near completion
