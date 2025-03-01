﻿namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class SearchLitterReportsViewModel(ILitterReportManager litterReportManager, INotificationService notificationService, IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IUserManager userManager = userManager;
    private IEnumerable<TrashMob.Models.Poco.Location> locations = [];

    [ObservableProperty]
    private string reportStatus = "New";

    private string? selectedCity;
    private string? selectedCountry;
    private LitterImageViewModel? selectedLitterImage;
    private LitterReportViewModel? selectedLitterReport;
    private string? selectedRegion;

    [ObservableProperty]
    private AddressViewModel? userLocation;

    private IEnumerable<LitterReport> RawLitterReports { get; set; } = [];

    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = [];

    public ObservableCollection<string> CountryCollection { get; set; } = [];
    public ObservableCollection<string> RegionCollection { get; set; } = [];
    public ObservableCollection<string> CityCollection { get; set; } = [];

    public ObservableCollection<string> CreatedDateRanges { get; set; } = [];

    [ObservableProperty]
    private bool isMapSelected;

    [ObservableProperty]
    private bool isListSelected;

    [ObservableProperty]
    private bool isNewSelected;

    [ObservableProperty]
    private bool isAssignedSelected;

    [ObservableProperty]
    private bool isCleanedSelected;

    [ObservableProperty]
    private bool areLitterReportsFound;

    [ObservableProperty]
    private bool areNoLitterReportsFound;

    private string selectedCreatedDateRange = DateRanges.LastWeek;

    public string SelectedCreatedDateRange
    {
        get => selectedCreatedDateRange;
        set
        {
            if (value == null)
            {
                return;
            }

            if (selectedCreatedDateRange != value)
            {
                selectedCreatedDateRange = value;
                OnPropertyChanged();
                HandleCreatedDateRangeSelected();
            }
        }
    }

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

                    if (litterReport != null)
                    {
                        PerformNavigation(litterReport.Id);
                    }
                }
            }
        }
    }

    public async Task Init()
    {
        IsBusy = true;

        try
        {
            IsMapSelected = true;
            IsListSelected = false;
            IsNewSelected = true;
            IsAssignedSelected = false;
            IsCleanedSelected = false;

            UserLocation = userManager.CurrentUser.GetAddress();

            foreach (var date in DateRanges.CreatedDateRangeDictionary)
            {
                CreatedDateRanges.Add(date.Key);
            }

            SelectedCreatedDateRange = DateRanges.LastMonth;

            IsBusy = false;
            await NotificationService.Notify("Litter Report list has been refreshed.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("Failed to initialize Litter Report search page.");
        }
    }

    private async void PerformNavigation(Guid litterReportId)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportId}");
    }

    private async Task RefreshLitterReports()
    {
        AreLitterReportsFound = false;
        AreNoLitterReportsFound = true;

        LitterReports.Clear();

        DateTimeOffset startDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CreatedDateRangeDictionary[SelectedCreatedDateRange].Item1);
        DateTimeOffset endDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CreatedDateRangeDictionary[SelectedCreatedDateRange].Item2);
     
        locations = await litterReportManager.GetLocationsByTimeRangeAsync(DateTimeOffset.Now.AddDays(-180),
            DateTimeOffset.Now);
        CountryCollection.Clear();
        RegionCollection.Clear();
        CityCollection.Clear();

        var countries = locations.Select(l => l.Country).Distinct();

        foreach (var country in countries)
        {
            if (!string.IsNullOrEmpty(country))
            {
                CountryCollection.Add(country);
            }
        }

        var litterReportFilter = new LitterReportFilter
        {
            StartDate = startDate,
            EndDate = endDate,
            PageIndex = 0,
            PageSize = 1000,
            IncludeLitterImages = true,
        };

        if (IsAssignedSelected)
        {
            litterReportFilter.LitterReportStatusId = (int)LitterReportStatusEnum.Assigned;
        }
        else if (IsNewSelected)
        {
            litterReportFilter.LitterReportStatusId = (int)LitterReportStatusEnum.New;
        }
        else if (IsCleanedSelected)
        {
            litterReportFilter.LitterReportStatusId = (int)LitterReportStatusEnum.Cleaned;
        }

        RawLitterReports = await litterReportManager.GetLitterReportsAsync(litterReportFilter, ImageSizeEnum.Thumb, true);

        UpdateLitterReportViewModels();
    }

    private async void HandleCreatedDateRangeSelected()
    {
        IsBusy = true;

        await RefreshLitterReports();

        IsBusy = false;
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
            if (!string.IsNullOrEmpty(region))
            {
                RegionCollection.Add(region);
            }
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
            if (!string.IsNullOrEmpty(city))
            {
                CityCollection.Add(city);
            }
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
            var vm = litterReport.ToLitterReportViewModel(NotificationService);

            foreach (var litterImage in litterReport.LitterImages)
            {
                var litterImageViewModel = litterImage.ToLitterImageViewModel(litterReport.LitterReportStatusId, NotificationService);

                if (litterImageViewModel != null)
                {
                    LitterImages.Add(litterImageViewModel);
                }
            }

            LitterReports.Add(vm);
        }

        AreLitterReportsFound = LitterReports.Any();
        AreNoLitterReportsFound = !LitterReports.Any();
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

    [RelayCommand]
    private async Task ViewNew()
    {
        IsBusy = true;

        IsNewSelected = true;
        IsAssignedSelected = false;
        IsCleanedSelected = false;

        await RefreshLitterReports();

        IsBusy = false;
    }

    [RelayCommand]
    private async Task ViewAssigned()
    {
        IsBusy = true;

        IsNewSelected = false;
        IsAssignedSelected = true;
        IsCleanedSelected = false;

        await RefreshLitterReports();

        IsBusy = false;
    }

    [RelayCommand]
    private async Task ViewCleaned()
    {
        IsBusy = true;

        IsNewSelected = false;
        IsAssignedSelected = false;
        IsCleanedSelected = true;

        await RefreshLitterReports();

        IsBusy = false;
    }

    [RelayCommand]
    private Task MapSelected()
    {
        IsMapSelected = true;
        IsListSelected = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ListSelected()
    {
        IsMapSelected = false;
        IsListSelected = true;
        return Task.CompletedTask;
    }
}