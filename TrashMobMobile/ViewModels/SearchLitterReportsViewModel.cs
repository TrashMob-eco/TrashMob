namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class SearchLitterReportsViewModel : BaseViewModel
{
    private IEnumerable<LitterReport> RawLitterReports { get; set; } = [];
    private string? selectedCountry;
    private string? selectedRegion;
    private string? selectedCity;

    private readonly ILitterReportRestService litterReportRestService;
    private LitterReportViewModel? selectedLitterReport;

    [ObservableProperty]
    private string reportStatus = "New";

    public ObservableCollection<string> CountryCollection { get; set; } = [];
    public ObservableCollection<string> RegionCollection { get; set; } = [];
    public ObservableCollection<string> CityCollection { get; set; } = [];

    public string? SelectedCountry
    {
        get 
        { 
            return selectedCountry;
        }
        set
        {
            selectedCountry = value;
            OnPropertyChanged(nameof(SelectedCountry));

            HandleCountrySelected(value);
        }
    }

    public string? SelectedRegion
    {
        get
        {
            return selectedRegion;
        }
        set
        {
            selectedRegion = value;
            OnPropertyChanged(nameof(SelectedRegion));

            HandleRegionSelected(value);
        }
    }

    public string? SelectedCity
    {
        get
        {
            return selectedCity;
        }
        set
        {
            selectedCity = value;
            OnPropertyChanged(nameof(SelectedCity));

            HandleCitySelected(value);
        }
    }

    public SearchLitterReportsViewModel(ILitterReportRestService litterReportRestService)
    {
        this.litterReportRestService = litterReportRestService;
    }

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    public LitterReportViewModel? SelectedLitterReport
    {
        get { return selectedLitterReport; }
        set
        {
            if (selectedLitterReport != value)
            {
                selectedLitterReport = value;
                OnPropertyChanged(nameof(selectedLitterReport));

                if (selectedLitterReport != null)
                {
                    PerformNavigation(selectedLitterReport);
                }
            }
        }
    }

    public async Task Init()
    {
        await RefreshLitterReports();
    }

    private async void PerformNavigation(LitterReportViewModel litterReportViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportViewModel.Id}");
    }

    private async Task RefreshLitterReports()
    {
        IsBusy = true;

        LitterReports.Clear();

        if (ReportStatus == "Assigned")
        {
            RawLitterReports = await litterReportRestService.GetAssignedLitterReportsAsync();
        }
        else if (ReportStatus == "New")
        {
            RawLitterReports = await litterReportRestService.GetNewLitterReportsAsync();
        }
        else if (ReportStatus == "Cleaned")
        {
            RawLitterReports = await litterReportRestService.GetCleanedLitterReportsAsync();
        }
        else
        {
            RawLitterReports = await litterReportRestService.GetAllLitterReportsAsync();
        }

        var countryList = new List<string>();
        foreach (var litterReport in RawLitterReports)
        {
            countryList.AddRange(litterReport.LitterImages.Select(i => i.Country));
        }

        CountryCollection.Clear();
        RegionCollection.Clear();
        CityCollection.Clear();

        var countries = countryList.Distinct();

        foreach (var country in countries)
        {
            CountryCollection.Add(country);
        }

        UpdateLitterReportViewModels();

        IsBusy = false;

        await Notify("Litter Report list has been refreshed.");
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
        var regionList = new List<string>();

        foreach (var litterReport in RawLitterReports)
        {
            regionList.AddRange(litterReport.LitterImages.Select(i => i.Region));
        }

        RegionCollection.Clear();

        var regions = regionList.Distinct();

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
        var cityList = new List<string>();

        foreach (var litterReport in RawLitterReports)
        {
            cityList.AddRange(litterReport.LitterImages.Select(i => i.City));
        }

        CityCollection.Clear();

        var cities = cityList.Distinct();

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

        foreach (var litterReport in RawLitterReports)
        {
            var vm = litterReport.ToLitterReportViewModel();
            LitterReports.Add(vm);
        }
    }
}
