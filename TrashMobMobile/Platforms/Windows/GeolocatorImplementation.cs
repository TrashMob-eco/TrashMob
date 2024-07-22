namespace TrashMobMobile
{
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Services;

    public class GeolocatorImplementation : IGeolocator
    {
        // Do not need to implement this method for Windows
        public Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
