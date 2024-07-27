namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class CreateLitterReportViewModel : BaseViewModel
{
    private const string DefaultLitterReportName = "New Litter Report";
    private const int NewLitterReportStatus = 1;
    public const int MaxImages = 5;
    private readonly ILitterReportManager litterReportManager;
    private readonly IMapRestService mapRestService;

    [ObservableProperty]
    private bool canAddImages;

    private string description = string.Empty;

    [ObservableProperty]
    private bool hasMaxImages;

    [ObservableProperty]
    private bool hasNoImages = true;

    [ObservableProperty]
    private LitterReportViewModel litterReportViewModel;

    private string name = DefaultLitterReportName;

    [ObservableProperty]
    private bool reportIsValid;

    public CreateLitterReportViewModel(ILitterReportManager litterReportManager, IMapRestService mapRestService)
    {
        this.litterReportManager = litterReportManager;
        this.mapRestService = mapRestService;
        LitterReportViewModel = new LitterReportViewModel
        {
            LitterReportStatusId = NewLitterReportStatus,
        };
    }

    public string Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged();
            ValidateReport();
        }
    }

    public string Description
    {
        get => description;
        set
        {
            description = value;
            OnPropertyChanged();
            ValidateReport();
        }
    }

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; init; } = [];

    public LitterImageViewModel? SelectedLitterImageViewModel { get; set; }

    public string LocalFilePath { get; set; } = string.Empty;

    public async Task AddImageToCollection()
    {
        var location = await GetCurrentLocation();

        if (location != null)
        {
            if (SelectedLitterImageViewModel == null)
            {
                SelectedLitterImageViewModel = new LitterImageViewModel();
            }

            var address = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);
            SelectedLitterImageViewModel.Address.Longitude = location.Longitude;
            SelectedLitterImageViewModel.Address.Latitude = location.Latitude;
            SelectedLitterImageViewModel.Address.City = address.City;
            SelectedLitterImageViewModel.Address.Country = address.Country;
            SelectedLitterImageViewModel.Address.County = address.County;
            SelectedLitterImageViewModel.Address.PostalCode = address.PostalCode;
            SelectedLitterImageViewModel.Address.Region = address.Region;
            SelectedLitterImageViewModel.Address.StreetAddress = address.StreetAddress;
            SelectedLitterImageViewModel.Address.Location = new Location(
                SelectedLitterImageViewModel.Address.Latitude.Value,
                SelectedLitterImageViewModel.Address.Longitude.Value);
            SelectedLitterImageViewModel.FilePath = LocalFilePath;

            LitterImageViewModels.Add(SelectedLitterImageViewModel);
            SelectedLitterImageViewModel = null;

            ValidateReport();
        }
        else
        {
            await NotifyError("Could not get location for image");
        }
    }

    public async Task<Location?> GetCurrentLocation()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

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
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await NotifyError($"An error has occured while gettign your location. Please wait and try again in a moment.");
        }

        return null;
    }

    [RelayCommand]
    private async Task SaveLitterReport()
    {
        IsBusy = true;

        try
        {
            if (!ReportIsValid)
            {
                IsBusy = false;
                return;
            }

            var litterReport = LitterReportViewModel.ToLitterReport();
            litterReport.Name = Name;
            litterReport.Description = Description;

            foreach (var litterImageViewModel in LitterImageViewModels)
            {
                var litterImage = new LitterImage
                {
                    Id = Guid.NewGuid(),
                    City = litterImageViewModel.Address.City,
                    Country = litterImageViewModel.Address.Country,
                    LitterReportId = litterImageViewModel.LitterReportId,
                    Latitude = litterImageViewModel.Address.Latitude,
                    Longitude = litterImageViewModel.Address.Longitude,
                    PostalCode = litterImageViewModel.Address.PostalCode,
                    Region = litterImageViewModel.Address.Region,
                    StreetAddress = litterImageViewModel.Address.StreetAddress,

                    // Use the Azure Blob Url as local file on create
                    AzureBlobURL = litterImageViewModel.FilePath,
                };

                litterReport.LitterImages.Add(litterImage);
            }

            await litterReportManager.AddLitterReportAsync(litterReport);

            await Notify("Litter Report has been submitted.");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            await NotifyError($"An error has occured while saving the litter report. Please wait and try again in a moment.");
        }

        IsBusy = false;
    }

    public void ValidateReport()
    {
        HasNoImages = !LitterImageViewModels.Any();
        HasMaxImages = LitterImageViewModels.Count >= MaxImages;
        CanAddImages = LitterImageViewModels.Count < MaxImages;
        ReportIsValid = LitterImageViewModels.Count > 0 && Name.Length > 5 && Description.Length > 5;
    }
}