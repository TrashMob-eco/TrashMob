# Project 9 — TrashMob Teams

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for Review |
| **Priority** | High |
| **Risk** | Large |
| **Size** | Very Large |
| **Dependencies** | None |

---

## Business Rationale

Enable users to form and join teams for recurring cleanup efforts. Teams provide social cohesion, long-term engagement, friendly competition, and easier event organization. Many volunteers want to participate as part of a consistent group rather than as individuals.

---

## Objectives

### Primary Goals
- **Create and manage teams** with public/private visibility
- **Team membership** with join requests and invitations
- **Multiple team leads** for distributed management
- **Team metrics and impact** tracking (events, bags, weight, hours)
- **Team photo album** for showcasing work
- **Teams map** for discovery
- **Team event creation** and participation tracking

### Secondary Goals
- Team messaging/announcements
- Team-specific waivers
- Team sponsorships
- Team merchandise/branding

---

## Scope

### Phase 1 - Team Creation & Management
- ? Create team with name, description, location
- ? Public vs private teams
- ? Team leads can edit team details
- ? Upload team logo/photo
- ? Set team visibility and join rules

### Phase 2 - Membership
- ? Users can join public teams directly
- ? Private teams require approval
- ? Team leads can invite users
- ? Members can leave teams
- ? Leads can remove members
- ? Multiple leads per team

### Phase 3 - Team Events
- ? Teams can create events
- ? Link existing events to teams
- ? Track team participation at events
- ? Team event history and calendar

### Phase 4 - Team Metrics
- ? Total events by team
- ? Total bags collected
- ? Total weight collected (if tracked)
- ? Total hours volunteered
- ? Member count and growth
- ? Public leaderboards (if Project 20 complete)

### Phase 5 - Discovery & Social
- ? Teams map showing team locations
- ? Search and filter teams
- ? Team detail pages
- ? Photo album/gallery
- ? "My Teams" dashboard section

---

## Out-of-Scope

- ? Team chat/messaging (covered in Project 12)
- ? Team competitions (covered in Project 20)
- ? Team fundraising
- ? Cross-team collaborations (future)
- ? Team-specific merchandise store

---

## Success Metrics

### Quantitative
- **Teams created:** 100+ teams within 6 months
- **Team participation:** ? 30% of events have team participation
- **Team member retention:** ? 70% of team members attend 2+ team events
- **Average team size:** 5-15 members
- **Teams with recurring events:** ? 40%

### Qualitative
- Positive user feedback on team experience
- Increased volunteer retention through teams
- Community partners adopt teams for programs
- Success stories of impactful teams

---

## Dependencies

### Blockers
None - independent feature

### Enables
- **Project 11 (Adopt-A-Location):** Teams can adopt locations
- **Project 20 (Gamification):** Team leaderboards
- **Project 22 (Attendee Metrics):** Team-level roll-ups

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Inactive teams clutter system** | High | Medium | Auto-archive after 6 months inactivity; reactivation option |
| **Team name/content moderation** | Medium | Medium | Review queue for team creation; report function |
| **Complex permissions** | Medium | High | Clear role model; extensive testing; good UX |
| **Team disputes** | Low | Medium | Clear ToS; admin intervention process |

---

## Implementation Plan

### Data Model Changes

```sql
-- Teams table
CREATE TABLE Teams (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    LogoUrl NVARCHAR(500) NULL,
    IsPublic BIT NOT NULL DEFAULT 1,
    RequiresApproval BIT NOT NULL DEFAULT 0, -- For private teams
    Latitude DECIMAL(9,6) NULL,
    Longitude DECIMAL(9,6) NULL,
    City NVARCHAR(100) NULL,
    Region NVARCHAR(100) NULL,
    Country NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    LastUpdatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

-- Team membership
CREATE TABLE TeamMembers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TeamId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    IsTeamLead BIT NOT NULL DEFAULT 0,
    JoinedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE (TeamId, UserId)
);

-- Team join requests (for private teams)
CREATE TABLE TeamJoinRequests (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TeamId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RequestDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected
    ReviewedByUserId UNIQUEIDENTIFIER NULL,
    ReviewedDate DATETIMEOFFSET NULL,
    FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ReviewedByUserId) REFERENCES Users(Id)
);

-- Team events (linking events to teams)
CREATE TABLE TeamEvents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TeamId UNIQUEIDENTIFIER NOT NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE CASCADE,
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    UNIQUE (TeamId, EventId)
);

-- Team photos
CREATE TABLE TeamPhotos (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TeamId UNIQUEIDENTIFIER NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    Caption NVARCHAR(500) NULL,
    UploadedByUserId UNIQUEIDENTIFIER NOT NULL,
    UploadedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE CASCADE,
    FOREIGN KEY (UploadedByUserId) REFERENCES Users(Id)
);

-- Add TeamId to EventAttendees for tracking team participation
ALTER TABLE EventAttendees
ADD TeamId UNIQUEIDENTIFIER NULL,
    FOREIGN KEY (TeamId) REFERENCES Teams(Id);

CREATE INDEX IX_Teams_IsPublic_IsActive ON Teams(IsPublic, IsActive);
CREATE INDEX IX_TeamMembers_TeamId ON TeamMembers(TeamId);
CREATE INDEX IX_TeamMembers_UserId ON TeamMembers(UserId);
CREATE INDEX IX_TeamEvents_TeamId ON TeamEvents(TeamId);
CREATE INDEX IX_TeamEvents_EventId ON TeamEvents(EventId);
CREATE INDEX IX_EventAttendees_TeamId ON EventAttendees(TeamId);
```

### API Changes

```csharp
// Team CRUD
[HttpGet("api/teams")]
public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams([FromQuery] TeamFilterRequest filters)
{
    // Get public teams + teams user is member of
}

[HttpGet("api/teams/{id}")]
public async Task<ActionResult<TeamDto>> GetTeam(Guid id)
{
    // Get team details with member count, event count, metrics
}

[Authorize]
[HttpPost("api/teams")]
public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamRequest request)
{
    // Create team, add creator as lead
}

[Authorize]
[HttpPut("api/teams/{id}")]
public async Task<ActionResult<TeamDto>> UpdateTeam(Guid id, [FromBody] UpdateTeamRequest request)
{
    // Update team (leads only)
}

// Membership management
[Authorize]
[HttpPost("api/teams/{teamId}/join")]
public async Task<ActionResult> JoinTeam(Guid teamId)
{
    // Join public team or create join request for private
}

[Authorize]
[HttpPost("api/teams/{teamId}/members/{userId}")]
public async Task<ActionResult> AddMember(Guid teamId, Guid userId)
{
    // Invite user to team (leads only)
}

[Authorize]
[HttpDelete("api/teams/{teamId}/members/{userId}")]
public async Task<ActionResult> RemoveMember(Guid teamId, Guid userId)
{
    // Remove member (leads only) or leave team (self)
}

// Team events
[HttpGet("api/teams/{teamId}/events")]
public async Task<ActionResult<IEnumerable<EventDto>>> GetTeamEvents(Guid teamId)
{
    // Get events associated with team
}

[Authorize]
[HttpPost("api/teams/{teamId}/events/{eventId}")]
public async Task<ActionResult> LinkEventToTeam(Guid teamId, Guid eventId)
{
    // Associate event with team
}

// Team metrics
[HttpGet("api/teams/{teamId}/metrics")]
public async Task<ActionResult<TeamMetricsDto>> GetTeamMetrics(Guid teamId)
{
    // Return aggregated team statistics
}
```

### Web UX Changes

**New Pages:**

1. `/teams` - Teams Map & List
   - Map showing team locations
   - List/grid view toggle
   - Search and filters (location, public/private)

2. `/teams/create` - Create Team
   - Team name, description
   - Location picker
   - Public/private toggle
   - Logo upload

3. `/teams/{id}` - Team Detail Page
   - Team info and description
   - Member list with leads highlighted
   - Upcoming events
   - Past events and metrics
   - Photo gallery
   - Join button (if public) or Request to Join (if private)

4. `/teams/{id}/edit` - Edit Team (Leads Only)
   - Update team details
   - Manage members (invite, promote to lead, remove)
   - Approve join requests (if private)

5. `/teams/{id}/events/create` - Create Team Event
   - Pre-fill team affiliation
   - Standard event creation flow

6. `/dashboard` - Enhanced with "My Teams" section
   - Teams I'm a member of
   - Teams I lead
   - Quick actions (create event, view team)

**Components:**
```tsx
<TeamCard team={team} />
<TeamMembersList members={members} leads={leads} />
<TeamMetricsWidget metrics={metrics} />
<TeamEventsList events={events} />
<TeamPhotoGallery photos={photos} />
```

### Mobile App Changes

- Teams map and list
- Join teams
- View team details
- Create team events
- "My Teams" in dashboard

---

## Implementation Phases

### Phase 1: Core Infrastructure
- Database schema
- API endpoints
- Team creation and basic CRUD

### Phase 2: Membership Management
- Join/leave workflows
- Invitation system
- Private team approval
- Multiple leads

### Phase 3: Team Events
- Link events to teams
- Team event creation
- Track team participation
- Event history

### Phase 4: Discovery & Metrics
- Teams map
- Search and filters
- Team metrics calculation
- Photo gallery

### Phase 5: Dashboard Integration
- "My Teams" section
- Quick actions
- Notifications for team activity

**Note:** Phases can be worked on by different volunteers with coordination on shared components.

---

## Open Questions

1. **Maximum team size limit?**  
   **Recommendation:** No hard limit; soft warning at 50 members  
   **Owner:** Product Lead  
   **Due:** Before Phase 1

2. **Can users be on multiple teams?**  
   **Recommendation:** Yes, no limit  
   **Owner:** Product Lead  
   **Due:** Before Phase 1

3. **How to handle inactive teams?**  
   **Recommendation:** Auto-archive after 6 months no activity; can be reactivated  
   **Owner:** Product Lead  
   **Due:** Before Phase 4

4. **Team name moderation?**  
   **Recommendation:** Review queue for new teams; report function; admin override  
   **Owner:** Product + Admin  
   **Due:** Before Phase 1

5. **Can teams own multiple adopt-a-locations?**  
   **Recommendation:** Yes (covered in Project 11)  
   **Owner:** Product Lead  
   **Due:** When Project 11 starts

---

## Related Documents

- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** - Teams adopt locations
- **[Project 20 - Gamification](./Project_20_Gamification.md)** - Team leaderboards
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Team-level metrics

---

**Last Updated:** January 24, 2026  
**Owner:** Product Lead + Web Team  
**Status:** Ready for Review  
**Next Review:** When volunteer picks up work
