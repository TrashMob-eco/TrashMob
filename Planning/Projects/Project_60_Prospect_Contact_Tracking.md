# Project 60 — Prospect Multi-Contact Tracking

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 40 (AI Community Sales Agent) — ✅ Complete |

---

## Business Rationale

Signing a municipality, county, HOA, or other partner organization rarely happens on the first contact. The first email goes to a generic info@ address, gets forwarded twice, lands with someone who isn't the decision-maker, and three weeks later we finally find the right person — the sustainability coordinator, the parks director, the city clerk. Today the `CommunityProspect` record has room for exactly one contact: name, email, title, phone. Every time we discover a new person at the same organization we overwrite the old one or stuff the history into the notes field, losing the audit trail of who we actually reached out to and when.

This causes two real problems:

1. **No accounting trail for a future salesperson hire.** When TrashMob brings on a paid sales/business-development person, we need to demonstrably show "this is how many touchpoints it took to land this community" — both to justify their compensation and to set realistic ramp-up expectations. Without per-contact attempt history we cannot produce that report.
2. **Lost institutional knowledge.** If the volunteer who managed outreach to a city moves on, the next person has no record of "we tried the parks dept., they redirected us to community engagement, who said talk to the mayor's chief of staff." That knowledge is currently in someone's inbox.

This project gives every prospect organization a list of contact people, each with their own outreach history, and attributes each activity (call, email, meeting) to the volunteer who performed it. It's a small data-model change that unlocks accurate pipeline reporting and preserves outreach history across volunteer turnover.

---

## Objectives

### Primary Goals
- **Multiple contacts per prospect:** Each `CommunityProspect` can have any number of people associated with it (name, title, email, phone, role, status)
- **Per-contact activity history:** `ProspectActivity` and `ProspectOutreachEmail` can be linked to a specific contact person, not just the prospect
- **Attempt attribution:** Every activity records which volunteer performed it (via existing `CreatedByUserId` audit field) and the timestamp, so we can aggregate "X attempts by Y people to land this prospect"
- **Contact status tracking:** Mark a contact as Active, Wrong Person, No Response, Left Organization, or Right Person so future outreach skips dead ends
- **Referral chain:** Optional "referred by" link so we can record "person A pointed us at person B"

### Secondary Goals (Nice-to-Have)
- Per-user activity report ("touchpoints by volunteer, last 90 days") — foundation for future salesperson accounting
- Per-prospect summary card on the prospect detail page: total attempts, distinct contacts tried, days-in-pipeline
- Bulk import contacts from CSV (for prospects where we already have a contact list from a partner directory)

---

## Scope

### Phase 1 — Backend: Data Model & Migration
- ☐ Create `ProspectContact` entity (`TrashMob.Models/ProspectContact.cs`, extends `KeyedModel`) with fields:
  - `ProspectId` (FK to `CommunityProspect`)
  - `Name`, `Title`, `Email`, `Phone`
  - `Role` — free-text or enum: e.g. "Sustainability Coordinator", "Parks Director", "Front Desk"
  - `ContactStatus` — enum: `Active`, `WrongPerson`, `NoResponse`, `LeftOrganization`, `RightPerson`
  - `ReferredByContactId` (nullable self-FK) — captures who pointed us at this person
  - `IsPrimary` (bool) — designates the current best contact for the prospect
  - `Notes` — string(2000)
  - Standard audit fields (CreatedDate, CreatedByUserId, LastUpdatedDate, LastUpdatedByUserId)
- ☐ Add `ProspectContactId` (nullable `Guid?`) to `ProspectActivity`
- ☐ Add `ProspectContactId` (nullable `Guid?`) to `ProspectOutreachEmail`
- ☐ Configure FK relationships and indexes in `MobDbContext` (`IX_ProspectContact_ProspectId`, `IX_ProspectActivity_ProspectContactId`, `IX_ProspectOutreachEmail_ProspectContactId`)
- ☐ EF Core migration with **data backfill**: for every existing `CommunityProspect` where `ContactName` or `ContactEmail` is populated, insert a `ProspectContact` row (`IsPrimary = true`, `ContactStatus = Active`) and copy values across
- ☐ Migration drops the legacy `ContactName`, `ContactEmail`, `ContactTitle`, `ContactPhone` columns from `CommunityProspect` **after** the backfill step
- ☐ Update `CommunityProspect` POCO: remove `ContactName/Email/Title/Phone`, add `virtual ICollection<ProspectContact> Contacts`
- ☐ Update `CommunityProspectDto` (V2) accordingly; add `ProspectContactDto`

### Phase 2 — Backend: Manager & API Layer
- ☐ Create `IProspectContactManager` / `ProspectContactManager` (extend `KeyedManager<ProspectContact>`)
  - Methods: `GetByProspectAsync`, `SetPrimaryAsync` (atomically clear other primaries on the same prospect), `UpdateStatusAsync`
- ☐ Register in `ServiceBuilder.cs`
- ☐ Create v2 controller `ProspectContactsController` at `api/v2/prospects/{prospectId}/contacts`:
  - `GET /` — list contacts for a prospect
  - `GET /{contactId}` — single contact
  - `POST /` — add contact (admin only)
  - `PUT /{contactId}` — update contact
  - `DELETE /{contactId}` — soft delete (or hard delete if no activities reference it)
  - `POST /{contactId}/set-primary` — designate primary contact
- ☐ All endpoints require `[Authorize(Policy = AuthorizationPolicyConstants.SiteAdmin)]` — same authorization as existing admin prospect endpoints
- ☐ Extend activity / outreach endpoints to accept an optional `prospectContactId` when logging a call/meeting or sending an email
- ☐ Extend `ProspectOutreachManager` to send the outreach email to the selected contact's address (falling back to primary contact if none specified)

### Phase 3 — Frontend: Admin Prospect Detail Page
- ☐ Add a **Contacts** section to the existing `/siteadmin/prospects/$prospectId.tsx` detail page
  - Table of contacts: Name, Title, Email, Phone, Status, Last Contacted, Actions
  - "Add Contact" button → modal form
  - Inline status badge (color-coded: Active = blue, WrongPerson = gray, NoResponse = yellow, LeftOrganization = gray, RightPerson = green)
  - "Set as primary" action on each row
  - Edit / delete actions per row
- ☐ Add a contact selector to the **Log Activity** modal (dropdown of this prospect's contacts, defaults to primary)
- ☐ Add a contact selector to the **Send Outreach Email** flow (defaults to primary contact's email)
- ☐ Update the prospect summary card to show:
  - "X contacts tried, Y attempts" (count of activities + outreach emails)
  - Primary contact name + status
- ☐ Add a "Referred by" dropdown on the contact form (filterable list of other contacts at this prospect)
- ☐ Create React Query service `prospect-contacts.ts` following the service factory pattern

### Phase 4 — Reporting & Polish
- ☐ Add a per-user activity tally on the admin prospect dashboard: "Touchpoints last 30/90 days, by volunteer" (groups `ProspectActivity` and `ProspectOutreachEmail` by `CreatedByUserId`)
- ☐ Add a "Days in pipeline" column to the prospect list page (computed from `CreatedDate` of the prospect)
- ☐ Update the activity timeline on the prospect detail page to show **which contact** each activity targeted (e.g., "Jane Doe — Email sent — 'Intro to TrashMob'")
- ☐ Verify the AI outreach generation in `ProspectOutreachManager` still works with the new model — outreach defaults to the primary contact, falls back gracefully if no contact exists

---

## Out-of-Scope

- ❌ Community-scoped prospects (Project 54). This project applies to the **global admin prospect pipeline only**. Community-scoped prospects can adopt the same model in a follow-up project if needed.
- ❌ Time-tracking per activity (`DurationMinutes`). Effort attribution is "count of attempts per user" — sufficient for the current stage. Explicit time tracking can come later if/when a paid salesperson is on board and needs billable-hour reports.
- ❌ Commission / compensation calculations. The data this project captures is the **input** to a future compensation model, not the model itself.
- ❌ Contact deduplication across prospects. If "John Smith" appears at two cities, they're two separate `ProspectContact` rows. No global contact index.
- ❌ Mobile app — admin web only.
- ❌ Public-facing display of any contact info — admin-only data.
- ❌ Bulk CSV import of contacts (listed as Nice-to-Have only; not in core scope).

---

## Success Metrics

### Quantitative
- **Migration completeness:** 100% of existing `CommunityProspect` rows with non-null contact fields have a corresponding `ProspectContact` (`IsPrimary = true`) after the migration runs
- **Adoption:** Within 60 days of release, ≥ 70% of new outreach activities on the admin pipeline reference a specific `ProspectContactId` (i.e., users are picking a contact, not just leaving it null)
- **Audit trail coverage:** ≥ 90% of converted prospects (status = Converted) have ≥ 2 distinct contacts on file by the time of conversion
- **Zero data loss:** No production support tickets caused by missing contact info post-migration

### Qualitative
- Admin users can answer "who did we try at this city, and what did they say?" without leaving the prospect detail page
- The per-user activity tally produces a sensible report that the board / founder can use when justifying a future salesperson hire
- New volunteer admins can pick up an in-flight prospect and understand the contact history at a glance

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Project 40 (AI Community Sales Agent):** ✅ Complete — provides the `CommunityProspect`, `ProspectActivity`, and `ProspectOutreachEmail` models this project extends

### Enables Other Projects
- **Future salesperson hire / compensation model:** The per-user attempt tally is the audit data required to justify and structure compensation
- **Project 54 (Community Adoption Outreach):** Same multi-contact pattern can be adopted for community-scoped prospects in a follow-up

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Migration drops legacy columns before backfill succeeds** | Low | High | Migration is split: step 1 inserts `ProspectContact` rows from existing data; step 2 drops legacy columns. Both steps in one migration so rollback is atomic. Verify row counts before drop. Test on a restored prod backup before running on prod. |
| **AI outreach generation breaks when primary contact is null** | Medium | Medium | Outreach manager has a fallback: if no primary contact exists, fall back to the prospect's generic info (org name, website). Add unit tests covering the null-contact path. |
| **Activity log becomes noisy with stale "Wrong Person" contacts** | Medium | Low | UI filter on the contacts table defaults to hiding `WrongPerson` / `LeftOrganization` rows; admin can toggle to show all. |
| **Volunteers don't bother attaching a contact to activities** | Medium | Medium | Default the activity modal's contact selector to the primary contact so the common case is one click. Track `% of activities with ProspectContactId` and revisit UX if adoption is low. |
| **Existing API consumers break when legacy fields disappear from the DTO** | Low | Medium | The only consumer of these fields is the admin web app (this project updates it). External integrations don't use the admin prospect endpoints. Verify with a grep of the codebase before merging. |

---

## Implementation Plan

### Existing Infrastructure to Extend

| Component | File | Change |
|-----------|------|--------|
| `CommunityProspect` model | `TrashMob.Models/CommunityProspect.cs` | Drop legacy contact fields; add `Contacts` collection |
| `ProspectActivity` model | `TrashMob.Models/ProspectActivity.cs` | Add `ProspectContactId?` and navigation property |
| `ProspectOutreachEmail` model | `TrashMob.Models/ProspectOutreachEmail.cs` | Add `ProspectContactId?` and navigation property |
| `MobDbContext` | `TrashMob.Shared/Persistence/MobDbContext.cs` | Add `DbSet<ProspectContact>`; configure FKs and indexes |
| `CommunityProspectDto` | `TrashMob.Models/Poco/V2/CommunityProspectDto.cs` | Replace single-contact fields with `Contacts` array |
| `ProspectOutreachManager` | `TrashMob.Shared/Managers/Prospects/ProspectOutreachManager.cs` | Resolve target email from `ProspectContact` (primary or selected); null-safe fallback |
| Admin prospect detail page | `TrashMob/client-app/src/pages/siteadmin/prospects/$prospectId.tsx` | Add Contacts section, update activity / outreach modals |
| Admin prospect list page | `TrashMob/client-app/src/pages/siteadmin/prospects/page.tsx` | Show primary contact name + attempt count column |

### New Code to Create

| Component | File | Pattern to Follow |
|-----------|------|-------------------|
| `ProspectContact` entity | `TrashMob.Models/ProspectContact.cs` | `ProspectActivity.cs` |
| `ProspectContactDto` (V2) | `TrashMob.Models/Poco/V2/ProspectContactDto.cs` | `ProspectActivityDto.cs` |
| `ProspectContactDto` mapper | `TrashMob.Models/Extensions/V2/ProspectContactExtensions.cs` | Existing V2 extension mappers |
| `IProspectContactManager` | `TrashMob.Shared/Managers/Prospects/IProspectContactManager.cs` | `IProspectActivityManager.cs` |
| `ProspectContactManager` | `TrashMob.Shared/Managers/Prospects/ProspectContactManager.cs` | `ProspectActivityManager.cs` |
| `ProspectContactsController` | `TrashMob/Controllers/V2/ProspectContactsController.cs` | Existing admin prospect controller pattern (primary constructor, `SiteAdmin` policy) |
| Frontend service | `TrashMob/client-app/src/services/prospect-contacts.ts` | Existing prospect services |
| Contact modal component | `TrashMob/client-app/src/components/admin/prospects/ProspectContactForm.tsx` | Existing modal form pattern (Zod + React Hook Form) |
| Contact table component | `TrashMob/client-app/src/components/admin/prospects/ProspectContactList.tsx` | DataTable pattern |

### Data Model

**`ProspectContact`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `ProspectId` | Guid (FK) | Yes | Parent `CommunityProspect` |
| `Name` | string (200) | Yes | Person's name |
| `Title` | string (200) | No | Job title / role at the organization |
| `Email` | string (256) | No | Contact email |
| `Phone` | string (30) | No | Contact phone |
| `Role` | string (100) | No | Free-text categorization ("Decision-maker", "Gatekeeper", "Referral") |
| `ContactStatus` | int (enum) | Yes | `Active`, `WrongPerson`, `NoResponse`, `LeftOrganization`, `RightPerson` |
| `IsPrimary` | bool | Yes | Current best contact for this prospect (only one true per prospect) |
| `ReferredByContactId` | Guid? (FK self) | No | Person who pointed us at this contact |
| `Notes` | string (2000) | No | Free-form notes about this specific person |
| Audit fields | | | `CreatedDate`, `CreatedByUserId`, `LastUpdatedDate`, `LastUpdatedByUserId` |

**`ProspectActivity` (modified):**

Add nullable `ProspectContactId` (Guid?) FK to `ProspectContact`.

**`ProspectOutreachEmail` (modified):**

Add nullable `ProspectContactId` (Guid?) FK to `ProspectContact`.

### Migration Sketch

```csharp
// Up
migrationBuilder.CreateTable(
    name: "ProspectContact",
    columns: table => new {
        Id = table.Column<Guid>(nullable: false),
        ProspectId = table.Column<Guid>(nullable: false),
        Name = table.Column<string>(maxLength: 200, nullable: false),
        Title = table.Column<string>(maxLength: 200, nullable: true),
        Email = table.Column<string>(maxLength: 256, nullable: true),
        Phone = table.Column<string>(maxLength: 30, nullable: true),
        Role = table.Column<string>(maxLength: 100, nullable: true),
        ContactStatus = table.Column<int>(nullable: false, defaultValue: 0),
        IsPrimary = table.Column<bool>(nullable: false, defaultValue: false),
        ReferredByContactId = table.Column<Guid>(nullable: true),
        Notes = table.Column<string>(maxLength: 2000, nullable: true),
        // ... audit fields
    });

migrationBuilder.CreateIndex("IX_ProspectContact_ProspectId", "ProspectContact", "ProspectId");

// Backfill from existing CommunityProspect rows
migrationBuilder.Sql(@"
    INSERT INTO ProspectContact (Id, ProspectId, Name, Title, Email, Phone, ContactStatus, IsPrimary, CreatedDate, CreatedByUserId, LastUpdatedDate, LastUpdatedByUserId)
    SELECT NEWID(), Id,
           ISNULL(ContactName, '(unknown)'),
           ContactTitle, ContactEmail, ContactPhone,
           0, -- Active
           1, -- IsPrimary
           CreatedDate, CreatedByUserId, LastUpdatedDate, LastUpdatedByUserId
    FROM CommunityProspect
    WHERE ContactName IS NOT NULL OR ContactEmail IS NOT NULL OR ContactPhone IS NOT NULL;
");

// Add nullable FK columns to existing tables
migrationBuilder.AddColumn<Guid>("ProspectContactId", "ProspectActivity", nullable: true);
migrationBuilder.AddColumn<Guid>("ProspectContactId", "ProspectOutreachEmail", nullable: true);

// Drop legacy columns from CommunityProspect (after backfill confirmed)
migrationBuilder.DropColumn("ContactName", "CommunityProspect");
migrationBuilder.DropColumn("ContactEmail", "CommunityProspect");
migrationBuilder.DropColumn("ContactTitle", "CommunityProspect");
migrationBuilder.DropColumn("ContactPhone", "CommunityProspect");
```

### API Endpoints

```text
GET    /api/v2/prospects/{prospectId}/contacts
GET    /api/v2/prospects/{prospectId}/contacts/{contactId}
POST   /api/v2/prospects/{prospectId}/contacts
PUT    /api/v2/prospects/{prospectId}/contacts/{contactId}
DELETE /api/v2/prospects/{prospectId}/contacts/{contactId}
POST   /api/v2/prospects/{prospectId}/contacts/{contactId}/set-primary
```

All endpoints require `[Authorize(Policy = AuthorizationPolicyConstants.SiteAdmin)]`.

The existing activity-logging and outreach-send endpoints get an optional `prospectContactId` field in their request body.

### Web UX Changes

- Prospect detail page (`/siteadmin/prospects/:id`): new **Contacts** section above the activity timeline
- Activity timeline entries show "→ Contact: [Name]" badge when an activity is contact-scoped
- "Log Activity" and "Send Outreach" modals get a contact dropdown (defaults to primary, "—" option for prospect-level activity)
- Prospect list page (`/siteadmin/prospects`): replace the single contact name column with "Primary Contact" + "Attempts" count

### Mobile App Changes

None.

---

## Implementation Phases

### Phase 1: Backend data model & migration
1. Create `ProspectContact` POCO + V2 DTO + mapper
2. Add nullable `ProspectContactId` to `ProspectActivity` and `ProspectOutreachEmail`
3. Configure `MobDbContext` (DbSet, FKs, indexes)
4. Write migration with backfill + column drop
5. Test migration on a restored prod backup (count rows pre/post)
6. Update unit tests covering the modified entities

### Phase 2: Backend manager & API
1. Implement `IProspectContactManager` / `ProspectContactManager` (`SetPrimaryAsync` is atomic — wraps in a transaction)
2. Register in `ServiceBuilder.cs`
3. Add `ProspectContactsController` (v2, primary constructor, `SiteAdmin` policy)
4. Extend activity / outreach endpoints to accept `prospectContactId`
5. Update `ProspectOutreachManager` to target the contact's email with primary-contact fallback
6. xUnit tests for manager + controller

### Phase 3: Frontend admin UI
1. Add React Query service `prospect-contacts.ts`
2. Build `ProspectContactList` + `ProspectContactForm` components
3. Wire Contacts section into the prospect detail page
4. Add contact dropdown to Log Activity + Send Outreach modals
5. Update activity timeline rendering to show contact badge
6. Update prospect list page columns

### Phase 4: Reporting & polish
1. Add per-user touchpoint tally to admin prospect dashboard
2. Add "Days in pipeline" + "Attempts" columns to prospect list
3. Verify AI outreach generation behaves correctly with null/missing primary contact
4. Manual QA: end-to-end flow on a dev prospect with multiple contacts
5. Update Swagger XML docs

**Note:** Phases are sequential. Phase 1 ships and bakes for a week on dev before Phase 2 to confirm migration didn't lose data.

---

## Open Questions

1. **Should `ContactStatus` be a free-text field or a strict enum?**
   **Recommendation:** Strict enum (`Active`, `WrongPerson`, `NoResponse`, `LeftOrganization`, `RightPerson`). Keeps reporting clean. Add a `Notes` field for nuance.
   **Owner:** Engineering
   **Due:** Phase 1

2. **What happens to outreach already in-flight when a contact is marked `LeftOrganization`?**
   **Recommendation:** Nothing automatic — already-sent emails stay logged against that contact. Future cadence steps don't re-target a non-Active contact (the outreach engine skips them).
   **Owner:** Engineering
   **Due:** Phase 2

3. **Should we expose contact data through the v2 admin prospect endpoints by default, or only when explicitly requested?**
   **Recommendation:** Include a `contacts` array on the prospect detail endpoint (already a heavy response); keep the list endpoint lightweight (only `primaryContact` summary).
   **Owner:** Engineering
   **Due:** Phase 2

4. **Should "delete contact" be soft or hard?**
   **Recommendation:** Hard delete if no `ProspectActivity` or `ProspectOutreachEmail` references the contact; otherwise block and surface "this contact has history — mark as Inactive instead." Avoids orphan FK references.
   **Owner:** Engineering
   **Due:** Phase 2

5. **Should the per-user touchpoint tally be exposed to all admins or scoped per-user?**
   **Recommendation:** Visible to all SiteAdmins. Small team, full transparency is the right default for a small nonprofit. Revisit if/when the team grows.
   **Owner:** Product
   **Due:** Phase 4

---

## Related Documents

- **[Project 40 - AI Community Sales Agent](./Archive/Project_40_AI_Community_Sales_Agent.md)** — The pipeline this project extends
- **[Project 54 - Community Adoption Outreach](./Project_54_Community_Adoption_Outreach.md)** — Could adopt the same multi-contact pattern in a follow-up
- **[Project 51 - Contact Management System](./Archive/Project_51_Contact_Management.md)** — Separate nonprofit-donor CRM (different domain; not to be confused with prospect contacts)

---

**Last Updated:** May 20, 2026
**Owner:** Product & Engineering
**Status:** Not Started
**Next Review:** When Project 54 enters development or a salesperson hire is on the calendar — whichever comes first
