# Project 24 - API v2 Modernization

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Very Large |
| **Dependencies** | None (v1 endpoints remain untouched throughout) |

---

## Business Rationale

Create a modern, scalable, and developer-friendly API layer (v2) that improves reliability, debuggability, and reduces manual code maintenance. Current v1 APIs lack pagination, consistent error handling, and require significant manual client code in both React (49 service files) and mobile (95 service files) apps. Auto-generated clients will accelerate development and reduce bugs.

---

## Prior Work & Current State

Before starting, it's important to recognize what already exists. These items were completed through other projects (primarily Project 6 and ongoing infrastructure work) and do **not** need to be repeated:

| Already Done | Where | Notes |
|---|---|---|
| Response compression (Brotli + Gzip) | `Program.cs` | `CompressionLevel.Fastest`, HTTPS enabled |
| XML documentation on all public APIs | All `.csproj` files | `<GenerateDocumentationFile>true</GenerateDocumentationFile>` |
| Swagger/OpenAPI v1 configuration | `Program.cs` | Single doc, Bearer auth, XML comments included |
| OpenTelemetry (traces, metrics, logs) | `Program.cs` | Already configured for Application Insights |
| 7 authorization policies | `Program.cs` | ValidUser, UserOwnsEntity, UserIsAdmin, etc. |
| ServiceResult pattern defined | `TrashMob.Shared/Poco/ServiceResult.cs` | Exists but not used in controllers yet |
| 59 Poco/DTO files | `TrashMob.Models/Poco/` | Stats, DisplayEvent, filters, requests, etc. |
| Entity-to-DTO extension methods | `PocoExtensions.cs`, `LitterReportExtensions.cs` | `ToDisplayEvent()`, `ToDisplayUser()`, `ToFullLitterReport()` |
| Primary constructors on all classes | Project 6 Phase 2.5 | Controllers, managers, repositories |
| Structured logging | Project 6 Phase 2.5 | Message templates, not string interpolation |
| Health checks (SQL Server) | `Program.cs` | Basic database health check configured |
| Rate limiting thresholds decided | Project 6 | Public: 100/min, Auth: 300/min, Admin: 600/min |

### Current DTO Coverage Gaps

Controllers currently return **raw entities** inconsistently:

| Controller | Returns Entities Directly | Returns DTOs |
|---|---|---|
| EventsController | `Event` (Get, Add, Update, Delete) | `DisplayEvent` (Active, Completed, NotCanceled) |
| UsersController | `User` (all endpoints) | `UserImpactStats` (impact only) |
| CommunitiesController | `Partner` (list, get) | `CommunityDashboard`, `CommunityPublicStats` |
| TeamsController | `Team` (all endpoints) | None |
| StatsController | None | `Stats` (all endpoints) |
| LitterReportsController | `LitterReport` (CRUD) | `FullLitterReport` (display) |

This means entities with navigation properties, audit fields, and internal IDs are crossing the wire to clients.

### PR #2490 (Closed Without Merge)

A community contributor (Francisco Loureiro) submitted PR #2490 "Add swagger v2" which attempted API versioning and an optimized stats endpoint. It was closed because:
- Used deprecated `Microsoft.AspNetCore.Mvc.Versioning` package (should use `Asp.Versioning.Mvc`)
- SQL logic bugs in raw query (`PickedWeight` vs `PickedWeightUnitId`)
- Generated C# code pasted into a `.ts` file (wrong code generator)
- Large formatting-only noise in MobDbContext.cs
- Debug logger left in MobDbContext

The goals were valid and are fully covered by this project.

---

## Objectives

### Core Improvements
- Implement pagination on all collection endpoints (offset-based default)
- Standardized error responses (RFC 9457 Problem Details)
- Auto-generate TypeScript clients for React app (NSwag)
- Auto-generate .NET clients for MAUI mobile app (Kiota)
- Comprehensive OpenAPI 3.1 documentation with v1 + v2 docs
- Correlation IDs for distributed tracing
- Server-side filtering to reduce network traffic (especially mobile)

### Modernization Strategies
- API versioning via URL path (`/api/v2/events`)
- ETags for conditional requests and caching
- Rate limiting with token buckets (thresholds already decided)
- Consistent DTO layer — no entities cross the wire in v2
- Standard query parameter filtering per endpoint
- Sorting query parameters (`?sort=-date,name`)

---

## Scope

### Phase 1 - Foundation

Infrastructure that all v2 endpoints will use. No v1 endpoints are modified.

- [ ] **API versioning** - Install `Asp.Versioning.Mvc` + `Asp.Versioning.Mvc.ApiExplorer`; configure default v1; add `[ApiVersion("1.0")]` to `BaseController`; add v2 Swagger doc alongside v1
- [ ] **Problem Details middleware** - Global exception handler returning RFC 9457 responses with correlation IDs; replace manual error responses
- [ ] **Correlation ID middleware** - Generate/propagate `X-Correlation-ID` header; integrate with OpenTelemetry and structured logging
- [ ] **Pagination framework** - `PagedRequest` (page, pageSize, sort) and `PagedResponse<T>` (items, pagination metadata); reusable extension method on `IQueryable<T>`
- [ ] **Server-side filtering framework** - Standard `IFilterable<T>` pattern with per-endpoint filter classes (see Filtering Strategy below)
- [ ] **V2 DTO layer** - Create `TrashMob.Models/Poco/V2/` with DTOs decoupled from entities; manual mapping extension methods; reusable for MCP server
- [ ] **OpenAPI 3.1 dual-doc** - v1 (existing endpoints, unchanged) + v2 (new endpoints) side by side in Swagger UI
- [ ] **Client generation pipeline** - NSwag for TypeScript (Fetch template), Kiota for .NET; GitHub Actions workflow triggered by V2 controller changes

### Phase 2 - Core Endpoints

New v2 controllers alongside existing v1 controllers. V1 remains untouched.

- [ ] **Events v2** - Pilot endpoint; paginated list, filtered by status/city/region/date range; `EventDto` response
- [ ] **Users v2** - Paginated list; `UserDto` (no PII leaks, minor privacy masking built into DTO)
- [ ] **Partners/Communities v2** - Paginated list; community dashboard aggregates
- [ ] **EventAttendees v2** - Filtered by event; paginated
- [ ] **EventSummaries/Stats v2** - Aggregate stats (optimized query); filtered summaries
- [ ] **LitterReports v2** - Paginated with geospatial filtering
- [ ] **Generate and commit TypeScript client** (NSwag)
- [ ] **Generate and commit .NET MAUI client** (Kiota)

### Phase 3 - Client Migration

Migrate web and mobile apps to use v2 endpoints and generated clients. V1 endpoints still running.

- [ ] **Migrate React app** - Replace 49 manual service files with generated client, one page at a time
- [ ] **Migrate MAUI mobile app** - Replace 95 manual service files with generated client, one screen at a time
- [ ] **Verify v1 traffic drops to zero** via Application Insights

### Phase 4 - Advanced Features

Only after v2 is stable and clients are migrated.

- [ ] ETags and conditional requests (304 Not Modified)
- [ ] Rate limiting middleware (apply decided thresholds)
- [ ] Bulk operations (batch create/update for events, litter reports)
- [ ] Webhook infrastructure for event notifications

---

## Filtering Strategy

### Standard Framework (Build Once)

A reusable filtering/sorting/pagination pipeline that applies to any `IQueryable<T>`:

```csharp
// Shared infrastructure - works for any entity
public class QueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;  // Max 100
    public string? Sort { get; set; }         // e.g., "-date,name"
}

public static class QueryableExtensions
{
    public static async Task<PagedResponse<TDto>> ToPagedAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        QueryParameters parameters,
        Func<TEntity, TDto> mapper,
        CancellationToken cancellationToken) { ... }
}
```

### Per-Endpoint Filters (Case by Case)

Each endpoint defines which fields are filterable, because the useful filters differ:

```csharp
// Events: filter by status, location, date range, type
public class EventFilterV2 : QueryParameters
{
    public int? EventStatusId { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public int? EventTypeId { get; set; }
}

// LitterReports: filter by status, location radius, reporter
public class LitterReportFilterV2 : QueryParameters
{
    public int? StatusId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusMiles { get; set; }
}
```

### Mobile-Specific Benefits

The biggest network savings for mobile come from:

1. **Pagination** - Currently fetching all events/litter reports in one call. A 25-item page vs. 500+ items is a 95% reduction.
2. **Status filtering server-side** - Mobile currently fetches all events then filters client-side. Server-side `?eventStatusId=1` avoids transferring cancelled/completed events.
3. **DTOs without navigation properties** - Entity `Event` includes lazy-loaded `CreatedByUser`, `EventType`, `EventStatus`, attendees, etc. A flat `EventDto` with just the needed fields is dramatically smaller.
4. **ETags (Phase 4)** - Mobile repeatedly fetches the same lists. `304 Not Modified` eliminates redundant transfers entirely.

---

## Out-of-Scope

- v1 endpoint removal (maintain both until all clients migrated)
- GraphQL API (evaluate separately if demand exists)
- API Gateway (defer until microservices architecture needed)
- gRPC endpoints (not needed for current use cases)
- External/public developer API (internal use only)
- Field-level sparse fieldsets (`?fields=name,date`) — adds complexity with diminishing returns when DTOs are already lean
- OData query syntax — too complex; simple typed filter classes are clearer and safer

---

## Rollout Plan (Risk-Minimized)

The key principle: **v1 endpoints are never modified or removed.** All v2 work is purely additive until Phase 3 client migration, which is incremental (one page/screen at a time).

### Step 1 - Infrastructure (No user-facing changes)

1. Install `Asp.Versioning.Mvc` + `Asp.Versioning.Mvc.ApiExplorer` NuGet packages
2. Configure API versioning with default v1 (all existing controllers automatically stay on v1)
3. Add `[ApiVersion("1.0")]` to `BaseController` — this is a no-op since it matches the default
4. Add Problem Details global exception handler (replaces generic 500s; improves v1 too)
5. Add Correlation ID middleware (applies to all requests; improves v1 too)
6. Create `PagedResponse<T>`, `QueryParameters`, `IQueryable` extensions in `TrashMob.Shared`
7. Create v2 DTO folder structure and first DTO (`EventDto`) with mapping extension
8. Add v2 Swagger doc alongside v1 in Swagger UI
9. Set up NSwag + Kiota in GitHub Actions (generates from v2 Swagger spec)

**Risk:** Low. Steps 1-3 add versioning infrastructure without changing any existing routes. Steps 4-5 improve error handling for all requests (beneficial side effect). Steps 6-9 are new code with no impact on existing functionality.

**Validation:** Run full test suite (442 backend tests). Verify Swagger UI shows both v1 and v2 docs. Verify all existing API calls from web and mobile still work unchanged.

### Step 2 - Pilot Endpoint (Events v2)

1. Create `EventsV2Controller` with `[ApiVersion("2.0")]` and `[Route("api/v{version:apiVersion}/events")]`
2. Implement `GET /api/v2/events` with pagination + filtering → returns `PagedResponse<EventDto>`
3. Implement `GET /api/v2/events/{id}` → returns `EventDto`
4. Generate TypeScript client from v2 spec
5. Generate .NET client from v2 spec
6. Migrate **one** React page (e.g., Events list) to use generated v2 client
7. Migrate **one** MAUI screen (e.g., Events list) to use generated v2 client
8. Validate both work correctly in dev environment

**Risk:** Low. New controller at new route. Existing `EventsController` at `/api/events` is untouched. One page/screen migrated as proof of concept — easy to revert.

**Validation:** Existing web and mobile apps still use v1 and work normally. New v2 page/screen works with generated client. Compare response sizes between v1 (full entity) and v2 (DTO with pagination).

### Step 3 - Remaining v2 Endpoints

1. Create v2 controllers for: Users, Partners/Communities, EventAttendees, EventSummaries/Stats, LitterReports, Teams
2. Each with appropriate per-endpoint filters
3. Regenerate TypeScript and .NET clients after each batch
4. Write integration tests for v2 endpoints

**Risk:** Low. Same pattern as Step 2, repeated. V1 still untouched.

### Step 4 - Web Migration (Incremental)

1. Migrate React pages one at a time to use generated v2 TypeScript client
2. Replace manual service files as each page migrates
3. Each migration is a separate PR — easy to review and revert
4. Track v1 vs v2 traffic in Application Insights

**Risk:** Medium. This is where users start seeing v2 responses. Mitigated by incremental approach (one page per PR) and the fact that v1 endpoints remain as fallback.

### Step 5 - Mobile Migration (Incremental)

1. Migrate MAUI screens one at a time to use generated .NET client
2. Replace manual RestService files as each screen migrates
3. Test on both Android and iOS
4. Monitor Sentry for any new crash patterns

**Risk:** Medium. Same as Step 4 but for mobile. App store release cycle adds latency. Mitigated by keeping v1 endpoints alive.

### Step 6 - Advanced Features

1. Implement ETags (biggest mobile benefit — eliminates redundant transfers)
2. Apply rate limiting middleware (thresholds: 100/300/600 per min)
3. Bulk operations where needed (e.g., batch event creation for community programs)
4. Webhook infrastructure (event created/updated notifications)

**Risk:** Low-Medium. These are additive features on v2 endpoints. ETags are purely additive. Rate limiting needs careful threshold tuning.

### Step 7 - Stabilization & v1 Deprecation

1. Monitor v1 traffic — should be near zero
2. Add deprecation headers to v1 endpoints (`Sunset` header)
3. Remove v1 endpoints only after confirming zero traffic for 30+ days
4. Clean up old service files from web and mobile

**Risk:** Low by this point. Data-driven decision based on traffic monitoring.

---

## Success Metrics

### Quantitative
- API response times <= 200ms (p95)
- Auto-generated clients reduce manual API code by 70% (49 web + 95 mobile service files)
- Error resolution time decreased by 50% (via correlation IDs + Problem Details)
- Mobile network traffic reduced by 80%+ on list endpoints (pagination + DTOs)
- Zero breaking changes to v1 endpoints during entire transition

### Qualitative
- Developer onboarding time reduced (generated clients + better docs)
- Improved API consumer satisfaction
- Faster feature development (add endpoint -> regenerate client -> done)

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Maintaining two API versions increases complexity | Medium | Medium | Shared business logic (managers); v2 controllers are thin wrappers; time-bound v1 deprecation |
| Client generation doesn't handle edge cases | Low | Medium | Manual client overrides where needed; comprehensive testing; fallback to v1 |
| Breaking changes during development | Low | High | V1 never modified; v2 is additive; incremental migration with per-PR rollback |
| Team learning curve for new patterns | Medium | Low | Clear examples in pilot endpoint; CLAUDE.md updated with v2 patterns |
| Mobile app store release latency | High | Medium | Keep v1 alive until app store updates propagate (minimum 2 weeks) |

---

## Dependencies

### Blockers
None — v2 work is purely additive and can proceed in parallel with other projects.

### Enables
- MCP server can reuse v2 DTOs for tool schemas
- Future external API access built on v2 foundation
- Mobile app performance improvements (pagination, smaller payloads)

---

## Implementation Examples

### Pagination Models (C#)

```csharp
public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    public PaginationMetadata Pagination { get; set; }
}

public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}
```

### V2 DTO Pattern (Manual Mapping)

```csharp
// TrashMob.Models/Poco/V2/EventDto.cs
public class EventDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset EventDate { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int MaxNumberOfParticipants { get; set; }
    public bool IsEventPublic { get; set; }
    public string EventTypeName { get; set; }
    public string EventStatusName { get; set; }
    public string CreatedByUserName { get; set; }
    public int AttendeeCount { get; set; }
}

// TrashMob.Models/Extensions/V2/EventMappingsV2.cs
public static class EventMappingsV2
{
    public static EventDto ToV2Dto(this Event entity, string userName, int attendeeCount = 0)
    {
        return new EventDto
        {
            Id = entity.Id,
            Name = entity.Name,
            // ... flat mapping, no navigation properties
        };
    }
}
```

### Error Models (RFC 9457 Problem Details)

```csharp
public class ProblemDetailsExtension : ProblemDetails
{
    public string? TraceId { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime Timestamp { get; set; }
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
}
```

### V2 Controller Pattern

```csharp
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/events")]
[Produces("application/json")]
public class EventsV2Controller(
    IEventManager eventManager,
    ILogger<EventsV2Controller> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEvents(
        [FromQuery] EventFilterV2 filter,
        CancellationToken cancellationToken)
    {
        var result = await eventManager.GetPagedAsync(filter, cancellationToken);
        return Ok(result);
    }
}
```

### Correlation ID Middleware

```csharp
public class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                         ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await next(context);
        }
    }
}
```

### Client Generation CI/CD (GitHub Actions)

```yaml
name: Generate API Clients
on:
  push:
    paths:
      - 'TrashMob/Controllers/V2/**'
      - 'TrashMob.Models/Poco/V2/**'
jobs:
  generate-clients:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Generate OpenAPI JSON
        run: |
          dotnet swagger tofile --output openapi-v2.json \
            TrashMob/bin/Release/net10.0/TrashMob.dll v2
      - name: Generate TypeScript Client
        run: |
          npx nswag openapi2tsclient \
            /input:openapi-v2.json \
            /output:TrashMob/client-app/src/api/generated/v2-client.ts \
            /template:Fetch
      - name: Generate .NET Client
        run: |
          dotnet tool install --global Microsoft.Kiota
          kiota generate \
            --openapi openapi-v2.json \
            --output TrashMobMobile/Generated/ApiClient \
            --language CSharp \
            --class-name TrashMobApiClient
```

---

## Decisions

1. **Client generation tools?**
   **Decision:** NSwag for TypeScript (React), Kiota for .NET (MAUI)

2. **Query filtering syntax?**
   **Decision:** Typed filter classes per endpoint (not OData). Simpler, type-safe, no parser complexity.

3. **Pagination default?**
   **Decision:** Offset-based with 25 items/page default, 100 max. Cursor-based not needed for current scale.

4. **DTO mapping approach?**
   **Decision:** Manual mapping via extension methods (no AutoMapper). Consistent with existing `PocoExtensions.cs` pattern.

5. **Should DTOs be reused for MCP server?**
   **Decision:** Yes. V2 DTOs designed for reuse as MCP tool schemas.

6. **Sparse fieldsets (`?fields=`)?**
   **Decision:** Out of scope. Lean DTOs eliminate the need. Adds serialization complexity for marginal benefit.

7. **Where to put V2 DTOs?**
   **Decision:** `TrashMob.Models/Poco/V2/` — keeps them near existing Pocos, follows established conventions.

---

## Monitoring & Observability

### Key Metrics to Track
- API response times (p50, p95, p99) by endpoint and version
- Error rates by type (4xx vs 5xx) and version
- V1 vs V2 traffic split (migration progress indicator)
- Response payload sizes (v1 entity vs v2 DTO)
- Mobile-specific: network bytes per session
- Cache hit rates (ETags, Phase 4)
- Rate limit violations (Phase 4)

### Dashboards
- Application Insights: API performance by version
- Sentry: Mobile crash patterns during migration
- Custom: Migration progress (% of traffic on v2)

---

## Related Documents

- **[Project 6 - Backend Standards](./Project_06_Backend_Standards.md)** - Foundation work (code modernization, auth, tests)
- **[Project 5 - Deployment](./Project_05_Deployment_Pipelines.md)** - CI/CD pipeline for client generation
- **PR #2490** (closed) - Community contribution that motivated timeline review

---

**Last Updated:** March 8, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** Before Phase 1 kickoff

---

## Changelog

- **2026-03-08:** Major revision — fixed misleading status markers (were showing checkmarks for unstarted items); documented prior work from Project 6; added "Current State" inventory; incorporated learnings from PR #2490; added server-side filtering strategy section; restructured rollout plan with per-step risk analysis and validation criteria; moved response compression to "already done"; moved sparse fieldsets and OData to out-of-scope; added per-endpoint typed filter approach
- **2026-01-31:** Added v2 DTO layer requirement with manual mapping (MCP server reuse)
- **2026-01-31:** Removed week-based schedule from rollout plan (agile approach)
- **2026-01-31:** Resolved open questions; confirmed scope items; added out-of-scope items
