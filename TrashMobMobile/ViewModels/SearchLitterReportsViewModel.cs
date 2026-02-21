namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using Sentry;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class SearchLitterReportsViewModel(ILitterReportManager litterReportManager, INotificationService notificationService, IUserManager userManager) : LocationFilterViewModel(notificationService)
{
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private string reportStatus = "New";

    private LitterImageViewModel? selectedLitterImage;
    private LitterReportViewModel? selectedLitterReport;

    [ObservableProperty]
    private AddressViewModel? userLocation;

    private IEnumerable<LitterReport> AllLitterReports { get; set; } = [];
    private IEnumerable<LitterReport> RawLitterReports { get; set; } = [];

    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = [];

    public ObservableCollection<string> CreatedDateRanges { get; set; } = [];

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
        await ExecuteAsync(async () =>
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

            await NotificationService.Notify("Litter Report list has been refreshed.");
        }, "Failed to initialize Litter Report search page.");
    }

    private async void PerformNavigation(Guid litterReportId)
    {
        try
        {
            await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportId}");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    private async Task RefreshLitterReports()
    {
        AreLitterReportsFound = false;
        AreNoLitterReportsFound = true;

        LitterReports.Clear();

        DateTimeOffset startDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CreatedDateRangeDictionary[SelectedCreatedDateRange].Item1);
        DateTimeOffset endDate = DateTimeOffset.Now.Date.AddDays(DateRanges.CreatedDateRangeDictionary[SelectedCreatedDateRange].Item2);

        Locations = await litterReportManager.GetLocationsByTimeRangeAsync(DateTimeOffset.Now.AddDays(-180),
            DateTimeOffset.Now);
        PopulateCountries();

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

        AllLitterReports = (await litterReportManager.GetLitterReportsAsync(litterReportFilter, ImageSizeEnum.Thumb, true)).ToList();
        RawLitterReports = AllLitterReports;

        UpdateLitterReportViewModels();
    }

    private async void HandleCreatedDateRangeSelected()
    {
        try
        {
            IsBusy = true;

            await RefreshLitterReports();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected override void ApplyFilters()
    {
        IEnumerable<LitterReport> filtered = AllLitterReports;

        if (!string.IsNullOrEmpty(SelectedCountry))
        {
            filtered = filtered.Where(l => l.LitterImages.Any(i => i.Country == SelectedCountry));
        }

        if (!string.IsNullOrEmpty(SelectedRegion))
        {
            filtered = filtered.Where(l => l.LitterImages.Any(i => i.Region == SelectedRegion));
        }

        if (!string.IsNullOrEmpty(SelectedCity))
        {
            filtered = filtered.Where(l => l.LitterImages.Any(i => i.City == SelectedCity));
        }

        RawLitterReports = filtered;
        UpdateLitterReportViewModels();
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

        ClearLocationSelections();

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
}
