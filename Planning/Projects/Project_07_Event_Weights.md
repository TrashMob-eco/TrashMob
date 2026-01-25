# Project 7 — Add Weights to Event Summaries

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for Review |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Very Small |
| **Dependencies** | None |

---

## Business Rationale

Enable communities and attendees to track weight metrics accurately for events. Currently, TrashMob only tracks bag counts, but many communities and stakeholders want to know the actual weight of litter collected for more meaningful impact measurement.

---

## Objectives

### Primary Goals
- **Phase 1:** Add total event weight to EventSummary
- **Phase 2:** Enable attendee-level weight entries with validation and unit preferences

### Secondary Goals
- Support both imperial (lbs) and metric (kgs) units
- Automatic unit conversion for display
- Roll-up attendee weights to event totals

---

## Scope

### Phase 1 - Event-Level Weight
- ? Add `TotalWeight` and `WeightUnits` fields to `EventSummary` table
- ? Update Event Summary form to include weight entry
- ? Display weight in event details and dashboards
- ? Include weight in email notifications
- ? Add weight to statistics/metrics displays

### Phase 2 - Attendee-Level Weight
- ? Create `EventSummaryAttendee` table
- ? Allow attendees to enter their individual weights
- ? Event lead can review and reconcile entries
- ? Roll-up attendee weights to event total
- ? Validation to prevent double-counting

---

## Out-of-Scope

- ? Weight estimation based on bag count (requires research)
- ? Integration with scales/IoT devices
- ? Historical data backfill (Phase 3)
- ? Weight-based leaderboards (covered in Project 20)

---

## Success Metrics

### Quantitative
- **Events with weight recorded:** ? 60% within 6 months
- **Attendee weight entries:** ? 30% of attendees at events
- **Data accuracy:** < 5% discrepancies between total and sum of attendees

### Qualitative
- Positive feedback from communities on weight tracking
- Easier grant reporting with weight data
- Increased confidence in impact metrics

---

## Dependencies

### Blockers
None - independent feature

### Enables
- **Project 22 (Attendee Metrics):** Attendee-level tracking foundation
- **Project 20 (Gamification):** Weight-based leaderboards

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Unit conversion errors** | Low | Medium | Thorough testing; store in canonical unit internally |
| **Attendees over-reporting** | Medium | Low | Event lead review; reasonable range validation |
| **Double-counting** | Low | High | Clear UI indicators; validation logic |

---

## Implementation Plan

### Data Model Changes

```sql
-- Phase 1: Event-level weight
ALTER TABLE EventSummary
ADD TotalWeight DECIMAL(10,2) NULL,
    WeightUnits VARCHAR(10) NULL; -- 'lbs' or 'kgs'

-- Phase 2: Attendee-level weight
CREATE TABLE EventSummaryAttendee (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventSummaryId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    NumberOfBags INT NULL,
    Weight DECIMAL(10,2) NULL,
    WeightUnits VARCHAR(10) NULL,
    DurationInMinutes INT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    LastUpdatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (EventSummaryId) REFERENCES EventSummary(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE INDEX IX_EventSummaryAttendee_EventSummaryId 
    ON EventSummaryAttendee(EventSummaryId);
CREATE INDEX IX_EventSummaryAttendee_UserId 
    ON EventSummaryAttendee(UserId);
```

### API Changes

**Phase 1 - Event Summary:**
```csharp
public class EventSummaryDto
{
    // Existing fields...
    public decimal? TotalWeight { get; set; }
    public string? WeightUnits { get; set; } // "lbs" or "kgs"
}
```

**Phase 2 - Attendee Entries:**
```csharp
[HttpGet("api/eventsummaries/{eventSummaryId}/attendees")]
public async Task<ActionResult<IEnumerable<EventSummaryAttendeeDto>>> GetAttendeeEntries(
    Guid eventSummaryId)
{
    // Return attendee-level data for event lead review
}

[HttpPost("api/eventsummaries/{eventSummaryId}/attendees")]
public async Task<ActionResult<EventSummaryAttendeeDto>> CreateAttendeeEntry(
    Guid eventSummaryId,
    [FromBody] CreateAttendeeEntryRequest request)
{
    // Validate user is registered attendee
    // Save entry
    // Recalculate event totals
}
```

### Web UX Changes

**Event Summary Form (Phase 1):**
```tsx
<FormGroup>
  <Label>Total Weight Collected</Label>
  <InputGroup>
    <Input
      type="number"
      step="0.1"
      value={totalWeight}
      onChange={(e) => setTotalWeight(e.target.value)}
      placeholder="Enter weight"
    />
    <Select value={weightUnits} onChange={(e) => setWeightUnits(e.target.value)}>
      <option value="lbs">pounds (lbs)</option>
      <option value="kgs">kilograms (kgs)</option>
    </Select>
  </InputGroup>
  <FormText>Optional: Enter the total weight of litter collected</FormText>
</FormGroup>
```

**Attendee Entry Form (Phase 2):**
```tsx
<AttendeeMetricsForm>
  <h4>My Contribution</h4>
  <Input label="Bags I Collected" type="number" />
  <Input label="Weight I Collected" type="number" step="0.1" />
  <Select label="Units">
    <option value="lbs">pounds</option>
    <option value="kgs">kilograms</option>
  </Select>
  <Button>Submit My Entry</Button>
</AttendeeMetricsForm>
```

**Event Lead Reconciliation View (Phase 2):**
- Table showing all attendee entries
- Calculated total vs. event lead's total
- Flag discrepancies
- Allow adjustments

### Mobile App Changes

- Add weight fields to event summary form
- Display weight in event details
- Phase 2: Attendee entry form on mobile

---

## Implementation Phases

### Phase 1: Event-Level Weight
- Add database columns
- Update DTOs and API
- Update web forms
- Update mobile forms
- Update display components
- Update email templates

### Phase 2: Attendee-Level Weight
- Create new table
- Build API endpoints
- Create attendee entry form
- Build event lead reconciliation view
- Implement roll-up logic
- Add validation rules

**Note:** Phase 1 can be completed quickly (1-2 developers). Phase 2 can be picked up independently.

---

## Validation Rules

### Weight Entry
- Must be positive number
- Reasonable range: 0.1 - 10,000 lbs (or kg equivalent)
- Optional field (not required)

### Unit Preference
- Default to user's preference (from profile)
- Fall back to imperial (lbs) in US, metric elsewhere
- Always store conversion factor for accuracy

### Attendee Entries (Phase 2)
- Sum of attendee weights should be ? event total × 1.2 (20% tolerance)
- Warning (not error) if discrepancy
- Event lead can override

---

## Open Questions

1. **Should we require weight or keep it optional?**  
   **Recommendation:** Keep optional for Phase 1; encourage in Phase 2  
   **Owner:** Product Lead  
   **Due:** Before Phase 1

2. **What's the canonical storage unit?**  
   **Recommendation:** Store as entered; convert for display only  
   **Owner:** Engineering Team  
   **Due:** Before implementation

3. **How do we handle discrepancies between attendee sum and lead's total?**  
   **Recommendation:** Show warning; use lead's total as authoritative; log for review  
   **Owner:** Product Lead  
   **Due:** Before Phase 2

4. **Should we backfill historical events with estimated weights?**  
   **Recommendation:** No; too much uncertainty; focus on new events  
   **Owner:** Product Lead  
   **Due:** N/A (out of scope)

---

## Related Documents

- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Builds on attendee-level tracking
- **[Project 20 - Gamification](./Project_20_Gamification.md)** - Uses weight for leaderboards
- **Database:** `EventSummary` and `EventSummaryAttendee` tables

---

**Last Updated:** January 24, 2026  
**Owner:** Product Lead + Engineering Team  
**Status:** Ready for Review  
**Next Review:** When volunteer picks up work
