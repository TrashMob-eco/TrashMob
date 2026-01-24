# Project 22 — Attendee-Level Event Metrics

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
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

### Phase 1 - Attendee Entry
- ✅ Attendees can enter their own metrics
- ✅ Bags collected (count)
- ✅ Weight collected (with units)
- ✅ Time spent (duration)
- ✅ Notes/comments

### Phase 2 - Reconciliation
- ✅ Lead sees all attendee entries
- ✅ Approve/adjust individual entries
- ✅ Auto-calculate event totals
- ✅ Handle double-counting conflicts

### Phase 3 - Display
- ✅ Event summary shows breakdown
- ✅ Per-attendee contributions visible
- ✅ Dashboard shows personal impact
- ✅ Event leaderboards

### Phase 4 - Integration
- ❓ Route association (Project 15)
- ❓ Photo association (Project 18)
- ❓ Gamification integration (Project 20)
- ❓ Team roll-ups (Project 9)

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

```sql
-- Attendee event metrics
CREATE TABLE EventAttendeeMetrics (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    -- Metrics
    BagsCollected INT NULL,
    WeightCollected DECIMAL(10,2) NULL,
    WeightUnitId INT NULL,
    DurationMinutes INT NULL,
    Notes NVARCHAR(500) NULL,
    -- Verification
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Adjusted, Rejected
    AdjustedBags INT NULL,
    AdjustedWeight DECIMAL(10,2) NULL,
    AdjustedDuration INT NULL,
    AdjustmentNotes NVARCHAR(500) NULL,
    ReviewedByUserId UNIQUEIDENTIFIER NULL,
    ReviewedDate DATETIMEOFFSET NULL,
    -- Audit
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    LastUpdatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (WeightUnitId) REFERENCES WeightUnits(Id),
    FOREIGN KEY (ReviewedByUserId) REFERENCES Users(Id),
    UNIQUE (EventId, UserId)
);

-- Bag drop locations (for pickup coordination)
CREATE TABLE AttendeeDropLocations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventAttendeeMetricsId UNIQUEIDENTIFIER NOT NULL,
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    BagCount INT NOT NULL DEFAULT 1,
    Notes NVARCHAR(200) NULL,
    ImageUrl NVARCHAR(500) NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (EventAttendeeMetricsId) REFERENCES EventAttendeeMetrics(Id) ON DELETE CASCADE
);

CREATE INDEX IX_EventAttendeeMetrics_EventId ON EventAttendeeMetrics(EventId);
CREATE INDEX IX_EventAttendeeMetrics_UserId ON EventAttendeeMetrics(UserId);
CREATE INDEX IX_EventAttendeeMetrics_Status ON EventAttendeeMetrics(Status);
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

**Event Lead Reconciliation:**
- List of attendee submissions
- Comparison view (claimed vs. adjusted)
- Quick approve/adjust actions
- Auto-calculate totals button
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
   - If attendee metrics: Auto-sum approved entries

2. **Double Counting Prevention:**
   - Lead can mark entries as "shared" (e.g., two people carried same bag)
   - Adjusted values used for totals
   - Original values preserved for audit

3. **Late Submissions:**
   - Attendees can submit up to 7 days after event
   - Leads notified of new submissions
   - Late submissions don't auto-recalculate totals

---

## Open Questions

1. **Time window for attendee submissions?**
   **Recommendation:** 7 days post-event; leads can extend
   **Owner:** Product Lead
   **Due:** Before Phase 1

2. **Default metric values?**
   **Recommendation:** No defaults; require entry if participating
   **Owner:** Product Lead
   **Due:** Before Phase 1

3. **Visibility of individual metrics?**
   **Recommendation:** Public by default; privacy option to hide
   **Owner:** Product Lead
   **Due:** Before Phase 3

4. **Integration with volunteer hour systems?**
   **Recommendation:** Export feature; no direct integration for v1
   **Owner:** Product Lead
   **Due:** Future

---

## Related Documents

- **[Project 7 - Event Weights](./Project_07_Event_Weights.md)** - Event-level weight tracking
- **[Project 15 - Route Tracing](./Project_15_Route_Tracing.md)** - Route-attributed metrics
- **[Project 20 - Gamification](./Project_20_Gamification.md)** - Individual stats for rankings

---

**Last Updated:** January 24, 2026
**Owner:** Product Lead + Engineering
**Status:** Not Started
**Next Review:** When Project 7 complete
