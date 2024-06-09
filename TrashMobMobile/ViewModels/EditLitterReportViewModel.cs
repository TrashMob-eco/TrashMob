namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class EditLitterReportViewModel : BaseViewModel
{
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

    private LitterReport litterReport;

    [ObservableProperty]
    private string litterReportStatus;

    private string name = string.Empty;

    [ObservableProperty]
    private bool reportIsValid;

    public EditLitterReportViewModel(ILitterReportManager litterReportManager, IMapRestService mapRestService)
    {
        this.litterReportManager = litterReportManager;
        this.mapRestService = mapRestService;
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

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;

        litterReport = await litterReportManager.GetLitterReportAsync(litterReportId, ImageSizeEnum.Thumb);

        if (litterReport != null)
        {
            Name = litterReport.Name;
            Description = litterReport.Description;

            LitterReportStatus = LitterReportExtensions.GetLitterStatusFromId(litterReport.LitterReportStatusId);
            Name = litterReport.Name;
            Description = litterReport.Description;

            LitterImageViewModels.Clear();
            foreach (var litterImage in litterReport.LitterImages)
            {
                var litterImageViewModel = litterImage.ToLitterImageViewModel();
                LitterImageViewModels.Add(litterImageViewModel);
            }
        }

        IsBusy = false;
    }

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

    public static async Task<Location?> GetCurrentLocation()
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
        catch
        {
            // Unable to get location
        }

        return null;
    }

    [RelayCommand]
    private async Task SaveLitterReport()
    {
        IsBusy = true;

        if (!ReportIsValid)
        {
            IsBusy = false;
            return;
        }

        litterReport.Name = Name;
        litterReport.Description = Description;
        litterReport.LitterImages.Clear();
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
                CreatedByUserId = litterImageViewModel.CreatedByUserId,
                LastUpdatedByUserId = litterImageViewModel.LastUpdatedByUserId,
                CreatedDate = litterImageViewModel.CreatedDate,
                LastUpdatedDate = litterImageViewModel.LastUpdatedDate,

                // Use the Azure Blob Url as local file on create
                AzureBlobURL = litterImageViewModel.FilePath,
            };

            litterReport.LitterImages.Add(litterImage);
        }

        var updatedLitterReport = await litterReportManager.UpdateLitterReportAsync(litterReport);

        IsBusy = false;

        await Notify("Litter Report has been updated.");

        await Navigation.PopAsync();
    }

    public void ValidateReport()
    {
        HasNoImages = !LitterImageViewModels.Any();
        HasMaxImages = LitterImageViewModels.Count >= MaxImages;
        CanAddImages = LitterImageViewModels.Count < MaxImages;
        ReportIsValid = LitterImageViewModels.Count > 0 && Name.Length > 5 && Description.Length > 5;
    }
}