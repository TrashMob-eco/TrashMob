# Project 39 — Regional/State-Level Communities

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 10 (Community Pages), Project 11 (Adopt-A-Location) |

---

## Business Rationale

Enable larger government entities (states, provinces, regions) with existing Adopt-a-Highway or similar programs to leverage TrashMob for volunteer coordination. Many state DOTs and regional authorities already have established cleanup programs but lack modern volunteer management tools. This extends the Communities feature to support non-city-based partners, opening TrashMob to a much larger potential user base.

**Example Use Cases:**
- Minnesota DOT's Adopt-a-Highway program
- California Coastal Commission cleanup programs
- Regional watershed authorities
- State park systems
- Multi-county environmental districts

---

## Objectives

### Primary Goals
- **Support region-based communities** without requiring a specific city
- **Flexible geographic scoping** (state, province, county, region)
- **Adapted map views** that center on larger geographic areas
- **Simplified slugs** like `/communities/minnesota` or `/communities/california-coastal`

### Secondary Goals
- Sub-region organization (counties within a state program)
- Integration with existing government volunteer systems
- Bulk event import from existing programs
- Custom branding for government partners

---

## Scope

### Phase 1 - Data Model & API
- [ ] Make Partner.City optional (nullable)
- [ ] Add Partner.RegionType enum (City, County, State, Province, Region, Country)
- [ ] Add Partner.GeoBounds field (bounding box for map centering)
- [ ] Add Partner.CenterLatitude/CenterLongitude for map default position
- [ ] Update Partner validation to require either City OR RegionType
- [ ] Update community slug generation to handle region-only partners

### Phase 2 - Map Enhancements
- [ ] Calculate appropriate zoom level based on RegionType
- [ ] Center map on GeoBounds or Center coordinates
- [ ] Show events within geographic bounds (not just city radius)
- [ ] Update event filtering to work with regional boundaries

### Phase 3 - UI Updates
- [ ] Update community page to display region name instead of city
- [ ] Adapt "Events near [location]" messaging for regions
- [ ] Update partner registration flow to allow region selection
- [ ] Add region search/filter on community discovery page

### Phase 4 - Sub-Regions (Future)
- [ ] Parent/child relationship for communities (e.g., Minnesota -> Hennepin County)
- [ ] Hierarchical event aggregation
- [ ] Delegated administration for sub-regions

---

## Out-of-Scope

- Multi-country support (international programs)
- Automatic boundary detection from government APIs
- Integration with specific DOT systems
- Volunteer background check integration
- Government procurement/contracting workflows

---

## Success Metrics

### Quantitative
- **Regional partners onboarded:** >= 5 state-level programs in first year
- **Events created under regional programs:** >= 100 in first 6 months
- **Volunteer reach expansion:** 25% increase in geographic coverage

### Qualitative
- Government partners find the platform easy to use
- No confusion between city-based and region-based communities
- Seamless experience for volunteers in regional programs

---

## Dependencies

### Blockers
- **Project 10 (Community Pages):** Must be complete (currently complete)
- **Project 11 (Adopt-A-Location):** Location infrastructure needed (currently complete)

### Enables
- State DOT partnerships
- Large-scale environmental programs
- Government grant applications requiring volunteer management

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Geographic boundary complexity** | Medium | Medium | Start with simple bounding boxes; add polygon support later |
| **Event discovery confusion** | Low | Medium | Clear UI indication of community scope; filtered search results |
| **Partner administration at scale** | Medium | Low | Leverage existing admin delegation patterns from Teams |
| **Performance with large regions** | Low | Medium | Lazy load events; implement clustering on maps |

---

## Implementation Plan

### Data Model Changes

**Partner Model Updates:**
```csharp
public class Partner
{
    // Existing fields...

    // Make City nullable
    public string? City { get; set; }

    // New fields
    public RegionType? RegionType { get; set; }
    public decimal? CenterLatitude { get; set; }
    public decimal? CenterLongitude { get; set; }
    public decimal? BoundsNorth { get; set; }
    public decimal? BoundsSouth { get; set; }
    public decimal? BoundsEast { get; set; }
    public decimal? BoundsWest { get; set; }
}

public enum RegionType
{
    City = 0,
    County = 1,
    State = 2,
    Province = 3,
    Region = 4,
    Country = 5
}
```

### API Changes

**Slug Generation:**
- City-based: `/communities/{state}-{city}` (e.g., `/communities/wa-seattle`)
- Region-based: `/communities/{region}` (e.g., `/communities/minnesota`)

**Event Filtering:**
- For city communities: filter by radius from city center
- For regional communities: filter by bounding box or state/region field

### Web UX Changes

**Community Page:**
- Display region name prominently when RegionType is not City
- Adjust "X events in [location]" to show region name
- Map initializes at appropriate zoom level for region size

**Partner Registration:**
- Step to choose "City-based" or "Regional" community
- For regional: dropdown for RegionType, text field for region name
- Optional: map tool to define custom bounding box

### Mobile App Changes

- Update community discovery to show regional communities
- Adjust map zoom defaults based on community type

---

## Zoom Level Guidelines

| RegionType | Default Zoom | Typical Area |
|------------|--------------|--------------|
| City | 12-13 | ~10 mile radius |
| County | 9-10 | ~50 mile radius |
| State | 6-7 | ~200 mile radius |
| Province | 6-7 | ~200 mile radius |
| Region | 7-8 | ~100 mile radius |
| Country | 4-5 | ~500+ mile radius |

---

## Open Questions

1. **How do we handle events that span multiple regions?**
   **Recommendation:** Events belong to one community; can be discovered from neighboring regions via search
   **Owner:** Product
   **Due:** Before Phase 2

2. **Should regional communities require government verification?**
   **Recommendation:** Yes, add verification flag and manual approval for RegionType >= County
   **Owner:** Product + Legal
   **Due:** Before launch

3. **How do we prevent slug conflicts (e.g., "minnesota" city vs state)?**
   **Recommendation:** Region-based slugs use state abbreviation prefix: `/communities/mn` for state, `/communities/mn-minnesota` if there's a city named Minnesota
   **Owner:** Engineering
   **Due:** Phase 1

4. **Should we support custom polygon boundaries?**
   **Recommendation:** Not for v1; bounding box is sufficient. Add polygon support if needed for coastal/watershed programs
   **Owner:** Engineering
   **Due:** Future consideration

---

## Related Documents

- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Base community feature
- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** - Location adoption infrastructure
- **[Project 35 - Partner Location Map](./Project_35_Partner_Location_Map.md)** - Partner map display

---

**Last Updated:** February 3, 2026
**Owner:** Product Team
**Status:** Not Started
**Next Review:** When government partnership opportunities are identified
