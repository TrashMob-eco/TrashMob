# Project 44 — Adoptable Area Map Editor & Bulk Import

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
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

### Phase 1 — Interactive Map Editor
- [ ] Replace GeoJSON textarea with Google Maps drawing tools on create/edit pages
- [ ] Support drawing Polygon and LineString geometries
- [ ] Edit existing geometries (move vertices, reshape)
- [ ] Preview area on map with fill/stroke styling
- [ ] Show community boundary context on map
- [ ] Auto-populate lat/lng fields from drawn geometry centroid
- [ ] GeoJSON generated automatically from drawn shapes (hidden from user)
- [ ] Update both entry points: Partner Dashboard and Community Admin area pages

### Phase 2 — Nearby Areas & Context
- [ ] Display existing adoptable areas on map during create/edit (different colors by status)
- [ ] Show adoption status overlay (Available = green, Adopted = blue, Unavailable = gray)
- [ ] Overlap detection warning when new area intersects existing area
- [ ] Area measurement display (square footage / acreage for polygons, length for linestrings)
- [ ] Search/geocode to navigate map to a specific address or location

### Phase 3 — AI-Assisted Area Definition
- [ ] Natural language input: "The 200 block of Main Street from Oak Ave to Elm Ave"
- [ ] AI generates suggested polygon/linestring using geocoding + street network data
- [ ] User reviews and adjusts suggestion on map before saving
- [ ] AI can suggest area names based on geography
- [ ] Batch AI suggestions: "All blocks along Main Street between 1st and 10th"

### Phase 4 — Bulk Import
- [ ] GeoJSON file upload with drag-and-drop
- [ ] KML/KMZ file upload with server-side conversion to GeoJSON
- [ ] Shapefile (.zip containing .shp/.dbf/.shx/.prj) upload with server-side conversion
- [ ] Import preview: show all areas on map before committing
- [ ] Field mapping UI: map source properties to TrashMob fields (name, areaType, description, etc.)
- [ ] Validation report: missing required fields, invalid geometries, duplicate names, overlaps
- [ ] Batch create with progress indicator
- [ ] Import history log (who imported, when, how many areas)

### Phase 5 — Export
- [ ] Export community areas to GeoJSON FeatureCollection
- [ ] Export to KML for Google Earth/Maps compatibility
- [ ] Include area metadata in exports (name, type, status, adoption info)

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

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **Should we use Google Maps Drawing Manager or a third-party library like Leaflet Draw?**
   **Recommendation:** Google Maps Drawing Manager — already using `@vis.gl/react-google-maps`, consistent UX with existing map components, and better integration with Google's geocoding/places APIs.
   **Owner:** Engineering
   **Due:** Before Phase 1 development

2. **What LLM should power AI area suggestions?**
   **Recommendation:** Claude API — already integrated for Project 40 (AI Community Sales Agent). Use structured output to generate geocoding queries from natural language.
   **Owner:** Engineering
   **Due:** Before Phase 3 development

3. **Should Shapefile conversion happen client-side or server-side?**
   **Recommendation:** Server-side — Shapefiles are binary multi-file archives; server-side conversion via NetTopologySuite is more reliable and handles coordinate system transformations.
   **Owner:** Engineering
   **Due:** Before Phase 4 development

4. **What is the maximum import batch size?**
   **Recommendation:** 500 areas per import. Larger datasets should be split into multiple files. Background processing with progress indicator for > 50 areas.
   **Owner:** Product
   **Due:** Before Phase 4 development

---

## Related Documents

- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** — Parent feature providing AdoptableArea entity and CRUD
- **[Project 41 - Sponsored Adoptions](./Project_41_Sponsored_Adoptions.md)** — Professional cleanup areas that benefit from visual management
- **[Project 43 - Sign Management](./Project_43_Sign_Management.md)** — Sign placement benefits from map-based area visualization
- **[Project 15 - Route Tracing](./Project_15_Route_Tracing.md)** — Similar map drawing/tracing patterns for event routes

---

**Last Updated:** February 8, 2026
**Owner:** Product & Engineering Team
**Status:** Not Started
**Next Review:** When volunteer picks up work
