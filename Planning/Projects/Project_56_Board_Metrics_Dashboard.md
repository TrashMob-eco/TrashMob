# Project 56 — Board Metrics Dashboard

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | Medium |
| **Risk** | Medium |
| **Size** | Medium |
| **Dependencies** | Project 29 (Feature Usage Metrics) |

---

## Business Rationale

TrashMob's operational data is spread across six separate systems: Application Insights (website telemetry), Google Analytics 4 (traffic sources/conversions/ad campaigns), Sentry.io (mobile crash/performance), Microsoft Clarity (user flows/heatmaps), Azure billing (infrastructure costs), and QuickBooks (finances). Preparing for monthly board meetings requires manually logging into each system, pulling numbers, and assembling a report.

A unified dashboard in the site admin area gives board members a single, branded view of organizational health. It also creates a living record of metrics over time, supports grant applications with ready-to-export data, and reduces the volunteer time spent assembling reports.

Because Claude can handle the implementation work (API integrations, UI components, charting), the primary effort shifts to API access setup and metric selection — making the in-app approach comparable in effort to a Power BI solution while delivering a better integrated experience.

---

## Objectives

### Primary Goals
- Provide a single admin page showing key metrics from all six data sources
- Reduce monthly board report preparation from hours to minutes
- Maintain a historical record of metrics for trend analysis and grant applications

### Secondary Goals (Nice-to-Have)
- Exportable PDF/CSV for board packets
- Automated monthly email summary to board members
- Drill-down views for deeper investigation

---

## Scope

### Phase 1 - Platform Metrics + Impact Stats (App Insights + Sentry + DB)
- ☐ Admin-only dashboard page at `/admin/board-metrics`
- ☐ App Insights integration: DAU, WAU, MAU, events created, registrations, litter reports
- ☐ Sentry integration: crash-free rate, error count, mobile session count
- ☐ Impact metrics from app database: total bags collected, total weight cleaned, total volunteer hours, total events completed
- ☐ Date range selector (last 30d, last quarter, custom)
- ☐ Simple trend charts for each metric

### Phase 2 - Traffic Analytics + Google Ads (GA4 + Clarity)
- ✅ Add Google Analytics 4 to the site (PR #3110 — gtag.js with cookie consent, prod-only)
- ☐ Configure GA4 conversion events: signup, event registration, event creation, litter report
- ☐ GA4 dashboard integration: traffic sources, top channels, conversion rates, campaign performance
- ☐ Link GA4 to Google Ads account (and apply for Google Ad Grants — $10K/month free search ads for nonprofits)
- ☐ Microsoft Clarity integration: top pages, session recordings count, user flow completion rates
- ☐ Azure Cost Management integration: monthly spend, cost by resource category, trend
- ☐ Comparison view (this month vs. last month, this quarter vs. last quarter)

### Phase 3 - Financial Metrics + Polish
- ☐ QuickBooks integration: revenue, expenses, net income, cash on hand
- ☐ Executive summary card with red/yellow/green health indicators
- ☐ PDF export for board packets
- ☐ Historical snapshots (store monthly snapshots for long-term trend analysis)

### Phase 4 - Automation (Future)
- ☐ Automated monthly email with dashboard summary to board distribution list
- ☐ Alert thresholds (e.g., crash-free rate drops below 99%, spend exceeds budget)

---

## Out-of-Scope

- ☐ Real-time monitoring or alerting (that's operations, not board reporting)
- ☐ Individual user-level analytics (privacy concern, not needed for board)
- ☐ Replacing App Insights/Sentry/Clarity for day-to-day debugging
- ☐ Mobile app version of the dashboard
- ☐ Non-board-member access (this is admin-only)

---

## Success Metrics

### Quantitative
- **Board report prep time:** < 15 minutes (baseline: 2-3 hours of manual assembly)
- **Data freshness:** Metrics no more than 24 hours old
- **Coverage:** All 6 data sources integrated by Phase 3 completion

### Qualitative
- Board members report improved understanding of organizational health
- Grant applications cite dashboard data as evidence of impact
- Reduced ad-hoc metric requests to engineering team

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **API access credentials:** Need API keys/tokens for GA4, Sentry, Clarity, QuickBooks, and Azure Cost Management
- **Google Analytics setup:** GA4 property must be created and tracking snippet deployed before Phase 2 data is available
- **Project 29 (Feature Usage Metrics):** App Insights custom events must be in place (✅ already done)

### Enablers for Other Projects
- **Grant applications:** Provides ready-to-export impact data
- **Project 54 (Community Adoption Outreach):** Usage metrics help demonstrate platform value to prospective communities

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **QuickBooks API complexity/cost** | Medium | Medium | Phase 3 — defer if API costs are prohibitive; manual entry fallback |
| **Clarity API only returns last 1-3 days** | High | Medium | Poll daily via scheduled job and store results in snapshot table for historical access |
| **API rate limits on free tiers** | Medium | Low | Cache responses server-side; refresh daily, not on every page load |
| **Sensitive financial data exposure** | Low | High | Admin-only route with ValidUser + Admin policy; no financial data in client bundle unless authenticated |
| **External API breaking changes** | Medium | Medium | Isolate each integration behind its own service class; version-pin API clients |
| **Sentry API token scope** | Low | Medium | Use read-only org-level token; document required scopes |
| **GA4 data delay** | High | Low | GA4 data can be 24-48 hours delayed; set expectations in UI. Use Realtime API only for live demo scenarios |
| **Google Ad Grants compliance** | Medium | Medium | Grants require conversion tracking, 5% CTR, and active management; plan for ongoing maintenance |

---

## Implementation Plan

### Data Model Changes

No new database tables for Phase 1-2 (metrics are fetched live from external APIs and cached in memory).

Phase 3 adds a snapshot table for historical records:

```csharp
/// <summary>
/// Monthly snapshot of board metrics for historical trend analysis.
/// </summary>
public class BoardMetricsSnapshot : KeyedModel
{
    public DateOnly SnapshotMonth { get; set; }

    // Platform metrics
    public int MonthlyActiveUsers { get; set; }
    public int EventsCreated { get; set; }
    public int Registrations { get; set; }
    public int LitterReports { get; set; }

    // Impact metrics (from app database)
    public int TotalBagsCollected { get; set; }
    public decimal TotalWeightCleaned { get; set; }
    public decimal TotalVolunteerHours { get; set; }
    public int TotalEventsCompleted { get; set; }

    // Mobile metrics
    public decimal CrashFreeRate { get; set; }
    public int MobileSessions { get; set; }

    // Traffic metrics (Phase 2)
    public int? WebsiteSessions { get; set; }
    public int? UniqueVisitors { get; set; }
    public decimal? AdGrantSpend { get; set; }
    public int? AdConversions { get; set; }

    // Financial metrics (Phase 3)
    public decimal? AzureMonthlySpend { get; set; }
    public decimal? Revenue { get; set; }
    public decimal? Expenses { get; set; }

    // Metadata
    public DateTime CreatedDate { get; set; }
}
```

### API Changes

#### Backend Proxy Endpoints

External API calls are made server-side to protect API keys and manage rate limiting.

```csharp
[ApiController]
[Route("api/v2/admin/board-metrics")]
[Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
public class BoardMetricsController(
    IAppInsightsMetricsService appInsightsService,
    IGoogleAnalyticsMetricsService googleAnalyticsService,
    ISentryMetricsService sentryService,
    IClarityMetricsService clarityService,
    IAzureCostService azureCostService,
    IQuickBooksMetricsService quickBooksService)
    : SecureController
{
    [HttpGet("platform")]
    [ProducesResponseType(typeof(PlatformMetricsDto), 200)]
    public async Task<IActionResult> GetPlatformMetrics(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        var metrics = await appInsightsService.GetMetricsAsync(from, to, cancellationToken);
        return Ok(metrics);
    }

    [HttpGet("mobile")]
    [ProducesResponseType(typeof(MobileMetricsDto), 200)]
    public async Task<IActionResult> GetMobileMetrics(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        var metrics = await sentryService.GetMetricsAsync(from, to, cancellationToken);
        return Ok(metrics);
    }

    [HttpGet("costs")]
    [ProducesResponseType(typeof(CostMetricsDto), 200)]
    public async Task<IActionResult> GetCostMetrics(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        var metrics = await azureCostService.GetMetricsAsync(from, to, cancellationToken);
        return Ok(metrics);
    }

    [HttpGet("traffic")]
    [ProducesResponseType(typeof(TrafficMetricsDto), 200)]
    public async Task<IActionResult> GetTrafficMetrics(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        var metrics = await googleAnalyticsService.GetMetricsAsync(from, to, cancellationToken);
        return Ok(metrics);
    }

    [HttpGet("financial")]
    [ProducesResponseType(typeof(FinancialMetricsDto), 200)]
    public async Task<IActionResult> GetFinancialMetrics(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        var metrics = await quickBooksService.GetMetricsAsync(from, to, cancellationToken);
        return Ok(metrics);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(BoardMetricsSummaryDto), 200)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        // Aggregates all sources into a single summary view
    }
}
```

#### Service Layer Pattern

Each external integration follows the same pattern:

```csharp
public interface IAppInsightsMetricsService
{
    Task<PlatformMetricsDto> GetMetricsAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
}

public class AppInsightsMetricsService(
    IConfiguration configuration,
    IMemoryCache cache,
    ILogger<AppInsightsMetricsService> logger) : IAppInsightsMetricsService
{
    // Uses Azure Monitor Query SDK to fetch metrics
    // Caches results for 1 hour to avoid API rate limits
}
```

### Web UX Changes

New admin page: `/admin/board-metrics`

**Layout:**
- Top bar: date range selector + export button
- Executive summary cards (4-6 KPI cards with trend arrows)
- Tabbed sections: Platform | Traffic & Ads | Mobile | User Analytics | Costs | Financial
- Each tab contains 2-3 charts (line charts for trends, bar charts for comparisons)

**Charting library:** Recharts (already common in React ecosystems, lightweight, composable)

**Components:**
- `BoardMetricsDashboard` — main page container
- `MetricCard` — single KPI display with value, trend, and health indicator
- `MetricChart` — reusable chart wrapper for line/bar charts
- `DateRangeSelector` — preset ranges + custom date picker
- `ExportButton` — PDF/CSV export (Phase 3)

### Mobile App Changes

None — this is an admin-only web feature.

---

## Implementation Phases

### Phase 1: Platform + Mobile Metrics
- Set up backend service interfaces and DI registration
- Implement App Insights metrics service (using Azure Monitor Query SDK)
- Implement Sentry metrics service (using Sentry API)
- Create `BoardMetricsController` with `/platform` and `/mobile` endpoints
- Build React dashboard page with date selector and metric cards
- Add charting with Recharts
- Add admin route and navigation link

### Phase 2: Traffic Analytics + Google Ads + Cost Metrics
- Add GA4 tracking snippet to `index.html` and configure conversion events
- Set up GA4 property in Google Analytics console
- Apply for Google Ad Grants ($10K/month free nonprofit search ads)
- Link GA4 ↔ Google Ads for conversion tracking and campaign attribution
- Implement GA4 Data API service (using `Google.Analytics.Data.V1Beta` NuGet)
- Create `BoardMetricsController` `/traffic` endpoint
- Add Traffic & Ads tab to dashboard (sources, channels, conversions, campaign ROAS)
- Implement Clarity integration (API or embed approach, depending on API availability)
- Implement Azure Cost Management service (using Azure SDK)
- Add cost and analytics tabs to dashboard
- Add comparison view (period-over-period)

### Phase 3: Financial + Export + History
- Implement QuickBooks integration (OAuth2 + QuickBooks API)
- Add financial tab to dashboard
- Create `BoardMetricsSnapshot` table and migration
- Build monthly snapshot job (can run in existing daily/hourly jobs)
- Add PDF export functionality
- Add executive summary card with health indicators

### Phase 4: Automation
- Monthly email summary via SendGrid
- Configurable alert thresholds
- Board member distribution list management

**Note:** Phases are sequential but not time-bound. Claude can handle the implementation for each phase, with the main human effort being API credential setup and metric selection review.

---

## External API Details

### App Insights
- **SDK:** `Azure.Monitor.Query` NuGet package
- **Auth:** Managed identity or service principal (already configured for Azure resources)
- **Key data:** Custom events, page views, unique users, session counts
- **Rate limits:** Generous for query API

### Google Analytics 4
- **SDK:** `Google.Analytics.Data.V1Beta` NuGet package (GA4 Data API)
- **Auth:** Service account with Viewer role on the GA4 property (JSON key stored in Key Vault)
- **Key data:** Sessions, users, traffic sources/channels, conversion events, campaign performance
- **Google Ads link:** When GA4 is linked to Google Ads, campaign metrics (impressions, clicks, cost, conversions) flow into GA4 and can be queried via the same API
- **Google Ad Grants:** TrashMob qualifies as a 501(c)(3) for up to $10K/month in free Google Search ads. Requires: conversion tracking in GA4, maintaining 5% CTR, active account management
- **Rate limits:** 10 concurrent requests, generous daily quota
- **Note:** GA4 data has 24-48 hour processing delay; dashboard should display "as of" timestamp

### Sentry
- **API:** REST API (`https://sentry.io/api/0/`)
- **Auth:** Org-level auth token (read-only scope: `org:read`, `project:read`)
- **Key data:** Crash-free rate, error count, session count per platform
- **Rate limits:** 40 req/sec for org tokens

### Microsoft Clarity
- **API:** [Data Export API](https://learn.microsoft.com/en-us/clarity/setup-and-installation/clarity-data-export-api) — `GET https://www.clarity.ms/export-data/api/v1/project-live-insights`
- **Auth:** JWT bearer token (generated in Clarity Settings → Data Export → Generate new API token)
- **Key data:** Traffic (sessions, bot sessions, unique users, pages/session), Popular Pages, Engagement Time, Scroll Depth, Dead/Rage/Error click counts
- **Dimensions:** Browser, Device, Country/Region, OS, Source, Medium, Campaign, Channel, URL (up to 3 per request)
- **Rate limits:** 10 requests/project/day
- **Limitation:** Data only covers last 1-3 days (no arbitrary date ranges). Must poll daily and store snapshots for historical trends.

### Azure Cost Management
- **SDK:** `Azure.ResourceManager.CostManagement` NuGet package
- **Auth:** Managed identity with Cost Management Reader role
- **Key data:** Monthly cost by resource type, cost trends
- **Note:** Sponsorship subscriptions may have limited API support (see memory notes)

### QuickBooks
- **API:** QuickBooks Online API v3
- **Auth:** OAuth2 (requires periodic token refresh)
- **Key data:** P&L summary, balance sheet summary, cash flow
- **Complexity:** Highest of all integrations due to OAuth2 flow and token management

---

## Open Questions

1. ~~**Which specific metrics does the board want to see?**~~ **RESOLVED**
   **Decision:** Project 29 KPIs (DAU, events created, registrations, litter reports) + impact metrics from DB (bags collected, weight cleaned, volunteer hours, total events) + crash-free rate + monthly Azure spend. Financial metrics deferred (see Q3). Refine after first board meeting.

2. ~~**Clarity API access — is there a usable API?**~~ **RESOLVED — Yes**
   **Findings:** Clarity has a [Data Export API](https://learn.microsoft.com/en-us/clarity/setup-and-installation/clarity-data-export-api). Auth via JWT bearer token (generated in Clarity Settings → Data Export). Single endpoint: `GET https://www.clarity.ms/export-data/api/v1/project-live-insights`. Returns JSON with metrics (Traffic, Popular Pages, Dead/Rage/Error clicks, Scroll Depth, Engagement Time) broken down by up to 3 dimensions (Browser, Device, Country, OS, Source, URL, etc.).
   **Limitations:** Max 10 requests/project/day, data limited to last 1-3 days only, 1000 row response cap. No historical range queries — we'll need to poll daily and store snapshots for trend analysis.
   **Action:** Generate an API token in Clarity settings. Backend service should cache daily pulls and store in the snapshot table for historical access.

3. ~~**QuickBooks integration — is the API cost justified?**~~ **DEFERRED**
   **Decision:** Hold off on QuickBooks integration for now. Revisit after Phases 1-2 are complete and board feedback is collected.

4. ~~**Should we apply for Google Ad Grants?**~~ **IN PROGRESS**
   **Status:** Application submitted, waiting for Google approval. GA4 tracking is now in place (PR #3110) to satisfy the conversion tracking prerequisite.

5. ~~**Should historical snapshots be daily or monthly?**~~ **RESOLVED**
   **Decision:** Monthly — matches board meeting cadence, reduces storage, and simplifies the snapshot job.

---

## Related Documents

- **[Project 29 — Feature Usage Metrics](./Project_29_Feature_Usage_Metrics.md)** — Existing App Insights custom events and KPI definitions
- **[Project 29 — Metrics Guide](./Project_29_Metrics_Guide.md)** — Dashboard documentation and KPI targets
- **[Project 30 — Azure Billing Alerts](./Project_30_Azure_Billing_Alerts.md)** — Related cost monitoring infrastructure
- **[Operations Runbook](../../Deploy/OPERATIONS_RUNBOOK.md)** — Infrastructure access patterns

---

**Last Updated:** 2026-03-15
**Owner:** Joe / Engineering
**Status:** Planning
**Next Review:** Before Phase 1 kickoff
