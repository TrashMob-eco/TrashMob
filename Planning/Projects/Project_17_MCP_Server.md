# Project 17 — TrashMob.eco MCP Server (AI)

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phase 1 Complete) |
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

### Phase 1 - Core MCP Server ✅
- ✅ MCP server implementation (.NET) - `TrashMobMCP/`
- ✅ Event search tool (location, date, type) - `SearchEventsTool`
- ✅ Stats/metrics tool (sitewide) - `GetStatsTool`
- ⬜ Authentication via API tokens (deferred)

### Phase 2 - Enhanced Tools (Partial)
- ⬜ User dashboard data (authenticated)
- ✅ Team lookup - `SearchTeamsTool`
- ✅ Litter report discovery - `SearchLitterReportsTool`
- ⬜ Partner/location search

### Phase 3 - AI Features
- ⬜ Event recommendations based on user location/history
- ⬜ Impact summaries (event, community, sitewide)
- ⬜ Volunteer activity analysis (anonymized trends)
- ⬜ Community health metrics

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

1. ~~**Hosting model?**~~
   **Decision:** Separate container app for easier scaling and isolation from main API
   **Status:** ✅ Resolved

2. ~~**Authentication mechanism?**~~
   **Decision:** API tokens only for server-to-server calls; fits MCP model better than OAuth
   **Status:** ✅ Resolved

3. ~~**Which AI platforms to target?**~~
   **Decision:** All major platforms - Claude (Anthropic), ChatGPT (OpenAI), Copilot (Microsoft), and any MCP-compatible client
   **Status:** ✅ Resolved

4. ~~**Rate limits?**~~
   **Decision:** 50 calls/minute per token (conservative); higher limits available for partners
   **Status:** ✅ Resolved

5. ~~**Implementation technology?**~~
   **Decision:** .NET (consistent with main TrashMob backend; enables code/manager reuse)
   **Status:** ✅ Resolved

---

## AI Platform Integration

### How MCP Works

MCP (Model Context Protocol) is an open protocol that allows AI assistants to connect to external data sources and tools. The integration model is:

1. **TrashMob hosts the MCP server** - We build and deploy a server exposing tools (search_events, get_stats, etc.)
2. **Users configure their AI client** - Users add our server URL to their AI assistant settings
3. **AI discovers available tools** - The MCP protocol lets AI clients query what tools are available
4. **AI calls tools as needed** - When users ask questions like "Find cleanup events near me", the AI calls our tools

### Platform-Specific Integration

#### Claude (Anthropic)
- **Setup:** Users add MCP server in Claude Desktop settings or claude.ai
- **Config location:** `~/.config/claude/mcp_servers.json` (desktop) or web settings
- **Documentation:** [MCP Quickstart](https://modelcontextprotocol.io/quickstart)
- **Our action:** Publish server URL and configuration instructions

```json
// Example user configuration for Claude Desktop
{
  "mcpServers": {
    "trashmob": {
      "url": "https://mcp.trashmob.eco",
      "apiKey": "user-api-token"
    }
  }
}
```

#### ChatGPT (OpenAI)
- **Setup:** OpenAI uses "GPTs" with custom actions (similar concept, different protocol)
- **Our action:** Create a TrashMob GPT with OpenAPI spec pointing to our API
- **Alternative:** If OpenAI adds MCP support, same server works
- **Documentation:** [OpenAI Actions](https://platform.openai.com/docs/actions)

#### Copilot (Microsoft)
- **Setup:** Copilot plugins/extensions
- **Our action:** Create Copilot plugin manifest
- **Documentation:** [Copilot Extensibility](https://learn.microsoft.com/copilot-extensibility)

### What TrashMob Needs to Build

| Component | Description | Required |
|-----------|-------------|----------|
| **MCP Server** | .NET server implementing MCP protocol | Yes |
| **Public endpoint** | `https://mcp.trashmob.eco` or similar | Yes |
| **API token management** | Generate/revoke tokens for users | Yes |
| **User documentation** | How to connect from each AI platform | Yes |
| **OpenAPI spec** | For ChatGPT/non-MCP platforms | Nice-to-have |

### User Experience Flow

1. **User requests API token** from TrashMob account settings
2. **User configures AI client** with server URL + token
3. **User asks AI** natural language questions about events
4. **AI calls MCP tools** and returns formatted response
5. **User clicks links** to event pages on trashmob.eco

### Implementation Resources

| Resource | URL | Purpose |
|----------|-----|---------|
| **MCP Specification** | https://modelcontextprotocol.io/specification | Protocol details |
| **MCP .NET SDK** | https://github.com/modelcontextprotocol/csharp-sdk | .NET implementation |
| **MCP Quickstart** | https://modelcontextprotocol.io/quickstart | Getting started guide |
| **Example Servers** | https://github.com/modelcontextprotocol/servers | Reference implementations |

### Deployment Checklist

- [ ] Build MCP server with core tools (Phase 1)
- [ ] Deploy to separate container app (`mcp.trashmob.eco`)
- [ ] Implement API token generation in user settings
- [ ] Write user documentation for Claude setup
- [ ] Create OpenAPI spec for ChatGPT integration
- [ ] Test with Claude Desktop and claude.ai
- [ ] Announce availability to users

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2238](https://github.com/trashmob/TrashMob/issues/2238)** - Project 17: TrashMob.eco MCP Server (tracking issue)

---

## Related Documents

- **[MCP Specification](https://modelcontextprotocol.io)** - Protocol documentation
- **[MCP .NET SDK](https://github.com/modelcontextprotocol/csharp-sdk)** - C# implementation library
- **[MCP Servers Repository](https://github.com/modelcontextprotocol/servers)** - Example server implementations
- **[Project 6 - Backend Standards](./Project_06_Backend_Standards.md)** - API patterns
- **TrashMob API** - Existing endpoints to wrap

---

**Last Updated:** January 31, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** When AI integration becomes priority
