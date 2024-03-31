namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class CreateLitterImageViewModel : BaseViewModel
{
    [ObservableProperty]
    LitterImageViewModel litterImageViewModel;

    [ObservableProperty]
    LitterReportViewModel litterReportViewModel;

    public CreateLitterImageViewModel(ILitterReportRestService litterReportService, ILitterImageRestService litterImageRestService, IMapRestService mapRestService)
    {
        SaveLitterImageCommand = new Command(async () => await SaveLitterImage());
        this.litterReportService = litterReportService;
        this.litterImageRestService = litterImageRestService;
        this.mapRestService = mapRestService;
    }

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;

        var litterReport = await litterReportService.GetLitterReportAsync(litterReportId);

        LitterReportViewModel = litterReport.ToLitterReportViewModel();

        LitterImageViewModel = new LitterImageViewModel(litterReportService, litterImageRestService)
        {
            Address = new AddressViewModel(),
            Notify = Notify,
            NotifyError = NotifyError,
            Navigation = Navigation,
        };

        await LitterImageViewModel.Init(litterReportId);

        IsBusy = false;
    }

    // This is only for the map point
    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = new ObservableCollection<LitterImageViewModel>();

    public ICommand SaveLitterImageCommand { get; set; }

    private readonly ILitterReportRestService litterReportService;
    private readonly ILitterImageRestService litterImageRestService;
    private readonly IMapRestService mapRestService;

    public string LocalFilePath { get; set; }

    public async Task UpdateLocation()
    {
        Location? location = await GetCurrentLocation();

        if (location != null)
        {
            var address = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);
            LitterImageViewModel.Address.Longitude = location.Longitude;
            LitterImageViewModel.Address.Latitude = location.Latitude;
            LitterImageViewModel.Address.City = address.City;
            LitterImageViewModel.Address.Country = address.Country;
            LitterImageViewModel.Address.County = address.County;
            LitterImageViewModel.Address.PostalCode = address.PostalCode;
            LitterImageViewModel.Address.Region = address.Region;
            LitterImageViewModel.Address.StreetAddress = address.StreetAddress;
            LitterImageViewModel.Address.Location = new Location(LitterImageViewModel.Address.Latitude.Value, LitterImageViewModel.Address.Longitude.Value);

            LitterImages.Clear();
            LitterImages.Add(LitterImageViewModel);
        }
        else
        {
            await NotifyError("Could not get location for image");
        }
    }

    public static async Task<Location?> GetCurrentLocation()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

            var cancelTokenSource = new CancellationTokenSource();

            return await Geolocation.Default.GetLocationAsync(request, cancelTokenSource.Token);
        }
        //catch (FeatureNotSupportedException fnsEx)
        //{
        //    // Handle not supported on device exception
        //}
        //catch (FeatureNotEnabledException fneEx)
        //{
        //    // Handle not enabled on device exception
        //}
        //catch (PermissionException pEx)
        //{
        //    // Handle permission exception
        //}
        catch
        {
            // Unable to get location
        }

        return null;
    }

    private async Task SaveLitterImage()
    {
        IsBusy = true;

        var litterImage = new LitterImage
        {
            City = LitterImageViewModel.Address.City,
            Country = LitterImageViewModel.Address.Country,
            LitterReportId = LitterImageViewModel.LitterReportId,
            Latitude = LitterImageViewModel.Address.Latitude,
            Longitude = LitterImageViewModel.Address.Longitude,
            PostalCode = LitterImageViewModel.Address.PostalCode,
            Region = LitterImageViewModel.Address.Region,
            StreetAddress = LitterImageViewModel.Address.StreetAddress,
        };

        var updatedLitterImage = await litterImageRestService.AddLitterImageAsync(litterImage);
        await litterImageRestService.AddLitterImageFileAsync(updatedLitterImage.LitterReportId, updatedLitterImage.Id, LocalFilePath);

        IsBusy = false;

        await Notify("Litter Image has been saved.");
        await Navigation.PopAsync();
    }
}
