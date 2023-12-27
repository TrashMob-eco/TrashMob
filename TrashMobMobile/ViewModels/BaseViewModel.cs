namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

public abstract partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;

    public INavigation Navigation { get; set; }
}
