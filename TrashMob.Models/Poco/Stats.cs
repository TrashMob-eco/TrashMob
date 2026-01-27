namespace TrashMob.Models.Poco;

/// <summary>
/// Represents aggregate statistics for the TrashMob platform.
/// </summary>
public class Stats
{
    /// <summary>
    /// Gets or sets the total number of bags of litter collected.
    /// </summary>
    public int TotalBags { get; set; }

    /// <summary>
    /// Gets or sets the total number of volunteer hours contributed.
    /// </summary>
    public int TotalHours { get; set; }

    /// <summary>
    /// Gets or sets the total number of cleanup events held.
    /// </summary>
    public int TotalEvents { get; set; }

    /// <summary>
    /// Gets or sets the total weight of litter collected in pounds.
    /// </summary>
    public int TotalWeightInPounds { get; set; }

    /// <summary>
    /// Gets or sets the total weight of litter collected in kilograms.
    /// </summary>
    public int TotalWeightInKilograms { get; set; }

    /// <summary>
    /// Gets or sets the total number of participants across all events.
    /// </summary>
    public int TotalParticipants { get; set; }

    /// <summary>
    /// Gets or sets the total number of litter reports submitted.
    /// </summary>
    public int TotalLitterReportsSubmitted { get; set; }

    /// <summary>
    /// Gets or sets the total number of litter reports that have been closed.
    /// </summary>
    public int TotalLitterReportsClosed { get; set; }
}