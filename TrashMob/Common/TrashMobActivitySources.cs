namespace TrashMob.Common;

using System.Diagnostics;

/// <summary>
/// Defines OpenTelemetry activity sources for tracing business operations in TrashMob.
/// </summary>
public static class TrashMobActivitySources
{
    /// <summary>
    /// Activity source for event-related operations (create, update, delete events).
    /// </summary>
    public static readonly ActivitySource Events = new("TrashMob.Events", "1.0.0");

    /// <summary>
    /// Activity source for user-related operations (registration, profile updates).
    /// </summary>
    public static readonly ActivitySource Users = new("TrashMob.Users", "1.0.0");

    /// <summary>
    /// Activity source for notification-related operations (email, push notifications).
    /// </summary>
    public static readonly ActivitySource Notifications = new("TrashMob.Notifications", "1.0.0");

    /// <summary>
    /// Activity source for litter report operations.
    /// </summary>
    public static readonly ActivitySource LitterReports = new("TrashMob.LitterReports", "1.0.0");

    /// <summary>
    /// Activity source for partner-related operations.
    /// </summary>
    public static readonly ActivitySource Partners = new("TrashMob.Partners", "1.0.0");

    /// <summary>
    /// Activity source for feature usage metrics.
    /// </summary>
    public static readonly ActivitySource FeatureMetrics = new("TrashMob.FeatureMetrics", "1.0.0");

    /// <summary>
    /// All activity source names for registration with OpenTelemetry.
    /// </summary>
    public static readonly string[] AllSourceNames =
    [
        "TrashMob.Events",
        "TrashMob.Users",
        "TrashMob.Notifications",
        "TrashMob.LitterReports",
        "TrashMob.Partners",
        "TrashMob.FeatureMetrics"
    ];
}
