# Project 2 — Home Page Improvements

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for Design Review |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Medium |
| **Dependencies** | Project 16 (Content Management) |

---

## Business Rationale

Increase engagement and value communication by surfacing dynamic content, highlighting community participation opportunities (Teams/Communities/Partners), and introducing tasteful sponsorship placements. The home page is the primary entry point for most users and should effectively communicate TrashMob's mission, impact, and participation opportunities.

---

## Objectives

### Primary Goals
- **Dynamic content surfaces** for news, featured entities, and impact photos
- **Map upgrades** with improved icons, legends, and toggle controls
- **Enable litter report creation** directly from home page (web)
- **Footer EIN documentation** link for transparency
- **Tasteful sponsorship/advertising** placements that respect user experience

### Secondary Goals
- Improve home page load performance
- Better mobile responsiveness
- A/B test different layouts for conversion optimization

---

## Scope

### Phase 1 - Dynamic Content
- ? Integrate with CMS for dynamic news/announcements
- ? Featured entity carousel (teams, communities, partners)
- ? Impact photo gallery with recent event photos
- ? Dynamic statistics dashboard

### Phase 2 - Map Enhancements
- ? New event pin icons (upcoming vs past, with/without summary)
- ? Map legend with toggle controls
- ? Litter report pins integration
- ? Filter controls (event type, date range, etc.)

### Phase 3 - Sponsorship Integration
- ? Sponsor banner zones (top, sidebar, bottom)
- ? Rotation logic for multiple sponsors
- ? Click tracking and analytics
- ? Admin UI for sponsor management

### Phase 4 - Quick Actions
- ? "Report Litter" quick action button
- ? "Create Event" prominent CTA
- ? "Join a Team" discovery link
- ? EIN documentation footer link

---

## Out-of-Scope

- ? Complete site redesign (incremental improvements only)
- ? User personalization based on ML (future phase)
- ? Video content hosting
- ? Real-time chat or forums
- ? Multi-language support (separate project)

---

## Success Metrics

### Quantitative
- **CTR to create/join team/community:** Increase by 25%
- **Event registration from home page:** Increase by 15%
- **Litter report creation:** 50+ new reports from home page CTA per month
- **Sponsor ad viewability:** ? 70% viewability score
- **Sponsor CTR:** ? 1.5%
- **Home page bounce rate:** Decrease by 10%

### Qualitative
- Positive user feedback on new layout
- Sponsors express satisfaction with visibility
- No user complaints about "too many ads"
- Mobile experience improved

---

## Dependencies

### Blockers
- **Project 16 (CMS):** Required for dynamic content management
- **Design approval:** Mockups and sponsor placement guidelines
- **Sponsor contracts:** At least 2-3 sponsors lined up

### Enables
- **Project 10 (Community Pages):** Links from home page
- **Project 9 (Teams):** Team discovery from home page
- **Project 3 (Litter Reporting Web):** Report creation flow

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Sponsors perceived as intrusive** | Medium | High | User testing, tasteful placement, clear labeling as "Sponsors" |
| **Performance degradation** | Low | Medium | Lazy loading, CDN for images, performance budget |
| **CMS delays project** | Medium | Medium | Fallback to static content if CMS not ready |
| **Design disagreements** | Medium | Low | User research, A/B testing, stakeholder reviews |

---

## Implementation Plan

### Data Model Changes

No direct database changes. Content served via CMS API.

### API Changes

**New endpoints:**
```csharp
// Sponsor management
[HttpGet("api/v2/sponsors/active")]
public async Task<ActionResult<IEnumerable<SponsorDto>>> GetActiveSponsors()
{
    // Return sponsors with active placements
}

// Featured content
[HttpGet("api/v2/featured")]
public async Task<ActionResult<FeaturedContentDto>> GetFeaturedContent()
{
    // Return featured teams, communities, events
}
```

**CMS integration:**
- Proxy endpoints to Strapi for news/announcements
- Cache responses for performance

### Web UX Changes

**Home Page Sections (Top to Bottom):**

1. **Hero Section** (Dynamic from CMS)
   - Background image
   - Call-to-action buttons
   - Impact stats overlay

2. **What is TrashMob** (Dynamic from CMS)
   - Mission statement
   - How it works
   - Video placeholder

3. **Events Map** (Enhanced)
   - Improved pins and legends
   - Filter controls
   - Toggle for litter reports

4. **Featured Content** (New)
   - Rotating carousel
   - Teams, communities, partners
   - Recent event photos

5. **Impact Statistics** (Dynamic)
   - Total bags, volunteers, events
   - Real-time or daily refresh

6. **Sponsor Banner** (New)
   - Tasteful placement
   - Rotation if multiple sponsors
   - Click tracking

7. **Getting Started** (Dynamic from CMS)
   - Quick action buttons
   - Links to guides

8. **Footer** (Enhanced)
   - EIN documentation link
   - Social media
   - Contact info

**Responsive Design:**
- Mobile-first approach
- Stack sponsor banners vertically on mobile
- Optimize map for touch interactions

### Mobile App Changes

No mobile app changes required (web-only improvements).

---

## Implementation Phases

### Phase 1: Design & CMS Setup
- Finalize mockups
- Set up CMS content types
- Get sponsor contracts and assets

### Phase 2: Dynamic Content Implementation
- Integrate CMS API
- Build featured content carousel
- Implement news/announcements

### Phase 3: Map Enhancements
- New pin icons and legend
- Filter controls
- Litter report toggle
- Performance optimization

### Phase 4: Sponsorship Integration
- Banner zones implementation
- Admin UI for sponsor management
- Click tracking setup
- Analytics integration

### Phase 5: Testing & Polish
- User acceptance testing
- Performance testing
- A/B test setup
- Accessibility audit

### Phase 6: Deployment
- Staged rollout (10% ? 50% ? 100%)
- Monitor metrics
- Collect feedback

**Note:** Volunteers can work on multiple phases in parallel where dependencies allow.

---

## Sponsorship Guidelines

### Placement Options
1. **Top Banner:** 728x90 (desktop), 320x50 (mobile)
2. **Sidebar:** 300x250
3. **Footer Banner:** 728x90

### Design Requirements
- Clear "Sponsored" or "Our Sponsors" label
- Professional, non-intrusive design
- No animation or auto-play video
- Static images or simple CSS animations only

### Rotation Logic
- Equal impression share for all active sponsors
- Random rotation on page load
- Session-based (same sponsor for duration of session)

---

## Open Questions

1. **How many sponsor slots do we allow simultaneously?**  
   **Recommendation:** Start with 3 slots (top, sidebar, footer), max 1 sponsor per slot at a time  
   **Owner:** Product Lead + Board  
   **Due:** Before sponsorship phase

2. **What's the pricing model for sponsors?**  
   **Recommendation:** Monthly flat rate based on historical traffic, reviewed quarterly  
   **Owner:** Finance + Board  
   **Due:** Before contracts signed

3. **Do we allow sponsor tracking pixels?**  
   **Recommendation:** No third-party tracking; provide aggregated analytics reports monthly  
   **Owner:** Security + Privacy team  
   **Due:** Before sponsorship phase

4. **Should featured content be curated or algorithmic?**  
   **Recommendation:** Manual curation for now (2-3 features per week), algorithmic in future  
   **Owner:** Product team  
   **Due:** Early in project

---

## Related Documents

- **[Project 16 - Content Management](./Project_16_Content_Management.md)** - CMS dependency
- **[Project 3 - Litter Reporting Web](./Project_03_Litter_Reporting_Web.md)** - Report creation flow
- **[Project 9 - Teams](./Project_09_Teams.md)** - Team discovery links
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community discovery links

---

**Last Updated:** January 24, 2026  
**Owner:** Web Product Lead + UX Designer  
**Status:** Ready for Design Review  
**Next Review:** Before implementation begins
