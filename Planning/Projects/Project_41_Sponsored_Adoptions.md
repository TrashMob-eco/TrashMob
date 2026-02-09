# Project 41 — Sponsored Adoptions & Professional Cleanup Tracking

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phases 1-4 Complete) |
| **Priority** | Medium |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | Project 11 (Adopt-A-Location), Project 39 (Regional Communities) |

---

## Business Rationale

Many adoption programs (Adopt-a-Highway, Adopt-a-Road, etc.) allow businesses or individuals to **sponsor** a segment by paying a fee. The sponsor's name goes on the sign, but the actual cleanup work is performed by a **professional third-party company** hired by the community or the sponsor — not by volunteers. Currently, TrashMob only models volunteer-based adoption where the adopting team does the cleanup work themselves.

Communities (especially counties and states) need TrashMob to track both models side-by-side:
- **Volunteer adoptions** — teams adopt an area and do the cleanup themselves (existing model)
- **Sponsored adoptions** — a sponsor pays for an area and a professional company handles the cleanup

The professional companies need to log their work (pickup dates, weight removed, photos) for compliance tracking, but this work should **not** count toward volunteer leaderboards, impact stats, or gamification — it's paid professional work, not volunteering.

**Example Scenarios:**
- A local car dealership sponsors a 2-mile highway segment; a professional litter service cleans it monthly
- A county's Adopt-a-Road program has 40% sponsor-funded segments and 60% volunteer segments
- A state DOT contracts with three cleanup companies and needs to track which segments each company services
- A sponsor wants to see reports showing their adopted segment is being maintained

---

## Objectives

### Primary Goals
- **Track sponsored adoptions** separately from volunteer adoptions
- **Professional company accounts** — companies can log cleanup activity on their assigned segments
- **Compliance tracking** — communities verify that professional companies are meeting cleanup schedules
- **Exclude from volunteer metrics** — sponsored/professional work does not appear on leaderboards, volunteer stats, or gamification
- **Sponsor visibility** — sponsors see reports on their adopted segments

### Secondary Goals
- Invoice/payment tracking for sponsor fees (or integration point for external billing)
- Sponsor recognition on community pages (logo, name on map)
- Professional company performance dashboards
- Contract management (company ↔ community agreements)

---

## Scope

### Phase 1 — Data Model & Roles ✅ (PR #2616)

- [x] Add `Sponsor` entity (name, contact, logo, linked to partner/community)
- [x] Add `ProfessionalCompany` entity (name, contact, linked to partner/community)
- [x] Add `ProfessionalCompanyUser` join entity (user ↔ company membership)
- [x] Add `SponsoredAdoption` entity (area, sponsor, company, frequency, status)
- [x] Add `ProfessionalCleanupLog` entity (date, segment, company, weight, bags, duration, notes)
- [x] EF Core migration with proper indexes and relationships
- [x] Managers: Sponsor, ProfessionalCompany, ProfessionalCompanyUser, SponsoredAdoption, ProfessionalCleanupLog
- [x] Controllers: CommunitySponsors, CommunityProfessionalCompanies, CommunitySponsoredAdoptions, ProfessionalCleanupLogs, SponsorReports
- [x] Authorization policies: `UserIsProfessionalCompanyUserOrIsAdmin`, `UserIsPartnerUserOrIsAdmin`
- [x] Professional cleanup logs stored in separate table — excluded from volunteer stats by design

### Phase 2 — Community Admin Tools ✅ (PR #2618)

- [x] Community admins manage sponsors (CRUD) via partner dashboard
- [x] Community admins manage professional companies (CRUD) with user assignment
- [x] Create/edit sponsored adoptions: assign area, sponsor, company, frequency
- [x] Compliance dashboard: on-schedule vs overdue stats, tabbed adoption views
- [x] View professional cleanup logs per adoption
- [x] CSV export of adoption data for signage updates

### Phase 3 — Professional Company Portal ✅ (PR #2621)

- [x] `GET /api/professional-companies/mine` — users discover their companies
- [x] Company dashboard at `/companydashboard/:companyId` with sidebar nav
- [x] View assigned segments with schedule status (on-schedule/overdue badges)
- [x] Log a cleanup: mobile-friendly form with large touch targets
- [x] View cleanup history with summary stats (total cleanups, bags, weight)
- [x] MyDashboard integration: "My Professional Companies" section

### Phase 4 — Sponsor Portal ✅ (PR #2629)

- [x] `GET /api/sponsors/mine` — users discover sponsors via partner admin chain
- [x] `GET /api/sponsors/{id}/cleanup-logs` — aggregated logs across all adoptions
- [x] Sponsor dashboard at `/sponsordashboard/:sponsorId` with sidebar nav
- [x] View adopted segments with compliance status (on-schedule/overdue badges)
- [x] Cleanup history with summary stats
- [x] Download cleanup logs as CSV
- [x] MyDashboard integration: "My Sponsors" section

### Phase 5 — Reporting & Analytics (Future)

- [ ] Community-level report: volunteer vs sponsored adoption breakdown
- [ ] Cost-per-mile or cost-per-segment analytics for budgeting
- [ ] Professional company comparison metrics (for communities with multiple contractors)
- [ ] Annual summary reports for government compliance

---

## Out-of-Scope

- Payment processing (sponsors pay communities directly; TrashMob doesn't handle money)
- Bidding/procurement workflows for professional company contracts
- GPS tracking of professional cleanup crews
- Integration with specific state DOT systems
- Volunteer-to-sponsor conversion workflows

---

## Key Design Decisions

### Leaderboard & Stats Exclusion

Professional cleanup data is tracked in a separate `ProfessionalCleanupLog` table, not through the existing `EventSummary` pipeline. This provides a clean separation:

| Data Source | Counts Toward Leaderboards | Visible in Stats | Visible in Community Reports |
|---|---|---|---|
| Volunteer events/summaries | Yes | Yes | Yes |
| Professional cleanup logs | **No** | **No** (separate "Sponsored" section) | Yes (labeled as sponsored) |

### Account Types

| Role | Can Log Cleanups | Can View Reports | Can Manage Adoptions | Appears on Leaderboard |
|---|---|---|---|---|
| Volunteer/Team | Via events | Own stats | Apply for adoption | Yes |
| Professional Company User | Via cleanup log | Own assigned segments | No | **No** |
| Sponsor Viewer | No | Own sponsored segments | No | **No** |
| Community Admin | No (manages) | All segments | Full control | N/A |

---

## Success Metrics

### Quantitative
- **Sponsored adoptions tracked:** >= 50 segments in first year
- **Professional companies onboarded:** >= 10 companies
- **Compliance logs submitted:** >= 200 cleanup logs in first 6 months
- **Sponsor satisfaction:** >= 80% find reports useful

### Qualitative
- Communities can manage mixed volunteer/sponsor programs in a single platform
- Professional companies find data entry quick and field-friendly
- No pollution of volunteer stats with professional work
- Sponsors feel recognized and informed

---

## Dependencies

### Blockers
- **Project 11 (Adopt-A-Location):** Adoptable area infrastructure (complete)
- **Project 39 (Regional Communities):** County/state communities that run adoption programs

### Enables
- Government adoption program partnerships (county/state DOTs)
- Revenue model for community subscriptions (sponsor management as premium feature)
- Complete adoption program coverage (volunteer + sponsored)

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Data entry burden on companies** | High | Medium | Mobile-optimized quick-log form; minimal required fields |
| **Sponsors expect payment integration** | Medium | Low | Clear messaging that TrashMob tracks adoption, not payments; provide invoice reference field |
| **Professional data leaking into volunteer stats** | Low | High | Separate data tables; query-level filtering; automated tests to verify exclusion |
| **Complex role permissions** | Medium | Medium | Start with simple roles; use existing auth patterns from Partner admin |
| **Low adoption by professional companies** | Medium | Medium | Make data entry as frictionless as possible; offer email-based log submission |

---

## Implementation Plan

### Data Model

```
Sponsor
├── Id (Guid)
├── Name
├── ContactEmail
├── ContactPhone
├── LogoUrl
├── CommunityId (FK → Community)
├── CreatedDate, LastUpdatedDate

ProfessionalCompany
├── Id (Guid)
├── Name
├── ContactEmail
├── ContactPhone
├── CommunityId (FK → Community)
├── CreatedDate, LastUpdatedDate

ProfessionalCompanyUser
├── UserId (FK → User)
├── ProfessionalCompanyId (FK → ProfessionalCompany)

SponsoredAdoption
├── Id (Guid)
├── AdoptableAreaId (FK → AdoptableArea)
├── SponsorId (FK → Sponsor)
├── ProfessionalCompanyId (FK → ProfessionalCompany)
├── StartDate, EndDate
├── CleanupFrequencyDays
├── Status (Active, Expired, Terminated)

ProfessionalCleanupLog
├── Id (Guid)
├── SponsoredAdoptionId (FK → SponsoredAdoption)
├── ProfessionalCompanyId (FK → ProfessionalCompany)
├── CleanupDate
├── DurationMinutes
├── BagsCollected
├── WeightInPounds
├── WeightInKilograms
├── Notes
├── CreatedByUserId
├── CreatedDate
```

### API Endpoints

```
# Community admin
POST   /api/communities/{id}/sponsors
POST   /api/communities/{id}/professional-companies
POST   /api/communities/{id}/sponsored-adoptions
GET    /api/communities/{id}/sponsored-adoptions/compliance

# Professional company
GET    /api/professional-companies/{id}/assignments
POST   /api/professional-companies/{id}/cleanup-logs
GET    /api/professional-companies/{id}/cleanup-logs

# Sponsor
GET    /api/sponsors/{id}/adoptions
GET    /api/sponsors/{id}/adoptions/{adoptionId}/reports
```

---

## Open Questions

1. **Should professional companies be able to self-register, or must communities invite them?**
   **Recommendation:** Community invitation only (maintains trust and control)
   **Owner:** Product
   **Due:** Before Phase 2

2. **How do we handle a company that services segments across multiple communities?**
   **Recommendation:** Company entity is per-community; a real-world company creates separate accounts per community they work with. Revisit if cross-community companies become common.
   **Owner:** Engineering
   **Due:** Phase 1

3. **Should sponsor names be visible on the public community map?**
   **Recommendation:** Community-controlled toggle per sponsored adoption. Default: visible.
   **Owner:** Product
   **Due:** Phase 4

4. **Do we need a separate mobile app experience for professional companies?**
   **Recommendation:** No. Use the web app with a mobile-optimized cleanup log form. Evaluate native app support based on demand.
   **Owner:** Product
   **Due:** After Phase 3 launch

---

## Related Documents

- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** — Volunteer adoption infrastructure
- **[Project 39 - Regional Communities](./Project_39_Regional_Communities.md)** — County/state community pages
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** — Base community feature
- **[Project 20 - Gamification](./Project_20_Gamification.md)** — Leaderboard system (must exclude professional data)

---

**Last Updated:** February 9, 2026
**Owner:** Product Team
**Status:** In Progress (Phases 1-4 Complete, Phase 5 Future)
**Next Review:** When real adoption data is available for analytics
