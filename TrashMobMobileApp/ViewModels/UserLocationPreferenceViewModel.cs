namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class UserLocationPreferenceViewModel : ObservableObject
{
    public UserLocationPreferenceViewModel()
    {
    }

    [ObservableProperty]
    AddressViewModel addressViewModel;

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
