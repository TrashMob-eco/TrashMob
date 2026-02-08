namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ViewTeamViewModel(
    ITeamManager teamManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly ITeamManager teamManager = teamManager;
    private readonly IUserManager userManager = userManager;

    private Guid teamId;

    [ObservableProperty]
    private TeamViewModel team = new();

    [ObservableProperty]
    private bool isUserMember;

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

        Members.Clear();
        foreach (var member in members.OrderByDescending(m => m.IsTeamLead).ThenBy(m => m.JoinedDate))
        {
            Members.Add(member.ToTeamMemberViewModel());
        }

        AreMembersFound = Members.Count > 0;
        AreNoMembersFound = !AreMembersFound;
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
