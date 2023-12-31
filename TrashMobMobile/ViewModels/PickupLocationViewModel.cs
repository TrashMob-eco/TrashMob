namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class PickupLocationViewModel : ObservableObject
{
    public PickupLocationViewModel()
    {
    }

    [ObservableProperty]
    Guid id;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string notes;

    [ObservableProperty]
    AddressViewModel address;
}
