# TrashMob MCP Server

An [MCP (Model Context Protocol)](https://modelcontextprotocol.io/) server that exposes TrashMob data and tools to AI assistants like Claude.

## Available Tools

### Read-Only (Public Data)
- **SearchCommunities** - Search community pages by location or slug
- **SearchEvents** - Search cleanup events by location, date, and status
- **SearchLitterReports** - Search litter reports by location and status
- **SearchPartnerLocations** - Search partner locations by service type
- **SearchTeams** - Search volunteer teams by location or name
- **GetStats** - Get platform-wide statistics
- **GetLeaderboard** - Get volunteer/team leaderboards
- **GetAchievementTypes** - Get available achievement badges
- **GetEventRouteStats** - Get statistics for event routes

### Prospect Management (Admin)
- **DiscoverProspects** - AI-powered discovery of potential community partners
- **SearchProspects** - Search existing prospects in the pipeline
- **GetGeographicGaps** - Find areas with events but no community partner
- **AddProspect** - Create a new prospect from research
- **UpdateProspect** - Update an existing prospect's details or pipeline stage

## Authentication

All requests require an API key passed via the `Authorization` header. The server validates against the `MCP_API_KEY` environment variable.

## Connecting from Claude Desktop

### 1. Get the API Key

The MCP API key is stored in Azure Key Vault. Retrieve it with:

```bash
az keyvault secret show --vault-name kv-tm-dev-westus2 --name MCP-API-KEY --query value -o tsv
```

Or ask a project admin for the key.

### 2. Configure Claude Desktop

Open Claude Desktop settings and navigate to the MCP servers configuration. On Windows, edit:

```
%APPDATA%\Claude\claude_desktop_config.json
```

On macOS, edit:

```
~/Library/Application Support/Claude/claude_desktop_config.json
```

Add the TrashMob MCP server entry:

```json
{
  "mcpServers": {
    "trashmob": {
      "type": "http",
      "url": "https://ca-mcp-tm-dev-westus2.<region>.azurecontainerapps.io/mcp",
      "headers": {
        "Authorization": "Bearer YOUR_API_KEY_HERE"
      }
    }
  }
}
```

Replace `YOUR_API_KEY_HERE` with the actual API key and `<region>` with the Azure region (e.g., `westus2`).

### 3. Connecting to a Local Server

If running the MCP server locally for development:

```bash
# Set the API key
export MCP_API_KEY="your-dev-api-key"

# Set required environment variables
export ASPNETCORE_ENVIRONMENT=Development
export StorageAccountUri="https://your-storage-account.blob.core.windows.net/"
export TrashMobBackendTenantId="your-tenant-id"

# Run the server
cd TrashMobMCP
dotnet run
```

Then configure Claude Desktop to point to localhost:

```json
{
  "mcpServers": {
    "trashmob-local": {
      "type": "http",
      "url": "http://localhost:5000/mcp",
      "headers": {
        "Authorization": "Bearer your-dev-api-key"
      }
    }
  }
}
```

### 4. Restart Claude Desktop

After saving the configuration, restart Claude Desktop. The TrashMob tools should appear in the tools list (hammer icon).

## Connecting from Claude Code

Add the server to your project's `.mcp.json` or your user-level MCP config:

```json
{
  "mcpServers": {
    "trashmob": {
      "type": "http",
      "url": "https://ca-mcp-tm-dev-westus2.<region>.azurecontainerapps.io/mcp",
      "headers": {
        "Authorization": "Bearer YOUR_API_KEY_HERE"
      }
    }
  }
}
```

## Example Prompts

Once connected, you can ask Claude things like:

- "Search for community cleanup events in Seattle"
- "Find geographic gaps where we have events but no community partner"
- "Discover potential nonprofit partners in Portland, OR that focus on waterway cleanup"
- "Add City of Bellevue as a municipality prospect with the Parks Department as the contact"
- "Show me all prospects in the Contacted stage"
- "What are the platform-wide stats for TrashMob?"
