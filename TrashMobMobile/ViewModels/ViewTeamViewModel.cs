namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ViewTeamViewModel(
    ITeamManager teamManager,
    IMobEventManager mobEventManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly ITeamManager teamManager = teamManager;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IUserManager userManager = userManager;

    private Guid teamId;

    [ObservableProperty]
    private TeamViewModel team = new();

    [ObservableProperty]
    private bool isUserMember;

    [ObservableProperty]
    private bool isTeamLead;

    [ObservableProperty]
    private bool canJoin;

    [ObservableProperty]
    private string joinButtonText = "Join Team";

    [ObservableProperty]
    private bool areMembersFound;

    [ObservableProperty]
    private bool areNoMembersFound = true;

    [ObservableProperty]
    private bool areUpcomingEventsFound;

    [ObservableProperty]
    private bool areNoUpcomingEventsFound = true;

    public ObservableCollection<TeamMemberViewModel> Members { get; } = [];

    public ObservableCollection<EventViewModel> UpcomingEvents { get; } = [];

    public async Task Init(Guid teamId)
    {
        this.teamId = teamId;

        await ExecuteAsync(async () =>
        {
            await Task.WhenAll(
                RefreshTeam(),
                RefreshMembers(),
                RefreshUpcomingEvents());
        }, "Failed to load team details. Please try again.");
    }

    [RelayCommand]
    private async Task JoinTeam()
    {
        await ExecuteAsync(async () =>
        {
            await teamManager.JoinTeamAsync(teamId);
            await NotificationService.Notify("You have joined the team!");

            // Refresh to update UI
            await Task.WhenAll(
                RefreshTeam(),
                RefreshMembers());
        }, "Failed to join team. Please try again.");
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
    private async Task LinkEvent()
    {
        await ExecuteAsync(async () =>
        {
            var userId = userManager.CurrentUser.Id;
            var userEvents = await mobEventManager.GetUserEventsAsync(userId, true);
            var existingEventIds = UpcomingEvents.Select(e => e.Id).ToHashSet();

            var availableEvents = userEvents
                .Where(e => !existingEventIds.Contains(e.Id))
                .OrderBy(e => e.EventDate)
                .ToList();

            if (availableEvents.Count == 0)
            {
                await Shell.Current.DisplayAlert("No Events",
                    "You have no upcoming events available to link to this team.",
                    "OK");
                return;
            }

            var eventNames = availableEvents.Select(e => e.Name).ToArray();
            var selected = await Shell.Current.DisplayActionSheet(
                "Select an event to link", "Cancel", null, eventNames);

            if (string.IsNullOrEmpty(selected) || selected == "Cancel")
            {
                return;
            }

            var selectedEvent = availableEvents.First(e => e.Name == selected);
            await teamManager.LinkEventAsync(teamId, selectedEvent.Id);
            await NotificationService.Notify("Event linked to team!");
            await RefreshUpcomingEvents();
        }, "Failed to link event. Please try again.");
    }

    [RelayCommand]
    private async Task UnlinkEvent(EventViewModel? eventVm)
    {
        if (eventVm == null)
        {
            return;
        }

        var confirm = await Shell.Current.DisplayAlert(
            "Unlink Event",
            $"Remove \"{eventVm.Name}\" from this team?",
            "Unlink",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            await teamManager.UnlinkEventAsync(teamId, eventVm.Id);
            await NotificationService.Notify("Event unlinked from team.");
            await RefreshUpcomingEvents();
        }, "Failed to unlink event. Please try again.");
    }

    private async Task RefreshTeam()
    {
        var teamData = await teamManager.GetTeamAsync(teamId);
        var memberCount = teamData.Members?.Count ?? 0;

        var userId = userManager.CurrentUser.Id;
        var isMember = teamData.Members?.Any(m => m.UserId == userId) ?? false;

        Team = teamData.ToTeamViewModel(memberCount, isMember);
        IsUserMember = isMember;
        CanJoin = !isMember && teamData.IsPublic;
        JoinButtonText = teamData.RequiresApproval ? "Request to Join" : "Join Team";
    }

    private async Task RefreshMembers()
    {
        var members = await teamManager.GetTeamMembersAsync(teamId);
        var userId = userManager.CurrentUser.Id;

        Members.Clear();
        foreach (var member in members.OrderByDescending(m => m.IsTeamLead).ThenBy(m => m.JoinedDate))
        {
            Members.Add(member.ToTeamMemberViewModel());
        }

        AreMembersFound = Members.Count > 0;
        AreNoMembersFound = !AreMembersFound;
        IsTeamLead = members.Any(m => m.UserId == userId && m.IsTeamLead);
    }

    private async Task RefreshUpcomingEvents()
    {
        var events = await teamManager.GetUpcomingTeamEventsAsync(teamId);
        var userId = userManager.CurrentUser.Id;

        UpcomingEvents.Clear();
        foreach (var mobEvent in events.OrderBy(e => e.EventDate).Take(10))
        {
            UpcomingEvents.Add(mobEvent.ToEventViewModel(userId));
        }

        AreUpcomingEventsFound = UpcomingEvents.Count > 0;
        AreNoUpcomingEventsFound = !AreUpcomingEventsFound;
    }
}
