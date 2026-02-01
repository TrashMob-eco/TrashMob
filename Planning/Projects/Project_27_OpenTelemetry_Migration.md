# Project 27 — Migrate Web App Observability to OpenTelemetry

| Attribute | Value |
|-----------|-------|
| **Status** | Complete |
| **Priority** | Low |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 6 (Backend Standards - .NET 10), Project 5 (Deployment Pipelines) |

---

## Business Rationale

Migrate web app, API, and background jobs from Azure Application Insights SDK to OpenTelemetry for vendor-neutral observability. This enables flexibility to use multiple backends, standardizes instrumentation across the industry, and future-proofs the telemetry stack.

**Current State:**
- **Mobile App:** Uses Sentry.io for crash reporting and error tracking (no change planned)
- **Web App/API/Jobs:** Uses Azure Application Insights SDK directly

---

## Objectives

### Primary Goals
- Replace Application Insights SDK with OpenTelemetry .NET SDK
- Configure OpenTelemetry exporter to continue sending data to Application Insights
- Add standard OpenTelemetry instrumentation for HTTP, database, and custom spans
- Enable future flexibility to export to additional backends (e.g., Jaeger, Prometheus, Grafana)

### Secondary Goals (Nice-to-Have)
- Maintain existing dashboards and alerts during migration
- Add correlation IDs across frontend and backend
- Document OpenTelemetry patterns for contributors

---

## Scope

### Phase 1 - Backend (.NET) ✅
- ✅ Add OpenTelemetry.Extensions.Hosting and Azure Monitor exporter NuGet packages
- ✅ Configure OpenTelemetry in Program.cs with auto-instrumentation
- ✅ Add custom activity sources for business operations
- ✅ Remove direct Application Insights SDK references
- ✅ Validate telemetry in Application Insights portal

### Phase 2 - Frontend (React)
- ✅ Add @opentelemetry/sdk-trace-web package
- ✅ Configure browser instrumentation for fetch/XHR
- ✅ Export traces to backend collector endpoint
- ✅ Correlate frontend traces with backend spans

### Phase 3 - Validation
- ✅ Compare telemetry coverage before/after migration
- ✅ Update dashboards if needed
- ✅ Document new instrumentation patterns

---

## Out-of-Scope

- ❌ Mobile app changes (continues using Sentry.io)
- ❌ Changing the observability backend (Application Insights remains the data store)
- ❌ Major dashboard redesign (incremental updates only)

---

## Success Metrics

### Quantitative
- **Telemetry Coverage:** All existing telemetry data continues flowing to Application Insights
- **Performance Impact:** No increase in P95 latency from instrumentation overhead
- **Flexibility:** Ability to add additional exporters without code changes

### Qualitative
- Standard OpenTelemetry semantic conventions used for all spans
- Cleaner instrumentation code following industry standards
- Easier onboarding for contributors familiar with OpenTelemetry

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Project 6 (Backend Standards):** .NET 10 upgrade required for latest OpenTelemetry packages
- **Project 5 (Deployment Pipelines):** Reliable CI/CD for validation testing

### Enablers for Other Projects (What this unlocks)
- Future observability improvements (e.g., Grafana dashboards, distributed tracing visualization)
- Potential cost optimization by exporting to lower-cost backends

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Telemetry gaps during migration** | Low | Medium | Shadow production traffic to validate data completeness before switching |
| **Performance overhead** | Low | Medium | Benchmark instrumentation overhead before deployment |
| **Learning curve for contributors** | Low | Low | Document patterns and provide examples |

---

## Implementation Plan

### Data Model Changes
None required - this is purely an observability infrastructure change.

### API Changes
No API changes required. OpenTelemetry instrumentation is transparent to API consumers.

### Web UX Changes
No visible UX changes. Frontend instrumentation is transparent to users.

### Backend Changes

**Program.cs configuration:**
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("TrashMob.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddAzureMonitorTraceExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddAzureMonitorMetricExporter());
```

**Custom activity sources:**
```csharp
public static class TrashMobActivitySources
{
    public static readonly ActivitySource Events = new("TrashMob.Events");
    public static readonly ActivitySource Users = new("TrashMob.Users");
    public static readonly ActivitySource Notifications = new("TrashMob.Notifications");
}
```

---

## Implementation Phases

### Phase 1: Development Environment
- Add OpenTelemetry packages to TrashMob, TrashMobDailyJobs, TrashMobHourlyJobs
- Configure instrumentation in development environment
- Validate telemetry appears in Application Insights

### Phase 2: Production Validation
- Deploy to production with dual-write (both SDKs active)
- Compare telemetry coverage between old and new
- Remove Application Insights SDK after validation

### Phase 3: Frontend & Documentation
- Add frontend OpenTelemetry instrumentation
- Correlate browser traces with backend
- Document patterns for contributors

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Rollout Plan

1. Implement in development environment first
2. Shadow production traffic to validate data completeness
3. Switch production after validation period
4. Remove legacy SDK after stable monitoring period

---

## Decisions

1. **Should we add custom metrics for business events?**
   **Decision:** Yes, add metrics for event creation, user signups, attendance

---

## Related Documents

- **[Project 6 - Backend Standards](./Project_06_Backend_Standards.md)** - .NET 10 upgrade dependency
- **[Project 5 - Deployment Pipelines](./Project_05_Deployment_Pipelines.md)** - CI/CD infrastructure
- **[CLAUDE.md](../../CLAUDE.md)** - Observability section documents current state

---

**Last Updated:** January 31, 2026
**Owner:** Engineering Team
**Status:** Complete
**Next Review:** N/A

---

## Changelog

- **2026-01-31:** Marked project as Complete
- **2026-01-31:** Converted open question to decision; confirmed all scope items
