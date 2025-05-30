﻿namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Authentication;
using TrashMobMobile.Services;

public partial class WelcomeViewModel(IAuthService authService, IStatsRestService statsRestService, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IAuthService authService = authService;
    private readonly IStatsRestService statsRestService = statsRestService;

    [ObservableProperty]
    private StatisticsViewModel statisticsViewModel = new();

    public async Task Init()
    {
        IsBusy = true;

        try
        {
            var stats = await statsRestService.GetStatsAsync();

            StatisticsViewModel.TotalAttendees = stats.TotalParticipants;
            StatisticsViewModel.TotalBags = stats.TotalBags;
            StatisticsViewModel.TotalEvents = stats.TotalEvents;
            StatisticsViewModel.TotalHours = stats.TotalHours;
            StatisticsViewModel.TotalLitterReportsSubmitted = stats.TotalLitterReportsSubmitted;
            StatisticsViewModel.TotalLitterReportsClosed = stats.TotalLitterReportsClosed;

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while loading this page. Please try again.");
        }
    }

    [RelayCommand]
    private async Task SignIn()
    {
        IsBusy = true;

        try
        {
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
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while signing you in. Please try again.");
        }
    }
}