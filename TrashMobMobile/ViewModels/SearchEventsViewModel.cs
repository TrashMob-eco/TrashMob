namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using Sentry;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class SearchEventsViewModel(IMobEventManager mobEventManager,
                                           INotificationService notificationService,
                                           IUserManager userManager)
    : LocationFilterViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IUserManager userManager = userManager;

    private EventViewModel selectedEvent = new();

    [ObservableProperty]
    private AddressViewModel userLocation = new();

    private const int PageSize = 25;
    private int currentPageIndex;

    [ObservableProperty]
    private bool hasMoreResults;

    private IEnumerable<Event> AllEvents { get; set; } = [];
    private IEnumerable<Event> RawEvents { get; set; } = [];

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public ObservableCollection<string> UpcomingDateRanges { get; set; } = [];

    public ObservableCollection<string> CompletedDateRanges { get; set; } = [];

    [ObservableProperty]
    private bool isUpcomingSelected;

    [ObservableProperty]
    private bool isCompletedSelected;

    [ObservableProperty]
    private bool areEventsFound;

    [ObservableProperty]
    private bool areNoEventsFound;

    private string selectedUpcomingDateRange = DateRanges.Today;

    private string selectedCompletedDateRange = DateRanges.Yesterday;

    public string SelectedUpcomingDateRange
    {
        get => selectedUpcomingDateRange;
        set
        {
            if (value == null)
            {
                return;
            }

            if (selectedUpcomingDateRange != value)
            {
                selectedUpcomingDateRange = value;
                OnPropertyChanged();
                HandleUpcomingDateRangeSelected();
            }
        }
    }

    public string SelectedCompletedDateRange
    {
        get => selectedCompletedDateRange;
        set
        {
            if (value == null)
            {
                return;
            }

            if (selectedCompletedDateRange != value)
            {
                selectedCompletedDateRange = value;
                OnPropertyChanged();
                HandleCompletedDateRangeSelected();
            }
        }
    }

    public EventViewModel SelectedEvent
    {
        get => selectedEvent;
        set
        {
            if (selectedEvent != value)
            {
                selectedEvent = value;
                OnPropertyChanged(nameof(selectedEvent));

                if (selectedEvent != null)
                {
                    PerformNavigation(selectedEvent);
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
            IsUpcomingSelected = true;
            IsCompletedSelected = false;
            UserLocation = userManager.CurrentUser.GetAddress();

            foreach (var date in DateRanges.UpcomingRangeDictionary)
            {
                UpcomingDateRanges.Add(date.Key);
            }

            SelectedUpcomingDateRange = DateRanges.ThisMonth;

            foreach (var date in DateRanges.CompletedRangeDictionary)
            {
                CompletedDateRanges.Add(date.Key);
            }

            SelectedCompletedDateRange = DateRanges.LastMonth;

            await NotificationService.Notify("Event list has been refreshed.");
        }, "An error has occurred while loading the events. Please try again in a few moments.");
    }

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        try
        {
            await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    private (DateTimeOffset Start, DateTimeOffset End) GetCurrentDateRange()
    {
        if (IsUpcomingSelected)
        {
            var start = DateTimeOffset.Now.Date.AddDays(DateRanges.UpcomingRangeDictionary[SelectedUpcomingDateRange].Item1);
            var end = DateTimeOffset.Now.Date.AddDays(DateRanges.UpcomingRangeDictionary[SelectedUpcomingDateRange].Item2);
            return (start, end);
        }
        else
        {
            var start = DateTimeOffset.Now.Date.AddDays(DateRanges.CompletedRangeDictionary[SelectedCompletedDateRange].Item1);
            var end = DateTimeOffset.Now.Date.AddDays(DateRanges.CompletedRangeDictionary[SelectedCompletedDateRange].Item2);
            return (start, end);
        }
    }

    private async Task RefreshEvents()
    {
        Events.Clear();
        AreEventsFound = false;
        AreNoEventsFound = true;
        currentPageIndex = 0;
        HasMoreResults = false;

        var (startDate, endDate) = GetCurrentDateRange();

        Locations = await mobEventManager.GetLocationsByTimeRangeAsync(startDate, endDate);
        PopulateCountries();

        if (Locations == null || !Locations.Any())
        {
            return;
        }

        var eventFilter = new EventFilter
        {
            StartDate = startDate,
            EndDate = endDate,
            PageIndex = currentPageIndex,
            PageSize = PageSize,
            EventStatusId = null,
        };

        var events = await mobEventManager.GetFilteredEventsAsync(eventFilter);

        if (IsUpcomingSelected)
        {
            AllEvents = events.Where(e => !e.IsCompleted()).ToList();
        }
        else
        {
            AllEvents = events.Where(e => e.IsCompleted()).ToList();
        }

        RawEvents = AllEvents;
        HasMoreResults = events.Count >= PageSize;

        if (!RawEvents.Any())
        {
            return;
        }

        UpdateEventReportViewModels();
    }

    [RelayCommand]
    private async Task LoadMoreEvents()
    {
        IsBusy = true;

        try
        {
            currentPageIndex++;
            var (startDate, endDate) = GetCurrentDateRange();

            var eventFilter = new EventFilter
            {
                StartDate = startDate,
                EndDate = endDate,
                PageIndex = currentPageIndex,
                PageSize = PageSize,
                EventStatusId = null,
            };

            var events = await mobEventManager.GetFilteredEventsAsync(eventFilter);

            IEnumerable<Event> filtered;
            if (IsUpcomingSelected)
            {
                filtered = events.Where(e => !e.IsCompleted()).ToList();
            }
            else
            {
                filtered = events.Where(e => e.IsCompleted()).ToList();
            }

            AllEvents = AllEvents.Concat(filtered).ToList();
            RawEvents = AllEvents;
            HasMoreResults = events.Count >= PageSize;

            // Re-apply location filters and rebuild
            ApplyFilters();
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

    private async void HandleUpcomingDateRangeSelected()
    {
        try
        {
            IsBusy = true;

            if (IsUpcomingSelected)
            {
                await RefreshEvents();
            }
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

    private async void HandleCompletedDateRangeSelected()
    {
        try
        {
            IsBusy = true;

            if (IsCompletedSelected)
            {
                await RefreshEvents();
            }
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
        IEnumerable<Event> filtered = AllEvents;

        if (!string.IsNullOrEmpty(SelectedCountry))
        {
            filtered = filtered.Where(e => e.Country == SelectedCountry);
        }

        if (!string.IsNullOrEmpty(SelectedRegion))
        {
            filtered = filtered.Where(e => e.Region == SelectedRegion);
        }

        if (!string.IsNullOrEmpty(SelectedCity))
        {
            filtered = filtered.Where(e => e.City == SelectedCity);
        }

        RawEvents = filtered;
        UpdateEventReportViewModels();
    }

    private void UpdateEventReportViewModels()
    {
        Events.Clear();

        foreach (var mobEvent in RawEvents.OrderBy(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);

            Events.Add(vm);
        }

        AreEventsFound = Events.Any();
        AreNoEventsFound = !Events.Any();
    }

    [RelayCommand]
    private async Task ViewUpcoming()
    {
        IsBusy = true;

        IsUpcomingSelected = true;
        IsCompletedSelected = false;

        await RefreshEvents();

        IsBusy = false;
    }

    [RelayCommand]
    private async Task ViewCompleted()
    {
        IsBusy = true;

        IsUpcomingSelected = false;
        IsCompletedSelected = true;

        await RefreshEvents();

        IsBusy = false;
    }

    [RelayCommand]
    private async Task ClearSelections()
    {
        IsBusy = true;

        ClearLocationSelections();

        await RefreshEvents();

        IsBusy = false;
    }
}
