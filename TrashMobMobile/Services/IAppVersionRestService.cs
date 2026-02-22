namespace TrashMobMobile.Services;

using TrashMob.Models.Poco;

public interface IAppVersionRestService
{
    Task<AppVersionInfo?> GetAppVersionAsync(CancellationToken cancellationToken = default);
}
