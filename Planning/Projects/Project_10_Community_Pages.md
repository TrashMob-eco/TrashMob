# Project 10 — Community Pages

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in Progress |
| **Priority** | High |
| **Risk** | Very High |
| **Size** | Very Large |
| **Dependencies** | Project 1 (Auth), Project 8 (Waivers), Project 9 (Teams), Project 16 (CMS) |

---

## Business Rationale

Branded public pages for partner communities (cities, counties, organizations) with metrics, photos, contact info, SSO, and adopt-a programs. Community Pages are a key revenue driver as cities can subscribe for branded presence and program management tools.

---

## Objectives

### Primary Goals
- **Community discovery map** showing all partnered communities
- **Branded public home pages** for each community
- **Admin management** with SSO for community staff
- **Metrics & notifications** for community stakeholders
- **Opt-in to adopt-a programs** integration

### Secondary Goals
- Custom community branding (colors, logos)
- Community-specific waivers
- Community newsletters
- Sponsorship display areas

---

## Scope

### Phase 1 - Community Discovery
- ✅ Map showing community locations
- ✅ List view with search/filter
- ✅ Basic community detail pages
- ✅ Link from partner pages

### Phase 2 - Community Home Pages
- ✅ Branded header with logo
- ✅ About section (CMS-editable)
- ✅ Contact information
- ✅ Events in this community
- ✅ Teams in this community
- ✅ Impact metrics display

### Phase 3 - Community Admin
- ✅ SSO integration for community staff
- ✅ Admin dashboard for community
- ✅ Manage community content
- ✅ View community metrics and reports
- ✅ Download reports (CSV, PDF)

### Phase 4 - Programs Integration
- ✅ Opt-in to Adopt-A-Location program
- ✅ Manage adoptable areas
- ✅ Review team applications
- ✅ Community-specific events

---

## Out-of-Scope

- ❌ Billing/subscription management (manual for 2026)
- ❌ Community-specific mobile apps
- ❌ Multi-language community pages
- ❌ Community forums/discussion boards
- ❌ Custom domain support (future)

---

## Success Metrics

### Quantitative
- **Paid communities onboarded:** N communities
- **Community page views:** Track engagement
- **Renewal rate:** ≥ P%
- **Events created through community pages:** Track attribution
- **Teams formed within communities:** Track growth

### Qualitative
- Community administrators find system easy to use
- Communities report increased volunteer engagement
- Positive feedback from community stakeholders

---

## Dependencies

### Blockers
- **Project 1 (Auth Revamp):** SSO for community admins
- **Project 8 (Waivers V3):** Community-specific waivers
- **Project 9 (Teams):** Teams associated with communities
- **Project 16 (CMS):** Editable community content

### Enables
- **Project 11 (Adopt-A-Location):** Communities define adoptable areas
- **Subscription billing:** Future monetization

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Complex SSO integration** | High | High | Partner with Entra team; clear requirements |
| **Scope creep from communities** | High | Medium | Fixed feature set for v1; roadmap for v2 |
| **Low initial adoption** | Medium | High | Pilot with 3-5 communities; iterate on feedback |
| **Performance with many communities** | Low | Medium | Caching; pagination; CDN for assets |

---

## Implementation Plan

### Data Model Changes

```sql
-- Add home page support to Partners (Communities are Partners)
ALTER TABLE Partners
ADD HomePageStartDate DATETIMEOFFSET NULL,
    HomePageEndDate DATETIMEOFFSET NULL,
    HomePageEnabled BIT NOT NULL DEFAULT 0,
    BrandingPrimaryColor NVARCHAR(7) NULL,
    BrandingSecondaryColor NVARCHAR(7) NULL,
    BannerImageUrl NVARCHAR(500) NULL;

-- Community program types (Adopt-A-Highway, Adopt-A-Park, etc.)
CREATE TABLE CommunityProgramTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Community programs
CREATE TABLE CommunityPrograms (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PartnerId UNIQUEIDENTIFIER NOT NULL,
    ProgramTypeId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (PartnerId) REFERENCES Partners(Id),
    FOREIGN KEY (ProgramTypeId) REFERENCES CommunityProgramTypes(Id),
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

CREATE INDEX IX_Partners_HomePageEnabled ON Partners(HomePageEnabled) WHERE HomePageEnabled = 1;
CREATE INDEX IX_CommunityPrograms_PartnerId ON CommunityPrograms(PartnerId);
```

### API Changes

```csharp
// Community discovery
[HttpGet("api/communities")]
public async Task<ActionResult<IEnumerable<CommunityDto>>> GetCommunities([FromQuery] CommunityFilter filter)
{
    // Get partners with HomePageEnabled = true
}

[HttpGet("api/communities/{id}")]
public async Task<ActionResult<CommunityDetailDto>> GetCommunity(Guid id)
{
    // Get community details with metrics, events, teams
}

// Community admin (requires community admin role)
[Authorize(Policy = "CommunityAdmin")]
[HttpGet("api/communities/{id}/admin")]
public async Task<ActionResult<CommunityAdminDto>> GetCommunityAdmin(Guid id)
{
    // Admin dashboard data
}

[Authorize(Policy = "CommunityAdmin")]
[HttpGet("api/communities/{id}/reports")]
public async Task<ActionResult<CommunityReportDto>> GetCommunityReport(Guid id, [FromQuery] ReportFilter filter)
{
    // Generate report for date range
}

// Community programs
[HttpGet("api/communities/{id}/programs")]
public async Task<ActionResult<IEnumerable<CommunityProgramDto>>> GetCommunityPrograms(Guid id)
{
    // Get programs for this community
}
```

### Web UX Changes

**New Pages:**

1. `/communities` - Community Discovery Map
   - Map with community boundaries
   - List/grid view toggle
   - Search by location, name

2. `/communities/{slug}` - Community Public Page
   - Branded header
   - About section (from CMS)
   - Impact metrics widget
   - Upcoming events in community
   - Active teams in community
   - Programs available
   - Contact information

3. `/communities/{id}/admin` - Community Admin Dashboard
   - Overview metrics
   - Recent activity
   - Quick actions
   - Report generation

4. `/communities/{id}/admin/content` - Manage Content
   - Edit about section
   - Update branding
   - Manage contact info

5. `/communities/{id}/admin/programs` - Manage Programs
   - View programs
   - Review applications
   - Manage adoptable areas

### Mobile App Changes

- View community pages
- Filter events by community
- Join community programs
- View community metrics

---

## Implementation Phases

### Phase 1: Discovery & Basic Pages
- Community map and list
- Basic community detail pages
- Link partners to community pages

### Phase 2: Branded Content
- CMS integration for content
- Branding customization
- Metrics widgets
- Events and teams integration

### Phase 3: Admin Dashboard
- SSO integration
- Admin dashboard
- Content management
- Report generation

### Phase 4: Programs
- Program types and setup
- Integration with Adopt-A-Location
- Application management

**Note:** Phases are sequential; SSO requires Project 1 completion.

---

## Open Questions

1. **URL structure for communities?**
   **Recommendation:** `/communities/{slug}` with auto-generated slugs
   **Owner:** Product Lead
   **Due:** Before Phase 1

2. **How to handle overlapping community boundaries?**
   **Recommendation:** Allow overlap; events show in all applicable communities
   **Owner:** Product Lead
   **Due:** Before Phase 1

3. **SSO provider requirements?**
   **Recommendation:** Support Entra/Azure AD; SAML for enterprise
   **Owner:** Security + Auth Team
   **Due:** Before Phase 3

4. **Subscription tiers and pricing?**
   **Recommendation:** Manual billing for 2026; define tiers in contracts
   **Owner:** Business Team
   **Due:** Before pilot launch

---

## Related Documents

- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** - SSO for community admins
- **[Project 8 - Waivers V3](./Project_08_Waivers_V3.md)** - Community-specific waivers
- **[Project 9 - Teams](./Project_09_Teams.md)** - Teams in communities
- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** - Community programs
- **[Project 16 - Content Management](./Project_16_Content_Management.md)** - Editable content

---

**Last Updated:** January 24, 2026
**Owner:** Product Lead + Web Team
**Status:** Planning in Progress
**Next Review:** When dependencies complete
