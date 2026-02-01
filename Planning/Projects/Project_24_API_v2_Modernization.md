# Project 24 � API v2 Modernization

| Attribute | Value |
|-----------|-------|
| **Status** | Not started |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Very Large |

## Business Rationale

Create a modern, scalable, and developer-friendly API layer (v2) that improves reliability, debuggability, and reduces manual code maintenance. Current v1 APIs lack pagination, consistent error handling, and require significant manual client code in both React and mobile apps. Auto-generated clients will accelerate development and reduce bugs.

## Objectives

### Core Improvements:
- Implement pagination on all collection endpoints (cursor and offset-based)
- Standardized error responses (RFC 9457 Problem Details)
- Auto-generate TypeScript clients for React app
- Auto-generate .NET clients for MAUI mobile app
- Comprehensive OpenAPI 3.1 documentation
- Correlation IDs for distributed tracing
- Improved observability and debugging

### Modernization Strategies:
- API versioning via URL path (`/api/v2/events`)
- Response compression (Gzip/Brotli)
- ETags for conditional requests and caching
- Rate limiting with token buckets
- Result wrapper pattern for consistent responses
- Bulk operations support
- Webhook infrastructure for event notifications
- Enhanced health checks with dependencies
- Field filtering and sparse fieldsets (`?fields=name,date`)
- Sorting query parameters (`?sort=-date,name`)
- Advanced filtering (OData-style simplified queries)

## Scope

### Phase 1 - Foundation (Q2 2026):
- ✅ API versioning infrastructure
- ✅ Pagination framework (offset default, cursor optional)
- ✅ Problem Details error responses (RFC 9457, formerly RFC 7807)
- ✅ OpenAPI 3.1 specification
- ✅ NSwag (TypeScript) + Kiota (.NET) for client generation
- ✅ Response wrapper pattern
- ✅ Correlation ID middleware

### Phase 2 - Core Endpoints (Q3 2026):
- ✅ Events v2 endpoints with pagination
- ✅ Users v2 endpoints
- ✅ Partners v2 endpoints
- ✅ EventAttendees v2 endpoints
- ✅ EventSummaries v2 endpoints
- ✅ LitterReports v2 endpoints
- ✅ Auto-generated TypeScript client (NSwag)
- ✅ Auto-generated .NET MAUI client (Kiota)

### Phase 3 - Advanced Features (Q4 2026):
- ✅ Response compression (Gzip/Brotli)
- ✅ ETags and conditional requests
- ✅ Bulk operations (batch create/update)
- ✅ Webhook infrastructure
- ✅ Field filtering (`?fields=name,date`)
- ✅ Sorting (`?sort=-date,name`)
- ✅ Advanced filtering (OData subset)

## Out-of-Scope

- ❌ v1 endpoint removal (maintain both versions until mobile apps updated)
- ❌ GraphQL API (evaluate separately if demand exists)
- ❌ API Gateway (defer until microservices architecture needed)
- ❌ gRPC endpoints (not needed for current use cases)
- ❌ External API access (public developer API) - internal use only for now

## Success Metrics

- API response times ? 200ms (p95)
- Auto-generated clients reduce manual API code by 70%
- Error resolution time decreased by 50% (via correlation IDs)
- Developer onboarding time reduced (better docs + clients)
- Mobile app code reduction by eliminating manual DTOs
- React app API service code reduction by 60%
- Zero breaking changes to v1 endpoints during transition

## Dependencies

- OpenAPI documentation (enhanced)
- CI/CD pipeline updates for client generation
- Developer documentation for v2 adoption
- Mobile app refactor to use generated clients

## Risks & Mitigations

**Risk:** Maintaining two API versions increases complexity  
**Mitigation:** Shared business logic layer; automated deprecation warnings; 12-month transition plan

**Risk:** Client generation doesn't handle all edge cases  
**Mitigation:** Manual client overrides where needed; comprehensive testing; fallback patterns

**Risk:** Breaking changes during development  
**Mitigation:** Semantic versioning; changelogs; beta labels during stabilization

**Risk:** Team learning curve for new patterns  
**Mitigation:** Documentation; examples; pair programming sessions

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
    public string? NextPage { get; set; }
    public string? PreviousPage { get; set; }
}

public class CursorPagedResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    public string? NextCursor { get; set; }
    public string? PreviousCursor { get; set; }
    public bool HasMore { get; set; }
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
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class EventsV2Controller : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsV2Controller> _logger;

    /// <summary>
    /// Gets a paginated list of events
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page (max 100)</param>
    /// <param name="sort">Sort expression (e.g., "-date,name")</param>
    /// <param name="filter">Filter expression (e.g., "status eq 'active'")</param>
    /// <param name="fields">Comma-separated fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated event list</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<EventDto>>> GetEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sort = null,
        [FromQuery] string? filter = null,
        [FromQuery] string? fields = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _eventService.GetEventsPagedAsync(
                page, pageSize, sort, filter, fields, cancellationToken);
            
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation Error",
                detail: ex.Message,
                instance: HttpContext.Request.Path
            );
        }
    }
}
```

### Correlation ID Middleware

```csharp
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                         ?? Guid.NewGuid().ToString();
        
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Add("X-Correlation-ID", correlationId);
        
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await _next(context);
        }
    }
}
```

### Rate Limiting (ASP.NET Core 7+)

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", config =>
    {
        config.PermitLimit = 100;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 10;
    });
});

[EnableRateLimiting("api")]
public class EventsV2Controller : ControllerBase { }
```

### Auto-Generated TypeScript Client Usage

```typescript
// Generated by NSwag/Kiota
import { EventsClient, PagedResponseOfEventDto } from '@/api/generated';

export const useEvents = () => {
    const client = new EventsClient();
    
    return useQuery({
        queryKey: ['events', page, pageSize],
        queryFn: () => client.getEvents(page, pageSize)
    });
};
```

### Auto-Generated .NET Client Usage (Mobile)

```csharp
// Generated by NSwag/Kiota
using TrashMob.Api.Generated;

public class EventsViewModel : BaseViewModel
{
    private readonly IEventsClient _eventsClient;

    public async Task LoadEventsAsync()
    {
        var response = await _eventsClient.GetEventsAsync(
            page: CurrentPage,
            pageSize: PageSize
        );
        
        Events.Clear();
        foreach (var evt in response.Items)
        {
            Events.Add(evt);
        }
        
        HasMore = response.Pagination.HasNext;
    }
}
```

### OpenAPI Configuration

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TrashMob API v1",
        Version = "v1",
        Description = "Legacy API (deprecated, use v2)"
    });
    
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "TrashMob API v2",
        Version = "v2",
        Description = "Modern API with pagination, error handling, and enhanced features",
        Contact = new OpenApiContact
        {
            Name = "TrashMob Support",
            Email = "api@trashmob.eco",
            Url = new Uri("https://www.trashmob.eco/help")
        }
    });

    // XML comments for documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Add examples
    options.ExampleFilters();
    
    // Add operation filters for correlation IDs
    options.OperationFilter<CorrelationIdHeaderFilter>();
});
```

### Client Generation CI/CD (GitHub Actions)

```yaml
name: Generate API Clients

on:
  push:
    paths:
      - 'TrashMob/Controllers/V2/**'
      - 'TrashMob/Models/**'

jobs:
  generate-clients:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      # Generate OpenAPI spec
      - name: Generate OpenAPI JSON
        run: |
          dotnet swagger tofile --output openapi-v2.json \
            TrashMob/bin/Release/net10.0/TrashMob.dll v2
      
      # Generate TypeScript client
      - name: Generate TypeScript Client
        run: |
          npx nswag openapi2tsclient \
            /input:openapi-v2.json \
            /output:TrashMob/client-app/src/api/generated/client.ts \
            /template:Fetch \
            /generateClientClasses:true \
            /generateDtoTypes:true
      
      # Generate .NET client for mobile
      - name: Generate .NET Client
        run: |
          dotnet tool install --global Microsoft.Kiota
          kiota generate \
            --openapi openapi-v2.json \
            --output TrashMobMobile/Generated/ApiClient \
            --language CSharp \
            --class-name TrashMobApiClient
      
      - name: Commit generated clients
        run: |
          git config user.name "API Client Generator"
          git add TrashMob/client-app/src/api/generated/
          git add TrashMobMobile/Generated/ApiClient/
          git commit -m "chore: regenerate API clients"
          git push
```

## Rollout Plan

### Step 1 - Infrastructure
1. Set up API versioning middleware
2. Implement pagination framework
3. Configure OpenAPI 3.1
4. Add Problem Details error handling
5. Deploy correlation ID middleware

### Step 2 - Pilot Endpoint
1. Create Events v2 controller as pilot
2. Generate and test TypeScript client
3. Generate and test .NET client
4. Migrate one web page to use v2
5. Migrate one mobile screen to use v2

### Step 3 - Core Endpoints
1. Create remaining v2 controllers (Users, Partners, etc.)
2. Update documentation

### Step 4 - Web Migration
1. Migrate all React pages to v2 incrementally
2. Remove manual API service code

### Step 5 - Mobile Migration
1. Migrate all mobile screens to v2
2. Remove manual DTO classes

### Step 6 - Advanced Features
1. Add response compression
2. Implement ETags
3. Add bulk operations
4. Webhook infrastructure

### Step 7 - Stabilization
1. Bug fixes and performance optimization
2. v1 deprecation announcements

## Monitoring & Observability

### Key Metrics to Track:
- API response times (p50, p95, p99) by endpoint
- Error rates by type (4xx vs 5xx)
- Client generation success rate in CI/CD
- Correlation ID tracing coverage
- Cache hit rates (ETags)
- Rate limit violations
- Webhook delivery success rates

### Dashboards:
- API v2 performance dashboard (Grafana)
- Error tracking (Sentry with correlation IDs)
- Usage analytics (which endpoints, which versions)
- Migration progress (v1 vs v2 traffic)

## Documentation

### Developer Resources:
1. **API v2 Migration Guide** - Step-by-step for moving from v1 to v2
2. **OpenAPI Documentation** - Interactive Swagger UI
3. **Client Generation Guide** - How to regenerate clients
4. **Examples Repository** - Code samples for common scenarios
5. **Changelog** - Detailed version history
6. **Deprecation Timeline** - v1 sunset schedule

### Auto-Generated:
- TypeScript client documentation (TSDoc)
- .NET client documentation (XML docs)
- Postman collection
- OpenAPI specification JSON/YAML

## Decisions

1. **Client generation tools?**
   **Decision:** NSwag for TypeScript (React), Kiota for .NET (MAUI) - best of both worlds

2. **Query filtering syntax?**
   **Decision:** Simplified OData subset for consistency with industry standards (Azure APIs)

3. **Pagination default?**
   **Decision:** Offset-based default for simplicity; cursor-based available as option for real-time feeds

4. **API Gateway?**
   **Decision:** Defer until microservices architecture needed (out of scope for v2)

---

**Last Updated:** January 31, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** Before Phase 1 kickoff

---

## Changelog

- **2026-01-31:** Removed week-based schedule from rollout plan (agile approach)
- **2026-01-31:** Resolved open questions; confirmed all Phase 1-3 scope items; added out-of-scope items
