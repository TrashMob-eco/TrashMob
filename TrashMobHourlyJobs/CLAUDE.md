# TrashMobHourlyJobs — AI Assistant Context

> For overall project context and coding standards, see [/CLAUDE.md](../CLAUDE.md) at the repository root.

## Application Overview

Scheduled background job processor that runs **every hour** as an Azure Container App Job. Sends automated email notifications to users about upcoming events, new events in their area, and event updates.

- .NET 10 Console Application (stateless — run once and exit)
- Uses `TrashMob.Shared` managers and repositories via DI
- Sends emails via SendGrid

## Project Structure

```
TrashMobHourlyJobs/
├── Program.cs               # Entry point and DI configuration
└── appsettings.json         # Base configuration (overridden by env vars)
```

## Execution Flow

1. Configure services and build DI container
2. Resolve `IUserNotificationManager` from scoped service provider
3. Call `RunAllNotifications()`:
   - Query users who opted-in for notifications
   - Find relevant events (upcoming, in their area, not yet notified)
   - Generate personalized email content
   - Send via SendGrid
   - Mark notifications as sent to avoid duplicates
4. Exit

## Environment Variables

| Variable | Description |
|----------|-------------|
| `TMDBServerConnectionString` | SQL Server connection string |
| `ASPNETCORE_ENVIRONMENT` | `Development` or `Production` |
| `StorageAccountUri` | Azure Blob Storage URI |
| `VaultUri` | Azure Key Vault URI (Production) |
| `TrashMobBackendTenantId` | Azure AD Tenant ID (Development) |
| `SendGridApiKey` | SendGrid API key |

## Quick Reference

```bash
dotnet build TrashMobHourlyJobs         # Build
dotnet run --project TrashMobHourlyJobs # Run locally
```

## Related Documentation

- [Root CLAUDE.md](../CLAUDE.md) — Architecture and coding standards
- [TrashMobDailyJobs CLAUDE.md](../TrashMobDailyJobs/CLAUDE.md) — Daily job patterns
- [Planning/README.md](../Planning/README.md) — 2026 roadmap
