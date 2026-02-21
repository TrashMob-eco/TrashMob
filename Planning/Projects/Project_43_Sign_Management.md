# Project 43 — Adopt-A-Location Sign Management

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Low |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 11 (Adopt-A-Location), Project 41 (Sponsored Adoptions) |

---

## Business Rationale

Communities running Adopt-A-Location programs place physical signs at adopted areas (e.g., "This area maintained by [Team Name]"). These signs are a key part of the adoption program — they provide public recognition, deter littering, and show community investment. Currently there is no way to track sign inventory: what each sign says, where it is, when it was ordered, installed, or removed. Community admins manage this manually via spreadsheets or not at all, leading to lost signs, delayed installations, and no visibility into sign lifecycle.

This project adds sign management to the existing adoption infrastructure so community admins can track the full lifecycle of every sign from order to removal.

---

## Objectives

### Primary Goals
- Track individual signs with geo coordinates, text content, and lifecycle dates (ordered, installed, removed)
- Support signs for both volunteer (TeamAdoption) and sponsored (SponsoredAdoption) adoptions
- Allow communities to define a default sign text template with per-sign override capability
- Provide a sign inventory dashboard for community admins

### Secondary Goals (Nice-to-Have)
- Display sign locations on the community adoption map
- Bulk sign ordering workflow
- Export sign inventory to CSV
- Overdue installation alerts

---

## Scope

### Phase 1 — Data Model & Admin CRUD
- [ ] `Sign` entity with geo coordinates, text, status, and lifecycle dates
- [ ] `DefaultSignTemplate` field on Partner model for community-wide sign template
- [ ] EF Core migration for new table and Partner column
- [ ] Signs controller (CRUD, scoped to community)
- [ ] Partner admin UI: list, create, edit, delete signs
- [ ] Link signs to TeamAdoption or SponsoredAdoption

### Phase 2 — Sign Lifecycle Tracking
- [ ] Status workflow enforcement (Ordered → Installed → Removed)
- [ ] Bulk sign creation (create multiple signs for an adoption at once)
- [ ] Sign inventory dashboard per community (counts by status)
- [ ] Export sign list to CSV

### Phase 3 — Map & Reporting
- [ ] Display sign locations on community adoption map (pin per sign with status color)
- [ ] Sign status summary report (ordered/installed/removed counts, trends)
- [ ] Overdue installation alerts (ordered but not installed after configurable threshold)

---

## Out-of-Scope

- [ ] Sign design/artwork tools — communities design signs externally
- [ ] Ordering integration with print vendors — this tracks the order, not the procurement
- [ ] Photo upload of installed signs — could be added later, but not in initial scope
- [ ] Mobile app sign management — web admin only for initial release
- [ ] QR code generation on signs — future enhancement

---

## Success Metrics

### Quantitative
- **Sign tracking adoption:** ≥ 3 communities actively tracking signs within 3 months of launch
- **Data completeness:** ≥ 80% of signs have both ordered and installed dates populated
- **Admin time savings:** Eliminate need for external spreadsheet tracking

### Qualitative
- Community admins can answer "how many signs do we have installed?" in under 10 seconds
- Clear visibility into which adoptions are missing signs

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Project 11 (Adopt-A-Location):** ✅ Complete — provides AdoptableArea, TeamAdoption, TeamAdoptionEvent entities
- **Project 41 (Sponsored Adoptions):** Required for SponsoredAdoption FK — Phase 1 can proceed with TeamAdoption-only signs; SponsoredAdoption support added once Project 41 ships

### Enablers for Other Projects
- **Project 41 (Sponsored Adoptions):** Sign tracking is valuable for sponsors who want visibility into sign placement at their funded locations

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Project 41 not complete when sign management starts** | Medium | Low | TeamAdoptionId is sufficient for Phase 1; SponsoredAdoptionId FK is nullable and added when Project 41 ships |
| **Low adoption by community admins** | Medium | Medium | Keep UI simple; auto-suggest sign text from template; show value via dashboard |
| **Geo coordinate accuracy** | Low | Low | Use map picker in UI for coordinate entry; allow manual lat/lng override |

---

## Implementation Plan

### Data Model Changes

#### New Table: `Signs`

| Column | Type | Constraints |
|--------|------|-------------|
| `Id` | `uniqueidentifier` | PK, default `newsequentialid()` |
| `TeamAdoptionId` | `uniqueidentifier` | FK → TeamAdoptions, nullable |
| `SponsoredAdoptionId` | `uniqueidentifier` | FK → SponsoredAdoptions, nullable |
| `Latitude` | `float` | Required |
| `Longitude` | `float` | Required |
| `SignText` | `nvarchar(2000)` | Nullable — overrides community template if set |
| `Status` | `nvarchar(20)` | Default `'Ordered'` — values: Ordered, Installed, Removed |
| `OrderedDate` | `datetimeoffset` | Nullable |
| `InstalledDate` | `datetimeoffset` | Nullable |
| `RemovedDate` | `datetimeoffset` | Nullable |
| `RemovalReason` | `nvarchar(500)` | Nullable |
| `Notes` | `nvarchar(2000)` | Nullable |
| `CreatedByUserId` | `uniqueidentifier` | FK → Users |
| `CreatedDate` | `datetimeoffset` | Audit |
| `LastUpdatedByUserId` | `uniqueidentifier` | FK → Users |
| `LastUpdatedDate` | `datetimeoffset` | Audit |

**Indexes:**
- `IX_Signs_TeamAdoptionId`
- `IX_Signs_SponsoredAdoptionId`
- `IX_Signs_Status`

#### Partner Table Addition

| Column | Type | Constraints |
|--------|------|-------------|
| `DefaultSignTemplate` | `nvarchar(2000)` | Nullable |

Template supports placeholders: `{TeamName}`, `{AreaName}`, `{AdoptionDate}`, `{CommunityName}`.

Example: `"This area proudly maintained by {TeamName} through the {CommunityName} Adopt-A-Location program. Adopted {AdoptionDate}."`

### API Changes

```
GET    /api/communities/{partnerId}/signs                           — List all signs for community
GET    /api/communities/{partnerId}/signs/{signId}                  — Get single sign
POST   /api/communities/{partnerId}/signs                           — Create sign
PUT    /api/communities/{partnerId}/signs/{signId}                  — Update sign
DELETE /api/communities/{partnerId}/signs/{signId}                  — Delete sign
GET    /api/communities/{partnerId}/adoptions/{adoptionId}/signs    — Signs for a specific adoption
PUT    /api/communities/{partnerId}/sign-template                   — Update community sign template
```

### Web UX Changes

- **Community Admin → Signs tab:** List view with status filters, link to adoption, map coordinates
- **Sign form:** Map picker for coordinates, text editor (pre-filled from template), status dropdown, date fields
- **Adoption detail page:** Show linked signs count and list
- **Sign template editor:** On community settings page, with placeholder guide

### Mobile App Changes
- None in initial scope — web admin only

---

## Implementation Phases

### Phase 1: Data Model & Admin CRUD
- Create `Sign` model in `TrashMob.Models/`
- Add `DefaultSignTemplate` to `Partner` model
- Configure in `MobDbContext` with indexes and FK relationships
- Generate EF migration
- Create `ISignManager` interface and `SignManager` implementation
- Create `ISignRepository` and `SignRepository`
- Register in `ServiceBuilder.cs`
- Create `SignsController` with CRUD endpoints
- Build frontend sign management pages

### Phase 2: Sign Lifecycle & Reporting
- Add status transition validation in manager
- Bulk creation endpoint
- Sign inventory dashboard component
- CSV export endpoint

### Phase 3: Map Integration
- Add sign markers to community adoption map
- Sign status summary report page
- Overdue installation alert logic (background job or dashboard warning)

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **Should signs be deletable, or only removable (soft delete)?**
   **Recommendation:** Allow hard delete for signs that were created in error; use "Removed" status for signs that were physically removed.
   **Owner:** Product
   **Due:** Before Phase 1 development

2. **Should we track sign dimensions or material type?**
   **Recommendation:** Not in initial scope — can add later if communities request it.
   **Owner:** Product
   **Due:** Phase 2 planning

3. **What happens to signs when an adoption ends (team leaves or sponsorship expires)?**
   **Recommendation:** Signs remain in the system with their last status; admin can bulk-update to "Removed" when adoption ends.
   **Owner:** Product
   **Due:** Before Phase 1 development

---

## Related Documents

- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** — Parent feature providing AdoptableArea and TeamAdoption entities
- **[Project 41 - Sponsored Adoptions](./Project_41_Sponsored_Adoptions.md)** — SponsoredAdoption entity for paid cleanup tracking
- **[Project 39 - Regional Communities](./Project_39_Regional_Communities.md)** — Community/Partner infrastructure

---

**Last Updated:** February 8, 2026
**Owner:** Product & Engineering Team
**Status:** Not Started
**Next Review:** When volunteer picks up work
