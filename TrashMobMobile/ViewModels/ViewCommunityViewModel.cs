namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ViewCommunityViewModel(
    ICommunityManager communityManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly ICommunityManager communityManager = communityManager;
    private readonly IUserManager userManager = userManager;

    private string slug = string.Empty;

    [ObservableProperty]
    private string communityName = string.Empty;

    [ObservableProperty]
    private string tagline = string.Empty;

    [ObservableProperty]
    private string publicNotes = string.Empty;

    [ObservableProperty]
    private bool hasPublicNotes;

    [ObservableProperty]
    private string location = string.Empty;

    [ObservableProperty]
    private string website = string.Empty;

    [ObservableProperty]
    private bool hasWebsite;

    [ObservableProperty]
    private string contactEmail = string.Empty;

    [ObservableProperty]
    private bool hasContactEmail;

    [ObservableProperty]
    private string contactPhone = string.Empty;

    [ObservableProperty]
    private bool hasContactPhone;

    [ObservableProperty]
    private StatisticsViewModel stats = new();

    [ObservableProperty]
    private bool areEventsFound;

    [ObservableProperty]
    private bool areNoEventsFound = true;

    [ObservableProperty]
    private bool areTeamsFound;

    [ObservableProperty]
    private bool areNoTeamsFound = true;

    public ObservableCollection<EventViewModel> UpcomingEvents { get; } = [];

    public ObservableCollection<TeamViewModel> NearbyTeams { get; } = [];

    public async Task Init(string slug)
    {
        this.slug = slug;

        await ExecuteAsync(async () =>
        {
            await Task.WhenAll(
                RefreshCommunity(),
                RefreshStats(),
                RefreshUpcomingEvents(),
                RefreshNearbyTeams());
        }, "Failed to load community details. Please try again.");
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
    private async Task ViewTeam(TeamViewModel? teamVm)
    {
        if (teamVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewTeamPage)}?TeamId={teamVm.Id}");
    }

    [RelayCommand]
    private async Task OpenWebsite()
    {
        if (!string.IsNullOrEmpty(website))
        {
            await Browser.Default.OpenAsync(website, BrowserLaunchMode.SystemPreferred);
        }
    }

    [RelayCommand]
    private async Task SendEmail()
    {
        if (!string.IsNullOrEmpty(contactEmail))
        {
            await Browser.Default.OpenAsync($"mailto:{contactEmail}", BrowserLaunchMode.SystemPreferred);
        }
    }

    [RelayCommand]
    private async Task CallPhone()
    {
        if (!string.IsNullOrEmpty(contactPhone))
        {
            await Browser.Default.OpenAsync($"tel:{contactPhone}", BrowserLaunchMode.SystemPreferred);
        }
    }

    private async Task RefreshCommunity()
    {
        var community = await communityManager.GetCommunityBySlugAsync(slug);

        CommunityName = community.Name ?? string.Empty;
        Tagline = community.Tagline ?? string.Empty;
        PublicNotes = community.PublicNotes ?? string.Empty;
        HasPublicNotes = !string.IsNullOrWhiteSpace(PublicNotes);

        var locationParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(community.City))
        {
            locationParts.Add(community.City);
        }

        if (!string.IsNullOrWhiteSpace(community.Region))
        {
            locationParts.Add(community.Region);
        }

        if (!string.IsNullOrWhiteSpace(community.Country))
        {
            locationParts.Add(community.Country);
        }

        Location = string.Join(", ", locationParts);

        Website = community.Website ?? string.Empty;
        HasWebsite = !string.IsNullOrWhiteSpace(Website);
        ContactEmail = community.ContactEmail ?? string.Empty;
        HasContactEmail = !string.IsNullOrWhiteSpace(ContactEmail);
        ContactPhone = community.ContactPhone ?? string.Empty;
        HasContactPhone = !string.IsNullOrWhiteSpace(ContactPhone);
    }

    private async Task RefreshStats()
    {
        var statsData = await communityManager.GetCommunityStatsAsync(slug);

        Stats = new StatisticsViewModel
        {
            TotalAttendees = statsData.TotalParticipants,
            TotalEvents = statsData.TotalEvents,
            TotalBags = statsData.TotalBags,
            TotalHours = statsData.TotalHours,
            TotalLitterReportsSubmitted = statsData.TotalLitterReportsSubmitted,
            TotalLitterReportsClosed = statsData.TotalLitterReportsClosed,
        };
    }

    private async Task RefreshUpcomingEvents()
    {
        var events = await communityManager.GetCommunityEventsAsync(slug, true);
        var userId = userManager.CurrentUser.Id;

        UpcomingEvents.Clear();
        foreach (var mobEvent in events.OrderBy(e => e.EventDate).Take(10))
        {
            UpcomingEvents.Add(mobEvent.ToEventViewModel(userId));
        }

        AreEventsFound = UpcomingEvents.Count > 0;
        AreNoEventsFound = !AreEventsFound;
    }

    private async Task RefreshNearbyTeams()
    {
        var teams = await communityManager.GetCommunityTeamsAsync(slug);

        NearbyTeams.Clear();
        foreach (var team in teams.Where(t => t.IsActive).Take(10))
        {
            NearbyTeams.Add(team.ToTeamViewModel());
        }

        AreTeamsFound = NearbyTeams.Count > 0;
        AreNoTeamsFound = !AreTeamsFound;
    }
}
