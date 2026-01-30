# Project 7 � Add Weights to Event Summaries

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

> **Note:** `EventSummary` already has `PickedWeight` (int) and `PickedWeightUnitId` (int) fields.
> Phase 1 may only require changing `PickedWeight` from `int` to `decimal` for precision.

**Phase 1: Update EventSummary (if precision change needed)**
```csharp
// In TrashMob.Models/EventSummary.cs - existing field may need type change
public class EventSummary : BaseModel
{
    // ... existing fields ...

    /// <summary>
    /// Gets or sets the total weight of trash picked up during the event.
    /// </summary>
    public decimal PickedWeight { get; set; }  // Changed from int to decimal

    /// <summary>
    /// Gets or sets the identifier of the weight unit used for the picked weight.
    /// </summary>
    public int PickedWeightUnitId { get; set; }

    // ... navigation properties ...
}
```

**Phase 2: New Entity - EventSummaryAttendee**
```csharp
// New file: TrashMob.Models/EventSummaryAttendee.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents an individual attendee's contribution metrics for an event.
    /// </summary>
    public class EventSummaryAttendee : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the event summary.
        /// </summary>
        public Guid EventSummaryId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the attendee.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the number of bags collected by this attendee.
        /// </summary>
        public int? NumberOfBags { get; set; }

        /// <summary>
        /// Gets or sets the weight collected by this attendee.
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the weight unit used.
        /// </summary>
        public int? WeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes this attendee participated.
        /// </summary>
        public int? DurationInMinutes { get; set; }

        /// <summary>
        /// Gets or sets any notes from the attendee about their contribution.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the event summary this contribution belongs to.
        /// </summary>
        public virtual EventSummary EventSummary { get; set; }

        /// <summary>
        /// Gets or sets the attendee user.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the weight unit used for the weight measurement.
        /// </summary>
        public virtual WeightUnit WeightUnit { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<EventSummaryAttendee>(entity =>
{
    entity.HasOne(e => e.EventSummary)
        .WithMany(es => es.AttendeeContributions)
        .HasForeignKey(e => e.EventSummaryId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.WeightUnit)
        .WithMany()
        .HasForeignKey(e => e.WeightUnitId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.EventSummaryId);
    entity.HasIndex(e => e.UserId);
});
```

**Add Navigation Property to EventSummary:**
```csharp
// Add to TrashMob.Models/EventSummary.cs
/// <summary>
/// Gets or sets the collection of attendee contributions for this event summary.
/// </summary>
public virtual ICollection<EventSummaryAttendee> AttendeeContributions { get; set; }
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

**User Preference Integration:**
All weight displays and entry forms must respect the signed-in user's `PrefersMetric` setting:

```tsx
// Hook to get user's preferred weight unit
const usePreferredWeightUnit = () => {
  const { data: user } = useGetCurrentUser();
  return user?.prefersMetric ? 'kg' : 'lbs';
};

// Usage in components
const preferredUnit = usePreferredWeightUnit();
const displayWeight = convertToUnit(rawWeight, rawUnit, preferredUnit);
```

**Event Summary Form (Phase 1):**
```tsx
const preferredUnit = usePreferredWeightUnit();

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
    <Select
      value={weightUnits}
      onChange={(e) => setWeightUnits(e.target.value)}
      defaultValue={preferredUnit}  // Default to user's preference
    >
      <option value="lbs">pounds (lbs)</option>
      <option value="kg">kilograms (kg)</option>
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

**User Preference Integration:**
The mobile app must also respect the user's `PrefersMetric` setting from their profile.

**Phase 1 Requirements:**
- **Event Summary Form:** Add weight entry fields with unit selector defaulting to user's preference
- **Event Summary Display:** Show weight in user's preferred unit
- **Event Details View:** Display weight converted to user's preferred unit
- **Statistics/Dashboard:** Show total weights in user's preferred unit
- **My Dashboard:** Display user's personal weight contributions in preferred unit

**Implementation Notes:**
```csharp
// In MAUI ViewModels - get user's preference
var prefersMetric = _userService.CurrentUser?.PrefersMetric ?? false;
var displayUnit = prefersMetric ? "kg" : "lbs";

// Convert for display
var displayWeight = prefersMetric
    ? weight * 0.453592m  // lbs to kg
    : weight;             // already in lbs
```

**Screens to Update:**
1. `EventSummaryPage` - Add weight entry with unit picker
2. `EventDetailsPage` - Display weight in user's preferred unit
3. `MyDashboardPage` - Show personal stats with preferred unit
4. `StatisticsPage` (if exists) - Display aggregate weights in preferred unit

**Phase 2 Requirements:**
- Attendee entry form on mobile with weight input
- Unit selector defaulting to user's preference

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

**User Preference Storage:**
The `User` table already has a `PrefersMetric` boolean field that stores the user's weight unit preference.

```csharp
// In TrashMob.Models/User.cs (existing)
public bool PrefersMetric { get; set; }  // true = metric (kg), false = imperial (lbs)
```

**Display and Entry Behavior:**
- **Signed-in users:** Use `PrefersMetric` from the user's profile for all weight displays and form defaults
- **Anonymous users:** Default to imperial (lbs)
- **Null/unset preference:** Default to imperial (lbs)
- Forms should pre-select the user's preferred unit
- Displays should convert and show weights in the user's preferred unit

**Implementation Requirements:**
1. **Frontend:** Fetch user preference on load; use for all weight-related components
2. **API responses:** Include both raw value + unit, let frontend handle conversion
3. **Statistics displays:** Convert to user's preferred unit (home page stats, dashboards, event details)
4. **Event summary forms:** Default unit selector to user's preference
5. **Attendee entry forms (Phase 2):** Default unit selector to user's preference

**Conversion Constants:**
```typescript
const LBS_TO_KG = 0.453592;
const KG_TO_LBS = 2.20462;
```

**Example Frontend Logic:**
```typescript
const getUserWeightUnit = (user: User | null): 'lbs' | 'kg' => {
  return user?.prefersMetric ? 'kg' : 'lbs';
};

const convertWeight = (weight: number, fromUnit: string, toUnit: string): number => {
  if (fromUnit === toUnit) return weight;
  return fromUnit === 'lbs' ? weight * LBS_TO_KG : weight * KG_TO_LBS;
};
```

### Attendee Entries (Phase 2)
- Sum of attendee weights should be ? event total � 1.2 (20% tolerance)
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

**Last Updated:** January 28, 2026
**Owner:** Product Lead + Engineering Team
**Status:** Ready for Review
**Next Review:** When volunteer picks up work

---

## Changelog

- **2026-01-28:** Added detailed requirements for user weight unit preference (`User.PrefersMetric`) integration across all displays and entry forms
