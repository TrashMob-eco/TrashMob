# Project 45 — Community Showcase & Conversion

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 10 (Community Pages), Project 36 (Marketing Materials) |

---

## Business Rationale

Communities are the primary revenue driver for TrashMob.eco — they represent the community tier subscription that funds the platform. In 2026, the platform has shipped a comprehensive community feature set (branded pages, regional hierarchy, adopt-a-location, sponsored adoptions, analytics, bulk invites, and more), but the website doesn't prominently showcase these capabilities to prospective communities.

Today, the home page CTA is "Join us today" — entirely volunteer-focused. Communities are buried under the Explore dropdown. The Partnerships page targets service providers (hauling, disposal), not geographic community partners. There is no dedicated landing page explaining why a city, county, or nonprofit should become a TrashMob community, and no self-service enrollment path.

This project creates a clear funnel: **Discover → Understand → Enroll** for prospective communities.

---

## Objectives

### Primary Goals
- **Dedicated "For Communities" landing page** showcasing the community feature set with clear value propositions
- **Home page community CTA** — prominent call-to-action alongside the existing volunteer CTA
- **Self-service interest form** — simple enrollment inquiry that feeds into the AI Sales Agent pipeline (Project 40)
- **Community success stories** — showcase existing communities with metrics and testimonials

### Secondary Goals
- Improve community discovery in main navigation (promote from dropdown submenu)
- Add community count and impact metrics to the home page stats section
- Create a "Featured Communities" carousel/section on the home page
- SEO optimization for community-related search terms

---

## Scope

### Phase 1 — "For Communities" Landing Page

Create a new public page at `/for-communities` that serves as the primary conversion page for prospective communities.

**Page Sections:**
- ? **Hero** — "Bring TrashMob to Your Community" with compelling imagery and primary CTA
- ? **Value Proposition Cards** — 3-4 cards highlighting key benefits:
  - Branded community page with your identity
  - Volunteer recruitment and engagement tools
  - Impact tracking and grant-ready reporting
  - Professional adoption and sponsor programs
- ? **Feature Showcase** — Visual walkthrough of community features:
  - Community pages with custom branding
  - Adopt-a-location with area maps
  - Analytics dashboards and heat maps
  - Bulk invites and newsletter management
  - Event management with co-leads
  - Sponsored adoption compliance tracking
- ? **Pricing Overview** — Free vs Community tier comparison table (from Project 36)
- ? **Community Success Stories** — 2-3 featured communities with:
  - Community name, logo, and branded page screenshot
  - Key metrics (events held, volunteers engaged, bags collected)
  - Brief testimonial quote from community admin
- ? **Enrollment CTA** — "Start Your Community" form:
  - Organization name, type (city/county/nonprofit/HOA), contact name, email, website
  - Estimated volunteer base size
  - Geographic area of interest
  - Submits to backend and creates a prospect in the AI Sales Agent pipeline
- ? **FAQ Section** — Common questions about communities:
  - What is a TrashMob community?
  - How much does it cost?
  - How long does setup take?
  - Can we customize our page?
  - What support do you provide?

### Phase 2 — Home Page Community Integration

- ? **Add community CTA to hero section** — Second button alongside "Join us today": "Start a Community" or "For Communities" linking to `/for-communities`
- ? **Featured Communities section** — Carousel or grid showing 3-4 active communities with logos, names, and key metrics, with "View All Communities" link
- ? **Update stats section** — Add community-specific stats:
  - Number of active communities
  - Cities/regions covered
  - Community-organized events count
- ? **Community testimonial** — Brief quote from a community admin in the social proof area

### Phase 3 — Navigation & Discovery Improvements

- ? **Add "For Communities" to main navigation** — Either as a top-level nav item or prominently in the About dropdown (not buried in Explore)
- ? **Update Getting Started page** — Expand the "Explore Your Community" section with a stronger CTA to become a community (not just browse existing ones)
- ? **Update Partnerships page** — Add a "Looking to start a community?" banner that distinguishes geographic community partners from service partners, linking to `/for-communities`
- ? **Footer link** — Add "For Communities" to the site footer

### Phase 4 — Backend & Integration

- ? **Community interest form API endpoint** — `POST /api/community-interest`:
  - Accepts form submission (name, type, contact, email, website, size, area)
  - Creates a prospect record in the AI Sales Agent pipeline (Project 40)
  - Sends confirmation email to the submitter
  - Sends notification to site admins
- ? **Community metrics API** — `GET /api/communities/public-stats`:
  - Total active communities
  - Total community events
  - Total community volunteers
  - Geographic coverage summary
- ? **Featured communities configuration** — Admin ability to mark communities as "featured" for the home page carousel (could be a flag on the Partner/Community model, or managed via Strapi CMS)

---

## Out-of-Scope

- ? Automated community self-provisioning (communities are still set up by admins after enrollment)
- ? Payment processing or subscription management (handled offline for now)
- ? Community comparison or directory features beyond existing communities list page
- ? Redesign of existing community detail pages (Project 10 already handles this)
- ? Video testimonials or production content (Project 36 handles marketing collateral)

---

## Success Metrics

### Quantitative
- **Community inquiry form submissions:** Target 10+ per month within 3 months of launch
- **For Communities page visits:** Track via Feature Usage Metrics (Project 29)
- **Home page → For Communities conversion rate:** % of home page visitors who navigate to the community page
- **Inquiry-to-enrollment conversion:** % of form submissions that become active communities
- **Time to first inquiry:** How quickly after launch the first organic inquiry arrives

### Qualitative
- Prospective communities can understand the value proposition without a phone call
- Existing communities feel showcased and valued
- The enrollment path is clear and friction-free

---

## Dependencies

### Blockers
- **Project 10 (Community Pages)** — ✅ Complete. Existing community pages provide the feature set to showcase.
- **Project 36 (Marketing Materials)** — Pricing table and feature descriptions needed for the landing page.

### Enables
- Organic community growth without manual outreach
- Self-service funnel that feeds Project 40 (AI Sales Agent) pipeline
- Community-focused marketing campaigns (Project 36)

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **No existing community testimonials** | Medium | Medium | Work with current community admins to gather quotes; use metrics as social proof in the interim |
| **Pricing not finalized** | Medium | High | Show feature comparison without specific pricing; use "Contact us" CTA until pricing is set |
| **Low traffic to new page** | Medium | Medium | Ensure strong internal linking (home page, nav, footer) and SEO optimization |
| **Form spam** | Low | Low | Add CAPTCHA or honeypot field; rate limiting on the API endpoint |

---

## Implementation Plan

### Technical Architecture

**Frontend:**
- New page: `src/pages/for-communities/page.tsx` — Main landing page
- New component: `src/components/CommunityShowcase/` — Reusable feature showcase cards
- New component: `src/components/CommunityInterestForm/` — Enrollment form with validation
- Modify: `src/pages/_home/index.tsx` — Add community CTA and featured communities
- Modify: `src/components/SiteHeader/MainNav.tsx` — Add navigation item
- New service: `src/services/community-interest.ts` — Form submission API

**Backend:**
- New controller: `CommunityInterestController.cs` — Form submission + public stats
- New model: `CommunityInterestSubmission` — Form data entity
- Integration: Create prospect in AI Sales Agent pipeline on submission

### Wireframes

**For Communities Landing Page:**
```
┌─────────────────────────────────────────────┐
│  [Logo]  Nav: Explore | Action | For Communities | About  │
├─────────────────────────────────────────────┤
│                                             │
│   Bring TrashMob to Your Community          │
│   Organize cleanups, engage volunteers,     │
│   and track your environmental impact.      │
│                                             │
│   [Start Your Community]  [Browse Communities] │
│                                             │
├─────────────────────────────────────────────┤
│                                             │
│   ┌──────┐  ┌──────┐  ┌──────┐  ┌──────┐   │
│   │Brand │  │Recruit│  │Impact│  │Adopt │   │
│   │Page  │  │Volun- │  │Report│  │Loca- │   │
│   │      │  │teers  │  │ing   │  │tions │   │
│   └──────┘  └──────┘  └──────┘  └──────┘   │
│                                             │
├─────────────────────────────────────────────┤
│  Feature Showcase (visual walkthrough)      │
│  Screenshots + descriptions of key features │
├─────────────────────────────────────────────┤
│  Free vs Community Tier comparison table    │
├─────────────────────────────────────────────┤
│  Community Success Stories                  │
│  ┌────────┐  ┌────────┐  ┌────────┐        │
│  │ City A │  │ City B │  │ City C │        │
│  │ Stats  │  │ Stats  │  │ Stats  │        │
│  │ Quote  │  │ Quote  │  │ Quote  │        │
│  └────────┘  └────────┘  └────────┘        │
├─────────────────────────────────────────────┤
│  Start Your Community                       │
│  [Organization Name]  [Type dropdown]       │
│  [Contact Name]  [Email]  [Website]         │
│  [Area of Interest]  [Est. Volunteers]      │
│  [Submit Inquiry]                           │
├─────────────────────────────────────────────┤
│  FAQ Accordion                              │
├─────────────────────────────────────────────┤
│  Footer                                     │
└─────────────────────────────────────────────┘
```

**Home Page Community Section:**
```
┌─────────────────────────────────────────────┐
│  Existing Hero Section                      │
│  [Join Us Today]  [Start a Community →]     │
├─────────────────────────────────────────────┤
│  ...existing sections...                    │
├─────────────────────────────────────────────┤
│  Featured Communities                       │
│  ┌────────┐  ┌────────┐  ┌────────┐        │
│  │ Logo   │  │ Logo   │  │ Logo   │        │
│  │ Name   │  │ Name   │  │ Name   │        │
│  │ Events │  │ Events │  │ Events │        │
│  └────────┘  └────────┘  └────────┘        │
│  [View All Communities]  [Start Your Own →] │
├─────────────────────────────────────────────┤
│  ...existing sections...                    │
└─────────────────────────────────────────────┘
```

---

## Deliverables

| Deliverable | Phase | Description |
|-------------|-------|-------------|
| "For Communities" landing page | Phase 1 | Full conversion page at `/for-communities` |
| Community interest form + API | Phase 1+4 | Self-service enrollment inquiry |
| Home page community CTA | Phase 2 | Hero button + featured communities section |
| Navigation updates | Phase 3 | "For Communities" in nav, footer, and related pages |
| Public stats API | Phase 4 | Community metrics endpoint for landing page |

---

## Open Questions

1. **Which existing communities should be featured?**
   **Decision:** Pending
   **Status:** Need to identify 2-3 active communities willing to be showcased

2. **Should "For Communities" be a top-level nav item?**
   **Decision:** Pending
   **Status:** Need UX input — top-level item vs prominent placement in About dropdown

3. **Should the interest form create a full prospect or just send an email?**
   **Decision:** Pending
   **Status:** If Project 40 pipeline is active, integrate directly; otherwise start with email notification

4. **What community-specific metrics should be shown on the home page?**
   **Decision:** Pending
   **Status:** Need to determine which stats are most compelling (communities count, events, volunteers)

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **TBD** - Create tracking issues when work begins

---

## Related Documents

- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Existing community feature set
- **[Project 36 - Marketing Materials](./Project_36_Marketing_Materials.md)** - Pricing and feature documentation
- **[Project 39 - Regional Communities](./Project_39_Regional_Communities.md)** - Regional hierarchy features
- **[Project 40 - AI Community Sales Agent](./Project_40_AI_Community_Sales_Agent.md)** - Pipeline integration
- **[Project 2 - Home Page Improvements](./Project_02_Home_Page.md)** - Related home page work

---

**Last Updated:** February 10, 2026
**Owner:** Product & Engineering Team
**Status:** Not Started
**Next Review:** When Phase 1 design is approved
