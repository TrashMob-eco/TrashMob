# Project 40 — AI Community Sales Agent

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | Project 10 (Community Pages), Project 1 (Auth Revamp for SSO onboarding) |

---

## Business Rationale

TrashMob's growth depends on signing up communities (cities, counties, nonprofits, HOAs) that bring organized groups of volunteers and sustained engagement. Today, community outreach is entirely manual — finding potential communities, sending introductory emails, following up, and guiding them through onboarding. This doesn't scale.

An AI-powered sales agent can automate community discovery, personalized outreach, follow-up cadences, and onboarding assistance — dramatically increasing the pipeline of new community partners while reducing the manual effort required from the small TrashMob team. This is a force multiplier for growth.

---

## Objectives

### Primary Goals
- **Discover communities:** Automatically identify and prioritize potential community partners (municipalities, environmental nonprofits, HOAs, civic organizations) based on geography, population, existing cleanup activity, and fit
- **Automate outreach:** Generate and send personalized introductory communications explaining TrashMob's value proposition
- **Track engagement:** Monitor responses, opens, clicks, and reply sentiment; maintain a CRM-style pipeline
- **Follow up intelligently:** Automate follow-up sequences based on response status, with escalation to human when needed
- **Onboard communities:** Guide interested communities through the signup and configuration process (community page setup, branding, waiver configuration)

### Secondary Goals (Nice-to-Have)
- Identify seasonal outreach windows (e.g., Earth Day, Great American Cleanup)
- Generate custom pitch decks or one-pagers per community
- Integrate with existing CRM tools (HubSpot, Salesforce) if adopted later
- Analyze competitor/peer community programs for differentiation

---

## Scope

### Phase 1 - Community Discovery & Database
- ☐ Build a community prospect database schema (name, type, location, population, contact info, website, social media, status)
- ☐ Implement web research agent to discover potential communities from public data (city websites, environmental org directories, existing TrashMob event locations without communities)
- ☐ Score and rank prospects by fit (population, geographic coverage gaps, existing cleanup activity in area)
- ☐ Admin UI to review, approve, and manage prospect pipeline

### Phase 2 - Outreach & Communication
- ☐ Design email templates for initial outreach, follow-ups, and onboarding
- ☐ AI agent generates personalized outreach based on community profile (city name, local events, environmental initiatives)
- ☐ Integrate with SendGrid for email delivery with tracking (opens, clicks, replies)
- ☐ Implement follow-up cadence engine (Day 1: intro, Day 4: follow-up, Day 10: value add, Day 21: final follow-up)
- ☐ Reply detection and sentiment analysis to route responses appropriately

### Phase 3 - Pipeline Management & CRM
- ☐ Pipeline dashboard showing prospects by stage (New, Contacted, Responded, Interested, Onboarding, Active, Declined)
- ☐ Activity timeline per prospect (emails sent, responses, notes, status changes)
- ☐ Notification system for human handoff (interested responses, questions, objections)
- ☐ Reporting: conversion rates, outreach effectiveness, geographic coverage

### Phase 4 - Onboarding Assistant
- ☐ AI-guided onboarding flow for communities that express interest
- ☐ Auto-generate community page draft from prospect data (name, location, branding colors from website)
- ☐ Waiver template suggestions based on community type
- ☐ Welcome email sequences for newly onboarded communities
- ☐ Success metrics tracking for onboarded communities (events created, volunteers joined)

---

## Out-of-Scope

- ❌ Phone/voice outreach (email and web only)
- ❌ Paid advertising or ad management
- ❌ Legal contract negotiation or partnership agreements
- ❌ Financial transactions or payment processing
- ❌ Replacing human relationship management for high-value prospects

---

## Success Metrics

### Quantitative
- **Community pipeline:** ≥ 100 qualified prospects identified per month
- **Outreach volume:** ≥ 50 personalized emails sent per week
- **Response rate:** ≥ 15% response rate on initial outreach
- **Conversion rate:** ≥ 5% of contacted prospects become active communities within 90 days
- **Time to onboard:** Reduce average onboarding time from 2 weeks to 3 days

### Qualitative
- Communities report a smooth, professional onboarding experience
- AI-generated outreach is indistinguishable from human-written emails
- Sales team (when hired) can focus on high-touch relationships rather than prospecting

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Project 10 (Community Pages):** ✅ Complete — Communities need a destination to onboard into
- **SendGrid integration:** Already in place for transactional email

### Enablers for Other Projects
- **Project 36 (Marketing Materials):** Sales agent can distribute marketing content
- **Project 38 (Mobile Feature Parity):** More community features drive mobile adoption

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Spam perception** | Medium | High | Careful rate limiting; CAN-SPAM compliance; easy opt-out; warm up sending domain; personalization quality |
| **Low-quality AI outreach** | Medium | Medium | Human review of initial templates; A/B testing; sentiment analysis on responses; continuous prompt tuning |
| **Data privacy concerns** | Low | High | Only use publicly available contact information; comply with GDPR/CAN-SPAM; clear privacy policy |
| **Community contact info hard to find** | Medium | Medium | Multiple data sources; manual enrichment for high-priority prospects; web scraping of public directories |
| **AI hallucination in outreach** | Low | High | Template-based generation with strict guardrails; human review queue for first N emails per template; factual claims limited to verified data |

---

## Implementation Plan

### Data Model Changes

**New Entity: CommunityProspect**
```csharp
public class CommunityProspect : KeyedModel
{
    public string Name { get; set; }
    public string Type { get; set; }  // Municipality, Nonprofit, HOA, CivicOrg, Other
    public string City { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? Population { get; set; }
    public string Website { get; set; }
    public string ContactEmail { get; set; }
    public string ContactName { get; set; }
    public string ContactTitle { get; set; }
    public int PipelineStage { get; set; }  // New, Contacted, Responded, Interested, Onboarding, Active, Declined
    public int FitScore { get; set; }  // 0-100
    public string Notes { get; set; }
    public DateTimeOffset? LastContactedDate { get; set; }
    public DateTimeOffset? NextFollowUpDate { get; set; }
    public Guid? ConvertedPartnerId { get; set; }  // FK to Partner when converted
}
```

**New Entity: ProspectActivity**
```csharp
public class ProspectActivity : KeyedModel
{
    public Guid ProspectId { get; set; }
    public string ActivityType { get; set; }  // EmailSent, EmailOpened, EmailClicked, Reply, StatusChange, Note
    public string Subject { get; set; }
    public string Details { get; set; }
    public string SentimentScore { get; set; }  // Positive, Neutral, Negative (for replies)
}
```

### API Changes

```csharp
// Prospect management
[HttpGet("api/admin/prospects")]
public async Task<ActionResult<PaginatedList<CommunityProspect>>> GetProspects(
    [FromQuery] ProspectFilter filter)

[HttpPost("api/admin/prospects/{prospectId}/outreach")]
public async Task<ActionResult> SendOutreach(Guid prospectId)

[HttpGet("api/admin/prospects/pipeline")]
public async Task<ActionResult<PipelineSummary>> GetPipelineSummary()

// AI agent endpoints
[HttpPost("api/agent/discover")]
public async Task<ActionResult<DiscoveryResult>> RunDiscovery(
    [FromBody] DiscoveryRequest request)

[HttpPost("api/agent/generate-outreach")]
public async Task<ActionResult<OutreachDraft>> GenerateOutreach(
    Guid prospectId)
```

### Web UX Changes
- **Admin > Sales Pipeline** page with Kanban-style board (prospects by stage)
- **Prospect Detail** page with activity timeline, contact info, outreach history
- **Outreach Templates** management page
- **Discovery Dashboard** showing geographic coverage and gaps
- **Reporting** page with conversion funnels and outreach effectiveness charts

### Mobile App Changes
None — this is an admin/internal tool only.

---

## Implementation Phases

### Phase 1: Foundation
- Database schema and migrations
- Basic CRUD for prospects
- Admin UI for prospect management
- Manual prospect entry and CSV import

### Phase 2: AI Discovery
- Web research agent using Claude API for community discovery
- Prospect scoring algorithm
- Geographic gap analysis
- Auto-population of prospect database

### Phase 3: Outreach Engine
- Email template system with AI personalization
- SendGrid integration for tracked sending
- Follow-up cadence automation
- Reply detection and routing

### Phase 4: Intelligence & Onboarding
- Pipeline analytics and reporting
- Sentiment analysis on responses
- AI-assisted onboarding flow
- Success tracking for converted communities

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **Which AI model/service for outreach generation?**
   **Recommendation:** Claude API (Anthropic) for generation quality and safety guardrails; already familiar from Claude Code usage
   **Owner:** Engineering Lead
   **Due:** Before Phase 2

2. **Should outreach come from a dedicated sales email or individual team member?**
   **Recommendation:** Dedicated sales email (e.g., communities@trashmob.eco) for consistency and deliverability management
   **Owner:** Product Owner
   **Due:** Before Phase 2

3. **What is the acceptable daily sending volume to avoid spam flags?**
   **Recommendation:** Start at 10/day, warm up over 4 weeks to 50/day; use dedicated subdomain for outreach
   **Owner:** Engineering Lead
   **Due:** Before Phase 3

4. **How should we handle prospects in regions with GDPR requirements?**
   **Recommendation:** Initially focus on US communities only; add GDPR-compliant flows in Phase 4
   **Owner:** Product Owner
   **Due:** Before Phase 1

---

## Related Documents

- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - The destination for onboarded communities
- **[Project 36 - Marketing Materials](./Project_36_Marketing_Materials.md)** - Content the agent can distribute
- **[Project 13 - Bulk Email Invites](./Project_13_Bulk_Email_Invites.md)** - SendGrid integration patterns to reuse

---

**Last Updated:** February 6, 2026
**Owner:** Product & Engineering
**Status:** Not Started
**Next Review:** Q2 2026
