# Project 31 — Feature Flags

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Feature flags enable safer deployments by decoupling code deployment from feature release. They allow the team to:
- Gradually roll out features to subsets of users
- Quickly disable problematic features without redeployment
- A/B test new functionality
- Enable features for specific communities or user segments

This capability is referenced as a mitigation strategy in multiple project risks (PR-1, SR-2) and supports the overall goal of reducing deployment risk.

---

## Objectives

### Primary Goals
- Implement a feature flag system that works across web and mobile platforms
- Enable runtime toggling of features without code deployment
- Support targeting rules (user segments, percentages, specific users)

### Secondary Goals (Nice-to-Have)
- Admin UI for managing flags without code changes
- Audit logging for flag changes
- Integration with analytics to measure feature impact

---

## Scope

### Phase 1 - Core Infrastructure
- ☐ Select and integrate feature flag provider (Azure App Configuration, LaunchDarkly, or custom)
- ☐ Implement server-side flag evaluation in .NET backend
- ☐ Create flag configuration storage and management
- ☐ Add feature flag middleware/service to API

### Phase 2 - Client Integration
- ☐ Implement React hook/context for feature flags
- ☐ Implement MAUI service for feature flags
- ☐ Add flag caching and offline support for mobile
- ☐ Create developer documentation

### Phase 3 - Management & Operations
- ☐ Build admin UI for flag management (or configure provider dashboard)
- ☐ Implement targeting rules (user %, community, role-based)
- ☐ Add audit logging for flag changes
- ☐ Document operational procedures

---

## Out-of-Scope

- ❌ Full A/B testing platform with statistical analysis
- ❌ Feature flag as a service for external consumers
- ❌ Complex experimentation workflows

---

## Success Metrics

### Quantitative
- **Flag evaluation latency:** < 10ms server-side, < 50ms client-side
- **Adoption:** At least 3 features using flags within 3 months of launch
- **Availability:** 99.9% uptime for flag service

### Qualitative
- Engineers confident using flags for new feature rollouts
- Reduced anxiety around deployments

---

## Dependencies

### Blockers (Must be complete before this project starts)
- None

### Enablers for Other Projects (What this unlocks)
- **Project 1 (Auth Revamp):** Gradual rollout of new auth system
- **Project 24 (API v2):** Control v2 endpoint availability
- **All projects:** Safer feature releases

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Flag service becomes single point of failure** | Medium | High | Implement fallback defaults; cache flag values |
| **Flag sprawl / technical debt** | Medium | Medium | Establish flag lifecycle policy; remove flags after full rollout |
| **Inconsistent flag state across platforms** | Low | Medium | Centralized flag storage; consistent SDK behavior |

---

## Implementation Plan

### Technology Options

**Option A: Azure App Configuration (Recommended)**
- Native Azure integration
- Built-in feature flag support
- Cost-effective for current scale
- Supports targeting and filters

**Option B: LaunchDarkly**
- Industry-leading feature flag platform
- Rich targeting and analytics
- Higher cost but more features

**Option C: Custom Implementation**
- Full control
- No additional cost
- Higher maintenance burden

### Backend Implementation

```csharp
// Feature flag service interface
public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(string flagName, Guid? userId = null);
    Task<T> GetValueAsync<T>(string flagName, T defaultValue);
}

// Usage in controller
[HttpGet("api/events")]
public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
{
    if (await _featureFlags.IsEnabledAsync("new-event-list"))
    {
        return await _eventManager.GetEventsV2Async();
    }
    return await _eventManager.GetEventsAsync();
}
```

### Frontend Implementation

```typescript
// React hook
const { isEnabled, isLoading } = useFeatureFlag('new-event-list');

// Usage in component
{isEnabled('new-dashboard') ? <NewDashboard /> : <Dashboard />}
```

### Mobile Implementation

```csharp
// MAUI service
public class FeatureFlagService : IFeatureFlagService
{
    public async Task<bool> IsEnabledAsync(string flagName)
    {
        // Check cached value first, then remote
    }
}
```

---

## Implementation Phases

### Phase 1: Core Infrastructure
- Evaluate and select feature flag provider
- Set up Azure App Configuration (or chosen provider)
- Implement .NET feature flag service
- Add to dependency injection
- Create initial flags for testing

### Phase 2: Client Integration
- Create React feature flag context/hook
- Create MAUI feature flag service
- Implement caching strategy
- Write developer documentation
- Add sample usage to one feature

### Phase 3: Management & Operations
- Configure admin access to flag management
- Implement targeting rules
- Set up audit logging
- Create flag lifecycle documentation
- Train team on usage

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **Which feature flag provider should we use?**
   **Recommendation:** Azure App Configuration - native Azure integration, cost-effective
   **Owner:** Engineering Lead
   **Due:** Before Phase 1 begins

2. **Should flags be evaluated server-side only, or also client-side?**
   **Recommendation:** Both - server for API behavior, client for UI variations
   **Owner:** Engineering Lead
   **Due:** Phase 1

3. **What is the flag naming convention?**
   **Recommendation:** kebab-case with feature prefix (e.g., `events-new-list`, `auth-entra-id`)
   **Owner:** Engineering Team
   **Due:** Phase 1

---

## Related Documents

- **[Risks & Mitigations](../Risks_and_Mitigations.md)** - PR-1, SR-2 reference feature flags
- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** - Will use flags for rollout
- **[Project 24 - API v2](./Project_24_API_v2_Modernization.md)** - Will use flags for migration

---

**Last Updated:** January 26, 2026
**Owner:** Engineering Lead
**Status:** Not Started
**Next Review:** When volunteer available
