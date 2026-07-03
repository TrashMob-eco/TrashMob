# Project 62 — TrashMob Site and Codebase Re-evaluation

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | _None_ |

> **Document type:** Principal-engineer-style codebase health check. Answers "what's the engineering state, and what should we work on next?" This document is meant to be updated in place every 6 months as a living quarterly-ish review, not a one-off strategic memo.

---

## Business Rationale

It's been months since the last top-down engineering review. The codebase has grown (92 v2 controllers, 31 active projects, 35 archived, 260+ EF Core migrations). Multiple structural shifts are landing simultaneously: a 1099 sales contractor is starting (see [Project 63](./Project_63_Municipal_Sales_Pipeline_Reporting.md)), and the Renovate auto-merge cadence has surfaced two production-impacting version-skew incidents in the last two weeks — SQLitePCLRaw `CVE-2025-6965`, and the MAUI 10.0.20 vs 10.0.70 `MissingMethodException` that caused a Sentry alert 17 minutes after a merge. Time for a fresh, honest look at where we are.

The goal isn't to find problems to manufacture work — the audit confirms the codebase is healthy. The goal is to make sure Joe's engineering bandwidth (the binding constraint on this entire organization) goes to the work with the highest leverage in light of what's actually next: the first real paid municipal contracts.

---

## Snapshot: Codebase Health

Findings from a fresh principal-engineer audit. Use this as the ground truth for any "where are we?" question for the next 90 days.

### Scale & velocity

| Dimension | Value | Notes |
|-----------|-------|-------|
| Active projects | 31 | 13 in-progress simultaneously |
| Archived projects | 35 | Delivered, in production |
| v2 controllers | 92 | ~19.5K LOC total, average 212 LOC each — well-distributed, no god-controllers |
| EF Core migrations | 260+ | Healthy schema evolution; no rollback debt detected |
| `DbContext` entities | 150+ DbSet properties | Broad but manageable |
| Playwright E2E tests | 598 | Run against deployed dev environment |
| Backend xUnit tests | 1,115+ | Healthy ratio |
| 90-day commit count | ~215 | Joe ~58%, Renovate ~42% |
| 90-day commits per day | ~2.4 | Healthy cadence |
| Open PRs (typical) | 4–10 | Mostly Renovate; low WIP |

### Health ratings

| Dimension | Rating | Comment |
|-----------|--------|---------|
| Architecture | A- | Clean Manager/Repository/Controller separation; DI consistent; patterns documented in `CLAUDE.md`. Mobile MVVM solid. v2 API well-distributed. |
| Test coverage | B+ | 598 E2E + 1,115+ unit tests. Some `[Skip]` debt. Mobile nullable issues create future build-warning friction. |
| Dependency management | A | Renovate cadence high; version skew now actively tracked (post-#3435 `MauiVersion` regex + follow-ups); SQLitePCLRaw 3.50.3 override for `CVE-2025-6965`; no CVE backlog. |
| Documentation | A | Root + per-area `CLAUDE.md` files excellent. Planning portfolio comprehensive. Code self-documenting via enforced patterns. |
| Velocity | B+ | 2.4 commits/day. 13 concurrent in-progress projects is a yellow flag — suggests capacity pressure or insufficient prioritization. |
| Deployment readiness | B | CI/CD partial. Project 5 Phases 4–6 deferred (health dashboards, auto-scaling, security scanning). Docker transition underway. |
| Production risk | B+ | Known risks (auth migration, mobile testing, COPPA legal — the last of which is now closed via Project 23) all tracked with mitigation. No silent landmines. |

**Maturity level:** early-to-mid-stage growth phase. The codebase has graduated from prototype to production-grade. Debt is strategic (intentional deferment with documented rationale) rather than rot.

### What this means

The instinct after a re-evaluation is to find big problems to attack. **There aren't any.** The platform is in good engineering shape. The real questions are about *prioritization* (too much WIP, not enough finishing) and *readiness for the first paid municipal contracts* (which features would be embarrassing gaps in a city procurement conversation), not about firefighting unknown technical debt.

---

## Where the Real Constraints Are

### 1. Volunteer/founder capacity is the binding constraint

Joe is the only senior engineer doing ~58% of all commits. Volunteers contribute via individual projects but turnover is constant. With 13 projects in-progress and 5+ not started, the organization is taking on more new work than it can reliably finish. This is the single biggest risk to *anything* on the roadmap.

**Recommendation:** Cap in-progress at 6 projects, with a hard requirement that one ships before another starts. Defer or close any in-progress project that hasn't seen a commit in 30 days.

### 2. CI/CD and mobile-test gaps slow every feature

Two specific blockers compound across the portfolio:

- **MAUI + Appium emulator instability in GitHub Actions** ([Project 25 Phase 4](./Project_25_Automated_Testing.md)). Tests exist but don't gate CI. Every mobile feature ships partially blind.
- **Manual device testing matrix for Mobile Robustness** ([Project 4 Phases 3–5](./Project_04_Mobile_Robustness.md)). Without this, app-store readiness is delayed and crash-rate risk is unmanaged.

Both are *infrastructure* work, not feature work. They unblock every downstream mobile project.

**Recommendation:** Treat these as P0 for Q3 2026. They're prerequisites for everything mobile.

### 3. Auth migration tail risk (mostly resolved)

Project 1 (Azure B2C → Entra External ID) is mostly done. Phases 0–5a, 7 are complete and [Project 23 (Parental Consent / PRIVO)](./Project_23_Parental_Consent.md) went live in production on 2026-05-20 with Flow 3 E2E verification finishing 2026-07-01. **Phase 6 (mobile app update — MSAL config, profile-photo handling, coordinated app-store submission) remains partial.**

**Recommendation:** Finish Phase 6 by end of Q3 2026. It's a known-scope cleanup task. Nothing new is gated on it, but every day it stays partial is a day the mobile app sign-in flow diverges from web.

### 4. Strapi CMS dormancy

[Project 16 (Strapi)](./Archive/Project_16_Strapi_Setup.md) is "complete" but the CMS is lightly used. Its main consumer is the News & Blog feature (Project 50). The infrastructure exists but isn't paying for its complexity.

**Recommendation:** Either commit to deeper Strapi adoption (case studies, partner content, community pages content — would matter for the sales conversation with cities) or sunset it and replace with markdown-in-repo + build-time rendering. Don't leave it in limbo.

### 5. WIP-overload symptom: "deferred phases" accumulating

Several projects have explicitly deferred phases (Project 24 Phase 4 ETags/webhooks, Project 48 Phase 2b density heatmap, Project 51 deadline calendar, Project 57 QR codes). These are *zombies* — neither shipping nor closed. They take planning attention but produce no value.

**Recommendation:** Quarterly "defer-or-cut" pass. If a deferred phase isn't going to ship in the next 6 months, mark the parent project Complete and move the phase to a new tracking list ("Backlog candidates"). Cleanup is a 1-hour exercise; the clarity benefit is large.

### 6. Authorization surface is single-tier

The current `IsSiteAdmin` boolean is a binary — either you have everything or nothing. That was fine for the last five years but breaks the moment the 1099 sales contractor needs an account (see [Project 64](./Project_64_Roles_and_RBAC_Refactor.md)). The refactor is fundamentally straightforward but touches 234 files, so it's real work.

**Recommendation:** Coordinate Project 64 Phase 1 + 2 to land before the salesperson gets a login. If schedule slips, grant them explicit time-boxed `SiteAdmin` and revoke on Project 64 Phase 2 merge.

---

## Top 5 Highest-Leverage Engineering Work

The five items that carry the highest engineering ROI over the next 90 days:

1. **Finish Mobile Robustness ([Project 4](./Project_04_Mobile_Robustness.md) Phases 3–5)** — manual device testing, accessibility audit (TalkBack/VoiceOver), regression suite. Unblocks app-store launch readiness, closes Project 38 parity gaps, reduces crash-rate risk.

2. **Unblock E2E test CI ([Project 25](./Project_25_Automated_Testing.md) Phase 4)** — solve MAUI + Appium emulator stability in GitHub Actions. Alternatives to investigate: Browserstack, AWS Device Farm, self-hosted Mac runner. Unblocks safe CI/CD gating for every mobile change.

3. **Mobile nullable reference types cleanup** (currently deferred inside `TrashMobMobile.csproj`) — resolve CS8618/CS8602 warnings so `TreatWarningsAsErrors=true` can be re-enabled. Mechanical refactor, Claude-assistable.

4. **Auth migration finish ([Project 1](./Project_01_Auth_Revamp.md) Phase 6)** — close out Entra External ID edge cases in the mobile app, finalize canary + rollback runbooks, coordinate app-store push. Removes the last piece of the multi-year auth modernization.

5. **Quarterly "deferred phase" defer-or-cut pass** — go through the active project list, find every deferred phase, and either schedule it within 6 months or move it to a separate backlog file. Clears planning noise; prevents zombie projects.

---

## What "City-Ready" Would Actually Require

The 1099 sales contractor is engaging cities. When one signs, engineering has to be ready. This section catalogues the features that would be *embarrassing gaps* in a real municipal contract conversation, in rough order of pain-if-missing.

- **Multi-tenant *logical* separation** — cities want their event / volunteer / incident data partitioned, even if it's all in the same physical database. We don't need full multi-tenancy infrastructure; we need a `TenantId` (or `CommunityId` extension) on the right entities and consistent filtering. Foundation for everything else.
- **Waiver workflow polish** ([Project 8](./Project_08_Waivers_V3.md)) — currently legal-pending; needs to land for any city sale.
- **FOIA-friendly export** — event-scoped record sets (documents, activity logs, attendance histories, photos, incident reports) as a single downloadable bundle. [Project 24 Phase 4 (ETags & bulk export)](./Archive/Project_24_API_v2_Modernization.md) has the plumbing. UX + a filter-driven "give me everything for these events" flow layers on top.
- **District / zone reporting in admin** — cities ask first for geographic rollups; we have the lat/long data, we don't have a UI for it. Small project, big perception.
- **Public status page** — cities in procurement want observable uptime. We instrument via App Insights and Sentry, we don't surface it publicly.
- **Security scanning in CI** — OWASP ZAP, CodeQL, Trivy on every build. Cheap to add, expected by enterprise buyers. Lives in [Project 5 Phase 6](./Project_05_Deployment_Pipelines.md).
- **SOC 2 Type II initiation** — $30–80K, 6 months. Minimum credibility bar for city procurement > $10K ACV. Its own project when the first paid contract is imminent.

The point isn't to pre-build all of these — it's to know that when the salesperson brings back a "yes if you can also do X" from a city, the answer is likely in this list and roughly costed. Don't try to build them speculatively; wait for the deal.

---

## Scope

### Phase 1 — Codebase audit + this document

- ☐ Engineering audit complete (see Snapshot section above)
- ☐ Board reads this document at the next scheduled meeting

### Phase 2 — Q3 2026 engineering rebalancing

- ☐ Cap in-progress projects at 6 (close 7 zombies or move to backlog)
- ☐ Complete Mobile Robustness Phases 3–5
- ☐ Unblock E2E test CI (Project 25 Phase 4)
- ☐ Mobile nullable cleanup; re-enable `TreatWarningsAsErrors` in TrashMobMobile.csproj
- ☐ Strapi commit-or-sunset decision

### Phase 3 — Sales-motion-conditional engineering

Not started speculatively. Triggered when the sales contractor's pipeline reaches a specific milestone (first LOI, first paid pilot, first signed contract — TBD in Project 63):

- ☐ Open new project for SOC 2 Type II preparation
- ☐ Open new project for multi-tenant logical isolation
- ☐ Prioritize Project 8 (Waivers) for legal completion
- ☐ Open new project for event-scoped FOIA-friendly export
- ☐ Open new project for district/zone reporting in admin

### Phase 4 — Recurring cadence

- ☐ Repeat this audit every 6 months, edited in place — this document is the log
- ☐ Quarterly defer-or-cut pass on deferred phases

---

## Out-of-Scope

- **Large refactors of the existing v2 controllers, managers, or DbContext.** No architectural rot detected; refactor work is not justified by the audit.
- **Migrating off Strapi.** That's a decision to be made (commit or sunset), not a refactor to start.
- **Migrating off any current vendor** (Azure SQL, App Insights, SendGrid, Azure Maps for backend, Google Maps for frontend Android). All are working; replacing them is not on the leverage list.
- **Adding new domain features that don't ship in Phase 2 or Phase 3.** WIP discipline matters more than feature count.

---

## Success Metrics

### Quantitative

- **In-progress project count:** Drop from 13 to ≤ 6 by end of Q3 2026.
- **Mobile crash-free sessions:** ≥ 99.5% (Sentry) once Project 4 Phases 3–5 ship.
- **E2E CI green rate:** ≥ 95% over a rolling 30-day window after Project 25 Phase 4 unblock.
- **Build warnings in TrashMobMobile:** zero (currently many CS8618/CS8602 deferred under `TreatWarningsAsErrors=false`).
- **Days from PR open to merge** for low-risk Renovate updates: median ≤ 1 (currently good; track to detect regressions).

### Qualitative

- Engineer (Joe) reports feeling "less pulled in 13 directions" — measurable via gut check at each half-yearly audit.
- No surprise version-skew or transitive-dependency incidents in 6 months (post-#3435 customManager, post-SQLitePCLRaw, and post-follow-up-hardening).
- New volunteer engineers (if any join) can onboard from `CLAUDE.md` files alone — re-tested via at least one volunteer onboarding.
- Salesperson never comes back with a "city asked for X and we can't demo it in the current dev environment" that isn't already on the "What City-Ready Would Actually Require" list.

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Cutting in-progress projects causes morale drop with volunteer contributors who picked them up** | Medium | Medium | Frame as "completing work we've committed to before starting new things" — not "abandoning your work." Communicate via individual conversations, not bulk announcement. |
| **Mobile testing infra investment (Browserstack / AWS Device Farm) costs $200–500/mo recurring** | High | Low | Within budget; cheap compared to crash-rate risk. Approve in Q3 2026. |
| **Sales-motion-conditional engineering (Phase 3) gets pulled forward speculatively and cannibalizes nonprofit roadmap** | Medium | Medium | Hard rule: Phase 3 items don't start until Project 63 hits an explicit milestone. Track engineering-hours-on-city-features as a separate line in the quarterly review. |
| **Renovate auto-merge keeps surfacing skew incidents we don't anticipate** | Medium | Medium | The #3435 `MauiVersion` customManager is a template — apply to other "implicit version" properties proactively when discovered. Recent skew from `Microsoft.Maui.Controls.Compatibility` bumping independently is a case in point: the customManager needs a wider anchor set. |
| **A volunteer engineer hits a deferred phase on a "complete" project, gets confused** | Low | Low | The defer-or-cut pass creates a separate `Backlog_Candidates.md` list. Cross-reference from each affected project's footer. |

---

## Open Questions

1. **Cap in-progress projects at 6 — or different number?**
   **Recommendation:** 6. Heuristic from team size of 1 senior + ~3 active volunteers at any time. Trial for Q3; adjust if obviously wrong.
   **Owner:** Joe
   **Due:** 2026-07-15

2. **Strapi: commit or sunset?**
   **Recommendation:** Commit — case studies and community-content marketing matter for the municipal sales conversation. But acknowledge the CMS hasn't earned its complexity yet and the commit means a concrete content pipeline, not just "keep it running."
   **Owner:** Joe + Cynthia
   **Due:** 2026-09-30

3. **Do we hire a second engineer, and when?**
   **Recommendation:** Not yet. Capacity discipline (cap at 6 in-progress) is the lever, not headcount. Revisit if Project 63 lands the first 5 paying municipal contracts and the sales-motion Phase 3 backlog crosses one engineer's capacity.
   **Owner:** Board
   **Due:** Defer to milestone

4. **Should the codebase audit become a recurring quarterly artifact, or only on demand?**
   **Recommendation:** Every 6 months — quarterly is too often given the codebase's stability, on-demand misses slow-rotting issues. Edit this document in place rather than creating new ones.
   **Owner:** Joe
   **Due:** 2027-01-03 (next audit cadence)

5. **What's the trigger to open the Phase 3 (sales-motion-conditional) engineering projects?**
   **Recommendation:** First signed paid contract, not first LOI. LOIs are cheap to give; a signed contract with revenue attached is the signal that engineering investment is justified.
   **Owner:** Joe + Cynthia
   **Due:** As part of Project 63 milestone planning

---

## Related Documents

- **[Project 63 - Municipal Sales Pipeline Reporting](./Project_63_Municipal_Sales_Pipeline_Reporting.md)** — the sales motion whose demands drive Phase 3 engineering priorities
- **[Project 64 - Roles & RBAC Refactor](./Project_64_Roles_and_RBAC_Refactor.md)** — a real engineering item that surfaced from this audit; unblocks Project 63
- **[Planning/README.md](../README.md)** — will need quarter section updates after this re-evaluation lands
- **[Planning/Executive_Summary.md](../Executive_Summary.md)** — roadmap section to reflect WIP cap + sales-motion conditional Phase 3

---

**Last Updated:** 2026-07-03
**Owner:** Joe (engineering)
**Status:** Planning
**Next Review:** 2027-01-03 (half-yearly) or when Project 63 hits Phase 3 trigger — whichever first
