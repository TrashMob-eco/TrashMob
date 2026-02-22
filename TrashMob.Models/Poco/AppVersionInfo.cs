namespace TrashMob.Models.Poco;

/// <summary>
/// Represents app version requirements returned by the server.
/// </summary>
public class AppVersionInfo
{
    /// <summary>
    /// Gets or sets the minimum required app version.
    /// Users below this version are blocked from using the app.
    /// </summary>
    public string MinimumVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recommended app version.
    /// Users below this version see a dismissible update prompt.
    /// </summary>
    public string RecommendedVersion { get; set; } = string.Empty;
}
