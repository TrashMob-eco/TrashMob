namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class BrowseTeamsViewModel(
    ITeamManager teamManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly ITeamManager teamManager = teamManager;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private bool areTeamsFound;

    [ObservableProperty]
    private bool areNoTeamsFound = true;

    public ObservableCollection<TeamViewModel> Teams { get; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            var user = userManager.CurrentUser;
            var address = user.GetAddress();

            var teams = await teamManager.GetPublicTeamsAsync(
                address.Latitude, address.Longitude, 50);

            var myTeams = await teamManager.GetMyTeamsAsync();
            var myTeamIds = new HashSet<Guid>(myTeams.Select(t => t.Id));

            Teams.Clear();
            foreach (var team in teams.Where(t => t.IsActive))
            {
                var memberCount = team.Members?.Count ?? 0;
                var isUserMember = myTeamIds.Contains(team.Id);
                Teams.Add(team.ToTeamViewModel(memberCount, isUserMember));
            }

            AreTeamsFound = Teams.Count > 0;
            AreNoTeamsFound = !AreTeamsFound;
        }, "Failed to load teams. Please try again.");
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await Init();
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
}
