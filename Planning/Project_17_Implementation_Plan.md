# Project 17 Implementation Plan: TrashMob MCP Server

## Overview

Build a Model Context Protocol (MCP) server that exposes TrashMob data to AI assistants like Claude, enabling natural language queries for events, stats, teams, and litter reports.

---

## Phase 1: Core MCP Server Setup

### 1.1 Create TrashMobMCP Project

Create new .NET 10 console application:

```
TrashMobMCP/
├── TrashMobMCP.csproj
├── Program.cs
├── appsettings.json
├── Dockerfile
├── Tools/
│   ├── SearchEventsTool.cs
│   ├── GetStatsTool.cs
│   ├── SearchTeamsTool.cs
│   └── SearchLitterReportsTool.cs
└── Sanitizers/
    └── DataSanitizer.cs
```

**Dependencies:**
- `ModelContextProtocol` (official MCP .NET SDK)
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Logging.Console`
- Project references: `TrashMob.Shared`, `TrashMob.Models`

### 1.2 MCP Server Entry Point

```csharp
// Program.cs
var builder = Host.CreateApplicationBuilder(args);

// Register TrashMob services
ServiceBuilder.AddManagers(builder.Services);
ServiceBuilder.AddRepositories(builder.Services);
builder.Services.AddDbContext<MobDbContext>();

// Register MCP tools
builder.Services.AddMcpServer()
    .WithStdioTransport()
    .WithTool<SearchEventsTool>()
    .WithTool<GetStatsTool>()
    .WithTool<SearchTeamsTool>()
    .WithTool<SearchLitterReportsTool>();

var host = builder.Build();
await host.RunAsync();
```

### 1.3 Implement Core Tools

**Tool 1: search_events**
- Parameters: location, radius_miles, start_date, end_date, event_type
- Uses: `IEventManager.GetFilteredEventsAsync()`
- Returns: Sanitized event list (no PII)

**Tool 2: get_stats**
- Parameters: scope (sitewide/community/team), community_id, team_id
- Uses: `IEventSummaryManager.GetStatsAsync()`
- Returns: Aggregate statistics (bags, hours, events, participants)

**Tool 3: search_teams**
- Parameters: location, radius_miles, name
- Uses: `ITeamManager.GetPublicTeamsAsync()`
- Returns: Public team info

**Tool 4: search_litter_reports**
- Parameters: location, status, start_date, end_date
- Uses: `ILitterReportManager.GetFilteredLitterReportsAsync()`
- Returns: Sanitized litter report list

---

## Phase 2: Data Sanitization & Privacy

### 2.1 DataSanitizer Class

Ensure no PII is exposed:

```csharp
public class DataSanitizer
{
    public EventDto Sanitize(Event e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        EventDate = e.EventDate,
        City = e.City,
        Region = e.Region,
        Country = e.Country,
        EventType = e.EventType?.Name,
        AttendeeCount = e.EventAttendees?.Count ?? 0,
        Url = $"https://trashmob.eco/eventdetails/{e.Id}"
        // NO: CreatedByUserId, Email, exact address, attendee names
    };
}
```

### 2.2 Response DTOs

Create minimal DTOs for MCP responses:
- `EventDto` - Public event info only
- `TeamDto` - Public team info only
- `StatsDto` - Aggregate numbers only
- `LitterReportDto` - Location and status only

---

## Phase 3: Deployment Infrastructure

### 3.1 Dockerfile

Multi-stage build (same pattern as existing jobs):

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TrashMobMCP/TrashMobMCP.csproj", "TrashMobMCP/"]
COPY ["TrashMob.Shared/TrashMob.Shared.csproj", "TrashMob.Shared/"]
COPY ["TrashMob.Models/TrashMob.Models.csproj", "TrashMob.Models/"]
RUN dotnet restore "TrashMobMCP/TrashMobMCP.csproj"
COPY . .
WORKDIR "/src/TrashMobMCP"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrashMobMCP.dll"]
```

### 3.2 Container App (NOT Job)

Unlike DailyJobs/HourlyJobs, MCP server is a long-running service:
- Container App (not Container App Job)
- External ingress on custom domain: `mcp.trashmob.eco`
- Min replicas: 1 (always running)
- HTTP transport for MCP (not stdio)

### 3.3 Bicep Template: containerAppMCP.bicep

```bicep
resource containerAppMCP 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'ca-mcp-tm-${environment}-${region}'
  location: region
  identity: { type: 'SystemAssigned' }
  properties: {
    environmentId: containerAppsEnvironmentId
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
      }
    }
    template: {
      containers: [{
        name: 'mcp-server'
        image: containerImage
        resources: { cpu: json('0.5'), memory: '1Gi' }
        env: [
          { name: 'ASPNETCORE_ENVIRONMENT', value: 'Production' }
          { name: 'TMDBServerConnectionString', secretRef: 'db-connection-string' }
        ]
      }]
      scale: {
        minReplicas: 1
        maxReplicas: 3
      }
    }
  }
}
```

### 3.4 GitHub Workflow

Trigger on changes to TrashMobMCP/ folder:
1. Build and push Docker image
2. Deploy via Bicep
3. Assign RBAC roles to managed identity

---

## Phase 4: Authentication & Rate Limiting

### 4.1 API Token Authentication

- Users request API tokens from account settings
- Tokens stored in database (hashed)
- MCP server validates token on each request
- Rate limit: 50 calls/minute per token

### 4.2 Token Validation Middleware

```csharp
public class McpAuthMiddleware
{
    public async Task ValidateToken(string token)
    {
        var hashedToken = HashToken(token);
        var apiToken = await _tokenManager.GetByHashAsync(hashedToken);
        if (apiToken == null || apiToken.IsRevoked)
            throw new UnauthorizedException();

        await _tokenManager.RecordUsageAsync(apiToken.Id);
    }
}
```

---

## Implementation Order

1. **Week 1:** Create TrashMobMCP project structure, implement search_events tool
2. **Week 2:** Implement remaining tools (get_stats, search_teams, search_litter_reports)
3. **Week 3:** Add DataSanitizer, create DTOs, write unit tests
4. **Week 4:** Create Dockerfile, Bicep template, GitHub workflow
5. **Week 5:** Deploy to dev, test with Claude Desktop
6. **Week 6:** Add authentication, rate limiting, deploy to production

---

## Files to Create

| File | Purpose |
|------|---------|
| `TrashMobMCP/TrashMobMCP.csproj` | Project file |
| `TrashMobMCP/Program.cs` | MCP server entry point |
| `TrashMobMCP/appsettings.json` | Configuration |
| `TrashMobMCP/Dockerfile` | Container build |
| `TrashMobMCP/Tools/SearchEventsTool.cs` | Event search MCP tool |
| `TrashMobMCP/Tools/GetStatsTool.cs` | Stats MCP tool |
| `TrashMobMCP/Tools/SearchTeamsTool.cs` | Team search MCP tool |
| `TrashMobMCP/Tools/SearchLitterReportsTool.cs` | Litter report MCP tool |
| `TrashMobMCP/Sanitizers/DataSanitizer.cs` | Privacy sanitization |
| `TrashMobMCP/Dtos/EventDto.cs` | Event response DTO |
| `TrashMobMCP/Dtos/StatsDto.cs` | Stats response DTO |
| `TrashMobMCP/Dtos/TeamDto.cs` | Team response DTO |
| `Deploy/containerAppMCP.bicep` | Azure deployment |
| `.github/workflows/container_mcp-tm-dev-westus2.yml` | Dev deployment |

---

## Success Criteria

- [ ] MCP server starts and responds to tool discovery
- [ ] search_events returns events near a location
- [ ] get_stats returns platform statistics
- [ ] search_teams returns public teams
- [ ] search_litter_reports returns litter reports
- [ ] No PII exposed in any response
- [ ] Claude Desktop can connect and query events
- [ ] Response latency < 500ms
- [ ] Deployed to dev environment

---

## Open Questions Resolved

1. **Transport:** HTTP (not stdio) for cloud deployment
2. **Hosting:** Container App (not Job) - long-running server
3. **Auth:** API tokens validated per-request
4. **Rate Limits:** 50 calls/min per token (enforced in middleware)

---

## Notes

- MCP .NET SDK: https://github.com/modelcontextprotocol/csharp-sdk
- MCP Specification: https://modelcontextprotocol.io/specification
- Existing managers provide all needed data access
- DataSanitizer ensures privacy compliance
