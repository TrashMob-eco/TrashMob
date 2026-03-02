# Support Channels & Triage Workflow

## Support Channels

### Primary Channels

| Channel | URL / Address | Best For |
|---------|---------------|----------|
| **Email** | info@trashmob.eco | General questions, community admin help, partnership inquiries |
| **GitHub Issues** | github.com/TrashMob-eco/TrashMob/issues | Bug reports, feature requests, technical issues |
| **In-App Feedback** | Feedback widget (bottom-right of web app) | Quick bug reports, usability feedback, feature suggestions |

### Channel Guidelines

- **Email** — Preferred channel for community admins and non-technical users. Provides a personal touch for onboarding and relationship building.
- **GitHub Issues** — Preferred for anything technical: bugs with reproduction steps, feature requests with user stories, or improvements to documentation.
- **In-App Feedback** — Low-friction channel for quick reports. Support should review submissions regularly and create GitHub Issues for actionable items.

## Response Time Targets (SLA)

| Severity | Examples | Initial Response | Resolution Target |
|----------|----------|------------------|-------------------|
| **Critical** | Event day failures, data loss, security issues, site down | Same day | 24 hours |
| **High** | Feature broken for multiple users, new community blocked from onboarding | 1 business day | 3 business days |
| **Normal** | Single-user bug, how-to question, feature request | 2 business days | 5 business days |
| **Low** | Cosmetic issues, minor UX suggestions, documentation typos | 5 business days | Best effort |

> **Note:** These are targets, not guarantees. TrashMob is volunteer-driven and support operates on a best-effort basis. The goal is to set expectations and prioritize effectively.

## Triage Workflow

### Step 1: Intake

When a support request arrives (email, GitHub Issue, or in-app feedback):

1. **Acknowledge receipt** — Send a brief reply confirming the request was received and provide an estimated timeframe based on severity.
2. **Categorize** the request (see categories below).
3. **Assign severity** based on the impact table above.

### Step 2: Categorize

| Category | Description | Route To |
|----------|-------------|----------|
| **Bug Report** | Something is broken or not working as expected | Support attempts to reproduce → if confirmed, create GitHub Issue with `bug` + `support-originated` labels |
| **How-To Question** | User needs help using a feature | Support answers directly using knowledge base / admin guide; update knowledge base if question is new |
| **Feature Request** | User wants something the platform doesn't do yet | Create GitHub Issue with `enhancement` + `support-originated` labels |
| **Community Admin Help** | Onboarding, configuration, or operational question from a community admin | Support handles directly using onboarding guide; escalate complex issues |
| **Account / Access Issue** | Login problems, permission errors, account setup | Support checks common causes (see Troubleshooting); escalate to engineering if auth-related |
| **Event Day Emergency** | Something broke during a live cleanup event | Follow [Event Day Playbook](EVENT_DAY_PLAYBOOK.md) |

### Step 3: Resolve

- **Answer directly** if the issue is covered by the knowledge base, troubleshooting guide, or admin guide.
- **Reproduce and document** if it's a potential bug. Include: steps to reproduce, expected vs. actual behavior, screenshots, browser/device info, user ID if available.
- **Create a GitHub Issue** for confirmed bugs or feature requests. Use the templates below.
- **Escalate to engineering** for issues that require code changes, data fixes, or infrastructure investigation.

### Step 4: Close

- Confirm with the user that the issue is resolved.
- For bugs routed to engineering, follow up when the fix is deployed and notify the user.
- Update the knowledge base if the resolution reveals a common issue that should be documented.

## Escalation Path

```
User Report
    │
    ▼
Support (Triage & First Response)
    │
    ├── Resolved directly ──────────► Close & document
    │
    ├── Bug confirmed ──────────────► GitHub Issue (bug label) → Engineering
    │
    ├── Feature request ────────────► GitHub Issue (enhancement label) → Product backlog
    │
    ├── Event day emergency ────────► Event Day Playbook → Engineering if needed
    │
    └── Security / data loss ───────► Immediate escalation to Director of Product & Engineering
```

## GitHub Issue Templates

### Bug Report (Support-Originated)

```markdown
**Title:** [Brief description of the bug]

**Reported by:** [User type: volunteer / community admin / partner]
**Reported via:** [Email / In-app feedback / GitHub]
**Date reported:** [YYYY-MM-DD]

**Description:**
[What the user reported]

**Steps to Reproduce:**
1. [Step 1]
2. [Step 2]
3. [Step 3]

**Expected Behavior:**
[What should happen]

**Actual Behavior:**
[What actually happens]

**Environment:**
- Browser/Device: [e.g., Chrome 120 on Windows 11, iPhone 15 iOS 18]
- App version: [Web / Mobile v1.x]
- User role: [Volunteer / Community Admin / Site Admin]

**Screenshots:**
[Attach if available]

**Labels:** `bug`, `support-originated`, `[severity]`
```

### Feature Request (Support-Originated)

```markdown
**Title:** [Brief description of the request]

**Requested by:** [User type: volunteer / community admin / partner]
**Reported via:** [Email / In-app feedback]
**Date reported:** [YYYY-MM-DD]
**Number of users requesting:** [1 / multiple — note if recurring]

**User Story:**
As a [user type], I want to [action] so that [benefit].

**Current Workaround:**
[How the user is handling this today, if applicable]

**Labels:** `enhancement`, `support-originated`
```

## Feedback Loop: Support → Product

### Weekly/Biweekly Sync Agenda

1. **Open issues summary** — Count by category and severity, any aging items
2. **Top user pain points** — Recurring themes from the past week
3. **Feature requests** — New requests and vote counts on existing ones
4. **Knowledge base gaps** — Questions that couldn't be answered with existing docs
5. **Metrics review** — Response times, resolution rates, ticket volume trends

### Quarterly Review

- Review support KPIs (response time, resolution rate, volume trends)
- Identify top 5 pain points for product roadmap input
- Update troubleshooting guide and knowledge base based on ticket patterns
- Evaluate whether support capacity matches demand

---

**Last Updated:** March 1, 2026
