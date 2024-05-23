namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Authentication;
using TrashMobMobile.Data;

public partial class WelcomeViewModel : BaseViewModel
{
    private readonly IAuthService authService;
    private readonly IStatsRestService statsRestService;

    [ObservableProperty]
    private StatisticsViewModel statisticsViewModel = new();

    public WelcomeViewModel(IAuthService authService, IStatsRestService statsRestService)
    {
        this.authService = authService;
        this.statsRestService = statsRestService;
    }

    public async Task Init()
    {
        IsBusy = true;

        var stats = await statsRestService.GetStatsAsync();

        StatisticsViewModel.TotalAttendees = stats.TotalParticipants;
        StatisticsViewModel.TotalBags = stats.TotalBags;
        StatisticsViewModel.TotalEvents = stats.TotalEvents;
        StatisticsViewModel.TotalHours = stats.TotalHours;

        IsBusy = false;
    }

    [RelayCommand]
    private async Task SignIn()
    {
        IsBusy = true;

        var signedIn = await authService.SignInAsync();

        IsBusy = false;

        if (signedIn.Succeeded)
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            IsError = true;
        }
    }
}