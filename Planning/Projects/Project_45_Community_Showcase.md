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

**Key design decision:** Rather than building a completely separate enrollment system, this project reuses the existing partner request pipeline (`/becomeapartner` form and `PartnerRequest` backend). A new "Community" partner type option is added, and a community-focused landing page provides the value proposition context before funneling users to the existing form.

---

## Objectives

### Primary Goals
- **Dedicated "For Communities" landing page** showcasing the community feature set with clear value propositions
- **Home page community CTA** — prominent call-to-action alongside the existing volunteer CTA
- **Self-service enrollment** — enhance existing partner request form with "Community" type, linked from the landing page
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
  - **Initial launch:** No communities exist yet, so this section shows placeholder/aspirational content (e.g. "Be one of the first communities to join TrashMob" with illustrative mockup imagery). Once real communities are onboarded and marked as featured by site admins, placeholder content is replaced automatically.
- ? **Enrollment CTA** — "Start Your Community" button linking to `/becomeapartner?type=community`:
  - Reuses the existing partner request form, pre-selecting the new "Community" partner type
  - Form shows community-specific intro text and descriptions (see Partner Type Selection UX below)
  - No new form needed — the existing Name, Email, Website, Phone, Notes, Location fields work as-is
  - Backend already creates partner requests that admins review and approve
- ? **FAQ Section** — Common questions about communities:
  - What is a TrashMob community?
  - How much does it cost?
  - How long does setup take?
  - Can we customize our page?
  - What support do you provide?

### Phase 2 — Home Page Community Integration

- ? **Add community CTA to hero section** — Second button alongside "Join us today": "Start a Community" or "For Communities" linking to `/for-communities`
- ? **Featured Communities section** — Carousel or grid showing communities marked as featured (via admin `IsFeatured` flag) with logos, names, and key metrics, with "View All Communities" link. Section is hidden or shows a "Start the first community" CTA when no featured communities exist.
- ? **Update stats section** — Add community-specific stats:
  - Number of active communities
  - Cities/regions covered
  - Community-organized events count
- ? **Community testimonial** — Brief quote from a community admin in the social proof area

### Phase 3 — Navigation & Discovery Improvements

- ? **Add "For Communities" to About dropdown** — Add as a link in the About dropdown menu (not a top-level nav item, not buried in Explore)
- ? **Update Getting Started page** — Expand the "Explore Your Community" section with a stronger CTA to become a community (not just browse existing ones)
- ? **Update Partnerships page** — Add a "Looking to start a community?" banner that distinguishes geographic community partners from service partners, linking to `/for-communities`
- ? **Footer link** — Add "For Communities" to the site footer

### Phase 4 — Backend & Integration

- ? **Add "Community" partner type** — Add `Community = 3` to `PartnerTypeEnum` as a new third type (not replacing Government). The three types represent different relationships:
  - **Government (= 1):** Service-providing agencies (waste management, hauling, disposal)
  - **Business (= 2):** Commercial service partners (recycling firms, safety kit providers)
  - **Community (= 3):** Geographic entities (cities, counties, nonprofits) wanting branded presence, volunteer tools, and analytics
  - Note: A city government could be both a Government service partner AND a Community partner — the types are not mutually exclusive
- ? **Community metrics API** — `GET /api/communities/public-stats`:
  - Total active communities
  - Total community events
  - Total community volunteers
  - Geographic coverage summary
- ? **Featured communities flag** — Add `IsFeatured` boolean to the Partner model (default `false`). Site admins can toggle this in the admin dashboard to control which communities appear on the landing page and home page. When no communities are featured, the pages show placeholder/aspirational content instead.
- ? **AI Sales Agent integration** — When a partner request with type "Community" is created, optionally create a prospect in the AI Sales Agent pipeline (Project 40)

---

## Out-of-Scope

- ? Automated community self-provisioning (communities are still set up by admins after enrollment)
- ? Payment processing or subscription management (handled offline for now)
- ? Separate enrollment form or backend pipeline (reuse existing partner request flow)
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
- New page: `src/pages/for-communities/page.tsx` — Community-focused landing page with value prop, features, pricing, success stories
- Modify: `src/components/partner-requests/partner-request-form.tsx` — Enhance partner type selection with clear descriptions (see UX details below)
- Modify: `src/enums/PartnerType.ts` — Add `COMMUNITY = '3'`
- Modify: `src/store/ToolTips.tsx` — Update `PartnerType` tooltip and add per-type descriptions
- Modify: `src/pages/_home/index.tsx` — Add community CTA and featured communities section
- Modify: `src/components/SiteHeader/MainNav.tsx` — Add "For Communities" navigation item
- Modify: `src/pages/partnerships/page.tsx` — Add "Looking to start a community?" section with link to `/for-communities`

**Backend:**
- Add `Community = 3` to `PartnerTypeEnum` in `TrashMob.Models/Enums.cs`
- Add `IsFeatured` boolean property to `Partner` model + migration
- Modify: `PartnerRequest` processing to recognize community type and optionally create AI Sales Agent prospect
- New endpoint: `GET /api/communities/public-stats` — Aggregate stats (community count, events, volunteers) for the landing page
- New endpoint: `GET /api/communities/featured` — Return partners where `IsFeatured = true` and partner type is Community

### Partner Type Selection UX

The current form shows bare radio buttons ("Government" / "Business") with no descriptions. Adding "Community" as a third option requires clear guidance so users know which to select. The radio group should be redesigned as a card-style selector with descriptions:

```
Partner Type:

  ○ Government
    City, county, or state agency providing waste management,
    hauling, or disposal services for cleanup events.

  ○ Business
    Commercial company offering recycling, hauling, disposal,
    or other cleanup-related services.

  ○ Community                                        ← NEW
    City, county, nonprofit, or organization that wants a
    branded TrashMob community page with volunteer engagement
    tools, adoption programs, and analytics.
```

**Behavior when arriving from `/for-communities`:**
- Pre-select "Community" via `?type=community` query param
- Show community-specific intro text instead of the current government-focused paragraph:
  > "Start your TrashMob community! Tell us about your organization and the area you'd like to cover. Our team will review your request and set up your branded community page."
- The government-focused helper text ("If connecting with a government partner, the department responsible for managing waste...") should only display when Government is selected

**Behavior for existing `/becomeapartner` flow (no query param):**
- Default to Government (current behavior preserved)
- Show current intro text
- All three options available

**Tooltip update:** Change `PartnerType` tooltip from "The type of the partner" to "Select the type of partnership you're looking for. Government and Business partners provide services for events. Community partners get a branded page with volunteer and adoption tools."

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
   **Decision:** Resolved — use placeholder content initially; add a site-admin "featured" flag on communities
   **Status:** No communities exist yet, so the landing page and home page will launch with placeholder/aspirational content. Once communities are onboarded, site admins can toggle a "featured" flag to showcase them. See Phase 4 featured communities configuration.

2. **Should "For Communities" be a top-level nav item?**
   **Decision:** Resolved — place in the About dropdown menu, plus a prominent CTA on the home page hero
   **Status:** Not a top-level nav item (avoids nav clutter). Instead: (a) add "For Communities" link in the About dropdown, and (b) add a prominent "For Communities" or "Start a Community" button in the home page hero section alongside the existing "Join us today" CTA.

3. **Should the interest form create a full prospect or just send an email?**
   **Decision:** Resolved — reuse the existing partner request pipeline
   **Status:** Form creates a `PartnerRequest` with type "Community" (existing admin review flow). Phase 4 optionally creates an AI Sales Agent prospect for Community-type requests.

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
