namespace TrashMobMobileApp.Features.Pickups;

using TrashMob.Models;
using TrashMobMobileApp.Data;

public partial class AddPickupLocation : ContentPage
{
    private CancellationTokenSource _cancelTokenSource;

    private PickupLocation pickupLocation = new PickupLocation();
    private string localFilePath = string.Empty;
    private IMapRestService mapRestService;

    public AddPickupLocation(IMapRestService mapRestService)
    {
        this.mapRestService = mapRestService;
    }

    public AddPickupLocation()
	{
		InitializeComponent();
	}

    public async void TakePhoto_Clicked(object sender, EventArgs e)
    {
        Location location = await GetCurrentLocation();
        var address = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);
        pickupLocation.Longitude = location.Longitude;
        pickupLocation.Latitude = location.Latitude;
        pickupLocation.City = address.City; 
        pickupLocation.Country = address.Country; 
        pickupLocation.County = address.County; 
        pickupLocation.PostalCode = address.PostalCode;
        pickupLocation.Region = address.Region;
        pickupLocation.StreetAddress = address.StreetAddress;

        SetFields(address);

        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);
            }
        }
    }

    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            return location;
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Handle not supported on device exception
        }
        catch (FeatureNotEnabledException fneEx)
        {
            // Handle not enabled on device exception
        }
        catch (PermissionException pEx)
        {
            // Handle permission exception
        }
        catch (Exception ex)
        {
            // Unable to get location
        }

        return null;
    }

    private void SetFields(Address address)
    {
        streetAddress.Text = address.StreetAddress;
        city.Text = address.City;
        state.Text = address.Region;
        postalCode.Text = address.PostalCode;
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
        
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}