# Project 51 — Contact Management System

| Attribute | Value |
|-----------|-------|
| **Status** | ✅ Complete (all 8 phases) |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | None (new standalone admin feature) |

---

## Business Rationale

As TrashMob.eco grows from a volunteer platform into a sustainable nonprofit organization, it needs internal tools to manage the relationships that fund its mission. Today there is no way to track:

- **Donations** — When a user or external supporter donates, there's no record in the system
- **Donors** — Non-registered individuals and organizations who financially support TrashMob have no representation in the platform
- **Grants** — Applications, deadlines, reporting requirements, and funding source research are managed ad hoc (spreadsheets, email)
- **Prospects** — Potential corporate partners and sponsors exist only in personal knowledge, not in shared organizational memory

A lightweight Contact Management System (CMS/CRM) built into the Site Admin area gives the TrashMob team a single place to manage all fundraising relationships, track grant pipelines, send acknowledgments, and identify new funding opportunities — without the cost and complexity of a third-party CRM.

**All features in this project are restricted to Site Admin access only.**

---

## Objectives

### Primary Goals
- **Donor tracking** — Record donations (cash, in-kind, matching gifts) from both registered users and external contacts
- **Contact management** — Maintain records for prospects, donors, foundation contacts, and corporate leads not yet converted to Partners
- **Grant management** — Track the full grant lifecycle: research → application → award → reporting → renewal
- **Communication tools** — Send thank-you messages, donation receipts, and fundraising appeals via email
- **Grant source discovery** — AI-assisted search for grant opportunities relevant to TrashMob's environmental cleanup mission

### Secondary Goals
- **Engagement scoring** — Algorithmic score combining volunteer activity, donations, and event attendance
- **Stewardship automation** — Triggered workflows (e.g., auto thank-you after donation, anniversary recognition)
- **Volunteer-to-donor pipeline** — Identify highly engaged volunteers and track cultivation toward financial support
- **Reporting dashboards** — Fundraising totals, donor retention, grant pipeline, and impact metrics for board reporting

---

## Scope

### Phase 1 — Contact & Donor Data Model ✅ (PR #2945)

- ✅ Create `Contact` entity — represents any person or organization the nonprofit interacts with (donors, prospects, foundation officers, corporate leads)
- ✅ Create `Donation` entity — tracks individual gifts linked to a Contact (and optionally to a registered User)
- ✅ Create `ContactNote` entity — timestamped interaction log (calls, meetings, emails, observations)
- ✅ Create `ContactTag` entity — flexible tagging for segmentation (e.g., "Major Donor", "Corporate Lead", "Foundation")
- ✅ Link Contact ↔ User (optional) — a Contact may or may not correspond to a registered TrashMob user
- ✅ Link Contact ↔ Partner (optional) — a Contact may be associated with an existing Partner organization
- ✅ Database migrations with proper indexes and audit fields

### Phase 2 — Contact Management UI (Admin) ✅ (PR #2946)

- ✅ Contact list page with search, filter by tag, and sort by last interaction
- ✅ Contact detail page showing profile, donation history, notes timeline, tags, and linked User/Partner
- ✅ Add/edit contact form (name, email, phone, organization, address, type, source)
- ✅ Contact notes: add timestamped notes with category (Call, Meeting, Email, Note)
- ✅ Tag management: create, assign, remove tags on contacts
- ✅ Duplicate detection: warn when creating a contact with matching name or email

### Phase 3 — Donation Tracking ✅ (PR #2948)

- ✅ Record donations: amount, date, type (Cash, Check, Online, In-Kind, Matching Gift), campaign, notes
- ✅ Recurring donation tracking: link related gifts, track frequency, flag missed payments
- ✅ Pledge management: record multi-payment pledges with schedules and fulfillment status
- ✅ In-kind donation tracking: description, estimated fair market value, donor
- ✅ Donation list with filtering by date range, amount, type, donor, campaign
- ✅ Donation summary on Contact detail page (lifetime total, this year, last gift date, giving trend)

### Phase 4 — Thank You & Appeal Communications ✅ (PR #2949)

- ✅ Thank-you email templates (first-time donor, repeat donor, major gift, in-kind)
- ✅ Send thank-you from Contact detail page, auto-logged as a ContactNote
- ✅ Fundraising appeal email templates with merge fields (name, last gift amount, impact stats)
- ✅ Bulk appeal: select contacts by tag/segment, send personalized appeal emails
- ✅ Tax receipt generation: IRS-compliant donation acknowledgment with organization details
- ✅ Communication history: track all outbound emails with open/delivery status
- ✅ Document attachment library: admin-managed library of PDF/DOCX files (annual reports, impact summaries, sponsorship decks) that can be attached to outbound emails

### Phase 5 — Grant Management ✅ (PR #2954)

- ✅ Create `Grant` entity — represents a grant opportunity (funder, program, amount range, deadlines, status)
- ✅ Create `GrantTask` entity — checklist items for each grant (application steps, reporting deliverables)
- ✅ Grant lifecycle tracking: Prospect → LOI Submitted → Application Submitted → Awarded / Declined → Reporting → Renewal
- ✅ Grant list page with status pipeline view (table with status filters)
- ✅ Grant detail page: funder info, amounts, deadlines, task checklist, notes
- ❌ Deadline calendar: upcoming grant deadlines with automated email reminders
- ✅ Link grants to Contacts (funder program officers) for relationship tracking
- ❌ Budget tracking per grant: awarded amount, expenditures, remaining balance

### Phase 6 — AI Grant Source Discovery ✅ (PR #2957)

- ✅ Search interface: describe funding needs and TrashMob's mission, get AI-suggested grant opportunities
- ✅ GrantDiscoveryService: Claude API integration for grant research (mirrors ClaudeDiscoveryService pattern)
- ✅ Discovery UI with "Custom Query" and "By Focus Area" tabs
- ✅ Save discovered opportunities directly as Grant records via "Add to Pipeline" pre-fill
- ❌ Periodic scan: scheduled job to check for new opportunities matching saved search criteria (deferred)

### Phase 7 — Engagement Scoring & Pipeline ✅ (PR #2959)

- ✅ Engagement score algorithm (0–100) combining: donation history (recency, frequency, amount), volunteer activity (events attended, hours), interaction frequency (notes)
- ✅ Score displayed on Contact detail page with breakdown and on engagement page (sortable)
- ✅ Score redistribution when contact has no linked volunteer account
- ✅ Volunteer-to-donor pipeline: identify top-engaged volunteers who haven't donated
- ✅ Donor lifecycle stages: Prospect → First-Time Donor → Repeat Donor → Major Donor → Lapsed (auto-calculated)
- ✅ Lapsed donor alerts: LYBUNT (Last Year But Unfortunately Not This Year) detection and report

### Phase 8 — Reporting & Dashboards ✅ (PR #2959)

- ✅ Fundraising dashboard: total raised (YTD, by campaign, by month), average gift size, donor count
- ✅ Donor retention report: retention rate year-over-year, new vs. repeat donors
- ✅ Grant pipeline report: grants by status, total awarded, pending applications, upcoming deadlines
- ❌ Impact-to-giving report: connect TrashMob event data (bags, weight, volunteers) to donor/grant records for stewardship reporting (deferred)
- ✅ Export reports to CSV for board presentations and grant applications (donor report + fundraising summary)
- ✅ Campaign performance: track fundraising campaigns with goal progress

---

## Out-of-Scope

- ❌ Online donation payment processing (Stripe, PayPal integration) — this project tracks donations received through external channels; payment processing is a separate future project
- ❌ Public-facing donor recognition page ("Wall of Donors") — could be a future enhancement
- ❌ Peer-to-peer fundraising / crowdfunding pages — future project
- ❌ Planned giving / bequest management — premature for current organizational stage
- ❌ Social media monitoring / integration — not needed for admin CRM
- ❌ Mobile app access to CRM features — web admin only
- ❌ Integration with external CRM systems (Salesforce, Bloomerang) — build in-house first
- ❌ Automated recurring payment collection — manual tracking only in this project

---

## Success Metrics

### Quantitative
- **All donations recorded** — 100% of donations tracked in system within 30 days of implementation
- **Contact database populated** — 50+ contacts entered within first quarter
- **Grant pipeline active** — All active grant applications tracked with deadlines
- **Thank-you turnaround** — Donation acknowledgment sent within 48 hours of gift receipt
- **Donor retention** — Establish baseline retention rate, target improvement year-over-year

### Qualitative
- Site admins can find any donor's complete history in under 30 seconds
- Grant deadlines are never missed due to lack of tracking
- Board receives monthly fundraising reports generated from the system
- Team has shared visibility into all fundraising relationships (no single point of failure)

---

## Dependencies

### Blockers
- None — this is a standalone admin feature using existing authentication and authorization infrastructure

### Enables
- **Future: Online Donation Processing** — payment integration can build on the donation tracking model
- **Future: Donor Recognition** — public-facing donor pages can pull from the Contact/Donation data
- **Future: Automated Grant Reporting** — grant reports can pull Event Summary metrics from existing TrashMob data
- **Project 19 (Newsletter)** — newsletter can target contacts by tag/segment from the CRM

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Scope creep** — CRM features expand beyond MVP | High | Medium | Strict phase boundaries; each phase delivers standalone value; resist adding features until current phase is complete |
| **Data entry burden** — staff don't maintain contact records | Medium | High | Keep UI fast and minimal; auto-populate from existing User/Partner data where possible; show immediate value through dashboards |
| **AI grant search accuracy** — recommendations are irrelevant | Medium | Medium | Human review before saving; refine prompts based on feedback; start with well-known environmental grant databases |
| **Email deliverability** — thank-you/appeal emails go to spam | Low | Medium | Use existing SendGrid infrastructure with proper authentication; follow email best practices |
| **Privacy concerns** — storing donor PII | Low | High | Admin-only access; encrypted at rest (Azure SQL TDE); follow existing data handling practices; include in privacy policy |
| **Complexity of grant lifecycle** — too many states and edge cases | Medium | Low | Start with simple linear workflow; add complexity only when real usage reveals needs |

---

## Implementation Plan

### Data Model Changes

**New Entities:**

**`Contact`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `FirstName` | string (100) | Yes | |
| `LastName` | string (100) | Yes | |
| `Email` | string (256) | No | May not have email for all contacts |
| `Phone` | string (20) | No | |
| `OrganizationName` | string (256) | No | Company or foundation name |
| `Title` | string (100) | No | Job title / role |
| `Address` | string (500) | No | Mailing address |
| `City` | string (100) | No | |
| `Region` | string (100) | No | State/province |
| `PostalCode` | string (20) | No | |
| `Country` | string (100) | No | |
| `ContactType` | enum | Yes | Individual, Organization, Foundation |
| `Source` | string (100) | No | How contact was acquired (Event, Referral, Website, etc.) |
| `UserId` | Guid? (FK) | No | Link to registered User if applicable |
| `PartnerId` | Guid? (FK) | No | Link to Partner if applicable |
| `Notes` | string (2000) | No | General notes |
| `IsActive` | bool | Yes | Soft active/inactive flag |
| Audit fields | | | CreatedDate, CreatedByUserId, LastUpdatedDate, LastUpdatedByUserId |

**`Donation`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `ContactId` | Guid (FK) | Yes | Donor |
| `Amount` | decimal | Yes | Dollar amount (or FMV for in-kind) |
| `DonationDate` | DateTimeOffset | Yes | When gift was received |
| `DonationType` | enum | Yes | Cash, Check, Online, InKind, MatchingGift |
| `Campaign` | string (200) | No | Associated fundraising campaign |
| `IsRecurring` | bool | No | Part of a recurring gift |
| `RecurringFrequency` | enum? | No | Monthly, Quarterly, Annually |
| `PledgeId` | Guid? (FK) | No | Link to pledge if applicable |
| `InKindDescription` | string (500) | No | Description for in-kind donations |
| `MatchingGiftEmployer` | string (200) | No | Employer for matching gifts |
| `Notes` | string (2000) | No | |
| `ReceiptSent` | bool | No | Whether tax receipt has been sent |
| `ThankYouSent` | bool | No | Whether thank-you has been sent |
| Audit fields | | | |

**`Pledge`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `ContactId` | Guid (FK) | Yes | Pledgor |
| `TotalAmount` | decimal | Yes | Total pledged amount |
| `StartDate` | DateTimeOffset | Yes | Pledge start |
| `EndDate` | DateTimeOffset | No | Pledge fulfillment deadline |
| `Frequency` | enum | Yes | OneTime, Monthly, Quarterly, Annually |
| `Status` | enum | Yes | Active, Fulfilled, Lapsed, Cancelled |
| `Notes` | string (2000) | No | |
| Audit fields | | | |

**`ContactNote`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `ContactId` | Guid (FK) | Yes | |
| `NoteType` | enum | Yes | Call, Meeting, Email, Note, ThankYou, Appeal |
| `Subject` | string (200) | No | Brief summary |
| `Body` | string (4000) | Yes | Full note text |
| Audit fields | | | CreatedByUserId serves as "logged by" |

**`ContactTag`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `Name` | string (100) | Yes | Tag display name |
| `Color` | string (7) | No | Hex color for UI badge |
| Audit fields | | | |

**`ContactContactTag` (junction):**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `ContactId` | Guid (FK) | Yes | Composite PK |
| `ContactTagId` | Guid (FK) | Yes | Composite PK |

**`Grant`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `FunderName` | string (256) | Yes | Foundation or government agency |
| `ProgramName` | string (256) | No | Specific grant program |
| `Description` | string (2000) | No | Grant purpose and requirements |
| `AmountMin` | decimal? | No | Minimum award amount |
| `AmountMax` | decimal? | No | Maximum award amount |
| `AmountAwarded` | decimal? | No | Actual award amount (if awarded) |
| `Status` | enum | Yes | Prospect, LOISubmitted, ApplicationSubmitted, Awarded, Declined, Reporting, Renewal, Closed |
| `SubmissionDeadline` | DateTimeOffset? | No | Application deadline |
| `AwardDate` | DateTimeOffset? | No | When award was received |
| `ReportingDeadline` | DateTimeOffset? | No | Next report due date |
| `RenewalDate` | DateTimeOffset? | No | Renewal application deadline |
| `FunderContactId` | Guid? (FK) | No | Program officer Contact |
| `GrantUrl` | string (500) | No | Link to grant opportunity page |
| `Notes` | string (4000) | No | |
| Audit fields | | | |

**`GrantTask`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `GrantId` | Guid (FK) | Yes | Parent grant |
| `Title` | string (256) | Yes | Task description |
| `DueDate` | DateTimeOffset? | No | Task deadline |
| `IsCompleted` | bool | Yes | Checklist status |
| `CompletedDate` | DateTimeOffset? | No | When completed |
| `SortOrder` | int | Yes | Display order |
| `Notes` | string (1000) | No | |
| Audit fields | | | |

### API Changes

All endpoints require `[Authorize(Policy = AuthorizationPolicyConstants.SiteAdmin)]`.

**ContactsController:**
- `GET /api/contacts` — List contacts with search, filter, sort, pagination
- `GET /api/contacts/{id}` — Get contact with donation summary, recent notes, tags
- `POST /api/contacts` — Create contact (with duplicate detection)
- `PUT /api/contacts/{id}` — Update contact
- `DELETE /api/contacts/{id}` — Soft delete contact
- `POST /api/contacts/{id}/notes` — Add note to contact
- `GET /api/contacts/{id}/notes` — Get contact notes (paginated)
- `PUT /api/contacts/{id}/tags` — Set tags for a contact

**DonationsController:**
- `GET /api/donations` — List donations with filters (date range, type, donor, campaign)
- `GET /api/donations/{id}` — Get donation detail
- `POST /api/donations` — Record donation
- `PUT /api/donations/{id}` — Update donation
- `DELETE /api/donations/{id}` — Delete donation
- `POST /api/donations/{id}/send-receipt` — Send tax receipt email
- `POST /api/donations/{id}/send-thankyou` — Send thank-you email

**PledgesController:**
- `GET /api/pledges` — List pledges with status filter
- `POST /api/pledges` — Create pledge
- `PUT /api/pledges/{id}` — Update pledge

**GrantsController:**
- `GET /api/grants` — List grants with status filter, sort by deadline
- `GET /api/grants/{id}` — Get grant with tasks and funder contact
- `POST /api/grants` — Create grant
- `PUT /api/grants/{id}` — Update grant (including status transitions)
- `DELETE /api/grants/{id}` — Soft delete grant
- `POST /api/grants/{id}/tasks` — Add task to grant
- `PUT /api/grants/{id}/tasks/{taskId}` — Update task (toggle complete, edit)
- `DELETE /api/grants/{id}/tasks/{taskId}` — Remove task

**ContactTagsController:**
- `GET /api/contacttags` — List all tags
- `POST /api/contacttags` — Create tag
- `PUT /api/contacttags/{id}` — Update tag
- `DELETE /api/contacttags/{id}` — Delete tag

**GrantDiscoveryController (Phase 6):**
- `POST /api/grants/discover` — AI-powered search for grant opportunities
- `POST /api/grants/discover/save` — Save discovered opportunity as Grant record

**ReportsController (Phase 8):**
- `GET /api/reports/fundraising-summary` — YTD totals, donor count, average gift
- `GET /api/reports/donor-retention` — Year-over-year retention metrics
- `GET /api/reports/grant-pipeline` — Grants by status with totals
- `GET /api/reports/lybunt` — Last Year But Unfortunately Not This Year donors
- `GET /api/reports/impact` — Event metrics linked to donor/grant data

### Web UX Changes

**New Admin Pages (under `/admin/`):**

**Contact Management:**
- `/admin/contacts` — Searchable contact list with tag filter pills, engagement score badges
- `/admin/contacts/new` — Add contact form with duplicate detection
- `/admin/contacts/:id` — Contact detail: profile, donation history, notes timeline, tags, linked User/Partner

**Donation Tracking:**
- `/admin/donations` — Donation log with filters, totals, export
- `/admin/donations/new` — Record donation form (linked to contact)

**Grant Management:**
- `/admin/grants` — Grant pipeline table/kanban with status filters and deadline highlights
- `/admin/grants/new` — Create grant opportunity
- `/admin/grants/:id` — Grant detail with task checklist, funder contact, timeline, budget
- `/admin/grants/discover` — AI grant search interface

**Dashboards:**
- `/admin/fundraising` — Fundraising dashboard with charts, KPIs, and quick actions
- `/admin/reports` — Report builder with export options

### Mobile App Changes

None — all CRM features are web admin only.

---

## Implementation Phases

### Phase 1: Contact & Donor Data Model (Foundation) ✅
1. ✅ Create `Contact`, `Donation`, `Pledge`, `ContactNote`, `ContactTag`, `ContactContactTag`, `Grant`, `GrantTask` entities in `TrashMob.Models/`
2. ✅ Add EF Core configurations in `MobDbContext` (indexes, relationships, enums)
3. ✅ Create database migration
4. ✅ Create repository interfaces and implementations
5. ✅ Create manager interfaces and implementations
6. ✅ Register in `ServiceBuilder.cs`
7. ✅ Add unit tests for managers

### Phase 2: Contact Management UI ✅
1. ✅ Create `ContactsController` with CRUD endpoints (SiteAdmin only)
2. ✅ Create `ContactTagsController`
3. ✅ Add TypeScript types and React Query services
4. ✅ Build Contact list page with search, tag filters, pagination
5. ✅ Build Contact detail page with profile, notes, tags
6. ✅ Build Add/Edit Contact form with duplicate detection
7. ✅ Build note entry and timeline display

### Phase 3: Donation Tracking ✅
1. ✅ Create `DonationsController` and `PledgesController`
2. ✅ Add donation TypeScript types and services
3. ✅ Build Donation list page with filters and totals
4. ✅ Build Record Donation form (linked to Contact)
5. ✅ Add donation summary section to Contact detail page
6. ✅ Build Pledge management (create, track, link payments)
7. ✅ Add recurring donation tracking

### Phase 4: Communications ✅
1. ✅ Create thank-you and appeal email templates in `EmailCopy/`
2. ✅ Add send-receipt and send-thankyou endpoints
3. ✅ Build email composition UI on Contact detail page
4. ✅ Implement bulk appeal: select contacts by tag, send personalized emails
5. ✅ Add communication history to Contact notes timeline
6. ✅ Tax receipt PDF generation

### Phase 5: Grant Management ✅
1. ✅ Create `GrantsController` with full CRUD
2. ✅ Add grant TypeScript types and services
3. ✅ Build Grant pipeline page (table with status filters)
4. ✅ Build Grant detail page with task checklist
5. ❌ Build deadline calendar with email reminders (deferred)
6. ✅ Link grants to funder Contacts
7. ❌ Add budget tracking — awarded vs. spent (deferred)

### Phase 6: AI Grant Discovery ✅
1. ✅ Design prompt engineering for grant opportunity search
2. ✅ Implement GrantDiscoveryService with Claude API integration
3. ✅ Build discovery UI with Custom Query and Focus Area tabs
4. ✅ Add discovery endpoint to GrantsController
5. ✅ Save-to-Grant workflow via "Add to Pipeline" with query param pre-fill
6. ❌ Scheduled periodic scan for new opportunities (deferred)

### Phase 7: Engagement Scoring & Pipeline ✅
1. ✅ Design engagement score algorithm (weighted: donation 0–40, volunteer 0–40, interaction 0–20)
2. ✅ Implement on-demand score computation (FundraisingAnalyticsManager, no DB columns)
3. ✅ Add score to Contact detail page (with breakdown) and engagement page (sortable)
4. ✅ Build volunteer-to-donor pipeline view (tabbed on engagement page)
5. ✅ Implement donor lifecycle stage auto-calculation
6. ✅ Build LYBUNT report (tabbed on engagement page)

### Phase 8: Reporting & Dashboards ✅
1. ✅ Build fundraising dashboard with KPI stat cards (YTD totals, retention, average gift)
2. ✅ Build donor retention metrics (year-over-year, new vs. repeat)
3. ✅ Build grant pipeline report (by status, awarded, pending, upcoming deadlines)
4. ❌ Build impact-to-giving report (connect event data to donors) — deferred
5. ✅ Add CSV export for donor report and fundraising summary
6. ✅ Build campaign performance tracking (breakdown by campaign)

**Note:** Phases are sequential but not time-bound. Each phase delivers standalone value. Volunteers pick up work based on priority and availability.

---

## Open Questions

1. **Should we support multiple currencies?**
   **Decision:** Resolved — USD only.
   **Status:** TrashMob operates in the US only. Keep the model simple with USD. Add a currency field later if international expansion happens.

2. **Should donation data sync with an external accounting system?**
   **Decision:** Resolved — CSV export only.
   **Status:** Add CSV export for donations so data can be imported into QuickBooks or other accounting software manually. No direct API integration needed.

3. **Which AI provider for grant discovery?**
   **Decision:** Resolved — Anthropic Claude API.
   **Status:** Use Claude API for grant research and eligibility assessment. Strong reasoning capabilities for evaluating grant fit. Adds a new vendor but worth it for quality of analysis.

4. **Should we integrate with Double the Donation for matching gift verification?**
   **Decision:** Resolved — Defer.
   **Status:** Evaluate after Phase 3 based on actual volume of matching gift donations received. Not worth the integration cost until there's meaningful volume.

5. **Email template system — SendGrid dynamic templates or custom HTML?**
   **Decision:** Resolved — Existing EmailManager pattern.
   **Status:** Use the same HTML template approach already used for event emails. Consistent with existing patterns, no new SendGrid template setup needed.

---

## Additional CRM Features for Future Consideration

The following features were identified as valuable for nonprofit CRMs but are beyond the current project scope:

- **Online donation processing** — Stripe/PayPal integration for website donations
- **Peer-to-peer fundraising** — Supporters create personal fundraising pages tied to events
- **Donor recognition wall** — Public-facing page acknowledging supporters
- **Planned giving management** — Bequests, charitable remainder trusts
- **Board member portal** — Dedicated dashboard for board governance
- **Advocacy / action alerts** — Mobilize supporters around policy issues (anti-littering legislation)
- **Data enrichment** — Auto-append wealth indicators, employer info, philanthropic history
- **Social media engagement tracking** — Tie social interactions to contact records
- **Event-to-fundraising integration** — Enable fundraising tied to specific cleanup events
- **Matching gift automation** — Employer matching program discovery and submission tracking
- **Milestone recognition automation** — Auto-celebrate volunteer milestones (10th event, 100 hours)

---

## Related Documents

- **[Project 19 - Newsletter Support](./Project_19_Newsletter.md)** — Newsletter can target CRM segments
- **[Project 34 - User Feedback](./Project_34_User_Feedback.md)** — Feedback from donors/contacts
- **[Project 40 - AI Community Sales Agent](./Project_40_AI_Community_Sales_Agent.md)** — AI pattern for grant discovery
- **[Project 45 - Community Showcase](./Project_45_Community_Showcase.md)** — Corporate partner conversion funnel

---

**Last Updated:** March 1, 2026
**Owner:** Product & Engineering Team
**Status:** ✅ Complete — All 8 phases delivered (PRs #2945, #2946, #2948, #2949, #2954, #2957, #2959).
**Deferred Items:** Deadline calendar (Phase 5), budget tracking per grant (Phase 5), periodic grant scan (Phase 6), impact-to-giving report (Phase 8)
