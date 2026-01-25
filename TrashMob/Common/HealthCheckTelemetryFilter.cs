namespace TrashMob.Common;

using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

/// <summary>
/// Filters out health check endpoint requests from Application Insights telemetry
/// to reduce costs and noise in logs.
/// </summary>
public class HealthCheckTelemetryFilter : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

    public HealthCheckTelemetryFilter(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        // Filter out health check requests
        if (item is RequestTelemetry request)
        {
            if (request.Url?.AbsolutePath != null &&
                (request.Url.AbsolutePath.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
                 request.Url.AbsolutePath.Equals("/health", StringComparison.OrdinalIgnoreCase)))
            {
                return; // Don't send this telemetry
            }
        }

        // Filter out dependency calls that are health-related (e.g., SQL health checks)
        if (item is DependencyTelemetry dependency)
        {
            if (dependency.Name != null &&
                dependency.Name.Contains("health", StringComparison.OrdinalIgnoreCase))
            {
                return; // Don't send this telemetry
            }
        }

        _next.Process(item);
    }
}
