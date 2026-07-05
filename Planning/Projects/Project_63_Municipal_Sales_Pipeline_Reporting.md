# Project 63 — Municipal Sales Pipeline Reporting

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | [Project 60 — Prospect Multi-Contact Tracking](./Project_60_Prospect_Contact_Tracking.md), [Project 64 — Roles & RBAC Refactor](./Project_64_Roles_and_RBAC_Refactor.md) |

---

## Business Rationale

TrashMob is hiring a **1099 contract salesperson** to sell the existing SaaS platform to US municipalities. This is the first paid revenue channel for the platform and the first operational test of whether cities will actually pay for a volunteer-cleanup coordination + FOIA-friendly reporting product. Board President Cynthia Mitchell has proposed a set of pipeline, weekly, and monthly reporting spreadsheets to run the sales motion. This project translates those spreadsheets into first-class product features inside the existing Site Admin CRM.

The salesperson will use the CRM daily. Cynthia and the Board need weekly and monthly rollups without asking Joe or the salesperson to build them in Excel. And every measurable data point captured here — pipeline conversion rates, common objections, pricing feedback, which departments are most responsive — feeds directly into the strategic question the organization is trying to answer: **is there real, paid demand for TrashMob-as-SaaS to cities?**

Without this reporting infrastructure, we'd land the salesperson with a spreadsheet-based operating rhythm, which lasts about 3 months before the data becomes uneditable in practice.

---

## Snapshot: What Already Exists

Substantial prospect-tracking infrastructure landed in **[Project 60](./Project_60_Prospect_Contact_Tracking.md)** and adjacent work. Reuse first, build second.

### Entities (in `TrashMob.Models/`)

| Entity | Purpose | Fields relevant here |
|---|---|---|
| **CommunityProspect** | The prospect record | `Name`, `Type` (free-form string), `City`, `Region`, `Country`, `Population`, `Website`, `PipelineStage` (int), `FitScore`, `Notes`, `LastContactedDate`, `NextFollowUpDate`, `ConvertedPartnerId` |
| **ProspectContact** | Multi-contact per prospect (from Project 60) | Name, Title, Email, Phone, Role, ContactStatus, IsPrimary, ReferredByContactId |
| **ProspectActivity** | Touchpoint log | `ProspectId`, `ProspectContactId`, `ActivityType`, `Subject`, `Details`, `SentimentScore`, standard audit fields |
| **ProspectOutreachEmail** | Structured outreach records | (per-contact via Project 60 Phase 2) |

### UI (in `TrashMob/client-app/src/pages/siteadmin/prospects/`)

- `page.tsx` — list view with `columns.tsx` DataTable
- `$prospectId.tsx` / `$prospectId.edit.tsx` — detail + edit
- `create.tsx` — new prospect form
- `discovery.tsx` — AI-assisted discovery (from Project 40, archived)
- `import.tsx` — bulk import
- **`analytics.tsx`** — existing analytics starting point; will be extended for the new reports

### Controllers

- `CommunityProspectsV2Controller`
- `ProspectContactsV2Controller`

### Gap vs. Cynthia's spreadsheet

| Spreadsheet column | Current state | Gap |
|---|---|---|
| Prospect ID (`TM-MUN-001` style) | GUID `Id` only | New: human-readable auto-numbered display id |
| Municipality | `Name` | none |
| State | `Region` | none |
| Municipality Type | `Type` (free-form string) | Constrain to enum (City / Town / County / Regional Agency / Special District / Other) |
| Department | — | **New field** — critical for "best responding department" analysis |
| Contact Name / Title / Email / Phone | on `ProspectContact` | none |
| Website | `Website` | none |
| **Priority** (High/Med/Low) | — | **New field** |
| Stage | `PipelineStage` (int) | Expand enum to the 10 stages Cynthia listed |
| First Contact Date | — | Derived from earliest `ProspectActivity` — computed, not stored |
| Last Touch Date | `LastContactedDate` | none |
| Next Follow-Up Date | `NextFollowUpDate` | none |
| Meeting Requested | — | Modelled as a specific `ProspectActivity.ActivityType = "Meeting Requested"` |
| Meeting Date | — | Modelled as `"Meeting Scheduled"` activity with the meeting time in `Details` |
| Pricing Feedback | — | **New field** on prospect (short-text) + also captured per-activity |
| Key Objection / Question | — | **New field** on prospect (short-text) |
| Notes | `Notes` | none |

**Verdict:** ~70% of the pipeline sheet is already modelled. Six fields to add, one enum to constrain, one to expand.

---

## Objectives

### Primary Goals

- **Give the salesperson a CRM that mirrors the spreadsheet on day 1** so they never need to open Cynthia's Excel file except for cross-checking.
- **Auto-generate the Weekly Report** as a screen in the Site Admin — no manual data entry, no double-tracking.
- **Auto-generate the Monthly Goals report** with actuals rolled up from the CRM against configurable targets.
- **Surface market-intelligence signal** (best-responding departments, common objections, pricing feedback, messaging that worked) as first-class outputs, not free-text notes trapped in the pipeline entity.
- **Give Cynthia and the Board weekly and monthly emails** with the reports rendered inline (no logging in to see them).

### Secondary Goals

- Export any report to CSV or PDF for external sharing (grant applications, Board decks).
- Support one salesperson today; keep the data model general so a second seat can be added without rework.
- Feed the same underlying queries into a "sales narrative" summary that ties measurable outcomes to Project 61's Option B milestone gate (15 paying cities, $300K ARR, 3 references by mid-2027).

---

## Scope

### Phase 1 — Model + admin form extensions (backend + web) *[Ships first — unblocks the salesperson]*

- ☐ Add `Department` (nullable string) to `CommunityProspect`
- ☐ Add `Priority` (enum: High / Medium / Low; nullable) to `CommunityProspect`
- ☐ Add `PricingFeedback` (nvarchar(500), nullable) to `CommunityProspect`
- ☐ Add `KeyObjection` (nvarchar(500), nullable) to `CommunityProspect`
- ☐ Introduce `MunicipalityType` enum (City / Town / County / Regional Agency / Special District / Other) and migrate free-form `Type` values to constrained values (data migration + fallback bucket for anything unmapped)
- ☐ Expand `PipelineStage` enum to the 10 stages: Identified, Researched, Contacted, Follow-up needed, Responded, Discovery in progress, Meeting requested, Meeting scheduled, Not a fit, Future follow-up
- ☐ Introduce `ProspectActivityType` enum with values that support the weekly-report categorisation: Outreach / Follow-up / Response received / Meeting requested / Meeting scheduled / Meeting held / Note (add lookup rows in `LookupData` seeder)
- ☐ Migration (`AddMunicipalPipelineFields` or similar) with backfill of existing rows to the new enum values
- ☐ Update `CommunityProspectDto` + mappings + controller signatures (V2)
- ☐ Update admin edit form (`$prospectId.edit.tsx`) + create form (`create.tsx`) to expose new fields
- ☐ Update the list view (`columns.tsx`) to add Priority + Stage columns with badge/pill rendering
- ☐ Update filters on the prospect list page to include Priority + Municipality Type + Pipeline Stage

### Phase 2 — Weekly Report screen (backend + web)

- ☐ New endpoint `GET /api/v2/reports/weekly?weekEnding=YYYY-MM-DD` returning the following in one payload:
  - Period start / end (calculated from `weekEnding` — Monday → Sunday by default; make first day of week configurable)
  - Prospects researched (count of `CommunityProspect` with `CreatedDate` in window)
  - New contacts added (count of `ProspectContact` created in window)
  - Outreach touches (count of `ProspectActivity` with type = Outreach in window)
  - Follow-up touches (count with type = Follow-up)
  - Responses / conversations (count with type = Response received)
  - Meetings requested (count with type = Meeting requested)
  - Meetings scheduled (count with type = Meeting scheduled)
  - Aggregated "Key Municipal Feedback" — concatenation of non-empty `KeyObjection` fields on prospects touched in the window
  - Aggregated "Pricing / Business Model Feedback" — concatenation of non-empty `PricingFeedback` fields
  - Free-text "Next Steps" (see Phase 4)
- ☐ New page `/siteadmin/prospects/reports/weekly` reading the endpoint, rendered with the same visual layout as Cynthia's spreadsheet so the transition is invisible
- ☐ Week picker + prev/next navigation
- ☐ CSV export button

### Phase 3 — Monthly Goals + market intelligence (backend + web)

- ☐ New `SalesMonthlyTarget` entity: `Month` (first-of-month `DateOnly`), `Metric` (enum: ProspectsResearched, NewContacts, OutreachTouches, FollowUpTouches, Responses, MeetingsRequested, MeetingsScheduled), `Target` (int), `Notes` (nvarchar(500))
- ☐ Seed default targets from Cynthia's baseline: `20, 20, 15, 10, 3, 2, 1`
- ☐ New endpoint `GET /api/v2/reports/monthly?month=YYYY-MM` returning the same 7 metrics with target + actual + status (`Behind` if actual < 70% of target, `On track` if 70–110%, `Exceeded` if > 110%)
- ☐ Endpoint `PUT /api/v2/reports/monthly/{month}/targets` to update targets in place
- ☐ New page `/siteadmin/prospects/reports/monthly` reading the endpoint
- ☐ **Market Intelligence Notes** section below the metrics — top-N breakdown of:
  - Best responding departments (group prospects with `type=Response received` activity in the month by `Department`)
  - Common objections (top-N unique `KeyObjection` values across prospects touched in the month)
  - Pricing feedback (top-N `PricingFeedback` values)
  - Messaging that worked (from `ProspectActivity.Details` on `type=Response received` records — long-text summary; may need free-text input to complement)
  - Recommended next-month priority (free-text field on `MonthlyReport` entity — see Phase 4)

### Phase 4 — Report free-text sections + scheduled email delivery (backend + jobs)

- ☐ New `SalesReport` entity: `PeriodType` (Weekly / Monthly), `PeriodStart`, `PeriodEnd`, `NextSteps` (nvarchar(2000)), `NextMonthPriority` (nvarchar(2000)), `CreatedByUserId`, standard audit fields. One row per period per type — the free-text side of the report that doesn't come from queries.
- ☐ CRUD endpoints for `SalesReport`
- ☐ On the weekly / monthly report screens, add a "Save narrative" panel at the bottom for `NextSteps` (weekly) / `NextMonthPriority` (monthly). Auto-saves on blur. Read on page load if a row exists for the period.
- ☐ New scheduled job in [`TrashMobHourlyJobs`](../../TrashMobHourlyJobs/): every Monday 08:00 America/Los_Angeles, generate the weekly report for the just-ended week and email it to a distribution list. Every 1st of month at 08:00, do the same for the previous month.
- ☐ Distribution list stored as a `SalesReportSubscriber` mini-entity (User FK + `IncludeWeekly` + `IncludeMonthly`) so Cynthia + Board can opt in/out without an engineering change
- ☐ Email templates (in `TrashMob.Shared/Engine/EmailCopy/`): `SalesReportWeekly.html`, `SalesReportMonthly.html` — render the same layout as the on-screen reports

### Phase 5 — Nice-to-haves (deferred; open a follow-up if any block progress)

- ☐ Human-readable prospect IDs (`TM-MUN-001` style) as a computed / stored `DisplayId` column
- ☐ CSV / PDF export of the pipeline itself (not just the reports)
- ☐ "Cohort" view — how did the salesperson's Month 1 cohort of prospects progress by Month 3 vs Month 2 cohort?
- ☐ Board dashboard tie-in — surface the monthly metrics on the [Project 56 (Board Metrics Dashboard)](./Project_56_Board_Metrics_Dashboard.md) once that lands
- ☐ Slack notification on stage changes ("Alameda County moved to Meeting Scheduled")

---

## Out of Scope

- **Full pipeline import from Cynthia's spreadsheet.** Manual first-time entry is fine for ~20 prospects; a one-off import script is available via existing `import.tsx` if she really wants it.
- **Multi-tenant CRM.** One organisation, one salesperson today. Do not build tenant separation for this project. If [Project 61 (Aegis)](./Project_61_Aegis_Municipal_OS_Spinout_Evaluation.md) triggers Option C spinout, tenant separation lands then, not now.
- **Deal / opportunity / quote pricing.** We're not doing e-signature contracts or a formal quote workflow in this project. When a city says yes, we convert them to a `Partner` via the existing `ConvertedPartnerId` field and take contract flow into whatever the partner-onboarding project produces.
- **Automated outreach.** No sending emails on behalf of the salesperson from within TrashMob. All emails still go from a real Gmail/Outlook account; we log them after the fact via `ProspectOutreachEmail`.
- **Board Metrics Dashboard cross-integration** — deferred to Project 56.

---

## Success Metrics

### Quantitative

- **Zero double-entry:** the salesperson never needs to touch Cynthia's spreadsheet after Phase 1 ships (measured by asking her at 30/60/90 days).
- **Weekly report is emailed with real data every Monday for 3 consecutive months** before we consider Phase 4 stable.
- **Monthly report shows metrics on the same grid as the spreadsheet** — matches the layout Cynthia proposed within ±1 column.
- **All 7 monthly metrics report actuals within 5 minutes of the source touchpoint being logged** (real-time enough that the salesperson can trust the numbers when they check on Friday afternoon).
- **Time to close a weekly review meeting drops from ~45 minutes to ≤ 15 minutes** once we're on the auto-generated report (baseline: Cynthia and Joe's current dry-run).

### Qualitative

- The salesperson prefers the CRM to their own private spreadsheet after 30 days.
- Cynthia can walk into a Board meeting with the monthly report as her only slide.
- The Market Intelligence Notes surface at least one insight per month that changes messaging or target list (i.e. the data is actionable, not just descriptive).

---

## Dependencies

### Blockers (Must be complete before this project starts)

- **[Project 60 — Prospect Multi-Contact Tracking](./Project_60_Prospect_Contact_Tracking.md):** Phase 1 (model) and Phase 2 (API) already shipped; Phase 3 (admin UI) also shipped. This project reuses that infrastructure directly. Nothing to wait for.
- **[Project 64 — Roles & RBAC Refactor](./Project_64_Roles_and_RBAC_Refactor.md):** the 1099 salesperson needs a scoped `SalesRep` role rather than the sledgehammer of `IsSiteAdmin`. At minimum Project 64 Phase 1 (data model + policy migration) needs to ship before this project's Phase 1 goes live in production; the `SalesRep` role itself is seeded in Project 64.

### Enablers for Other Projects (What this unlocks)

- **[Project 56 — Board Metrics Dashboard](./Project_56_Board_Metrics_Dashboard.md):** the monthly metrics + market intelligence become natural cards on the Board dashboard once both land.

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| **Salesperson doesn't log activities consistently → reports look wrong** | High | High | Keep the activity-logging UI dead simple (one click from prospect detail). Include a "days since last touch" chip on the list view so the salesperson sees stale rows and self-corrects. Run a review call at Day 14 with the salesperson to check that the activity types are being applied correctly. |
| **Spreadsheet vs. app metric definitions drift over time** | Medium | Medium | Metric definitions live in one place — `SalesReportMetrics.cs` (or similar) with a docstring per metric. Any change goes through a code review, not a Slack message. |
| **Weekly email is noisy and gets muted → nobody reads it** | Medium | Medium | Design the email so the top 3 metrics are the whole thing; details are one click away. Send only if there was at least one activity in the window; skip empty weeks. |
| **Free-text fields (`KeyObjection`, `PricingFeedback`) get inconsistent phrasing → aggregation is meaningless** | High | Medium | Accept the messiness in Phase 2. In Phase 3, introduce a light auto-suggest / dedupe pass (Levenshtein distance) that hints "did you mean 'budget constraints'?" when typing. Not a strict enum — the salesperson needs freedom. |
| **`Type` free-form → enum migration collides with existing data** | Medium | Low | Migration includes a lookup table pass; anything unmapped goes to `Other` with the original string preserved in a `TypeRaw` column so nothing is lost. |
| **Cynthia and the Board treat this as the source of truth *before* the salesperson has trained** | Medium | High | Explicit "beta" banner on the report pages for the first 60 days. Weekly cadence: Cynthia + Joe + salesperson review the numbers *together* for the first 4 weeks before treating them as Board-grade. |
| **Scope creep — the "sales OS" instinct kicks in and this becomes 6 months of work** | High | High | Phase 1 ships in 2 weeks or the scope is wrong. Phase 5 items don't count as commitments; they are explicitly opened as follow-up projects if pursued. Track WIP against Project 62's cap. |
| **Salesperson leaves; institutional knowledge disappears** | Medium | Medium | Every prospect has an `Owner` (a User FK we already have via audit fields). Reports work per-owner or org-wide. Handoff to a new salesperson is a matter of reassigning owner, not exporting/importing data. |
| **Municipal Type migration downcasting existing "Type" values loses metadata** | Low | Low | Preserve original string in `TypeRaw` column during migration; audit logs the mapping decisions. |
| **Cynthia's monthly targets are wrong for the first hire (too high or too low)** | High | Low | Targets are editable via the UI. Don't hardcode them beyond the initial seed. Revisit at end of Month 1. |

---

## Implementation Plan

### Data Model Changes

```sql
-- Phase 1 — CommunityProspect extensions
ALTER TABLE CommunityProspects ADD Department NVARCHAR(120) NULL;
ALTER TABLE CommunityProspects ADD Priority INT NULL; -- enum: 1=High, 2=Medium, 3=Low
ALTER TABLE CommunityProspects ADD PricingFeedback NVARCHAR(500) NULL;
ALTER TABLE CommunityProspects ADD KeyObjection NVARCHAR(500) NULL;
ALTER TABLE CommunityProspects ADD TypeRaw NVARCHAR(120) NULL; -- preserve original Type string during enum migration
-- Constrain existing Type to MunicipalityType enum values via data migration
-- PipelineStage int already exists — expand the enum to 10 values in code, no schema change

-- Phase 3 — Monthly targets
CREATE TABLE SalesMonthlyTargets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Month DATE NOT NULL,
    Metric INT NOT NULL, -- enum: ProspectsResearched=1 ... MeetingsScheduled=7
    Target INT NOT NULL,
    Notes NVARCHAR(500) NULL,
    CreatedDate DATETIMEOFFSET NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    LastUpdatedDate DATETIMEOFFSET NOT NULL,
    LastUpdatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT UQ_SalesMonthlyTargets_MonthMetric UNIQUE (Month, Metric)
);
CREATE INDEX IX_SalesMonthlyTargets_Month ON SalesMonthlyTargets(Month);

-- Phase 4 — Free-text sections + subscribers
CREATE TABLE SalesReports (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PeriodType INT NOT NULL, -- 1=Weekly, 2=Monthly
    PeriodStart DATE NOT NULL,
    PeriodEnd DATE NOT NULL,
    NextSteps NVARCHAR(2000) NULL,
    NextMonthPriority NVARCHAR(2000) NULL,
    CreatedDate DATETIMEOFFSET NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    LastUpdatedDate DATETIMEOFFSET NOT NULL,
    LastUpdatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT UQ_SalesReports_PeriodTypeStart UNIQUE (PeriodType, PeriodStart)
);

CREATE TABLE SalesReportSubscribers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    IncludeWeekly BIT NOT NULL DEFAULT 1,
    IncludeMonthly BIT NOT NULL DEFAULT 1,
    -- audit fields
    CONSTRAINT UQ_SalesReportSubscribers_User UNIQUE (UserId)
);
```

### API Changes

```csharp
// Phase 2 — Weekly report
[HttpGet("api/v2/reports/sales/weekly")]
[Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
public async Task<ActionResult<WeeklySalesReportDto>> GetWeeklyReport(
    [FromQuery] DateOnly weekEnding, CancellationToken ct)
{
    // Compute period, count activities by type, aggregate feedback strings,
    // read SalesReport.NextSteps for the period, return as one payload.
}

// Phase 3 — Monthly report
[HttpGet("api/v2/reports/sales/monthly")]
public async Task<ActionResult<MonthlySalesReportDto>> GetMonthlyReport(
    [FromQuery] DateOnly month, CancellationToken ct);

[HttpPut("api/v2/reports/sales/monthly/{month}/targets")]
public async Task<IActionResult> UpdateMonthlyTargets(
    DateOnly month, [FromBody] MonthlyTargetsUpdateDto targets, CancellationToken ct);

// Phase 4 — Free-text sections
[HttpPut("api/v2/reports/sales/{periodType}/{periodStart}")]
public async Task<IActionResult> UpsertReportNarrative(
    string periodType, DateOnly periodStart, [FromBody] SalesReportNarrativeDto body,
    CancellationToken ct);
```

### Web UX Changes

- **Prospect list (`/siteadmin/prospects`):** add Priority and Stage columns to `columns.tsx`; add filters to the toolbar
- **Prospect detail (`/siteadmin/prospects/$prospectId`):** show the new fields prominently (Priority pill in the header, Pricing Feedback + Key Objection in a "Signal" card)
- **Prospect edit form (`$prospectId.edit.tsx`):** new fields in the same order Cynthia's spreadsheet lists them
- **`/siteadmin/prospects/reports/weekly`** — grid layout matching Cynthia's spreadsheet exactly, week picker, prev/next arrows, CSV export
- **`/siteadmin/prospects/reports/monthly`** — grid of the 7 metrics with target vs actual columns, status badges, Market Intelligence Notes below, editable free-text panel at the bottom for `NextMonthPriority`
- **Subscription toggle** on user settings so anyone can opt in / out of the emails without going through admin

### Mobile App Changes

- **Read-only Weekly Report screen** for Cynthia's phone during Monday morning coffee. No editing needed on mobile — the salesperson lives at a laptop for outreach work.
- **Push notification** on Monday when the weekly report drops (optional, gated behind Project 12 - In-App Messaging when it lands).

### Background Jobs

- **`WeeklySalesReportEmailJob`** in `TrashMobHourlyJobs`, fires every Monday 08:00 PT
- **`MonthlySalesReportEmailJob`** fires on the 1st of each month at 08:00 PT

---

## Implementation Phases

### Phase 1 — Model + admin form extensions (~2 weeks, ships first)

Salesperson can start using the CRM immediately after this ships. Everything else is layered on afterwards.

### Phase 2 — Weekly Report screen (~1 week)

Delivers the Board's most-frequently-requested artifact.

### Phase 3 — Monthly Goals + market intelligence (~2 weeks)

Delivers the strategic input Cynthia needs for Board meetings.

### Phase 4 — Scheduled email delivery (~1 week)

Makes the reports show up in inboxes without anyone logging in.

### Phase 5 — Nice-to-haves

Deferred; each item is a candidate follow-up project, not a commitment.

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **`SalesRep` role — tracked separately as [Project 64](./Project_64_Roles_and_RBAC_Refactor.md).**
   **Recommendation:** The role itself is small. But the current `IsSiteAdmin` boolean on `User` doesn't extend cleanly to a second role, so Project 64 delivers the RBAC infrastructure and this project depends on it. Coordinate so Project 64 Phase 1 (data model + policy migration) lands before Project 63 Phase 1 goes to production.
   **Owner:** Joe
   **Due:** Coordinated with Project 64 Phase 1

2. **Does the salesperson want the CRM optimised for one-at-a-time detail work, or bulk day-planning ("show me everyone I owe a follow-up to today")?**
   **Recommendation:** Both. Phase 1 delivers detail work. Phase 1.5 (added if the salesperson asks in the first 2 weeks) adds a "Today" dashboard on the prospect list that groups by `NextFollowUpDate`.
   **Owner:** Cynthia (surface the question to the hire) + salesperson
   **Due:** 30 days after salesperson start

3. **Distribution list for the weekly / monthly emails on Day 1?**
   **Recommendation:** Cynthia + Joe + salesperson at minimum. Add Board members once we've run 3 weekly cycles cleanly.
   **Owner:** Cynthia
   **Due:** Before Phase 4 ships

4. **What happens to the `Type` free-form values that don't map to the new enum?**
   **Recommendation:** Bucket to `Other`, preserve original in `TypeRaw`. Log the mapping decisions during migration. Rarely-used enough today that the manual review is trivial.
   **Owner:** Joe
   **Due:** Before Phase 1 migration runs

5. **Do the weekly / monthly emails need TrashMob branding, or plain-text-first for deliverability into Cynthia's / Board members' inboxes?**
   **Recommendation:** HTML with a minimal plain-text fallback. Reuse the [existing SendGrid dynamic template pattern](../../TrashMob.Shared/Engine/EmailCopy/) and keep the template deliberately simple.
   **Owner:** Joe
   **Due:** Phase 4 kickoff

---

## Related Documents

- **[Project 60 - Prospect Multi-Contact Tracking](./Project_60_Prospect_Contact_Tracking.md)** — the model and API foundation this project extends
- **[Project 64 - Roles & RBAC Refactor](./Project_64_Roles_and_RBAC_Refactor.md)** — delivers the `SalesRep` role and the underlying RBAC infrastructure
- **[Project 56 - Board Metrics Dashboard](./Project_56_Board_Metrics_Dashboard.md)** — future dashboard that will surface these same metrics alongside App Insights, GA4, Sentry, Clarity, Azure costs, and QuickBooks
- **[Project 41 - Sponsored Adoptions](./Project_41_Sponsored_Adoptions.md)** — existing paid-relationship model within the nonprofit; adjacent revenue channel, worth cross-referencing when the salesperson pitches
- **[Project 46 - Product Support](./Project_46_Product_Support.md)** — the sister function once cities become paying customers
- **[Project 62 - TrashMob Site and Codebase Re-evaluation](./Project_62_TrashMob_Site_and_Codebase_Reevaluation.md)** — Engineering health check that catalogues sales-readiness gaps

---

**Last Updated:** 2026-07-03
**Owner:** Joe (engineering) + Cynthia (sales lead + Board) + 1099 salesperson
**Status:** Planning
**Next Review:** 2026-07-10 — align on Phase 1 scope before starting model changes
