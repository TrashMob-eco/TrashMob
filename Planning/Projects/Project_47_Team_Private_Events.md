# Project 47 — Team-Visible Private Events

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Medium |
| **Size** | Medium |
| **Dependencies** | Project 9 (Teams) ✅ Complete |

---

## Business Rationale

Currently, events are either **public** (visible to all users) or **private** (visible only to the creator). Teams have no way to organize events exclusively for their members. This limits teams' ability to coordinate group cleanups without broadcasting to the general public. Allowing team-visible events would enable teams to plan internal activities, coordinate member-only cleanups, and strengthen team engagement — all while keeping the events discoverable to the right audience.

The FAQ already mentions: *"Eventually, you will be able to send invitations to specific people for a private event."* Team-visible events are a natural step toward this promise.

---

## Objectives

### Primary Goals
- **Team-scoped visibility**: Allow event leads to create events visible only to members of a specific team
- **Team member discovery**: Team members can see team events in their dashboard, on the map, and in search results
- **Non-member exclusion**: Users who are not members of the team cannot see or register for team-visible events

### Secondary Goals
- Team events appear in a dedicated section on the team detail page
- Team leads can manage team events alongside regular events
- Notifications for team events go only to team members

---

## Scope

### Phase 1 — Data Model & API
- Replace `IsEventPublic` boolean with an `EventVisibility` enum: `Public`, `TeamOnly`, `Private`
- Add nullable `TeamId` foreign key on Event (required when visibility is `TeamOnly`)
- Migrate existing data: `IsEventPublic = true` → `Public`, `IsEventPublic = false` → `Private`
- Update API query filters to include team-visible events for team members
- Update event creation/edit API to accept visibility + team association

### Phase 2 — Web UX
- Update event creation wizard to offer three visibility options: Public, Team Only, Private
- When "Team Only" is selected, show a team picker (user's teams)
- Update event search/map to include team events the user belongs to
- Show team events on the team detail page
- Update My Dashboard to show team events

### Phase 3 — Mobile App
- Update mobile event creation to support team visibility
- Update mobile event search/explore to include team events
- Update mobile team detail page to show team events

### Phase 4 — Notifications & Polish
- Send event notifications only to team members for team-visible events
- Update email templates to indicate team context
- Update FAQ to describe the three visibility levels

---

## Out-of-Scope

- ❌ Inviting specific individuals to private events (separate future feature)
- ❌ Multiple-team visibility (event visible to more than one team)
- ❌ Community-visible events (visible to all members of a community)
- ❌ Changing visibility after event creation (keep it simple for v1)

---

## Success Metrics

### Quantitative
- **Adoption:** ≥ 20% of teams create at least one team-visible event within 3 months of launch
- **Engagement:** Team members who see team events have higher event attendance rates than baseline

### Qualitative
- Positive feedback from team leads about coordination capabilities
- Reduced confusion about public vs. private event visibility

---

## Dependencies

### Blockers
- **Project 9 — Teams** ✅ Complete — Teams infrastructure must exist

### Enables
- Future invite-only events (extend the visibility model further)
- Team-based event analytics
- Project 12 — In-App Messaging (team event announcements)

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Data migration complexity** | Medium | Medium | Careful migration script; keep backward compatibility during transition |
| **Query performance** | Low | Medium | Index on `TeamId`; team membership check is already indexed |
| **Visibility confusion** | Medium | Low | Clear UX labels and tooltips; update FAQ |
| **Breaking existing behavior** | Low | High | Migration preserves all existing events exactly; boolean → enum is additive |

---

## Implementation Plan

### Data Model Changes

**Option A (Recommended): Add EventVisibility enum**

```csharp
// New file: TrashMob.Models/Enums/EventVisibilityEnum.cs
public enum EventVisibilityEnum
{
    Public = 1,
    TeamOnly = 2,
    Private = 3
}
```

**Update Event model:**
```csharp
// Replace: public bool IsEventPublic { get; set; }
// With:
public int EventVisibilityId { get; set; } = (int)EventVisibilityEnum.Public;
public Guid? TeamId { get; set; }

// Navigation property
public virtual Team Team { get; set; }
```

**Migration:**
```sql
-- Add new columns
ALTER TABLE Events ADD EventVisibilityId INT NOT NULL DEFAULT 1;
ALTER TABLE Events ADD TeamId UNIQUEIDENTIFIER NULL;

-- Migrate existing data
UPDATE Events SET EventVisibilityId = 1 WHERE IsEventPublic = 1;  -- Public
UPDATE Events SET EventVisibilityId = 3 WHERE IsEventPublic = 0;  -- Private

-- Add foreign key
ALTER TABLE Events ADD CONSTRAINT FK_Events_Teams FOREIGN KEY (TeamId) REFERENCES Teams(Id);

-- Drop old column (after verifying migration)
ALTER TABLE Events DROP COLUMN IsEventPublic;
```

### API Changes

**Event queries must be updated to include team-visible events:**
```csharp
// Updated GetActiveEventsAsync to include team events for the current user
public async Task<IEnumerable<Event>> GetActiveEventsAsync(
    Guid? userId, CancellationToken cancellationToken = default)
{
    var userTeamIds = userId.HasValue
        ? await GetUserTeamIdsAsync(userId.Value, cancellationToken)
        : new List<Guid>();

    return await Repo.Get(e =>
            (e.EventStatusId == (int)EventStatusEnum.Active
             || e.EventStatusId == (int)EventStatusEnum.Full)
            && (e.EventVisibilityId == (int)EventVisibilityEnum.Public
                || (e.EventVisibilityId == (int)EventVisibilityEnum.TeamOnly
                    && e.TeamId.HasValue
                    && userTeamIds.Contains(e.TeamId.Value)))
            && e.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-1 * StandardEventWindowInMinutes))
        .Include(e => e.CreatedByUser)
        .ToListAsync(cancellationToken);
}
```

### Web UX Changes

**Event creation wizard — visibility selector:**
- Radio buttons or dropdown: Public / Team Only / Private
- When "Team Only" selected, show team picker populated with user's teams
- Validation: TeamId required when visibility is TeamOnly
- Tooltip updated to explain all three options

**Event search/map:**
- Include team-visible events for authenticated users
- Team events show a team badge/indicator

**Team detail page:**
- New "Events" tab or section showing team-visible events

### Mobile App Changes

- Mirror web visibility selector in create event flow
- Update explore/search to include team events
- Show team events on team detail page

---

## Rollout Plan

1. **Phase 1 — Backend:** Deploy data model changes and API updates behind feature flag
2. **Phase 2 — Web:** Update event creation and discovery UX
3. **Phase 3 — Mobile:** Update mobile app to support team visibility
4. **Phase 4 — Polish:** Update FAQ, tooltips, notifications; remove feature flag

---

## Open Questions

1. **Should team-visible events allow backdating like private events?**
   Status: Open — Private events allow past dates for logging purposes; should team events also allow this?

2. **Can a team event be converted to public after creation?**
   Status: Open — v1 proposes no visibility changes after creation; revisit based on user feedback

3. **Should team admins be able to create team events, or any team member?**
   Status: Open — Likely any team member can create, but team admin can manage/delete

---

## Related Documents

- **[Project 9 — Teams](./Project_09_Teams.md)** — Teams infrastructure (prerequisite)
- **[Project 12 — In-App Messaging](./Project_12_In_App_Messaging.md)** — Future team announcements
- **[Project 38 — Mobile Feature Parity](./Project_38_Mobile_Feature_Parity.md)** — Mobile teams support

---

**Last Updated:** February 13, 2026
**Owner:** Product & Engineering
**Status:** Not Started
**Next Review:** When prioritized
