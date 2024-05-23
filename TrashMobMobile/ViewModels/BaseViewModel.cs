namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isError;

    public INavigation Navigation { get; set; }

    public Func<string, Task> Notify { get; set; }

    public Func<string, Task> NotifyError { get; set; }
}