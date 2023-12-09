#nullable enable

namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TrashMobMobile.Authentication;
using TrashMobMobile.Data;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly IUserRestService userRestService;
    private readonly IStatsRestService statsRestService;

    public MainViewModel(IAuthService authService, IUserRestService userRestService, IStatsRestService statsRestService)
    {
        this.authService = authService;
        this.userRestService = userRestService;
        this.statsRestService = statsRestService;
    }

    [ObservableProperty]
    StatisticsViewModel statisticsViewModel = new StatisticsViewModel();

    public async Task Init()
    {
        IsBusy = true;

        var signedIn = await authService.SignInSilentAsync(true);

        if (signedIn.Succeeded)
        {
            var email = authService.GetUserEmail();
            var user = await userRestService.GetUserByEmailAsync(email, UserState.UserContext);
            WelcomeMessage = $"Welcome, {user.UserName}!";

            IsBusy = false;
        }
        else
        {
            try
            {
                await Shell.Current.GoToAsync($"{nameof(WelcomePage)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        await RefreshStatistics();
    }

    private async Task RefreshStatistics()
    {
        var stats = await statsRestService.GetStatsAsync();

        StatisticsViewModel.TotalAttendees = stats.TotalParticipants;
        StatisticsViewModel.TotalBags = stats.TotalBags;
        StatisticsViewModel.TotalEvents = stats.TotalEvents;
        StatisticsViewModel.TotalHours = stats.TotalHours;
    }

    [ObservableProperty]
    private string? welcomeMessage;

    ObservableCollection<EventViewModel> UpcomingEvents { get; set; } = new ObservableCollection<EventViewModel>();
}
