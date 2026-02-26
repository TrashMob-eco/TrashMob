# Project 22 — Attendee-Level Event Metrics

| Attribute | Value |
|-----------|-------|
| **Status** | ✅ Complete |
| **Priority** | Medium |
| **Risk** | Medium |
| **Size** | Medium |
| **Dependencies** | Project 7 (Event Weights) |

---

## Business Rationale

Let attendees enter personal stats and give leads tools to reconcile without double counting. Currently, only event-level totals are tracked, which doesn't capture individual contributions or enable per-person gamification.

---

## Objectives

### Primary Goals
- **Attendee entries** for bags, weight, and time
- **Reconciliation workflow** for event leads
- **Event leaderboards** showing top contributors
- **Bag drop location notes** and alerts

### Secondary Goals
- Photo association with metrics
- Route-based metric attribution
- Team-level roll-ups
- Export for volunteer hour tracking

---

## Scope

### Phase 1 - Backend Infrastructure ✅ Complete
- ✅ `EventAttendeeMetrics` model with approval workflow
- ✅ API endpoints for attendee submission (`/my-metrics`)
- ✅ API endpoints for lead operations (approve/reject/adjust)
- ✅ Totals calculation with weight conversion
- ✅ Bulk approve-all functionality

### Phase 2 - Frontend UI ✅ Complete
- ✅ Attendee submission form on event page
- ✅ Lead review dashboard with approve/reject/adjust actions
- ✅ Totals display in lead review dashboard

### Phase 3 - Display ✅ Complete
- ✅ Event summary shows breakdown
- ✅ Per-attendee contributions visible
- ✅ Dashboard shows personal impact
- ✅ Event leaderboards

### Phase 4 - Integration
- ✅ Gamification integration (Project 20) — AchievementProcessor and LeaderboardGenerator use EventAttendeeMetrics
- ✅ Team roll-ups (Project 9) — Team leaderboards aggregate member EventAttendeeMetrics
- ⏸️ Route association (Project 15) — Deferred until routes are routinely used
- ❌ Photo association (Project 18) — Dropped; photos already linked to user via UploadedByUserId

---

## Out-of-Scope

- ❌ Volunteer hour certification
- ❌ Third-party volunteer tracking integration
- ❌ Compensation/payment tracking
- ❌ Insurance/liability calculations

---

## Success Metrics

### Quantitative
- **Events with attendee metrics:** ≥ 50% of completed events
- **Attendee participation in metrics:** ≥ 60% of attendees enter stats
- **Reconciliation completion rate:** ≥ 80% of events reconciled by lead
- **Data quality issues:** < 5% of entries need adjustment

### Qualitative
- Volunteers feel recognized for contributions
- Leads find reconciliation intuitive
- Accurate impact reporting

---

## Dependencies

### Blockers
- **Project 7 (Event Weights):** Weight metrics infrastructure

### Enables
- **Project 20 (Gamification):** Individual stats for leaderboards
- **Project 15 (Route Tracing):** Route-attributed metrics
- **Volunteer recognition:** Individual impact tracking

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Inflated self-reporting** | High | Medium | Lead verification; anomaly detection; peer comparison |
| **Double counting** | Medium | Medium | Reconciliation workflow; clear attribution rules |
| **Low adoption** | Medium | Medium | Easy UI; gamification incentives; reminders |
| **Complexity for leads** | Medium | Medium | Smart defaults; auto-calculation; simple approve flow |

---

## Implementation Plan

### Data Model Changes

> **Note:** This project builds on Project 7's `EventSummaryAttendee` for Phase 2 (attendee-level weight).
> Consider whether `EventAttendeeMetrics` should replace or extend `EventSummaryAttendee`.

> **Unattributed Totals:** When partial reporting occurs, unattributed totals (for non-reporting attendees)
> are stored in `EventSummary.UnattributedBags`, `EventSummary.UnattributedWeight`, etc. The event's final
> totals = sum of approved attendee metrics + unattributed amounts.

**New Entity: EventAttendeeMetrics**
```csharp
// New file: TrashMob.Models/EventAttendeeMetrics.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents an attendee's self-reported metrics for an event, subject to lead verification.
    /// </summary>
    public class EventAttendeeMetrics : KeyedModel
    {
        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the attendee's user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        #region Reported Metrics

        /// <summary>
        /// Gets or sets the number of bags collected by this attendee.
        /// </summary>
        public int? BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the weight collected by this attendee.
        /// </summary>
        public decimal? WeightCollected { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the weight unit used.
        /// </summary>
        public int? WeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes this attendee participated.
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets any notes from the attendee about their contribution.
        /// </summary>
        public string Notes { get; set; }

        #endregion

        #region Verification

        /// <summary>
        /// Gets or sets the verification status (Pending, Approved, Adjusted, Rejected).
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the adjusted bag count (if lead modified the original).
        /// </summary>
        public int? AdjustedBags { get; set; }

        /// <summary>
        /// Gets or sets the adjusted weight (if lead modified the original).
        /// </summary>
        public decimal? AdjustedWeight { get; set; }

        /// <summary>
        /// Gets or sets the adjusted duration (if lead modified the original).
        /// </summary>
        public int? AdjustedDuration { get; set; }

        /// <summary>
        /// Gets or sets notes explaining any adjustments made by the lead.
        /// </summary>
        public string AdjustmentNotes { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the lead who reviewed this entry.
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date this entry was reviewed.
        /// </summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        #endregion

        #region Privacy

        /// <summary>
        /// Gets or sets whether this attendee's metrics are publicly visible.
        /// Default is true; attendee can opt-out to hide their contributions.
        /// </summary>
        public bool IsPublic { get; set; } = true;

        #endregion

        // Navigation properties
        public virtual Event Event { get; set; }
        public virtual User User { get; set; }
        public virtual WeightUnit WeightUnit { get; set; }
        public virtual User ReviewedByUser { get; set; }
        public virtual ICollection<AttendeeDropLocation> DropLocations { get; set; }
    }
}
```

**New Entity: AttendeeDropLocation**
```csharp
// New file: TrashMob.Models/AttendeeDropLocation.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a location where an attendee dropped bags for pickup coordination.
    /// </summary>
    public class AttendeeDropLocation : KeyedModel
    {
        /// <summary>
        /// Gets or sets the parent metrics entry identifier.
        /// </summary>
        public Guid EventAttendeeMetricsId { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the drop location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the drop location.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the number of bags at this drop location.
        /// </summary>
        public int BagCount { get; set; } = 1;

        /// <summary>
        /// Gets or sets notes about this drop location (e.g., landmarks, access info).
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the URL of a photo of the drop location.
        /// </summary>
        public string ImageUrl { get; set; }

        // Navigation property
        public virtual EventAttendeeMetrics EventAttendeeMetrics { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<EventAttendeeMetrics>(entity =>
{
    entity.Property(e => e.Notes).HasMaxLength(500);
    entity.Property(e => e.Status).HasMaxLength(20);
    entity.Property(e => e.AdjustmentNotes).HasMaxLength(500);
    entity.Property(e => e.WeightCollected).HasPrecision(10, 2);
    entity.Property(e => e.AdjustedWeight).HasPrecision(10, 2);

    entity.HasOne(e => e.Event)
        .WithMany()
        .HasForeignKey(e => e.EventId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.WeightUnit)
        .WithMany()
        .HasForeignKey(e => e.WeightUnitId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.ReviewedByUser)
        .WithMany()
        .HasForeignKey(e => e.ReviewedByUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.EventId);
    entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => e.Status);
    entity.HasIndex(e => new { e.EventId, e.UserId }).IsUnique();
});

modelBuilder.Entity<AttendeeDropLocation>(entity =>
{
    entity.Property(e => e.Notes).HasMaxLength(200);
    entity.Property(e => e.ImageUrl).HasMaxLength(500);

    entity.HasOne(e => e.EventAttendeeMetrics)
        .WithMany(m => m.DropLocations)
        .HasForeignKey(e => e.EventAttendeeMetricsId)
        .OnDelete(DeleteBehavior.Cascade);
});
```

### API Changes

```csharp
// Submit attendee metrics
[Authorize]
[HttpPost("api/events/{eventId}/my-metrics")]
public async Task<ActionResult<AttendeeMetricsDto>> SubmitMyMetrics(
    Guid eventId, [FromBody] SubmitMetricsRequest request)
{
    // Validate user attended event
    // Create or update metrics entry
}

// Get my metrics for event
[Authorize]
[HttpGet("api/events/{eventId}/my-metrics")]
public async Task<ActionResult<AttendeeMetricsDto>> GetMyMetrics(Guid eventId)
{
    // Return user's metrics if submitted
}

// Get all attendee metrics (for leads)
[Authorize]
[HttpGet("api/events/{eventId}/attendee-metrics")]
public async Task<ActionResult<IEnumerable<AttendeeMetricsDto>>> GetAllAttendeeMetrics(Guid eventId)
{
    // Validate caller is event lead
    // Return all submissions
}

// Review/adjust metrics (for leads)
[Authorize]
[HttpPut("api/events/{eventId}/attendee-metrics/{userId}")]
public async Task<ActionResult> ReviewMetrics(
    Guid eventId, Guid userId, [FromBody] ReviewMetricsRequest request)
{
    // Validate caller is event lead
    // Approve, adjust, or reject
}

// Bulk approve all pending
[Authorize]
[HttpPost("api/events/{eventId}/attendee-metrics/approve-all")]
public async Task<ActionResult> ApproveAllMetrics(Guid eventId)
{
    // Validate caller is event lead
    // Approve all pending without adjustments
}

// Calculate event totals from attendee metrics
[Authorize]
[HttpPost("api/events/{eventId}/calculate-totals")]
public async Task<ActionResult<EventSummaryDto>> CalculateTotals(Guid eventId)
{
    // Sum approved attendee metrics
    // Update EventSummary
}

// Report bag drop location
[Authorize]
[HttpPost("api/events/{eventId}/my-metrics/drop-locations")]
public async Task<ActionResult<DropLocationDto>> ReportDropLocation(
    Guid eventId, [FromBody] DropLocationRequest request)
{
    // Record bag drop with location
}

// Get drop locations for event (for pickup coordination)
[HttpGet("api/events/{eventId}/drop-locations")]
public async Task<ActionResult<IEnumerable<DropLocationDto>>> GetDropLocations(Guid eventId)
{
    // Return all reported drop locations with map data
}
```

### Web UX Changes

**Attendee Post-Event:**
- "Report Your Impact" prompt after event
- Simple form: bags, weight, time
- Optional notes
- Drop location reporting with map pin
- Photo upload

**Attendee Notifications:**
- **Email reminder:** Sent 24 hours after event ends ("Report your impact from [Event Name]!")
- **In-app banner:** Dashboard shows "You have unreported metrics" for recent events
- **Follow-up email:** Sent at day 5 if not yet submitted ("2 days left to report...")
- Link directly to metrics submission form

**Event Lead Reconciliation:**
- **Reporting status:** "3 of 10 attendees reported metrics" prominently displayed
- List of attendee submissions
- Comparison view (claimed vs. adjusted)
- Quick approve/adjust actions
- **Unattributed totals:** Input fields for bags/weight not reported by specific attendees
- Auto-calculate totals button (sums approved + unattributed)
- Warning when reporting rate < 50%
- Notes for adjustments

**Event Summary Display:**
- Total metrics (as before)
- Breakdown by attendee (optional view)
- Top contributors highlight
- "X attendees reported metrics"

**User Dashboard:**
- Personal impact totals
- Per-event breakdown
- Trend charts

### Mobile App Changes

- Post-event metric entry prompt
- Drop location with GPS
- Photo capture for drop points
- View personal stats

---

## Implementation Phases

### Phase 1: Attendee Entry
- Database schema
- Submit metrics API
- Basic UI for entry
- Dashboard display

### Phase 2: Lead Reconciliation
- Review API
- Reconciliation UI
- Total calculation
- Notification to attendees

### Phase 3: Drop Locations
- Location reporting
- Map display
- Pickup coordination

### Phase 4: Integration
- Gamification integration
- Route association
- Team roll-ups

---

## Reconciliation Rules

1. **Default Behavior:**
   - If no attendee metrics: Lead enters event-level totals only
   - If attendee metrics: Auto-sum approved entries + unattributed totals

2. **Partial Reporting:**
   - UI clearly displays "X of Y attendees reported metrics"
   - Lead can enter **unattributed totals** for non-reporting attendees
   - Event totals = Sum of approved attendee entries + unattributed amounts
   - Warning displayed when calculating totals with < 50% reporting rate

3. **Double Counting Prevention:**
   - Lead can mark entries as "shared" (e.g., two people carried same bag)
   - Adjusted values used for totals
   - Original values preserved for audit

4. **Late Submissions:**
   - Attendees can submit up to 7 days after event
   - Leads notified of new submissions
   - Late submissions don't auto-recalculate totals

---

## Data Migration

Existing events with EventSummary data need migration to populate EventAttendeeMetrics:

```csharp
// Migration logic for existing events
foreach (var eventSummary in existingEventSummaries)
{
    var attendees = await GetEventAttendees(eventSummary.EventId);

    if (attendees.Count == 1)
    {
        // Single attendee: attribute all metrics to them
        CreateAttendeeMetrics(attendees[0], eventSummary.TotalBags, eventSummary.TotalWeight);
    }
    else if (attendees.Count > 1)
    {
        // Multiple attendees: split equally
        var bagsPerAttendee = eventSummary.TotalBags / attendees.Count;
        var weightPerAttendee = eventSummary.TotalWeight / attendees.Count;

        foreach (var attendee in attendees)
        {
            CreateAttendeeMetrics(attendee, bagsPerAttendee, weightPerAttendee);
        }

        // Handle remainder as unattributed
        var remainderBags = eventSummary.TotalBags % attendees.Count;
        if (remainderBags > 0)
            eventSummary.UnattributedBags = remainderBags;
    }

    // Mark migrated entries as "Approved" (already verified by lead)
    SetStatus("Approved");
}
```

**Migration Notes:**
- All migrated entries marked as "Approved" status (lead already submitted the totals)
- Fractional weights rounded; remainder stored as unattributed
- Events with 0 attendees: metrics remain event-level only (unattributed)
- Migration runs once during deployment; new events use new workflow

---

## Resolved Questions

1. **Time window for attendee submissions?**
   **Decision:** 7 days post-event by default; event leads can extend the deadline if needed

2. **Default metric values?**
   **Decision:** No defaults; require entry if participating in metrics tracking

3. **Visibility of individual metrics?**
   **Decision:** Public by default; privacy option available for attendees to hide their contributions

4. **Integration with volunteer hour systems?**
   **Decision:** Export feature only; no direct third-party integration for v1

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2241](https://github.com/trashmob/TrashMob/issues/2241)** - Project 22: Attendee Level Event Metrics (tracking issue)
- **[#2263](https://github.com/trashmob/TrashMob/issues/2263)** - Update Data Model to Allow event participants to contribute to event summary
- **[#2264](https://github.com/trashmob/TrashMob/issues/2264)** - Update API's to allow for management of attendee-level metrics for an event
- **[#2265](https://github.com/trashmob/TrashMob/issues/2265)** - Update Mobile app to allow for individual metrics
- **[#2266](https://github.com/trashmob/TrashMob/issues/2266)** - Update Web UX to allow for individual metrics
- **[#2267](https://github.com/trashmob/TrashMob/issues/2267)** - Update Mobile app design for individual metrics
- **[#2268](https://github.com/trashmob/TrashMob/issues/2268)** - Update Web app design for individual metrics

---

## Related Documents

- **[Project 7 - Event Weights](./Project_07_Event_Weights.md)** - Event-level weight tracking
- **[Project 15 - Route Tracing](./Project_15_Route_Tracing.md)** - Route-attributed metrics
- **[Project 20 - Gamification](./Project_20_Gamification.md)** - Individual stats for rankings

---

**Last Updated:** February 24, 2026
**Owner:** Product Lead + Engineering
**Status:** ✅ Complete (Phases 1-4; route association deferred)
**Next Review:** Closed
