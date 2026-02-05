# TrashMob Web API & Frontend — AI Assistant Context

> **Note:** For overall project context, architecture, and coding standards, see [/CLAUDE.md](../CLAUDE.md) at the repository root. This document covers patterns specific to the web application.

## Application Overview

This folder contains the main TrashMob web application:
- **ASP.NET Core Web API** — Backend services and REST endpoints
- **React SPA (TypeScript)** — Client-side web application in `client-app/`

## Folder Structure

```
TrashMob/
├── Controllers/              # API endpoints (REST controllers)
├── client-app/               # React TypeScript SPA
│   ├── src/
│   │   ├── components/       # Reusable React components
│   │   ├── pages/            # Page components
│   │   ├── lib/              # Utilities and helpers
│   │   ├── hooks/            # Custom React hooks
│   │   └── services/         # API client services
│   ├── package.json
│   └── vite.config.ts
├── wwwroot/                  # Static assets
├── appsettings.json          # Configuration (DO NOT commit secrets)
└── Program.cs                # Application startup
```

## Controller Patterns

### Standard REST Controller Template

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExampleController : SecureController
{
    private readonly IExampleManager _manager;

    public ExampleController(IExampleManager manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// Gets an item by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExampleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExampleDto>> Get(Guid id)
    {
        var result = await _manager.GetAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExampleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExampleDto>> Create([FromBody] CreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _manager.CreateAsync(request, UserId);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }
}
```

### Controller Guidelines

- Use XML comments for Swagger documentation
- Include `ProducesResponseType` attributes for all responses
- Use proper HTTP status codes (200, 201, 204, 400, 401, 403, 404, 500)
- Inherit from `SecureController` to access `UserId` property
- Use async/await throughout

## React Frontend Patterns

### Component Example

```tsx
import { useParams } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { GetEvent } from '@/services/events';

export const EventDetails = () => {
    const { eventId } = useParams<{ eventId: string }>();

    const { data: event, isLoading, error } = useQuery({
        queryKey: GetEvent({ id: eventId! }).key,
        queryFn: GetEvent({ id: eventId! }).service,
        enabled: !!eventId,
    });

    if (isLoading) return <LoadingSpinner />;
    if (error || !event) return <NotFound />;

    return (
        <div className="event-details">
            <h1>{event.name}</h1>
            <p>{event.description}</p>
        </div>
    );
};
```

### Frontend Guidelines

- Use TypeScript for all new code
- Use functional components with hooks
- Use TanStack Query for server state management
- Handle loading/error states explicitly
- Use Radix UI for accessible components
- Style with Tailwind CSS utility classes

### Service Pattern

```typescript
// src/services/example.ts
export type GetExample_Params = { id: string };
export type GetExample_Response = ExampleDto;

export const GetExample = (params: GetExample_Params) => ({
    key: ['/example', params.id],
    service: async () =>
        ApiService('protected').fetchData<GetExample_Response>({
            url: `/example/${params.id}`,
            method: 'get',
        }),
});
```

## Authentication

### Getting User Context

```csharp
// In SecureController-derived classes
var userId = UserId;  // From SecureController base class

// Check ownership
if (entity.CreatedByUserId != UserId)
    return Forbid();
```

## Quick Reference

### Run Locally

```bash
# Backend (from TrashMob folder)
dotnet run --environment Development

# Frontend (from TrashMob/client-app folder)
npm start
```

### Local URLs
- API: https://localhost:44332
- Swagger: https://localhost:44332/swagger/index.html
- Frontend: http://localhost:3000

---

**Related Documentation:**
- [Root CLAUDE.md](../CLAUDE.md) — Architecture, patterns, coding standards
- [Planning/README.md](../Planning/README.md) — 2026 roadmap
- [TrashMob.prd](./TrashMob.prd) — Product requirements

**Last Updated:** February 4, 2026
