namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class EditLitterReportViewModel : BaseViewModel
{
    private readonly IMapRestService mapRestService;
    private readonly ILitterReportManager litterReportManager;
    private const int NewLitterReportStatus = 1;
    public const int MaxImages = 5;

    private string name = string.Empty;
    private string description = string.Empty;

    [ObservableProperty]
    LitterReportViewModel litterReportViewModel;

    [ObservableProperty]
    bool hasNoImages = true;

    [ObservableProperty]
    bool hasMaxImages = false;

    [ObservableProperty]
    bool canAddImages = false;

    [ObservableProperty]
    bool reportIsValid = false;

    public string Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged(nameof(Name));
            ValidateReport();
        }
    }

    public string Description
    {
        get => description;
        set
        {
            description = value;
            OnPropertyChanged(nameof(Description));
            ValidateReport();
        }
    }

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;

        var litterReport = await litterReportManager.GetLitterReportAsync(litterReportId, ImageSizeEnum.Thumb);

        if (litterReport != null)
        {
            LitterReportViewModel = litterReport.ToLitterReportViewModel();

            LitterReportStatus = LitterReportExtensions.GetLitterStatusFromId(LitterReportViewModel?.LitterReportStatusId);
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
    
    [ObservableProperty]
    string litterReportStatus;

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; init; } = [];
    
    public LitterImageViewModel? SelectedLitterImageViewModel { get; set; }

    public EditLitterReportViewModel(ILitterReportManager litterReportManager, IMapRestService mapRestService)
    {
        SaveLitterReportCommand = new Command(async () => await SaveLitterReport());
        this.litterReportManager = litterReportManager;
        this.mapRestService = mapRestService;
        LitterReportViewModel = new LitterReportViewModel
        {
            LitterReportStatusId = NewLitterReportStatus
        };
    }

    public ICommand SaveLitterReportCommand { get; set; }

    public string LocalFilePath { get; set; } = string.Empty;

    public async Task AddImageToCollection()
    {
        Location? location = await GetCurrentLocation();

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
            SelectedLitterImageViewModel.Address.Location = new Location(SelectedLitterImageViewModel.Address.Latitude.Value, SelectedLitterImageViewModel.Address.Longitude.Value);
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

    private async Task SaveLitterReport()
    {
        IsBusy = true;
        
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
                AzureBlobURL = litterImageViewModel.FilePath
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
