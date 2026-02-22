# Project 44 — Adoptable Area Map Editor & Bulk Import

| Attribute | Value |
|-----------|-------|
| **Status** | ✅ Complete (Phases 1-7) |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | Project 11 (Adopt-A-Location) |

---

## Business Rationale

Community admins currently define adoptable areas by pasting raw GeoJSON into a textarea — a placeholder UI with the note "Map-based editing will be available in a future update." This makes area creation inaccessible to non-technical users, error-prone, and slow. Communities with existing adoption programs (e.g., Adopt-a-Highway, Adopt-a-Street) have no way to bulk import their existing areas and must re-enter each one manually.

This project replaces the GeoJSON textarea with an interactive map editor, adds AI-assisted area suggestions, shows nearby existing areas for context, and provides a bulk import pipeline for communities migrating from other systems.

---

## Objectives

### Primary Goals
- Interactive map editor for drawing, editing, and previewing adoptable area polygons and linestrings
- AI-assisted area definition: describe an area in natural language and get a suggested polygon
- Show nearby existing areas on the map while creating/editing (prevent overlaps, show context)
- Bulk import of areas from standard GIS formats (GeoJSON, KML, Shapefile)

### Secondary Goals (Nice-to-Have)
- Snap-to-road/trail for linear areas (streets, highways, trails)
- Area templates (common shapes: city block, park boundary, trail segment)
- Import validation report (duplicates, overlaps, missing fields)
- Export existing areas to GeoJSON/KML for sharing with partners

---

## Scope

### Phase 1 — Interactive Map Editor ✅
- [x] Replace GeoJSON textarea with Google Maps drawing tools on create/edit pages
- [x] Support drawing Polygon and LineString geometries
- [x] Edit existing geometries (move vertices, reshape)
- [x] Preview area on map with fill/stroke styling
- [x] Show community boundary context on map
- [x] Auto-populate lat/lng fields from drawn geometry centroid
- [x] GeoJSON generated automatically from drawn shapes (hidden from user)
- [x] Update both entry points: Partner Dashboard and Community Admin area pages

### Phase 2 — Nearby Areas & Context ✅
- [x] Display existing adoptable areas on map during create/edit (different colors by status)
- [x] Show adoption status overlay (Available = green, Adopted = blue, Unavailable = gray)
- [x] Random area colors and map overview for adoptable areas (PR #2746)
- [x] Overlap detection warning when new area intersects existing area
- [x] Area measurement display (square footage / acreage for polygons, length for linestrings)
- [x] Search/geocode to navigate map to a specific address or location

### Phase 3 — AI-Assisted Area Definition ✅
- [x] Natural language input: "The 200 block of Main Street from Oak Ave to Elm Ave"
- [x] AI generates suggested polygon/linestring using geocoding + street network data
- [x] Improved AI suggestions with viewport bounds and Nominatim polygons (PR #2751)
- [x] User reviews and adjusts suggestion on map before saving
- [x] AI can suggest area names based on geography
- [ ] Batch AI suggestions: "All blocks along Main Street between 1st and 10th"

### Phase 4 — Bulk Import ✅
- [x] GeoJSON file upload with drag-and-drop
- [x] KML/KMZ file upload with server-side conversion to GeoJSON
- [x] Shapefile (.zip containing .shp/.dbf/.shx/.prj) upload with server-side conversion
- [x] Import preview: show all areas on map before committing
- [x] Field mapping UI: map source properties to TrashMob fields (name, areaType, description, etc.)
- [x] Validation report: missing required fields, invalid geometries, duplicate names, overlaps
- [x] Batch create with progress indicator
- [x] Import history log (who imported, when, how many areas)

### Phase 5 — Export ✅
- [x] Export community areas to GeoJSON FeatureCollection
- [x] Export to KML for Google Earth/Maps compatibility
- [x] Include area metadata in exports (name, type, status, adoption info)

### Phase 6 — AI Bulk Area Generation ✅
Generate adoptable areas in bulk for an entire community using AI + public geodata (OSM Nominatim, Overpass API). Targets common feature categories:

**Feature Categories:**
- [x] All schools (public/private elementary, middle, high schools)
- [x] All parks (city parks, pocket parks, dog parks, nature preserves)
- [x] All interchanges (highway on/off ramps, freeway interchanges) — enriched with parent motorway refs (e.g., "I 90 Exit 17") (PR #2806)
- [x] All streets (merge disjoint OSM segments into continuous paths, split into ~0.25-mile compass-labeled sections, e.g., "Front St West", "Front St Central", "Front St East") (PR #2806)
- [x] All neighborhoods (residential/suburb boundaries from OSM) — closed ways detected as proper Polygon geometry (PR #2808)

**Deduplication & Naming:**
- [x] Check existing areas by name similarity and geographic overlap before creating
- [x] Naming convention per category (e.g., "Roosevelt Elementary School", "Maple Leaf Park", "I-90/Rainier Ave Interchange", "Main St — 200 Block")
- [x] Flag potential duplicates for human review rather than silently skipping

**Approval Workflow:**
- [x] Generate candidate areas into a staging/review table (not directly into adoptable areas)
- [x] Review UI: map showing all candidates, checkbox to approve/reject each
- [x] Bulk approve/reject with filters (approve all schools, reject specific entries)
- [x] Only approved candidates get created as actual adoptable areas

**Execution Model:**
- [x] Long-running job (could be hundreds or thousands of areas for a large city)
- [x] Background processing via BackgroundService + Channel queue within web API
- [x] Progress tracking: areas discovered, areas processed, areas pending review
- [ ] Resumable: if job is interrupted, pick up where it left off
- [x] Rate limiting for external APIs (Nominatim requires 1 req/sec)

**Map & Geometry Handling:**
- [x] Point geometry support in frontend GeoJSON parser and map overlay (PR #2808)
- [x] Closed-way polygon detection: OSM ways where first coord == last coord emit Polygon instead of LineString (PR #2808)
- [x] Centroid marker pins for all geometry types (Polygon, LineString, Point)

**Quality Controls:**
- [x] Minimum polygon size filter (skip tiny features that aren't meaningful areas)
- [x] Maximum polygon size filter (skip features that are too large to adopt)
- [x] Geographic bounds: only generate areas within community boundary
- [x] Confidence scoring: flag low-confidence areas for closer review

### Phase 7 — Admin Tools ✅
- [x] **Clear All Areas** button on Adoptable Areas page with strong confirmation dialog ("Are you REALLY sure?") (PR #2810)
  - Soft-deletes all adoptable areas (`IsActive = false`) — preserves FK integrity with TeamAdoption/SponsoredAdoption
  - Hard-deletes all generation batches (staged areas cascade-delete)
  - Returns counts of areas deactivated, batches deleted, staged areas deleted
  - `DELETE /api/communities/{partnerId}/areas/clear-all` endpoint with `UserIsPartnerUserOrIsAdmin` authorization

---

## Out-of-Scope

- [ ] 3D terrain/elevation data — flat 2D polygons only
- [ ] Satellite imagery analysis for auto-detecting litter zones
- [ ] Mobile app area editing — web admin only
- [ ] Real-time collaborative editing (multiple admins editing same area simultaneously)
- [ ] Custom map tile layers or offline map support

---

## Success Metrics

### Quantitative
- **Area creation time:** Reduce from 10+ minutes (manual GeoJSON) to < 2 minutes (map drawing)
- **Bulk import adoption:** ≥ 2 communities successfully import existing programs within 3 months
- **AI suggestion acceptance rate:** ≥ 60% of AI-suggested areas accepted with minor or no modifications

### Qualitative
- Non-technical community admins can create areas without developer assistance
- Communities can see their full adoption program on a map for the first time
- Migrating from spreadsheet-based adoption programs becomes a single upload

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Project 11 (Adopt-A-Location):** ✅ Complete — provides AdoptableArea entity and CRUD API

### Enablers for Other Projects
- **Project 43 (Sign Management):** Map editor enables placing signs at precise coordinates within adoption areas
- **Project 41 (Sponsored Adoptions):** Visual area management improves sponsor-facing experience

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Google Maps Drawing API complexity** | Medium | Medium | Use `@vis.gl/react-google-maps` already in codebase; Google Drawing Manager is well-documented |
| **AI geocoding accuracy for area suggestions** | Medium | Medium | Always show AI suggestion on map for human review; never auto-save AI output |
| **Shapefile conversion on server** | Low | Medium | Use proven library (e.g., NetTopologySuite or ogr2ogr); validate output before import |
| **Large imports overwhelming the system** | Low | High | Limit batch size (500 areas per import); background processing with progress updates |
| **Invalid/corrupt GeoJSON from imports** | Medium | Low | Validate geometry before saving; reject invalid features with clear error messages |
| **AI bulk generation creates nonsense areas** | High | High | Mandatory human approval workflow — nothing auto-created. Confidence scoring flags questionable results. Small pilot (single category) before full city generation |
| **OSM data quality varies by region** | Medium | Medium | Some cities have sparse OSM coverage. Show "X areas found" before processing; warn if count seems low. Allow manual additions alongside generated areas |
| **Rate limiting / API quota for Nominatim/Overpass** | Medium | Medium | Respect Nominatim 1 req/sec limit; use Overpass batch queries. Background job with built-in delays. Resumable if interrupted |
| **Large cities produce thousands of candidates** | Medium | Medium | Process by category (schools first, then parks, etc.). Paginated review UI. Cap at configurable limit per batch (e.g., 1000) |

---

## Implementation Plan

### Map Editor Architecture

**Frontend:**
- Google Maps Drawing Manager for polygon/linestring creation
- `@vis.gl/react-google-maps` (v1.5.1, already installed)
- Custom `AreaMapEditor` component replacing GeoJSON textarea
- `google.maps.drawing.DrawingManager` with Polygon and Polyline modes
- Geometry serialized to GeoJSON via `google.maps.Data` layer

**Existing Infrastructure:**
- Google Maps API key fetched via `/maps/googlemapkey` proxy endpoint
- Map components already exist for events, communities, teams
- Address search via Azure Maps (`services/maps.ts`)

### AI Area Suggestion Architecture

**Approach:** Use geocoding + Google Maps Roads/Places API to convert natural language descriptions into geometry.

1. User enters text description (e.g., "200 block of Main Street")
2. Backend geocodes the description to get coordinates
3. For streets: use Roads API / Directions API to trace the street segment
4. For parks/landmarks: use Places API to get boundary or approximate polygon
5. Return suggested GeoJSON to frontend for review on map
6. User adjusts and confirms

**Alternative:** Use an LLM to interpret ambiguous descriptions and generate structured geocoding queries.

### Bulk Import Architecture

**Accepted Formats:**

| Format | Extension | Conversion Method |
|--------|-----------|-------------------|
| GeoJSON | `.geojson`, `.json` | Native — parse directly |
| KML | `.kml` | Server-side XML → GeoJSON conversion |
| KMZ | `.kmz` | Unzip → KML → GeoJSON |
| Shapefile | `.zip` (containing .shp/.dbf/.shx/.prj) | Server-side via NetTopologySuite or ogr2ogr |

**Import GeoJSON Schema:**

Each feature in the FeatureCollection maps to an AdoptableArea:

```json
{
  "type": "FeatureCollection",
  "features": [
    {
      "type": "Feature",
      "properties": {
        "name": "Main St — 100 Block",
        "description": "North side of Main St from 1st Ave to 2nd Ave",
        "areaType": "Street",
        "cleanupFrequencyDays": 90,
        "minEventsPerYear": 4,
        "safetyRequirements": "High-visibility vest required",
        "allowCoAdoption": false
      },
      "geometry": {
        "type": "Polygon",
        "coordinates": [[[-122.4194, 47.6062], [-122.4180, 47.6062], [-122.4180, 47.6055], [-122.4194, 47.6055], [-122.4194, 47.6062]]]
      }
    }
  ]
}
```

**Required properties:** `name` (or mapped field)
**Required geometry:** Valid Polygon or LineString
**Optional properties:** All others default to community settings if omitted

### API Changes

```
POST   /api/communities/{partnerId}/areas/import           — Bulk import from file upload
GET    /api/communities/{partnerId}/areas/export            — Export all areas as GeoJSON
GET    /api/communities/{partnerId}/areas/export?format=kml — Export as KML
POST   /api/communities/{partnerId}/areas/suggest           — AI area suggestion from text
GET    /api/communities/{partnerId}/areas/nearby?lat=&lng=&radius= — Get nearby areas for map context
POST   /api/communities/{partnerId}/areas/generate          — Trigger AI bulk generation job
GET    /api/communities/{partnerId}/areas/generate/status    — Get generation job progress
GET    /api/communities/{partnerId}/areas/staged             — Get staged areas pending review
PUT    /api/communities/{partnerId}/areas/staged/{id}/approve — Approve a staged area
PUT    /api/communities/{partnerId}/areas/staged/{id}/reject  — Reject a staged area
POST   /api/communities/{partnerId}/areas/staged/approve-batch — Bulk approve staged areas
POST   /api/communities/{partnerId}/areas/staged/create-approved — Create adoptable areas from approved staged areas
DELETE /api/communities/{partnerId}/areas/clear-all           — Clear all areas + generation history
```

### Web UX Changes

**Phase 1 — Map Editor:**
- New `AreaMapEditor` component with Google Maps Drawing Manager
- Replace GeoJSON textarea on both create.tsx and edit.tsx pages (4 files total)
- Toolbar: Draw Polygon, Draw Line, Edit, Delete, Undo
- GeoJSON stored in hidden form field, generated from map drawing

**Phase 2 — Context overlay:**
- Existing areas rendered as colored overlays on the editor map
- Status legend (Available/Adopted/Unavailable)
- Address search bar integrated into map
- Area measurement tooltip

**Phase 3 — AI assistant:**
- Text input field above map: "Describe the area you want to define..."
- "Suggest Area" button → shows loading → renders suggestion on map
- "Accept" / "Adjust" / "Try Again" buttons

**Phase 4 — Import:**
- New "Import Areas" page accessible from area list
- Drag-and-drop file upload zone
- Format auto-detection from file extension
- Preview map showing all imported areas
- Field mapping step (table mapping source → TrashMob fields)
- Validation results panel (errors, warnings, info)
- "Import All" / "Import Valid Only" buttons
- Progress bar for batch creation

### Mobile App Changes
- None — web admin only

---

## Implementation Phases

### Phase 1: Interactive Map Editor
- Build `AreaMapEditor` component using Google Maps Drawing Manager
- Integrate into create/edit pages (4 files: 2 partner dashboard + 2 community admin)
- Handle Polygon and LineString creation/editing
- Auto-generate GeoJSON from drawn shapes
- Remove GeoJSON textarea, keep as fallback toggle for power users

### Phase 2: Nearby Areas & Context
- Fetch existing areas via `GetAdoptableAreas()` service
- Render as colored overlays on editor map
- Add overlap detection (client-side using Turf.js or similar)
- Add area measurement display
- Integrate address search into map

### Phase 3: AI-Assisted Area Definition
- Backend endpoint for geocoding text descriptions
- Frontend text input + suggestion UI
- LLM integration for parsing ambiguous descriptions
- Review-and-adjust workflow

### Phase 4: Bulk Import
- File upload endpoint with format detection
- Server-side KML → GeoJSON and Shapefile → GeoJSON converters
- Import preview UI with map display
- Field mapping UI
- Validation engine
- Batch creation with progress tracking

### Phase 5: Export
- GeoJSON export endpoint
- KML export endpoint
- Download buttons on area list page

### Phase 6: AI Bulk Area Generation
- **Data Source:** OSM Nominatim search + Overpass API for polygon boundaries
  - Nominatim: search by category within community bounding box (e.g., `amenity=school`, `leisure=park`)
  - Overpass: fetch actual polygon/multipolygon geometry for each result; two-pass parsing enriches junction nodes with parent motorway refs
  - Closed-way detection: OSM ways where first coord == last coord emit Polygon geometry (neighborhoods, schools with outlined boundaries)
  - Fallback: if no polygon exists, create approximate rectangle from Nominatim bounding box
- **Post-processing by category:**
  - Streets: greedy segment chaining merges disjoint OSM segments, then splits into ~0.25-mile sections with compass-based naming (West/Central/East or South/Central/North)
  - Interchanges: deduplicate by enriched name (e.g., "I 90 Exit 17" prevents EB/WB duplicates)
  - Other categories: deduplicate by name similarity
- **Naming Engine:** AI (Claude) generates standardized names from OSM tags
  - Schools: `{name}` (e.g., "Roosevelt Elementary School")
  - Parks: `{name}` (e.g., "Maple Leaf Park"), fall back to `{leisure} near {street}` if unnamed
  - Interchanges: `{highway_ref} Exit {ref}` (e.g., "I 90 Exit 17") — enriched via two-pass Overpass parsing that maps junction nodes to parent motorway way refs
  - Streets: `{street_name} {compass_label}` (e.g., "Front St West", "Front St Central", "Front St East") — disjoint OSM segments merged via greedy nearest-endpoint chaining, then split into ~0.25-mile sections with compass labels based on dominant direction
  - Neighborhoods: `{name}` from OSM `place=neighbourhood/suburb` tags
- **Staging Table:** New `StagedAdoptableArea` entity with `ReviewStatus` (Pending/Approved/Rejected) and `GenerationBatchId` to group results from a single run
- **Background Job:** Triggered Container App Job (similar to existing daily/hourly jobs pattern)
  - Input: partnerId, feature category, optional sub-filters
  - Output: staged areas with polygons, names, suggested area types
  - Job status tracked in database for progress UI
- **Review UI:** New page under community admin
  - Map showing all staged areas for a batch
  - Table with name, category, area type, confidence, approve/reject toggle
  - Bulk actions: approve all, reject all, approve by category
  - "Create Approved Areas" button to promote staged areas to real adoptable areas
- **Deduplication:**
  - Name fuzzy matching (Levenshtein distance or similar)
  - Geographic overlap check (reuse existing `useOverlapDetection` pattern)
  - Existing areas shown as dimmed overlay on review map

### Phase 7: Admin Tools
- "Clear All Areas" button on Adoptable Areas page
- Strong confirmation dialog with destructive styling
- Backend endpoint: soft-delete all adoptable areas + hard-delete all generation batches/staged areas
- `AreaBulkClearResult` POCO returns counts for success toast

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Phase 6 UX Flow

### Trigger: "Generate Areas" Page

Located under **Community Admin → Adoptable Areas → Generate Areas** (new tab/page alongside the existing area list and import pages).

**Step 1 — Configure Generation**

The admin sees a configuration form:

| Field | Type | Description |
|-------|------|-------------|
| **Feature Category** | Dropdown (single-select) | Schools, Parks, Interchanges, Streets, Neighborhoods |
| **Geographic Scope** | Map + auto-populated | Defaults to community boundary; admin can optionally draw a sub-region to limit scope |
| **Preview Count** | Read-only estimate | After selecting category, a quick Nominatim count query shows "~47 schools found in this area" so the admin knows what to expect before committing |

**Step 2 — Start Generation**

- Admin clicks **"Start Generation"** button
- Confirmation dialog: *"This will search for all [Schools] within [Community Name] and create candidate areas for your review. This may take several minutes for large areas. Continue?"*
- On confirm, frontend calls `POST /api/communities/{partnerId}/areas/generate` with category + optional sub-region bounds
- Backend creates a `GenerationBatch` record (status: `Queued`) and triggers the Container App Job
- Frontend navigates to the **Generation Status** view

**Step 3 — Monitor Progress**

The Generation Status view shows a progress card that polls `GET /api/communities/{partnerId}/areas/generate/status` every 5 seconds:

```
┌─────────────────────────────────────────────────────┐
│  🔍 Generating: Schools in Seattle                   │
│                                                      │
│  ████████████░░░░░░░░░░░░  47%                      │
│                                                      │
│  ✅ Discovered: 47 features                          │
│  ✅ Processed: 22 / 47                               │
│  ⏳ Pending: 25                                      │
│  ⚠️  Skipped: 2 (too small / outside bounds)         │
│                                                      │
│  Started: 2:34 PM  •  Elapsed: 3m 12s               │
│                                                      │
│  [ Cancel ]                                          │
└─────────────────────────────────────────────────────┘
```

**Status states:**
- **Queued** — Job submitted, waiting for Container App to start
- **Discovering** — Querying Nominatim/Overpass for features (shows spinner + "Searching for schools...")
- **Processing** — Fetching polygons + generating names (shows progress bar with count)
- **Complete** — All features processed, ready for review
- **Failed** — Job encountered an error (show error message + retry button)
- **Cancelled** — Admin cancelled the job

**Polling behavior:**
- Poll every 5 seconds while Queued/Discovering/Processing
- Stop polling on Complete/Failed/Cancelled
- If the admin navigates away and comes back, the status page picks up where it left off (status is persisted in DB)

**Step 4 — Review & Approve**

When the job completes, the status card updates to show a summary and a prominent **"Review Results"** button:

```
┌─────────────────────────────────────────────────────┐
│  ✅ Generation Complete: Schools in Seattle          │
│                                                      │
│  45 candidate areas ready for review                 │
│  2 skipped (below minimum size)                      │
│                                                      │
│  [ Review Results → ]                                │
└─────────────────────────────────────────────────────┘
```

Clicking "Review Results" navigates to `GET /api/communities/{partnerId}/areas/staged` — the **Review UI**:

- **Map view**: All 45 candidates shown as polygons, color-coded by confidence (green = high, yellow = medium, red = low)
- **Existing areas**: Shown as dimmed/hatched overlay so admin can spot overlaps
- **Table below map**: Sortable/filterable list with columns:
  - Checkbox (for bulk actions)
  - Name (editable inline)
  - Category (School, Park, etc.)
  - Confidence (High/Medium/Low)
  - Potential Duplicate? (flag icon if name or location matches existing area)
  - Action: Approve / Reject toggle

- **Bulk actions toolbar**:
  - "Select All" / "Deselect All"
  - "Approve Selected" / "Reject Selected"
  - Filter: show only High confidence / show flagged duplicates / show all

- **Final step**: After reviewing, admin clicks **"Create Approved Areas"** (`POST /api/communities/{partnerId}/areas/staged/create-approved`)
  - Shows count: "Create 42 adoptable areas? (3 rejected)"
  - Progress bar as areas are created
  - On complete: "42 areas created! View in area list →"

### Generation History

The Generate Areas page also shows a **history table** of past generation batches:

| Date | Category | Discovered | Approved | Created | Status |
|------|----------|------------|----------|---------|--------|
| Feb 15, 2026 | Schools | 47 | 42 | 42 | Complete |
| Feb 10, 2026 | Parks | 83 | 78 | 78 | Complete |
| Feb 8, 2026 | Schools | 47 | - | - | Cancelled |

Each row links to its review page (read-only for completed batches).

### Edge Cases

- **Job takes too long**: After 30 minutes, job auto-completes with whatever it has processed so far. Remaining items marked as "unprocessed" with option to resume.
- **Admin closes browser**: Job continues running. Next time admin visits the page, they see the status/results.
- **Duplicate batch**: If admin tries to generate the same category while a job is already running, show warning: "A generation job for Schools is already in progress."
- **Zero results**: If Nominatim returns no features, show friendly message: "No [schools] found within [Community Name]. This may mean OSM data is sparse for this area. You can still create areas manually."
- **Concurrent generation**: Only one generation job per community at a time. Button disabled with tooltip explaining why.

---

## Open Questions

1. ~~**Should we use Google Maps Drawing Manager or a third-party library like Leaflet Draw?**~~
   **Resolved:** Google Maps Drawing Manager. ✅

2. ~~**What LLM should power AI area suggestions?**~~
   **Resolved:** Claude API. ✅

3. ~~**Should Shapefile conversion happen client-side or server-side?**~~
   **Resolved:** Server-side. ✅

4. ~~**What is the maximum import batch size?**~~
   **Resolved:** 500 areas per import. ✅

5. ~~**Should bulk generation run as a Container App Job or inline background task?**~~
   **Resolved:** Container App Job (triggered). Costs nothing when not running. ✅

6. ~~**What OSM feature categories should we support initially?**~~
   **Resolved:** Schools, parks, interchanges, streets, and neighborhoods all implemented. ✅

7. ~~**How long should staged (pending review) areas persist?**~~
   **Resolved:** 90 days, auto-delete after expiration. ✅

8. ~~**Should we use OSM Nominatim directly or a self-hosted instance?**~~
   **Resolved:** Public Nominatim to start. Extend existing NominatimService. ✅

9. **What is the UX for triggering generation and monitoring job progress?**
   See Phase 6 UX Flow section below for proposed design.
   **Owner:** Product & Engineering
   **Due:** Before Phase 6 development

---

## Related Documents

- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** — Parent feature providing AdoptableArea entity and CRUD
- **[Project 41 - Sponsored Adoptions](./Project_41_Sponsored_Adoptions.md)** — Professional cleanup areas that benefit from visual management
- **[Project 43 - Sign Management](./Project_43_Sign_Management.md)** — Sign placement benefits from map-based area visualization
- **[Project 15 - Route Tracing](./Project_15_Route_Tracing.md)** — Similar map drawing/tracing patterns for event routes

---

**Last Updated:** February 18, 2026
**Owner:** Product & Engineering Team
**Status:** ✅ Complete (Phases 1-7)
**Next Review:** N/A — Project Complete
