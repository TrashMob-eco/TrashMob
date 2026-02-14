# Planning System Adoption Guide

**For non-profit organizations adopting the TrashMob project planning system.**

This guide tells an AI assistant (Claude, Copilot, etc.) how to set up this planning system in a new repository. Follow the steps in order.

---

## What This System Is

A markdown-based project planning system designed for volunteer-driven non-profits. It uses structured project files, a central navigation hub, AI assistant commands, and consistent conventions to track dozens of initiatives without any paid tools.

**Key benefits:**
- No paid project management software required (everything lives in your repo)
- AI assistants can create, update, and query projects via natural language
- Standardized templates ensure consistency across all contributors
- Phase-based tracking works well for volunteer teams with unpredictable availability

---

## Step 1: Create the Directory Structure

```
Planning/
  README.md                          # Navigation hub (you'll customize this)
  Executive_Summary.md               # Strategic overview (you'll write this)
  Risks_and_Mitigations.md           # Cross-project risks (you'll write this)
  Projects/
    _Project_Template.md             # Standard template for all projects
  TechnicalDesigns/                  # Optional: deep-dive architecture docs
  .claude_commands_index.md          # AI assistant command index
  .claude_add_project_command.md     # Detailed "add project" workflow
  .claude_add_project_quick.md       # Quick checklist for adding projects
  .claude_common_commands.md         # Common operations (status, deps, risks)
  .claude_command_system_summary.md  # Overview of the command system
```

Create `Planning/` and `Planning/Projects/` and `Planning/TechnicalDesigns/` directories in the repository root.

---

## Step 2: Copy and Customize the Project Template

Copy the file `Planning/Projects/_Project_Template.md` as-is. This is the foundation of the entire system. Every project file follows this structure:

```markdown
# Project ## — [PROJECT TITLE]

| Attribute | Value |
|-----------|-------|
| **Status** | [Not Started / Planning / Ready for Review / In Progress / Developers Engaged / Complete] |
| **Priority** | [Low / Medium / High / Critical] |
| **Risk** | [Low / Medium / High / Very High] |
| **Size** | [Very Small / Small / Medium / Large / Very Large] |
| **Dependencies** | [List of dependent projects] |

---

## Business Rationale
[Why this project matters for the organization's mission]

---

## Objectives

### Primary Goals
- [Goal 1]
- [Goal 2]
- [Goal 3]

### Secondary Goals (Nice-to-Have)
- [Optional goal 1]

---

## Scope

### Phase 1 - [Phase Name]
- ☐ [Deliverable 1]
- ☐ [Deliverable 2]

### Phase 2 - [Phase Name]
- ☐ [Deliverable 1]

---

## Out-of-Scope
- ☐ [What we're explicitly not doing]

---

## Success Metrics

### Quantitative
- **[Metric 1]:** [Target value and current baseline]

### Qualitative
- [Qualitative success indicator]

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **[Project/Resource]:** [Why it's required]

### Enablers for Other Projects (What this unlocks)
- **[Dependent Project]:** [How this project helps]

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **[Risk 1]** | [Low/Med/High] | [Low/Med/High/Critical] | [How to reduce or respond] |

---

## Implementation Plan

### Data Model Changes
[Schema changes, migrations]

### API Changes
[New or modified endpoints]

### UX Changes
[New pages, components, UI modifications]

---

## Implementation Phases

### Phase 1: [Phase Name]
- [Activity 1]
- [Activity 2]

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **[Question 1]**
   **Recommendation:** [Your recommendation]
   **Owner:** [Who should answer]
   **Due:** [When decision is needed]

---

## Related Documents

- **[Related Project](./Project_XX_Name.md)** - [How it relates]

---

**Last Updated:** [Date]
**Owner:** [Team/Person responsible]
**Status:** [Current status]
**Next Review:** [When to review again]
```

**No customization needed** for the template itself. It's intentionally generic.

---

## Step 3: Create the README Navigation Hub

Create `Planning/README.md`. This is the central index. Customize the structure below for your organization:

```markdown
# [Your Organization] — [Year] Planning Documentation

**Version:** 1.0
**Date:** [Date]
**Owner:** [Role or person]

---

## Navigation

### Core Documents

- **[Executive Summary](./Executive_Summary.md)** - High-level overview and roadmap
- **[Risks & Mitigations](./Risks_and_Mitigations.md)** - Cross-project risks

### [Year] Projects ([N] Total)

**Note:** Projects are not time-bound. Volunteers pick up work based on priority and availability.

#### Critical Priority (Infrastructure & Blockers)

| Project | Description | Status |
|---------|-------------|--------|
| [Project 1 - Title](./Projects/Project_01_Title.md) | Brief description | Not Started |

#### High Priority (Core Features)

| Project | Description | Status |
|---------|-------------|--------|

#### Medium Priority (Enhancements)

| Project | Description | Status |
|---------|-------------|--------|

#### Low Priority (Nice-to-Have)

| Project | Description | Status |
|---------|-------------|--------|

---

## Quick Links by Theme

Organize projects into groups that make sense for your domain. Examples:

### [Theme 1 — e.g. "Safety & Compliance"]
- [Project 1 - Title](./Projects/Project_01_Title.md)

### [Theme 2 — e.g. "User Experience"]
- [Project 2 - Title](./Projects/Project_02_Title.md)

### [Theme 3 — e.g. "Infrastructure"]
- [Project 3 - Title](./Projects/Project_03_Title.md)

---

## Project Status Overview

| Status | Count | Projects |
|--------|-------|----------|
| ✅ **Complete** | 0 | — |
| **In Progress** | 0 | — |
| **Ready for Review** | 0 | — |
| **Planning** | 0 | — |
| **Not Started** | 0 | — |
| **Deprioritized** | 0 | — |

**Total:** 0 project specifications documented

---

## Related Documents

- **[AI Assistant Commands Index](./.claude_commands_index.md)** - Guide to all AI commands
- **[Add New Project Command](./.claude_add_project_command.md)** - Adding projects
- **[Common Commands](./.claude_common_commands.md)** - Status, dependencies, risks

---

## Document Conventions

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
- **High:** Important for [year] goals
- **Medium:** Should be done but flexible timing
- **Low:** Nice-to-have, can defer

---

**Last Updated:** [Date]
**Maintained By:** [Team]
```

**Key rules to preserve:**
- The status overview table counts must always add up to the total
- Every project appears in exactly one priority section in the main table
- Every project appears in at least one theme in Quick Links
- Status descriptions in the table should include phase details (e.g., "In Progress (Phase 2 Complete)")

---

## Step 4: Copy the AI Assistant Command Files

Copy these 5 files from `Planning/` into your `Planning/` directory:

| File | Purpose | Customization Needed |
|------|---------|---------------------|
| `.claude_commands_index.md` | Master index of all commands | Update org name, file references |
| `.claude_add_project_command.md` | 9-step workflow for adding projects | Update org name, remove TrashMob-specific references, update code language examples to match your stack |
| `.claude_add_project_quick.md` | Quick checklist for adding projects | Update org name |
| `.claude_common_commands.md` | 20+ common operations | Update org name |
| `.claude_command_system_summary.md` | Overview and metrics | Update org name and statistics |

### Customization checklist for each command file:

1. **Replace organization name:** Change "TrashMob" to your organization name throughout
2. **Replace tech stack references:** The originals reference C# (.NET), TypeScript (React), SQL, and Azure. Replace code examples with your stack
3. **Remove domain-specific references:** Remove mentions of events, litter reports, communities, etc. and replace with your domain concepts
4. **Update file path references:** If you renamed any files, update internal links
5. **Remove the `TODO_Project_Extraction.md` references** — that file was specific to TrashMob's initial setup. You won't need it unless you're extracting projects from an existing backlog

---

## Step 5: Create the Executive Summary

Create `Planning/Executive_Summary.md`. This is your strategic overview. Use this structure:

```markdown
# [Your Organization] — [Year] Executive Summary

**Version:** 1.0
**Date:** [Date]
**Owner:** [Role]

---

## Purpose

[2-3 sentences: What are you trying to accomplish this year?]

---

## Strategic Objectives

| # | Objective | Description |
|---|-----------|-------------|
| 1 | **[Objective]** | [Description] |
| 2 | **[Objective]** | [Description] |
| 3 | **[Objective]** | [Description] |

---

## Key Feature Themes

### [Theme 1]
- Bullet points of planned features/initiatives

### [Theme 2]
- Bullet points

---

**Last Updated:** [Date]
```

---

## Step 6: Create the Risks Document

Create `Planning/Risks_and_Mitigations.md` for cross-cutting risks that span multiple projects:

```markdown
# Cross-Project Risks & Mitigations

**Version:** 1.0
**Date:** [Date]

---

## Risk Dashboard

| ID | Risk | Priority | Status | Last Updated |
|----|------|----------|--------|-------------|
| PR-1 | [Risk title] | [High/Med/Low] | Open | [Date] |

---

## PR-1: [Risk Title]

**Category:** [Technical / Organizational / External]
**Likelihood:** [Low / Medium / High]
**Impact:** [Low / Medium / High / Critical]
**Affected Projects:** [List]

**Mitigations:**
- [Mitigation 1]
- [Mitigation 2]

**Owner:** [Person/Team]

---

**Last Updated:** [Date]
```

---

## Step 7: Add CLAUDE.md Integration

Add a section to your repository's `CLAUDE.md` (or create one) that points AI assistants to the planning system:

```markdown
## Planning System

Refer to `Planning/README.md` for the project roadmap and navigation hub.

### Key Planning Files

| Purpose | Location |
|---------|----------|
| Navigation Hub | `Planning/README.md` |
| Project Template | `Planning/Projects/_Project_Template.md` |
| AI Commands Index | `Planning/.claude_commands_index.md` |
| Add Project Guide | `Planning/.claude_add_project_command.md` |
| Common Commands | `Planning/.claude_common_commands.md` |

### AI Assistant Boundaries (Planning)

- Do not make autonomous structural changes to project plans (moving features between projects, removing rollout strategies, reorganizing phases) unless explicitly asked
- When updating planning docs, always verify status overview counts add up to the total
- Do not move features between projects or remove rollout strategies without explicit approval
```

This ensures any Claude instance working in your repo knows the planning system exists and how to interact with it.

---

## Step 8: Create Your First Project

Test the system by creating Project 1:

1. Create `Planning/Projects/Project_01_[Title].md`
2. Copy the contents of `_Project_Template.md`
3. Fill in all sections with your first initiative
4. Update `Planning/README.md`:
   - Add the project to the appropriate priority table
   - Add it to at least one theme in Quick Links
   - Update the status overview count
5. Verify everything links correctly

Or simply tell your AI assistant: **"Add a new project for [description]"** — it will follow the `.claude_add_project_command.md` workflow automatically.

---

## Conventions to Maintain

These conventions keep the system healthy as it grows:

### Naming

- Project files: `Project_##_Descriptive_Title.md` (two-digit numbers, underscores for spaces)
- Claude command files: `.claude_[purpose].md` (dot-prefixed to sort separately)
- Technical designs: `TechnicalDesigns/[Topic].md`

### Status Tracking

- Phase completion uses checkboxes: `✅` (done) and `☐` (pending)
- README status entries include phase details: `"In Progress (Phases 1-3 Complete)"`
- Status table counts must always sum to the project total

### Cross-References

- Projects link to related projects: `[Project XX - Name](./Project_XX_Name.md)`
- Dependencies are bidirectional: if A blocks B, update both A (Enablers) and B (Blockers)
- Use relative paths for all internal links

### Footer

Every project file ends with:
```markdown
**Last Updated:** [Date]
**Owner:** [Team/Person]
**Status:** [Current status]
**Next Review:** [Date]
```

Update the date whenever the file is modified.

---

## Files Summary

### Must Create (your content)

| File | Description |
|------|-------------|
| `Planning/README.md` | Navigation hub — customize from Step 3 template |
| `Planning/Executive_Summary.md` | Strategic overview — customize from Step 5 template |
| `Planning/Risks_and_Mitigations.md` | Cross-project risks — customize from Step 6 template |

### Copy As-Is (no changes needed)

| File | Description |
|------|-------------|
| `Planning/Projects/_Project_Template.md` | Standard project file template |

### Copy and Customize (replace org name, tech stack, domain terms)

| File | Description |
|------|-------------|
| `Planning/.claude_commands_index.md` | AI command master index |
| `Planning/.claude_add_project_command.md` | Detailed "add project" workflow |
| `Planning/.claude_add_project_quick.md` | Quick add checklist |
| `Planning/.claude_common_commands.md` | 20+ common operations |
| `Planning/.claude_command_system_summary.md` | System overview and metrics |

### Create as Needed

| File | Description |
|------|-------------|
| `Planning/Projects/Project_01_*.md` | Your first project |
| `Planning/TechnicalDesigns/*.md` | Deep-dive architecture docs for complex projects |

---

## Optional Extras

These files existed in the original TrashMob system but are **not required** for the core planning system to work:

| File | Purpose | When to Add |
|------|---------|-------------|
| `MOBILE_APP_MASTER_PLAN.md` | Platform-specific planning | If you have a mobile app |
| `PRODUCTION_DEPLOYMENT_CHECKLIST.md` | Go-live procedures | When you have production deployments |
| `Cross_Cutting_Minor_Privacy_Standards.md` | Privacy standards for minors | If your platform serves minors |
| `Orphan_Issues.md` | Track unlinked GitHub issues | When you have many open issues to triage |

---

## Troubleshooting

**AI assistant doesn't follow the command files:**
Make sure your `CLAUDE.md` references the planning system. AI assistants need to know the files exist to use them.

**Status counts don't add up:**
Always verify the status overview table sums to the total project count after any status change. This is the most common mistake.

**Projects get out of date:**
Add "Next Review" dates to project footers and periodically audit. The status report command in `.claude_common_commands.md` helps with this.

**Too many projects, hard to navigate:**
Use the Quick Links by Theme section aggressively. Group projects by domain concept, not by timeline.

---

**Source:** [TrashMob.eco](https://github.com/TrashMob-eco/TrashMob) Planning System
**License:** This planning system structure is freely available for any non-profit to adopt.
**Questions:** Open an issue at https://github.com/TrashMob-eco/TrashMob/issues
