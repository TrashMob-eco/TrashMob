#nullable enable

namespace TrashMob.Shared.Services
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Service for tracking feature usage metrics using OpenTelemetry.
    /// </summary>
    public interface IFeatureMetricsService
    {
        /// <summary>
        /// Track a feature usage event.
        /// </summary>
        /// <param name="category">Category of the feature (e.g., "Event", "User", "LitterReport")</param>
        /// <param name="action">Action performed (e.g., "Create", "Update", "Delete")</param>
        /// <param name="properties">Additional properties to attach to the event</param>
        void TrackFeatureUsage(string category, string action, Dictionary<string, object>? properties = null);

        /// <summary>
        /// Track a user journey milestone.
        /// </summary>
        /// <param name="userId">User identifier (will be hashed for privacy)</param>
        /// <param name="milestone">Milestone name (e.g., "FirstLogin", "FirstEventCreated")</param>
        /// <param name="properties">Additional properties</param>
        void TrackUserJourney(Guid userId, string milestone, Dictionary<string, object>? properties = null);

        /// <summary>
        /// Track event lifecycle metrics.
        /// </summary>
        void TrackEventAction(string action, Guid eventId, Guid? userId = null, Dictionary<string, object>? properties = null);

        /// <summary>
        /// Track attendance action metrics.
        /// </summary>
        void TrackAttendanceAction(string action, Guid eventId, Guid userId, Dictionary<string, object>? properties = null);

        /// <summary>
        /// Track litter report metrics.
        /// </summary>
        void TrackLitterReportAction(string action, Guid litterReportId, Guid? userId = null, Dictionary<string, object>? properties = null);

        /// <summary>
        /// Track team-related metrics.
        /// </summary>
        void TrackTeamAction(string action, Guid teamId, Guid? userId = null, Dictionary<string, object>? properties = null);
    }
}
