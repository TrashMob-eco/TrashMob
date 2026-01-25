# TrashMobDailyJobs � AI Assistant Context

> **Note:** This document provides context specific to the TrashMobDailyJobs background processing application. For overall project context, see `/claude.md` at the repository root.

## Application Overview

TrashMobDailyJobs is a scheduled background job processor that runs daily to perform:
- **Statistics generation** � Calculate and record platform-wide metrics
- **Daily summary reports** � Email comprehensive stats to administrators
- **Metrics persistence** � Store time-series data in SiteMetrics table for trending

**Architecture:**
- .NET 10 Console Application
- Designed to run as an Azure WebJob or scheduled task
- Stateless execution (run once per day and exit)
- Direct SQL queries for performance (bypasses EF Core for read-heavy operations)
- Uses TrashMob.Shared library for email services
- Configured entirely through environment variables

## Project Structure

```
TrashMobDailyJobs/
??? Program.cs                # Entry point and DI configuration
??? StatGenerator.cs          # Core statistics collection and reporting logic
??? appsettings.json          # Base configuration (overridden by env vars)
??? TrashMobDailyJobs.csproj
```

## Execution Flow

```
1. Start ? Configure Services ? Build Service Provider
2. Create Scope ? Resolve StatGenerator
3. StatGenerator.RunAsync():
   a. Connect to database
   b. Execute count queries for each metric
   c. Insert metrics into SiteMetrics table
   d. Build summary email
   e. Send via SendGrid
4. Log Completion ? Exit
```

## Key Components

### StatGenerator Class

The main job processor that:
1. Collects 12 different platform metrics
2. Records each metric in the `SiteMetrics` table with timestamp
3. Formats and sends a daily summary email to administrators

**Metrics Collected:**

| Metric | Description | SQL Source |
|--------|-------------|------------|
| **TotalSiteUsers** | Total registered users | `Users` table |
| **TotalEvents** | All events (excluding cancelled) | `Events` where `eventstatusid != 3` |
| **TotalEventAttendees** | Total signups for events | `EventAttendees` joined with `Events` |
| **TotalFutureEvents** | Upcoming events | `Events` where `EventDate > GetDate()` |
| **TotalFutureEventAttendees** | Signups for future events | Future events joined with attendees |
| **TotalContactRequests** | Contact form submissions | `ContactRequests` table |
| **TotalBags** | Bags + buckets/3 collected | `EventSummaries` aggregation |
| **TotalMinutes** | Person-minutes volunteered | `DurationInMinutes * ActualNumberOfAttendees` |
| **ActualAttendees** | Attendees who showed up | `EventSummaries.ActualNumberOfAttendees` |
| **TotalLitterReports** | All litter reports | `LitterReports` table |
| **TotalNewLitterReports** | New (unassigned) reports | `LitterReports` with status = New |
| **TotalCleanedLitterReports** | Cleaned reports | `LitterReports` with status = Cleaned |

## Key Patterns

### Console Application Entry Point

```csharp
public static async Task Main(string[] args)
{
    var services = new ServiceCollection();
    ConfigureServices(services);

    using var serviceProvider = services.BuildServiceProvider();
    using var scope = serviceProvider.CreateScope();

    var statGenerator = scope.ServiceProvider.GetRequiredService<StatGenerator>();
    await statGenerator.RunAsync();
}
```

### Dependency Injection Configuration

```csharp
private static void ConfigureServices(IServiceCollection services)
{
    // 1. Configuration from environment variables
    var configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    // 2. Logging
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Information);
    });

    // 3. Register job processor
    services.AddScoped<StatGenerator>();

    // 4. Register shared services (managers, repositories)
    ServiceBuilder.AddManagers(services);
    ServiceBuilder.AddRepositories(services);

    // 5. Database context
    services.AddDbContext<MobDbContext>();

    // 6. Azure services (Blob Storage, Key Vault)
    ConfigureAzureServices(services);
}
```

### Statistics Collection Pattern

```csharp
private async Task<int> CountUsers(SqlConnection conn)
{
    var sql = "SELECT count(*) FROM dbo.Users";
    var numberOfUsers = 0;

    using (var cmd = new SqlCommand(sql, conn))
    {
        var result = await cmd.ExecuteScalarAsync();
        numberOfUsers = result is DBNull or null ? 0 : Convert.ToInt32(result);
        logger.LogInformation("There are currently '{NumberOfUsers}' Users.", numberOfUsers);
    }

    // Persist metric for historical tracking
    await AddSiteMetrics(conn, "TotalSiteUsers", numberOfUsers);
    return numberOfUsers;
}
```

### Metrics Persistence

```csharp
private async Task AddSiteMetrics(SqlConnection conn, string metricType, long metricValue)
{
    var id = Guid.NewGuid();
    var processedTime = DateTimeOffset.Now;
    var sql = "INSERT INTO dbo.SiteMetrics (id, processedtime, metricType, metricValue) " +
              "VALUES (@id, @processedTime, @metricType, @metricValue)";
    
    using var command = new SqlCommand(sql, conn);
    command.Parameters.AddWithValue("@id", id);
    command.Parameters.AddWithValue("@processedTime", processedTime);
    command.Parameters.AddWithValue("@metricType", metricType);
    command.Parameters.AddWithValue("@metricValue", metricValue);

    var result = await command.ExecuteNonQueryAsync();

    if (result < 0)
    {
        logger.LogError("Error inserting data into Database!");
    }
}
```

## Environment Variables (Required)

| Variable | Description | Example |
|----------|-------------|---------|
| `TMDBServerConnectionString` | SQL Server connection string | `Server=...;Database=TrashMob;...` |
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Development`, `Production` |
| `StorageAccountUri` | Azure Blob Storage URI | `https://mystorageaccount.blob.core.windows.net` |
| `VaultUri` | Azure Key Vault URI (Production) | `https://mykeyvault.vault.azure.net/` |
| `TrashMobBackendTenantId` | Azure AD Tenant ID (Development) | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `SendGridApiKey` | SendGrid API key for emails | Retrieved from Key Vault or env var |
| `InstanceName` | Name of the environment for email subject | `Production`, `Staging`, `Development` |

## Job Implementation Guidelines

### Adding a New Daily Metric

1. **Add count method to StatGenerator:**

```csharp
private async Task<int> CountMyNewMetric(SqlConnection conn)
{
    var sql = "SELECT count(*) FROM dbo.MyTable WHERE SomeCondition = true";
    var count = 0;

    using (var cmd = new SqlCommand(sql, conn))
    {
        var result = await cmd.ExecuteScalarAsync();
        count = result is DBNull or null ? 0 : Convert.ToInt32(result);
        logger.LogInformation("There are currently '{Count}' MyNewMetric records.", count);
    }

    await AddSiteMetrics(conn, "TotalMyNewMetric", count);
    return count;
}
```

2. **Add field to SiteStats model (TrashMob.Shared):**

```csharp
public class SiteStats
{
    // ... existing properties
    public int MyNewMetricCount { get; set; }
}
```

3. **Call in RunAsync:**

```csharp
using (var conn = new SqlConnection(connectionString))
{
    await conn.OpenAsync();
    
    // ... existing metrics
    siteStats.MyNewMetricCount = await CountMyNewMetric(conn);
}
```

4. **Add to summary email:**

```csharp
private static Task SendSummaryReport(SiteStats siteStats, string instanceName, string sendGridApiKey)
{
    var sb = new StringBuilder();
    // ... existing metrics
    sb.AppendLine($"Total Number of MyNewMetric: {siteStats.MyNewMetricCount}\n");
    // ... rest of email
}
```

### Best Practices for Daily Jobs

1. **Use Direct SQL for Performance:**
   - Daily jobs query large datasets
   - Raw SQL is faster than EF Core for read-only aggregations
   - Always use parameterized queries to prevent SQL injection

2. **Handle Null Results:**
   ```csharp
   var count = result is DBNull or null ? 0 : Convert.ToInt32(result);
   ```

3. **Log All Metrics:**
   ```csharp
   logger.LogInformation("Metric '{MetricName}' calculated: {Value}", metricName, value);
   ```

4. **Persist for Time-Series:**
   - Every metric is inserted into `SiteMetrics` table
   - Enables historical trending and charts
   - Includes timestamp for accurate point-in-time snapshots

5. **Graceful Error Handling:**
   ```csharp
   if (sendGridApiKey == null)
   {
       logger.LogError("SendGrid API Key is not configured. Cannot send summary report email.");
       return; // Exit gracefully, don't crash
   }
   ```

6. **Connection Management:**
   ```csharp
   using (var conn = new SqlConnection(connectionString))
   {
       await conn.OpenAsync();
       // All queries use same connection
   } // Automatically disposed and closed
   ```

## Database Schema

### SiteMetrics Table

```sql
CREATE TABLE dbo.SiteMetrics
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProcessedTime DATETIMEOFFSET NOT NULL,
    MetricType NVARCHAR(100) NOT NULL,
    MetricValue BIGINT NOT NULL,
    INDEX IX_SiteMetrics_ProcessedTime (ProcessedTime),
    INDEX IX_SiteMetrics_MetricType (MetricType)
);
```

**Purpose:** Time-series storage for daily metrics to enable:
- Historical trend analysis
- Dashboard charts
- Performance monitoring
- Growth tracking

## Email Report Format

The daily summary email includes:

```
Subject: Summary Report for '{InstanceName}'

Summary Report for 'Production'

Total Number of Users: 5,234
Total Number of Events: 1,892
Total Number of Attendees: 12,456
Total Number of Future Events: 47
Total Number of Future Event Attendees: 389
Total Number of Contact Requests: 156
Total Number of Bags Collected: 8,921
Total Number of Minutes: 456,789
Total Number of Actual Attendees: 9,234
Total Number of Litter Reports: 234
Total Number of New Litter Reports: 12
Total Number of Cleaned Litter Reports: 198

End Report.
```

**Sent to:** Admin email addresses configured in SendGrid

## Testing Locally

### Prerequisites
- SQL Server with TrashMob database
- SendGrid API key (for email testing)
- Azure Storage Emulator or Azurite (for Azure services)

### Running the Job

```bash
# Set environment variables
$env:TMDBServerConnectionString="Server=(localdb)\MSSQLLocalDB;Database=TrashMob;Integrated Security=true;"
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:StorageAccountUri="http://127.0.0.1:10000/devstoreaccount1"
$env:TrashMobBackendTenantId="your-tenant-id"
$env:SendGridApiKey="your-sendgrid-key"
$env:InstanceName="Local Development"

# Run the job
dotnet run --project TrashMobDailyJobs
```

### Debugging Tips

1. **Test with smaller dataset:**
   ```csharp
   // Temporarily add WHERE clause
   var sql = "SELECT count(*) FROM dbo.Users WHERE CreatedDate >= DATEADD(day, -7, GETDATE())";
   ```

2. **Skip email sending during development:**
   ```csharp
   if (environment == "Development")
   {
       logger.LogInformation("Skipping email send in development");
       logger.LogInformation(sb.ToString()); // Log the report instead
       return Task.CompletedTask;
   }
   ```

3. **Verify SQL queries:**
   ```bash
   # Run query directly in SQL Server Management Studio
   SELECT count(*) FROM dbo.Users;
   ```

4. **Check SiteMetrics table:**
   ```sql
   SELECT TOP 100 * FROM dbo.SiteMetrics
   ORDER BY ProcessedTime DESC;
   ```

## Performance Considerations

### Query Optimization

1. **Use Indexes:**
   - Ensure `EventDate` is indexed on `Events` table
   - Index foreign keys (`EventId`, `UserId`, etc.)
   - Index status columns (`eventstatusid`, `LitterReportStatusId`)

2. **Avoid N+1 Queries:**
   - Use JOINs instead of multiple queries
   - All metrics in this job use single aggregate queries

3. **Connection Reuse:**
   ```csharp
   using (var conn = new SqlConnection(connectionString))
   {
       await conn.OpenAsync();
       // Reuse same connection for all queries
       await CountUsers(conn);
       await CountEvents(conn);
       // ... all other metrics
   }
   ```

4. **Execution Time:**
   - Target: Complete in < 5 minutes
   - Monitor via logging: "StatGenerator job started/completed"
   - If exceeding 10 minutes, investigate slow queries

## Deployment

### Azure WebJob Configuration

**Job Type:** Triggered (scheduled)  
**Schedule:** Daily at 2 AM UTC: `0 0 2 * * *` (CRON expression)  
**Always On:** Required for reliable execution  
**Platform:** Windows or Linux (64-bit)

### App Settings (Azure Portal)

Set these in the WebJob's Application Settings:
- `TMDBServerConnectionString`
- `ASPNETCORE_ENVIRONMENT=Production`
- `StorageAccountUri`
- `VaultUri`
- `InstanceName=Production`
- SendGrid key retrieved from Key Vault

### Monitoring

**Key Metrics:**
- Execution time (should complete in < 5 minutes)
- Success/failure rate (should be 100%)
- Email delivery confirmation
- Trend analysis of collected metrics

**Alerts:**
- Job fails 2+ days consecutively
- Job takes > 15 minutes
- Email send failure
- Metric values drop significantly (possible data issue)

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **Query timeout** | Optimize slow queries; add indexes |
| **Email not sent** | Verify SendGrid API key; check spam folder |
| **Wrong metric values** | Verify SQL query logic; check event status exclusions |
| **Database connection error** | Check connection string; verify network access |
| **Missing metrics in table** | Check `AddSiteMetrics` error logs; verify table exists |
| **Job runs multiple times** | Verify CRON schedule; check Azure WebJob logs |

## Common Patterns

### Safe Null Handling

```csharp
// ? GOOD: Handle DBNull and null
var count = result is DBNull or null ? 0 : Convert.ToInt32(result);

// ? BAD: Direct conversion can throw exception
var count = Convert.ToInt32(result); // Throws if result is DBNull
```

### Proper SQL Parameter Usage

```csharp
// ? GOOD: Parameterized query
var sql = "SELECT count(*) FROM dbo.Events WHERE eventstatusid != @statusId";
command.Parameters.AddWithValue("@statusId", 3);

// ? BAD: String concatenation (SQL injection risk)
var sql = $"SELECT count(*) FROM dbo.Events WHERE eventstatusid != {statusId}";
```

### Resource Cleanup

```csharp
// ? GOOD: Using statement ensures disposal
using (var conn = new SqlConnection(connectionString))
{
    // Connection automatically closed and disposed
}

// ? BAD: Manual disposal (easy to forget)
var conn = new SqlConnection(connectionString);
conn.Open();
// ... if exception occurs, connection leaks
conn.Close();
```

## Extending Functionality

### Adding a New Daily Job

If you need to add another daily task (not just statistics):

1. **Create new job class:**

```csharp
public class DataCleanupJob
{
    private readonly ILogger<DataCleanupJob> _logger;
    private readonly MobDbContext _context;

    public DataCleanupJob(ILogger<DataCleanupJob> logger, MobDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("DataCleanupJob started");
        
        // Delete expired data
        var expiredDate = DateTime.UtcNow.AddDays(-90);
        var deleted = await _context.OldRecords
            .Where(r => r.CreatedDate < expiredDate)
            .ExecuteDeleteAsync();
        
        _logger.LogInformation("Deleted {Count} expired records", deleted);
    }
}
```

2. **Register in Program.cs:**

```csharp
services.AddScoped<DataCleanupJob>();
```

3. **Call in Main:**

```csharp
var statGenerator = scope.ServiceProvider.GetRequiredService<StatGenerator>();
await statGenerator.RunAsync();

var cleanupJob = scope.ServiceProvider.GetRequiredService<DataCleanupJob>();
await cleanupJob.RunAsync();
```

## Quick Reference

### Useful Commands

```bash
# Build
dotnet build TrashMobDailyJobs

# Run locally
dotnet run --project TrashMobDailyJobs

# Publish for deployment
dotnet publish TrashMobDailyJobs -c Release -o ./publish

# Test specific metric query
sqlcmd -S localhost -d TrashMob -Q "SELECT count(*) FROM dbo.Users"
```

### SQL Queries for Verification

```sql
-- View recent metrics
SELECT TOP 100 * FROM dbo.SiteMetrics
ORDER BY ProcessedTime DESC;

-- Check metric trends
SELECT 
    CAST(ProcessedTime AS DATE) AS Date,
    MetricType,
    MetricValue
FROM dbo.SiteMetrics
WHERE MetricType = 'TotalSiteUsers'
ORDER BY ProcessedTime DESC;

-- Verify event counts manually
SELECT count(*) FROM dbo.Events WHERE eventstatusid != 3;
```

### Related Projects

- **TrashMobHourlyJobs** � Hourly tasks (notifications, reminders)
- **TrashMob.Shared** � Shared business logic and data access
- **TrashMob** � Main web API and frontend

---

**Related Documentation:**
- Root `/claude.md` � Overall project context
- `/TrashMob/claude.md` � Web API patterns
- `/TrashMobHourlyJobs/claude.md` � Hourly job patterns
- `/Planning/README.md` � 2026 roadmap

**Last Updated:** January 23, 2026
