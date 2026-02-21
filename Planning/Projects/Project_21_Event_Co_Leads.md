# Project 21 — Event Co-Leads

| Attribute | Value |
|-----------|-------|
| **Status** | Complete |
| **Priority** | Medium |
| **Risk** | High |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Support multiple admins per event to distribute workload and provide backup if primary lead is unavailable. Currently, only the event creator can manage the event, which limits flexibility for larger cleanups.

---

## Objectives

### Primary Goals
- **Promote and manage co-leads** for events
- **Attendee list management UI** with lead status display
- **Notifications** when co-lead status changes
- **Security updates** for all admin checks

### Secondary Goals
- Transfer primary lead role (deferred)
- Co-lead activity history (deferred)
- Co-lead communication tools (deferred)
- Bulk co-lead assignment (deferred)

---

## Scope

### Phase 1 - Data Model & Security
- ✅ Add `IsEventLead` flag to EventAttendees
- ✅ Update all admin checks to query EventAttendees
- ✅ Migrate existing event creators as leads
- ✅ Security authorization handlers

### Phase 2 - Co-Lead Management
- ✅ Promote attendee to co-lead
- ✅ Demote co-lead back to attendee
- ✅ Enforce maximum 5 co-leads per event
- ✅ Prevent demoting last remaining lead

### Phase 3 - UI Enhancements
- ✅ Event details: attendee list with Lead badges
- ✅ Event details: promote/demote dropdown for leads
- ✅ Create Event: auto-add creator as lead

### Phase 4 - Notifications
- ✅ Co-lead added notification email
- ✅ Co-lead removed notification email

---

## Out-of-Scope

- ❌ Different permission levels for co-leads
- ❌ Co-lead voting/consensus features
- ❌ Automatic co-lead assignment
- ❌ Co-lead compensation tracking
- ❌ Invitation workflow (rejected - use direct promote/demote instead)

---

## Success Metrics

### Quantitative
- **Events with co-leads:** ≥ 20% of events with 10+ attendees
- **Event management issues:** Reduction in "can't edit event" support requests

### Qualitative
- Event leads find co-lead feature useful
- Larger events better managed
- Smooth handoff when leads unavailable

---

## Dependencies

### Blockers
None - independent feature

### Enables
- Better event management
- Succession planning for recurring events
- Team-based event organization

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Permission confusion** | Medium | Medium | Clear UI; consistent permissions; documentation |
| **Security gaps** | Medium | High | Thorough audit of all admin endpoints; testing |
| **Co-lead disputes** | Low | Low | Primary lead has final authority; ToS clarity |
| **Migration issues** | Low | High | Careful migration; testing; rollback plan |

---

## Implementation (Completed)

### Data Model Changes

**Modification: EventAttendee (IsEventLead field)**
```csharp
// In TrashMob.Models/EventAttendee.cs
/// <summary>
/// Gets or sets whether this attendee is an event lead with management permissions.
/// Maximum 5 co-leads per event enforced at API level.
/// </summary>
public bool IsEventLead { get; set; }
```

**Migration: AddEventCoLeads**
```sql
-- Set existing event creators as leads
UPDATE ea
SET IsEventLead = 1
FROM EventAttendees ea
INNER JOIN Events e ON ea.EventId = e.Id
WHERE ea.UserId = e.CreatedByUserId;
```

### API Endpoints

```csharp
// Get event leads
[HttpGet("api/eventattendees/{eventId}/leads")]
// Returns all users with IsEventLead = true

// Promote to lead
[HttpPut("api/eventattendees/{eventId}/{userId}/promote")]
// Sets IsEventLead = true, sends notification

// Demote from lead
[HttpPut("api/eventattendees/{eventId}/{userId}/demote")]
// Sets IsEventLead = false, sends notification
```

### Security Updates

Authorization handlers check `IsEventLead` via `EventAttendeeManager.IsEventLeadAsync()`:
- `UserIsEventLeadAuthHandler`
- `UserIsEventLeadOrIsAdminAuthHandler`

**Endpoints using event lead authorization:**
- `PUT /api/events/{id}` - Edit event
- `DELETE /api/events/{id}` - Cancel event
- `POST /api/events/{id}/summary` - Add event summary
- `PUT /api/events/{id}/summary` - Edit event summary
- `POST /api/pickuplocations` - Add pickup location
- Event partner and litter report management

### Web UI

**Event Details Page (Attendee Table):**
- Shows "Lead" badge for all event leads
- Actions dropdown for current leads:
  - Promote to Lead (if < 5 leads)
  - Remove Lead Status (if > 1 lead)
- Real-time updates via React Query

### Email Notifications

**Co-Lead Added (EventCoLeadAdded.html):**
- Sent when user is promoted to co-lead
- Includes event name, date, and list of permissions

**Co-Lead Removed (EventCoLeadRemoved.html):**
- Sent when user is demoted from co-lead
- Confirms user is still an event attendee

---

## Account Deletion Handling

When the primary event creator deletes their account:

1. **For each event they created:**
   - Query co-leads ordered by `CreatedDate` (earliest first)
   - Promote the longest-tenured co-lead to primary creator
   - Update `Event.CreatedByUserId` to the new primary creator
   - Notify the new primary creator via email

2. **If no co-leads exist:**
   - Event remains with original creator reference (soft-deleted user)
   - Event continues to function but cannot be edited
   - Admin can assign a new lead if needed

---

## Resolved Questions

1. **Maximum number of co-leads per event?**
   **Decision:** Hard limit of 5 co-leads per event (enforced by API)

2. **Can co-leads remove other co-leads?**
   **Decision:** Yes, any event lead can demote other leads (except the last one)

3. **What happens if primary creator deletes account?**
   **Decision:** Automatically promote the longest-tenured co-lead to primary creator

4. **Should we use an invitation workflow?**
   **Decision:** No - rejected in favor of direct promote/demote for simplicity

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#968](https://github.com/trashmob/TrashMob/issues/968)** - Project 21: Event Co-Leads (tracking issue)

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Team leads pattern
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Lead-verified metrics

---

## Post-Completion Bug Fixes

- **PR #2758** (Feb 15, 2026): Fixed bug where event creator was not automatically set as event lead (`IsEventLead = true`) when creating a new event. The `EventAttendee` record was created but `IsEventLead` defaulted to `false`, causing `GetEventLeadsAsync()` to miss the creator.

---

**Last Updated:** February 15, 2026
**Owner:** Engineering Team
**Status:** Complete
**Completed:** February 1, 2026

---

## Changelog

- **2026-02-01:** Marked complete; added UI for promote/demote in attendee table; added email notifications
- **2026-01-31:** Rejected invitation workflow in favor of direct promote/demote
- **2026-01-31:** Initial implementation of IsEventLead field and security handlers
