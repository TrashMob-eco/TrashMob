# Project 57 — Participation Report Email

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 22 (Attendee Metrics — ✅ Complete) |

---

## Business Rationale

Volunteers frequently need proof of participation for school volunteer-hour requirements, court-ordered community service, employer volunteer programs, and grant-funded community initiatives. Currently, there's no way for a volunteer to generate an official record of their verified contribution to a cleanup event.

TrashMob already has all the data needed: per-attendee metrics (bags, weight, duration) submitted by volunteers and approved/adjusted by event leads. This project adds a branded "Participation Report" email that serves as an official record, sent on demand from both the web and mobile apps.

This directly supports volunteer retention (people come back when their effort is recognized and documented) and organizational credibility with schools, courts, and employers.

---

## Objectives

### Primary Goals
- Allow volunteers to request an official participation report email for any event where their metrics were approved
- Allow event leads to send participation reports to all verified attendees at once
- Generate a professional, branded email with TrashMob logo, event details, and individual metrics
- Include a PDF attachment for easy printing/forwarding

### Secondary Goals (Nice-to-Have)
- Cumulative report across multiple events (date range)
- QR code or verification URL on the report for third-party validation
- Event lead digital signature on the report

---

## Scope

### Phase 1 - Backend API + Email Template
- ☐ Create `ParticipationReport` email HTML template (branded, professional)
- ☐ Create PDF generation service for the participation report
- ☐ Add `POST /api/v2/events/{eventId}/participation-report` endpoint (sends report to requesting user)
- ☐ Add `POST /api/v2/events/{eventId}/participation-report/send-all` endpoint (event lead sends to all verified attendees)
- ☐ Validate: only send for attendees with Approved or Adjusted metrics
- ☐ Include PDF attachment in the email

### Phase 2 - Web UI
- ☐ Add "Request Participation Report" button on event summary page (visible to attendees with approved metrics)
- ☐ Add "Send Reports to All Attendees" button for event leads on event summary page
- ☐ Toast confirmation when report is sent
- ☐ Disable button if metrics are still Pending or Rejected

### Phase 3 - Mobile UI
- ☐ Add "Request Participation Report" button on View Event screen (after event completion)
- ☐ Add "Send Reports to All" option for event leads
- ☐ Toast/notification confirmation when report is sent

### Phase 4 - Verification (Future)
- ☐ Generate unique verification URL per report (e.g., `trashmob.eco/verify/{reportId}`)
- ☐ Public verification page showing event name, date, volunteer name, and hours
- ☐ QR code on the PDF linking to the verification URL

---

## Out-of-Scope

- ☐ Cumulative multi-event reports (defer to future — can be added as a dashboard/profile feature)
- ☐ Custom report branding per community (use standard TrashMob branding)
- ☐ Physical certificate mailing
- ☐ Integration with external volunteer-hour tracking systems (e.g., VolunteerHub)

---

## Success Metrics

### Quantitative
- **Report requests:** Track via App Insights custom event `ParticipationReport_Requested`
- **Email delivery rate:** Monitor via SendGrid delivery stats
- **Adoption:** >10% of attendees at events with approved metrics request a report within 3 months

### Qualitative
- Volunteers report the email is accepted by schools/courts/employers as proof of participation
- Event leads find the "send all" feature saves time vs. individual correspondence

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Project 22 (Attendee Metrics):** ✅ Complete — per-attendee metrics with approval workflow in place

### Enablers for Other Projects
- **Project 20 (Gamification):** Participation reports reinforce the achievement/impact narrative
- **Project 52 (Volunteer Rewards):** Reports provide the verified-participation data that reward eligibility could be based on

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **PDF generation library complexity** | Medium | Medium | Use a lightweight .NET PDF library (QuestPDF or similar); keep layout simple |
| **Email deliverability** | Low | Medium | Use existing SendGrid infrastructure with established sender reputation |
| **Spam/abuse (mass report requests)** | Low | Low | Rate limit: one report request per event per user per day |
| **Legal validity concerns** | Medium | Low | Disclaimer: "This report is a record of participation, not a legal document." Label as "Volunteer Participation Report" not "Certificate" |

---

## Implementation Plan

### Email Template Design

The participation report email should look official and professional:

**Email body:**
- TrashMob logo header
- "Volunteer Participation Report" title
- Event details: name, date, time, location, event lead name
- Volunteer details: name, metrics (bags, weight, duration)
- Event totals: total bags, total weight, total attendees
- "Verified by [Event Lead Name] on [Date]" footer
- TrashMob mission statement and website link

**PDF attachment:**
- Same content as email but formatted for print (letter size)
- TrashMob logo watermark
- Unique report reference number
- "Verified" badge/stamp with event lead name and date

### Data Model Changes

No new database tables for Phase 1-3 (reports are generated on-demand from existing data).

Phase 4 adds a verification table:

```csharp
/// <summary>
/// Stores issued participation reports for third-party verification.
/// </summary>
public class ParticipationReportRecord : KeyedModel
{
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public Guid IssuedByUserId { get; set; }

    // Snapshot of metrics at time of issuance
    public int BagsCollected { get; set; }
    public decimal WeightPounds { get; set; }
    public int DurationMinutes { get; set; }
}
```

### API Changes

```csharp
[ApiController]
[Route("api/v2/events/{eventId}/participation-report")]
[Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
public class ParticipationReportController(
    IParticipationReportService reportService,
    ILogger<ParticipationReportController> logger) : SecureController
{
    /// <summary>
    /// Sends a participation report email to the requesting user.
    /// </summary>
    [HttpPost]
    [RequiredScope(Constants.TrashMobWriteScope)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestReport(
        Guid eventId, CancellationToken cancellationToken)
    {
        var result = await reportService.SendReportAsync(eventId, UserId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        TrackEvent("ParticipationReport_Requested");
        return Ok();
    }

    /// <summary>
    /// Sends participation report emails to all verified attendees. Event lead only.
    /// </summary>
    [HttpPost("send-all")]
    [RequiredScope(Constants.TrashMobWriteScope)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendAllReports(
        Guid eventId, CancellationToken cancellationToken)
    {
        var result = await reportService.SendAllReportsAsync(eventId, UserId, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        TrackEvent("ParticipationReport_SentAll");
        return Ok(new { sentCount = result.Data });
    }
}
```

### Service Layer

```csharp
public interface IParticipationReportService
{
    Task<ServiceResult<bool>> SendReportAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
    Task<ServiceResult<int>> SendAllReportsAsync(Guid eventId, Guid requestingUserId, CancellationToken cancellationToken);
}
```

The service:
1. Validates the user has Approved/Adjusted metrics for the event
2. Fetches event details, user details, attendee metrics, event summary
3. Generates PDF using a PDF library
4. Sends email via SendGrid with PDF attachment

### Web UX Changes

**Event summary page** (`/eventsummary/{eventId}`):
- "Request Participation Report" button — visible to attendees with approved metrics
- "Send Reports to All Attendees" button — visible to event leads
- Both buttons show a loading spinner while sending, then a success toast

**My Dashboard** (future consideration):
- "My Reports" section showing events where reports are available

### Mobile App Changes

**View Event screen** (after event completion):
- "Request Participation Report" button in the post-event actions area
- "Send Reports to All" in event lead menu
- Success notification via `NotificationService`

---

## Implementation Phases

### Phase 1: Backend + Email Template
- Choose and integrate PDF generation library (QuestPDF recommended — MIT license, .NET native)
- Create HTML email template in `EmailCopy/ParticipationReport.html`
- Create PDF layout service
- Implement `ParticipationReportService` with `SendReportAsync` and `SendAllReportsAsync`
- Add `ParticipationReportController` with both endpoints
- Register in DI (`ServiceBuilder.cs`)
- Add rate limiting (1 request per event per user per day)
- Unit tests for service logic

### Phase 2: Web UI
- Add buttons to event summary page
- Wire up API calls with React Query mutations
- Toast notifications for success/error
- Disable state when metrics not approved

### Phase 3: Mobile UI
- Add button to ViewEventViewModel (post-event state)
- Add "Send All" to event lead actions
- Wire up REST service call
- Notification on success

### Phase 4: Verification (Future)
- Add `ParticipationReportRecord` table and migration
- Generate unique verification codes on report issuance
- Create public verification page (`/verify/{code}`)
- Add QR code to PDF

**Note:** Phases are sequential but not time-bound. Claude can handle implementation for each phase.

---

## Open Questions

1. **Which PDF generation library?**
   **Recommendation:** QuestPDF — MIT license, .NET native, fluent API, no external dependencies. Generates professional layouts without HTML-to-PDF conversion.
   **Owner:** Engineering
   **Due:** Before Phase 1

2. **Should the report include the event lead's name as "verifier"?**
   **Recommendation:** Yes — the `ReviewedByUserId` on the approved metrics identifies who verified. Include their name as "Verified by [Name]" on the report.
   **Owner:** Joe
   **Due:** Before Phase 1

3. **Should there be a separate SendGrid template for this, or use the GenericEmail template?**
   **Recommendation:** Use GenericEmail template with custom `emailCopy` HTML. The PDF attachment is the official document; the email body is just context.
   **Owner:** Engineering
   **Due:** Phase 1

---

## Related Documents

- **[Project 22 — Attendee Metrics](./Project_22_Attendee_Metrics.md)** — Per-attendee metrics and approval workflow
- **[Project 20 — Gamification](./Project_20_Gamification.md)** — Achievement/impact tracking
- **[Project 52 — Volunteer Rewards](./Project_52_Volunteer_Rewards.md)** — Future reward eligibility based on verified participation
- **[GitHub Issue #675](https://github.com/TrashMob-eco/TrashMob/issues/675)** — Original feature request

---

**Last Updated:** 2026-03-15
**Owner:** Joe / Engineering
**Status:** Planning
**Next Review:** Before Phase 1 kickoff
