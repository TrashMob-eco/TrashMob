# Project 21 — Event Co-Leads

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in Progress |
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
- **Invite and manage co-leads** for events
- **Attendee list management UI** improvements
- **Notifications parity** for co-leads
- **Security updates** for all admin checks

### Secondary Goals
- Transfer primary lead role
- Co-lead activity history
- Co-lead communication tools
- Bulk co-lead assignment

---

## Scope

### Phase 1 - Data Model & Security
- ✅ Add `IsEventLead` flag to EventAttendees
- ✅ Update all admin checks to query EventAttendees
- ✅ Migrate existing event creators as leads

### Phase 2 - Co-Lead Management
- ✅ Invite attendee to be co-lead
- ✅ Accept/decline co-lead invitation
- ✅ Remove co-lead role
- ✅ Co-leads can manage event

### Phase 3 - UI Enhancements
- ✅ Edit Event: attendee list with lead toggles
- ✅ Create Event: auto-add creator as lead
- ✅ Event detail shows all leads

### Phase 4 - Notifications
- ✅ Co-leads receive same notifications as creator
- ✅ Attendee notifications include all leads
- ✅ Co-lead added/removed notifications

---

## Out-of-Scope

- ❌ Different permission levels for co-leads
- ❌ Co-lead voting/consensus features
- ❌ Automatic co-lead assignment
- ❌ Co-lead compensation tracking

---

## Success Metrics

### Quantitative
- **Events with co-leads:** ≥ 20% of events with 10+ attendees
- **Co-lead acceptance rate:** ≥ 80%
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

## Implementation Plan

### Data Model Changes

**Modification: EventAttendee (add IsEventLead)**
```csharp
// Add to existing TrashMob.Models/EventAttendee.cs
/// <summary>
/// Gets or sets whether this attendee is an event lead with management permissions.
/// </summary>
public bool IsEventLead { get; set; }
```

**New Entity: EventLeadInvitation**
```csharp
// New file: TrashMob.Models/EventLeadInvitation.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents an invitation for a user to become a co-lead of an event.
    /// </summary>
    public class EventLeadInvitation : KeyedModel
    {
        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the invited user's identifier.
        /// </summary>
        public Guid InvitedUserId { get; set; }

        /// <summary>
        /// Gets or sets the inviting user's identifier.
        /// </summary>
        public Guid InvitedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the invitation status (Pending, Accepted, Declined).
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets when the invitation was sent.
        /// </summary>
        public DateTimeOffset InvitedDate { get; set; }

        /// <summary>
        /// Gets or sets when the user responded to the invitation.
        /// </summary>
        public DateTimeOffset? RespondedDate { get; set; }

        // Navigation properties
        public virtual Event Event { get; set; }
        public virtual User InvitedUser { get; set; }
        public virtual User InvitedByUser { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<EventAttendee>(entity =>
{
    // Add to existing configuration
    entity.HasIndex(e => e.IsEventLead)
        .HasFilter("[IsEventLead] = 1");
});

modelBuilder.Entity<EventLeadInvitation>(entity =>
{
    entity.Property(e => e.Status).HasMaxLength(20);

    entity.HasOne(e => e.Event)
        .WithMany()
        .HasForeignKey(e => e.EventId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.InvitedUser)
        .WithMany()
        .HasForeignKey(e => e.InvitedUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.InvitedByUser)
        .WithMany()
        .HasForeignKey(e => e.InvitedByUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.InvitedUserId);
    entity.HasIndex(e => e.Status)
        .HasFilter("[Status] = 'Pending'");
    entity.HasIndex(e => new { e.EventId, e.InvitedUserId }).IsUnique();
});
```

**Data Migration:**
```csharp
// EF Core migration to set existing creators as leads
migrationBuilder.Sql(@"
    UPDATE ea
    SET IsEventLead = 1
    FROM EventAttendees ea
    INNER JOIN Events e ON ea.EventId = e.Id
    WHERE ea.UserId = e.CreatedByUserId;
");
```

### API Changes

```csharp
// Check if user is event lead (reusable method)
public async Task<bool> IsEventLeadAsync(Guid eventId, Guid userId)
{
    return await _context.EventAttendees
        .AnyAsync(ea => ea.EventId == eventId &&
                        ea.UserId == userId &&
                        ea.IsEventLead);
}

// Get event leads
[HttpGet("api/events/{eventId}/leads")]
public async Task<ActionResult<IEnumerable<EventLeadDto>>> GetEventLeads(Guid eventId)
{
    // Return all users with IsEventLead = true
}

// Invite co-lead
[Authorize]
[HttpPost("api/events/{eventId}/leads/invite")]
public async Task<ActionResult> InviteCoLead(
    Guid eventId, [FromBody] InviteCoLeadRequest request)
{
    // Validate caller is event lead
    // Create invitation
    // Send notification
}

// Respond to co-lead invitation
[Authorize]
[HttpPut("api/events/{eventId}/leads/invitation")]
public async Task<ActionResult> RespondToInvitation(
    Guid eventId, [FromBody] InvitationResponseRequest request)
{
    // Accept or decline
    // If accept, add IsEventLead to attendance
}

// Remove co-lead
[Authorize]
[HttpDelete("api/events/{eventId}/leads/{userId}")]
public async Task<ActionResult> RemoveCoLead(Guid eventId, Guid userId)
{
    // Validate caller is event lead
    // Cannot remove last lead
    // Set IsEventLead = false
}

// Promote attendee to co-lead (already attending)
[Authorize]
[HttpPut("api/events/{eventId}/attendees/{userId}/promote")]
public async Task<ActionResult> PromoteToCoLead(Guid eventId, Guid userId)
{
    // Validate caller is event lead
    // Set IsEventLead = true
    // Notify promoted user
}
```

### Security Audit Required

Update all endpoints that check for event ownership:

```csharp
// BEFORE (checks CreatedByUserId only)
if (existingEvent.CreatedByUserId != currentUserId)
    return Forbid();

// AFTER (checks EventAttendees.IsEventLead)
if (!await IsEventLeadAsync(eventId, currentUserId))
    return Forbid();
```

**Endpoints to update:**
- `PUT /api/events/{id}` - Edit event
- `DELETE /api/events/{id}` - Cancel event
- `POST /api/events/{id}/summary` - Add event summary
- `PUT /api/events/{id}/summary` - Edit event summary
- `POST /api/events/{id}/pickuplocations` - Add pickup location
- `PUT /api/events/{id}/partners` - Manage partner services
- `POST /api/events/{id}/litterreports` - Link litter reports

### Web UX Changes

**Edit Event Page:**
- Attendee list with columns: Name, Email, Status, Lead (toggle)
- "Invite Co-Lead" button
- Lead badge next to lead names
- Prevent removing last lead

**Event Detail Page:**
- Show all leads in event info
- "You are a co-lead" indicator

**Dashboard:**
- "Events I Lead" section shows all led events (not just created)

**Notifications:**
- Co-lead invitation notification
- Co-lead added to event notification
- Co-lead removed notification

### Mobile App Changes

- View event leads
- Accept/decline co-lead invitations
- Manage event if co-lead
- Co-lead notifications

---

## Implementation Phases

### Phase 1: Data Model & Migration
- Add IsEventLead column
- Migrate existing data
- Update base authorization method

### Phase 2: Security Updates
- Audit all event admin endpoints
- Update authorization checks
- Test thoroughly

### Phase 3: Co-Lead Management API
- Invitation endpoints
- Promote/demote endpoints
- Get leads endpoint

### Phase 4: UI Updates
- Edit event attendee list
- Invitation flow
- Dashboard updates
- Notifications

**Note:** Security is critical; extensive testing required before release.

---

## Open Questions

1. **Maximum number of co-leads per event?**
   **Recommendation:** No hard limit; soft warning at 5
   **Owner:** Product Lead
   **Due:** Before Phase 3

2. **Can co-leads remove other co-leads?**
   **Recommendation:** No, only primary creator can remove leads
   **Owner:** Product Lead
   **Due:** Before Phase 2

3. **What happens if primary creator deletes account?**
   **Recommendation:** Promote longest-tenured co-lead automatically
   **Owner:** Product Lead
   **Due:** Before Phase 4

4. **Should co-leads be able to transfer primary role?**
   **Recommendation:** Only primary creator can transfer; implement later
   **Owner:** Product Lead
   **Due:** Future phase

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Team leads pattern
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Lead-verified metrics

---

**Last Updated:** January 24, 2026
**Owner:** Engineering Team
**Status:** Planning in Progress
**Next Review:** When prioritized
