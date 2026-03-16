# Project 58 — Community Event Seeding

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | Medium |
| **Risk** | Medium |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

TrashMob faces a classic cold-start problem: organizers won't list events on a platform with low traffic, and users won't visit a platform with few events. This creates a negative feedback loop that limits organic growth.

The solution is to allow TrashMob administrators to manually seed the platform with publicly listed cleanup events from other sources (city websites, park districts, community Facebook groups, Eventbrite, etc.). This populates the event map, drives organic traffic from people searching for local cleanups, and gives organizers a reason to claim and manage their events on TrashMob.

This approach is well-established — Facebook, Google Maps, Yelp, and Eventbrite all create stub listings from public data and invite businesses/organizers to claim them.

---

## Objectives

### Primary Goals
- Allow site admins to create events on behalf of external organizers from public sources
- Clearly distinguish admin-seeded events from organizer-created events
- Enable external organizers to "claim" seeded events and become the event lead
- Drive organic traffic to the platform through a populated event map

### Secondary Goals (Nice-to-Have)
- Automated outreach email to source contacts inviting them to claim
- Bulk import from structured data sources (CSV, community calendars)
- Analytics on claim conversion rate

---

## Scope

### Phase 1 - Admin Event Seeding
- ☐ Add `isSeeded` flag and `sourceUrl` field to Event model
- ☐ Add `sourceContactEmail` field to Event model (admin-only, not displayed publicly)
- ☐ Create site admin "Seed Event" form with standard event fields plus source URL and contact email
- ☐ Seeded events are created under a system "TrashMob Community" user account
- ☐ Display "Community Event" badge on seeded events in the event list and detail pages
- ☐ Show "Source: [link]" attribution on the event detail page
- ☐ Allow admins to edit/cancel seeded events from site admin

### Phase 2 - Claim Workflow
- ☐ "Claim This Event" button on seeded event detail pages (visible to logged-in users)
- ☐ Claim request creates a pending claim record with requester's user ID
- ☐ Admin review queue: approve/reject claims in site admin
- ☐ On approval: transfer event ownership to claiming user, remove `isSeeded` flag
- ☐ Notification email to the claiming user on approval/rejection

### Phase 3 - Outreach
- ☐ "Send Outreach" button on admin seed form (sends email to source contact)
- ☐ Outreach email template: introduces TrashMob, links to the seeded event, invites claim
- ☐ Track outreach sent/opened/claimed for conversion metrics

### Phase 4 - Bulk Seeding (Future)
- ☐ CSV import for batch-seeding events (name, date, location, source URL)
- ☐ Integration with public event APIs (community calendar feeds, iCal)
- ☐ Deduplication against existing events by date + location proximity

---

## Out-of-Scope

- ☐ Automated web scraping of other platforms (legal risk, ToS violations)
- ☐ Scraping behind login walls or authenticated APIs
- ☐ Importing events from private/paid platforms without permission
- ☐ Automatic event creation without admin review
- ☐ Allowing non-admin users to seed events

---

## Success Metrics

### Quantitative
- **Events seeded per month:** Target 20+ seeded events within first quarter
- **Claim conversion rate:** Target 10% of seeded events claimed by organizers
- **Traffic increase:** Measurable lift in organic search traffic to event pages
- **New user registrations:** Track users who signed up after viewing a seeded event

### Qualitative
- Organizers find value in claiming and managing their events on TrashMob
- The event map looks populated and active in target regions
- No legal complaints from source organizations

---

## Dependencies

### Blockers (Must be complete before this project starts)
- None — existing event infrastructure supports this

### Enablers for Other Projects
- **Project 40 (AI Community Sales Agent):** Seeded events provide talking points when reaching out to communities
- **Project 54 (Community Adoption Outreach):** Demonstrates platform value to prospective community partners

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Organizer complaints about unauthorized listings** | Medium | Medium | Clear attribution, source link, easy removal request process. All data from public sources only |
| **Stale seeded events (not cleaned up)** | Medium | Low | Auto-expire seeded events 7 days after event date if unclaimed |
| **Low claim conversion** | Medium | Low | Focus seeding on active community groups likely to engage. Quality over quantity |
| **Legal challenge on event data usage** | Low | Medium | Event facts (date, time, location, name) are not copyrightable. Always attribute source. Add removal request process |
| **Admin burden of manual seeding** | Medium | Low | Phase 4 adds CSV bulk import. Start with high-value regions only |

---

## Implementation Plan

### Data Model Changes

```csharp
// Add to existing Event model
public class Event
{
    // ... existing fields ...

    /// <summary>
    /// Whether this event was seeded by an admin from a public source.
    /// </summary>
    public bool IsSeeded { get; set; }

    /// <summary>
    /// URL of the public source where the event was originally listed.
    /// </summary>
    public string SourceUrl { get; set; } = string.Empty;

    /// <summary>
    /// Contact email from the source listing (admin-only, not displayed publicly).
    /// </summary>
    public string SourceContactEmail { get; set; } = string.Empty;
}

/// <summary>
/// Tracks claims on seeded events by external organizers.
/// </summary>
public class EventClaim : KeyedModel
{
    public Guid EventId { get; set; }
    public Guid ClaimingUserId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string Notes { get; set; } = string.Empty;
    public Guid? ReviewedByUserId { get; set; }
    public DateTimeOffset? ReviewedDate { get; set; }
}
```

### API Changes

```csharp
// Site Admin — Seed Event
[HttpPost("seed")]
[Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
public async Task<IActionResult> SeedEvent(SeedEventRequest request, CancellationToken cancellationToken)

// Public — Claim Event
[HttpPost("{eventId}/claim")]
[Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
public async Task<IActionResult> ClaimEvent(Guid eventId, CancellationToken cancellationToken)

// Site Admin — Review Claims
[HttpGet("claims/pending")]
[Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
public async Task<IActionResult> GetPendingClaims(CancellationToken cancellationToken)

[HttpPost("claims/{claimId}/approve")]
[Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
public async Task<IActionResult> ApproveClaim(Guid claimId, CancellationToken cancellationToken)
```

### Web UX Changes

**Event Detail Page:**
- "Community Event" badge for seeded events
- "Source: [link]" attribution line
- "Claim This Event" button (logged-in users, seeded events only)

**Site Admin:**
- "Seed Event" form (standard event fields + source URL + contact email)
- "Pending Claims" queue with approve/reject actions
- "Seeded Events" list with status indicators

### Mobile App Changes

- Display "Community Event" badge on seeded events
- "Claim This Event" button on event detail screen

---

## Implementation Phases

### Phase 1: Admin Seeding
- Add `IsSeeded`, `SourceUrl`, `SourceContactEmail` fields to Event model + migration
- Create system "TrashMob Community" user if not exists (used as CreatedByUserId for seeded events)
- Site admin seed event form
- "Community Event" badge + source attribution on event detail pages (web + mobile)

### Phase 2: Claim Workflow
- Add EventClaim model + migration
- Claim request endpoint and UI button
- Admin claim review queue
- Ownership transfer on approval
- Email notifications

### Phase 3: Outreach
- Outreach email template
- "Send Outreach" on admin seed form
- Outreach tracking metrics

### Phase 4: Bulk Seeding
- CSV import endpoint and admin UI
- Deduplication logic
- Public calendar feed integration (iCal)

**Note:** Phases are sequential but not time-bound.

---

## Legal Safeguards

To minimize legal risk:
1. **Only use publicly available event data** — never scrape behind login walls or ToS-protected sources
2. **Always attribute the source** with a visible link on the event page
3. **Provide a removal process** — if a source organization requests removal, comply promptly
4. **Label clearly** — "Community Event" badge, not implying TrashMob organized it
5. **No automated scraping** — all seeding is manual by admins (Phase 1-3). Bulk import (Phase 4) uses structured feeds, not scraping
6. **Add disclaimer** — "This event was sourced from a public listing and is not organized by TrashMob.eco"

---

## Open Questions

1. **Should seeded events allow registration, or just display information?**
   **Recommendation:** Display only with a link to the source for registration. Don't allow TrashMob registration for events we don't organize — avoids liability and confusion.
   **Owner:** Joe
   **Due:** Before Phase 1

2. **Should there be a "TrashMob Community" system user, or use the admin's own account?**
   **Recommendation:** System user — keeps seeded events separate from admin's personal events in dashboards and prevents confusion if admin leaves.
   **Owner:** Engineering
   **Due:** Phase 1

3. **What regions should initial seeding focus on?**
   **Recommendation:** Start with regions where TrashMob already has active communities. Seeded events in empty regions won't drive engagement if there are no nearby users.
   **Owner:** Joe
   **Due:** Before Phase 1

---

## Related Documents

- **[Project 40 — AI Community Sales Agent](./Project_40_AI_Community_Sales_Agent.md)** — AI-powered discovery and outreach
- **[Project 54 — Community Adoption Outreach](./Project_54_Community_Adoption_Outreach.md)** — Sponsor/adopter discovery
- **[GitHub Issue #1077](https://github.com/TrashMob-eco/TrashMob/issues/1077)** — Original feature request

---

**Last Updated:** 2026-03-15
**Owner:** Joe / Engineering
**Status:** Planning
**Next Review:** Before Phase 1 kickoff
