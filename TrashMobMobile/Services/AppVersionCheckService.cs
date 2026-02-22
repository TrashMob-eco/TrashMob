namespace TrashMobMobile.Services;

public class AppVersionCheckService(IAppVersionRestService appVersionRestService) : IAppVersionCheckService
{
    private bool hasChecked;

    public async Task<VersionCheckResult> CheckVersionAsync(CancellationToken cancellationToken = default)
    {
        if (hasChecked)
        {
            return VersionCheckResult.Skipped;
        }

        hasChecked = true;

        var versionInfo = await appVersionRestService.GetAppVersionAsync(cancellationToken);

        if (versionInfo is null)
        {
            return VersionCheckResult.Skipped;
        }

        var currentVersion = ParseVersion(AppInfo.Current.VersionString);
        var minimumVersion = ParseVersion(versionInfo.MinimumVersion);
        var recommendedVersion = ParseVersion(versionInfo.RecommendedVersion);

        if (currentVersion < minimumVersion)
        {
            return VersionCheckResult.HardBlock;
        }

        if (currentVersion < recommendedVersion)
        {
            return VersionCheckResult.SoftNudge;
        }

        return VersionCheckResult.UpToDate;
    }

    private static Version ParseVersion(string versionString)
    {
        if (Version.TryParse(versionString, out var version))
        {
            return version;
        }

        return new Version(0, 0, 0);
    }
}
