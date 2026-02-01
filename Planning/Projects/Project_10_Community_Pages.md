# Project 10 — Community Pages

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in Progress |
| **Priority** | High |
| **Risk** | Very High |
| **Size** | Very Large |
| **Dependencies** | Project 1 (Auth), Project 8 (Waivers), Project 9 (Teams) |

---

## Business Rationale

Branded public pages for partner communities (cities, counties, organizations) with metrics, photos, contact info, SSO, and adopt-a programs. Community Pages are a key revenue driver as cities can subscribe for branded presence and program management tools.

---

## Objectives

### Primary Goals
- **Community discovery map** showing all partnered communities
- **Branded public home pages** for each community
- **Custom community branding** (colors, logos)
- **Community-specific waivers**
- **Admin management** for community staff
- **Metrics & notifications** for community stakeholders
- **Opt-in to adopt-a programs** integration

### Secondary Goals
- SSO for community admin login
- Community newsletters
- **Community leaderboards** (individual and team rankings within the community)

---

## Scope

### Phase 1 - Community Discovery
- ✅ Map showing community locations
- ✅ List view with search/filter
- ✅ Basic community detail pages
- ✅ Link from partner pages
- ☐ Friendly URLs for each community (`/communities/{city-state}` e.g., `/communities/seattle-wa`)

### Phase 2 - Community Home Pages
- ✅ Branded header with logo
- ✅ About section (admin-editable, stored in database)
- ✅ Contact information
- ✅ Community map centered on location (events, litter reports, teams)
- ✅ Events in this community
- ✅ Teams in this community
- ✅ Impact metrics display
- ☐ Community leaderboards (individual and team) - future enhancement

### Community Page Customizable Fields (V1)

| Field | Type | Notes |
|-------|------|-------|
| Logo | Image upload | Standard size (200x200) |
| Hero/Banner image | Image upload | Standard dimensions |
| Primary color | Color picker | Hex value |
| Secondary color | Color picker | Hex value |
| About text | Textarea | Plain text or markdown |
| Tagline | Text | Short descriptor |
| Contact email | Email | Required |
| Contact phone | Text | Optional |
| Website URL | URL | Optional |
| Social links | URLs | Facebook, Instagram, Twitter, LinkedIn |
| Physical address | Text | For display |

### Community Page Customizable Fields (V1)

| Field | Type | Notes |
|-------|------|-------|
| Logo | Image upload | Standard size (200x200) |
| Hero/Banner image | Image upload | Standard dimensions |
| Primary color | Color picker | Hex value |
| Secondary color | Color picker | Hex value |
| About text | Textarea | Plain text or markdown |
| Tagline | Text | Short descriptor |
| Contact email | Email | Required |
| Contact phone | Text | Optional |
| Website URL | URL | Optional |
| Social links | URLs | Facebook, Instagram, Twitter, LinkedIn |
| Physical address | Text | For display |

### Phase 3 - Community Admin
- ✅ Admin dashboard for community
- ✅ Manage community content
- ✅ View community metrics and reports
- ✅ Download reports (CSV, PDF)
- ☐ SSO integration for community staff (secondary goal)

### Phase 4 - Programs Integration
- ✅ Opt-in to Adopt-A-Location program
- ✅ Manage adoptable areas
- ✅ Review team applications
- ✅ Community-specific events

### Phase 5 - Documentation
- ☐ Update FAQ page with Community-related questions (see #2307)
- ☐ Update Help documentation for Community features
- ☐ Add Communities section to Getting Started guide
- ☐ Create Community Admin guide/tips content

---

## Out-of-Scope

- ❌ Billing/subscription management (manual for 2026)
- ❌ Community-specific mobile apps
- ❌ Multi-language community pages
- ❌ Community forums/discussion boards
- ❌ Custom domain support (future)
- ❌ Community sponsorship display areas

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
- **Project 8 (Waivers V3):** Community-specific waivers
- **Project 9 (Teams):** Teams associated with communities

### Related (Non-Blocking)
- **Project 1 (Auth Revamp):** SSO for community admins (secondary goal)

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

**Modification: Partner (add home page properties)**
```csharp
// Add to existing TrashMob.Models/Partner.cs
#region Community Home Page Properties

/// <summary>
/// Gets or sets when the community home page subscription starts.
/// </summary>
public DateTimeOffset? HomePageStartDate { get; set; }

/// <summary>
/// Gets or sets when the community home page subscription ends.
/// </summary>
public DateTimeOffset? HomePageEndDate { get; set; }

/// <summary>
/// Gets or sets whether the community home page is enabled.
/// </summary>
public bool HomePageEnabled { get; set; }

/// <summary>
/// Gets or sets the primary branding color (hex, e.g., "#FF5733").
/// </summary>
public string BrandingPrimaryColor { get; set; }

/// <summary>
/// Gets or sets the secondary branding color (hex).
/// </summary>
public string BrandingSecondaryColor { get; set; }

/// <summary>
/// Gets or sets the URL of the community banner image.
/// </summary>
public string BannerImageUrl { get; set; }

#endregion

// Navigation property for programs
public virtual ICollection<CommunityProgram> Programs { get; set; }
```

**New Entity: CommunityProgramType (lookup table)**
```csharp
// New file: TrashMob.Models/CommunityProgramType.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a type of community program (e.g., Adopt-A-Highway, Adopt-A-Park).
    /// </summary>
    public class CommunityProgramType : LookupModel
    {
        /// <summary>
        /// Gets or sets the description of this program type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order for sorting.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets whether this program type is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<CommunityProgram> Programs { get; set; }
    }
}
```

**New Entity: CommunityProgram**
```csharp
// New file: TrashMob.Models/CommunityProgram.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a program offered by a community (partner).
    /// </summary>
    public class CommunityProgram : KeyedModel
    {
        /// <summary>
        /// Gets or sets the community (partner) identifier.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the program type identifier.
        /// </summary>
        public int ProgramTypeId { get; set; }

        /// <summary>
        /// Gets or sets the program name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the program description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether this program is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Partner Partner { get; set; }
        public virtual CommunityProgramType ProgramType { get; set; }
        public virtual ICollection<AdoptableArea> AdoptableAreas { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<Partner>(entity =>
{
    // Add to existing Partner configuration
    entity.Property(e => e.BrandingPrimaryColor).HasMaxLength(7);
    entity.Property(e => e.BrandingSecondaryColor).HasMaxLength(7);
    entity.Property(e => e.BannerImageUrl).HasMaxLength(500);
    entity.HasIndex(e => e.HomePageEnabled)
        .HasFilter("[HomePageEnabled] = 1");
});

modelBuilder.Entity<CommunityProgramType>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
    entity.Property(e => e.Description).HasMaxLength(500);
});

modelBuilder.Entity<CommunityProgram>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(200).IsRequired();

    entity.HasOne(e => e.Partner)
        .WithMany(p => p.Programs)
        .HasForeignKey(e => e.PartnerId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.ProgramType)
        .WithMany(pt => pt.Programs)
        .HasForeignKey(e => e.ProgramTypeId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.PartnerId);
});
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
   - About section (admin-editable)
   - **Community map** centered on community location showing:
     - Events in this community
     - Litter reports in this community
     - Teams in this community
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
- Admin-editable content fields (about, tagline, contact info)
- Branding customization (logo, colors, banner image)
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

### Phase 5: Documentation
- Update FAQ with Community questions
- Update Help docs for Community features
- Add to Getting Started guide
- Create Community Admin guide

**Note:** Phases are sequential; SSO requires Project 1 completion.

---

## Open Questions

1. ~~**URL structure for communities?**~~
   **Decision:** `/communities/{slug}` with friendly slugs (e.g., `/communities/seattle`)
   **Status:** ✅ Resolved

2. ~~**How to handle non-unique city names and spaces in slugs?**~~
   **Decision:** Always include state abbreviation: `/communities/seattle-wa`, `/communities/portland-or`, `/communities/new-york-ny`. Use hyphens for spaces.
   **Status:** ✅ Resolved

3. **How to handle overlapping community boundaries?**
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

5. ~~**How are community boundaries defined geographically?**~~
   **Decision:** Simple city/state matching for v1; polygon boundaries for v2 if needed for complex cases (multi-city regions, unincorporated areas)
   **Status:** ✅ Resolved

6. ~~**What happens to community content when subscription lapses?**~~
   **Decision:** 30-day grace period with warning; then page shows "Inactive Community" with historical metrics preserved but no active features; full data deletion after 1 year of inactivity
   **Status:** ✅ Resolved

7. ~~**Can communities have multiple admin roles with different permissions?**~~
   **Decision:** No. Single role: Community Admin (full access). Multiple people can have the admin role for a community.
   **Status:** ✅ Resolved

8. ~~**What content guidelines apply to community uploads (logos, banners)?**~~
   **Decision:** Same moderation rules as team photos; logos must be appropriate and non-offensive; review queue for all community branding changes; integration with Project 28
   **Status:** ✅ Resolved

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2232](https://github.com/trashmob/TrashMob/issues/2232)** - Project 10: Community Pages (tracking issue)
- **[#2299](https://github.com/trashmob/TrashMob/issues/2299)** - Create data model for community pages
- **[#2300](https://github.com/trashmob/TrashMob/issues/2300)** - Update entities for community pages
- **[#2301](https://github.com/trashmob/TrashMob/issues/2301)** - Add back end apis for community pages
- **[#2302](https://github.com/trashmob/TrashMob/issues/2302)** - Community Page UX Design
- **[#2303](https://github.com/trashmob/TrashMob/issues/2303)** - Website - Add Community Page View
- **[#2304](https://github.com/trashmob/TrashMob/issues/2304)** - Website - Add Community Page Edit
- **[#2305](https://github.com/trashmob/TrashMob/issues/2305)** - Website - Add Navigation from Home Page to Community Page
- **[#2306](https://github.com/trashmob/TrashMob/issues/2306)** - Website - Add Featured Communities Widget to home page
- **[#2307](https://github.com/trashmob/TrashMob/issues/2307)** - Website - Add FAQ for Communities Functionality

---

## Related Documents

- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** - SSO for community admins
- **[Project 8 - Waivers V3](./Project_08_Waivers_V3.md)** - Community-specific waivers
- **[Project 9 - Teams](./Project_09_Teams.md)** - Teams in communities
- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** - Community programs

---

**Last Updated:** January 31, 2026
**Owner:** Product Lead + Web Team
**Status:** Planning in Progress
**Next Review:** When dependencies complete
