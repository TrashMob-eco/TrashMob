namespace TrashMobMobileApp.Features.Map
{
    using Maui.GoogleMaps;
    using System;
    using System.Threading.Tasks;
    
    internal class LocationHelper
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        public const double DefaultLatitude = 39.8283;
        public const double DefaultLongitude = 98.5795;
        public const double DefaultLatitudeDegreesSingleEvent = 0.02;
        public const double DefaultLongitudeDegreesSingleEvent = 0.02;
        public const double DefaultLatitudeDegreesMultipleEvents = 1.0;
        public const double DefaultLongitudeDegreesMultipleEvents = 1.0;

        public async Task<Position> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                var location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                var position = new Position(location.Latitude, location.Longitude);

                return position;
            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception)
            {
                // Unable to get location
            }
            finally
            {
                _isCheckingLocation = false;
            }

            return new Position(DefaultLatitude,DefaultLongitude);
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }
    }
}
