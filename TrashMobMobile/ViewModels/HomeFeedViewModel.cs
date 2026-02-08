namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class HomeFeedViewModel(
    IMobEventManager mobEventManager,
    ILitterReportManager litterReportManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    [ObservableProperty]
    private bool areEventsFound;

    [ObservableProperty]
    private bool areNoEventsFound = true;

    [ObservableProperty]
    private bool areLitterReportsFound;

    [ObservableProperty]
    private bool areNoLitterReportsFound = true;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; } = [];

    public ObservableCollection<LitterReportViewModel> NearbyLitterReports { get; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            var user = userManager.CurrentUser;
            WelcomeMessage = $"Welcome, {user.UserName}!";

            await Task.WhenAll(
                RefreshUpcomingEvents(),
                RefreshNearbyLitterReports());
        }, "Failed to load feed. Please try again.");
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await Init();
    }

    [RelayCommand]
    private async Task ViewEvent(EventViewModel? eventVm)
    {
        if (eventVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewEventPage)}?EventId={eventVm.Id}");
    }

    [RelayCommand]
    private async Task BrowseTeams()
    {
        await Shell.Current.GoToAsync(nameof(BrowseTeamsPage));
    }

    [RelayCommand]
    private async Task ViewLitterReport(LitterReportViewModel? reportVm)
    {
        if (reportVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewLitterReportPage)}?LitterReportId={reportVm.Id}");
    }

    private async Task RefreshUpcomingEvents()
    {
        var filter = new EventFilter
        {
            StartDate = DateTimeOffset.UtcNow,
            EndDate = DateTimeOffset.UtcNow.AddMonths(3),
            PageIndex = 0,
            PageSize = 20,
        };

        var events = await mobEventManager.GetFilteredEventsAsync(filter);

        UpcomingEvents.Clear();
        foreach (var mobEvent in events.Where(e => !e.IsCompleted()).OrderBy(e => e.EventDate).Take(10))
        {
            UpcomingEvents.Add(mobEvent.ToEventViewModel(userManager.CurrentUser.Id));
        }

        AreEventsFound = UpcomingEvents.Count > 0;
        AreNoEventsFound = !AreEventsFound;
    }

    private async Task RefreshNearbyLitterReports()
    {
        var filter = new LitterReportFilter
        {
            StartDate = DateTimeOffset.UtcNow.AddMonths(-3),
            EndDate = DateTimeOffset.UtcNow,
        };

        var reports = await litterReportManager.GetLitterReportsAsync(filter, ImageSizeEnum.Thumb, true);

        NearbyLitterReports.Clear();
        foreach (var report in reports.OrderByDescending(r => r.CreatedDate).Take(5))
        {
            NearbyLitterReports.Add(report.ToLitterReportViewModel(NotificationService));
        }

        AreLitterReportsFound = NearbyLitterReports.Count > 0;
        AreNoLitterReportsFound = !AreLitterReportsFound;
    }
}
