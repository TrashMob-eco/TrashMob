# Project 36 — Marketing Materials for 2026 Release

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

With the completion of major features in early 2026, TrashMob needs professional marketing materials to promote the platform, attract new communities, and communicate the value proposition to potential partners. Clear documentation of features, pricing, and enrollment processes will drive community adoption.

---

## Objectives

### Primary Goals
- **Feature overview document** highlighting all 2026 releases
- **Community pricing guide** with tier details
- **Enrollment guide** for new communities
- **One-pager** for quick partner outreach

### Secondary Goals
- Social media announcement templates
- Email campaign content
- Press release draft
- Partner presentation deck

---

## Scope

### Phase 1 - Feature Documentation
- ? Document all completed features from 2026:

  **Volunteer & Event Features:**
  - **Teams** (Project 9) - User-created volunteer teams with membership, logos, and collective impact tracking
  - **Event Co-Leads** (Project 21) - Multiple administrators per event
  - **Event Weights** (Project 7) - Track weight collected per event for grant reporting
  - **Before/After Photos** (Project 18) - Visual event impact documentation with photo galleries
  - **Gamification** (Project 20) - Leaderboards, achievements, and volunteer recognition
  - **Litter Reporting Web** (Project 3) - Full web parity for reporting litter locations

  **Community & Partner Features:**
  - **Community Pages** (Project 10) - Branded partner community pages with custom programs
  - **Regional Communities** (Project 39) - County/state/region community hierarchy with area-based adoption programs
  - **Adopt-A-Location** (Project 11) - Location adoption program for recurring cleanups
  - **Sponsored Adoptions** (Project 41) - Sponsor-funded professional cleanup tracking with company and sponsor portals
  - **Partner Document Management** (Project 42) - File upload and storage for agreements, contracts, and compliance docs
  - **Partner Location Map** (Project 35) - Partner locations displayed on interactive map

  **Communication & Growth:**
  - **Bulk Email Invites** (Project 13) - Admin, community, team, and user invitation system
  - **Newsletter Support** (Project 19) - Monthly newsletter with subscriber preferences and team event linking
  - **Social Media Integration** (Project 14) - Modernized social sharing across platforms
  - **User Feedback** (Project 34) - In-app feedback collection widget

  **Analytics & Site Administration:**
  - **Attendee Metrics** (Project 22) - Per-attendee statistics and tracking
  - **Photo Moderation** (Project 28) - Admin review and flagging system
  - **Feature Usage Metrics** (Project 29) - Analytics and reporting dashboards
  - **AI Community Sales Agent** (Project 40) - Site admin tool for AI-powered community discovery, prospect scoring, outreach, and conversion tracking

  **Platform & Technical:**
  - **Route Tracing** (Project 15) - GPS route recording during cleanups with heat maps and coverage statistics
  - **Content Management** (Project 16) - Strapi CMS integration for dynamic site content
  - **MCP Server / AI Integration** (Project 17) - AI access to TrashMob data via Model Context Protocol
  - **Mobile Feature Parity** (Project 38) - Teams, leaderboards, event photos, and achievements on mobile

- ? Create feature comparison table (Free vs Community tier)
- ? Write benefit-focused descriptions for each feature

### Phase 2 - Pricing & Enrollment
- ? Define community tier pricing structure
- ? Document what's included at each tier:
  - **Free Tier:** Event creation, co-leads, volunteer registration, teams, leaderboards, achievements, litter reporting, route tracing, before/after photos, event weights
  - **Community Tier:** Everything in Free, plus branded pages, regional hierarchy, adopt-a-location, sponsored adoptions, custom waivers, bulk invites, newsletters, analytics dashboards, heat maps, partner documents, MCP integration, priority support
- ? Create step-by-step enrollment guide
- ? Document community admin onboarding process
- ? Create FAQ for prospective communities

### Phase 3 - Marketing Collateral
- ? One-page feature overview (PDF)
- ? Community enrollment flyer
- ? Social media post templates (Twitter/X, LinkedIn, Facebook)
- ? Email announcement templates
- ? Partner presentation slides

---

## Out-of-Scope

- ? Paid advertising campaigns
- ? Video production
- ? Website redesign
- ? Print materials (physical brochures)
- ? Localized/translated versions

---

## Success Metrics

### Quantitative
- **Community inquiries:** Track increase after materials release
- **Enrollment conversion:** % of inquiries that become communities
- **Document downloads:** Track one-pager and guide downloads
- **Social engagement:** Likes, shares, clicks on announcements

### Qualitative
- Positive feedback from partner outreach
- Clear understanding of features by prospects
- Reduced questions during enrollment process

---

## Dependencies

### Blockers
None - all referenced features are complete

### Enables
- Community growth initiative
- Partner outreach program
- Volunteer recruitment campaigns

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Pricing not finalized** | Medium | High | Work with business team to confirm tiers before publishing |
| **Features change post-release** | Low | Low | Version documents; update as needed |
| **Inconsistent messaging** | Medium | Medium | Single source of truth; review process |

---

## Implementation Plan

### Feature Highlights Document

```markdown
# TrashMob 2026 - What's New

## Teams & Gamification
Create volunteer teams, track collective impact, upload team logos,
and invite members via email. Leaderboards and achievements motivate
volunteers with recognition for milestones and consistent participation.

## Community & Regional Pages
Get your own branded TrashMob community page with custom programs,
dedicated analytics, and professional presence. Now supports regional
hierarchies — cities, counties, states — for scaled adoption programs.

## Adopt-A-Location & Sponsored Adoptions
Adopt cleanup locations for recurring maintenance. Sponsors can fund
professional cleanups with dedicated company and sponsor portals
for compliance tracking, cleanup logs, and CSV report exports.

## Route Tracing & Heat Maps
Record GPS routes during cleanups to visualize coverage. Heat maps
show community-wide cleanup patterns and identify underserved areas.

## Event Photos & Before/After Documentation
Capture event impact with photo galleries. Before/after comparisons
tell the story of your cleanup efforts for reports and social media.

## Bulk Email Invites & Newsletter
Grow your volunteer base with bulk invitation tools. Monthly newsletters
keep volunteers engaged with subscriber preferences and team event linking.

## Event Co-Leads & Enhanced Metrics
Share event management with co-leads. Track individual volunteer
contributions, event weights, and export detailed reports for
grant applications and impact reporting.

## Partner Document Management
Upload and manage agreements, contracts, insurance certificates,
and compliance documents directly within the partner dashboard.

## Mobile App Improvements
Teams, leaderboards, event photos, and achievements now on mobile.
Improved navigation, error handling, and Sentry crash reporting
for a polished volunteer experience.

## AI & Developer Integration
MCP server provides AI tools with read access to TrashMob data
for building custom integrations, dashboards, and analysis tools.
```

### Pricing Structure (Draft)

| Feature | Free | Community |
|---------|------|-----------|
| Event creation & co-leads | ✓ | ✓ |
| Volunteer registration | ✓ | ✓ |
| Basic impact tracking & event weights | ✓ | ✓ |
| Teams & leaderboards | ✓ | ✓ |
| Litter reporting | ✓ | ✓ |
| Before/after photos | ✓ | ✓ |
| Route tracing | ✓ | ✓ |
| Achievements & gamification | ✓ | ✓ |
| Branded community page | - | ✓ |
| Regional community hierarchy | - | ✓ |
| Adopt-a-location program | - | ✓ |
| Sponsored adoption portals | - | ✓ |
| Custom waivers | - | ✓ |
| Bulk email invites (500/day) | - | ✓ |
| Newsletter management | - | ✓ |
| Analytics dashboards & heat maps | - | ✓ |
| Partner document management | - | ✓ |
| MCP/AI data integration | - | ✓ |
| Priority support | - | ✓ |

**Pricing:** Contact info@trashmob.eco for community tier pricing

### Enrollment Process

1. **Express Interest:** Email info@trashmob.eco or fill out contact form
2. **Discovery Call:** 30-minute call to understand community needs
3. **Agreement:** Sign community partner agreement
4. **Setup:** TrashMob team configures community page and branding
5. **Training:** 1-hour onboarding session for community admins
6. **Launch:** Announce community page and begin recruiting volunteers

---

## Deliverables

| Deliverable | Format | Audience |
|-------------|--------|----------|
| Feature Overview | Markdown/PDF | All |
| Pricing Guide | Markdown/PDF | Communities |
| Enrollment Guide | Markdown/PDF | Prospective communities |
| One-Pager | PDF | Partner outreach |
| Social Templates | Text files | Marketing |
| Email Templates | HTML/Text | Marketing |

---

## Open Questions

1. **What is the community tier pricing?**
   **Decision:** Pending
   **Status:** Needs business input

2. **Who reviews marketing materials before release?**
   **Decision:** Pending
   **Status:** Needs process definition

3. **Where will materials be hosted?**
   **Decision:** Pending (website, GitHub, both?)
   **Status:** Needs decision

4. **Should we create a landing page for communities?**
   **Decision:** Pending
   **Status:** Could be separate project

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **TBD** - Create tracking issue when work begins

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Teams feature details
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community feature details
- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** - Adoption program details
- **[Project 13 - Bulk Email Invites](./Project_13_Bulk_Email_Invites.md)** - Invite system details
- **[Project 15 - Route Tracing](./Project_15_Route_Tracing.md)** - Route tracing and heat maps
- **[Project 20 - Gamification](./Project_20_Gamification.md)** - Leaderboards and achievements
- **[Project 39 - Regional Communities](./Project_39_Regional_Communities.md)** - Regional hierarchy
- **[Project 40 - AI Community Sales Agent](./Project_40_AI_Community_Sales_Agent.md)** - AI-powered growth
- **[Project 41 - Sponsored Adoptions](./Project_41_Sponsored_Adoptions.md)** - Sponsor-funded cleanups
- **[Project 42 - Partner Document Management](./Project_42_Partner_Document_Management.md)** - Document storage
- **[Executive Summary](../Executive_Summary.md)** - Strategic objectives

---

**Last Updated:** February 10, 2026
**Owner:** Marketing & Product Team
**Status:** Not Started
**Next Review:** When pricing decisions are finalized
