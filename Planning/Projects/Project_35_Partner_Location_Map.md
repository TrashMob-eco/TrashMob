# Project 35 — Partner Dashboard Location Map

| Attribute | Value |
|-----------|-------|
| **Status** | Complete |
| **Priority** | Low |
| **Risk** | Low |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

Partners (cities, counties, organizations) often have multiple service locations, offices, or areas of responsibility. Currently, there's no visual way for partner administrators to see all their locations at a glance or for users to understand a partner's geographic coverage.

Adding a map view to the Partner Dashboard enables partners to visualize their footprint, manage location data, and provides a foundation for location-based partner discovery.

---

## Objectives

### Primary Goals
- **Map visualization** of all partner locations on Partner Dashboard
- **Location management** (add, edit, remove partner locations)
- **Public partner map** showing partner coverage areas

### Secondary Goals (Nice-to-Have)
- Partner location search/filter
- Service area polygons (not just points)
- Integration with Community Pages

---

## Scope

### Phase 1 - Partner Dashboard Map
- ✅ Add map component to Partner Dashboard
- ✅ Display all locations for the partner
- ✅ Click location for details popup
- ✅ Zoom to fit all locations

### Phase 2 - Location Management
- ✅ Add new location form (address, name, type)
- ✅ Edit existing locations
- ✅ Delete locations
- ✅ Geocoding from address input

### Phase 3 - Public Discovery
- ✅ Add partner locations to public partner detail page
- ✅ "Find Partners Near Me" search
- ✅ Filter partners by location

---

## Out-of-Scope

- ❌ Service area polygons (future enhancement)
- ❌ Real-time location tracking
- ❌ Partner location routing/directions
- ❌ Multi-partner location comparison
- ❌ Mobile app partner map (web only for now)

---

## Success Metrics

### Quantitative
- **Partners with locations:** ≥ 50% of active partners have at least one location
- **Location data quality:** ≥ 95% of locations geocode successfully
- **Partner discovery:** Track "Find Partners" usage

### Qualitative
- Partners find location management intuitive
- Users can easily find nearby partners
- Improved partner onboarding experience

---

## Dependencies

### Blockers (Must be complete before this project starts)
None - can proceed independently

### Enablers for Other Projects (What this unlocks)
- **Project 10 (Community Pages):** Community-partner location integration
- **Project 11 (Adopt-A-Location):** Partner location as adoption base

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Poor geocoding accuracy** | Medium | Medium | Use Azure Maps geocoding; allow manual pin adjustment |
| **Many locations slows map** | Low | Low | Clustering for partners with 50+ locations |
| **Partners don't add locations** | Medium | Medium | Onboarding prompt; make it easy; show value |

---

## Implementation Plan

### Data Model Changes

**New Entity: PartnerLocation**
```csharp
// New file: TrashMob.Models/PartnerLocation.cs
namespace TrashMob.Models
{
    public class PartnerLocation : KeyedModel
    {
        public Guid PartnerId { get; set; }
        public string Name { get; set; } // e.g., "City Hall", "Main Office"
        public string LocationType { get; set; } // Office, ServiceCenter, DropOff, etc.
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPrimaryLocation { get; set; }

        // Navigation property
        public virtual Partner Partner { get; set; }
    }
}
```

### API Changes

```csharp
// Get partner locations
[HttpGet("api/partners/{partnerId}/locations")]
public async Task<ActionResult<IEnumerable<PartnerLocationDto>>> GetPartnerLocations(Guid partnerId)
{
    // Return all locations for partner
}

// Add location (partner admin)
[Authorize]
[HttpPost("api/partners/{partnerId}/locations")]
public async Task<ActionResult<PartnerLocationDto>> AddLocation(
    Guid partnerId, [FromBody] CreatePartnerLocationRequest request)
{
    // Validate partner admin
    // Geocode address
    // Create location
}

// Update location
[Authorize]
[HttpPut("api/partners/{partnerId}/locations/{locationId}")]
public async Task<ActionResult<PartnerLocationDto>> UpdateLocation(
    Guid partnerId, Guid locationId, [FromBody] UpdatePartnerLocationRequest request)
{
    // Validate and update
}

// Delete location
[Authorize]
[HttpDelete("api/partners/{partnerId}/locations/{locationId}")]
public async Task<ActionResult> DeleteLocation(Guid partnerId, Guid locationId)
{
    // Soft delete or remove
}

// Public: Find partners near location
[HttpGet("api/partners/nearby")]
public async Task<ActionResult<IEnumerable<PartnerDto>>> FindNearbyPartners(
    [FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] int radiusMiles = 25)
{
    // Return partners with locations within radius
}
```

### Web UX Changes

**Partner Dashboard Enhancement:**
- Add "Locations" tab to Partner Dashboard
- Map showing all partner locations
- List view toggle
- Add/Edit/Delete location buttons

**Location Form:**
- Address input with autocomplete
- Map preview with draggable pin
- Location type dropdown
- Primary location checkbox
- Notes field

**Public Partner Page:**
- Mini-map showing partner locations
- Address list with icons

---

## Implementation Phases

### Phase 1: Dashboard Map
- Create database entity and migration
- Build location API endpoints
- Add map component to Partner Dashboard
- Display existing locations (if any)

### Phase 2: Location Management
- Build add/edit location form
- Integrate Azure Maps geocoding
- Add location type management
- Primary location designation

### Phase 3: Public Discovery
- Add location map to public partner page
- Implement nearby partner search
- Add to partner list filters

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **Should partners be limited to a maximum number of locations?**
   **Recommendation:** No hard limit; soft warning at 50+ locations for performance
   **Owner:** Product Lead
   **Due:** Before Phase 1

2. **Should location types be fixed or partner-configurable?**
   **Recommendation:** Fixed list initially (Office, ServiceCenter, DropOff, Other); expand based on feedback
   **Owner:** Product Lead
   **Due:** Before Phase 2

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#294](https://github.com/trashmob/TrashMob/issues/294)** - Partner Dashboard - Add Map of all Partner locations for a Partner (tracking issue)

---

## Related Documents

- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community-partner integration
- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** - Location adoption programs

---

**Last Updated:** February 3, 2026
**Owner:** Web Team
**Status:** Complete
**Next Review:** N/A

---

## Changelog

- **2026-02-03:** Completed all phases - added map view toggle, nearby search API
- **2026-01-31:** Created project from Issue #294
