namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class SearchLitterReportsViewModel(ILitterReportManager litterReportManager) : BaseViewModel
{
    private readonly ILitterReportManager litterReportManager = litterReportManager;

    private IEnumerable<TrashMob.Models.Poco.Location> locations = [];

    [ObservableProperty]
    private string reportStatus = "New";

    private string? selectedCity;
    private string? selectedCountry;
    private LitterImageViewModel? selectedLitterImage;
    private LitterReportViewModel? selectedLitterReport;
    private string? selectedRegion;

    [ObservableProperty]
    private AddressViewModel userLocation;

    private IEnumerable<LitterReport> RawLitterReports { get; set; } = [];

    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = [];

    public ObservableCollection<string> CountryCollection { get; set; } = [];
    public ObservableCollection<string> RegionCollection { get; set; } = [];
    public ObservableCollection<string> CityCollection { get; set; } = [];

    public string? SelectedCountry
    {
        get => selectedCountry;
        set
        {
            selectedCountry = value;
            OnPropertyChanged();

            HandleCountrySelected(value);
        }
    }

    public string? SelectedRegion
    {
        get => selectedRegion;
        set
        {
            selectedRegion = value;
            OnPropertyChanged();

            HandleRegionSelected(value);
        }
    }

    public string? SelectedCity
    {
        get => selectedCity;
        set
        {
            selectedCity = value;
            OnPropertyChanged();

            HandleCitySelected(value);
        }
    }

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    public LitterReportViewModel? SelectedLitterReport
    {
        get => selectedLitterReport;
        set
        {
            if (selectedLitterReport != value)
            {
                selectedLitterReport = value;
                OnPropertyChanged();

                if (selectedLitterReport != null)
                {
                    PerformNavigation(selectedLitterReport.Id);
                }
            }
        }
    }

    public LitterImageViewModel? SelectedLitterImage
    {
        get => selectedLitterImage;
        set
        {
            if (selectedLitterImage != value)
            {
                selectedLitterImage = value;
                OnPropertyChanged();

                if (selectedLitterImage != null)
                {
                    var litterReport =
                        RawLitterReports.FirstOrDefault(l => l.LitterImages.Any(i => i.Id == selectedLitterImage.Id));
                    PerformNavigation(litterReport.Id);
                }
            }
        }
    }

    public async Task Init()
    {
        IsBusy = true;

        try
        {
            UserLocation = App.CurrentUser.GetAddress();
            await RefreshLitterReports();
            IsBusy = false;
            await Notify("Litter Report list has been refreshed.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("Failed to initialize Litter Report search page.");
        }
    }

    private async void PerformNavigation(Guid litterReportId)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportId}");
    }

    private async Task RefreshLitterReports()
    {
        LitterReports.Clear();

        locations = await litterReportManager.GetLocationsByTimeRangeAsync(DateTimeOffset.Now.AddDays(-180),
            DateTimeOffset.Now);
        CountryCollection.Clear();
        RegionCollection.Clear();
        CityCollection.Clear();

        var countries = locations.Select(l => l.Country).Distinct();

        foreach (var country in countries)
        {
            CountryCollection.Add(country);
        }

        if (ReportStatus == "Assigned")
        {
            RawLitterReports = await litterReportManager.GetAssignedLitterReportsAsync();
        }
        else if (ReportStatus == "New")
        {
            RawLitterReports = await litterReportManager.GetNewLitterReportsAsync();
        }
        else if (ReportStatus == "Cleaned")
        {
            RawLitterReports = await litterReportManager.GetCleanedLitterReportsAsync();
        }
        else
        {
            RawLitterReports = await litterReportManager.GetAllLitterReportsAsync();
        }

        UpdateLitterReportViewModels();
    }

    private void HandleCountrySelected(string? selectedCountry)
    {
        IsBusy = true;

        if (selectedCountry != null)
        {
            RawLitterReports = RawLitterReports.Where(l => l.LitterImages.Any(i => i.Country == SelectedCountry));
        }

        UpdateLitterReportViewModels();

        RefreshRegionList();

        IsBusy = false;
    }

    private void RefreshRegionList()
    {
        RegionCollection.Clear();

        var regions = locations.Where(l => l.Country == selectedCountry).Select(l => l.Region).Distinct();

        foreach (var region in regions)
        {
            RegionCollection.Add(region);
        }
    }

    private void HandleRegionSelected(string? selectedRegion)
    {
        IsBusy = true;

        if (!string.IsNullOrEmpty(selectedRegion))
        {
            RawLitterReports = RawLitterReports.Where(l => l.LitterImages.Any(i => i.Region == selectedRegion));
        }

        UpdateLitterReportViewModels();

        RefreshCityList();

        IsBusy = false;
    }

    private void RefreshCityList()
    {
        CityCollection.Clear();

        var cities = locations.Where(l => l.Country == selectedCountry && l.Region == selectedRegion)
            .Select(l => l.City).Distinct();

        foreach (var city in cities)
        {
            CityCollection.Add(city);
        }
    }

    private void HandleCitySelected(string? selectedCity)
    {
        IsBusy = true;

        if (!string.IsNullOrEmpty(selectedCity))
        {
            RawLitterReports = RawLitterReports.Where(l => l.LitterImages.Any(i => i.City == selectedCity));
        }

        UpdateLitterReportViewModels();

        IsBusy = false;
    }

    private void UpdateLitterReportViewModels()
    {
        LitterReports.Clear();
        LitterImages.Clear();

        foreach (var litterReport in RawLitterReports.OrderByDescending(l => l.CreatedDate))
        {
            var vm = litterReport.ToLitterReportViewModel();

            foreach (var litterImage in litterReport.LitterImages)
            {
                var litterImageViewModel = litterImage.ToLitterImageViewModel();

                if (litterImageViewModel != null)
                {
                    LitterImages.Add(litterImageViewModel);
                }
            }

            LitterReports.Add(vm);
        }
    }

    [RelayCommand]
    private async Task ClearSelections()
    {
        IsBusy = true;

        SelectedCountry = null;
        SelectedRegion = null;
        SelectedCity = null;

        await RefreshLitterReports();

        IsBusy = false;
    }
}