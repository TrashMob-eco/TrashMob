# Cross-Cutting: Minor Privacy Standards

| Attribute | Value |
|-----------|-------|
| **Status** | Draft |
| **Priority** | High |
| **Applies To** | All Projects |

---

## Purpose

This document defines privacy and display standards for minor users (ages 13-17) across all TrashMob features. All projects must reference and comply with these standards when implementing features that display user information.

---

## Legal Context

- **COPPA (Children's Online Privacy Protection Act):** Applies to users under 13; TrashMob requires users to be 13+ so COPPA doesn't directly apply, but privacy best practices should be followed
- **State laws:** Various state laws provide additional protections for minors
- **Platform responsibility:** TrashMob has a duty of care to protect minor users from potential harm

---

## Core Principles

1. **Privacy by Default:** Minor information should be hidden from public view by default
2. **Minimal Exposure:** Only display minor information when necessary for functionality
3. **Parental Visibility:** Parents/guardians should have full visibility into their minor's activity
4. **No Public Attribution:** Avoid publicly crediting minors by name in leaderboards, stats, etc.

---

## Display Standards by Context

### Public-Facing Features

| Feature | Minor Display Rules |
|---------|---------------------|
| **Event attendee lists** | Show only "Minor participant" (no name) unless guardian consents |
| **Leaderboards** | Exclude minors OR show as "Anonymous Volunteer" |
| **Top volunteers** | Exclude minors from public "top volunteer" displays |
| **Event photos** | Require guardian photo consent; blur/exclude if not consented |
| **Litter report creator** | Show "Anonymous" for minor-submitted reports |
| **Team member lists** | Show only to team members; "Minor" indicator visible only to leads |
| **Community metrics** | Aggregate counts only; no individual minor attribution |

### Authenticated Features (Visible to Logged-In Users)

| Feature | Minor Display Rules |
|---------|---------------------|
| **Event attendee details** | Event leads see "Minor (Guardian: [name])" |
| **Team membership** | Team leads see full details; other members see first name only |
| **User profiles** | Minor profiles are private; not searchable by other users |

### Admin/Guardian Features

| Feature | Display Rules |
|---------|---------------|
| **Guardian dashboard** | Full visibility of all minor activity |
| **Admin tools** | Full visibility for TrashMob staff |
| **Community admin** | See minor participation counts but not names |
| **Event lead tools** | See minor names for check-in purposes only |

---

## Implementation Requirements

### Database/API Level

```csharp
// Always check user age before including in public responses
public bool IsMinor(User user) => user.BirthDate.HasValue &&
    (DateTimeOffset.UtcNow.Year - user.BirthDate.Value.Year) < 18;

// Filter minors from public-facing queries
public async Task<IEnumerable<UserDto>> GetTopVolunteers()
{
    return await _context.Users
        .Where(u => !IsMinor(u))  // Exclude minors from public lists
        .OrderByDescending(u => u.TotalEvents)
        .Take(10)
        .ToListAsync();
}
```

### Frontend Level

```typescript
// Component to safely display user name
const UserDisplayName: React.FC<{ user: User; context: 'public' | 'team' | 'admin' }> =
  ({ user, context }) => {
    if (user.isMinor) {
      switch (context) {
        case 'public': return <span>Anonymous Volunteer</span>;
        case 'team': return <span>{user.firstName}</span>;
        case 'admin': return <span>{user.fullName} (Minor)</span>;
      }
    }
    return <span>{user.fullName}</span>;
  };
```

### Photo Handling

1. **Upload:** Check if photo contains minors; flag for guardian consent verification
2. **Display:** Only show photos with proper consent
3. **Export:** Exclude minor photos from public exports/reports

---

## Project-Specific Guidance

### Project 3 (Litter Reporting)
- Minor-submitted reports show "Anonymous" as creator
- Minor photos require guardian consent

### Project 7 (Event Weights)
- Attendee-level weights for minors visible only to event lead and guardian
- Exclude from public attendee weight displays

### Project 9 (Teams)
- Minors cannot be team leads (see Project 9 open question #12)
- Minor member names visible only to team leads and other members
- Exclude minors from public team stats

### Project 10 (Community Pages)
- Exclude minors from "top community volunteers" displays
- Minor participation included in aggregate counts only

### Project 20 (Gamification)
- Exclude minors from public leaderboards by default
- Option for guardian to opt-in to leaderboard display
- Private achievement display for minors (visible to minor + guardian only)

### Project 22 (Attendee Metrics)
- Minor metrics visible to: the minor, their guardian, event lead, admin
- Not included in public metric displays

---

## Consent Tracking

### Guardian Consent Scopes (Reference: Project 23)

| Consent Type | Effect on Display |
|--------------|-------------------|
| `public_display` | Minor name can appear in public lists |
| `photo_consent` | Minor's photos can be displayed |
| `leaderboard` | Minor can appear on leaderboards |
| `team_visible` | Minor's full name visible to team members |

### Default Consent (No Guardian Opt-In)

- Name: Hidden (show "Anonymous" or "Minor participant")
- Photos: Hidden from public; visible to guardian only
- Leaderboards: Excluded
- Team visibility: First name only

---

## Audit Requirements

1. **Log access:** Track when minor data is accessed and by whom
2. **Consent changes:** Log all guardian consent changes with timestamp
3. **Exports:** Log all exports containing minor data
4. **Retention:** Follow standard data retention; no extended retention for minors

---

## Testing Checklist

Before releasing any feature that displays user information:

- [ ] Verify minors are excluded from public-facing lists/displays
- [ ] Verify guardian consent is checked before showing minor photos
- [ ] Verify minor names are appropriately masked by context
- [ ] Verify admin/lead views show appropriate minor indicators
- [ ] Verify leaderboards exclude minors (or show anonymized)
- [ ] Test with a minor user account to verify all restrictions

---

## References

- **[Project 23 - Parental Consent](./Projects/Project_23_Parental_Consent.md)** - Consent management system
- **COPPA FTC Guidelines:** https://www.ftc.gov/business-guidance/resources/complying-coppa-frequently-asked-questions
- **Privacy Policy:** (link to TrashMob privacy policy)

---

**Last Updated:** January 29, 2026
**Owner:** Product Lead + Legal
**Status:** Draft - Pending Legal Review
**Next Review:** Before any project with minor display goes to development
