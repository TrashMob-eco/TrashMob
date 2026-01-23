# TrashMob Web API & Frontend — AI Assistant Context

> **Note:** This document provides context specific to the TrashMob web application and API. For overall project context, see `/claude.md` at the repository root.

## Application Overview

This folder contains the main TrashMob web application, which includes:
- **ASP.NET Core Web API** — Backend services and REST endpoints
- **React SPA (TypeScript)** — Client-side web application in `client-app/`
- **Shared Models** — DTOs and domain models
- **Services** — Business logic layer
- **Database Context** — Entity Framework Core

## Folder Structure

```
TrashMob/
├── Controllers/              # API endpoints (REST controllers)
├── Models/                   # Domain models and DTOs
├── Services/                 # Business logic and external service integrations
├── Persistence/              # EF Core DbContext and configurations
├── Extensions/               # Extension methods and helpers
├── Filters/                  # Action filters and middleware
├── client-app/               # React TypeScript SPA
│   ├── src/
│   │   ├── components/       # Reusable React components
│   │   ├── pages/            # Page components
│   │   ├── lib/              # Utilities and helpers
│   │   ├── hooks/            # Custom React hooks
│   │   └── services/         # API client services
│   ├── package.json
│   └── vite.config.ts
├── wwwroot/                  # Static assets served by backend
├── appsettings.json          # Configuration (DO NOT commit secrets)
└── Program.cs                # Application startup and configuration
```

## Controller Patterns

### Standard REST Controller Template

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExampleController : ControllerBase
{
    private readonly IExampleService _service;
    private readonly ILogger<ExampleController> _logger;

    public ExampleController(
        IExampleService service,
        ILogger<ExampleController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets an item by ID
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>The requested item</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExampleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExampleDto>> Get(Guid id)
    {
        try
        {
            var result = await _service.GetAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item {Id}", id);
            return StatusCode(500, "An error occurred processing your request");
        }
    }

    /// <summary>
    /// Creates a new item
    /// </summary>
    /// <param name="request">The item to create</param>
    /// <returns>The created item</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExampleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExampleDto>> Create([FromBody] CreateExampleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = User.GetUserId(); // Extension method
            var result = await _service.CreateAsync(request, userId);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item");
            return StatusCode(500, "An error occurred processing your request");
        }
    }
}
```

### Controller Guidelines

1. **Always use XML comments** for Swagger documentation
2. **Include ProducesResponseType attributes** for all possible responses
3. **Use proper HTTP status codes**:
   - 200 OK — Successful GET/PUT
   - 201 Created — Successful POST with Location header
   - 204 No Content — Successful DELETE
   - 400 Bad Request — Validation errors
   - 401 Unauthorized — Missing/invalid auth token
   - 403 Forbidden — Insufficient permissions
   - 404 Not Found — Resource doesn't exist
   - 500 Internal Server Error — Unexpected errors
4. **Log errors with context** but never expose details to clients
5. **Validate input** using ModelState and data annotations
6. **Use async/await** throughout
7. **Extract user identity** early: `var userId = User.GetUserId()`

## Service Layer Patterns

### Service Interface & Implementation

```csharp
public interface IEventService
{
    Task<EventDto?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventDto>> GetUpcomingEventsAsync(double latitude, double longitude, int radiusMiles, CancellationToken cancellationToken = default);
    Task<EventDto> CreateEventAsync(CreateEventRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<EventDto> UpdateEventAsync(Guid eventId, UpdateEventRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
}

public class EventService : IEventService
{
    private readonly TrashMobDbContext _context;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ILogger<EventService> _logger;

    public EventService(
        TrashMobDbContext context,
        IMapper mapper,
        IEmailService emailService,
        ILogger<EventService> logger)
    {
        _context = context;
        _mapper = mapper;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<EventDto?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var evt = await _context.Events
            .Include(e => e.EventAttendees)
            .Include(e => e.EventPartners)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted, cancellationToken);

        return evt == null ? null : _mapper.Map<EventDto>(evt);
    }

    // Additional methods...
}
```

### Service Guidelines

1. **Accept CancellationToken** on all async methods
2. **Use Include/ThenInclude** for eager loading related data
3. **Use AsNoTracking()** for read-only queries
4. **Implement soft deletes** — check `!IsDeleted` in queries
5. **Validate business rules** before database operations
6. **Use transactions** for multi-step operations
7. **Map domain models to DTOs** — never return entities directly
8. **Handle concurrency** with optimistic locking where needed

## Database & Entity Framework

### Entity Base Class Pattern

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? LastUpdatedDate { get; set; }
    public Guid? LastUpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
}
```

### DbContext Configuration

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Event>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Description).HasMaxLength(2000);
        entity.HasIndex(e => e.EventDate);
        entity.HasIndex(e => new { e.Latitude, e.Longitude });
        entity.HasQueryFilter(e => !e.IsDeleted); // Global query filter

        entity.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    });
}
```

### Migration Best Practices

1. **Name migrations descriptively**: `Add_EventWeights_To_EventSummary`
2. **Review generated migrations** before applying
3. **Test rollback** scenarios
4. **Never edit applied migrations** — create new ones
5. **Use data migrations** for seeding/transforming data
6. **Add indexes** for foreign keys and query columns

```bash
# Create new migration
dotnet ef migrations add Add_EventWeights_To_EventSummary

# Apply migrations
dotnet ef database update

# Rollback to specific migration
dotnet ef database update Previous_Migration_Name
```

## Authentication & Authorization

### Current State (Azure B2C)
```csharp
[Authorize] // Requires valid JWT token
public class SecureController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetData()
    {
        var userId = User.GetUserId(); // ClaimsPrincipal extension
        var email = User.GetEmail();
        var isAdmin = User.IsInRole("Admin");
        
        // Use userId for queries...
    }
}
```

### User Context Extensions

```csharp
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier) 
                 ?? principal.FindFirst("sub");
        return Guid.Parse(claim?.Value ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value 
            ?? throw new UnauthorizedAccessException("Email not found");
    }
}
```

### Authorization Patterns

```csharp
// Check event ownership
var evt = await _context.Events.FindAsync(eventId);
if (evt.CreatedByUserId != userId && !User.IsInRole("Admin"))
{
    return Forbid();
}

// Check team membership
var isMember = await _context.TeamMembers
    .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
if (!isMember)
{
    return Forbid();
}
```

## React Frontend Patterns

### Component Example (TypeScript + React)

```tsx
import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { eventService } from '@/services/eventService';
import { EventDto } from '@/models';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import { NotFound } from '@/components/NotFound';
import { EventMap } from '@/components/EventMap';
import { AttendeeList } from '@/components/AttendeeList';

export const EventDetails = () => {
    const { eventId } = useParams<{ eventId: string }>();
    const navigate = useNavigate();

    const { data: event, isLoading, error } = useQuery({
        queryKey: ['event', eventId],
        queryFn: () => eventService.getEvent(eventId!),
        enabled: !!eventId,
    });

    if (isLoading) {
        return <LoadingSpinner />;
    }

    if (error || !event) {
        return <NotFound />;
    }

    return (
        <div className="event-details">
            <h1>{event.name}</h1>
            <p>{event.description}</p>
            <EventMap latitude={event.latitude} longitude={event.longitude} />
            <AttendeeList eventId={eventId!} />
        </div>
    );
};
```

### React/TypeScript Guidelines

1. **Use TypeScript** for all new code
2. **Use functional components** with hooks
3. **Use TanStack Query** for server state management
4. **Handle loading states** explicitly with `isLoading`
5. **Handle errors** gracefully with error boundaries or UI feedback
6. **Use React Router** for navigation
7. **Follow component composition** patterns
8. **Use Radix UI** for accessible components
9. **Style with Tailwind CSS** utility classes
10. **Custom hooks** for reusable logic

## API Documentation (Swagger)

### XML Documentation Requirements

All public API endpoints must have:
- `<summary>` — Brief description
- `<param>` — Parameter descriptions
- `<returns>` — Return value description
- `<response>` — Possible HTTP responses

Enable XML docs in `.csproj`:
```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

## Error Handling Strategy

### Global Exception Handler

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception occurred");

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred. Please try again later.",
            requestId = Activity.Current?.Id ?? context.TraceIdentifier
        });
    });
});
```

### Custom Exceptions

```csharp
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
    public IDictionary<string, string[]>? Errors { get; set; }
}

public class NotFoundException : Exception
{
    public NotFoundException(string entity, Guid id) 
        : base($"{entity} with ID {id} was not found") { }
}
```

## Configuration Management

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "TrashMobDatabase": "Server=..."
  },
  "AzureAdB2C": {
    "Instance": "https://...",
    "ClientId": "...",
    "Domain": "...",
    "SignUpSignInPolicyId": "..."
  },
  "SendGrid": {
    "ApiKey": "USE_USER_SECRETS_OR_ENV_VAR"
  },
  "GoogleMaps": {
    "ApiKey": "USE_USER_SECRETS_OR_ENV_VAR"
  },
  "Sentry": {
    "Dsn": "..."
  }
}
```

### User Secrets (Development)

```bash
# Set user secrets
dotnet user-secrets init
dotnet user-secrets set "SendGrid:ApiKey" "your-key"
dotnet user-secrets set "GoogleMaps:ApiKey" "your-key"
```

## Testing

### Unit Test Example (xUnit)

```csharp
public class EventServiceTests
{
    private readonly Mock<TrashMobDbContext> _contextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<EventService>> _loggerMock;
    private readonly EventService _sut;

    public EventServiceTests()
    {
        _contextMock = new Mock<TrashMobDbContext>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<EventService>>();
        _sut = new EventService(_contextMock.Object, _mapperMock.Object, null, _loggerMock.Object);
    }

    [Fact]
    public async Task GetEventAsync_WhenEventExists_ReturnsEventDto()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var evt = new Event { Id = eventId, Name = "Test Event" };
        var eventDto = new EventDto { Id = eventId, Name = "Test Event" };
        
        // Mock DbSet setup...
        _mapperMock.Setup(m => m.Map<EventDto>(evt)).Returns(eventDto);

        // Act
        var result = await _sut.GetEventAsync(eventId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventId, result.Id);
    }
}
```

## Performance Considerations

### Query Optimization

```csharp
// ✅ GOOD: Single query with includes
var events = await _context.Events
    .Include(e => e.EventAttendees)
    .Include(e => e.EventPartners)
    .Where(e => e.EventDate >= DateTime.UtcNow)
    .AsNoTracking()
    .ToListAsync();

// ❌ BAD: N+1 query problem
var events = await _context.Events.ToListAsync();
foreach (var evt in events)
{
    var attendees = await _context.EventAttendees
        .Where(a => a.EventId == evt.Id)
        .ToListAsync(); // Executes query for each event
}
```

### Pagination Pattern

```csharp
public async Task<PagedResult<EventDto>> GetEventsAsync(int page, int pageSize)
{
    var query = _context.Events.Where(e => !e.IsDeleted);
    
    var total = await query.CountAsync();
    var items = await query
        .OrderByDescending(e => e.EventDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .AsNoTracking()
        .ToListAsync();

    return new PagedResult<EventDto>
    {
        Items = _mapper.Map<List<EventDto>>(items),
        TotalCount = total,
        Page = page,
        PageSize = pageSize
    };
}
```

## Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| **Circular references in JSON** | Use `[JsonIgnore]` or DTOs to break cycles |
| **Timezone issues** | Always use `DateTimeOffset` or store UTC |
| **N+1 queries** | Use `.Include()` for eager loading |
| **Tracking overhead** | Use `.AsNoTracking()` for read-only queries |
| **Large result sets** | Implement pagination |
| **Sensitive data in logs** | Use structured logging with filters |
| **Concurrent updates** | Use optimistic concurrency with `RowVersion` |

## Project-Specific Patterns

### Minors Protection (2026 Initiative)

```csharp
// Check if user is a minor
public static bool IsMinor(this User user)
{
    return user.DateOfBirth.HasValue 
        && user.DateOfBirth.Value.AddYears(18) > DateTimeOffset.UtcNow;
}

// Ensure event visibility restrictions for minors
if (currentUser.IsMinor())
{
    // Don't expose contact info
    eventDto.ContactEmail = null;
    eventDto.ContactPhone = null;
    
    // Filter attendee list
    eventDto.Attendees = eventDto.Attendees
        .Select(a => new AttendeeDto { FirstName = "Volunteer" })
        .ToList();
}
```

### Waiver Validation

```csharp
public async Task<bool> HasValidWaiverAsync(Guid userId, Guid? communityId = null)
{
    var now = DateTimeOffset.UtcNow;
    
    return await _context.UserWaivers
        .AnyAsync(w => w.UserId == userId
            && (communityId == null || w.CommunityId == communityId)
            && w.EffectiveDate <= now
            && w.ExpirationDate >= now
            && w.IsActive);
}
```

## Deployment Notes

### Environment Variables (Production)

Set in Azure App Service Configuration:
- `ConnectionStrings__TrashMobDatabase`
- `AzureAdB2C__ClientId`
- `SendGrid__ApiKey`
- `GoogleMaps__ApiKey`
- `Sentry__Dsn`
- `ASPNETCORE_ENVIRONMENT` (Production)

### Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<TrashMobDbContext>()
    .AddUrlGroup(new Uri("https://api.sendgrid.com/v3"), "SendGrid");

app.MapHealthChecks("/health");
```

## Quick Reference

### Useful Commands

```bash
# Run locally
dotnet run

# Watch mode (auto-reload)
dotnet watch run

# Run tests
dotnet test

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Generate EF model from database
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer

# Build for production
dotnet publish -c Release
```

---

**Related Documentation:**
- Root `/claude.md` — Overall project context
- `/TrashMob_2026_Product_Engineering_Plan.md` — 2026 roadmap and priorities
- `/README.md` — Project setup and getting started

**Last Updated:** January 23, 2026


## Accessibility

- Commit to **WCAG 2.2 AA** compliance
- Semantic HTML on web
- Proper screen reader support on mobile
- Keyboard navigation on web
- Sufficient color contrast ratios

## Key 2026 Initiatives

Refer to `TrashMob_2026_Product_Engineering_Plan.md` for detailed roadmap. Priority areas:

1. **Project 1:** Auth migration (Azure B2C → Entra External ID)
2. **Project 4:** Mobile stabilization and error handling
3. **Project 7:** Event weight tracking (Phase 1 & 2)
4. **Project 9:** Teams feature (MVP)
5. **Project 10:** Community Pages (MVP)

## Development Workflow

### Branching Strategy
- `main` — production-ready code
- `dev` — integration branch
- `dev/{developer}/{feature}` — feature branches

### Commit Messages
- Use clear, descriptive messages
- Reference issue numbers where applicable

### Pull Requests
- Require review before merge
- Must pass all CI checks
- Include tests for new features

## Common Patterns & Examples

### Controller Example

````````
{
  "Event": {
    "Id": "123",
    "Title": "River Cleanup",
    "Description": "Join us for a cleanup of the local river.",
    "Location": {
      "Latitude": 34.0522,
      "Longitude": -118.2437
    },
    "Date": "2023-10-01T10:00:00Z",
    "Duration": 120,
    "VolunteersNeeded": 10,
    "Status": "Upcoming",
    "CreatedDate": "2023-09-01T12:00:00Z",
    "CreatedByUserId": "456",
    "LastUpdatedDate": "2023-09-15T12:00:00Z",
    "LastUpdatedByUserId": "456"
  },
  "Volunteer": {
    "Id": "456",
    "FirstName": "John",
    "LastName": "Doe",
    "Email": "john.doe@example.com",
    "Phone": "555-1234",
    "IsAdmin": false,
    "CommunityId": "789",
    "ConsentToContact": true,
    "CreatedDate": "2023-09-01T12:00:00Z",
    "LastLoginDate": "2023-09-20T12:00:00Z"
  }
}
````````

### Service Example


````````

## Testing

- **Unit Tests:** Business logic in services
- **Integration Tests:** API endpoints with test database
- **Manual Testing:** Mobile apps on physical devices
- Target **change failure rate ≤ 10%**

## Performance Goals

- **P95 API latency:** ≤ 300ms
- **Crash-free sessions (mobile):** ≥ 99.5%
- **Database queries:** Use proper indexing, avoid N+1 queries
- **Caching:** Implement where appropriate (Redis consideration)

## Observability

- **Sentry.io** for error tracking
- **Structured logging** with context
- **Business event tracking** (signups, event creation, attendance)
- **Dashboards** for key metrics
- **Alerting** for critical issues

## Cost Optimization

Monitor and optimize:
- Azure App Service costs
- Database DTU/vCore usage
- Google Maps API calls
- SendGrid email volume

## Getting Help

- **Product Plan:** `TrashMob_2026_Product_Engineering_Plan.md`
- **GitHub Issues:** Track bugs and features
- **Project Wiki:** (if available)
- **Code Comments:** Check inline documentation

## AI Assistant Guidelines

When working with this codebase:

1. **Respect existing patterns** — maintain consistency with current code style
2. **Consider volunteer context** — code should be maintainable by contributors with varying experience
3. **Think security-first** — especially for auth, minors protection, and data privacy
4. **Plan for scale** — features should work for 1 event or 10,000 events
5. **Mobile-first considerations** — ensure features work well on mobile devices
6. **Accessibility by default** — build inclusive features from the start
7. **Document as you go** — clear comments and XML docs are essential
8. **Test thoroughly** — volunteer-run project needs reliable code

---

**Last Updated:** January 23, 2026  
**For Questions:** Refer to product engineering plan or project maintainers

