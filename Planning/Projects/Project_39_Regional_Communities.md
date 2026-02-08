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

Enable larger government entities (counties, states, provinces, regions) with existing Adopt-a-Highway or similar programs to leverage TrashMob for volunteer coordination. Many county governments, state DOTs, and regional authorities already have established cleanup and adoption programs but lack modern volunteer management tools. This extends the Communities feature to support non-city-based partners — particularly counties and states — opening TrashMob to a much larger potential user base.

**Example Use Cases:**
- **Counties:** King County (WA) road adoption program, Hennepin County (MN) park cleanups, Maricopa County (AZ) litter prevention
- **States:** Minnesota DOT's Adopt-a-Highway program, California Coastal Commission cleanup programs
- **Regions:** Regional watershed authorities, multi-county environmental districts
- **Other:** State park systems, county park districts

---

## Objectives

### Primary Goals
- **Support region-based communities** without requiring a specific city
- **County-level community pages** with their own branding, events, and adoption programs
- **County and state adoption programs** — counties can manage Adopt-a-Road, Adopt-a-Park, and similar programs through their community page
- **Flexible geographic scoping** (county, state, province, region)
- **Adapted map views** that center on larger geographic areas
- **Simplified slugs** like `/communities/king-county-wa` or `/communities/minnesota`

### Secondary Goals
- Sub-region organization (counties within a state program, or cities within a county)
- Integration with existing government volunteer systems (county public works, state DOTs)
- Bulk event import from existing county/state programs
- Custom branding for government partners

---

## Scope

### Phase 1 - Data Model & API
- [ ] Make Partner.City optional (nullable)
- [ ] Add Partner.RegionType enum (City, County, State, Province, Region, Country)
- [ ] Add Partner.CountyName field for county-level communities
- [ ] Add Partner.GeoBounds field (bounding box for map centering)
- [ ] Add Partner.CenterLatitude/CenterLongitude for map default position
- [ ] Update Partner validation to require either City OR RegionType (County/State/etc.)
- [ ] Update community slug generation to handle county and region partners
- [ ] Extend Adopt-a-Location to support county-scoped adoption areas (roads, parks, trails)

### Phase 2 - County & State Community Pages
- [ ] Update community page to display county/region name instead of city
- [ ] Allow counties to list adoptable locations within their boundaries
- [ ] County adoption dashboard — manage adopted segments, track volunteer compliance
- [ ] Adapt "Events near [location]" messaging for counties and regions
- [ ] Update partner registration flow with county/region selection
- [ ] Add county/region search and filter on community discovery page

### Phase 3 - Map Enhancements
- [ ] Calculate appropriate zoom level based on RegionType
- [ ] Center map on GeoBounds or Center coordinates
- [ ] Show events within geographic bounds (not just city radius)
- [ ] Update event filtering to work with county/regional boundaries
- [ ] Show adoptable locations on county community map

### Phase 4 - Sub-Regions (Future)
- [ ] Parent/child relationship for communities (e.g., Minnesota -> Hennepin County -> City of Minneapolis)
- [ ] Hierarchical event aggregation (county rolls up to state)
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
- **County partners onboarded:** >= 10 county programs in first year
- **State/regional partners onboarded:** >= 5 state-level programs in first year
- **Adoption programs created:** >= 15 county/state adoption programs in first year
- **Events created under regional programs:** >= 100 in first 6 months
- **Volunteer reach expansion:** 25% increase in geographic coverage

### Qualitative
- County and state partners find the platform easy to use
- No confusion between city-based, county-based, and region-based communities
- Seamless experience for volunteers in county/regional adoption programs

---

## Dependencies

### Blockers
- **Project 10 (Community Pages):** Must be complete (currently complete)
- **Project 11 (Adopt-A-Location):** Location infrastructure needed (currently complete)

### Enables
- County government partnerships (public works, parks departments)
- State DOT partnerships
- County and state adoption programs (Adopt-a-Road, Adopt-a-Park)
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
- County-based: `/communities/{county}-{state}` (e.g., `/communities/king-county-wa`)
- State/Region-based: `/communities/{region}` (e.g., `/communities/minnesota`)

**Event Filtering:**
- For city communities: filter by radius from city center
- For county communities: filter by county bounding box
- For state/regional communities: filter by bounding box or state/region field

**Adoption Programs:**
- Counties can create adoptable locations (road segments, parks, trails) within their bounds
- Volunteers/teams claim adoptable locations through the county community page
- County admins manage adoption compliance (cleanup frequency, reporting)

### Web UX Changes

**Community Page:**
- Display county/region name prominently when RegionType is not City
- Adjust "X events in [location]" to show county/region name
- Map initializes at appropriate zoom level for region size
- County pages show an "Adopt a Location" section listing available segments

**Partner Registration:**
- Step to choose community scope: City, County, State/Province, or Region
- For county: state selector + county name, with optional bounding box
- For state/region: dropdown for RegionType, text field for region name
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

2. **Should county/regional communities require government verification?**
   **Recommendation:** Yes, add verification flag and manual approval for RegionType >= County
   **Owner:** Product + Legal
   **Due:** Before launch

3. **How do we prevent slug conflicts (e.g., "minnesota" city vs state, or multiple "Washington County" in different states)?**
   **Recommendation:** County slugs include state abbreviation: `/communities/king-county-wa`. State slugs use abbreviation: `/communities/mn`.
   **Owner:** Engineering
   **Due:** Phase 1

5. **Can counties define custom adoption zones or do they use pre-set boundaries?**
   **Recommendation:** Start with county bounding boxes from census data; allow manual adjustment. Custom polygon boundaries deferred to v2.
   **Owner:** Engineering
   **Due:** Phase 2

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

**Last Updated:** February 8, 2026
**Owner:** Product Team
**Status:** Not Started
**Next Review:** When government partnership opportunities are identified
