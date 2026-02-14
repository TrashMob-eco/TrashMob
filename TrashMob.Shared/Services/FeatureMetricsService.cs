#nullable enable

namespace TrashMob.Shared.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service for tracking feature usage metrics using OpenTelemetry Activities.
    /// </summary>
    public class FeatureMetricsService(ILogger<FeatureMetricsService> logger) : IFeatureMetricsService
    {
        private static readonly ActivitySource FeatureMetricsSource = new("TrashMob.FeatureMetrics", "1.0.0");

        /// <inheritdoc />
        public void TrackFeatureUsage(string category, string action, Dictionary<string, object>? properties = null)
        {
            var eventName = $"Feature_{category}_{action}";

            using var activity = FeatureMetricsSource.StartActivity(eventName, ActivityKind.Internal);

            if (activity != null)
            {
                activity.SetTag("feature.category", category);
                activity.SetTag("feature.action", action);
                activity.SetTag("feature.timestamp", DateTimeOffset.UtcNow.ToString("O"));

                if (properties != null)
                {
                    foreach (var (key, value) in properties)
                    {
                        activity.SetTag($"feature.{key}", value?.ToString() ?? string.Empty);
                    }
                }

                // Log the event for visibility
                logger.LogInformation("Feature metric tracked: {EventName}", eventName);
            }
        }

        /// <inheritdoc />
        public void TrackUserJourney(Guid userId, string milestone, Dictionary<string, object>? properties = null)
        {
            var eventName = $"UserJourney_{milestone}";

            using var activity = FeatureMetricsSource.StartActivity(eventName, ActivityKind.Internal);

            if (activity != null)
            {
                activity.SetTag("journey.milestone", milestone);
                activity.SetTag("journey.user_hash", HashUserId(userId));
                activity.SetTag("journey.timestamp", DateTimeOffset.UtcNow.ToString("O"));

                if (properties != null)
                {
                    foreach (var (key, value) in properties)
                    {
                        activity.SetTag($"journey.{key}", value?.ToString() ?? string.Empty);
                    }
                }

                logger.LogInformation("User journey milestone tracked: {Milestone}", milestone);
            }
        }

        /// <inheritdoc />
        public void TrackEventAction(string action, Guid eventId, Guid? userId = null, Dictionary<string, object>? properties = null)
        {
            var combinedProperties = new Dictionary<string, object>
            {
                ["eventId"] = eventId.ToString()
            };

            if (userId.HasValue)
            {
                combinedProperties["userIdHash"] = HashUserId(userId.Value);
            }

            if (properties != null)
            {
                foreach (var (key, value) in properties)
                {
                    combinedProperties[key] = value;
                }
            }

            TrackFeatureUsage("Event", action, combinedProperties);
        }

        /// <inheritdoc />
        public void TrackAttendanceAction(string action, Guid eventId, Guid userId, Dictionary<string, object>? properties = null)
        {
            var combinedProperties = new Dictionary<string, object>
            {
                ["eventId"] = eventId.ToString(),
                ["userIdHash"] = HashUserId(userId)
            };

            if (properties != null)
            {
                foreach (var (key, value) in properties)
                {
                    combinedProperties[key] = value;
                }
            }

            TrackFeatureUsage("Attendance", action, combinedProperties);
        }

        /// <inheritdoc />
        public void TrackLitterReportAction(string action, Guid litterReportId, Guid? userId = null, Dictionary<string, object>? properties = null)
        {
            var combinedProperties = new Dictionary<string, object>
            {
                ["litterReportId"] = litterReportId.ToString()
            };

            if (userId.HasValue)
            {
                combinedProperties["userIdHash"] = HashUserId(userId.Value);
            }

            if (properties != null)
            {
                foreach (var (key, value) in properties)
                {
                    combinedProperties[key] = value;
                }
            }

            TrackFeatureUsage("LitterReport", action, combinedProperties);
        }

        /// <inheritdoc />
        public void TrackTeamAction(string action, Guid teamId, Guid? userId = null, Dictionary<string, object>? properties = null)
        {
            var combinedProperties = new Dictionary<string, object>
            {
                ["teamId"] = teamId.ToString()
            };

            if (userId.HasValue)
            {
                combinedProperties["userIdHash"] = HashUserId(userId.Value);
            }

            if (properties != null)
            {
                foreach (var (key, value) in properties)
                {
                    combinedProperties[key] = value;
                }
            }

            TrackFeatureUsage("Team", action, combinedProperties);
        }

        /// <summary>
        /// Hash a user ID for privacy-preserving analytics.
        /// Returns a truncated hash that cannot be reversed to identify the user.
        /// </summary>
        private static string HashUserId(Guid userId)
        {
            var bytes = Encoding.UTF8.GetBytes(userId.ToString());
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash)[..8]; // First 8 chars only
        }
    }
}
