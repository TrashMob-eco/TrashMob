# TrashMobDailyJobs — AI Assistant Context

> For overall project context and coding standards, see [/CLAUDE.md](../CLAUDE.md) at the repository root.

## Application Overview

Scheduled background job processor that runs **once daily** as an Azure Container App Job. Collects platform-wide statistics, persists them to `SiteMetrics` for historical trending, and sends a summary report email via SendGrid.

- .NET 10 Console Application (stateless — run once and exit)
- Uses direct SQL queries for performance (bypasses EF Core for read-heavy aggregations)
- Uses `TrashMob.Shared` for email services and DI

## Project Structure

```
TrashMobDailyJobs/
├── Program.cs           # Entry point and DI configuration
├── StatGenerator.cs     # Core statistics collection and reporting logic
└── appsettings.json     # Base configuration (overridden by env vars)
```

## Execution Flow

1. Configure services and build DI container
2. Resolve `StatGenerator` from scoped service provider
3. `StatGenerator.RunAsync()`:
   - Open SQL connection
   - Execute count queries for each metric
   - Insert each metric into `SiteMetrics` table with timestamp
   - Build and send summary email via SendGrid
4. Exit

## Metrics Collected

| Metric | Description |
|--------|-------------|
| TotalSiteUsers | Registered users |
| TotalEvents | All events (excluding cancelled, status != 3) |
| TotalEventAttendees | Total signups for events |
| TotalFutureEvents | Upcoming events |
| TotalFutureEventAttendees | Signups for future events |
| TotalContactRequests | Contact form submissions |
| TotalBags | Bags + buckets/3 collected |
| TotalMinutes | Person-minutes volunteered |
| ActualAttendees | Attendees who showed up |
| TotalLitterReports | All litter reports |
| TotalNewLitterReports | New (unassigned) reports |
| TotalCleanedLitterReports | Cleaned reports |

## Adding a New Metric

1. Add a count method to `StatGenerator` following the existing pattern (SQL query → `AddSiteMetrics()`)
2. Add a field to `SiteStats` model in `TrashMob.Shared`
3. Call it in `RunAsync()` and add to the summary email

## Environment Variables

| Variable | Description |
|----------|-------------|
| `TMDBServerConnectionString` | SQL Server connection string |
| `ASPNETCORE_ENVIRONMENT` | `Development` or `Production` |
| `StorageAccountUri` | Azure Blob Storage URI |
| `VaultUri` | Azure Key Vault URI (Production) |
| `TrashMobBackendTenantId` | Azure AD Tenant ID (Development) |
| `SendGridApiKey` | SendGrid API key |
| `InstanceName` | Environment name for email subject |

## Quick Reference

```bash
dotnet build TrashMobDailyJobs         # Build
dotnet run --project TrashMobDailyJobs # Run locally
```

## Related Documentation

- [Root CLAUDE.md](../CLAUDE.md) — Architecture and coding standards
- [TrashMobHourlyJobs CLAUDE.md](../TrashMobHourlyJobs/CLAUDE.md) — Hourly job patterns
- [Planning/README.md](../Planning/README.md) — 2026 roadmap
