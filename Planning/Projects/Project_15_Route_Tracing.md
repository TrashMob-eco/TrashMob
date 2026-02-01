# Project 15 — Map Route Tracing with Decay

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in Progress |
| **Priority** | Medium |
| **Risk** | High (privacy) |
| **Size** | Large |
| **Dependencies** | Project 4 (Mobile Robustness), Project 22 (Attendee Metrics) |

---

## Business Rationale

Record and share anonymized routes that attendees walk during cleanup events; enable filters, editing, and association of metrics and notes. Route data helps demonstrate impact and prevents duplicate effort in adjacent areas.

---

## Objectives

### Primary Goals
- **Record routes** during active event participation
- **Trim/edit routes** to remove sensitive start/end points
- **Anonymized overlays** showing areas covered
- **Associate metrics** (bags, weight, notes) with route segments
- **Privacy controls** and sharing options

### Secondary Goals
- Heat maps of frequently cleaned areas
- Route suggestions for new events
- Integration with community adopt-a programs
- Route gamification (distance, coverage)

---

## Scope

### Phase 1 - Route Recording
- ✅ Start/stop route recording during event
- ✅ Background GPS tracking (battery optimized)
- ✅ Local storage until event completion
- ✅ Upload route to server

### Phase 2 - Route Management
- ✅ View recorded routes on map
- ✅ Trim start/end points for privacy
- ✅ Delete route entirely
- ✅ Associate route with event

### Phase 3 - Route Sharing
- ✅ Anonymized route overlays on event maps
- ✅ Event summary shows combined routes
- ✅ Privacy settings (private, event-only, public)
- ✅ Route decay (fade over time)

### Phase 4 - Analytics
- ✅ Heat maps of cleaned areas
- ✅ Coverage statistics
- ✅ Route segment metrics

---

## Out-of-Scope

- ❌ Real-time location sharing during events
- ❌ Turn-by-turn navigation
- ❌ Route planning/optimization
- ❌ Third-party fitness app integration (Strava, etc.)
- ❌ Live tracking by event leads

---

## Success Metrics

### Quantitative
- **Route adoption:** ≥ 30% of event attendees record routes
- **Privacy satisfaction:** < 1% complaints about route privacy
- **Battery impact:** < 5% additional drain during recording

### Qualitative
- Users find route recording intuitive
- Route maps add value to event summaries
- No privacy incidents

---

## Dependencies

### Blockers
- **Project 4 (Mobile Robustness):** Stable mobile app for GPS tracking
- **Project 22 (Attendee Metrics):** Required for route-associated litter volume tracking (bags, weight per route segment)

### Enables
- **Project 20 (Gamification):** Distance-based achievements

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Privacy violations** | Medium | Critical | Auto-trim; user control; decay; anonymization |
| **Battery drain** | Medium | High | Optimized GPS sampling; clear battery warnings |
| **GPS accuracy issues** | Medium | Medium | Smoothing algorithms; manual editing |
| **Large data storage** | Medium | Medium | Compression; decay/deletion policies |
| **User doesn't stop recording** | Medium | Low | Auto-stop based on event end time; max duration |

---

## Implementation Plan

### Data Model Changes

> **Note:** `EventAttendeeRoute` already exists in `TrashMob.Models/EventAttendeeRoute.cs` with basic fields.
> The following shows additional properties needed for the full feature set.

**Modification: EventAttendeeRoute (add new properties)**
```csharp
// Add to existing TrashMob.Models/EventAttendeeRoute.cs
#region Distance & Duration

/// <summary>
/// Gets or sets the total distance in meters.
/// </summary>
public int TotalDistanceMeters { get; set; }

/// <summary>
/// Gets or sets the duration in minutes.
/// </summary>
public int DurationMinutes { get; set; }

#endregion

#region Privacy Settings

/// <summary>
/// Gets or sets the privacy level (Private, EventOnly, Public).
/// </summary>
public string PrivacyLevel { get; set; } = "EventOnly";

/// <summary>
/// Gets or sets whether the route has been trimmed for privacy.
/// </summary>
public bool IsTrimmed { get; set; }

/// <summary>
/// Gets or sets meters trimmed from the start.
/// </summary>
public int TrimStartMeters { get; set; }

/// <summary>
/// Gets or sets meters trimmed from the end.
/// </summary>
public int TrimEndMeters { get; set; }

#endregion

#region Route Metrics

/// <summary>
/// Gets or sets bags collected along this route.
/// </summary>
public int? BagsCollected { get; set; }

/// <summary>
/// Gets or sets weight collected along this route.
/// </summary>
public decimal? WeightCollected { get; set; }

/// <summary>
/// Gets or sets notes about this route.
/// </summary>
public string Notes { get; set; }

#endregion

#region Decay

/// <summary>
/// Gets or sets when this route expires for public viewing.
/// </summary>
public DateTimeOffset? ExpiresDate { get; set; }

#endregion

// Navigation property for route points
public virtual ICollection<RoutePoint> RoutePoints { get; set; }
```

**New Entity: RoutePoint (optional detailed tracking)**
```csharp
// New file: TrashMob.Models/RoutePoint.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a single GPS point along an attendee's route.
    /// </summary>
    public class RoutePoint
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the parent route identifier.
        /// </summary>
        public Guid RouteId { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the altitude in meters.
        /// </summary>
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of this point.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the GPS accuracy in meters.
        /// </summary>
        public double? Accuracy { get; set; }

        // Navigation property
        public virtual EventAttendeeRoute Route { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<EventAttendeeRoute>(entity =>
{
    // Add to existing configuration
    entity.Property(e => e.PrivacyLevel).HasMaxLength(20);
    entity.Property(e => e.WeightCollected).HasPrecision(10, 2);

    entity.HasIndex(e => e.EventId);
    entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => e.PrivacyLevel);
});

modelBuilder.Entity<RoutePoint>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).UseIdentityColumn();

    entity.HasOne(e => e.Route)
        .WithMany(r => r.RoutePoints)
        .HasForeignKey(e => e.RouteId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasIndex(e => e.RouteId);
});
```

### API Changes

```csharp
// Upload route
[Authorize]
[HttpPost("api/events/{eventId}/routes")]
public async Task<ActionResult<RouteDto>> UploadRoute(
    Guid eventId, [FromBody] UploadRouteRequest request)
{
    // Validate user is attendee
    // Process and store route
    // Calculate distance/duration
}

// Get routes for event (respecting privacy)
[HttpGet("api/events/{eventId}/routes")]
public async Task<ActionResult<EventRoutesDto>> GetEventRoutes(Guid eventId)
{
    // Return anonymized combined routes
    // Individual routes only if privacy allows
}

// Get user's routes
[Authorize]
[HttpGet("api/users/me/routes")]
public async Task<ActionResult<IEnumerable<RouteDto>>> GetMyRoutes()
{
    // Return user's route history
}

// Update route (trim, privacy, notes)
[Authorize]
[HttpPut("api/routes/{routeId}")]
public async Task<ActionResult<RouteDto>> UpdateRoute(
    Guid routeId, [FromBody] UpdateRouteRequest request)
{
    // Validate ownership
    // Apply updates
}

// Delete route
[Authorize]
[HttpDelete("api/routes/{routeId}")]
public async Task<ActionResult> DeleteRoute(Guid routeId)
{
    // Validate ownership
    // Soft or hard delete based on policy
}

// Heat map data (aggregated, anonymized)
[HttpGet("api/areas/{areaId}/heatmap")]
public async Task<ActionResult<HeatmapDto>> GetAreaHeatmap(
    Guid areaId, [FromQuery] DateRange range)
{
    // Return aggregated route density data
}
```

### Mobile App Changes

**Route Recording:**
```csharp
// MAUI location tracking service
public class RouteRecordingService
{
    private List<RoutePoint> _currentRoute;
    private bool _isRecording;

    public async Task StartRecording(Guid eventId)
    {
        // Request location permissions
        // Start background GPS tracking
        // Optimize for battery (adaptive sampling rate)
    }

    public async Task StopRecording()
    {
        // Stop GPS tracking
        // Apply initial privacy trimming
        // Queue for upload
    }
}
```

**UI Components:**
- Start/stop recording button on event
- Recording indicator in status bar
- Route preview before upload
- Trim controls (slider for start/end)
- Privacy level selector

### Web UX Changes (Read-Only)

> **Note:** Web is view-only for routes. All route recording, editing, and privacy controls happen in the mobile app.

- Event map showing combined routes
- Individual route viewer (for own routes)
- Route statistics in event summary
- Heat map visualization (admin/community)

---

## Implementation Phases

### Phase 1: Mobile Recording
- GPS tracking service
- Local storage
- Basic upload

### Phase 2: Route Management
- Trim/edit UI
- Privacy controls
- Server storage

### Phase 3: Visualization
- Event route overlays
- Combined views
- Decay implementation

### Phase 4: Analytics
- Heat maps
- Coverage statistics
- Integration with metrics

**Note:** Privacy is critical; extensive testing and user feedback required.

### Current Implementation State
- **Mobile app:** Partial implementation exists but untested due to lack of route simulation
- **Priority:** Complete mobile app implementation first, then add web read-only views
- **Web:** Read-only - no route creation or editing (mobile-only feature)

### Testing Requirements
- **Route simulation tool** for development/QA testing without physically walking routes
- Generate synthetic routes with configurable parameters (distance, duration, accuracy)
- Simulate various GPS conditions (urban canyons, tunnels, poor signal)
- Seed test data for heat map visualization

---

## Privacy Considerations

1. **Default Privacy:** Routes are "EventOnly" by default (visible only on event map)
2. **Auto-Trim:** Automatically remove first/last 100m of routes
3. **Manual Trim:** Users can adjust trim points
4. **Decay:** Routes fade from public view after configurable period
5. **Anonymization:** Combined event maps don't identify individual routes
6. **Delete:** Users can delete routes at any time
7. **No Real-Time:** Routes uploaded only after event completion

---

## Open Questions

1. ~~**Default decay period?**~~
   **Decision:** 2 years for public routes (enables year-over-year comparison); indefinite for private routes
   **Status:** ✅ Resolved

2. ~~**GPS sampling rate?**~~
   **Decision:** Adaptive rate: 10s stationary, 5s walking, 3s running (balances accuracy and battery)
   **Status:** ✅ Resolved

3. ~~**Route data format?**~~
   **Decision:** Encoded polyline for compact storage; GeoJSON for detailed analysis/export
   **Status:** ✅ Resolved

4. ~~**Heat map granularity?**~~
   **Decision:** 25m grid cells; minimum 10 routes to display (finer detail, more privacy-preserving)
   **Status:** ✅ Resolved

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#1202](https://github.com/trashmob/TrashMob/issues/1202)** - Project 15: Map Route Tracing with Decay (tracking issue)

---

## Related Documents

- **[Project 4 - Mobile Robustness](./Project_04_Mobile_Robustness.md)** - Stable GPS/background
- **[Project 20 - Gamification](./Project_20_Gamification.md)** - Distance achievements
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Route-based metrics

---

**Last Updated:** January 31, 2026
**Owner:** Mobile Team + Backend
**Status:** Planning in Progress
**Next Review:** When Project 4 complete
