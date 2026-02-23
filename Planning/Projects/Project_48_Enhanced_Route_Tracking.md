# Project 48 — Enhanced Route Tracking: Metrics, Density Heatmap, and Route Editing

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phase 1 Complete) |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | Project 15 (Map Route Tracing), Project 7 (Weight Tracking) |

---

## Business Rationale

Route tracking currently records GPS paths but doesn't connect the litter collected on a route back to the event's totals. Volunteers enter bags and weight in the event summary separately, with no per-route attribution. This means:

1. **No per-route metrics:** Event leads can't see which routes produced the most impact. A 2-mile route through a park and a half-mile route through a parking lot look the same on the map, even though one may have yielded 10 bags while the other produced 1.

2. **No density comparison:** Communities can't identify high-litter corridors. If routes showed litter density (weight per meter), planners could prioritize areas that consistently produce heavy hauls and schedule more frequent cleanups there.

3. **Route data quality issues:** Users sometimes forget to stop tracking when they finish cleaning and drive home, adding kilometers of highway to their route. There's no way to fix this after the fact — the route must be deleted entirely and the data is lost.

This project addresses all three gaps by adding route-level metric entry, a litter density color gradient for visual comparison, and a post-completion route editor with end-time adjustment.

---

## Objectives

### Primary Goals
- Allow users to enter bags collected and weight picked up per route, and roll those values into the event summary automatically
- Render routes with a color gradient based on litter density (weight or bags per meter) so routes can be visually compared
- Allow users to edit the end time of a completed route by sliding backward, trimming off accidental post-cleanup travel

### Secondary Goals (Nice-to-Have)
- Show density legend on map (color scale with units)
- Per-segment density: if a user enters bags at waypoints during a route, show density variation within a single route
- Route comparison view: overlay two routes side-by-side with density coloring
- Auto-detect "driving away" heuristic: flag segments with speed > 10 km/h as likely non-cleanup

---

## Scope

### Phase 1 — Route-Level Metric Entry & Event Summary Rollup

**Goal:** Let users enter bags and weight on each route and auto-aggregate into the event summary.

#### Backend

- [x] Add `WeightUnitId` (int?, FK to WeightUnits) to `EventAttendeeRoute` — migration `20260222160059_AddRouteWeightUnitAndSummaryRouteFlag`
- [x] Add `PUT /api/routes/{routeId}` endpoint for updating route metrics (bags, weight, weight unit, notes) — `RouteMetadataController`
- [x] Create aggregation logic in `EventAttendeeRouteManager`:
  - Sums `BagsCollected` across all routes for an event
  - Sums `WeightCollected` (converting to a common unit if mixed) across all routes
  - Sums `DurationMinutes` across all routes
  - Returns `DisplayEventRouteStats` DTO
- [x] Add `GET /api/events/{eventId}/routes/stats` endpoint returning the aggregate — `EventRoutesController`
- [x] Add `GET /api/events/{eventId}/routes/summary-prefill` endpoint for event summary pre-fill
- [x] Add `IsFromRouteData` boolean to `EventSummary` to indicate whether totals were auto-populated vs manually entered

#### Mobile App

- [x] Add "Log Pickup" popup accessible from the active route tracking view — `LogPickupPopup`
  - Bags collected (integer stepper, 0-99)
  - Weight picked (decimal input with unit picker: lb / kg)
  - Notes (optional text, 200 chars)
  - Save button → calls `PUT /api/routes/{routeId}`
- [x] Show route metrics summary on route completion screen
- [x] Pre-fill event summary form with route aggregates when event lead taps "Complete Event" — `EditEventSummaryViewModel.TryPrefillFromRouteData()`
- [x] Show "(from route data)" indicator when summary values came from aggregation

#### Web App

- [x] Show per-route metrics in the event details route list (bags, weight, distance)
- [x] Show route aggregate totals card on event details page — `EventRouteStatsCard`
- [x] Pre-fill event summary form from route aggregates

### Phase 2 — Litter Density Color Gradient

**Goal:** Render routes with a color gradient from green (low density) to red (high density) so users and community admins can visually compare route productivity.

#### Density Calculation

Litter density is computed as:

```
density = weight_collected / total_distance_meters
```

If weight is not available, fall back to:

```
density = bags_collected / total_distance_meters
```

Units: grams per meter (g/m) or bags per kilometer (bags/km).

#### Color Scale

| Density (g/m) | Color | Hex | Meaning |
|---------------|-------|-----|---------|
| 0 (no data) | Gray | `#9E9E9E` | Route tracked but no metrics entered |
| 0.01 – 5 | Green | `#4CAF50` | Light litter |
| 5 – 15 | Yellow-Green | `#8BC34A` | Below average |
| 15 – 30 | Yellow | `#FFC107` | Average |
| 30 – 60 | Orange | `#FF9800` | Above average |
| 60 – 120 | Deep Orange | `#FF5722` | Heavy litter |
| 120+ | Red | `#F44336` | Extremely heavy |

These thresholds will be configurable via `appsettings.json` and adjustable per community (different areas have different baselines).

#### Per-Segment Coloring (Phase 2b — stretch goal)

If `RoutePoints` have timestamps and the user logs pickups at intervals, divide the route into segments between pickup timestamps and color each segment based on its local density. This requires:

- [ ] Add optional `Timestamp` to pickup log entries (new `RoutePickupLog` table)
- [ ] Divide route into segments between pickup events
- [ ] Calculate density per segment
- [ ] Render each segment as a separate polyline with its density color

#### Implementation

- [ ] Add `LitterDensityGramsPerMeter` computed property to `DisplayEventAttendeeRoute` and `DisplayAnonymizedRoute`
- [ ] Add `DensityColor` computed property using the color scale above
- [ ] Update `DisplayEventRouteStats` to include `AverageDensity` and `MaxDensity`
- [ ] **Mobile:** Render route polylines using density color instead of random/fixed color
- [ ] **Mobile:** Add density legend overlay on map (toggle-able)
- [ ] **Web:** Update Google Maps polyline rendering to use density color
- [ ] **Web:** Add density legend to route map view
- [ ] **Web:** Add density column to route list table with color indicator
- [ ] **API:** Add `GET /api/events/{eventId}/routes/density` returning per-route density data

### Phase 3 — Route End-Time Editing (Trim by Time)

**Goal:** Allow users to slide back the end time of a completed route to remove accidental post-cleanup travel.

#### How It Works

1. User completes a route and notices the distance seems too high (forgot to stop tracking while driving home)
2. User opens route details and taps "Edit Route"
3. A timeline slider shows the route duration with minute-level granularity
4. User drags the end handle backward to the time they actually stopped cleaning
5. The map preview updates in real-time, showing which RoutePoints would be removed
6. User confirms — the route is recalculated with the trimmed points

#### Backend

- [ ] Add `OriginalEndTime` (DateTimeOffset?) to `EventAttendeeRoute` — preserves the original end time before any edits
- [ ] Add `OriginalTotalDistanceMeters` (int?) to `EventAttendeeRoute` — preserves original distance
- [ ] Add `PUT /api/routes/{routeId}/trim-time` endpoint accepting `{ newEndTime: DateTimeOffset }`
  - Validates: `newEndTime` must be between `StartTime` and original `EndTime`
  - Filters `RoutePoints` to only those with `Timestamp <= newEndTime`
  - Recalculates `UserPath` LineString from remaining points
  - Recalculates `TotalDistanceMeters` via Haversine
  - Recalculates `DurationMinutes` as `(newEndTime - StartTime).TotalMinutes`
  - Stores `OriginalEndTime` and `OriginalTotalDistanceMeters` if not already set
  - Does NOT delete RoutePoints — they are retained for potential undo
- [ ] Add `PUT /api/routes/{routeId}/restore-time` endpoint to undo time trim (restores from RoutePoints)
- [ ] Add `IsTimeTrimmed` boolean to `EventAttendeeRoute` for UI indicators

#### Mobile App

- [ ] Add "Edit Route" button on route detail screen (only for own routes)
- [ ] Route editor screen with:
  - Map showing the full route (trimmed portion in dashed gray, kept portion in solid color)
  - Timeline slider (minute-level ticks) from StartTime to EndTime
  - Draggable end handle
  - Real-time distance recalculation as user drags
  - "Removed: X.X km, Y min" indicator showing what will be trimmed
  - "Save" and "Cancel" buttons
  - "Restore Original" button if route was previously trimmed
- [ ] Confirmation dialog: "This will remove {distance} from your route. Continue?"

#### Web App

- [ ] Add "Edit Route" option in route detail view (owner only)
- [ ] Route editor with Google Maps showing trim preview
- [ ] Timeline slider component with minute granularity

### Phase 4 — Smart Trim Suggestions (Nice-to-Have)

**Goal:** Automatically detect "driving away" segments and suggest a trim point.

- [ ] Analyze RoutePoints for speed anomalies:
  - Calculate speed between consecutive points: `distance / time_delta`
  - Flag segments where speed exceeds 10 km/h (configurable threshold)
  - Walking speed ~5 km/h; anything above 10 km/h is likely driving
- [ ] When user opens route editor, auto-suggest trim point at the last walking-speed point
- [ ] Show speed profile chart alongside the timeline slider
- [ ] "Auto-trim" button that applies the suggestion in one click

---

## Out of Scope

- Real-time litter density during active route tracking (this project only colors completed routes)
- Segment-level metric entry during active tracking (entering bags at GPS waypoints while walking) — deferred to a future "live pickup logging" project
- Route merging (combining two routes from the same event into one)
- Cross-event density comparison dashboards (community analytics) — belongs in a future analytics project
- Changing the route start time (only end time is editable, since starting early is rare and harmless)

---

## Success Metrics

| Metric | Target | How to Measure |
|--------|--------|----------------|
| Route metric entry adoption | ≥ 30% of routes have bags or weight entered within 3 months | Query `EventAttendeeRoute` where `BagsCollected IS NOT NULL OR WeightCollected IS NOT NULL` |
| Event summary auto-population | ≥ 50% of event summaries use route aggregate data | Query `EventSummary` where `IsFromRouteData = true` |
| Route editing usage | ≥ 10% of routes are time-trimmed | Query `EventAttendeeRoute` where `IsTimeTrimmed = true` |
| Density visualization engagement | Users spend more time on route map views | Application Insights page view duration for route map pages |
| Data quality improvement | Average route distance decreases (fewer driving segments) | Compare average `TotalDistanceMeters` before/after route editing launch |

---

## Dependencies

| Dependency | Type | Status |
|------------|------|--------|
| Project 15 (Map Route Tracing) | Required | In Progress — backend and web complete, mobile device testing remaining |
| Project 7 (Weight Tracking) | Required | Complete — `WeightUnits` table and decimal weight support |
| `RoutePoints` table with `Timestamp` | Required | Complete — migration `20260206171539` |
| Google Maps SDK (mobile + web) | Required | Already integrated |
| `EventAttendeeMetrics` approval workflow | Related | Complete — may integrate density data into lead review |

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Density thresholds are arbitrary and don't match real-world litter patterns | Medium | Medium | Make thresholds configurable per community; collect data for 3 months then adjust based on actual distribution |
| Users enter incorrect weight/bags (typos, wrong units) | Medium | Low | Show confirmation with unit conversion ("5 kg = 11 lbs — is this correct?"); event leads can review/adjust via existing metrics approval workflow |
| Route time trimming could lose legitimate data | Low | Medium | Never delete RoutePoints — only filter by timestamp; provide "Restore Original" undo |
| Multi-color polyline rendering is slow for long routes | Medium | Medium | Limit per-segment coloring to routes < 500 points; for longer routes, use single average density color |
| Mixed weight units across routes in same event | Medium | Low | Convert all weights to a common unit (grams) for density calculation; display in user's preferred unit |

---

## Implementation Plan

### Database Changes

#### New Migration: `AddRouteMetricsEnhancements`

```sql
-- Add weight unit to routes (currently only has decimal WeightCollected with no unit)
ALTER TABLE EventAttendeeRoutes ADD WeightUnitId INT NULL;
ALTER TABLE EventAttendeeRoutes ADD CONSTRAINT FK_EventAttendeeRoutes_WeightUnits
    FOREIGN KEY (WeightUnitId) REFERENCES WeightUnits(Id);

-- Route time-trim tracking
ALTER TABLE EventAttendeeRoutes ADD OriginalEndTime DATETIMEOFFSET NULL;
ALTER TABLE EventAttendeeRoutes ADD OriginalTotalDistanceMeters INT NULL;
ALTER TABLE EventAttendeeRoutes ADD IsTimeTrimmed BIT NOT NULL DEFAULT 0;

-- Event summary auto-population tracking
ALTER TABLE EventSummaries ADD IsFromRouteData BIT NOT NULL DEFAULT 0;
```

#### Optional Migration: `AddRoutePickupLog` (Phase 2b only)

```sql
CREATE TABLE RoutePickupLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    RouteId UNIQUEIDENTIFIER NOT NULL,
    Timestamp DATETIMEOFFSET NOT NULL,
    BagsCollected INT NULL,
    WeightCollected DECIMAL(18,2) NULL,
    WeightUnitId INT NULL,
    Notes NVARCHAR(500) NULL,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    CONSTRAINT FK_RoutePickupLogs_Route FOREIGN KEY (RouteId)
        REFERENCES EventAttendeeRoutes(Id) ON DELETE CASCADE
);
CREATE INDEX IX_RoutePickupLogs_RouteId ON RoutePickupLogs(RouteId);
```

### API Endpoints (New)

| Method | Path | Purpose |
|--------|------|---------|
| `PUT` | `/api/routes/{routeId}/metrics` | Update bags, weight, weight unit, notes for a route |
| `GET` | `/api/events/{eventId}/routes/aggregate` | Get summed metrics across all event routes |
| `GET` | `/api/events/{eventId}/routes/density` | Get per-route density data with colors |
| `PUT` | `/api/routes/{routeId}/trim-time` | Trim route end time, recalculate distance |
| `PUT` | `/api/routes/{routeId}/restore-time` | Undo time trim, restore original route |

### Density Color Algorithm (Pseudocode)

```csharp
public static string GetDensityColor(decimal? weightGrams, int distanceMeters)
{
    if (weightGrams is null || weightGrams == 0 || distanceMeters == 0)
        return "#9E9E9E"; // Gray — no data

    var density = (double)weightGrams / distanceMeters; // g/m

    return density switch
    {
        <= 5   => "#4CAF50",  // Green — light
        <= 15  => "#8BC34A",  // Yellow-green
        <= 30  => "#FFC107",  // Yellow — average
        <= 60  => "#FF9800",  // Orange — above average
        <= 120 => "#FF5722",  // Deep orange — heavy
        _      => "#F44336",  // Red — extremely heavy
    };
}
```

### Route Time Trim Algorithm (Pseudocode)

```csharp
public async Task<ServiceResult<EventAttendeeRoute>> TrimRouteByTime(
    Guid routeId, DateTimeOffset newEndTime)
{
    var route = await repository.GetAsync(routeId);
    var routePoints = await routePointRepo.GetByRouteIdAsync(routeId);

    // Preserve originals on first trim
    route.OriginalEndTime ??= route.EndTime;
    route.OriginalTotalDistanceMeters ??= route.TotalDistanceMeters;

    // Filter points to those within new time window
    var keptPoints = routePoints
        .Where(p => p.Timestamp <= newEndTime)
        .OrderBy(p => p.Timestamp)
        .ToList();

    // Rebuild LineString from kept points
    var coords = keptPoints
        .Select(p => new Coordinate(p.Longitude, p.Latitude))
        .ToArray();
    route.UserPath = new LineString(coords);

    // Recalculate metrics
    route.EndTime = newEndTime;
    route.TotalDistanceMeters = CalculateHaversineDistance(route.UserPath);
    route.DurationMinutes = (int)(newEndTime - route.StartTime).TotalMinutes;
    route.IsTimeTrimmed = true;

    await repository.UpdateAsync(route);
    return ServiceResult<EventAttendeeRoute>.Success(route);
}
```

---

## Implementation Phases

### Phase 1 — Route-Level Metrics & Summary Rollup (2-3 weeks)
1. Database migration: add `WeightUnitId`, `IsFromRouteData`
2. Backend: metrics update endpoint, aggregation service, summary pre-fill
3. Mobile: "Log Pickup" screen, completion summary, event summary pre-fill
4. Web: per-route metrics display, aggregate card, summary pre-fill
5. Tests: unit tests for aggregation logic, API integration tests

### Phase 2 — Density Color Gradient (2 weeks)
1. Backend: density calculation, color assignment, density API endpoint
2. Mobile: multi-color polyline rendering, density legend
3. Web: Google Maps polyline coloring, density legend, density column in route list
4. Tests: density calculation unit tests, color threshold tests

### Phase 3 — Route End-Time Editing (2-3 weeks)
1. Database migration: add `OriginalEndTime`, `OriginalTotalDistanceMeters`, `IsTimeTrimmed`
2. Backend: trim-time endpoint, restore-time endpoint, validation
3. Mobile: route editor screen with timeline slider, map preview, undo
4. Web: route editor with Google Maps trim preview
5. Tests: trim/restore round-trip tests, boundary validation tests

### Phase 4 — Smart Trim Suggestions (1 week, nice-to-have)
1. Backend: speed analysis algorithm, suggestion endpoint
2. Mobile/Web: auto-trim button, speed profile display

---

## Open Questions

1. ~~**Weight unit normalization:** When aggregating routes with mixed units (some lb, some kg), should we convert to a standard unit (grams) internally?~~ → **Resolved:** Yes, store and calculate in grams internally; display in user's preferred unit.

2. ~~**Density baseline per community:** Should communities be able to set their own density thresholds?~~ → **Resolved:** Yes, add optional `DensityThresholds` JSON column to `Partners` table; fall back to global defaults.

3. ~~**Existing routes without metrics:** How should routes without bags/weight data appear on the density map?~~ → **Resolved:** Gray color (#9E9E9E) with "No data" tooltip.

4. ~~**Time trim granularity:** Minutes are proposed. Should we support finer granularity (seconds)?~~ → **Resolved:** Minute-level is sufficient. Finer granularity adds UI complexity without meaningful benefit.

5. ~~**Route editing after event summary submission:** Should route metrics be locked once the event lead submits the summary?~~ → **Resolved:** No lock. Allow edits, but if `IsFromRouteData = true` and any underlying route metric changes after the summary was last updated, show a visual indicator on the event summary ("Route data updated — Refresh totals?"). Event lead can tap to re-aggregate or ignore. This avoids silently overwriting manual adjustments while making it easy to pick up changes.

---

## Related Documents

- [Project 15 — Map Route Tracing](Project_15_Map_Route_Tracing.md) — Base route tracking implementation
- [Project 7 — Weight Tracking](Project_07_Weight_Tracking.md) — Weight units and decimal weight support
- `TrashMob.Models/EventAttendeeRoute.cs` — Route entity (already has `BagsCollected`, `WeightCollected`)
- `TrashMob.Models/RoutePoint.cs` — GPS waypoint entity (already has `Timestamp`)
- `TrashMob.Models/EventSummary.cs` — Event-level aggregate metrics
- `TrashMob.Shared/Managers/Events/EventAttendeeRouteManager.cs` — Route business logic

---

## GitHub Issues

_To be created on project start._

---

**Last Updated:** February 22, 2026
**Owner:** @JoeBeernink
**Status:** In Progress (Phase 1 Complete)
**Next Review:** Phase 2 kickoff
