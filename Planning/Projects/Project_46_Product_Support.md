# Project 46 — Product Support

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phases 1-3 Complete) |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

TrashMob.eco is transitioning from a developer-built tool to a platform serving dozens of community partners, thousands of volunteers, and multiple stakeholder types (community admins, team leads, event organizers, sponsors, individual volunteers). As the platform grows, the gap between "feature exists" and "feature is successfully used" widens. Today there is no formal product support function — users who encounter issues either message the founder directly, post on social media, or silently churn.

A defined product support role ensures that users get help when they need it, feedback is captured systematically, bugs are triaged and prioritized, and community admins have a reliable partner for onboarding and ongoing success. Without this, growth will be capped by the founder's bandwidth, community satisfaction will decline, and engineering will lack the signal needed to prioritize effectively.

---

## Objectives

### Primary Goals
- Define the product support role and its responsibilities within the TrashMob organization
- Establish support channels and response-time expectations for different user types
- Create a triage workflow that routes user issues to the right team (engineering, product, community)
- Document onboarding support for new community partners
- Build a knowledge base of common questions, troubleshooting guides, and how-to articles

### Secondary Goals (Nice-to-Have)
- Define metrics to measure support effectiveness (response time, resolution time, satisfaction)
- Create a volunteer support contributor role for community members who want to help others
- Establish escalation paths for urgent issues (e.g., event day problems, data loss)
- Plan for self-service support tooling (help center, chatbot, in-app guidance)

---

## Scope

### Phase 1 — Define the Role & Responsibilities ✅
- [x] Document the product support role: what it covers, what it doesn't → `Support/PRODUCT_SUPPORT_ROLE.md`
- [x] Define user segments and their support needs (volunteers, community admins, team leads, sponsors) → covered in role doc and triage workflow
- [x] Map current support touchpoints (email, social media, GitHub issues, in-app feedback) → `Support/SUPPORT_CHANNELS_AND_TRIAGE.md`
- [x] Identify gaps in current support coverage → addressed across all support docs
- [x] Write a job description / role outline (volunteer or paid) → `Support/PRODUCT_SUPPORT_ROLE.md`

### Phase 2 — Support Channels & Workflows ✅
- [x] Establish primary support channel(s) (email, help desk tool, in-app) → `Support/SUPPORT_CHANNELS_AND_TRIAGE.md`
- [x] Define triage workflow: intake → categorize → route → resolve → close → `Support/SUPPORT_CHANNELS_AND_TRIAGE.md`
- [x] Set response-time targets by severity (critical: same day, high: 2 days, normal: 5 days) → SLA table in triage doc
- [x] Create escalation path for urgent issues (event-day problems, security, data loss) → escalation flowchart in triage doc + `Support/EVENT_DAY_PLAYBOOK.md`
- [x] Define handoff process between support and engineering (bug reports, feature requests) → GitHub Issue templates in triage doc

### Phase 3 — Knowledge Base & Onboarding ✅
- [x] Create FAQ / knowledge base covering common questions and troubleshooting → `Support/TROUBLESHOOTING.md` (11 feature areas, 30+ scenarios)
- [x] Write community admin onboarding guide (getting started, key features, best practices) → `Support/COMMUNITY_ADMIN_ONBOARDING.md`
- [x] Document "event day" support playbook (what to do if things break during an event) → `Support/EVENT_DAY_PLAYBOOK.md`
- [x] Create templates for common support responses → GitHub Issue templates (bug report, feature request) in `Support/SUPPORT_CHANNELS_AND_TRIAGE.md`

### Phase 4 — Metrics & Continuous Improvement (Operational — activate when support role is staffed)
- [ ] Define support KPIs (response time, resolution time, user satisfaction, ticket volume) — framework defined in `Support/SUPPORT_CHANNELS_AND_TRIAGE.md`
- [ ] Establish feedback loop: support insights → product roadmap input — weekly/biweekly sync agenda documented
- [ ] Quarterly support review process — process documented in triage doc
- [ ] Plan for scaling support as user base grows

---

## Out-of-Scope

- [ ] Building custom help desk software — use existing tools (e.g., GitHub Issues, email, or a free-tier help desk)
- [ ] 24/7 support coverage — TrashMob is volunteer-driven; support will follow reasonable response times
- [ ] Phone support — text-based channels only for initial scope
- [ ] Dedicated support engineering team — support routes issues to existing engineering volunteers
- [ ] Automated chatbot implementation — documented as a future enhancement, not built in this project

---

## Success Metrics

### Quantitative
- **Response time:** ≥ 80% of support requests receive initial response within target SLA
- **Resolution rate:** ≥ 70% of issues resolved without engineering escalation
- **Knowledge base coverage:** ≥ 20 articles covering top user questions within 3 months
- **Onboarding success:** New community admins complete onboarding checklist within 2 weeks

### Qualitative
- Community admins know where to go for help and feel supported
- Engineering team receives well-triaged bug reports with reproduction steps
- Product team has regular input from support on user pain points and feature requests
- Users who encounter problems have a clear path to resolution

---

## Dependencies

### Blockers (Must be complete before this project starts)
- None — this is an organizational/process project that can start immediately

### Enablers for Other Projects
- **Project 34 (User Feedback):** ✅ Complete — in-app feedback widget provides a support intake channel
- **Project 36 (Marketing Materials):** Better support enables better user onboarding and retention messaging
- **Project 45 (Community Showcase):** Onboarding support is critical for new communities enrolling via the showcase

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **No volunteer steps up for the role** | Medium | High | Start with founder handling support using documented processes; role is designed to be hand-off ready |
| **Support volume overwhelms capacity** | Medium | Medium | Knowledge base and FAQ reduce repeat questions; triage workflow prevents engineering overload |
| **Support feedback doesn't reach product/engineering** | Medium | High | Formalize feedback loop with regular sync (weekly or biweekly) between support and product |
| **Inconsistent support quality** | Low | Medium | Create response templates and escalation guides; periodic review of closed tickets |

---

## Implementation Plan

### Deliverables

This project produces **documentation and processes**, not code. Key deliverables:

1. ✅ **Product Support Role Document** — `Support/PRODUCT_SUPPORT_ROLE.md`
2. ✅ **Support Channel Guide** — `Support/SUPPORT_CHANNELS_AND_TRIAGE.md`
3. ✅ **Triage & Escalation Workflow** — included in Support Channel Guide (flowchart + templates)
4. ✅ **Community Admin Onboarding Guide** — `Support/COMMUNITY_ADMIN_ONBOARDING.md`
5. ✅ **Knowledge Base (Troubleshooting Guide)** — `Support/TROUBLESHOOTING.md` (11 sections, 30+ scenarios)
6. **Support Metrics Dashboard** — KPIs and review cadence documented; activate when role is staffed
7. ✅ **Event Day Playbook** — `Support/EVENT_DAY_PLAYBOOK.md`

### Technical Considerations

While this project is primarily organizational, some technical integration points exist:

- **Project 34 (User Feedback):** In-app feedback submissions could feed into the support triage workflow
- **Project 29 (Feature Usage Metrics):** Usage data helps support identify underused features and proactive outreach opportunities
- **GitHub Issues:** Continue using for bug tracking; add labels and templates for support-originated issues
- **Email (SendGrid):** Support responses can leverage existing email infrastructure

---

## Rollout Plan

### Phase 1 (Immediate)
- Document the role and current state of support
- Identify and fill the most critical gaps

### Phase 2 (1-2 months after Phase 1)
- Stand up support channels and triage workflow
- Begin tracking response times

### Phase 3 (2-3 months after Phase 1)
- Publish knowledge base
- Onboard first community using the new onboarding guide

### Phase 4 (Ongoing)
- Measure, review, and improve quarterly

---

## Open Questions

1. ~~**Should the support role be volunteer or paid?**~~
   **Resolved:** Start as a volunteer role with documented processes. Evaluate for paid support if ticket volume or community partner count justifies it. The in-app admin guide (`/siteadmin/guide`) and blog knowledge base reduce the support load, making a volunteer role feasible at current scale.

2. ~~**What tool should be used for support ticketing?**~~
   **Resolved:** GitHub Issues + email (info@trashmob.eco). Add issue templates for support-originated tickets (bug report, feature request, community admin help request). The in-app feedback widget (Project 34) feeds into this pipeline. Evaluate free-tier help desk tools (Freshdesk, HubSpot) only if volume exceeds what GitHub Issues can handle.

3. ~~**How should support interact with the community admin relationship?**~~
   **Resolved:** Single person handles both initially. Support = reactive issues (bug reports, how-to questions, event-day problems). Community success = proactive onboarding and check-ins. Split into separate roles only when workload justifies it. The admin guide page and onboarding documentation serve both functions.

---

## Related Documents

- **[Project 34 - User Feedback](./Project_34_User_Feedback.md)** — In-app feedback widget (support intake channel)
- **[Project 29 - Feature Usage Metrics](./Project_29_Feature_Usage_Metrics.md)** — Usage data for proactive support
- **[Project 36 - Marketing Materials](./Project_36_Marketing_Materials.md)** — Onboarding and retention messaging
- **[Project 45 - Community Showcase](./Project_45_Community_Showcase.md)** — Community enrollment funnel requiring onboarding support
- **[Support/](../../Support/)** — All support documentation deliverables

---

**Last Updated:** March 1, 2026
**Owner:** Director of Product & Engineering
**Status:** In Progress (Phases 1-3 Complete; Phase 4 activates when role is staffed)
**Next Review:** When volunteer picks up work
