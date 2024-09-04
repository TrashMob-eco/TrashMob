namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using TrashMobMobile.Services;

public abstract partial class BaseViewModel(INotificationService notificationService) : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isError;

    public INavigation Navigation { get; set; }

    public INotificationService NotificationService { get; } = notificationService;
}