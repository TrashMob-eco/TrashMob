namespace TrashMobMobile.Services;

public enum VersionCheckResult
{
    UpToDate,
    SoftNudge,
    HardBlock,
    Skipped,
}

public interface IAppVersionCheckService
{
    Task<VersionCheckResult> CheckVersionAsync(CancellationToken cancellationToken = default);
}
