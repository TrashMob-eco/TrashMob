# Project 54 — Community Adoption Outreach Tool

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 40 (AI Community Sales Agent), Project 11 (Adopt-A-Location), Project 41 (Sponsored Adoptions) |

---

## Business Rationale

Community managers are responsible for growing their adoption programs — finding local companies, nonprofits, and organizations willing to sponsor or volunteer-adopt cleanup locations (streets, parks, trails, waterways). Today, this outreach is entirely manual: the community manager must research local businesses, find contact information, send emails, track responses, and follow up — all outside the platform.

Meanwhile, TrashMob already has a powerful prospect pipeline (Project 40) with AI-powered discovery, automated outreach, scoring, and conversion — but it's accessible only to site administrators and operates globally. Community managers cannot use it.

This project brings that proven infrastructure to community managers, scoped to their community, with AI prompts tailored to finding local sponsors and adopters rather than new community partners. It's a force multiplier: every community manager gets an AI-assisted sales tool to grow their adoption program.

---

## Objectives

### Primary Goals
- **AI-powered local discovery:** Community managers can use AI to search for local businesses, nonprofits, and organizations that are good candidates for adopting or sponsoring cleanup locations
- **Per-community prospect pipeline:** Each community has its own prospect list with pipeline stages (Lead, Contacted, Engaged, Converted), independent of the global admin pipeline
- **Contact tracking:** Track who has been contacted, when, how (email, phone, in-person), and their response
- **Outreach automation:** Generate and send personalized outreach emails to prospects, with follow-up cadence tracking
- **Adoptable area targeting:** Optionally link prospects to specific adoptable areas (e.g., "this restaurant is near the Main Street segment")

### Secondary Goals (Nice-to-Have)
- Dashboard stats showing prospect pipeline health (count per stage, conversion rate)
- "Find Sponsors" action button on adoptable areas that links to discovery pre-filtered for that area
- Export prospect list to CSV for offline follow-up
- Batch outreach sending for multiple prospects at once

---

## Scope

### Phase 1 — Backend: Model & Data Layer
- ☐ Add `PartnerId` (nullable `Guid?`) to `CommunityProspect` model to scope prospects per community
- ☐ Add `TargetAdoptableAreaId` (nullable `Guid?`) to `CommunityProspect` to optionally link a prospect to a specific adoptable area
- ☐ Add navigation properties and FK configuration in `MobDbContext`
- ☐ Create EF Core migration with index on `PartnerId`
- ☐ Extend `ICommunityProspectManager` with community-scoped query methods: `GetByCommunityAsync`, `GetByCommunityAndStageAsync`, `SearchByCommunityAsync`
- ☐ Implement community-scoped queries in `CommunityProspectManager`

### Phase 2 — Backend: API & AI Discovery
- ☐ Create `CommunityAdoptionProspectsController` at route `api/communities/{partnerId}/prospects`
- ☐ Use `UserIsPartnerUserOrIsAdmin` authorization pattern (same as `CommunityAdoptionsController`)
- ☐ Endpoints: CRUD, pipeline stage updates, activity log, AI discovery, outreach preview/send/history
- ☐ Auto-set `PartnerId` on created prospects; verify prospect belongs to community on reads
- ☐ Update `ClaudeDiscoveryService` with a sponsor/adopter discovery prompt mode that asks Claude to find:
  - Local businesses near the community (restaurants, retail, offices, property managers)
  - Organizations with CSR programs or existing local sponsorships
  - HOAs, neighborhood associations, civic groups
  - Companies near specific adoptable areas (when area context provided)
- ☐ Auto-populate community location context (city, region, country, boundary) in discovery requests

### Phase 3 — Frontend: Community Dashboard Pages
- ☐ Create prospect list page at `partnerdashboard/$partnerId/community/prospects/index.tsx`
  - Pipeline stage tabs with counts
  - DataTable with search and columns: Name, Type, Contact, Stage, Fit Score, Last Contacted, Actions
  - Action buttons: AI Discovery, Add Prospect
- ☐ Create AI discovery page at `partnerdashboard/$partnerId/community/prospects/discover.tsx`
  - Pre-populated location fields from community data
  - Custom query tab for freeform AI search (e.g., "Find restaurants within 1 mile of our Main Street adoption area")
  - Optional adoptable area selector for targeted search
  - Results table with "Add to Pipeline" button
- ☐ Create prospect detail page at `partnerdashboard/$partnerId/community/prospects/$prospectId.tsx`
  - Contact info card, pipeline stage badge, fit score
  - Target adoptable area link (if set)
  - Activity log timeline (calls, emails, meetings, notes)
  - Outreach email history with preview/send buttons
- ☐ Create prospect form at `partnerdashboard/$partnerId/community/prospects/create.tsx`
  - Fields: name, type, contact name/email/phone, website, notes
  - Optional: select target adoptable area dropdown
- ☐ Create community-scoped frontend API service (`community-adoption-prospects.ts`)
- ☐ Add `partnerId` and `targetAdoptableAreaId` fields to `CommunityProspectData` model
- ☐ Add "Adoption Outreach" link to community sidebar navigation in `_layout.tsx`
- ☐ Add lazy-loaded routes in `App.tsx`

### Phase 4 — Integration & Polish
- ☐ Add "Find Sponsors" action button on adoptable areas page for Available areas → links to discovery with area pre-selected
- ☐ Add prospect pipeline summary card to community dashboard (count per stage)
- ☐ Verify existing global admin prospect pages (`/siteadmin/prospects/`) continue to work unchanged

---

## Out-of-Scope

- ❌ Phone or voice outreach automation
- ❌ Payment processing or sponsor contract management
- ❌ Sponsor billing or invoicing
- ❌ Mobile app prospect management (web-only, admin tool)
- ❌ Public-facing sponsor directory (separate future project)
- ❌ Replacing or modifying the existing admin-only prospect pipeline (Project 40)

---

## Success Metrics

### Quantitative
- **Adoption rate:** ≥ 20% increase in adoptable areas moving from Available to Adopted status within 6 months of launch
- **Prospect pipeline per community:** ≥ 10 qualified prospects per community within first month of use
- **Outreach effectiveness:** ≥ 10% response rate on AI-generated outreach emails
- **Feature adoption:** ≥ 50% of active community managers use the tool within 3 months of launch

### Qualitative
- Community managers report the tool saves significant time compared to manual research
- AI-discovered prospects are relevant and geographically appropriate
- Contact tracking reduces duplicate outreach and missed follow-ups

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Project 40 (AI Community Sales Agent):** ✅ Complete — Provides the prospect pipeline infrastructure (models, managers, AI services, outreach engine)
- **Project 11 (Adopt-A-Location):** ✅ Complete — Adoptable area system that prospects can be linked to

### Enablers (Benefit from but don't block)
- **Project 41 (Sponsored Adoptions):** In Progress — When complete, converted prospects can become sponsors with professional cleanup tracking
- **Project 43 (Sign Management):** Not Started — Could eventually link prospect-to-sponsor-to-sign lifecycle

### Enables Other Projects
- **Project 41 (Sponsored Adoptions):** This tool feeds the sponsor pipeline, making it easier to find and convert companies into sponsors
- **Project 52 (Volunteer Rewards):** Discovered partners could contribute volunteer rewards

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Spam perception from community outreach** | Medium | High | Rate limit outreach per community (max 10/day); CAN-SPAM compliance; require verified community manager; easy opt-out |
| **Low-quality AI suggestions for local businesses** | Medium | Medium | Allow community managers to provide custom queries; include community boundary/location context; A/B test prompts; provide manual add option as fallback |
| **Community managers don't adopt the tool** | Low | Medium | Simple onboarding flow; pre-populate with community location; surface "Find Sponsors" prompts on adoptable areas page |
| **Data overlap with admin prospect pipeline** | Low | Low | `PartnerId` scoping keeps community prospects separate; admin pipeline continues to work on global (null PartnerId) prospects |

---

## Implementation Plan

### Existing Infrastructure to Extend

The following code from Project 40 will be extended (not duplicated):

| Component | File | Extension Needed |
|-----------|------|-----------------|
| `CommunityProspect` model | `TrashMob.Models/CommunityProspect.cs` | Add `PartnerId`, `TargetAdoptableAreaId` (nullable FKs) |
| `ICommunityProspectManager` | `TrashMob.Shared/Managers/Prospects/ICommunityProspectManager.cs` | Add community-scoped query methods |
| `CommunityProspectManager` | `TrashMob.Shared/Managers/Prospects/CommunityProspectManager.cs` | Implement community-filtered queries |
| `ClaudeDiscoveryService` | `TrashMob.Shared/Managers/Prospects/ClaudeDiscoveryService.cs` | Add sponsor/adopter prompt mode |
| `MobDbContext` | `TrashMob.Shared/Persistence/MobDbContext.cs` | FK configuration for new columns |

### New Code to Create

| Component | File | Pattern to Follow |
|-----------|------|-------------------|
| Controller | `TrashMob/Controllers/CommunityAdoptionProspectsController.cs` | `CommunityAdoptionsController.cs` (auth pattern) + `CommunityProspectsController.cs` (endpoints) |
| Frontend service | `TrashMob/client-app/src/services/community-adoption-prospects.ts` | `community-prospects.ts` (service factory pattern) |
| Prospect list page | `partnerdashboard/$partnerId/community/prospects/index.tsx` | `siteadmin/prospects/page.tsx` (layout + DataTable) |
| AI discovery page | `partnerdashboard/$partnerId/community/prospects/discover.tsx` | `siteadmin/prospects/discovery.tsx` (AI query UI) |
| Prospect detail page | `partnerdashboard/$partnerId/community/prospects/$prospectId.tsx` | `siteadmin/prospects/$prospectId.tsx` (detail + activity timeline) |
| Create form | `partnerdashboard/$partnerId/community/prospects/create.tsx` | `siteadmin/prospects/create.tsx` (form pattern) |

### Authorization Pattern

```csharp
// Same pattern as CommunityAdoptionsController
[Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
[RequiredScope(Constants.TrashMobReadScope)]
public async Task<IActionResult> GetProspects(Guid partnerId, ...)
{
    var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
    if (partner is null) return NotFound();
    if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
        return Forbid();
    // ... community-scoped query
}
```

### AI Discovery Prompt (Sponsor/Adopter Mode)

```
You are a research assistant helping a community cleanup program find local businesses
and organizations that might be willing to sponsor or adopt a cleanup location.

The community is in {City}, {Region}, {Country}.
{Optional: "They have an adoptable area called '{AreaName}' ({AreaType}) that needs a sponsor."}

Look for: local businesses, restaurants, retail stores, property management companies,
corporate offices with CSR programs, HOAs, neighborhood associations, civic organizations,
environmental nonprofits, and companies that already sponsor local parks or events.

Return ONLY a JSON array with: name, type, website, contactSuggestion, rationale.
```

---

## Rollout Plan

1. **Development:** Build all 4 phases incrementally; each phase can be deployed independently
2. **Internal testing:** Site admins test with a dev community
3. **Beta:** Enable for 2-3 active community managers; gather feedback
4. **GA:** Enable for all community managers; announce via newsletter
5. **Iterate:** Tune AI prompts based on community manager feedback and prospect quality

---

## Open Questions

1. **Should outreach emails come from the community's contact email or a central TrashMob address?**
   **Recommendation:** Use a central address (e.g., outreach@trashmob.eco) initially for deliverability; add community-specific "reply-to" in Phase 4
   **Owner:** Product Owner
   **Due:** Before Phase 2

2. **Should community managers see each other's prospects (for multi-admin communities)?**
   **Recommendation:** Yes — prospects are scoped to the community (PartnerId), not to individual users. All community admins see the same pipeline.
   **Owner:** Engineering
   **Due:** Phase 1

3. **What daily outreach limit should apply per community?**
   **Recommendation:** 10 emails/day per community to start; configurable per community in future
   **Owner:** Product Owner
   **Due:** Before Phase 2

4. **Should converted prospects auto-create a Sponsor record or just link to the partner?**
   **Recommendation:** Initially just update prospect status to "Converted" and add a note. Auto-creation of Sponsor records can be added when Project 41 (Sponsored Adoptions) is fully complete.
   **Owner:** Engineering
   **Due:** Phase 4

---

## Related Documents

- **[Project 40 - AI Community Sales Agent](./Project_40_AI_Community_Sales_Agent.md)** — The global prospect pipeline this project extends
- **[Project 11 - Adopt-A-Location](./Project_11_Adopt_A_Location.md)** — The adoptable area system prospects can be linked to
- **[Project 41 - Sponsored Adoptions](./Project_41_Sponsored_Adoptions.md)** — Professional cleanup tracking that converted sponsors feed into
- **[Project 43 - Sign Management](./Project_43_Sign_Management.md)** — Future sign lifecycle tracking for adopted locations
- **[Project 52 - Volunteer Rewards](./Project_52_Volunteer_Rewards.md)** — Partner rewards that discovered organizations could contribute

---

**Last Updated:** March 4, 2026
**Owner:** Product & Engineering
**Status:** Not Started
**Next Review:** Q2 2026
