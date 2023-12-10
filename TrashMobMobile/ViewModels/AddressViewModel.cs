namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class AddressViewModel : ObservableObject
{
    public AddressViewModel()
    {
    }

    [ObservableProperty]
    string streetAddress;

    [ObservableProperty]
    string city;

    [ObservableProperty]
    string region;

    [ObservableProperty]
    string country;

    [ObservableProperty]
    string postalCode;

    [ObservableProperty]
    double? latitude;

    [ObservableProperty]
    double? longitude;

    [ObservableProperty]
    Location location;
}
