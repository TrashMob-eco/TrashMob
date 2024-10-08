namespace TrashMobMobile.Services
{
    public interface IWaiverManager
    {
        public Task<bool> HasUserSignedTrashMobWaiverAsync(CancellationToken cancellationToken = default);
    }
}