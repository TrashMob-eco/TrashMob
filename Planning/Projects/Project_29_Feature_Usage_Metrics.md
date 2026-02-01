# Project 29 — Feature Usage Metrics for Web App

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Understanding how users interact with TrashMob features is critical for making data-driven product decisions. Currently, we have basic telemetry through Application Insights but lack structured feature usage tracking. Adding explicit feature metrics will help:

- Identify which features are most/least used
- Understand user journeys and drop-off points
- Prioritize development efforts based on actual usage
- Measure the success of new feature launches
- Support grant applications with usage data

---

## Objectives

### Primary Goals
- Implement structured feature usage tracking across the web application
- Create dashboards to visualize feature adoption and engagement
- Track key user journeys (signup → first event → attendance)
- Measure feature-specific metrics (events created, litter reports, team joins)

### Secondary Goals (Nice-to-Have)
- A/B testing infrastructure for feature experiments
- Cohort analysis capabilities
- Export metrics for external reporting

---

## Scope

### Phase 1 - Core Infrastructure
- ✅ Define standard event taxonomy and naming conventions
- ✅ Create React hook/context for consistent event tracking
- ✅ Implement backend API for custom metrics ingestion
- ✅ Configure Application Insights custom events

### Phase 2 - Feature Instrumentation
- ✅ Track user authentication events (login, signup, logout)
- ✅ Track event lifecycle (create, edit, cancel, complete)
- ✅ Track attendance actions (register, unregister, check-in)
- ✅ Track litter report submissions
- ✅ Track partner/community interactions
- ✅ Track search and discovery actions

### Phase 3 - Dashboards & Reporting
- ✅ Create Application Insights workbook for feature metrics
- ✅ Define key performance indicators (KPIs)
- ✅ Set up automated weekly/monthly reports
- ✅ Document metrics for stakeholders

---

## Out-of-Scope

- ❌ Mobile app metrics (separate project, uses Sentry.io)
- ❌ Real-time analytics dashboard (batch processing sufficient)
- ❌ User-level tracking that violates privacy (aggregate only)
- ❌ Third-party analytics tools (use existing Application Insights)

---

## Success Metrics

### Quantitative
- **Instrumentation Coverage:** 100% of key user actions tracked
- **Data Freshness:** Metrics available within 5 minutes of action
- **Dashboard Usage:** Product team reviews metrics weekly

### Qualitative
- Product decisions are informed by usage data
- Feature prioritization backed by metrics
- Grant applications include usage statistics

---

## Dependencies

### Blockers (Must be complete before this project starts)
- None (Application Insights already configured)

### Enablers for Other Projects (What this unlocks)
- Data-driven feature prioritization
- A/B testing infrastructure (future)
- User journey optimization

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Performance overhead** | Low | Medium | Use async tracking, batch events |
| **PII in metrics** | Medium | High | Strict event schema, no user-identifiable data in custom dimensions |
| **Inconsistent tracking** | Medium | Medium | Centralized tracking hook, code reviews |
| **Dashboard maintenance** | Low | Low | Use Application Insights built-in workbooks |

---

## Implementation Plan

### Event Taxonomy

Standard event naming convention: `{Category}_{Action}_{Target}`

**Categories:**
- `Auth` - Authentication events
- `Event` - Cleanup event actions
- `Attendance` - Event registration/attendance
- `LitterReport` - Litter reporting
- `Partner` - Partner/community interactions
- `Search` - Search and discovery
- `Navigation` - Page views and navigation

**Example Events:**
```
Auth_Login_Success
Auth_Signup_Complete
Event_Create_Submit
Event_Edit_Save
Attendance_Register_Click
Attendance_Checkin_Complete
LitterReport_Submit_Success
Search_Events_Execute
Navigation_Page_View
```

### Frontend Implementation

**Tracking Hook:**
```typescript
// src/hooks/useFeatureMetrics.ts
import { useCallback } from 'react';
import { appInsights } from '../services/appInsights';

interface MetricEvent {
  category: string;
  action: string;
  target?: string;
  properties?: Record<string, string | number | boolean>;
}

export function useFeatureMetrics() {
  const trackEvent = useCallback((event: MetricEvent) => {
    const eventName = [event.category, event.action, event.target]
      .filter(Boolean)
      .join('_');

    appInsights.trackEvent({
      name: eventName,
      properties: {
        ...event.properties,
        timestamp: new Date().toISOString(),
      },
    });
  }, []);

  const trackPageView = useCallback((pageName: string, properties?: Record<string, string>) => {
    appInsights.trackPageView({
      name: pageName,
      properties,
    });
  }, []);

  return { trackEvent, trackPageView };
}
```

**Usage Example:**
```typescript
function CreateEventPage() {
  const { trackEvent } = useFeatureMetrics();

  const handleSubmit = async (eventData) => {
    trackEvent({
      category: 'Event',
      action: 'Create',
      target: 'Submit',
      properties: {
        eventType: eventData.eventType,
        hasPartner: !!eventData.partnerId,
      },
    });

    // ... submit logic
  };
}
```

### Backend Implementation

**Custom Metrics Service:**
```csharp
// TrashMob.Shared/Services/MetricsService.cs
public interface IMetricsService
{
    void TrackFeatureUsage(string feature, string action, Dictionary<string, string>? properties = null);
    void TrackUserJourney(Guid userId, string milestone);
}

public class MetricsService : IMetricsService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<MetricsService> _logger;

    public MetricsService(TelemetryClient telemetryClient, ILogger<MetricsService> logger)
    {
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public void TrackFeatureUsage(string feature, string action, Dictionary<string, string>? properties = null)
    {
        var eventProperties = properties ?? new Dictionary<string, string>();
        eventProperties["Feature"] = feature;
        eventProperties["Action"] = action;

        _telemetryClient.TrackEvent($"Feature_{feature}_{action}", eventProperties);
    }

    public void TrackUserJourney(Guid userId, string milestone)
    {
        _telemetryClient.TrackEvent("UserJourney_Milestone", new Dictionary<string, string>
        {
            ["Milestone"] = milestone,
            ["UserIdHash"] = HashUserId(userId), // Hash for privacy
        });
    }

    private static string HashUserId(Guid userId)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(userId.ToString()));
        return Convert.ToBase64String(hash)[..8]; // First 8 chars only
    }
}
```

### Key Metrics to Track

**User Acquisition:**
- New user signups (daily/weekly/monthly)
- Signup funnel completion rate
- Time from signup to first action

**Event Engagement:**
- Events created per user
- Event completion rate
- Average attendees per event
- Events with summaries submitted

**Feature Adoption:**
- Litter reports submitted
- Partner associations created
- Search usage patterns
- Mobile vs web usage split

**Retention:**
- Return user rate (7-day, 30-day)
- Users attending multiple events
- Event lead retention

### Dashboard Design

**Application Insights Workbook Sections:**

1. **Overview**
   - Total active users (daily/weekly/monthly)
   - Key action counts (events, registrations, reports)
   - Trend charts

2. **User Acquisition**
   - Signup funnel visualization
   - New user trends
   - Geographic distribution

3. **Feature Usage**
   - Feature usage heatmap
   - Top actions by count
   - Feature adoption over time

4. **User Journeys**
   - Funnel: Signup → First Event View → Registration → Attendance
   - Drop-off analysis
   - Time between milestones

5. **Engagement Metrics**
   - Session duration
   - Pages per session
   - Return visit frequency

---

## Implementation Phases

### Phase 1: Infrastructure
- Set up Application Insights SDK for custom events (if not already)
- Create `useFeatureMetrics` React hook
- Create `IMetricsService` backend service
- Define event taxonomy documentation

### Phase 2: Instrumentation
- Add tracking to authentication flows
- Add tracking to event CRUD operations
- Add tracking to attendance actions
- Add tracking to litter reports
- Add tracking to search and navigation

### Phase 3: Dashboards
- Create Application Insights workbook
- Define KPI targets
- Set up automated reports
- Document metrics for stakeholders

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Privacy Considerations

- **No PII in events:** Never include names, emails, or exact locations
- **Aggregate metrics:** Focus on counts and percentages, not individual users
- **User ID hashing:** If user-level tracking needed, hash IDs
- **Opt-out support:** Respect browser Do Not Track settings
- **Data retention:** Follow Application Insights default retention (90 days)

---

## Rollout Plan

1. Implement infrastructure and tracking hook
2. Add tracking to 2-3 key features as pilot
3. Validate data appears correctly in Application Insights
4. Roll out tracking to remaining features
5. Create dashboards and share with team
6. Iterate based on feedback

---

## Decisions

1. **Should we track session recordings?**
   **Decision:** No, too privacy-invasive and adds complexity. Focus on aggregate event tracking only.

2. **What retention period for metrics?**
   **Decision:** Use Application Insights default (90 days), archive monthly aggregates for long-term trend analysis.

---

## Related Documents

- **[Project 27 - OpenTelemetry Migration](./Project_27_OpenTelemetry_Migration.md)** - Future observability improvements
- **[CLAUDE.md](../../CLAUDE.md)** - Observability section
- **[Application Insights Documentation](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)**

---

**Last Updated:** January 31, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** Q2 2026

---

## Changelog

- **2026-01-31:** Converted open questions to decisions; confirmed all scope items
