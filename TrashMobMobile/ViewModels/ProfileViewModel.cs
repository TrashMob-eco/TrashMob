namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ProfileViewModel(
    IMobEventManager mobEventManager,
    ILitterReportManager litterReportManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private string userEmail = string.Empty;

    [ObservableProperty]
    private string memberSince = string.Empty;

    [ObservableProperty]
    private string locationSummary = string.Empty;

    [ObservableProperty]
    private bool showUpcoming = true;

    [ObservableProperty]
    private bool showCompleted;

    [ObservableProperty]
    private bool areUpcomingEventsFound;

    [ObservableProperty]
    private bool areNoUpcomingEventsFound = true;

    [ObservableProperty]
    private bool areCompletedEventsFound;

    [ObservableProperty]
    private bool areNoCompletedEventsFound = true;

    [ObservableProperty]
    private bool areLitterReportsFound;

    [ObservableProperty]
    private bool areNoLitterReportsFound = true;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; } = [];

    public ObservableCollection<EventViewModel> CompletedEvents { get; } = [];

    public ObservableCollection<LitterReportViewModel> LitterReports { get; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            var user = userManager.CurrentUser;
            UserName = user.UserName ?? string.Empty;
            UserEmail = user.Email ?? string.Empty;
            MemberSince = user.MemberSince.HasValue
                ? $"Member since {user.MemberSince.Value:MMMM yyyy}"
                : string.Empty;

            var locationParts = new List<string>();
            if (!string.IsNullOrEmpty(user.City))
            {
                locationParts.Add(user.City);
            }

            if (!string.IsNullOrEmpty(user.Region))
            {
                locationParts.Add(user.Region);
            }

            if (!string.IsNullOrEmpty(user.Country))
            {
                locationParts.Add(user.Country);
            }

            LocationSummary = locationParts.Count > 0
                ? string.Join(", ", locationParts)
                : "No location set";

            await Task.WhenAll(
                RefreshUpcomingEvents(),
                RefreshCompletedEvents(),
                RefreshLitterReports());
        }, "Failed to load profile. Please try again.");
    }

    [RelayCommand]
    private void ShowUpcomingEvents()
    {
        ShowUpcoming = true;
        ShowCompleted = false;
    }

    [RelayCommand]
    private void ShowCompletedEvents()
    {
        ShowUpcoming = false;
        ShowCompleted = true;
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
    private async Task ViewLitterReport(LitterReportViewModel? reportVm)
    {
        if (reportVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewLitterReportPage)}?LitterReportId={reportVm.Id}");
    }

    [RelayCommand]
    private async Task EditLocation()
    {
        await Shell.Current.GoToAsync(nameof(SetUserLocationPreferencePage));
    }

    [RelayCommand]
    private async Task OpenPrivacyPolicy()
    {
        await Browser.Default.OpenAsync("https://www.trashmob.eco/privacypolicy", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task OpenTermsOfService()
    {
        await Browser.Default.OpenAsync("https://www.trashmob.eco/termsofservice", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task SignWaiver()
    {
        await Shell.Current.GoToAsync(nameof(WaiverPage));
    }

    [RelayCommand]
    private async Task ContactUs()
    {
        await Shell.Current.GoToAsync(nameof(ContactUsPage));
    }

    [RelayCommand]
    private async Task SignOut()
    {
        await Shell.Current.GoToAsync(nameof(LogoutPage));
    }

    private async Task RefreshUpcomingEvents()
    {
        var filter = new EventFilter
        {
            StartDate = DateTimeOffset.UtcNow,
            EndDate = DateTimeOffset.UtcNow.AddYears(1),
        };

        var events = await mobEventManager.GetUserEventsAsync(filter, userManager.CurrentUser.Id);

        UpcomingEvents.Clear();
        foreach (var e in events)
        {
            UpcomingEvents.Add(e.ToEventViewModel(userManager.CurrentUser.Id));
        }

        AreUpcomingEventsFound = UpcomingEvents.Count > 0;
        AreNoUpcomingEventsFound = !AreUpcomingEventsFound;
    }

    private async Task RefreshCompletedEvents()
    {
        var filter = new EventFilter
        {
            StartDate = DateTimeOffset.UtcNow.AddYears(-1),
            EndDate = DateTimeOffset.UtcNow,
        };

        var events = await mobEventManager.GetUserEventsAsync(filter, userManager.CurrentUser.Id);

        CompletedEvents.Clear();
        foreach (var e in events)
        {
            CompletedEvents.Add(e.ToEventViewModel(userManager.CurrentUser.Id));
        }

        AreCompletedEventsFound = CompletedEvents.Count > 0;
        AreNoCompletedEventsFound = !AreCompletedEventsFound;
    }

    private async Task RefreshLitterReports()
    {
        var filter = new LitterReportFilter
        {
            StartDate = DateTimeOffset.UtcNow.AddYears(-1),
            EndDate = DateTimeOffset.UtcNow,
        };

        var reports = await litterReportManager.GetLitterReportsAsync(filter, ImageSizeEnum.Thumb, true);

        LitterReports.Clear();
        foreach (var r in reports)
        {
            LitterReports.Add(r.ToLitterReportViewModel(NotificationService));
        }

        AreLitterReportsFound = LitterReports.Count > 0;
        AreNoLitterReportsFound = !AreLitterReportsFound;
    }
}
