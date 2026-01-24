# Project 17 — TrashMob.eco MCP Server (AI)

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Low |
| **Risk** | Moderate |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Provide safe, privacy-aware AI access to events/metrics via Model Context Protocol (MCP) for natural language queries. This enables AI assistants to help users find events, understand impact, and engage with the platform.

---

## Objectives

### Primary Goals
- **MCP server** exposing scoped metrics and events
- **Privacy constraints** and data anonymization
- **Read-only access** to public data
- **Natural language event search**

### Secondary Goals
- AI-powered event recommendations
- Impact report generation
- Community insights
- Volunteer matching suggestions

---

## Scope

### Phase 1 - Core MCP Server
- ✅ MCP server implementation (.NET or Node.js)
- ✅ Event search tool (location, date, type)
- ✅ Stats/metrics tool (sitewide, community)
- ✅ Authentication via API tokens

### Phase 2 - Enhanced Tools
- ✅ User dashboard data (authenticated)
- ✅ Team and community lookup
- ✅ Litter report discovery
- ✅ Partner/location search

### Phase 3 - AI Features
- ❓ Event recommendations
- ❓ Impact summaries
- ❓ Volunteer activity analysis
- ❓ Community health metrics

---

## Out-of-Scope

- ❌ Write operations (creating events, etc.)
- ❌ Private user data access
- ❌ Real-time event updates
- ❌ AI model training on user data
- ❌ Chatbot UI (use existing AI clients)

---

## Success Metrics

### Quantitative
- **MCP tool calls:** Track usage
- **Response latency:** < 500ms for queries
- **Error rate:** < 1%

### Qualitative
- AI assistants can effectively find events
- No privacy incidents
- Positive developer feedback

---

## Dependencies

### Blockers
None - can use existing API

### Enables
- AI-assisted volunteer recruitment
- Natural language platform access
- Developer ecosystem

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Data privacy exposure** | Medium | High | Read-only; anonymization; no PII in responses |
| **API abuse** | Medium | Medium | Rate limiting; token-based auth; monitoring |
| **AI hallucinations** | Medium | Low | Structured responses; validation; disclaimers |
| **Maintenance burden** | Low | Medium | Simple tool set; automated testing |

---

## Implementation Plan

### MCP Server Structure

```
TrashMobMCP/
├── Program.cs                 # MCP server entry point
├── Tools/
│   ├── EventSearchTool.cs    # Search events by criteria
│   ├── StatsTool.cs          # Get platform/community stats
│   ├── TeamSearchTool.cs     # Find teams
│   ├── CommunityTool.cs      # Community information
│   └── LitterReportTool.cs   # Litter report lookup
├── Resources/
│   └── EventResource.cs      # Event data resource
└── appsettings.json
```

### MCP Tools

**Event Search Tool:**
```csharp
[McpTool("search_events")]
public class EventSearchTool
{
    [McpParameter("location", "City, state, or coordinates")]
    public string? Location { get; set; }

    [McpParameter("radius_miles", "Search radius in miles")]
    public int RadiusMiles { get; set; } = 25;

    [McpParameter("start_date", "Events starting after this date")]
    public DateTime? StartDate { get; set; }

    [McpParameter("event_type", "Type of cleanup event")]
    public string? EventType { get; set; }

    public async Task<EventSearchResult> Execute(IEventManager eventManager)
    {
        // Search events using existing manager
        // Return sanitized, public-only data
    }
}
```

**Stats Tool:**
```csharp
[McpTool("get_stats")]
public class StatsTool
{
    [McpParameter("scope", "sitewide, community, or team")]
    public string Scope { get; set; } = "sitewide";

    [McpParameter("community_id", "Community ID if scope is community")]
    public Guid? CommunityId { get; set; }

    public async Task<StatsResult> Execute(IStatsManager statsManager)
    {
        // Return aggregated, anonymized statistics
    }
}
```

**Team Search Tool:**
```csharp
[McpTool("search_teams")]
public class TeamSearchTool
{
    [McpParameter("location", "Location to search near")]
    public string? Location { get; set; }

    [McpParameter("name", "Team name to search for")]
    public string? Name { get; set; }

    public async Task<TeamSearchResult> Execute(ITeamManager teamManager)
    {
        // Search public teams
    }
}
```

### API Integration

```csharp
// MCP server calls existing API managers
public class McpApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiToken;

    public async Task<IEnumerable<Event>> SearchEventsAsync(EventSearchCriteria criteria)
    {
        // Call TrashMob API with service token
        var response = await _httpClient.GetAsync($"/api/events?{criteria.ToQueryString()}");
        return await response.Content.ReadFromJsonAsync<IEnumerable<Event>>();
    }
}
```

### Privacy & Security

```csharp
public class DataSanitizer
{
    public EventDto Sanitize(Event event)
    {
        return new EventDto
        {
            Id = event.Id,
            Name = event.Name,
            Description = event.Description,
            EventDate = event.EventDate,
            City = event.City,
            Region = event.Region,
            // NO: Email, exact address, attendee details
            AttendeeCount = event.EventAttendees.Count, // Count only
        };
    }
}
```

---

## Implementation Phases

### Phase 1: Core Server
- MCP server setup
- Event search tool
- Stats tool
- Basic authentication

### Phase 2: Extended Tools
- Team and community tools
- Litter report tool
- User dashboard (authenticated)

### Phase 3: Optimization
- Caching
- Rate limiting
- Monitoring

**Note:** Low priority; implement when AI ecosystem matures.

---

## Example Interactions

**User:** "Find cleanup events near Seattle this weekend"

**MCP Tool Call:**
```json
{
  "tool": "search_events",
  "parameters": {
    "location": "Seattle, WA",
    "radius_miles": 25,
    "start_date": "2026-01-25",
    "end_date": "2026-01-26"
  }
}
```

**Response:**
```json
{
  "events": [
    {
      "name": "Green Lake Park Cleanup",
      "date": "2026-01-25T10:00:00",
      "city": "Seattle",
      "type": "Park Cleanup",
      "attendee_count": 12,
      "url": "https://trashmob.eco/events/abc123"
    }
  ],
  "total_count": 3
}
```

---

## Open Questions

1. **Hosting model?**
   **Recommendation:** Separate container app; or integrate into main API
   **Owner:** Engineering
   **Due:** Before Phase 1

2. **Authentication mechanism?**
   **Recommendation:** API tokens for server-to-server; OAuth for user context
   **Owner:** Engineering
   **Due:** Before Phase 1

3. **Which AI platforms to target?**
   **Recommendation:** Claude (Anthropic), ChatGPT (OpenAI), Copilot (Microsoft)
   **Owner:** Product Lead
   **Due:** Before launch

4. **Rate limits?**
   **Recommendation:** 100 calls/minute per token; higher for partners
   **Owner:** Engineering
   **Due:** Before Phase 1

---

## Related Documents

- **[MCP Specification](https://modelcontextprotocol.io)** - Protocol documentation
- **[Project 6 - Backend Standards](./Project_06_Backend_Standards.md)** - API patterns
- **TrashMob API** - Existing endpoints to wrap

---

**Last Updated:** January 24, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** When AI integration becomes priority
