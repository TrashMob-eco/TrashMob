namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class UserLocationPreferenceViewModel : BaseViewModel
{
    public UserLocationPreferenceViewModel()
    {
    }

    [ObservableProperty]
    AddressViewModel addressViewModel;
}
