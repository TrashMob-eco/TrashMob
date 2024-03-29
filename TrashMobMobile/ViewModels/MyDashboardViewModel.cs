﻿namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class MyDashboardViewModel : BaseViewModel
{
    private EventViewModel selectedEvent;
    private readonly IMobEventManager mobEventManager;
    private readonly IStatsRestService statsRestService;

    public MyDashboardViewModel(IMobEventManager mobEventManager, IStatsRestService statsRestService)
    {
        this.mobEventManager = mobEventManager;
        this.statsRestService = statsRestService;
    }

    public ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = [];

    public ObservableCollection<EventViewModel> CompletedEvents { get; set; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    [ObservableProperty]
    public StatisticsViewModel statisticsViewModel;

    public EventViewModel SelectedEvent
    {
        get { return selectedEvent; }
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
        IsBusy = true;

        var task1 = RefreshEvents();
        var task2 = RefreshStatistics();

        await Task.WhenAll(task1, task2);
        
        IsBusy = false;
    }

    private async void PerformNavigation(EventViewModel eventViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventViewModel.Id}");
    }

    private async Task RefreshStatistics()
    {

        var stats = await statsRestService.GetUserStatsAsync(App.CurrentUser.Id);

        StatisticsViewModel = new StatisticsViewModel
        {
            TotalBags = stats.TotalBags,
            TotalEvents = stats.TotalEvents,
            TotalHours = stats.TotalHours,
        };
    }

    private async Task RefreshEvents()
    {
        CompletedEvents.Clear();
        UpcomingEvents.Clear();

        var events = await mobEventManager.GetUserEventsAsync(App.CurrentUser.Id, false);

        foreach (var mobEvent in events.OrderByDescending(e => e.EventDate))
        {
            var vm = mobEvent.ToEventViewModel();
            vm.IsUserAttending = true;

            if (mobEvent.IsCompleted())
            {
                vm.CanCancelEvent = false;
                CompletedEvents.Add(vm);
            }
            else
            {
                vm.CanCancelEvent = mobEvent.IsCancellable() && mobEvent.IsEventLead();
                UpcomingEvents.Add(vm);
            }
        }
    }
}
