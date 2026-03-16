# TrashMob.eco — 2026 Planning Documentation

**Version:** 1.2
**Date:** February 16, 2026
**Owner:** Director of Product & Engineering

---

## 📍 Navigation

### Core Documents

- **[Executive Summary](./Executive_Summary.md)** - High-level overview, strategic objectives, and 2026 roadmap
- **[Risks & Mitigations](./Risks_and_Mitigations.md)** - Cross-project risks and mitigation strategies

### 2026 Projects (58 Total)

**Note:** Projects are not time-bound. Volunteers pick up work based on priority and availability.

#### Critical Priority (Infrastructure & Blockers)

| Project | Description | Status |
|---------|-------------|--------|
| [Project 1 - Auth Revamp](./Projects/Project_01_Auth_Revamp.md) | Azure B2C → Entra External ID migration | In Progress (Phases 0-5a Complete, Production Live; Phase 6 Partial, Phase 7 Remaining) |
| [Project 4 - Mobile Robustness](./Projects/Project_04_Mobile_Robustness.md) | Stabilize MAUI apps, error handling | In Progress (Phases 1-2, 6 Complete; Phases 3-5 Substantial) |
| [Project 5 - Deployment Pipelines](./Projects/Project_05_Deployment_Pipelines.md) | CI/CD modernization, Docker | In Progress (Phases 1-3 Complete; Phase 4 Partial) |

#### High Priority (Core Features)

| Project | Description | Status |
|---------|-------------|--------|
| [Project 2 - Home Page Improvements](./Projects/Project_02_Home_Page.md) | Dynamic content, ads, sponsors | Ready for Design Review |
| [Project 3 - Litter Reporting Web](./Projects/Project_03_Litter_Reporting_Web.md) | Complete web parity for litter reports | ✅ Complete |
| [Project 7 - Event Weights](./Projects/Project_07_Event_Weights.md) | Track weight metrics | ✅ Complete |
| [Project 8 - Waivers V3](./Projects/Project_08_Waivers_V3.md) | Community waivers, minors coverage | In Progress (Phases 1-4, 5a-5e, 6 Complete; minor waiver signing pending legal) |
| [Project 9 - Teams](./Projects/Project_09_Teams.md) | User-created teams MVP | ✅ Complete |
| [Project 10 - Community Pages](./Projects/Project_10_Community_Pages.md) | Branded partner community pages | ✅ Complete |
| [Project 13 - Bulk Email Invites](./Projects/Project_13_Bulk_Email_Invites.md) | Scale email invitations | ✅ Complete |
| [Project 23 - Parental Consent](./Projects/Project_23_Parental_Consent.md) | Privo.com integration for minors (combined with Project 1) | In Progress (Phase 0, 3 Complete; Phases 1-2 blocked on Privo) |
| [Project 24 - API v2 Modernization](./Projects/Project_24_API_v2_Modernization.md) | Pagination, error handling, auto-generated clients | Not Started |
| [Project 38 - Mobile Feature Parity](./Projects/Project_38_Mobile_Feature_Parity.md) | Teams, leaderboards, photos for mobile volunteers | ✅ Complete |
| [Project 44 - Area Map Editor](./Projects/Project_44_Area_Map_Editor.md) | Interactive map editor, AI area suggestions, bulk import/export, AI generation | ✅ Complete |

#### Medium Priority (Enhancements)

| Project | Description | Status |
|---------|-------------|--------|
| [Project 6 - Backend Standards](./Projects/Project_06_Backend_Standards.md) | Code standards, .NET 10 upgrades | In Progress (Phases 1, 2.5, 3 Substantial) |
| [Project 11 - Adopt-A-Location](./Projects/Project_11_Adopt_A_Location.md) | Location adoption program | ✅ Complete |
| [Project 14 - Social Media Integration](./Projects/Project_14_Social_Media.md) | Modernize social sharing | ✅ Complete |
| [Project 15 - Route Tracing](./Projects/Project_15_Route_Tracing.md) | Map route tracing with privacy | In Progress (Phases 1-5 Complete, Mobile Device Testing Remaining) |
| [Project 16 - Content Management](./Projects/Project_16_Content_Management.md) | Strapi CMS integration | ✅ Complete |
| [Project 19 - Newsletter Support](./Projects/Project_19_Newsletter.md) | Monthly communication | ✅ Complete |
| [Project 20 - Gamification](./Projects/Project_20_Gamification.md) | Leaderboards and achievements | ✅ Complete |
| [Project 21 - Event Co-Leads](./Projects/Project_21_Event_Co_Leads.md) | Multiple event admins | ✅ Complete |
| [Project 22 - Attendee Metrics](./Projects/Project_22_Attendee_Metrics.md) | Per-attendee event statistics | ✅ Complete |
| [Project 25 - Automated Testing](./Projects/Project_25_Automated_Testing.md) | E2E tests for web and mobile | In Progress (Phase 1 Complete) |
| [Project 37 - Unit Test Coverage](./Projects/Project_37_Unit_Test_Coverage.md) | Improve backend unit test coverage | ✅ Complete |
| [Project 26 - KeyVault RBAC Migration](./Projects/Project_26_KeyVault_RBAC_Migration.md) | Migrate from access policies to RBAC | ✅ Complete |
| [Project 28 - Photo Moderation](./Projects/Project_28_Photo_Moderation.md) | Admin photo review and flagging | ✅ Complete |
| [Project 29 - Feature Usage Metrics](./Projects/Project_29_Feature_Usage_Metrics.md) | Track feature usage analytics | ✅ Complete |
| [Project 30 - Azure Billing Alerts](./Projects/Project_30_Azure_Billing_Alerts.md) | Cost monitoring and alerts | In Progress |
| [Project 31 - Feature Flags](./Projects/Project_31_Feature_Flags.md) | Feature flag infrastructure | Not Started |
| [Project 32 - Database Backups](./Projects/Project_32_Database_Backups.md) | Backup retention policies | ✅ Complete |
| [Project 33 - Localization](./Projects/Project_33_Localization.md) | Multi-language support | Deprioritized |
| [Project 34 - User Feedback](./Projects/Project_34_User_Feedback.md) | In-app feedback widget | ✅ Complete |
| [Project 35 - Partner Location Map](./Projects/Project_35_Partner_Location_Map.md) | Partner locations on map | ✅ Complete |
| [Project 39 - Regional Communities](./Projects/Project_39_Regional_Communities.md) | County/state community pages and adoption programs | ✅ Complete |
| [Project 41 - Sponsored Adoptions](./Projects/Project_41_Sponsored_Adoptions.md) | Sponsor-funded professional cleanup tracking | In Progress (Phases 1-4 Complete) |
| [Project 42 - Partner Document Management](./Projects/Project_42_Partner_Document_Management.md) | File upload & storage for partner documents | ✅ Complete |
| [Project 54 - Community Adoption Outreach](./Projects/Project_54_Community_Adoption_Outreach.md) | AI-powered sponsor/adopter discovery and outreach for community managers | Not Started |
| [Project 47 - Team-Visible Private Events](./Projects/Project_47_Team_Private_Events.md) | Team-scoped event visibility for member-only events | ✅ Complete |
| [Project 48 - Enhanced Route Tracking](./Projects/Project_48_Enhanced_Route_Tracking.md) | Route-level metrics, litter density heatmap, route time editing | Complete (Phases 1-3); Phase 2b & 4 Deferred |
| [Project 49 - Privacy & Compliance Review](./Projects/Project_49_Privacy_Compliance_Review.md) | Deletion audit, data export, GDPR compliance, cookie consent | In Progress (Phases 1-4 Complete, Phase 5 Partial) |
| [Project 53 - Mobile Offline Persistence](./Projects/Project_53_Mobile_Offline_Persistence.md) | Client-side persistence for routes, metrics, and photos with background sync | Complete (Phases 1–4); Deferred: platform background sync, server-side dedup |
| [Project 55 - Event Check-In](./Projects/Project_55_Event_Check_In.md) | Pre-event waiver validation, configurable check-in notifications, attendance roster for event leads | Not Started |
| [Project 56 - Board Metrics Dashboard](./Projects/Project_56_Board_Metrics_Dashboard.md) | Unified admin dashboard consolidating App Insights, GA4, Sentry, Clarity, Azure costs, and QuickBooks for board meetings | Planning |
| [Project 57 - Participation Report](./Projects/Project_57_Participation_Report.md) | Official volunteer participation report email with PDF for school/court/employer verification | In Progress (Phases 1-3 Complete) |

#### High Priority (Marketing & Growth)

| Project | Description | Status |
|---------|-------------|--------|
| [Project 36 - Marketing Materials](./Projects/Project_36_Marketing_Materials.md) | 2026 release marketing, pricing, enrollment | In Progress |
| [Project 40 - AI Community Sales Agent](./Projects/Project_40_AI_Community_Sales_Agent.md) | AI-powered community discovery, outreach, and onboarding | ✅ Complete |
| [Project 45 - Community Showcase](./Projects/Project_45_Community_Showcase.md) | "For Communities" landing page, home page CTAs, enrollment funnel | In Progress (Phases 1-4 Complete) |
| [Project 46 - Product Support](./Projects/Project_46_Product_Support.md) | Define product support role, channels, onboarding, and knowledge base | In Progress (Phases 1-3 Complete) |
| [Project 50 - News & Blog](./Projects/Project_50_News_Blog.md) | Blog-style news page powered by Strapi CMS with pagination and categories | ✅ Complete |
| [Project 51 - Contact Management](./Projects/Project_51_Contact_Management.md) | Donor tracking, grant management, contact CRM for nonprofit fundraising (Site Admin only) | ✅ Complete |
| [Project 58 - Community Event Seeding](./Projects/Project_58_Community_Event_Seeding.md) | Admin-seeded public cleanup events with claim workflow to solve cold-start growth problem | Planning |

#### Low Priority (Nice-to-Have)

| Project | Description | Status |
|---------|-------------|--------|
| [Project 12 - In-App Messaging](./Projects/Project_12_In_App_Messaging.md) | Event lead communications | Not Started |
| [Project 17 - MCP Server](./Projects/Project_17_MCP_Server.md) | AI access via MCP protocol | ✅ Complete |
| [Project 18 - Before/After Photos](./Projects/Project_18_Before_After_Photos.md) | Event impact photos | ✅ Complete |
| [Project 27 - OpenTelemetry Migration](./Projects/Project_27_OpenTelemetry_Migration.md) | Vendor-neutral observability | ✅ Complete |
| [Project 43 - Sign Management](./Projects/Project_43_Sign_Management.md) | Track adoption sign lifecycle, location, and text | Not Started |
| [Project 52 - Volunteer Rewards](./Projects/Project_52_Volunteer_Rewards.md) | Partner-contributed rewards for volunteer engagement (future) | Not Started |

---

## 🔗 Quick Links by Theme

### Trust & Safety
- [Project 1 - Auth Revamp](./Projects/Project_01_Auth_Revamp.md)
- [Project 8 - Waivers V3](./Projects/Project_08_Waivers_V3.md)
- [Project 23 - Parental Consent](./Projects/Project_23_Parental_Consent.md)
- [Project 28 - Photo Moderation](./Projects/Project_28_Photo_Moderation.md) ✅
- [Project 49 - Privacy & Compliance Review](./Projects/Project_49_Privacy_Compliance_Review.md)
- [Project 55 - Event Check-In](./Projects/Project_55_Event_Check_In.md)

### Community Features
- [Project 9 - Teams](./Projects/Project_09_Teams.md) ✅
- [Project 10 - Community Pages](./Projects/Project_10_Community_Pages.md) ✅
- [Project 11 - Adopt-A-Location](./Projects/Project_11_Adopt_A_Location.md) ✅
- [Project 18 - Before/After Photos](./Projects/Project_18_Before_After_Photos.md) ✅
- [Project 39 - Regional Communities](./Projects/Project_39_Regional_Communities.md) ✅
- [Project 41 - Sponsored Adoptions](./Projects/Project_41_Sponsored_Adoptions.md)
- [Project 42 - Partner Document Management](./Projects/Project_42_Partner_Document_Management.md)
- [Project 43 - Sign Management](./Projects/Project_43_Sign_Management.md)
- [Project 44 - Area Map Editor](./Projects/Project_44_Area_Map_Editor.md) ✅
- [Project 47 - Team-Visible Private Events](./Projects/Project_47_Team_Private_Events.md) ✅
- [Project 54 - Community Adoption Outreach](./Projects/Project_54_Community_Adoption_Outreach.md)

### Impact Tracking
- [Project 7 - Event Weights](./Projects/Project_07_Event_Weights.md) ✅
- [Project 15 - Route Tracing](./Projects/Project_15_Route_Tracing.md)
- [Project 21 - Event Co-Leads](./Projects/Project_21_Event_Co_Leads.md) ✅
- [Project 22 - Attendee Metrics](./Projects/Project_22_Attendee_Metrics.md)
- [Project 48 - Enhanced Route Tracking](./Projects/Project_48_Enhanced_Route_Tracking.md)
- [Project 53 - Mobile Offline Persistence](./Projects/Project_53_Mobile_Offline_Persistence.md) ✅
- [Project 57 - Participation Report](./Projects/Project_57_Participation_Report.md)

### Platform Quality
- [Project 4 - Mobile Robustness](./Projects/Project_04_Mobile_Robustness.md)
- [Project 5 - Deployment Pipelines](./Projects/Project_05_Deployment_Pipelines.md)
- [Project 24 - API v2 Modernization](./Projects/Project_24_API_v2_Modernization.md)
- [Project 25 - Automated Testing](./Projects/Project_25_Automated_Testing.md)
- [Project 27 - OpenTelemetry Migration](./Projects/Project_27_OpenTelemetry_Migration.md) ✅
- [Project 29 - Feature Usage Metrics](./Projects/Project_29_Feature_Usage_Metrics.md) ✅
- [Project 37 - Unit Test Coverage](./Projects/Project_37_Unit_Test_Coverage.md) ✅
- [Project 56 - Board Metrics Dashboard](./Projects/Project_56_Board_Metrics_Dashboard.md)
- [Project 38 - Mobile Feature Parity](./Projects/Project_38_Mobile_Feature_Parity.md)

### Infrastructure & Operations
- [Project 26 - KeyVault RBAC Migration](./Projects/Project_26_KeyVault_RBAC_Migration.md) ✅
- [Project 30 - Azure Billing Alerts](./Projects/Project_30_Azure_Billing_Alerts.md)
- [Project 31 - Feature Flags](./Projects/Project_31_Feature_Flags.md)
- [Project 32 - Database Backups](./Projects/Project_32_Database_Backups.md) ✅

### Engagement & Growth
- [Project 12 - In-App Messaging](./Projects/Project_12_In_App_Messaging.md)
- [Project 13 - Bulk Email Invites](./Projects/Project_13_Bulk_Email_Invites.md) ✅
- [Project 20 - Gamification](./Projects/Project_20_Gamification.md) ✅
- [Project 34 - User Feedback](./Projects/Project_34_User_Feedback.md) ✅
- [Project 36 - Marketing Materials](./Projects/Project_36_Marketing_Materials.md)
- [Project 40 - AI Community Sales Agent](./Projects/Project_40_AI_Community_Sales_Agent.md)
- [Project 45 - Community Showcase](./Projects/Project_45_Community_Showcase.md)
- [Project 46 - Product Support](./Projects/Project_46_Product_Support.md)
- [Project 50 - News & Blog](./Projects/Project_50_News_Blog.md)
- [Project 51 - Contact Management](./Projects/Project_51_Contact_Management.md)
- [Project 52 - Volunteer Rewards](./Projects/Project_52_Volunteer_Rewards.md)
- [Project 58 - Community Event Seeding](./Projects/Project_58_Community_Event_Seeding.md)

---

## Project Status Overview

| Status | Count | Projects |
|--------|-------|----------|
| ✅ **Complete** | 32 | Projects 3, 7, 9, 10, 11, 13, 14, 16, 17, 18, 19, 20, 21, 22, 26, 27, 28, 29, 32, 34, 35, 37, 38, 39, 40, 42, 44, 47, 48, 50, 51, 53 |
| **In Progress** | 14 | Projects 1, 4, 5, 6, 8, 15, 23, 25, 30, 41, 45, 46, 49, 57 |
| **Planning** | 2 | Projects 56, 58 |
| **Ready for Review** | 1 | Project 2 |
| **Not Started** | 7 | Projects 12, 24, 31, 36, 43, 52, 54, 55 |
| **Deprioritized** | 1 | Project 33 |

**Total:** 58 project specifications documented

---

## Remaining Work

### In Progress — Remaining Tasks

| Project | What's Left |
|---------|-------------|
| [Project 1 - Auth Revamp](./Projects/Project_01_Auth_Revamp.md) | Phase 7: Minor protections — core protections complete (name masking, adult presence, parent notifications, minor badges). Remaining: Privo documentation package |
| [Project 23 - Parental Consent](./Projects/Project_23_Parental_Consent.md) | Phases 0, 3 complete. Phases 1-2 (Privo age verification & parental consent) blocked on Privo API onboarding |
| [Project 4 - Mobile Robustness](./Projects/Project_04_Mobile_Robustness.md) | Phases 3-5: Manual test matrix on physical devices, regression testing, load testing, accessibility audit (TalkBack/VoiceOver), supported device docs |
| [Project 5 - Deployment Pipelines](./Projects/Project_05_Deployment_Pipelines.md) | Phase 4: Deployment health dashboards; Phase 5: Cost optimization & auto-scaling; Phase 6: Security scanning (OWASP ZAP, CodeQL, Trivy) |
| [Project 6 - Backend Standards](./Projects/Project_06_Backend_Standards.md) | Phase 3: Security audit — review remaining API endpoints for authorization, input validation, rate limiting |
| [Project 8 - Waivers V3](./Projects/Project_08_Waivers_V3.md) | Phase 5 remaining: Minor waiver signing dialog (web + mobile), WaiverVersion record with minor scope, compliance export with dependent waivers — all pending legal review of minor waiver text |
| [Project 15 - Route Tracing](./Projects/Project_15_Route_Tracing.md) | Mobile app route recording/upload device testing on iOS and Android |
| [Project 25 - Automated Testing](./Projects/Project_25_Automated_Testing.md) | Phase 2: Core web flow E2E tests (event CRUD, litter reports, partner management); Phase 3: Mobile test foundation; Phase 4: Remaining tests & perf baseline |
| [Project 30 - Azure Billing Alerts](./Projects/Project_30_Azure_Billing_Alerts.md) | Operational monitoring and alert tuning |
| [Project 41 - Sponsored Adoptions](./Projects/Project_41_Sponsored_Adoptions.md) | Phase 5: Reporting & analytics (community-level volunteer vs. sponsored breakdown, cost-per-mile, company comparison, compliance reports) |
| [Project 45 - Community Showcase](./Projects/Project_45_Community_Showcase.md) | Deferred: Community testimonials (waiting on onboarded communities), AI Sales Agent integration |
| [Project 46 - Product Support](./Projects/Project_46_Product_Support.md) | Phase 4: Support KPIs, feedback loop, quarterly review (activate when support role is staffed) |
| [Project 49 - Privacy & Compliance](./Projects/Project_49_Privacy_Compliance_Review.md) | Phase 5: Cookie consent & tracking audit — update Privacy Policy, document third-party services (Clarity, App Insights) |
| [Project 53 - Mobile Offline Persistence](./Projects/Project_53_Mobile_Offline_Persistence.md) | Deferred: Android WorkManager / iOS BGTaskScheduler background sync; server-side idempotent uploads (route SessionId dedup, photo PhotoId dedup) |
| [Project 57 - Participation Report](./Projects/Project_57_Participation_Report.md) | Phases 1-3 complete (backend + web + mobile). Phase 4 deferred: verification URLs with QR codes |

### Ready for Design Review

| Project | What's Left |
|---------|-------------|
| [Project 2 - Home Page Improvements](./Projects/Project_02_Home_Page.md) | All phases pending design review (dynamic content, map enhancements, sponsorship, quick actions) |

### Planning

| Project | Description |
|---------|-------------|
| [Project 56 - Board Metrics Dashboard](./Projects/Project_56_Board_Metrics_Dashboard.md) | Unified admin dashboard pulling from App Insights, GA4, Sentry, Clarity, Azure Cost Management, and QuickBooks for monthly board meetings |
| [Project 58 - Community Event Seeding](./Projects/Project_58_Community_Event_Seeding.md) | Admin-seeded public cleanup events with organizer claim workflow to solve cold-start growth |

### Not Started

| Project | Description |
|---------|-------------|
| [Project 12 - In-App Messaging](./Projects/Project_12_In_App_Messaging.md) | Push notifications, in-app message center, notification preferences |
| [Project 24 - API v2 Modernization](./Projects/Project_24_API_v2_Modernization.md) | Versioned API endpoints, deprecation, auto-generated clients |
| [Project 31 - Feature Flags](./Projects/Project_31_Feature_Flags.md) | Feature flag infrastructure for safe deployments and gradual rollouts |
| [Project 36 - Marketing Materials](./Projects/Project_36_Marketing_Materials.md) | Community tier pricing, feature comparison, branding guidelines, digital assets |
| [Project 43 - Sign Management](./Projects/Project_43_Sign_Management.md) | Track physical Adopt-A-Location signs (status, coordinates, text, lifecycle) |
| [Project 52 - Volunteer Rewards](./Projects/Project_52_Volunteer_Rewards.md) | Partner reward sourcing, criteria, distribution, fraud prevention (future) |
| [Project 54 - Community Adoption Outreach](./Projects/Project_54_Community_Adoption_Outreach.md) | AI-powered sponsor/adopter discovery and outreach for community managers |

### Complete with Deferred Items

| Project | Deferred Items |
|---------|---------------|
| [Project 22 - Attendee Metrics](./Projects/Project_22_Attendee_Metrics.md) | Route association (waiting for routes to be routinely used) |
| [Project 48 - Enhanced Route Tracking](./Projects/Project_48_Enhanced_Route_Tracking.md) | Phase 2b: Per-segment density; Phase 4: Smart trim suggestions (speed anomaly detection) |
| [Project 51 - Contact Management](./Projects/Project_51_Contact_Management.md) | Deadline calendar, budget tracking per grant, periodic grant scan, impact-to-giving report |

---

## 📚 Related Documents

- **[Product Requirements Document (PRD)](../TrashMob/TrashMob.prd)** - Complete user stories and requirements
- **[Website User Stories](../WebsiteUserStories.md)** - Detailed user story backlog
- **[Project-Specific Documentation](../claude.md)** - Root AI assistant context
- **[TrashMob API Context](../TrashMob/claude.md)** - API development patterns
- **[Mobile Development Guide](../TrashMobMobile/claude.md)** - MAUI app patterns (when created)
- **[Feature Metrics Guide](./Projects/Project_29_Metrics_Guide.md)** - Stakeholder metrics documentation

### 🤖 AI Assistant Commands

- **[Commands Index](./.claude_commands_index.md)** - Complete guide to all AI assistant commands
- **[Add New Project Command](./.claude_add_project_command.md)** - Detailed guide for adding projects
- **[Add Project Quick Reference](./.claude_add_project_quick.md)** - Quick checklist version
- **[Common Commands](./.claude_common_commands.md)** - Update status, dependencies, risks, etc.

---

## 📋 Document Conventions

### Project File Structure

Each project document follows a standardized format:

```markdown
# Project ## — Title

| Attribute | Value |
| Status | Not Started / Planning / In Progress / Ready for Review / Complete |
| Priority | Low / Medium / High / Critical |
| Risk | Low / Medium / High / Very High |
| Size | Very Small / Small / Medium / Large / Very Large |
| Timeline | Q# 2026 |

## Business Rationale
## Objectives
## Scope
## Out-of-Scope
## Success Metrics
## Dependencies
## Risks & Mitigations
## Implementation Plan
## Rollout Plan
## Open Questions
```

### Status Definitions

- **Not Started:** No work begun, awaiting prioritization
- **Planning:** Requirements being refined, design in progress
- **Ready for Review:** Spec complete, awaiting stakeholder approval
- **In Progress:** Active development underway
- **Developers Engaged:** Team assigned and working
- **Complete:** Delivered and in production
- **Deprioritized:** Deferred to future consideration

### Priority Levels

- **Critical:** Blocks other work or has imminent deadline
- **High:** Important for 2026 goals
- **Medium:** Should be done but flexible timing
- **Low:** Nice-to-have, can defer to 2027

### Feature Metrics

All new features should consider adding feature usage tracking. See [Project 29 - Feature Usage Metrics](./Projects/Project_29_Feature_Usage_Metrics.md) for the tracking infrastructure and [Feature Metrics Guide](./Projects/Project_29_Metrics_Guide.md) for implementation details.

---

**Last Updated:** March 16, 2026
**Maintained By:** Product & Engineering Team
**Next Review:** End of Q1 2026
