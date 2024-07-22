namespace TrashMobMobile.Services
{
    using System;
    using System.Threading.Tasks;

    public interface IGeolocator
    {
        Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken);
    }
}
