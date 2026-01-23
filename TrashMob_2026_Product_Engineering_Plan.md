# TrashMob.eco — 2026 Product & Engineering Plan

**Version:** 1.0  
**Date:** January 23, 2026  
**Owner:** Director of Product & Engineering

---

## Table of Contents

- [0) Document Guide](#0-document-guide)
- [1) Executive Summary](#1-executive-summary)
- [2) 2026 Strategic Objectives](#2-2026-strategic-objectives)
- [3) Success Metrics](#3-success-metrics-organization-level)
- [4) Product Pillars & Cross-Cutting Plans](#4-product-pillars--cross-cutting-plans)
- [5) 2026 Roadmap](#5-2026-roadmap-target-sequence)
- [6) Resourcing Plan](#6-resourcing-plan-2026)
- [7) Product Operations](#7-product-operations)
- [8) Project Catalog](#8-project-catalog-standardized-templates)
- [9) Risks & Mitigations](#9-risks--mitigations-across-all-projects)
- [10) Appendices](#10-appendices)

---

## 0) Document Guide

### How to use this plan

- **Section 1–4:** Strategy, growth levers, and cross-cutting plans
- **Section 5:** 2026 roadmap (Q1–Q4) and resourcing needs
- **Section 6:** Product operations (metrics, privacy, accessibility, observability)
- **Section 7:** Project Catalog (23 projects), each with a uniform template
- **Appendices:** Glossary, standard data definitions, and open questions

---

## 1) Executive Summary

### Purpose

Enable sustained growth of TrashMob.eco by strengthening onboarding & safety (minors and waivers), launching community & team features that cities will subscribe to, and raising product quality across mobile, web, and infrastructure.

### Key 2026 Feature Themes

- **Onboarding & Safety:** minors (13+), parental consent, SSO for partners, flexible waivers, smoother auth (B2C → Entra External ID)
- **Community & Teams:** branded community pages, adopt-a-location, team creation/management, leaderboards, content management
- **Impact Metrics:** event weights, attendee-level metrics, map route tracing and analytics
- **Platform Improvements:** complete litter reporting on web, advertising/sponsorship surfacing, newsletter support, in-app messaging (guardrails), AI access via MCP server, finish IFTTT
- **Technical & Ops:** mobile stabilization, .NET 10 upgrades, Docker hosting, cost & accessibility reviews, better error handling and observability

### Revenue Approach (2026)

**Primary:** Grants, paid community subscriptions (Community Pages, Adopt-A-* programs), and sponsorship/advertising surfaced tastefully on site. Billing/manual ops via QuickBooks for 2026.

### Risks to Manage

- Compliance & legal for minors and waivers
- Large auth migration
- Mobile stability debt
- Volunteer bandwidth/turnover
- Cost controls (Azure, Google Maps, SendGrid)

---

## 2) 2026 Strategic Objectives

| # | Objective | Description |
|---|-----------|-------------|
| 1 | **Trust & Safety** | Enable minors participation safely and legally; modernize auth/waivers |
| 2 | **City Value** | Deliver Community Pages and Adopt-A-Location as subscription-ready modules |
| 3 | **Volunteer Network Effects** | Launch Teams, make participation visible (metrics, photos, routes), and drive engagement (light gamification) |
| 4 | **Quality & Velocity** | Stabilize mobile, upgrade stacks, streamline CI/CD, and instrument end-to-end telemetry |
| 5 | **Monetization Foundations** | Introduce tasteful sponsorship on home page and simple manual billing |

---

## 3) Success Metrics (Organization-Level)

### Activation
- **New volunteer signups:** +X% increase
- **Signup drop-off:** <Y%

### Engagement
- **Monthly active volunteers:** +Z%
- **Events created per month:** +A%

### Impact
- **Total weight of litter:** Reported sitewide
- **Bags per event:** Average tracked
- **Events with complete summaries:** P% of all events

### City Adoption
- **Paid communities onboarded:** N communities
- **Renewal rate:** ≥ P%

### Quality
- **Crash-free sessions (mobile):** ≥ 99.5%
- **P95 API latency:** ≤ 300ms
- **Critical incidents/month:** ≤ 2

### Velocity
- **Lead time for change:** ↓ 30%
- **Deployment frequency:** ↑ 2×
- **Change failure rate:** ≤ 10%

> **Note:** Targets will be finalized with stakeholders in Q1 based on baseline telemetry and capacity planning.

---

## 4) Product Pillars & Cross-Cutting Plans

### 4.1 Trust, Privacy & Safety

#### Minors (13+)
- Parental consent & protections
- No personal details exposed to other attendees
- Minors cannot create events
- Ensure at least one adult at events

#### Waivers
- Flexible model for TrashMob default + community-specific waivers
- Validity windows
- Admin review
- Print/export capabilities
- Minors coverage

#### Compliance
- COPPA/legal review prior to development start

### 4.2 Accessibility

Commit to **WCAG 2.2 AA** on web and mobile with a full audit and remediation plan in 2026.

### 4.3 Observability & Reliability

- Front-to-back error handling
- Business event logging
- SLOs, alerting, and dashboards
- **Tooling:** Sentry.io for mobile + backend/APM

### 4.4 Cost & Infrastructure

- Docker-based hosting for web/function apps
- DB migrations via init container
- Evaluate Azure & Maps costs
- CI/CD remediation for GitHub pipelines, Apple/Google stores

---

## 5) 2026 Roadmap (Target Sequence)

> **Dependencies:** Auth migration & mobile stabilization unblock several downstream features.

### Q1 (Jan–Mar)
- **Project 4:** Mobile Robustness (launch readiness + error handling)
- **Project 5:** Pipelines & Infra (CI/CD, containerization plan)
- **Project 1:** Auth Revamp kickoff (Entra External ID design + migration plan)

### Q2 (Apr–Jun)
- **Project 1:** Auth Revamp (implementation)
- **Project 3:** Litter Reporting on Web (feature complete)
- **Project 7:** Event Weights (Phase 1)
- **Project 16:** Page Content Management (home/partner updates)

### Q3 (Jul–Sep)
- **Project 9:** Teams (MVP)
- **Project 10:** Community Pages (MVP)
- **Project 8:** Waivers V3 (MVP with legal gating)
- **Project 7:** Event Weights (Phase 2 – attendee entries)

### Q4 (Oct–Dec)
- **Project 11:** Adopt-A-Location (pilot communities)
- **Project 22:** Attendee-Level Metrics (consolidated with P7/P20)
- **Project 20:** Gamification (leaderboards)
- **Optional pilots:** Project 12 (Messaging, gated), Project 18 (Before/After photos), Project 17 (MCP server)

> **Note:** Specific launch dates will be finalized during quarterly planning and depend on volunteer capacity.

---

## 6) Resourcing Plan (2026)

### Roles & Counts (Target Ranges)

| Role | Count |
|------|-------|
| Mobile App Product Lead | 1 |
| Web Site Product Lead | 1 |
| UX Designers | 2–3 |
| .NET MAUI Mobile Devs | 2–3 |
| Web Devs | 2–3 |
| Build/Deployment Engineers | 1–2 |
| Security Engineers | 1–2 |

### Volunteer Dynamics

- Chunk work into **3–6 month deliverables**
- Ongoing recruiting to backfill attrition
- Handover documentation for continuity

---

## 7) Product Operations

### 7.1 Standard Data & Metrics Definitions

| Term | Definition |
|------|------------|
| **Event** | A scheduled cleanup instance with public or private visibility and defined owner(s) |
| **Bags** | Collected per event and (where enabled) per attendee; roll-up logic defined |
| **Weight** | Total weight collected, with units preference (lbs/kgs) |
| **Duration** | Time spent on cleanup activities |
| **Attendee Contribution** | Attendee-entered stats override "equal share" default; leads can reconcile |

### 7.2 Privacy, Retention & Governance

#### Minors
- Parental consent artifacts retained per legal guidance
- Extra visibility restrictions

#### Waivers
- Store version, effective/expiry, signatory, minor coverage
- Printable/exportable

#### Photos & UGC
- Moderation tools and TOS enforcement pipeline
- Admin removal + email notice

### 7.3 Accessibility Goals

Target **WCAG 2.2 AA** parity across web and mobile; perform full audit and remediation in 2026.

### 7.4 Observability & SLOs

#### SLOs
- Uptime, latency, error budgets per service
- End-user crash-free rate (mobile)

#### Tooling
- **Sentry** + dashboards
- Alerting for mobile, web, API, and jobs apps

---

## 8) Project Catalog (Standardized Templates)

> Each project follows a consistent format: **Status • Business Rationale • Objectives • Scope & Out-of-Scope • Success Metrics • Dependencies • Risks/Mitigations • Implementation Plan • Rollout • Open Questions**

---

### Project 1 — Revamp Sign-Up / Sign-In (Azure B2C → Entra External ID)

| Attribute | Value |
|-----------|-------|
| **Status** | Awaiting feedback on minors; age verification vendor discussions |
| **Priority** | High |
| **Risk** | Very Large |
| **Size** | Large |

#### Business Rationale

Modernize auth, enable minors (13+), improve brand and legal compliance, and future-proof identity.

#### Objectives

- Migrate to Entra External ID with parental consent flow
- Refresh sign-in UI with new branding
- Streamline ToS/Privacy consent and support re-consent
- Optional partner SSO; profile photo opt-in

#### Scope

- ✅ Auth migration
- ✅ Minors consent flow
- ✅ TOS/PP versioning
- ✅ Home location set during sign-up

#### Out-of-Scope

- ❌ Automated billing for subscriptions (manual in 2026)

#### Success Metrics

- Signup completion rate increases
- Percentage of minors successfully onboarded
- Reduction in auth-related support tickets
- Store listing updates published

#### Dependencies

- Legal review (minors/consent)
- Security engineering (Entra)
- Web/Mobile UI work

#### Risks & Mitigations

**Risk:** Scarce Entra expertise  
**Mitigation:** Contract SME and create detailed migration & rollback plans

#### Implementation Plan

**Data Model Changes:**
- User schema additions for consent artifacts and location

**API Changes:**
- Auth endpoints & claims
- Re-consent logic for updated TOS/PP

**Web UX Changes:**
- Rebrand sign-in pages
- Improve flows, return-to-page post-auth

**Mobile App Changes:**
- Align mobile UI with new consent states

#### Rollout Plan

Dev → Staging (shadow) → Canary (5%) → 100% with hot rollback

---

### Project 2 — Home Page Improvements

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for design review |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Medium |

#### Business Rationale

Make value visible via dynamic news, partners, Teams/Communities awareness, sponsorship placements, and impact metrics.

#### Objectives

- Dynamic content surfaces (news, featured entities, photos)
- Map upgrades (icons, legends, toggles)
- Add litter report creation on web
- Footer EIN doc link
- Tasteful ads for sponsors

#### Success Metrics

- Higher CTR to join/create team/community/partner pages
- Ad viewability within acceptable UX thresholds

#### Dependencies

- Content Management (Project 16)

#### Implementation Plan

**Data Model Changes:**
- Provided via CMS services

**API Changes:**
- CMS content endpoints

**Web UX Changes:**
- Responsive modules
- Personalization for signed-in vs guest

---

### Project 3 — Complete Litter Reporting (Web Parity)

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for team review |
| **Priority** | Moderate |
| **Risk** | Low |
| **Size** | Medium |

#### Business Rationale

Achieve web parity with existing mobile litter reporting and integrate across maps, lists, emails, and event workflows.

#### Objectives

- Map pins & legends
- List/detail views
- Create/edit flows
- Event association & emails

#### Success Metrics

- % of events with associated reports
- Weekly digest engagement
- Low edit error rates

#### Dependencies

- Backend complete; primarily Web UX coordination

#### Implementation Plan

**Web UX Changes:**
- Implement mapping toggles, tooltips, list/table of reports, details page, and editing

---

### Project 4 — Improve Mobile App Robustness

| Attribute | Value |
|-----------|-------|
| **Status** | Developers engaged |
| **Priority** | Very High |
| **Risk** | Low |
| **Size** | Medium |

#### Business Rationale

Stabilize MAUI app (launch issues, error handling), upgrade .NET/MAUI, and instrument with telemetry.

#### Objectives

- Crash handling and user-friendly errors
- Supported OS range defined
- Unit test coverage added
- MAUI toolchain upgrades

#### Success Metrics

- Crash-free sessions ≥ 99.5%
- Manual & automated test matrices passing

#### Dependencies

- CI/CD fixes (Project 5)

#### Implementation Plan

**Mobile App Changes:**
- Upgrade toolchains & MAUI
- Error surfaces & retry logic
- Sentry.io integration

---

### Project 5 — Deployment Pipelines & Infrastructure

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for dev review |
| **Priority** | Medium |
| **Risk** | Moderate |
| **Size** | Medium |

#### Business Rationale

Restore pipeline health, modernize hosting (Docker/ACA), reduce downtime (DB init container), and stand up alerting/monitoring.

#### Objectives

- Upgrade GitHub Actions
- Repair Apple/Google store flows
- Containerize apps
- Dashboards & alerts

#### Success Metrics

- Reduced downtime during deploys
- Faster, reliable releases

#### Implementation Plan

**Data Model Changes:**
- DB migrations through init container

**API Changes:**
- No direct API changes

**Web UX Changes:**
- No direct web UX changes

---

### Project 6 — Backend Code Standards & Structure

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for review |
| **Priority** | Low |
| **Risk** | Low |
| **Size** | Medium |

#### Business Rationale

Unify patterns, upgrade to .NET 10, secure endpoints, update dependencies, and improve Swagger docs.

#### Objectives

- .NET 10 upgrades
- Security pass on all endpoints
- NuGet updates
- XML docs for Swagger

#### Success Metrics

- Fewer defects post-release
- Improved API consumer satisfaction

---

### Project 7 — Add Weights to Event Summaries

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for review |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Very Small |

#### Business Rationale

Enable communities and attendees to track weight metrics accurately for events.

#### Objectives

- **Phase 1:** Total event weight
- **Phase 2:** Attendee-level weights with validation and units

#### Success Metrics

- % of events with weight recorded
- Accurate unit handling per user preference

#### Implementation Plan

**Data Model Changes:**
- Add `TotalWeight` & `WeightUnits` to `EventSummary`
- Create `EventSummaryAttendee` with roll-up to `EventSummary`

**API Changes:**
- **Phase 2:** Controller for `EventSummaryAttendee` (CRUD, queries)

**Web UX Changes:**
- Add weight fields & validation to Summary Edit/View, Dashboard, main metrics, emails

**Mobile App Changes:**
- Add weight fields & validation; Dashboard metrics

---

### Project 8 — Liability Waivers V3

| Attribute | Value |
|-----------|-------|
| **Status** | Requirements & legal review |
| **Priority** | High |
| **Risk** | Very Large |
| **Size** | Very Large |

#### Business Rationale

Support TrashMob and community-specific waivers with validity periods, minors coverage, admin visibility, and printing.

#### Objectives

- Upload & manage waivers with effective/expiry
- List and print waivers by user/community
- Event-time waiver checks
- Minor/guardian workflows

#### Open Questions

- Exact legal storage requirements
- Do TrashMob & community both need waivers?

---

### Project 9 — TrashMob Teams

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for review |
| **Priority** | High |
| **Risk** | Large |
| **Size** | Very Large |

#### Business Rationale

Enable users to form teams, manage membership, own/join events, and showcase impact publicly.

#### Objectives

- Public/private teams
- Multiple leads
- Team metrics & album
- Join/sign-up flows
- Teams map & discovery

#### Implementation Plan

**Data Model Changes:**
- `Team`, `TeamMembers`, `TeamEvents`, `TeamPhotos` tables
- `EventAttendees.TeamId`

**API Changes:**
- `Team` / `TeamMembers` / `TeamEvents` / `TeamPhotos` controllers

**Web UX Changes:**
- Teams page (map, search, details)
- Create/Edit/Manage Team pages
- Dashboard: My Teams

---

### Project 10 — Community Pages

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in progress |
| **Priority** | High |
| **Risk** | Very High |
| **Size** | Very Large |

#### Business Rationale

Branded public pages for partner communities with metrics, photos, contact info, SSO, and adopt-a programs.

#### Objectives

- Community discovery map
- Public home pages
- Admin management & SSO
- Metrics & notifications
- Opt-in to adopt-a programs

#### Dependencies

- Auth migration
- Waivers V3
- Teams
- CMS

#### Implementation Plan

**Data Model Changes:**
- `Partner`: `HomePageStart/EndDate`
- `CommunityProgramTypes` & `CommunityPrograms` tables

---

### Project 11 — Adopt-A-Location

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in progress |
| **Priority** | Medium |
| **Risk** | High |
| **Size** | Very Large |

#### Business Rationale

Model adoptable areas with availability and safety rules; manage team applications; reporting & reminders.

#### Objectives

- Create/manage adoptable areas
- Team applications & approvals
- Public visibility rules
- Delinquency reports & reminders

#### Implementation Plan

**Data Model Changes:**
- `CommunityPrograms` (polygons, frequency)
- `TeamPrograms` (membership & dates)
- `TeamProgramEvents` (event linkage)

---

### Project 12 — In-App Messaging

| Attribute | Value |
|-----------|-------|
| **Status** | Not started |
| **Priority** | Low |
| **Risk** | High |
| **Size** | Medium |

#### Business Rationale

Notify attendees about logistics with strong auditability and abuse prevention.

#### Objectives

- Lead → attendee broadcast
- Audit logs & moderation
- Rate limiting & canned messages

#### Open Questions

- Scope for communities/teams broadcast?
- Abuse prevention policy & enforcement

---

### Project 13 — Bulk Email Invites

| Attribute | Value |
|-----------|-------|
| **Status** | Not started |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Medium |

#### Business Rationale

Enable admins/communities/users to invite at scale with batching and audit trails while controlling email costs.

#### Objectives

- Paste lists; batch >100 sends
- History of batches & statuses
- User-level small batch invites

#### Success Metrics

- Successful delivery rates
- SendGrid cost ceilings respected

---

### Project 14 — Improve Social Media Integration

| Attribute | Value |
|-----------|-------|
| **Status** | Not started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Small |

#### Business Rationale

Modernize social integrations across key touchpoints with thoughtful UX.

#### Objectives

- Audit current integrations
- Add current platforms
- Define sharing by context with design/social input

---

### Project 15 — Map Route Tracing with Decay

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in progress |
| **Priority** | Medium |
| **Risk** | High (privacy) |
| **Size** | Large |

#### Business Rationale

Record and share anonymized routes; enable filters, editing, and association of metrics and notes.

#### Objectives

- Record & trim routes
- Anonymized overlays & filters
- Associate bags/weight/notes
- Privacy controls & sharing options

---

### Project 16 — Page Content Management (CMS)

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in progress |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |

#### Business Rationale

Allow non-developers to update home/partners with preview, scheduling, and versioning/rollback.

#### Objectives

- CMS tool & workflow
- Preview & scheduled publish
- Versioning/rollback

---

### Project 17 — TrashMob.eco MCP Server (AI)

| Attribute | Value |
|-----------|-------|
| **Status** | Not started |
| **Priority** | Low |
| **Risk** | Moderate |
| **Size** | Medium |

#### Business Rationale

Provide safe, privacy-aware AI access to events/metrics via MCP for natural language queries.

#### Objectives

- MCP server exposing scoped metrics/events
- Privacy constraints & anonymization

---

### Project 18 — Before & After Event Pictures

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in progress |
| **Priority** | Low |
| **Risk** | Moderate |
| **Size** | Medium |

#### Business Rationale

Empower leads to document impact visually with admin moderation and TOS enforcement.

#### Objectives

- Upload/manage photos; mark before/after
- Admin moderation queue & removal workflow
- Notification to uploader on removal

---

### Project 19 — Newsletter Support

| Attribute | Value |
|-----------|-------|
| **Status** | Not started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |

#### Business Rationale

Communicate monthly updates with categories/opt-outs, batching/scheduling, and templates (sitewide, team, community).

#### Objectives

- Template library
- SendGrid categories & opt-out respect
- Test sends & scheduling
- Batched processing

---

### Project 20 — Gamification

| Attribute | Value |
|-----------|-------|
| **Status** | Not started |
| **Priority** | Medium |
| **Risk** | High |
| **Size** | Medium |

#### Business Rationale

Drive engagement with leaderboards across roles and time ranges while preventing fraud.

#### Objectives

- Leaderboards by user/team/community
- Time windows (Today → All time)
- Anti-gaming guardrails

---

### Project 21 — Event Co-Leads

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in progress |
| **Priority** | Medium |
| **Risk** | High |
| **Size** | Medium |

#### Business Rationale

Support multiple admins per event and update security/queries accordingly.

#### Objectives

- Invite & manage co-leads
- Attendee list management UI
- Notifications parity for co-leads

#### Implementation Plan

**Data Model Changes:**
- `EventAttendee`: `IsEventLead` flag

**API Changes:**
- APIs to set/clear `IsEventLead`
- Update all admin checks to query `EventAttendee`

**Web UX Changes:**
- Edit Event: attendee list with lead toggles
- Create Event: auto-add creator as lead

**Mobile App Changes:**
- Same as web parity

---

### Project 22 — Attendee-Level Event Metrics

| Attribute | Value |
|-----------|-------|
| **Status** | Not started |
| **Priority** | Medium |
| **Risk** | Medium |
| **Size** | Medium |

#### Business Rationale

Let attendees enter personal stats and give leads tools to reconcile without double counting.

#### Objectives

- Attendee entries for bags/weight/time
- Reconciliation workflow
- Event leaderboards
- Bag drop location notes & alerts

---

### Project 23 — Parental Consent for Minors

| Attribute | Value |
|-----------|-------|
| **Status** | Planning in progress (legal & 3rd parties) |
| **Priority** | High |
| **Risk** | High |
| **Size** | Large |

#### Business Rationale

Support parent-managed dependents or direct minor registration with age verification and protections.

#### Objectives

- Parent-managed dependents flow
- Direct minor registration with verification
- Enhanced protections (no DMs, adult presence)

---

## 9) Risks & Mitigations (Across All Projects)

| Risk | Mitigation |
|------|------------|
| **Legal/Compliance** (COPPA & waiver enforceability) | Legal sign-off gates before build |
| **Auth Migration** (complex identity changes) | Canary rollouts, rollback playbooks, perf/load testing |
| **Volunteer Capacity** (rolling availability) | Modular backlog, handover checklists, maintainer rotation |
| **Mobile Quality Debt** (unstable toolchains) | Q1 stabilization, .NET/MAUI upgrades, instrumentation |
| **Cost Spikes** (Azure/Maps/Email) | Cost reviews, caching, quotas, batching |

---

## 10) Appendices

### A. Glossary (Selected)

| Term | Definition |
|------|------------|
| **Community** | A city/county/region with a TrashMob partnership; may have a branded page and programs |
| **Team** | User-created group with location, membership, and event participation |
| **Event Lead / Co-Lead** | User(s) with admin rights for an event |

### B. Standard Field Definitions (Sample)

| Field | Definition |
|-------|------------|
| **Weight Units** | lbs or kgs; user preference defaults to imperial |
| **EventSummaryAttendee** | Per-attendee metrics table (IDs, counts, duration, units, audit fields) |

### C. Open Questions (To Converge in Discovery)

- What exact documents/data are required to prove parental consent and for how long must we retain them?
- Which community waiver scenarios are MVP-critical vs Phase 2?
- Messaging scope: do communities/teams also need broadcast? Abuse prevention policy?

---

**End of Document**