# TrashMobHourlyJobs — AI Assistant Context

> **Note:** This document provides context specific to the TrashMobHourlyJobs background processing application. For overall project context, see `/claude.md` at the repository root.

## Application Overview

TrashMobHourlyJobs is a scheduled background job processor that runs hourly to perform automated tasks such as:
- **User notifications** — Send email notifications about upcoming events
- **Event reminders** — Alert attendees about events happening soon
- **Data cleanup** — Remove expired or stale data
- **Status updates** — Update event statuses based on time

**Architecture:**
- .NET 10 Console Application
- Designed to run as an Azure WebJob or scheduled task
- Stateless execution (run once and exit)
- Uses shared TrashMob.Shared library for business logic
- Configured entirely through environment variables

## Project Structure

```
TrashMobHourlyJobs/
??? Program.cs                # Entry point and DI configuration
??? appsettings.json          # Base configuration (overridden by env vars)
??? TrashMobHourlyJobs.csproj
```

## Execution Flow

```
1. Start ? Configure Services ? Build Service Provider
2. Create Scope ? Resolve Dependencies
3. Execute Job Logic (UserNotificationManager.RunAllNotifications)
4. Log Completion ? Exit
```

## Key Patterns

### Console Application Entry Point

```csharp
public static async Task Main(string[] args)
{
    var services = new ServiceCollection();
    ConfigureServices(services);

    using var serviceProvider = services.BuildServiceProvider();
    using var scope = serviceProvider.CreateScope();

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var userNotificationManager = scope.ServiceProvider.GetRequiredService<IUserNotificationManager>();

    logger.LogInformation("UserNotifier job started at: {Time}", DateTime.UtcNow);

    await userNotificationManager.RunAllNotifications();

    logger.LogInformation("UserNotifier job completed at: {Time}", DateTime.UtcNow);
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

    // 3. Register business logic services
    ServiceBuilder.AddManagers(services);
    ServiceBuilder.AddRepositories(services);
    services.AddScoped<IUserNotificationManager, UserNotificationManager>();

    // 4. Database context
    services.AddDbContext<MobDbContext>();

    // 5. Azure services (Blob Storage, Key Vault)
    ConfigureAzureServices(services);
}
```

### Azure Services Configuration

```csharp
// Development: Use Visual Studio credentials
if (environment == "Development")
{
    services.AddAzureClients(builder =>
    {
        builder.UseCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            VisualStudioTenantId = tenantId
        }));
        builder.AddBlobServiceClient(blobStorageUrl);
    });
}
// Production: Use managed identity
else
{
    services.AddAzureClients(builder =>
    {
        builder.UseCredential(new DefaultAzureCredential());
        builder.AddSecretClient(vaultUri);
        builder.AddBlobServiceClient(blobStorageUrl);
    });
}
```

## Environment Variables (Required)

| Variable | Description | Example |
|----------|-------------|---------|
| `TMDBServerConnectionString` | SQL Server connection string | `Server=...;Database=TrashMob;...` |
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Development`, `Production` |
| `StorageAccountUri` | Azure Blob Storage URI | `https://mystorageaccount.blob.core.windows.net` |
| `VaultUri` | Azure Key Vault URI (Production only) | `https://mykeyvault.vault.azure.net/` |
| `TrashMobBackendTenantId` | Azure AD Tenant ID (Development only) | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `SendGridApiKey` | SendGrid API key for emails | Retrieved from Key Vault or env var |

## Job Implementation Guidelines

### Creating a New Hourly Job

1. **Create job class in TrashMob.Shared.Engine:**

```csharp
public class MyNewJobProcessor
{
    private readonly ILogger<MyNewJobProcessor> _logger;
    private readonly IMyDataManager _dataManager;

    public MyNewJobProcessor(
        ILogger<MyNewJobProcessor> logger,
        IMyDataManager dataManager)
    {
        _logger = logger;
        _dataManager = dataManager;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("MyNewJob started");

        try
        {
            // Job logic here
            await ProcessDataAsync();

            _logger.LogInformation("MyNewJob completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MyNewJob failed");
            throw; // Re-throw to ensure job is marked as failed
        }
    }

    private async Task ProcessDataAsync()
    {
        // Implementation
    }
}
```

2. **Register in Program.cs:**

```csharp
services.AddScoped<MyNewJobProcessor>();
```

3. **Invoke in Main:**

```csharp
var myJob = scope.ServiceProvider.GetRequiredService<MyNewJobProcessor>();
await myJob.RunAsync();
```

### Job Best Practices

1. **Idempotent Operations:**
   - Jobs may run multiple times if there's a failure
   - Ensure operations can safely retry without side effects

2. **Comprehensive Logging:**
   ```csharp
   _logger.LogInformation("Processing {Count} records", records.Count);
   _logger.LogWarning("Skipping record {RecordId} - invalid data", record.Id);
   _logger.LogError(ex, "Failed to process record {RecordId}", record.Id);
   ```

3. **Transaction Management:**
   ```csharp
   using var transaction = await _context.Database.BeginTransactionAsync();
   try
   {
       await ProcessBatchAsync();
       await transaction.CommitAsync();
   }
   catch
   {
       await transaction.RollbackAsync();
       throw;
   }
   ```

4. **Batch Processing:**
   ```csharp
   const int batchSize = 100;
   var totalProcessed = 0;

   while (true)
   {
       var batch = await GetNextBatchAsync(batchSize);
       if (!batch.Any()) break;

       await ProcessBatchAsync(batch);
       totalProcessed += batch.Count;

       _logger.LogInformation("Processed {Total} records so far", totalProcessed);
   }
   ```

5. **Graceful Error Handling:**
   ```csharp
   var errors = new List<string>();

   foreach (var item in items)
   {
       try
       {
           await ProcessItemAsync(item);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Failed to process item {ItemId}", item.Id);
           errors.Add($"Item {item.Id}: {ex.Message}");
       }
   }

   if (errors.Any())
   {
       _logger.LogWarning("Job completed with {ErrorCount} errors", errors.Count);
   }
   ```

6. **Timeout Protection:**
   ```csharp
   using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(50));
   await ProcessWithTimeoutAsync(cts.Token);
   ```

## Current Hourly Jobs

### UserNotificationManager.RunAllNotifications()

Sends email notifications to users about:
- **Upcoming events** they're registered for (24-hour reminder)
- **New events** in their area
- **Event updates** or cancellations

**Key Logic:**
1. Query users who opted-in for notifications
2. Find relevant events (upcoming, in their area, not notified yet)
3. Generate personalized email content
4. Send via SendGrid
5. Mark notifications as sent to avoid duplicates

## Testing Locally

### Prerequisites
- Azure Storage Emulator or Azurite
- SQL Server (LocalDB or full instance)
- SendGrid API key (for email testing)

### Running the Job

```bash
# Set environment variables
$env:TMDBServerConnectionString="Server=...;Database=TrashMob;..."
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:StorageAccountUri="http://127.0.0.1:10000/devstoreaccount1"
$env:TrashMobBackendTenantId="your-tenant-id"
$env:SendGridApiKey="your-sendgrid-key"

# Run the job
dotnet run --project TrashMobHourlyJobs
```

### Debugging Tips

1. **Reduce scope for testing:**
   ```csharp
   // Temporarily limit to single user
   var users = await _context.Users
       .Where(u => u.Email == "test@example.com")
       .ToListAsync();
   ```

2. **Add verbose logging:**
   ```csharp
   builder.SetMinimumLevel(LogLevel.Debug);
   ```

3. **Use a test email:**
   ```csharp
   // Override recipient in development
   if (environment == "Development")
   {
       emailRequest.To = "test@example.com";
   }
   ```

## Deployment

### Azure WebJob Configuration

**Job Type:** Triggered (scheduled)  
**Schedule:** Every hour: `0 0 * * * *` (CRON expression)  
**Always On:** Required for consistent execution  
**Platform:** Windows or Linux (64-bit)

### App Settings (Azure Portal)

Set these in the WebJob's Application Settings:
- `TMDBServerConnectionString`
- `ASPNETCORE_ENVIRONMENT=Production`
- `StorageAccountUri`
- `VaultUri`
- SendGrid key retrieved from Key Vault

### Monitoring

**Key Metrics:**
- Execution time (should complete in < 10 minutes)
- Success/failure rate
- Email delivery rate
- Error logs in Application Insights

**Alerts:**
- Job fails 2+ times consecutively
- Job takes > 30 minutes
- Email send rate drops below threshold

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **Job times out** | Reduce batch sizes, add pagination |
| **Database connection errors** | Check connection string, verify network access |
| **SendGrid rate limits** | Implement batching with delays |
| **Memory issues** | Use `.AsNoTracking()` for read-only queries |
| **Duplicate notifications** | Check notification tracking logic |
| **Missing environment variables** | Verify all required vars are set |

## Performance Considerations

1. **Database Queries:**
   - Use indexes on date fields and foreign keys
   - Filter early in queries (WHERE clauses)
   - Use `.AsNoTracking()` for read-only operations
   - Batch updates instead of individual saves

2. **Email Sending:**
   - Batch emails (SendGrid supports up to 1,000 per request)
   - Use async/await properly
   - Implement retry logic for transient failures

3. **Memory Usage:**
   - Stream large result sets
   - Dispose resources properly (use `using` statements)
   - Avoid loading entire tables into memory

## Common Patterns

### Safe Async Operations

```csharp
// ? GOOD: Proper async pattern
public async Task ProcessAsync()
{
    var data = await _repository.GetDataAsync();
    await _processor.ProcessAsync(data);
}

// ? BAD: Blocking async call
public void Process()
{
    var data = _repository.GetDataAsync().Result; // Deadlock risk
}
```

### Resource Cleanup

```csharp
// ? GOOD: Proper disposal
using var scope = serviceProvider.CreateScope();
using var transaction = await _context.Database.BeginTransactionAsync();
// Resources automatically disposed

// ? BAD: Missing disposal
var scope = serviceProvider.CreateScope();
// Scope never disposed - memory leak
```

### Error Logging

```csharp
// ? GOOD: Structured logging with context
_logger.LogError(ex, 
    "Failed to send notification to user {UserId} for event {EventId}", 
    userId, eventId);

// ? BAD: Generic error message
_logger.LogError("An error occurred");
```

## Quick Reference

### Useful Commands

```bash
# Build
dotnet build TrashMobHourlyJobs

# Run locally
dotnet run --project TrashMobHourlyJobs

# Publish for deployment
dotnet publish TrashMobHourlyJobs -c Release -o ./publish

# Test with specific environment
$env:ASPNETCORE_ENVIRONMENT="Staging"
dotnet run --project TrashMobHourlyJobs
```

### Related Projects

- **TrashMobDailyJobs** — Daily scheduled tasks (stats generation, cleanup)
- **TrashMob.Shared** — Shared business logic and data access
- **TrashMob** — Main web API and frontend

---

**Related Documentation:**
- Root `/claude.md` — Overall project context
- `/TrashMob/claude.md` — Web API patterns
- `/TrashMob_2026_Product_Engineering_Plan.md` — 2026 roadmap

**Last Updated:** January 23, 2026
