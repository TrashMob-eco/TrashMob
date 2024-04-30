namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class SearchLitterReportsViewModel : BaseViewModel
{
    private readonly ILitterReportRestService litterReportRestService;
    private LitterReportViewModel? selectedLitterReport;

    [ObservableProperty]
    private string reportStatus = "New";

    public ObservableCollection<string> CountryCollection { get; set; } = [];

    private string selectedCountry;

    public string SelectedCountry
    {
        get 
        { 
            return selectedCountry;
        }
        set
        {
            selectedCountry = value;
            OnPropertyChanged(nameof(SelectedCountry));

            if (selectedCountry != null)
            {
                RefreshLitterReportsCommand.Execute(null);
            }
        }
    }
    
    public ObservableCollection<string> RegionCollection { get; set; } = [];

    private string selectedRegion;

    public string SelectedRegion
    {
        get
        {
            return selectedRegion;
        }
        set
        {
            selectedRegion = value;
            OnPropertyChanged(nameof(SelectedRegion));

            if (selectedRegion != null)
            {
                RefreshLitterReportsCommand.Execute(null);
            }
        }
    }

    public ObservableCollection<string> CityCollection { get; set; } = [];

    private string selectedCity;

    public string SelectedCity
    {
        get
        {
            return selectedCity;
        }
        set
        {
            selectedCity = value;
            OnPropertyChanged(nameof(SelectedCity));

            if (selectedCity != null)
            {
                RefreshLitterReportsCommand.Execute(null);
            }
        }
    }

    public ICommand RefreshLitterReportsCommand { get; set; }

    public SearchLitterReportsViewModel(ILitterReportRestService litterReportRestService)
    {
        this.litterReportRestService = litterReportRestService;
        RefreshLitterReportsCommand = new Command(async () => await RefreshLitterReports());
    }

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    public LitterReportViewModel SelectedLitterReport
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

        IEnumerable<LitterReport> litterReports;

        if (ReportStatus == "Assigned")
        {
            litterReports = await litterReportRestService.GetAssignedLitterReportsAsync();
        }
        else if (ReportStatus == "New")
        {
            litterReports = await litterReportRestService.GetNewLitterReportsAsync();
        }
        else if (ReportStatus == "Cleaned")
        {
            litterReports = await litterReportRestService.GetCleanedLitterReportsAsync();
        }
        else
        {
            litterReports = await litterReportRestService.GetAllLitterReportsAsync();
        }

        var countryList = new List<string>();
        foreach (var litterReport in litterReports)
        {
            countryList.AddRange(litterReport.LitterImages.Select(i => i.Country));
        }

        CountryCollection.Clear();

        var countries = countryList.Distinct();

        foreach (var country in countries)
        {
            CountryCollection.Add(country);
        }

        if (!string.IsNullOrEmpty(SelectedCountry))
        {
            litterReports = litterReports.Where(l => l.LitterImages.Any(i => i.Country == SelectedCountry));
        }

        var regionList = new List<string>();
        
        foreach (var litterReport in litterReports)
        {
            regionList.AddRange(litterReport.LitterImages.Select(i => i.Region));
        }

        RegionCollection.Clear();

        var regions = regionList.Distinct();

        foreach (var region in regions)
        {
            RegionCollection.Add(region);
        }

        if (!string.IsNullOrEmpty(SelectedRegion))
        {
            litterReports = litterReports.Where(l => l.LitterImages.Any(i => i.Region == SelectedRegion));
        }

        var cityList = new List<string>();

        foreach (var litterReport in litterReports)
        {
            cityList.AddRange(litterReport.LitterImages.Select(i => i.City));
        }

        CityCollection.Clear();

        var cities = cityList.Distinct();

        foreach (var city in cities)
        {
            CityCollection.Add(city);
        }

        if (!string.IsNullOrEmpty(SelectedCity))
        {
            litterReports = litterReports.Where(l => l.LitterImages.Any(i => i.Region == SelectedCity));
        }

        foreach (var litterReport in litterReports)
        {
            var vm = litterReport.ToLitterReportViewModel();
            LitterReports.Add(vm);
        }

        IsBusy = false;

        await Notify("Litter Report list has been refreshed.");
    }
}
