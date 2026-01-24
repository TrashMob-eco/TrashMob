# Project Template — [PROJECT TITLE]

| Attribute | Value |
|-----------|-------|
| **Status** | [Not Started / Planning / Ready for Review / In Progress / Developers Engaged / Complete] |
| **Priority** | [Low / Medium / High / Critical] |
| **Risk** | [Low / Medium / High / Very High] |
| **Size** | [Very Small / Small / Medium / Large / Very Large] |
| **Dependencies** | [List of dependent projects] |

---

## Business Rationale

[Explain why this project matters for TrashMob's mission, growth, or quality goals. What problem does it solve? What value does it deliver?]

---

## Objectives

### Primary Goals
- [Goal 1]
- [Goal 2]
- [Goal 3]

### Secondary Goals (Nice-to-Have)
- [Optional goal 1]
- [Optional goal 2]

---

## Scope

### Phase 1 - [Phase Name]
- ? [Deliverable 1]
- ? [Deliverable 2]
- ? [Deliverable 3]

### Phase 2 - [Phase Name]
- ? [Deliverable 1]
- ? [Deliverable 2]

### Phase 3 - [Phase Name]
- ? [Deliverable 1]
- ? [Deliverable 2]

---

## Out-of-Scope

- ? [What we're explicitly not doing]
- ? [Features deferred to future phases]
- ? [Related but separate initiatives]

---

## Success Metrics

### Quantitative
- **[Metric 1]:** [Target value and current baseline]
- **[Metric 2]:** [Target value and current baseline]
- **[Metric 3]:** [Target value and current baseline]

### Qualitative
- [Qualitative success indicator 1]
- [Qualitative success indicator 2]

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
| **[Risk 2]** | [Low/Med/High] | [Low/Med/High/Critical] | [How to reduce or respond] |
| **[Risk 3]** | [Low/Med/High] | [Low/Med/High/Critical] | [How to reduce or respond] |

---

## Implementation Plan

### Data Model Changes
[Describe any database schema changes, new tables, columns, indexes, or migrations needed]

```sql
-- Example migration
ALTER TABLE Events ADD COLUMN [NewColumn] [DataType];
CREATE INDEX IX_Events_[NewColumn] ON Events([NewColumn]);
```

### API Changes
[Describe new or modified API endpoints]

```csharp
// Example endpoint
[HttpGet("api/v2/[resource]")]
public async Task<ActionResult<[ResponseType]>> GetResource([parameters])
{
    // Implementation
}
```

### Web UX Changes
[Describe new pages, components, or modifications to existing UI]

### Mobile App Changes
[Describe mobile-specific changes (if applicable)]

---

## Implementation Phases

### Phase 1: [Phase Name]
- [Activity 1]
- [Activity 2]
- [Activity 3]

### Phase 2: [Phase Name]
- [Activity 1]
- [Activity 2]

### Phase 3: [Phase Name]
- [Activity 1]
- [Activity 2]

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **[Question 1]**  
   **Recommendation:** [Your recommendation]  
   **Owner:** [Who should answer]  
   **Due:** [When decision is needed]

2. **[Question 2]**  
   **Recommendation:** [Your recommendation]  
   **Owner:** [Who should answer]  
   **Due:** [When decision is needed]

---

## Related Documents

- **[Related Project 1](./Project_XX_Name.md)** - [How it relates]
- **[Related Project 2](./Project_XX_Name.md)** - [How it relates]
- **[Technical Design Doc]** - (Link when created)
- **[User Stories]** - (Link to PRD sections)

---

**Last Updated:** [Date]  
**Owner:** [Team/Person responsible]  
**Status:** [Current status]  
**Next Review:** [When to review again]
