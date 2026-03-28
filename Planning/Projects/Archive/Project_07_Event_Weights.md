# Project 7 � Add Weights to Event Summaries

| Attribute | Value |
|-----------|-------|
| **Status** | Complete |
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
- Add total event weight to EventSummary
- Support both imperial (lbs) and metric (kgs) units
- Automatic unit conversion for display based on user preference

### Secondary Goals
- Weight displayed in event details and dashboards
- Weight included in email notifications
- Weight-based statistics and metrics

> **Note:** Attendee-level weight tracking is covered in [Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)

---

## Scope

### In Scope
- ✅ Add `TotalWeight` and `WeightUnits` fields to `EventSummary` table
- ✅ Update Event Summary form to include weight entry
- ✅ Display weight in event details and dashboards
- ✅ Include weight in email notifications
- ✅ Add weight to statistics/metrics displays
- ✅ Support user preference for metric vs. imperial units

> **Note:** Attendee-level weight entries, reconciliation, and roll-up are covered in [Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)

---

## Out-of-Scope

- ❌ Attendee-level weight entries (covered in [Project 22](./Project_22_Attendee_Metrics.md))
- ❌ Weight estimation based on bag count (requires research)
- ❌ Integration with scales/IoT devices
- ❌ Historical data backfill
- ❌ Weight-based leaderboards (covered in [Project 20](./Project_20_Gamification.md))

---

## Success Metrics

### Quantitative
- **Events with weight recorded:** ≥ 60% within 6 months
- **Form completion time:** No increase in event summary submission time

### Qualitative
- Positive feedback from communities on weight tracking
- Easier grant reporting with weight data
- Increased confidence in impact metrics

---

## Dependencies

### Blockers
None - independent feature

### Enables
- **Project 22 (Attendee Metrics):** Weight unit infrastructure
- **Project 20 (Gamification):** Weight-based leaderboards

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Unit conversion errors** | Low | Medium | Thorough testing; store with unit identifier |
| **Low adoption** | Medium | Low | Optional field; easy UI; community encouragement |

---

## Implementation Plan

### Data Model Changes

> **Note:** `EventSummary` already has `PickedWeight` (int) and `PickedWeightUnitId` (int) fields.
> May only require changing `PickedWeight` from `int` to `decimal` for precision.

**Update EventSummary (if precision change needed)**
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

### API Changes

**Event Summary DTO:**
```csharp
public class EventSummaryDto
{
    // Existing fields...
    public decimal? TotalWeight { get; set; }
    public string? WeightUnits { get; set; } // "lbs" or "kg"
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

**Event Summary Form:**
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

**Event Details Display:**
- Show weight in user's preferred unit
- Include unit label (e.g., "150 lbs" or "68 kg")
- Conversion happens client-side based on user preference

### Mobile App Changes

**User Preference Integration:**
The mobile app must also respect the user's `PrefersMetric` setting from their profile.

**Requirements:**
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

---

## Implementation Steps

1. **Database:** Change `PickedWeight` from `int` to `decimal` (migration)
2. **API:** Update DTOs to include weight with unit
3. **Web - Event Summary Form:** Add weight entry with unit selector
4. **Web - Event Details:** Display weight in user's preferred unit
5. **Web - Dashboards:** Show weight in statistics displays
6. **Mobile - Event Summary:** Add weight entry with unit picker
7. **Mobile - Displays:** Show weight in user's preferred unit
8. **Email Templates:** Include weight in event summary notifications

**Note:** This is a small, focused project. Attendee-level weight tracking is covered in [Project 22](./Project_22_Attendee_Metrics.md).

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

---

## Decisions

1. **Should we require weight or keep it optional?**
   **Decision:** Optional - event leads can enter if they have the data

2. **What's the canonical storage unit?**
   **Decision:** Store as entered with unit identifier; convert for display only

3. **Should we backfill historical events with estimated weights?**
   **Decision:** No - too much uncertainty; focus on new events only

4. **How do we handle aggregations across events with different units?**
   **Decision:** Store both original value and unit; aggregate by summing values in each unit separately; display in user's preferred unit

5. **What precision should decimal weights support?**
   **Decision:** One decimal place (0.1 lbs or 0.1 kg); use `decimal(10,1)` in database

6. **Should we provide weight estimation guidance for users without scales?**
   **Decision:** Yes, add help text: "A full 33-gallon bag typically weighs 15-25 lbs depending on contents"

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#1181](https://github.com/trashmob/TrashMob/issues/1181)** - Project 7: Allow attendees to add bags/weight collected (tracking issue)

---

## Related Documents

- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Attendee-level weight and metrics tracking
- **[Project 20 - Gamification](./Project_20_Gamification.md)** - Uses weight for leaderboards
- **Database:** `EventSummary` table

---

**Last Updated:** January 31, 2026
**Owner:** Product Lead + Engineering Team
**Status:** Ready for Review
**Next Review:** When volunteer picks up work

---

## Changelog

- **2026-01-31:** Removed Phase 2 (attendee-level weights) - now covered in Project 22
- **2026-01-28:** Added detailed requirements for user weight unit preference (`User.PrefersMetric`) integration across all displays and entry forms
